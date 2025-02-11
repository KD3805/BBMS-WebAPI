using BBMS_WebAPI.Models;
using FluentValidation;

namespace BBMS_WebAPI.Validators
{
    public class AdminDashboardReportValidator : AbstractValidator<AdminDashboardReportModel>
    {
        public AdminDashboardReportValidator() 
        {
            RuleFor(x => x.TotalDonors).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalDonations).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PendingDonations).GreaterThanOrEqualTo(0);
            RuleFor(x => x.AcceptedDonations).GreaterThanOrEqualTo(0);
            RuleFor(x => x.RejectedDonations).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalRecipients).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalBloodRequests).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PendingRequests).GreaterThanOrEqualTo(0);
            RuleFor(x => x.AcceptedRequests).GreaterThanOrEqualTo(0);
            RuleFor(x => x.RejectedRequests).GreaterThanOrEqualTo(0);
        }
    }
}
