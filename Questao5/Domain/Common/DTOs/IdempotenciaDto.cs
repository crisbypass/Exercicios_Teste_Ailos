namespace Questao5.Domain.Common.DTOs
{
    public record IdempotenciaDto
    {        
        public Guid Chave_Idempotencia { get; }
        public string Requisicao { get; }
        public string Resultado { get; }

        public IdempotenciaDto(Guid chave_Idempotencia, string requisicao, string resultado)
        {
            Chave_Idempotencia = chave_Idempotencia;
            Requisicao = requisicao;
            Resultado = resultado;
        }
    }
}
