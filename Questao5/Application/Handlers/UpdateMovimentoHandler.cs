using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Handlers
{
    public class UpdateMovimentoHandler :
        IRequestHandler<UpdateMovimentoCommand, UpdateMovimentoResponse>
    {
        public Task<UpdateMovimentoResponse> Handle(UpdateMovimentoCommand request,
            CancellationToken cancellationToken)
        {
            
            throw new NotImplementedException();
        }
    }
}
