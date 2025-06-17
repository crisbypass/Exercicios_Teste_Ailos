using FluentValidation;
using MediatR;
using Questao5.Application.Events;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Common.DTOs;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.Repositories;

namespace Questao5.Application.Handlers
{
    public class GetSaldoHandler(
        IMediator mediator,
        IGenericRepository<ContaCorrente> contaRepository,
        IValidator<GetSaldoCommand> validator) :
        IRequestHandler<GetSaldoCommand, GetSaldoResponse>
    {
        private readonly IMediator _mediator = mediator;
        private readonly IGenericRepository<ContaCorrente> _contaRepository = contaRepository;
        private readonly IValidator<GetSaldoCommand> _validator = validator;

        public async Task<GetSaldoResponse> Handle(GetSaldoCommand request,
            CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                return new GetSaldoResponse { IsValid = false, Errors = validation.Errors };
            }

            var conta = await _contaRepository.BuscarUnicoAsync(f => f.IdContaCorrente, request.IdContaCorrente);

            var saldo = await _contaRepository.BuscarSaldoAsync(k => k.IdContaCorrente, conta.IdContaCorrente);

            var getSaldosDto = new GetSaldoDto(conta.IdContaCorrente, conta.Nome, DateTime.Now, saldo);

            await _mediator.Publish(new GetSaldoRequestedEvent(getSaldosDto), cancellationToken);

            return new GetSaldoResponse
            {
                GetSaldoDto = getSaldosDto,
                IsValid = true
            };            
        }
    }
}
