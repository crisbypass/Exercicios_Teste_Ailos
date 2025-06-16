using Microsoft.Extensions.Caching.Distributed;
using Questao5.Domain.Common.DTOs;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.Repositories;
using System.Text;

namespace Questao5.Infrastructure.Database.Cache
{
    public class SqliteCache(IServiceProvider serviceProvider) : IDistributedCache
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public byte[] Get(string key)
        {
            throw new NotImplementedException();
        }
        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            var repository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Idempotencia>>();

            var guid = Guid.Parse(key);

            var cache = await repository.BuscarUnicoAsync(k => k.Chave_Idempotencia, guid);

            var bytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(cache.ToDto()));
            
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

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {

            throw new NotImplementedException();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {

            await using var scope = _serviceProvider.CreateAsyncScope();

            var repository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Idempotencia>>();

            var guid = Guid.Parse(key);

            var content = Encoding.Default.GetString(value);

            var existing = await repository.BuscarUnicoAsync(k => k.Chave_Idempotencia, guid);

            if (existing != null)
            {
                existing.Resultado = content;

                await repository.EditarAsync(k => k.Chave_Idempotencia, existing);
            }
            else
            {
                var insert = new Idempotencia
                {
                    Chave_Idempotencia = guid,
                    Resultado = content
                };

                await repository.InserirAsync(insert);
            }
        }
    }
}
