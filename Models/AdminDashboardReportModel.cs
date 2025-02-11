namespace BBMS_WebAPI.Models
{
    public class AdminDashboardReportModel
    {
        // Donations Data
        public int TotalDonors { get; set; }
        public int TotalDonations { get; set; }
        public int PendingDonations { get; set; }
        public int AcceptedDonations { get; set; }
        public int RejectedDonations { get; set; }

        // Blood Requests Data
        public int TotalRecipients { get; set; }
        public int TotalBloodRequests { get; set; }
        public int PendingRequests { get; set; }
        public int AcceptedRequests { get; set; }
        public int RejectedRequests { get; set; }
    }


    public class DonationReportDTO
    {
        public int TotalDonors { get; set; }
        public int TotalDonations { get; set; }
        public int PendingDonations { get; set; }
        public int AcceptedDonations { get; set; }
        public int RejectedDonations { get; set; }
    }


    public class BloodRequestDTO
    {
        public int TotalRecipients { get; set; }
        public int TotalBloodRequests { get; set; }
        public int PendingRequests { get; set; }
        public int AcceptedRequests { get; set; }
        public int RejectedRequests { get; set; }
    }

}
