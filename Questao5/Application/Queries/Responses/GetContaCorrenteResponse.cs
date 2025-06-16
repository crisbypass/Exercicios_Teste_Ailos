using FluentValidation.Results;
using Questao5.Domain.Common;
using Questao5.Domain.Common.DTOs;
using Questao5.Domain.Enumerators;

namespace Questao5.Application.Queries.Responses
{
    public class GetContaCorrenteResponse() : BaseResponse
    {            
        //public GetContaCorrenteResponse(bool isValid, ContaCorrenteDto contaCorrenteDto = null!,
        //    List<ValidationFailure> errors = null!) : this()
        //{
        //    IsValid = isValid;
        //    Errors = errors;
        //    ContaCorrenteDto = contaCorrenteDto;            
        //}
        public  ContaCorrenteDto ContaCorrenteDto { get; set; } = default!;        
    }
}
