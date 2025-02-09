using FluentValidation;
using BBMS_WebAPI.Models;
using System;

namespace BBMS_WebAPI.Validators
{
    public class BloodRequestValidator : AbstractValidator<BloodRequestModel>
    {
        public BloodRequestValidator()
        {
            // Validate RecipientName
            RuleFor(request => request.RecipientName)
                .NotNull()
                .NotEmpty().WithMessage("Recipient name is required.")
                .MaximumLength(100).WithMessage("Recipient name cannot exceed 100 characters.");

            // Validate BloodGroupName
            RuleFor(request => request.BloodGroupName)
                .NotNull()
                .NotEmpty().WithMessage("Blood group is required.");

            // Validate Reason
            RuleFor(request => request.Reason)
                .NotNull()
                .NotEmpty().WithMessage("Reason is required.");

            // Validate Quantity
            RuleFor(request => request.Quantity)
                .NotNull()
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            // Validate Status
            RuleFor(request => request.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => new[] { "Pending", "Approved", "Rejected" }.Contains(status))
                .WithMessage("Status must be 'Pending', 'Approved', or 'Rejected'.");

            // Validate Reason
            RuleFor(request => request.Reason)
                .MaximumLength(255).WithMessage("Reason description cannot exceed 255 characters.")
                .When(request => !string.IsNullOrEmpty(request.Reason));
        }
    }

    public class BloodRequestUpdateStatusValidator : AbstractValidator<BloodRequestUpdateStatusModel>
    {
        public BloodRequestUpdateStatusValidator()
        {
            // Validate RequestID
            RuleFor(update => update.RequestID)
                .GreaterThan(0).WithMessage("Request ID must be a positive number.");

            // Validate NewStatus
            RuleFor(update => update.NewStatus)
                .NotEmpty().WithMessage("New status is required.")
                .Must(status => new[] { "Approved", "Rejected" }.Contains(status))
                .WithMessage("New status must be 'Approved' or 'Rejected'.");
        }
    }

    public class BloodRequestDropDownValidator : AbstractValidator<BloodRequestDropDownModel>
    {
        public BloodRequestDropDownValidator()
        {
            // Validate RequestID
            RuleFor(dropdown => dropdown.RequestID)
                .GreaterThan(0).WithMessage("Request ID must be a positive number.");

            // Validate RecipientName
            RuleFor(dropdown => dropdown.RecipientName)
                .NotEmpty().WithMessage("Recipient name is required.")
                .MaximumLength(100).WithMessage("Recipient name cannot exceed 100 characters.");

            // Validate BloodGroupName
            RuleFor(dropdown => dropdown.BloodGroupName)
                .NotEmpty().WithMessage("Blood group name is required.");
        }
    }

    public class RecipientRequestReportValidator : AbstractValidator<RecipientRequestReportModel>
    {
        public RecipientRequestReportValidator()
        {
            // Validate RecipientID
            RuleFor(report => report.RecipientID)
                .GreaterThan(0).WithMessage("Recipient ID must be a positive number.");
            // Validate Status
            RuleFor(report => report.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => new[] { "Pending", "Approved", "Rejected" }.Contains(status))
                .WithMessage("Status must be 'Pending', 'Approved', or 'Rejected'.");
            // Validate TotalRequest
            RuleFor(report => report.TotalRequest)
                .GreaterThanOrEqualTo(0).WithMessage("Total request must be a non-negative number.");
            // Validate TotalBloodRequested
            RuleFor(report => report.TotalBloodRequested)
                .GreaterThanOrEqualTo(0).WithMessage("Total blood requested must be a non-negative number.");
        }
    }
}
