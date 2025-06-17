using MediatR;
using Questao5.Domain.Common.DTOs;

namespace Questao5.Application.Events
{
    public class GetSaldoRequestedEvent(GetSaldoDto getSaldoDto) : INotification
    {
        public GetSaldoDto GetSaldoDto { get; set; } = getSaldoDto;
    }
}
