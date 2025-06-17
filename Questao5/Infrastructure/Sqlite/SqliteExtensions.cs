using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Enumerators;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using static Dapper.SqlMapper;

namespace Questao5.Infrastructure.Sqlite
{
    public static class SqliteExtensions
    {
        public static async Task<TEntity> BuscarUnicoAsync<TEntity>(this SqliteConnection connection,
            Expression<Func<TEntity, object>> keys, params object[] keyValues)
            where TEntity : class
        {
            var (entityName, _, whereColumns, parameters) = await ProcessCoreAsync(keys, keyValues);

            const string sqlTemplate = "SELECT * FROM {0} T1 WHERE {1} LIMIT 1;";

            string wherePiece = string.Join(" AND ", whereColumns);

            string sql = string.Format(sqlTemplate, entityName, wherePiece);

            SqliteHandlers.ApplyTypeHandlers();

            var result = await connection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters);

            return result!;
        }
        public static async Task<TEntity> BuscarUnicoAsync<TEntity, TInclude>(
            this SqliteConnection connection,
            Expression<Func<TEntity, object>> keys,
            Expression<Func<TEntity, TInclude, object>> relationOn = null!,
            params object[] keyValues
            )
            where TEntity : class
            where TInclude : class
        {
            var (entityName, includeName, keyInfos, joinPiece, whereColumns, parameters) =
                await ProcessCoreAsync(keys, relationOn, keyValues);

            var selectColsLeft = MappingUtils.FetchColumns<TEntity>(typeof(TInclude));

            var selectLeft = string.Join(", ", selectColsLeft.Select(x => string.Concat("T1.", x)));

            var selectColsRight = MappingUtils.FetchColumns<TInclude>(typeof(TEntity));

            var splitColumn = selectColsRight.FirstOrDefault();

            var selectRight = string.Join(", ", selectColsRight.Select(x => string.Concat("T2.", x)));

            const string sqlTemplate = "SELECT {0} FROM {1} T1 INNER JOIN {2} T2 ON {3} WHERE {4};";

            string wherePiece = string.Join(" AND ", whereColumns);

            string query = string.Format(sqlTemplate, string.Concat(selectLeft, ", ", selectRight),
                entityName, includeName, joinPiece, wherePiece);

            var pkInfo = keyInfos.FirstOrDefault().KeyInfo;

            var (IsValid, navigationInfo, isList) = MappingUtils.IsValidForNavigation<TEntity, TInclude>();

            var pinpoint = new Dictionary<object, TEntity>();

            SqliteHandlers.ApplyTypeHandlers();

            var result = await connection.QueryAsync<TEntity, TInclude, TEntity>(query, (entity1, entity2) =>
             {
                 if (!pinpoint.TryGetValue(pkInfo?.GetValue(entity1)!, out TEntity? entry))
                 {
                     entry = entity1;

                     pinpoint.Add(pkInfo?.GetValue(entity1)!, entry);
                 }

                 if (isList!.Value)
                 {
                     ((List<TInclude>)navigationInfo.GetValue(entry)!).Add(entity2);
                 }
                 else
                 {
                     navigationInfo?.SetValue(entry, entity2);
                 }

                 return entry;

             }, splitOn: splitColumn!, param: parameters);

            return result.FirstOrDefault()!;
        }
        public static async Task<bool> InserirAsync<TEntity, TInclude>(
            this SqliteConnection connection,
            TEntity item,
            Expression<Func<TEntity, TInclude, object>> relation)
            where TEntity : class
            where TInclude : class
        {
            var parameters = new DynamicParameters();

            var (relationInfos, IsValidRelation) = MappingUtils.FetchRelationInfo(relation);

            var (isValidNavigation, navigationInfo, isList) = MappingUtils.IsValidForNavigation<TEntity, TInclude>();

            if (!IsValidRelation || !isValidNavigation)
            {
                throw new ArgumentException("Os parâmetros fornecidos para o relacionamento na expressão são inválidos.");
            }

            var navigation = navigationInfo.GetValue(item)! ?? 
                throw new ArgumentException("O valor fornecido para a propriedade de navegação é nulo.");
            
            var leftInfos = MappingUtils.FetchInfoForColumns<TEntity>(typeof(TInclude));
            var rightInfos = MappingUtils.FetchInfoForColumns<TInclude>(typeof(TEntity));

            if (!isList!.Value)
            {
                foreach (var (_, _, LeftInfo, RightInfo) in relationInfos)
                {
                    var leftValue = LeftInfo.GetValue(item);
                    var rightValue = RightInfo.GetValue(navigation);

                    if (!leftValue!.Equals(rightValue))
                    {
                        LeftInfo.SetValue(item, rightValue);                        
                    }
                }
            }

            foreach (var prop in leftInfos)
            {
                parameters.Add($"@{prop.Name}", prop.GetValue(item));
            }

            const string insertTemplate = "INSERT INTO {0}({1}) VALUES ({2});";

            string insertSql = string.Format(insertTemplate,
                typeof(TEntity).Name,
                string.Join(", ", leftInfos.Select(p => p.Name)),
                string.Join(", ", parameters.ParameterNames.Select(x => string.Concat("@", x))));

            SqliteHandlers.ApplyTypeHandlers();

            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            var created = await connection.ExecuteAsync(insertSql, parameters) > 0;

            if (isList!.Value)
            {
                var navigationItems = (List<TInclude>)navigation;

                if (navigationItems is not null)
                {
                    foreach (var navigationItem in navigationItems)
                    {
                        foreach (var (_, _, LeftInfo, RightInfo) in relationInfos)
                        {
                            var leftValue = LeftInfo.GetValue(item);
                            var rightValue = RightInfo.GetValue(navigationItem);

                            if (!leftValue!.Equals(rightValue))
                            {
                                RightInfo.SetValue(navigationItem, leftValue);                                
                            }
                        }

                        parameters = new DynamicParameters();

                        foreach (var itemInfo in rightInfos)
                        {
                            parameters.Add($"@{itemInfo.Name}", itemInfo.GetValue(navigationItem));
                        }

                        insertSql = string.Format(insertTemplate,
                            typeof(TInclude).Name,
                            string.Join(", ", rightInfos.Select(p => p.Name)),
                            string.Join(", ", parameters.ParameterNames.Select(x => string.Concat("@", x))));

                        var inserted = await connection.ExecuteAsync(insertSql, parameters) > 0;

                        if (!inserted)
                        {
                            throw new ArgumentException($"Operação de inserção falhou: {insertSql}.");
                        }
                    }
                }
                else
                {
                    throw new ArgumentException($"Operação de inserção falhou: A propriedade de navegação é nula.");
                }
            }

            if (created)
            {
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }

            if (connection.State != ConnectionState.Closed)
                await connection.CloseAsync();

            return created;
        }
        public static async Task<bool> InserirAsync<TEntity>(this SqliteConnection connection, TEntity item)
            where TEntity : class
        {
            var parameters = new DynamicParameters();

            var entityInfos = MappingUtils.FetchInfoForColumns<TEntity>(typeof(TEntity));

            foreach (var prop in entityInfos)
            {
                parameters.Add($"@{prop.Name}", prop.GetValue(item));
            }

            const string insertTemplate = "INSERT INTO {0}({1}) VALUES ({2});";

            string insertSql = string.Format(insertTemplate,
                typeof(TEntity).Name,
                string.Join(", ", entityInfos.Select(p => p.Name)),
                string.Join(", ", parameters.ParameterNames.Select(x => string.Concat("@", x))));

            SqliteHandlers.ApplyTypeHandlers();

            var result = await connection.ExecuteAsync(insertSql, parameters) > 0;

            return result;
        }
        public static async Task<bool> EditarAsync<TEntity>(this SqliteConnection connection,
            Expression<Func<TEntity, object>> keys,
            TEntity item)
            where TEntity : class
        {
            var (keyInfos, isValid) =  MappingUtils.FetchKeysInfo(keys);

            var keyValue = keyInfos.FirstOrDefault().PropInfo.GetValue(item);

            var (entityName, _, whereColumns, parameters) = await ProcessCoreAsync(keys, keyValue!);

            string wherePiece = string.Join(" AND ", whereColumns).Replace("T1.", string.Empty);

            var entityInfos = MappingUtils.FetchInfoForColumns<TEntity>(typeof(TEntity));

            foreach (var prop in entityInfos)
            {
                parameters.Add($"@{prop.Name}", prop.GetValue(item));
            }

            const string updateTemplate = "UPDATE {0} SET {1} WHERE {2};";

            string updateSql = string.Format(updateTemplate,
                typeof(TEntity).Name,
                string.Join(", ", entityInfos.Select(p => string.Concat(p.Name," = @", p.Name))),
                wherePiece);

            SqliteHandlers.ApplyTypeHandlers();

            var result = await connection.ExecuteAsync(updateSql, parameters) > 0;

            return result;
        }
        public static async Task<bool> ExcluirAsync<TEntity>(this SqliteConnection connection,
            Expression<Func<TEntity, object>> keys,
            params object[] keyValues)
            where TEntity : class
        {
            var (entityName, _, whereColumns, parameters) = await ProcessCoreAsync(keys, keyValues);

            string wherePiece = string.Join(" AND ", whereColumns).Replace("T1.", string.Empty);

            const string deleteTemplate = "DELETE FROM {0} WHERE {1};";

            string deleteSql = string.Format(deleteTemplate, entityName, wherePiece);

            SqliteHandlers.ApplyTypeHandlers();

            var result = await connection.ExecuteAsync(deleteSql, parameters) > 0;

            return result;
        }
        public static async Task<bool> ExcluirExpiradosCacheAsync<TCacheEntity>(this SqliteConnection connection)
            where TCacheEntity : class
        {
            const string deleteTemplate = "DELETE FROM {0} WHERE Expiracao < @Data;";

            string deleteSql = string.Format(deleteTemplate, typeof(TCacheEntity).Name);

            SqliteHandlers.ApplyTypeHandlers();

            var result = await connection.ExecuteAsync(deleteSql, new { Data = DateTime.Now }) > 0;

            return result;
        }

