using MediatR;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Commands.Requests
{
    /// <summary>
    /// O serviço deve realizar as seguintes validações de negócio:
	/// Apenas contas correntes cadastradas podem receber movimentação; TIPO: INVALID_ACCOUNT.
    /// Apenas contas correntes ativas podem receber movimentação; TIPO: INACTIVE_ACCOUNT.
    /// Apenas valores positivos podem ser recebidos; TIPO: INVALID_VALUE.
    /// Apenas os tipos “débito” ou “crédito” podem ser aceitos; TIPO: INVALID_TYPE.
    /// O novo serviço deve requisitar a identificação da requisição,
    /// identificação da conta corrente, o valor a ser movimentado, 
    /// e o tipo de movimento(C = Credito, D = Débito).
    /// </summary>     
    public class AddMovimentoCommand : IRequest<AddMovimentoResponse>
    {
        public Guid IdContaCorrente { get; set; }
        public double Valor { get; set; } = default!;
        public char TipoMovimento { get; set; } = default!;
    }
}
