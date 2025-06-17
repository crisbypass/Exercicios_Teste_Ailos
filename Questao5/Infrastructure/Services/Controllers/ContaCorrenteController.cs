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

        /// <summary>
        /// retornará diretamente os resultados em cache e o pipeline será 
        /// interrompido antes de efetuar o processamento total. Foi adicionado
        /// para verificação de comportamento do cache.
        /// </summary>        
        [SwaggerOperationFilter(typeof(AddHeaderParameter))]
        [Idempotent]
        [HttpGet("[action]")]
        public async Task<IActionResult> Idempotencia()
        {            
            return await Task.FromResult(Ok());
        }
        /// <summary>
        /// Geralmente métodos como o Post, não são(ou não deveriam)
        /// ser Idempotentes. Mas é possível se adequar ao comportamento
        /// com algum código.
        /// </summary>
        /// <param name="command"></param>        
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
