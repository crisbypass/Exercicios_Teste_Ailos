using FluentValidation;
using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Events;
using Questao5.Domain.Common.DTOs;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.Repositories;

namespace Questao5.Application.Handlers
{
    public class AddMovimentoHandler(IMediator mediator,
        IValidator<AddMovimentoCommand> validator,
        IGenericRepository<ContaCorrente> contaRepository,
        IGenericRepository<Movimento, ContaCorrente> movimentoRepository
        ) :
        IRequestHandler<AddMovimentoCommand, AddMovimentoResponse>
    {
        private readonly IMediator _mediator = mediator;
        private readonly IValidator<AddMovimentoCommand> _validator = validator;
        private readonly IGenericRepository<ContaCorrente> _contaRepository = contaRepository;
        private readonly IGenericRepository<Movimento, ContaCorrente> _movimentorepository = movimentoRepository;

        public async Task<AddMovimentoResponse> Handle(AddMovimentoCommand request,
            CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                return new AddMovimentoResponse { IsValid = false, Errors = validation.Errors };
            }

            var conta = await _contaRepository.BuscarUnicoAsync(f => f.IdContaCorrente, request.IdContaCorrente);

            var movimento = new Movimento()
            {
                Valor = request.Valor,
                TipoMovimento = request.TipoMovimento,
                DataMovimento =
                DateTime.Now.Date,
                IdMovimento = Guid.NewGuid(),
                ContaCorrente = conta,
            };

            var success = await _movimentorepository.InserirAsync(movimento, (l, r) =>
            new { l.IdContaCorrente }.Equals(new { r.Numero }));

            if (success)
            {
                await _mediator.Publish(new MovimentoCreatedEvent
                {
                    Movimento = movimento.ToDto()
                }, cancellationToken);

                return new AddMovimentoResponse
                {
                    IdMovimento = movimento.IdMovimento,
                    IsValid = true
                };
            }

            return default!;
        }
    }
}
