using BBMS_WebAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;
using Dapper;

namespace BBMS_WebAPI.Data
{
    public class AdminRepository
    {
        private readonly string _connectionString;

        public AdminRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        #region GetAll
        public List<AdminModel> GetAll()
        {
            var admins = new List<AdminModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Admin_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    admins.Add(new AdminModel
                    {
                        AdminID = Convert.ToInt32(reader["AdminID"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Role = reader["Role"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                    });
                }
            }

            return admins;
        }
        #endregion

        #region GetById
        public AdminModel GetById(int adminId)
        {
            AdminModel admin = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Admin_SelectByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@AdminID", adminId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    admin = new AdminModel
                    {
                        AdminID = Convert.ToInt32(reader["AdminID"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Role = reader["Role"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                    };
                }
            }

            return admin;
        }
        #endregion

        #region GetByEmail
        public AdminModel GetByEmail(string email)
        {
            AdminModel admin = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Admin_SelectByEmail", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Email", email);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    admin = new AdminModel
                    {
                        AdminID = Convert.ToInt32(reader["AdminID"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Role = reader["Role"].ToString(),
                        Password = reader["Password"].ToString() // This is hashed
                    };
                }
            }

            return admin;
        }
        #endregion


        #region Insert
        public bool Insert(AdminModel adminModel)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Admin_Insert", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Name", adminModel.Name);
                cmd.Parameters.AddWithValue("@Email", adminModel.Email);
                cmd.Parameters.AddWithValue("@Phone", adminModel.Phone);

                // Hash the password before storing it
                //var hashedPassword = BCrypt.Net.BCrypt.HashPassword(adminModel.Password);
                cmd.Parameters.AddWithValue("@Password", adminModel.Password);
                cmd.Parameters.AddWithValue("@Role", adminModel.Role);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Update
        public bool Update(AdminModel adminModel)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Admin_Update", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@AdminID", adminModel.AdminID);
                cmd.Parameters.AddWithValue("@Name", adminModel.Name);
                cmd.Parameters.AddWithValue("@Email", adminModel.Email);
                cmd.Parameters.AddWithValue("@Phone", adminModel.Phone);
                cmd.Parameters.AddWithValue("@Role", adminModel.Role);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Delete
        public bool Delete(int adminId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Admin_Delete", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@AdminID", adminId);
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Login
        public AdminModel Login(string email, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Admin_SelectByEmail", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Email", email);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var hashedPassword = reader["Password"]?.ToString();
                    if (hashedPassword != null && BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                    {
                        return new AdminModel
                        {
                            AdminID = Convert.ToInt32(reader["AdminID"]),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Role = reader["Role"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                                        ? (DateTime?)null
                                        : Convert.ToDateTime(reader["UpdatedAt"])
                        };
                    }
                }
            }
            return null; // Return null if authentication fails
        }
        #endregion


        #region Admin Dashboard Report Counts
        public async Task<AdminDashboardReportModel> GetDashboardReportCountsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Execute the stored procedure and get multiple result sets.
                var results = await connection.QueryMultipleAsync("PR_Admin_SelectDonorRecipientReport", commandType: CommandType.StoredProcedure);

                // Read the first result set (donations data)
                var donationData = await results.ReadSingleAsync<DonationReportDTO>();
                // Read the second result set (blood requests data)
                var requestData = await results.ReadSingleAsync<BloodRequestDTO>();

                // Map the two result sets into a single DTO to return
                var report = new AdminDashboardReportModel
                {
                    // Donations info
                    TotalDonors = donationData.TotalDonors,
                    TotalDonations = donationData.TotalDonations,
                    PendingDonations = donationData.PendingDonations,
                    AcceptedDonations = donationData.AcceptedDonations,
                    RejectedDonations = donationData.RejectedDonations,

                    // Blood Requests info
                    TotalRecipients = requestData.TotalRecipients,
                    TotalBloodRequests = requestData.TotalBloodRequests,
                    PendingRequests = requestData.PendingRequests,
                    AcceptedRequests = requestData.AcceptedRequests,
                    RejectedRequests = requestData.RejectedRequests
                };

                return report;
            }
        }

        #endregion
    }
}
