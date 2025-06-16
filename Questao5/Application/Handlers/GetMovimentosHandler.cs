using MediatR;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Handlers
{
    public class GetMovimentosHandler :
        IRequestHandler<GetMovimentosCommand, GetMovimentosResponse>
    {
        public Task<GetMovimentosResponse> Handle(GetMovimentosCommand request,
            CancellationToken cancellationToken)
        {
            
            throw new NotImplementedException();
        }
    }
}
