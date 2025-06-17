using Microsoft.Extensions.Caching.Distributed;
using Questao5.Domain.Common.DTOs;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.Repositories;
using System.Text;

namespace Questao5.Infrastructure.Database.Cache
{
    /// <summary>
    /// Uma implementação mais completa de cache seria através do uso de 
    /// um período absoluto de expiração e outro de contagem em intervalos 
    /// menores. Mas não é o objetivo aqui.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public class SqliteCache(IServiceProvider serviceProvider) : IDistributedCache
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public byte[] Get(string key)
        {
            throw new NotImplementedException();
        }
        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            var split = key.Split('_');

            Guid guid = split.Length > 1 ? Guid.Parse(split[1]) : Guid.Parse(split[0]);

            await using var scope = _serviceProvider.CreateAsyncScope();

            var repository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Idempotencia>>();

            var cache = await repository.BuscarUnicoAsync(k => k.Chave_Idempotencia, guid);

            if (cache is null)
            {
                return null!;
            }

            var bytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(cache.ToDto()));

            await repository.ExcluirExpiradosCacheAsync<Idempotencia>();

            return bytes;
        }
        public void Refresh(string key)
        {
            throw new NotImplementedException();
        }
        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            var split = key.Split('_');

            Guid guid = split.Length > 1 ? Guid.Parse(split[1]) : Guid.Parse(split[0]);

            await using var scope = _serviceProvider.CreateAsyncScope();

            var repository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Idempotencia>>();

            var existing = await repository.ExcluirAsync(k => k.Chave_Idempotencia, guid);
        }
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            throw new NotImplementedException();
        }
        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            var split = key.Split('_');

            Guid guid;

            string requisicao = null!;

            if (split.Length > 1)
            {
                requisicao = split[0];
                guid = Guid.Parse(split[1]);
            }
            else
            {
                guid = Guid.Parse(split[0]);
            }

            var content = Encoding.Default.GetString(value);

            await using var scope = _serviceProvider.CreateAsyncScope();

            var repository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Idempotencia>>();

            var existing = await repository.BuscarUnicoAsync(k => k.Chave_Idempotencia, guid);

            options.SlidingExpiration ??= TimeSpan.FromMinutes(10);

            var slidingExpiration = DateTime.Now.Add(options.SlidingExpiration!.Value);

            if (existing != null && existing.Expiracao >= DateTime.Now)
            {
                existing.Expiracao = slidingExpiration;
                existing.Resultado = content;
                await repository.EditarAsync(k => k.Chave_Idempotencia, existing);
            }
            else
            {
                if (existing != null && existing!.Expiracao < DateTime.Now)
                {
                    await RemoveAsync(key, token);
                }

                var insert = new Idempotencia
                {
                    Chave_Idempotencia = guid,
                    Requisicao = requisicao,
                    Resultado = content,
                    Expiracao = slidingExpiration
                };

                await repository.InserirAsync(insert);
            }
        }
    }
}
