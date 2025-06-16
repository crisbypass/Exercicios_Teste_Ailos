using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Validators;
using Questao5.Infrastructure.Filters;
using Swashbuckle.AspNetCore.Annotations;

namespace Questao5.Infrastructure.Services.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContaCorrenteController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;        

        [SwaggerOperationFilter(typeof(AddHeaderParameter))]
        [Idempotent]
        [HttpGet("Teste")]
        public async Task<IActionResult> Teste()
        {
            return await Task.FromResult(Ok(new { Mensagem = "Tudo OK!" }));
        }
        
        [HttpPost("[action]")]
        public async Task<IActionResult> Movimentar(AddMovimentoCommand command)
        {
            var response = await _mediator.Send(command);

            if (!response.IsValid)
            {
                response.AddToModelState(ModelState);

                return ValidationProblem();
            }

            return Ok(new { response.IdMovimento });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> BuscaSaldo(GetSaldoCommand command)
        {
            var response = await _mediator.Send(command);

            if (!response.IsValid)
            {
                response.AddToModelState(ModelState);

                return ValidationProblem();
            }

            return Ok(new { response.GetSaldoDto });
        }
    }
}
