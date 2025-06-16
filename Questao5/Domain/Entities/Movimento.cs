namespace Questao5.Domain.Entities
{
    public class Movimento
    {
        /// <summary>
        /// identificacao unica do movimento
        /// </summary>        
        public Guid IdMovimento { get; set; }
        /// <summary>
        /// idcontacorrente INTEGER(10) NOT NULL, -- identificacao unica da conta corrente
        /// </summary>        
        public int IdContaCorrente { get; set; }
        /// <summary>
        /// datamovimento TEXT(25) NOT NULL, -- data do movimento no formato DD/MM/YYYY
        /// </summary>
        public DateTime DataMovimento { get; set; }
        /// <summary>
        /// tipomovimento TEXT(1) NOT NULL, -- tipo do movimento. (C = Credito, D = Debito).
        /// CHECK (tipomovimento in ('C','D'))
        /// </summary>
        public char TipoMovimento { get; set; } = default!;
        /// <summary>
        /// valor REAL NOT NULL, -- valor do movimento.Usar duas casas decimais.
        /// </summary>
        public double Valor { get; set; }
        /// <summary>
        /// Navigation Property.
        /// </summary>
        public ContaCorrente ContaCorrente { get; set; } = default!;
    }
}
