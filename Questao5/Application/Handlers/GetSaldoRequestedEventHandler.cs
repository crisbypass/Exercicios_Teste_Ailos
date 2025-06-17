using MediatR;
using Questao5.Application.Events;

namespace Questao5.Application.Handlers
{
    public class GetSaldoRequestedEventHandler : INotificationHandler<GetSaldoRequestedEvent>
    {
        public Task Handle(GetSaldoRequestedEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
