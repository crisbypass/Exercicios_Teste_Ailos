using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.Handlers
{
    public class DeleteMovimentoHandler :
        IRequestHandler<DeleteMovimentoCommand, DeleteMovimentoResponse>
    {
        public Task<DeleteMovimentoResponse> Handle(DeleteMovimentoCommand request,
            CancellationToken cancellationToken)
        {            
            throw new NotImplementedException();
        }
    }
}
