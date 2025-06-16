using Questao5.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Questao5.Domain.Common.DTOs
{
    public record MovimentoDto
    {
        /// <summary>
        /// identificacao unica do movimento
        /// </summary>        
        public Guid IdMovimento { get; }
        /// <summary>
        /// idcontacorrente INTEGER(10) NOT NULL, -- identificacao unica da conta corrente
        /// </summary>        
        public int IdContaCorrente { get; }
        /// <summary>
        /// datamovimento TEXT(25) NOT NULL, -- data do movimento no formato DD/MM/YYYY
        /// </summary>
        public DateTime DataMovimento { get; }
        /// <summary>
        /// tipomovimento TEXT(1) NOT NULL, -- tipo do movimento. (C = Credito, D = Debito).
        /// CHECK (tipomovimento in ('C','D'))
        /// </summary>
        public char TipoMovimento { get; }
        /// <summary>
        /// valor REAL NOT NULL, -- valor do movimento.Usar duas casas decimais.
        /// </summary>
        public double Valor { get; }

        public MovimentoDto(Guid idMovimento, int idContaCorrente, DateTime dataMovimento, char tipoMovimento, double valor)
        {
            IdMovimento = idMovimento;
            IdContaCorrente = idContaCorrente;
            DataMovimento = dataMovimento;
            TipoMovimento = tipoMovimento;
            Valor = valor;
        }
    }
}
