using BBMS_WebAPI.Models;
using FluentValidation;

namespace BBMS_WebAPI.Validators
{
    public class AdminValidator : AbstractValidator<AdminModel>
    {
        public AdminValidator() 
        {
            RuleFor(admin => admin.Name)
                .NotEmpty()
                .NotNull()
                .WithMessage("Name is required.");

            RuleFor(admin => admin.Email)
                .NotEmpty()
                .NotNull()
                .WithMessage("Email is required.");

            RuleFor(admin => admin.Phone)
                .NotEmpty()
                .NotNull()
                .WithMessage("Phone number is required.");

            RuleFor(admin => admin.Password)
                .NotEmpty()
                .NotNull()
                .WithMessage("Password is required.");

            RuleFor(admin => admin.Role)
                .NotEmpty()
                .NotNull()
                .WithMessage("Role is required.");

            // Valid email address
            RuleFor(donor => donor.Email)
                .EmailAddress()
                .WithMessage("Please enter a valid email address.");

            // Phone number must be exactly 10 digits
            RuleFor(donor => donor.Phone)
                .Matches("^\\d{10}$")
                .WithMessage("Phone number must be exactly 10 digits.");
        }
    }
}
