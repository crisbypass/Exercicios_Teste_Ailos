namespace Questao5.Domain.Entities
{
    public class ContaCorrente
    {
        /// <summary>
        /// id da conta corrente
        /// </summary>        
        public Guid IdContaCorrente { get; set; }
        /// <summary>
        /// numero INTEGER(10) NOT NULL UNIQUE, -- numero da conta corrente
        /// </summary>
        public int Numero { get; set; }
        /// <summary>
        /// nome TEXT(100) NOT NULL, -- nome do titular da conta corrente
        /// </summary>
        public string Nome { get; set; } = default!;
        /// <summary>
        /// ativo INTEGER(1) NOT NULL default 0, -- indicativo se a conta esta ativa. (0 = inativa, 1 = ativa).
        /// CHECK(ativo in (0,1))
        /// </summary>        
        public bool Ativo { get; set; }
        /// <summary>
        /// Navigation Property.
        /// </summary>
        public List<Movimento> Movimentos { get; set; } = [];
    }
}
