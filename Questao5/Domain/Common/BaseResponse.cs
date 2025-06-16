using FluentValidation.Results;

namespace Questao5.Domain.Common
{
    public abstract class BaseResponse
    {        
        public bool IsValid { get; set; }
        public List<ValidationFailure>? Errors { get; set; }
    }
}
