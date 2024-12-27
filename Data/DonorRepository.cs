using BBMS_WebAPI.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace BBMS_WebAPI.Data
{
    public class DonorRepository
    {
        private readonly string _connectionString;

        public DonorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        #region GetAll
        public List<DonorModel> GetAll()
        {
            var donors = new List<DonorModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donor_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    donors.Add(new DonorModel
                    {
                        DonorID = Convert.ToInt32(reader["DonorID"]),
                        Name = reader["Name"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        Age = Convert.ToInt32(reader["Age"]),
                        Gender = reader["Gender"].ToString(),
                        BloodGroup = reader["BloodGroup"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Email = reader["Email"].ToString(),
                        Address = reader["Address"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                    });
                }
            }
            return donors;
        }
        #endregion

        #region GetById
        public DonorModel GetById(int donorId)
        {
            DonorModel donor = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donor_SelectByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DonorID", donorId);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    donor = new DonorModel
                    {
                        DonorID = Convert.ToInt32(reader["DonorID"]),
                        Name = reader["Name"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        Age = Convert.ToInt32(reader["Age"]),
                        Gender = reader["Gender"].ToString(),
                        BloodGroup = reader["BloodGroup"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Email = reader["Email"].ToString(),
                        Address = reader["Address"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                    };
                }
            }
            return donor;
        }
        #endregion

        #region GetByEmail
        public DonorModel GetByEmail(string email)
        {
            DonorModel donor = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donor_SelectByEmail", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Email", email);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    donor = new DonorModel
                    {
                        DonorID = Convert.ToInt32(reader["DonorID"]),
                        Name = reader["Name"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        Age = Convert.ToInt32(reader["Age"]),
                        Gender = reader["Gender"].ToString(),
                        BloodGroup = reader["BloodGroup"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Email = reader["Email"].ToString(),
                        Address = reader["Address"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    };
                }
            }
            return donor;
        }
        #endregion

        #region Insert
        public bool Insert(DonorModel donorModel)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donor_Insert", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Name", donorModel.Name);
                cmd.Parameters.AddWithValue("@DOB", donorModel.DOB);
                cmd.Parameters.AddWithValue("@Age", donorModel.Age);
                cmd.Parameters.AddWithValue("@Gender", donorModel.Gender);
                cmd.Parameters.AddWithValue("@BloodGroup", donorModel.BloodGroup);
                cmd.Parameters.AddWithValue("@Phone", donorModel.Phone);
                cmd.Parameters.AddWithValue("@Email", donorModel.Email);
                cmd.Parameters.AddWithValue("@Address", donorModel.Address);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Update
        public bool Update(DonorModel donorModel)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donor_UpdateByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DonorID", donorModel.DonorID);
                cmd.Parameters.AddWithValue("@Name", donorModel.Name);
                cmd.Parameters.AddWithValue("@DOB", donorModel.DOB);
                cmd.Parameters.AddWithValue("@Age", donorModel.Age);
                cmd.Parameters.AddWithValue("@Gender", donorModel.Gender);
                cmd.Parameters.AddWithValue("@BloodGroup", donorModel.BloodGroup);
                cmd.Parameters.AddWithValue("@Phone", donorModel.Phone);
                cmd.Parameters.AddWithValue("@Email", donorModel.Email);
                cmd.Parameters.AddWithValue("@Address", donorModel.Address);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Delete
        public bool Delete(int donorId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donor_DeleteByPK", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@DonorID", donorId);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region DonorDropDown
        public List<DonorDropDownModel> GetDonorDropDown()
        {
            var donors = new List<DonorDropDownModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PR_Donor_DropDown", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    donors.Add(new DonorDropDownModel
                    {
                        DonorID = Convert.ToInt32(reader["DonorID"]),
                        Name = reader["Name"].ToString()
                    });
                }
            }
            return donors;
        }
        #endregion
    }
}
