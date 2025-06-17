using FluentValidation;
using FluentValidation.Results;
using Moq;
using NSubstitute;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Validators;
using Questao5.Domain.Common.DTOs;
using Questao5.Domain.Entities;
using Questao5.Domain.Enumerators;
using Questao5.Infrastructure.Database.Repositories;
using System.Diagnostics;
using System.Linq.Expressions;
using Xunit;

namespace Questao5
{
    public class ContaCorrenteRepositoryMock
    {
        public static List<ContaCorrente> ContaCorrentes =>
            [
            new ContaCorrente
            {
                IdContaCorrente = Guid.Parse("3622A44D-E511-4F6B-A8B4-E537F5665259"),
                Ativo = true,
                Nome = "User 1",
                Numero = 123
            },
            new ContaCorrente {
                IdContaCorrente = Guid.Parse("197459E8-2DBF-407A-98C6-6E4FE25892C7"),
                Ativo = true,
                Nome = "User 2",
                Numero = 753
            },
            new ContaCorrente {
                IdContaCorrente = Guid.Parse("7E18069A-DD02-4AF5-A6D4-298F35735A80"),
                Ativo = true,
                Nome = "User 3",
                Numero = 357
            },
            new ContaCorrente {
                IdContaCorrente = Guid.Parse("9B5A8C2C-E91E-4965-814E-DB4A98EE22F8"),
                Ativo = true,
                Nome = "User 4",
                Numero = 521
            },
            new ContaCorrente {
                IdContaCorrente = Guid.Parse("370E9A50-CA8A-405C-97DF-8899BEDD7A96"),
                Ativo = true,
                Nome = "User 5",
                Numero = 435
            },
            new ContaCorrente {
                IdContaCorrente = Guid.Parse("E6469DEA-50AC-457C-8623-711BB12C5183"),
                Ativo = true,
                Nome = "User 6",
                Numero = 788
            }
            ];
        public static IGenericRepository<ContaCorrente> RepositorySubstitute()
        {
            var substitute = Substitute.For<IGenericRepository<ContaCorrente>>();

            foreach (var item in ContaCorrentes)
            {
                substitute.BuscarUnicoAsync(
                    Arg.Any<Expression<Func<ContaCorrente, object>>>(),
                    item.IdContaCorrente
                    ).Returns(Task.FromResult(item));
            }

            return substitute;
        }
    }
    public class AddMovimentoCommandValidatorFixture : AbstractValidator<AddMovimentoCommand>
    {
        private readonly IGenericRepository<ContaCorrente> _repository;
        public AddMovimentoCommandValidatorFixture()
        {
            _repository = ContaCorrenteRepositoryMock.RepositorySubstitute();

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
    public class GetSaldoValidatorFixture : AbstractValidator<GetSaldoCommand>
    {
        private readonly IGenericRepository<ContaCorrente> _repository;
        public GetSaldoValidatorFixture()
        {
            _repository = ContaCorrenteRepositoryMock.RepositorySubstitute();

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
    public class UnitAndIntegrationTests(
        AddMovimentoCommandValidatorFixture movimentoValidator,
        GetSaldoValidatorFixture getsaldoValidator
        ) :
        IClassFixture<AddMovimentoCommandValidatorFixture>,
        IClassFixture<GetSaldoValidatorFixture>
    {
        private readonly AddMovimentoCommandValidatorFixture _addValidator = movimentoValidator;
        private readonly GetSaldoValidatorFixture _getsaldoValidator = getsaldoValidator;
        public static TheoryData<GetSaldoCommand> SaldoCommands =>
            [
            new GetSaldoCommand { IdContaCorrente = Guid.Parse("3622A44D-E511-4F6B-A8B4-E537F5665259") },
            new GetSaldoCommand { IdContaCorrente = Guid.Parse("197459E8-2DBF-407A-98C6-6E4FE25892C7") },
            new GetSaldoCommand { IdContaCorrente = Guid.Parse("7E18069A-DD02-4AF5-A6D4-298F35735A80") },
            new GetSaldoCommand { IdContaCorrente = Guid.Parse("9B5A8C2C-E91E-4965-814E-DB4A98EE22F8") },
            new GetSaldoCommand { IdContaCorrente = Guid.Parse("370E9A50-CA8A-405C-97DF-8899BEDD7A96") },
            new GetSaldoCommand { IdContaCorrente = Guid.Parse("E6469DEA-50AC-457C-8623-711BB12C5183") }
            ];
        public static TheoryData<AddMovimentoCommand> MovimentoCommands =>
            [                
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("3622A44D-E511-4F6B-A8B4-E537F5665259"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("3622A44D-E511-4F6B-A8B4-E537F5665259"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("3622A44D-E511-4F6B-A8B4-E537F5665259"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },                
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("197459E8-2DBF-407A-98C6-6E4FE25892C7"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("197459E8-2DBF-407A-98C6-6E4FE25892C7"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("197459E8-2DBF-407A-98C6-6E4FE25892C7"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },                
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("7E18069A-DD02-4AF5-A6D4-298F35735A80"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("7E18069A-DD02-4AF5-A6D4-298F35735A80"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("7E18069A-DD02-4AF5-A6D4-298F35735A80"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },                
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("9B5A8C2C-E91E-4965-814E-DB4A98EE22F8"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("9B5A8C2C-E91E-4965-814E-DB4A98EE22F8"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("9B5A8C2C-E91E-4965-814E-DB4A98EE22F8"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },                
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("370E9A50-CA8A-405C-97DF-8899BEDD7A96"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("370E9A50-CA8A-405C-97DF-8899BEDD7A96"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("370E9A50-CA8A-405C-97DF-8899BEDD7A96"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },                
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("E6469DEA-50AC-457C-8623-711BB12C5183"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("E6469DEA-50AC-457C-8623-711BB12C5183"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
                new AddMovimentoCommand{
                     IdContaCorrente = Guid.Parse("E6469DEA-50AC-457C-8623-711BB12C5183"),
                     TipoMovimento = Random.Shared.Next() % 2 == 0 ? 'D' : 'C',
                     Valor = Random.Shared.NextDouble() + DateTime.Now.Second
                 },
            ];

        [Theory]
        [MemberData(nameof(SaldoCommands), MemberType = typeof(UnitAndIntegrationTests))]
        public async Task ValidateSaldosAsync(GetSaldoCommand saldoCommand)
        {
            var validator = await _getsaldoValidator.ValidateAsync(saldoCommand);

            if (!validator.IsValid)
            {
                foreach (var item in validator.Errors)
                {
                    Assert.Fail(item.ErrorMessage);
                }
            }
            Assert.True(validator.IsValid);
        }

        [Theory]
        [MemberData(nameof(MovimentoCommands), MemberType = typeof(UnitAndIntegrationTests))]
        public async Task ValidateMovimentoAsync(AddMovimentoCommand movimentoCommand)
        {
            var validator = await _addValidator.ValidateAsync(movimentoCommand);

            if (!validator.IsValid)
            {
                foreach (var item in validator.Errors)
                {
                    Assert.Fail(item.ErrorMessage);
                }
            }

            Assert.True(validator.IsValid);
        }
    }
}
