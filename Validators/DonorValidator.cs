using FluentValidation;
using BBMS_WebAPI.Models;

namespace BBMS_WebAPI.Validators
{
    public class DonorValidator : AbstractValidator<DonorModel>
    {
        public DonorValidator()
        {
            // Rule 1: All fields are required
            RuleFor(donor => donor.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(donor => donor.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(donor => donor.Phone).NotEmpty().WithMessage("Phone number is required.");
            RuleFor(donor => donor.Gender).NotEmpty().WithMessage("Gender is required.");
            RuleFor(donor => donor.BloodGroup).NotEmpty().WithMessage("Blood group is required.");
            RuleFor(donor => donor.DOB).NotEmpty().WithMessage("Date of Birth is required.");

            // Rule 2: Donor must be at least 18 years old
            RuleFor(donor => donor.Age)
                .Must(age => age >= 18)
                .WithMessage("Donor must be at least 18 years old.");

            // Rule 3: Valid email address
            RuleFor(donor => donor.Email)
                .EmailAddress()
                .WithMessage("Please enter a valid email address.");

            // Rule 4: Phone number must be exactly 10 digits
            RuleFor(donor => donor.Phone)
                .Matches("^\\d{10}$")
                .WithMessage("Phone number must be exactly 10 digits.");

            // Rule 5: Gender should be from {Male, Female, Other}
            RuleFor(donor => donor.Gender)
                .Must(gender => new[] { "Male", "Female", "Other" }.Contains(gender))
                .WithMessage("Gender must be Male, Female, or Other.");

            // Rule 6: Blood group value should be valid
            RuleFor(donor => donor.BloodGroup)
                .Must(bg => new[] { "AB-Ve", "AB+Ve", "A-Ve", "A+Ve", "B-Ve", "B+Ve", "Oh-Ve", "Oh+Ve", "O-Ve", "O+Ve" }.Contains(bg))
                .WithMessage("Blood group must be a valid value.");
        }

    }
}
