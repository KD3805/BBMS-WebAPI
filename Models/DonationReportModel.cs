namespace BBMS_WebAPI.Models
{
    public class DonationReportModel
    {
        public int DonorID { get; set; }
        public string Status { get; set; }
        public int TotalDonation { get; set; }
        public int TotalBloodDonated { get; set; }
    }
}
