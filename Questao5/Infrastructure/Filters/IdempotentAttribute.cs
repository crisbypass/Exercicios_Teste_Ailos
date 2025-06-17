using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using Questao5.Domain.Common.DTOs;
using System.Text.Json;

namespace Questao5.Infrastructure.Filters
{
    /// <summary>
    /// Vamos aplicar uma verificação do cache, antes do processamento total do pipeline da requisição.
    /// </summary>
    /// <param name="cacheTimeInMinutes"></param>
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class IdempotentAttribute(int cacheTimeInMinutes = IdempotentAttribute.DefaultCacheTimeInMinutes) : Attribute, IAsyncActionFilter
    {
        private const int DefaultCacheTimeInMinutes = 60;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(cacheTimeInMinutes);
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {            
            if (!context.HttpContext.Request.Headers.TryGetValue(
                    "Idempotency_Key",
                    out StringValues idempotenceKeyValue) ||
                !Guid.TryParse(idempotenceKeyValue, out Guid idempotencyKey))
            {
                context.Result = new BadRequestObjectResult("O valor da chave de idempotência fornecido no cabeçalho é inválido.");
                return;
            }

            IDistributedCache cache = context.HttpContext
                .RequestServices.GetRequiredService<IDistributedCache>();
            
            string cacheKey = idempotencyKey.ToString();
            string? cachedResult = await cache.GetStringAsync(cacheKey);
            if (cachedResult is not null)
            {
                IdempotenciaDto response = JsonSerializer.Deserialize<IdempotenciaDto>(cachedResult)!;

                var result = new ObjectResult(response) { StatusCode = StatusCodes.Status200OK };
                
                context.Result = result;

                return;
            }
            
            ActionExecutedContext executedContext = await next();

            if (executedContext.Result is ObjectResult { StatusCode: >= 200 and < 300 } objectResult)
            {
                int statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
                IdempotenciaDto response = (IdempotenciaDto)objectResult.Value!;

                await cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(response),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheDuration }
                );
            }
        }
    }    
}
