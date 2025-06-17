using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Questao5.Infrastructure.Filters
{
    /// <summary>
    /// Será interessante acompanhar como o valor da chave única  
    /// poderá ser usada na Idempotência.
    /// </summary>
    public class AddHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= [];

            operation.Parameters.Add(new OpenApiParameter
            {                
                Name = "Idempotency_Key",
                In = ParameterLocation.Header,
                Required = true,                
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}
