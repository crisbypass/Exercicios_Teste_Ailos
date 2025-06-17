using System.Linq.Expressions;

namespace Questao5.Infrastructure.Database.Repositories
{
    public interface IGenericRepository<TEntity, TInclude> : IGenericRepository<TEntity>
        where TEntity : class
        where TInclude : class
    {
        Task<TEntity?> BuscarUnicoAsync(
            Expression<Func<TEntity, object>> keys,
            Expression<Func<TEntity, TInclude, object>> relationOn = null!,
            params object[] keyValues
            );
        Task<bool> InserirAsync(TEntity item, Expression<Func<TEntity, TInclude, object>> relation);
    }
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        Task<TEntity> BuscarUnicoAsync(Expression<Func<TEntity, object>> keys, params object[] keyValues);
        Task<IEnumerable<T>> BuscarVariosAsync<T>(Expression<Func<TEntity, object>> keys, params object[] keyValues);
        Task<bool> EditarAsync(Expression<Func<TEntity, object>> keys, TEntity item);
        Task<bool> ExcluirAsync(Expression<Func<TEntity, object>> keys, params object[] keyValues);
        Task<bool> InserirAsync(TEntity item);
        Task<double> BuscarSaldoAsync(Expression<Func<TEntity, object>> keys, params object[] contaId);
    }
}
