using FluentValidation;
using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Events;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.Repositories;

namespace Questao5.Application.Handlers
{
    public class GetSaldoHandler(
        IGenericRepository<ContaCorrente> contaRepository,
        IValidator<GetSaldoCommand> validator) :
        IRequestHandler<GetSaldoCommand, GetSaldoResponse>
    {
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

            var saldo = await _contaRepository.BuscarSaldoAsync(k => k.IdContaCorrente, conta);

            return new GetSaldoResponse
            {
                GetSaldoDto = new(conta.Numero, conta.Nome, DateTime.Now, saldo),
                IsValid = true
            };            
        }
    }
}
