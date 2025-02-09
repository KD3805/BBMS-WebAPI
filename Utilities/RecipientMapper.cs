using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace BBMS_WebAPI.Utilities
{
    public static class RecipientMapper
    {
        private static readonly Dictionary<string, int> RecipientMapping = new();

        // Populate mapping from the database (called once during application startup)
        public static void LoadMapping(IConfiguration configuration)
        {
            using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ConnectionString")))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT RecipientID, Name FROM Recipient", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var recipientID = Convert.ToInt32(reader["RecipientID"]);
                    var recipientName = reader["Name"].ToString();
                    if (!string.IsNullOrEmpty(recipientName))
                        RecipientMapping[recipientName] = recipientID;
                }
            }
        }

        // Dynamically update the mapping
        public static void AddToMapping(string recipientName, int recipientID)
        {
            if (!string.IsNullOrEmpty(recipientName))
            {
                RecipientMapping[recipientName] = recipientID;
            }
        }

        public static int? GetRecipientID(string recipientName)
        {
            return RecipientMapping.TryGetValue(recipientName, out var recipientID) ? recipientID : null;
        }

        public static string GetRecipientName(int recipientID)
        {
            return RecipientMapping.FirstOrDefault(x => x.Value == recipientID).Key;
        }
    }
}
