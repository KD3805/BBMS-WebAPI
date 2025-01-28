using FluentValidation;
using BBMS_WebAPI.Models;

namespace BBMS_WebAPI.Validators
{
    public class DonationValidator : AbstractValidator<DonationModel>
    {
        public DonationValidator()
        {
            // Validate DonorName
            RuleFor(donation => donation.DonorName)
                .NotNull()
                .NotEmpty().WithMessage("Donor name is required.")
                .MaximumLength(100).WithMessage("Donor name cannot exceed 100 characters.");

            // Validate BloodGroupName
            RuleFor(donation => donation.BloodGroupName)
                .NotNull()
                .NotEmpty().WithMessage("Blood group is required.");

            // Validate Quantity
            RuleFor(donation => donation.Quantity)
                .NotNull()
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            // Validate Weight
            RuleFor(donation => donation.Weight)
                .NotNull()
                .GreaterThan(0).WithMessage("Weight must be greater than 0.");

            // Validate LastDonationDate
            RuleFor(donation => donation.LastDonationDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Last donation date cannot be in the future.")
                .When(donation => donation.LastDonationDate.HasValue);

            // Validate Disease
            RuleFor(donation => donation.Disease)
                .MaximumLength(255).WithMessage("Disease description cannot exceed 255 characters.")
                .When(donation => !string.IsNullOrEmpty(donation.Disease));

            // Validate IsEligible
            RuleFor(donation => donation.IsEligible)
                .NotNull().WithMessage("Eligibility status is required.");

            // Validate Status
            RuleFor(donation => donation.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => new[] { "Pending", "Approved", "Rejected" }.Contains(status))
                .WithMessage("Status must be 'Pending', 'Approved', or 'Rejected'.");

            // Validate CertificatePath
            RuleFor(donation => donation.CertificatePath)
                .MaximumLength(255).WithMessage("Certificate path cannot exceed 255 characters.")
                .When(donation => !string.IsNullOrEmpty(donation.CertificatePath));
        }
    }

    public class DonationUpdateStatusValidator : AbstractValidator<DonationUpdateStatusModel>
    {
        public DonationUpdateStatusValidator()
        {
            // Validate DonationID
            RuleFor(update => update.DonationID)
                .GreaterThan(0).WithMessage("Donation ID must be a positive number.");

            // Validate NewStatus
            RuleFor(update => update.NewStatus)
                .NotEmpty().WithMessage("New status is required.")
                .Must(status => new[] { "Approved", "Rejected" }.Contains(status))
                .WithMessage("New status must be 'Approved' or 'Rejected'.");
        }
    }

    public class DonationDropDownValidator : AbstractValidator<DonationDropDownModel>
    {
        public DonationDropDownValidator()
        {
            // Validate DonationID
            RuleFor(dropdown => dropdown.DonationID)
                .GreaterThan(0).WithMessage("Donation ID must be a positive number.");

            // Validate DonorName
            RuleFor(dropdown => dropdown.DonorName)
                .NotEmpty().WithMessage("Donor name is required.")
                .MaximumLength(100).WithMessage("Donor name cannot exceed 100 characters.");

            // Validate BloodGroupName
            RuleFor(dropdown => dropdown.BloodGroupName)
                .NotEmpty().WithMessage("Blood group name is required.");
        }
    }
}
