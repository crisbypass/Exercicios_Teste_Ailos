using Questao5.Domain.Common;

namespace Questao5.Application.Commands.Responses
{
    public class UpdateMovimentoResponse : BaseResponse
    {
        public Guid IdMovimento { get; set; }
    }
}
