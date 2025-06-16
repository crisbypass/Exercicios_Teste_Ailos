using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Questao5.Domain.Common;

namespace Questao5.Application.Validators
{
    public static class ValidatorsExtensions
    {        
        public static ValidationResult AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return result;
        }
        public static BaseResponse AddToModelState(this BaseResponse response, ModelStateDictionary modelState)
        {
            foreach (var error in response.Errors!)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return response;
        }
    }
}
