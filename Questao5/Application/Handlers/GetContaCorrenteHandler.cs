using MediatR;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Handlers
{
    public class GetContaCorrenteHandler()
        : IRequestHandler<GetContaCorrenteCommand, GetContaCorrenteResponse>
    {        
        public Task<GetContaCorrenteResponse> Handle(GetContaCorrenteCommand request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<GetContaCorrenteResponse>(default!);
        }
    }
}
