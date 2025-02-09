namespace BBMS_WebAPI.Models
{
    public class BloodStockModel
    {
        public int StockID { get; set; }
        public string BloodGroupName { get; set; }
        public int Quantity { get; set; }
        // Allow LastUpdated to be null so that it isn’t required in the update payload.
        public DateTime? LastUpdated { get; set; }
    }
}
