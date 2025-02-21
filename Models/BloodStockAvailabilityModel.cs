namespace BBMS_WebAPI.Models
{
    public class BloodAvailabilityViewModel
    {
        public BloodStockDetailsModel StockDetails { get; set; }
        public List<BloodDonorModel> Donors { get; set; }
        public string AvailabilityMessage { get; set; }
    }

    public class BloodDonorModel
    {
        public int DonorID { get; set; }
        public string DonorName { get; set; }
        public string DonorPhone { get; set; }
        public string DonorEmail { get; set; }
    }

    public class BloodStockDetailsModel
    {
        public string BloodGroupName { get; set; }
        public int TotalBloodStock { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int TotalDonors { get; set; }
    }
}
