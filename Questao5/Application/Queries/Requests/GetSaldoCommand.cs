using MediatR;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Queries.Requests
{
    /*
     Fórmula:
    SALDO = SOMA_DOS_CREDITOS – SOMA_DOS_DEBITOS
    Observação: Caso a conta não possua nenhuma movimentação, a API deve retornar o valor 0.00 (Zero).
    O serviço deve realizar as seguintes validações de negócio:
	Apenas contas correntes cadastradas podem consultar o saldo; TIPO: INVALID_ACCOUNT.
	Apenas contas correntes ativas podem consultar o saldo; TIPO: INACTIVE_ACCOUNT.
    Caso os dados sejam recebidos e estejam válidos, deve retornar HTTP 200 e retornar no body com os seguintes dados:
	Número da conta corrente
	Nome do titular da conta corrente
	Data e hora da resposta da consulta
	Valor do Saldo atual
    Caso os dados estejam inconsistentes, deve retornar falha HTTP 400 (Bad Request) e no body uma mensagem descritiva de qual foi a falha e o tipo de falha.
     */
    public class GetSaldoCommand : IRequest<GetSaldoResponse>
    {
        public Guid IdContaCorrente { get; set; }
    }
}
