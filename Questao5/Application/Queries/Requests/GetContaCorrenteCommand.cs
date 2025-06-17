using MediatR;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Queries.Requests
{   
    public class GetContaCorrenteCommand : IRequest<GetContaCorrenteResponse>
    {
        public Guid IdContaCorrente { get; set; }
    }
}
