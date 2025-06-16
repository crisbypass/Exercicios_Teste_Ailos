using System.Runtime.ConstrainedExecution;

namespace Questao5.Domain.Enumerators
{
    /// <remarks>
    /// O serviço deve realizar as seguintes validações de negócio:
	/// Apenas contas correntes cadastradas podem receber movimentação; TIPO: INVALID_ACCOUNT.
    /// Apenas contas correntes ativas podem receber movimentação; TIPO: INACTIVE_ACCOUNT.
    /// Apenas valores positivos podem ser recebidos; TIPO: INVALID_VALUE.
    /// Apenas os tipos “débito” ou “crédito” podem ser aceitos; TIPO: INVALID_TYPE
    /// </remarks>     
    public static class ContaCorrenteInfo
    {
        public const string INVALID_ACCOUNT = "INVALID_ACCOUNT";
        public const string INACTIVE_ACCOUNT = "INACTIVE_ACCOUNT";
        public const string INVALID_VALUE = "INVALID_VALUE";
        public const string INVALID_TYPE = "INVALID_TYPE";
    }
}
