using FluentValidation;
using BBMS_WebAPI.Models;
using BBMS_WebAPI.Utilities;

namespace BBMS_WebAPI.Validators
{
    public class RecipientValidator : AbstractValidator<RecipientModel>
    {
        public RecipientValidator() 
        {
            // Rule 1: All required fields
            RuleFor(recipient => recipient.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(recipient => recipient.Email)
                .NotEmpty()
                .WithMessage("Email is required.");

            RuleFor(recipient => recipient.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required.");

            RuleFor(recipient => recipient.Gender)
                .NotEmpty()
                .WithMessage("Gender is required.");

            RuleFor(recipient => recipient.BloodGroupName)
                .NotEmpty()
                .WithMessage("Blood group is required.");

            RuleFor(recipient => recipient.DOB)
                .NotEmpty()
                .WithMessage("Date of Birth is required.");

            // Rule 2: Valid email address
            RuleFor(recipient => recipient.Email)
                .EmailAddress()
                .WithMessage("Please enter a valid email address.");

            // Rule 3: Phone number must be exactly 10 digits
            RuleFor(recipient => recipient.Phone)
                .Matches("^\\d{10}$")
                .WithMessage("Phone number must be exactly 10 digits.");

            // Rule 4: Gender should be from {Male, Female, Other}
            RuleFor(recipient => recipient.Gender)
                .Must(gender => new[] { "Male", "Female", "Other" }.Contains(gender))
                .WithMessage("Gender must be Male, Female, or Other.");

            // Rule 5: Blood group value should be valid
            RuleFor(recipient => recipient.BloodGroupName)
                .Must(bg => BloodGroupMapper.GetBloodGroupID(bg) != null)
                .WithMessage("Invalid Blood Group Name.");
        }
    }
}
