using MediatR;
using Questao5.Domain.Common.DTOs;

namespace Questao5.Application.Events
{
    public class MovimentoCreatedEvent : INotification
    {
        public MovimentoDto Movimento { get; set; } = default!;
    }
}
