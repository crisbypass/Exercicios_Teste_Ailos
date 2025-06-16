using Questao5.Domain.Entities;

namespace Questao5.Domain.Common.DTOs
{
    public static class DtosExtensions
    {
        public static ContaCorrenteDto ToDto(this ContaCorrente contaCorrente)
        {
            return new ContaCorrenteDto(
                contaCorrente.IdContaCorrente,
                contaCorrente.Numero,
                contaCorrente.Nome, contaCorrente.Ativo,
                [.. contaCorrente.Movimentos.Select(x=> x.ToDto())]
                );
        }
        public static MovimentoDto ToDto(this Movimento movimento)
        {
            return new MovimentoDto(
                movimento.IdMovimento,
                movimento.IdContaCorrente,
                movimento.DataMovimento,
                movimento.TipoMovimento,
                movimento.Valor
                );
        }
        public static IdempotenciaDto ToDto(this Idempotencia idempotencia)
        {
            return new IdempotenciaDto(
                idempotencia.Chave_Idempotencia,
                idempotencia.Requisicao,
                idempotencia.Resultado
                );
        }
    }
}
