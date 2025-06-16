using FluentValidation;
using FluentValidation.Results;
using Questao5.Application.Queries.Requests;
using Questao5.Domain.Common.DTOs;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Infrastructure.Database.Repositories;

namespace Questao5.Application.Validators
{
    public class GetSaldoValidator : AbstractValidator<GetSaldoCommand>
    {
        private readonly IGenericRepository<ContaCorrente> _repository;

        public GetSaldoValidator(IGenericRepository<ContaCorrente> repository)
        {
            _repository = repository;
            
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
