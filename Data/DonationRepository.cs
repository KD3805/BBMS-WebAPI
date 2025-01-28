using BBMS_WebAPI.Models;
using BBMS_WebAPI.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BBMS_WebAPI.Data
{
    public class DonationRepository
    {
        private readonly string _connectionString;

        public DonationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        #region GetAll
        public List<DonationModel> GetAll()
        {
            var donations = new List<DonationModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donation_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    donations.Add(new DonationModel
                    {
                        DonationID = Convert.ToInt32(reader["DonationID"]),
                        DonorName = reader["DonorName"].ToString(), // Using DonorName from join
                        BloodGroupName = reader["BloodGroupName"].ToString(), // Using BloodGroupName from join
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        Weight = Convert.ToInt32(reader["Weight"]),
                        LastDonationDate = reader.IsDBNull(reader.GetOrdinal("LastDonationDate")) ? (DateTime?)null : Convert.ToDateTime(reader["LastDonationDate"]),
                        Disease = reader["Disease"]?.ToString(),
                        IsEligible = Convert.ToBoolean(reader["IsEligible"]),
                        Status = reader["Status"].ToString(),
                        DateOfDonation = Convert.ToDateTime(reader["DateOfDonation"]),
                        CertificatePath = reader["CertificatePath"]?.ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                    });
                }
            }
            return donations;
        }
        #endregion


        #region Get Only Pending
        public List<DonationModel> GetOnlyPending()
        {
            var donations = new List<DonationModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donation_SelectPending", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    donations.Add(new DonationModel
                    {
                        DonationID = Convert.ToInt32(reader["DonationID"]),
                        DonorName = reader["DonorName"].ToString(), // Using DonorName from join
                        BloodGroupName = reader["BloodGroupName"].ToString(), // Using BloodGroupName from join
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        Weight = Convert.ToInt32(reader["Weight"]),
                        LastDonationDate = reader.IsDBNull(reader.GetOrdinal("LastDonationDate")) ? (DateTime?)null : Convert.ToDateTime(reader["LastDonationDate"]),
                        Disease = reader["Disease"]?.ToString(),
                        IsEligible = Convert.ToBoolean(reader["IsEligible"]),
                        Status = reader["Status"].ToString(),
                        DateOfDonation = Convert.ToDateTime(reader["DateOfDonation"]),
                        CertificatePath = reader["CertificatePath"]?.ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                    });
                }
            }
            return donations;
        }
        #endregion


        #region GetById
        public DonationModel GetById(int donationId)
        {
            DonationModel donation = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donation_SelectByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DonationID", donationId);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    donation = new DonationModel
                    {
                        DonationID = Convert.ToInt32(reader["DonationID"]),
                        DonorName = reader["DonorName"].ToString(),
                        BloodGroupName = reader["BloodGroupName"].ToString(),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        Weight = Convert.ToInt32(reader["Weight"]),
                        LastDonationDate = reader.IsDBNull(reader.GetOrdinal("LastDonationDate")) ? (DateTime?)null : Convert.ToDateTime(reader["LastDonationDate"]),
                        Disease = reader["Disease"]?.ToString(),
                        IsEligible = Convert.ToBoolean(reader["IsEligible"]),
                        Status = reader["Status"].ToString(),
                        DateOfDonation = Convert.ToDateTime(reader["DateOfDonation"]),
                        CertificatePath = reader["CertificatePath"]?.ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                    };
                }
            }
            return donation;
        }
        #endregion


        #region Determine Eligibility
        public bool DetermineEligibility(string donorName, int weight)
        {
            var donorID = DonorMapper.GetDonorID(donorName);
            if (donorID == null)
                throw new ArgumentException($"Invalid Donor Name: {donorName}");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donor_CheckEligibility", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DonorID", donorID.Value);
                cmd.Parameters.AddWithValue("@Weight", weight);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Ensure the value returned by the SP is valid
                    if (!reader.IsDBNull(reader.GetOrdinal("IsEligible")))
                        return Convert.ToBoolean(reader["IsEligible"]);
                }
            }

            // Default to not eligible if no result is returned
            return false;
        }
        #endregion


        #region Get Donation History By DonorID
        public List<DonationModel> GetDonationHistoryByDonorID(int donorID)
        {
            var donationHistory = new List<DonationModel>();    

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_DonorDonationHistory_SelectByDonorID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DonorID", donorID);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    donationHistory.Add(new DonationModel
                    {
                        DonationID = Convert.ToInt32(reader["DonationID"]),
                        DonorName = reader["DonorName"].ToString(),
                        BloodGroupName = reader["BloodGroupName"].ToString(),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        Weight = Convert.ToInt32(reader["Weight"]),
                        LastDonationDate = reader.IsDBNull(reader.GetOrdinal("LastDonationDate")) ? (DateTime?)null : Convert.ToDateTime(reader["LastDonationDate"]),
                        Disease = reader["Disease"]?.ToString(),
                        IsEligible = Convert.ToBoolean(reader["IsEligible"]),
                        Status = reader["Status"].ToString(),
                        CertificatePath = reader["CertificatePath"]?.ToString(),
                        DateOfDonation = Convert.ToDateTime(reader["DateOfDonation"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                    });
                }
            }

            return donationHistory;
        }
        #endregion


        #region Get Pending Donation History By DonorID
        public List<DonationModel> GetPendingDonationHistoryByDonorID(int donorID)
        {
            var donationHistory = new List<DonationModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_DonorPendingDonationHistory_SelectByDonorID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DonorID", donorID);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    donationHistory.Add(new DonationModel
                    {
                        DonationID = Convert.ToInt32(reader["DonationID"]),
                        DonorName = reader["DonorName"].ToString(),
                        BloodGroupName = reader["BloodGroupName"].ToString(),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        Weight = Convert.ToInt32(reader["Weight"]),
                        LastDonationDate = reader.IsDBNull(reader.GetOrdinal("LastDonationDate")) ? (DateTime?)null : Convert.ToDateTime(reader["LastDonationDate"]),
                        Disease = reader["Disease"]?.ToString(),
                        IsEligible = Convert.ToBoolean(reader["IsEligible"]),
                        Status = reader["Status"].ToString(),
                        CertificatePath = reader["CertificatePath"]?.ToString(),
                        DateOfDonation = Convert.ToDateTime(reader["DateOfDonation"]),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                    });
                }
            }

            return donationHistory;
        }
        #endregion


        #region Insert
        public bool Insert(DonationModel donationModel)
        {
            var donorID = DonorMapper.GetDonorID(donationModel.DonorName);
            if (donorID == null)
                throw new ArgumentException($"Invalid Donor Name: {donationModel.DonorName}");

            var bloodGroupID = BloodGroupMapper.GetBloodGroupID(donationModel.BloodGroupName);
            if (bloodGroupID == null)
                throw new ArgumentException($"Invalid Blood Group Name: {donationModel.BloodGroupName}");

            donationModel.IsEligible = DetermineEligibility(donationModel.DonorName, donationModel.Weight);
            if (!donationModel.IsEligible)
                throw new ArgumentException($"Donor {donationModel.DonorName} is not eligible for donation for now. Please refer eligibility conditions.");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donation_Insert", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DonorID", donorID.Value);
                cmd.Parameters.AddWithValue("@BloodGroupID", bloodGroupID.Value);
                cmd.Parameters.AddWithValue("@Quantity", donationModel.Quantity);
                cmd.Parameters.AddWithValue("@Weight", donationModel.Weight);
                cmd.Parameters.AddWithValue("@LastDonationDate", donationModel.LastDonationDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Disease", string.IsNullOrEmpty(donationModel.Disease) ? (object)DBNull.Value : donationModel.Disease);
                cmd.Parameters.AddWithValue("@IsEligible", donationModel.IsEligible);
                cmd.Parameters.AddWithValue("@Status", donationModel.Status);
                cmd.Parameters.AddWithValue("@CertificatePath", string.IsNullOrEmpty(donationModel.CertificatePath) ? (object)DBNull.Value : donationModel.CertificatePath);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion


        #region Update
        public bool Update(DonationModel donationModel)
        {
            var donorID = DonorMapper.GetDonorID(donationModel.DonorName);
            if (donorID == null)
            {
                throw new ArgumentException("Invalid Donor Name");
            }

            var bloodGroupID = BloodGroupMapper.GetBloodGroupID(donationModel.BloodGroupName);
            if (bloodGroupID == null)
            {
                throw new ArgumentException("Invalid Blood Group Name");
            }

            donationModel.IsEligible = DetermineEligibility(donationModel.DonorName, donationModel.Weight);
            if (!donationModel.IsEligible)
            {
                throw new ArgumentException($"Donor {donationModel.DonorName} is not eligible for donation for now. Please refer eligibility conditions.");
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donation_UpdateByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DonationID", donationModel.DonationID);
                cmd.Parameters.AddWithValue("@DonorID", donorID.Value);
                cmd.Parameters.AddWithValue("@BloodGroupID", bloodGroupID.Value);
                cmd.Parameters.AddWithValue("@Quantity", donationModel.Quantity);
                cmd.Parameters.AddWithValue("@Weight", donationModel.Weight);
                cmd.Parameters.AddWithValue("@LastDonationDate", donationModel.LastDonationDate ?? (object)DBNull.Value);
                // Use DBNull.Value only if Disease is null or empty
                cmd.Parameters.AddWithValue("@Disease",
                    !string.IsNullOrWhiteSpace(donationModel.Disease) ? donationModel.Disease.Trim() : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsEligible", donationModel.IsEligible);
                cmd.Parameters.AddWithValue("@Status", donationModel.Status);
                cmd.Parameters.AddWithValue("@CertificatePath", string.IsNullOrEmpty(donationModel.CertificatePath) ? (object)DBNull.Value : donationModel.CertificatePath);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion


        #region Update Status and Stock
        public bool UpdateStatusAndStock(DonationUpdateStatusModel updateStatusModel)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donation_UpdateStatusAndStock", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DonationID", updateStatusModel.DonationID);
                cmd.Parameters.AddWithValue("@NewStatus", updateStatusModel.NewStatus); 

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion


        #region Delete
        public bool Delete(int donationId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donation_DeleteByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DonationID", donationId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion
    }
}
