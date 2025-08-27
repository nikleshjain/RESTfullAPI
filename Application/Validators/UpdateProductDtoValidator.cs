using FluentValidation;
using RESTfullAPI.Application.DTOs;

namespace RESTfullAPI.Application.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(255).WithMessage("Product name cannot exceed 255 characters");
        
        RuleFor(x => x.ModifiedBy)
            .NotEmpty().WithMessage("Modified by is required")
            .MaximumLength(100).WithMessage("Modified by cannot exceed 100 characters");
    }
}



