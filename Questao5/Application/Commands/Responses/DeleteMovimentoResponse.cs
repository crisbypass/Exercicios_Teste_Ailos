using Questao5.Domain.Common;

namespace Questao5.Application.Commands.Responses
{
    public class DeleteMovimentoResponse : BaseResponse
    {
        public Guid IdMovimento { get; set; }
    }
}
