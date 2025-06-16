using Questao5.Domain.Common;

namespace Questao5.Application.Commands.Responses
{
    /*
    O serviço deve realizar as seguintes validações de negócio:
	Apenas contas correntes cadastradas podem receber movimentação; TIPO: INVALID_ACCOUNT.
	Apenas contas correntes ativas podem receber movimentação; TIPO: INACTIVE_ACCOUNT.
	Apenas valores positivos podem ser recebidos; TIPO: INVALID_VALUE.
	Apenas os tipos “débito” ou “crédito” podem ser aceitos; TIPO: INVALID_TYPE.

    O novo serviço deve requisitar a identificação da requisição,
    identificação da conta corrente, o valor a ser movimentado, e o tipo de movimento (C = Credito, D = Débito).

    Caso os dados sejam recebidos e estejam válidos, devem ser persistidos na tabela MOVIMENTO e deve retornar
    HTTP 200 e retornar no body Id do movimento gerado.
    Caso os dados estejam inconsistentes, deve retornar falha HTTP 400 (Bad Request) e no 
    body uma mensagem descritiva de qual foi a falha e o tipo de falha.
    */
    public class AddMovimentoResponse : BaseResponse
    {
        public Guid IdMovimento { get; set; }
    }
}
