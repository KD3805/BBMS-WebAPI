using BBMS_WebAPI.Models;
using BBMS_WebAPI.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BBMS_WebAPI.Data
{
    public class RecipientRepository
    {
        private readonly string _connectionString;

        public RecipientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        #region GetAll
        public List<RecipientModel> GetAll()
        {
            var recipients = new List<RecipientModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Recipient_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    recipients.Add(new RecipientModel
                    {
                        RecipientID = Convert.ToInt32(reader["RecipientID"]),
                        Name = reader["Name"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        Age = Convert.ToInt32(reader["Age"]),
                        Gender = reader["Gender"].ToString(),
                        BloodGroupName = reader["BloodGroupName"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Email = reader["Email"].ToString(),
                        Address = reader["Address"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                    });
                }
            }
            return recipients;
        }
        #endregion

        #region GetById
        public RecipientModel GetById(int recipientId)
        {
            RecipientModel recipient = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Recipient_SelectByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@RecipientID", recipientId);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    recipient = new RecipientModel
                    {
                        RecipientID = Convert.ToInt32(reader["RecipientID"]),
                        Name = reader["Name"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        Age = Convert.ToInt32(reader["Age"]),
                        Gender = reader["Gender"].ToString(),
                        BloodGroupName = reader["BloodGroupName"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Email = reader["Email"].ToString(),
                        Address = reader["Address"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                    };
                }
            }
            return recipient;
        }
        #endregion

        #region GetByEmail
        public RecipientModel GetByEmail(string email)
        {
            RecipientModel recipient = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Recipient_SelectByEmail", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Email", email);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    recipient = new RecipientModel
                    {
                        RecipientID = Convert.ToInt32(reader["RecipientID"]),
                        Name = reader["Name"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        Age = Convert.ToInt32(reader["Age"]),
                        Gender = reader["Gender"].ToString(),
                        BloodGroupName = reader["BloodGroupName"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Email = reader["Email"].ToString(),
                        Address = reader["Address"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                    };
                }
            }
            return recipient;
        }
        #endregion

        #region Insert
        public bool Insert(RecipientModel recipientModel)
        {
            int? bloodGroupID = BloodGroupMapper.GetBloodGroupID(recipientModel.BloodGroupName);
            if (bloodGroupID == null)
                throw new ArgumentException("Invalid Blood Group Name");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Recipient_Insert", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Name", recipientModel.Name);
                cmd.Parameters.AddWithValue("@DOB", recipientModel.DOB);
                cmd.Parameters.AddWithValue("@Age", recipientModel.Age);
                cmd.Parameters.AddWithValue("@Gender", recipientModel.Gender);
                cmd.Parameters.AddWithValue("@BloodGroupID", bloodGroupID.Value);
                cmd.Parameters.AddWithValue("@Phone", recipientModel.Phone);
                cmd.Parameters.AddWithValue("@Email", recipientModel.Email);
                cmd.Parameters.AddWithValue("@Address", recipientModel.Address);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Update
        public bool Update(RecipientModel recipientModel)
        {
            int? bloodGroupID = BloodGroupMapper.GetBloodGroupID(recipientModel.BloodGroupName);
            if (bloodGroupID == null)
                throw new ArgumentException("Invalid Blood Group Name");

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Recipient_UpdateByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@RecipientID", recipientModel.RecipientID);
                cmd.Parameters.AddWithValue("@Name", recipientModel.Name);
                cmd.Parameters.AddWithValue("@DOB", recipientModel.DOB);
                cmd.Parameters.AddWithValue("@Age", recipientModel.Age);
                cmd.Parameters.AddWithValue("@Gender", recipientModel.Gender);
                cmd.Parameters.AddWithValue("@BloodGroupID", bloodGroupID.Value);
                cmd.Parameters.AddWithValue("@Phone", recipientModel.Phone);
                cmd.Parameters.AddWithValue("@Email", recipientModel.Email);
                cmd.Parameters.AddWithValue("@Address", recipientModel.Address);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Delete
        public bool Delete(int recipientId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Recipient_DeleteByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@RecipientID", recipientId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region RecipientDropDown
        public List<RecipientDropDownModel> GetRecipientDropDown()
        {
            var recipients = new List<RecipientDropDownModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Recipient_DropDown", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    recipients.Add(new RecipientDropDownModel
                    {
                        RecipientID = Convert.ToInt32(reader["RecipientID"]),
                        Name = reader["Name"].ToString()
                    });
                }
            }
            return recipients;
        }
        #endregion
    }
}
