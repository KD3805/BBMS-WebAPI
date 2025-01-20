using Microsoft.Data.SqlClient;

namespace BBMS_WebAPI.Utilities
{
    public static class DonorMapper
    {
        private static readonly Dictionary<string, int> DonorMapping = new();

        // Populate mapping from the database (called once during application startup)
        public static void LoadMapping(IConfiguration configuration)
        {
            using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ConnectionString")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DonorID, Name FROM Donor", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var donorID = Convert.ToInt32(reader["DonorID"]);
                    var donorName = reader["Name"].ToString();
                    if (!string.IsNullOrEmpty(donorName))
                        DonorMapping[donorName] = donorID;
                }
            }
        }

        // Dynamically update the mapping
        public static void AddToMapping(string donorName, int donorID)
        {
            if (!string.IsNullOrEmpty(donorName))
            {
                DonorMapping[donorName] = donorID;
            }
        }

        public static int? GetDonorID(string donorName)
        {
            return DonorMapping.TryGetValue(donorName, out var donorID) ? donorID : null;
        }

        public static string GetDonorName(int donorID)
        {
            return DonorMapping.FirstOrDefault(x => x.Value == donorID).Key;
        }
    }
}
