namespace BBMS_WebAPI.Models
{
    public class RecipientRequestReportModel
    {
        public int RecipientID { get; set; }
        public string Status { get; set; }
        public int TotalRequest { get; set; }
        public int TotalBloodRequested { get; set; }
    }
}
