using FluentValidation;
using FluentValidation.Results;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Validators;
using Questao5.Domain.Enumerators;
using Xunit;

namespace Questao5
{
    public class AddMovimentoCommandoValidatorFixture : AbstractValidator<AddMovimentoCommand>
    {
        public AddMovimentoCommandoValidatorFixture()
        {
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
        }
    }
    public class GetSaldoValidatorFixture : AbstractValidator<GetSaldoCommand>
    {
        public GetSaldoValidatorFixture()
        {

            RuleFor(p => p.IdContaCorrente)
            .NotNull().WithMessage(p => $"{nameof(p.IdContaCorrente)} precisa ser especificado.")
            .Custom((guid, context) =>
            {
                if (!Guid.TryParse(guid.ToString(), out var isGuid))
                {
                    context.AddFailure(new ValidationFailure(context.PropertyPath, "GUID inválido."));
                }
            });
        }
    }
    public class UnitTests(IClassFixture<AddMovimentoCommandoValidator> addMovimentoCommandoValidator) :
        IClassFixture<AddMovimentoCommandoValidator>,
        IClassFixture<GetSaldoValidatorFixture>
    {
        private readonly IClassFixture<AddMovimentoCommandoValidator> addMovimentoCommandoValidator = addMovimentoCommandoValidator;

        public static TheoryData<GetSaldoCommand> Commands =>
               [
               new GetSaldoCommand { IdContaCorrente = Guid.Parse("7CC2604C-B344-4B96-8D76-B5B8DEE39610") },
                new GetSaldoCommand { IdContaCorrente = Guid.Parse("72BC4685-FD8E-43C3-AACE-19498B77DFF5") },
                new GetSaldoCommand { IdContaCorrente = Guid.Parse("7BC255E8-C4EA-4FB9-A094-AE91FEA050D7") },
                new GetSaldoCommand { IdContaCorrente = Guid.Parse("A10B0CE0-CE92-4FF3-A73F-7A8A5BA24C40") },
                new GetSaldoCommand { IdContaCorrente = Guid.Parse("A10B0CE0-CE92-4FF3-A73F-7A8A5BA24C40") },
                new GetSaldoCommand { IdContaCorrente = Guid.Parse("A10B0CE0-CE92-4FF3-A73F-7A8A5BA24C40") }
               ];
        public static TheoryData<AddMovimentoCommand> Commands2 =>
            [
            new AddMovimentoCommand { TipoMovimento = 'C', Valor = 95.93 },
                new AddMovimentoCommand { TipoMovimento = 'D', Valor = 25.24 },
                new AddMovimentoCommand { TipoMovimento = 'C', Valor = 85.71 },
                new AddMovimentoCommand { TipoMovimento = 'D', Valor = 25.27 },
                new AddMovimentoCommand { TipoMovimento = 'D', Valor = 75.83 },
                new AddMovimentoCommand { TipoMovimento = 'C', Valor = 25.22 }
            ];

        [Theory]
        [MemberData(nameof(Commands), MemberType = typeof(UnitTests))]
        public async Task CriarTarefaTesteAsync(GetSaldoCommand saldoCommand)
        {
            
        }

    }
}
