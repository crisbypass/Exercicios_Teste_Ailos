using FluentValidation;
using FluentValidation.Results;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Common.DTOs;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Infrastructure.Database.Repositories;

namespace Questao5.Application.Validators
{
    public class AddMovimentoCommandoValidator : AbstractValidator<AddMovimentoCommand>
    {
        private readonly IGenericRepository<ContaCorrente> _repository;

        public AddMovimentoCommandoValidator(IGenericRepository<ContaCorrente> repository)
        {
            _repository = repository;

            RuleFor(p => p.TipoMovimento)
                .NotNull().WithMessage(p => $"{nameof(p.Valor)} precisa ser especificado.")
                .Custom((value, context) =>
                {
                    if (value != 'C' && value != 'D')
                    {
                        context.AddFailure(new ValidationFailure(context.PropertyPath, ContaCorrenteInfo.INVALID_TYPE));
                    }
                });

            RuleFor(p => p.Valor)
                .Custom((value, context) =>
                {
                    if (value < 0)
                    {
                        context.AddFailure(new ValidationFailure(context.PropertyPath, ContaCorrenteInfo.INVALID_VALUE));
                    }
                })
                .NotNull().WithMessage("O valor precisa ser especificado.");

            RuleFor(p => p.IdContaCorrente)
                .NotNull().WithMessage(p => $"{nameof(p.IdContaCorrente)} precisa ser especificado.")
                .Custom((guid, context) =>
                {
                    if (!Guid.TryParse(guid.ToString(), out var isGuid))
                    {
                        context.AddFailure(new ValidationFailure(context.PropertyPath, "GUID inválido."));
                    }
                }).MustAsync(async (request, guid, context, token) =>
                {
                    var conta = await _repository.BuscarUnicoAsync(x => x.IdContaCorrente, request.IdContaCorrente);

                    if (conta == null)
                    {
                        context.AddFailure(new ValidationFailure(nameof(conta.IdContaCorrente),
                            ContaCorrenteInfo.INVALID_ACCOUNT));

                        return false;
                    }

                    var contaDto = conta?.ToDto();

                    if (!contaDto!.Ativo)
                    {
                        context.AddFailure(new ValidationFailure(
                                nameof(conta.IdContaCorrente), ContaCorrenteInfo.INACTIVE_ACCOUNT
                            ));

                        return false;
                    }

                    return true;

                });
        }
    }
}
