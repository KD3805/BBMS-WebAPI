using BBMS_WebAPI.Models;
using BBMS_WebAPI.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BBMS_WebAPI.Data
{
    public class BloodRequestRepository
    {
        private readonly string _connectionString;

        public BloodRequestRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        #region GetAll
        public List<BloodRequestModel> GetAll()
        {
            var requests = new List<BloodRequestModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_BloodRequest_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        requests.Add(new BloodRequestModel
                        {
                            RequestID = Convert.ToInt32(reader["RequestID"]),
                            RecipientName = reader["RecipientName"].ToString(),
                            BloodGroupName = reader["BloodGroupName"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Status = reader["Status"].ToString(),
                            Reason = reader["Reason"].ToString(),
                            RequestDate = Convert.ToDateTime(reader["RequestDate"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                        });
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception($"{ex.Message}");
                }
            }
            return requests;
        }
        #endregion


        #region GetById
        public BloodRequestModel GetById(int requestId)
        {
            BloodRequestModel request = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_BloodRequest_SelectByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                try
                {
                    cmd.Parameters.AddWithValue("@RequestID", requestId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        request = new BloodRequestModel
                        {
                            RequestID = Convert.ToInt32(reader["RequestID"]),
                            RecipientName = reader["RecipientName"].ToString(),
                            BloodGroupName = reader["BloodGroupName"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Status = reader["Status"].ToString(),
                            Reason = reader["Reason"].ToString(),
                            RequestDate = Convert.ToDateTime(reader["RequestDate"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                        };
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception($"{ex.Message}");
                }
            }
            return request;
        }
        #endregion


        #region Get Request History By RecipientID
        public List<BloodRequestModel> GetBloodRequestHistoryByRecipientID(int recipientID)
        {
            var requestHistory = new List<BloodRequestModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_RecipientRequestHistory_SelectByRecipientID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                try
                {
                    cmd.Parameters.AddWithValue("@RecipientID", recipientID);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        requestHistory.Add(new BloodRequestModel
                        {
                            RequestID = Convert.ToInt32(reader["RequestID"]),
                            RecipientName = reader["RecipientName"].ToString(),
                            BloodGroupName = reader["BloodGroupName"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Status = reader["Status"].ToString(),
                            Reason = reader["Reason"].ToString(),
                            RequestDate = Convert.ToDateTime(reader["RequestDate"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                        });
                    };
                }
                catch (SqlException ex)
{
                    throw new Exception($"{ex.Message}");
                }
            }

            return requestHistory;
        }
        #endregion


        #region Get Recipient Request Report by RecipientID
        public List<RecipientRequestReportModel> GetRecipientRequestReportByRecipientID(int recipientID)
        {
            var requestHistory = new List<RecipientRequestReportModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_RecipientRequest_SelectRequestReport", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@RecipientID", recipientID);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    requestHistory.Add(new RecipientRequestReportModel
                    {
                        RecipientID = Convert.ToInt32(reader["RecipientID"]),
                        Status = reader["Status"].ToString(),
                        TotalBloodRequested = Convert.ToInt32(reader["TotalBloodRequested"]),
                        TotalRequest = Convert.ToInt32(reader["TotalRequest"]),
                    });
                }
            }

            return requestHistory;
        }
        #endregion


        #region Insert
        public bool Insert(BloodRequestModel bloodRequestModel)
        {
            var recipientID = RecipientMapper.GetRecipientID(bloodRequestModel.RecipientName);
            if (recipientID == null)
                throw new ArgumentException($"Invalid Recipient Name: {bloodRequestModel.RecipientName}");

            var bloodGroupID = BloodGroupMapper.GetBloodGroupID(bloodRequestModel.BloodGroupName);
            if (bloodGroupID == null)
                throw new ArgumentException($"Invalid Blood Group Name: {bloodRequestModel.BloodGroupName}");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_BloodRequest_Insert", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@RecipientID", recipientID.Value);
                cmd.Parameters.AddWithValue("@BloodGroupID", bloodGroupID.Value);
                cmd.Parameters.AddWithValue("@Quantity", bloodRequestModel.Quantity);
                cmd.Parameters.AddWithValue("@Reason", bloodRequestModel.Reason);
                cmd.Parameters.AddWithValue("@Status", bloodRequestModel.Status);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception($"{ex.Message}");
                }
            }
        }
        #endregion


        #region Update
        public bool Update(BloodRequestModel bloodRequestModel)
        {
            var recipientID = RecipientMapper.GetRecipientID(bloodRequestModel.RecipientName);
            if (recipientID == null)
            {
                throw new ArgumentException("Invalid Recipient Name");
            }

            var bloodGroupID = BloodGroupMapper.GetBloodGroupID(bloodRequestModel.BloodGroupName);
            if (bloodGroupID == null)
            {
                throw new ArgumentException("Invalid Blood Group Name");
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_BloodRequest_UpdateByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@RequestID", bloodRequestModel.RequestID);
                cmd.Parameters.AddWithValue("@RecipientID", recipientID.Value);
                cmd.Parameters.AddWithValue("@BloodGroupID", bloodGroupID.Value);
                cmd.Parameters.AddWithValue("@Quantity", bloodRequestModel.Quantity);
                cmd.Parameters.AddWithValue("@Reason", bloodRequestModel.Reason);
                cmd.Parameters.AddWithValue("@Status", bloodRequestModel.Status);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
{
                    throw new Exception($"{ex.Message}");
                }
            }
        }
        #endregion


        #region Update Status and Stock
        public bool UpdateStatusAndStock(BloodRequestUpdateStatusModel updateStatusModel)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_BloodRequest_UpdateStatusAndStock", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@RequestID", updateStatusModel.RequestID);
                cmd.Parameters.AddWithValue("@NewStatus", updateStatusModel.NewStatus);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
{
                    throw new Exception($"{ex.Message}");
                }
            }
        }
        #endregion


        #region Delete
        public bool Delete(int bloodRequestId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_BloodRequest_DeleteByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@RequestID", bloodRequestId);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception($"{ex.Message}");
                }
            }
        }
        #endregion
    }
}
