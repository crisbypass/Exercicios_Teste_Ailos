using Questao5.Domain.Common;
using Questao5.Domain.Common.DTOs;

namespace Questao5.Application.Queries.Responses
{
    public class GetSaldoResponse : BaseResponse
    {
        public GetSaldoDto GetSaldoDto { get; set; } = default!;
    }
}
