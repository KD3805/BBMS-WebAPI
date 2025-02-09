using BBMS_WebAPI.Models;
using FluentValidation;

namespace BBMS_WebAPI.Validators
{
    public class BloodStockValidator : AbstractValidator<BloodStockModel>
    {
        public BloodStockValidator()
        {
            RuleFor(x => x.BloodGroupName)
                .NotEmpty().WithMessage("BloodGroupName is required.")
                .MaximumLength(10).WithMessage("BloodGroupName can not be more than 10 characters.");
            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity is required.")
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            RuleFor(x => x.LastUpdated)
                .NotEmpty().WithMessage("Last Updated Date is required.");
        }  
    }
}
