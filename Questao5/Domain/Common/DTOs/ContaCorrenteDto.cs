namespace Questao5.Domain.Common.DTOs
{
    [Serializable]
    public record ContaCorrenteDto
    {
        /// <summary>
        /// id da conta corrente
        /// </summary>        
        public Guid IdContaCorrente { get; }
        /// <summary>
        /// numero INTEGER(10) NOT NULL UNIQUE, -- numero da conta corrente
        /// </summary>
        public int Numero { get; }
        /// <summary>
        /// nome TEXT(100) NOT NULL, -- nome do titular da conta corrente
        /// </summary>
        public string Nome { get; }
        /// <summary>
        /// ativo INTEGER(1) NOT NULL default 0, -- indicativo se a conta esta ativa. (0 = inativa, 1 = ativa).
        /// CHECK(ativo in (0,1))
        /// </summary>        
        public bool Ativo { get; }

        public List<MovimentoDto> Movimentos { get; }
        public ContaCorrenteDto(Guid idContaCorrente, int numero, string nome, bool ativo, List<MovimentoDto> movimentos)
        {
            IdContaCorrente = idContaCorrente;
            Numero = numero;
            Nome = nome;
            Ativo = ativo;
            Movimentos = movimentos;
        }
    }
}
