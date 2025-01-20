using FluentValidation;
using BBMS_WebAPI.Models;
using BBMS_WebAPI.Utilities;

namespace BBMS_WebAPI.Validators
{
    public class DonorValidator : AbstractValidator<DonorModel>
    {
        public DonorValidator()
        {
            // Rule 1: All required fields
            RuleFor(donor => donor.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(donor => donor.Email)
                .NotEmpty()
                .WithMessage("Email is required.");

            RuleFor(donor => donor.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required.");

            RuleFor(donor => donor.Gender)
                .NotEmpty()
                .WithMessage("Gender is required.");

            RuleFor(donor => donor.BloodGroupName)
                .NotEmpty()
                .WithMessage("Blood group is required.");

            RuleFor(donor => donor.DOB)
                .NotEmpty()
                .WithMessage("Date of Birth is required.");

            // Rule 2: Donor must be at least 18 years old based on DOB
            RuleFor(donor => donor.DOB)
                .Must(dob => CalculateAge(dob) >= 18)
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
            RuleFor(donor => donor.BloodGroupName)
                .Must(bg => BloodGroupMapper.GetBloodGroupID(bg) != null)
                .WithMessage("Invalid Blood Group Name.");

        }

        // Helper Method: Calculate age from DOB
        private static int CalculateAge(DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
