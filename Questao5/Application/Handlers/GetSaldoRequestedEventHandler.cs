using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Questao5.Application.Events;
using System.Text;

namespace Questao5.Application.Handlers
{
    public class GetSaldoRequestedEventHandler(IDistributedCache distributedCache) : INotificationHandler<GetSaldoRequestedEvent>
    {
        private readonly IDistributedCache _distributedCache = distributedCache;
        public async Task Handle(GetSaldoRequestedEvent notification, CancellationToken cancellationToken)
        {
            var idConta = notification.GetSaldoDto.IdContaCorrente;

            var idKey = $"{nameof(GetSaldoRequestedEvent)}_{idConta}";

            var bytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(
                notification.GetSaldoDto));

            await _distributedCache.SetAsync(idKey, bytes, cancellationToken);
        }
    }
}