        public static async Task<double> BuscarSaldoAsync<TEntity>(this SqliteConnection connection,
            Expression<Func<TEntity, object>> keys,
            params object[] contaId)
            where TEntity : class
        {
            var (_, _, whereColumns, parameters) = await ProcessCoreAsync(keys, contaId);

            string wherePiece = string.Join(" AND ", whereColumns);

            const string queryTemplate =
                @"SELECT T.TipoMovimento Tipo, SUM(T.Valor) Soma
                FROM Movimento T INNER JOIN ContaCorrente T1 ON
                T.IdContaCorrente = T1.Numero WHERE {0} GROUP BY 
                T.TipoMovimento;";

            var saldoSql = string.Format(queryTemplate, wherePiece);

            SqliteHandlers.ApplyTypeHandlers();

            var result = await connection.QueryAsync<dynamic>(saldoSql, parameters);

            double saldo = 0;

            foreach (var item in result)
            {
                if (item.Tipo.Contains('C'))
                    saldo += item.Soma;
                else if (item.Tipo.Contains('D'))
                    saldo -= item.Soma;
            }
            
            return Math.Round(saldo, 2);
        }
        private static Task<(string EntityName, List<(string Name, PropertyInfo KeyInfo)> KeysInfo,
            List<string> WhereColumns, DynamicParameters Parameters)>
            ProcessCoreAsync<TEntity>(Expression<Func<TEntity, object>> keys, params object[] keyValues)
            where TEntity : class
        {
            var entityName = MappingUtils.FetchEntityName<TEntity>();

            var (keyResults, isValid) = MappingUtils.FetchKeysInfo(keys);

            if (!isValid)
            {
                throw new ArgumentException("Os parâmetros de expressão fornecidos para a chave são inválidos.");
            }

            if (keyResults.Count != keyValues.Length)
            {
                throw new ArgumentException("Os valores fornecidos para a chave não correspondem aos parâmetros da expressão.");
            }

            DynamicParameters parameters = new();

            List<string> whereColumns = [];

            for (int i = 0; i < keyResults.Count; i++)
            {
                var keyName = keyResults[i].Name;
                if (!keyResults[i].PropInfo.PropertyType.IsAssignableTo(keyValues[i].GetType()))
                {
                    throw new ArgumentException($"Os valores fornecidos para a chave '{keyName}' são de um tipo incorreto.");
                }

                whereColumns.Add($"T1.{keyName} = @{keyName}");
                parameters.Add($"@{keyName}", keyValues[i]);
            }

            return Task.FromResult((entityName, keyResults, whereColumns, parameters));
        }
        private static async Task<(string EntityName, string IncludeName, List<(string Name, PropertyInfo KeyInfo)> KeysInfo, string JoinPiece, List<string> WhereColumns, DynamicParameters Parameters)>
            ProcessCoreAsync<TEntity, TInclude>(
            Expression<Func<TEntity, object>> keys,
            Expression<Func<TEntity, TInclude, object>> relation,
            params object[] keyValues)
            where TEntity : class
            where TInclude : class
        {
            var (entityName, keyInfos, whereColumns, parameters) = await ProcessCoreAsync(keys, keyValues);

            var includeName = MappingUtils.FetchEntityName<TInclude>();

            var (columnInfos, isValid) = MappingUtils.FetchRelationInfo(relation);

            if (!isValid)
            {
                throw new ArgumentException("Os valores fornecidos para o relacionamento na expressão são inválidos.");
            }

            List<string> joinPieces = [];

            foreach (var (LeftColumn, RightColumn, _, _) in columnInfos)
            {
                joinPieces.Add($"T1.{LeftColumn} = T2.{RightColumn}");
            }

            string joinPiece = string.Join(" AND ", joinPieces);

            return (entityName, includeName, keyInfos, joinPiece, whereColumns, parameters);
        }
    }
    public static class MappingUtils
    {
        public static string FetchEntityName<TEntity>() where TEntity : class => typeof(TEntity).Name;

        public static (List<(string LeftColumn, string RightColumn,
                    PropertyInfo LeftInfo, PropertyInfo RightInfo)> ColumnInfos, bool IsValid)
            FetchRelationInfo<TEntity, TInclude>(Expression<Func<TEntity, TInclude, object>> exp)
            where TEntity : class
            where TInclude : class
        {
            List<(string LeftColumn, string RightColumn,
                    PropertyInfo LeftInfo, PropertyInfo RightInfo)> results = [];

            if (exp.Body is UnaryExpression)
            {
                var unary = exp.Body as UnaryExpression;

                if (unary?.Operand is not MethodCallExpression)
                {
                    return (default!, false);
                }

                var call = unary?.Operand as MethodCallExpression;

                if (!(call?.Method.Name == "Equals") ||
                    call.Object is not NewExpression ||
                    call.Arguments[0] is not NewExpression)
                {
                    return (default!, false);
                }

                var left = call.Object as NewExpression;
                var right = call.Arguments[0] as NewExpression;

                var p1 = exp.Parameters[0];
                var p2 = exp.Parameters[1];

                bool[] conditions = [
                    left!.Arguments.Any(p => p is not MemberExpression),
                    right!.Arguments.Any(p => p is not MemberExpression),
                    left.Arguments.Count == 0,
                    right.Arguments.Count == 0,
                    left.Arguments.Count != right.Arguments.Count
                ];

                if (conditions.Any(x => x) ||
                    left.Arguments.Cast<MemberExpression>().Any(l =>
                    (l.Expression as ParameterExpression)?.Name != p1.Name) ||
                    right.Arguments.Cast<MemberExpression>().Any(r =>
                    (r.Expression as ParameterExpression)?.Name != p2.Name)
                    )
                {
                    return (default!, false);
                }

                var leftArgs = left.Arguments.Cast<MemberExpression>().ToList();
                var rightArgs = right.Arguments.Cast<MemberExpression>().ToList();

                var leftColumns = FetchColumns<TEntity>(typeof(TInclude)).ToList();
                var rightColumns = FetchColumns<TInclude>(typeof(TEntity)).ToList();

                if (leftArgs.Select(x => leftColumns.Any(c => c == x.Member.Name)).Any(x => !x) ||
                    rightArgs.Select(x => rightColumns.Any(c => c == x.Member.Name)).Any(x => !x))
                {
                    return (default!, false);
                }

                for (int i = 0; i < leftArgs.Count && i < rightArgs.Count; i++)
                {
                    var itemLeft = leftArgs[i];
                    var itemRight = rightArgs[i];

                    (bool leftIsValid, PropertyInfo leftInfo) = IsValidForColumn<TEntity>(itemLeft.Member.Name, typeof(TInclude));
                    (bool rightIsValid, PropertyInfo rightInfo) = IsValidForColumn<TInclude>(itemRight.Member.Name, typeof(TEntity));

                    if (results.Any(x => x.LeftColumn == itemLeft.Member.Name && x.RightColumn == itemRight.Member.Name))
                        return (null!, false);

                    if (leftIsValid && rightIsValid && leftInfo.PropertyType.IsAssignableTo(rightInfo.PropertyType))
                    {
                        results.Add((itemLeft.Member.Name, itemRight.Member.Name, leftInfo, rightInfo));
                        continue;
                    }

                    return (default!, false);
                }
            }
            else
            {
                return (default!, false);
            }

            return (results, true);
        }

        public static ((string Name, PropertyInfo PropInfo) NavigationInfo, bool IsValid)
            FetchNavigationInfo<TEntity, TNavigation>(Expression<Func<TEntity, TNavigation>> exp)
            where TEntity : class
            where TNavigation : class
        {
            if (exp.Body is MemberExpression)
            {
                var memberExpression = exp.Body as MemberExpression;

                if (memberExpression?.Expression is not ParameterExpression)
                {
                    return (default, false);
                }

                var name = memberExpression.Member.Name;

                var v = IsValidForNavigation<TEntity, TNavigation>();

                if (!v.IsValid)
                {
                    return (default, false);
                }

                return ((name, v.PropInfo), true);
            }
            else
            {
                return (default, false);
            }
        }
        public static (bool IsValid, PropertyInfo PropInfo, bool? IsList) IsValidForNavigation<TEntity, TInclude>()
            where TEntity : class
        {
            var propInfo = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(p => p.PropertyType == typeof(TInclude) ||
            p.PropertyType.IsAssignableTo(typeof(List<TInclude>))
            );

            if (propInfo != null)
            {
                return (true, propInfo, propInfo.PropertyType.IsAssignableTo(typeof(List<TInclude>)));
            }
            return (false, null!, null!);
        }
        public static IEnumerable<PropertyInfo> FetchInfoForColumns<TEntity>(params Type[] exclude)
            where TEntity : class =>
            typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => exclude.Any(x => x != p.PropertyType) && !(p.PropertyType.IsGenericType &&
            (p.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
            p.PropertyType.GetInterfaces().Any(t => t.IsGenericType
            && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))));
        public static IEnumerable<string> FetchColumns<TEntity>(params Type[] exclude)
            where TEntity : class => FetchInfoForColumns<TEntity>(exclude).Select(p => p.Name);
        public static (bool IsValid, PropertyInfo PropInfo) IsValidForColumn<TEntity>(string name, params Type[] exclude)
            where TEntity : class
        {
            var propInfo = FetchInfoForColumns<TEntity>(exclude).FirstOrDefault(p => p.Name == name);

            if (propInfo != null)
            {
                return (true, propInfo);
            }
            return (false, null!);
        }
        public static (List<(string Name, PropertyInfo PropInfo)> Keys, bool IsValid)
            FetchKeysInfo<TEntity, TKey>(Expression<Func<TEntity, TKey>> exp)
            where TEntity : class
            where TKey : class
        {
            List<(string, PropertyInfo)> results = [];

            if (exp.Body is NewExpression)
            {
                var e = exp.Body as NewExpression;

                if (e!.Arguments.Any(p => p is not MemberExpression))
                {
                    return (null!, false);
                }

                foreach (var item in e!.Arguments)
                {

                    var memberExpression = item as MemberExpression;

                    if (memberExpression?.Expression is not ParameterExpression)
                    {
                        return (null!, false);
                    }

                    var name = memberExpression.Member.Name;

                    var v = IsValidForColumn<TEntity>(name);

                    if (v.IsValid)
                    {
                        return (null!, false);
                    }

                    results.Add((name, v.PropInfo));

                }
            }
            else if (exp.Body is UnaryExpression)
            {
                var unary = exp.Body as UnaryExpression;

                var operand = unary?.Operand;

                if (operand is not MemberExpression)
                {
                    return (null!, false);
                }

                var memberExpression = operand as MemberExpression;

                if (memberExpression?.Expression is not ParameterExpression)
                {
                    return (null!, false);
                }

                var name = memberExpression.Member.Name;

                var v = IsValidForColumn<TEntity>(name, typeof(TEntity));

                if (!v.IsValid)
                {
                    return (null!, false);
                }

                results.Add((name, v.PropInfo));
            }
            else
            {
                return (null!, false);
            }

            return (results, true);
        }
    }
    abstract class SqliteTypeHandler<T> : TypeHandler<T>
    {
        // Parameters are converted by Microsoft.Data.Sqlite
        public override void SetValue(IDbDataParameter parameter, T? value) => parameter.Value = value;
    }
    class GuidHandler : SqliteTypeHandler<Guid>
    {
        public override Guid Parse(object value) => Guid.Parse((string)value);
    }
    class TipoMovimentoHandler : SqliteTypeHandler<TipoMovimento>
    {
        public override TipoMovimento Parse(object value) => Enum.Parse<TipoMovimento>(
            Enum.GetNames(typeof(TipoMovimento)).FirstOrDefault(p => p[..1] ==
            ((string)value)[..1])!);
    }
    public static class SqliteHandlers
    {
        /// <summary>
        /// Aplica handlers adicionais para o Sqlite.
        /// </summary>
        public static void ApplyTypeHandlers()
        {
            AddTypeHandler(new GuidHandler());
        }
    }
}