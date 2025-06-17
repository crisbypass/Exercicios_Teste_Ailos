using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Questao5.Application.Events;
using System.Text;

namespace Questao5.Application.Handlers
{
    public class MovimentoCreatedEventHandler(IDistributedCache distributedCache)
        : INotificationHandler<MovimentoCreatedEvent>
    {
        private readonly IDistributedCache _distributedCache = distributedCache;
        public async Task Handle(MovimentoCreatedEvent notification, CancellationToken cancellationToken)
        {
            var idMovimento = notification.Movimento.IdMovimento.ToString();

            var bytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(
                notification.Movimento));

            await _distributedCache.SetAsync(idMovimento, bytes, cancellationToken);            
        }
    }
}
