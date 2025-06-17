using Dapper;
using FluentAssertions.Formatting;
using Microsoft.Data.Sqlite;
using Microsoft.OpenApi.Expressions;
using NSubstitute;
using Questao5.Infrastructure.Sqlite;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;

namespace Questao5.Infrastructure.Database.Repositories
{
    public class GenericRepository<TEntity, TInclude>(SqliteConnection sqliteConnection)
        : GenericRepository<TEntity>(sqliteConnection),
        IGenericRepository<TEntity, TInclude>
        where TEntity : class
        where TInclude : class
    {
        public Task<TEntity?> BuscarUnicoAsync(
            Expression<Func<TEntity, object>> keys,
            Expression<Func<TEntity, TInclude, object>> relationOn = null!,
            params object[] keyValues
            ) => _sqlConnection.BuscarUnicoAsync(keys, relationOn, keyValues)!;

        public Task<bool> InserirAsync(TEntity item, Expression<Func<TEntity, TInclude, object>> relation)
            => _sqlConnection.InserirAsync(item, relation);
    }
    public class GenericRepository<TEntity>(SqliteConnection sqlConnection) : IGenericRepository<TEntity>
        where TEntity : class
    {
        protected readonly SqliteConnection _sqlConnection = sqlConnection;
        public Task<TEntity> BuscarUnicoAsync(Expression<Func<TEntity, object>> keys, params object[] keyId)
            => _sqlConnection.BuscarUnicoAsync(keys, keyId);
        public Task<IEnumerable<T>> BuscarVariosAsync<T>(Expression<Func<TEntity, object>> keys, params object[] keyValues)
        {
            throw new NotImplementedException();
        }        
        public Task<bool> EditarAsync(Expression<Func<TEntity, object>> keys, TEntity item) =>
            _sqlConnection.EditarAsync(keys, item);
        public Task<bool> ExcluirAsync(Expression<Func<TEntity, object>> keys, params object[] keyValues) => 
            _sqlConnection.ExcluirAsync(keys, keyValues);        
        public Task<bool> InserirAsync(TEntity item) => _sqlConnection.InserirAsync(item);

        public Task<double> BuscarSaldoAsync(Expression<Func<TEntity, object>> keys, params object[] contaId)
            => _sqlConnection.BuscarSaldoAsync(keys, contaId);
    }
}
