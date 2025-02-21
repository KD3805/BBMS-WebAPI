using BBMS_WebAPI.Models;
using BBMS_WebAPI.Utilities;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace BBMS_WebAPI.Data
{
    public class BloodStockRepository
    {
        private readonly string _connectionString;

        public BloodStockRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        #region GetAll
        public List<BloodStockModel> GetAll()
        {
            var bloodStocks = new List<BloodStockModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("PR_BloodStock_SelectAll", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bloodStocks.Add(new BloodStockModel
                                {
                                    StockID = Convert.ToInt32(reader["StockID"]),
                                    BloodGroupName = reader["BloodGroupName"].ToString(),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    LastUpdated = reader["LastUpdated"] as DateTime?
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{ex.Message}");
                    }
                }
            }
            return bloodStocks;
        }
        #endregion

        #region GetById
        public BloodStockModel GetById(int stockId)
        {
            BloodStockModel bloodStock = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("PR_BloodStock_SelectByPK", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StockID", stockId);
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bloodStock = new BloodStockModel
                                {
                                    StockID = Convert.ToInt32(reader["StockID"]),
                                    BloodGroupName = reader["BloodGroupName"].ToString(),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    LastUpdated = reader["LastUpdated"] as DateTime?
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{ex.Message}");
                    }
                }
            }
            return bloodStock;
        }
        #endregion

        #region Insert
        public bool Insert(BloodStockModel bloodStockModel)
        {
            var bloodGroupID = BloodGroupMapper.GetBloodGroupID(bloodStockModel.BloodGroupName);
            if (bloodGroupID == null)
                throw new ArgumentException($"Invalid Blood Group Name: {bloodStockModel.BloodGroupName}");

            int result = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("PR_BloodStock_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BloodGroupID", bloodGroupID.Value);
                    cmd.Parameters.AddWithValue("@Quantity", bloodStockModel.Quantity);
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{ex.Message}");
                    }
                }
            }
            return result > 0;
        }
        #endregion

        #region Update
        public bool Update(BloodStockModel bloodStockModel)
        {
            var bloodGroupID = BloodGroupMapper.GetBloodGroupID(bloodStockModel.BloodGroupName);
            if (bloodGroupID == null)
                throw new ArgumentException($"Invalid Blood Group Name: {bloodStockModel.BloodGroupName}");

            int result = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("PR_BloodStock_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StockID", bloodStockModel.StockID);
                    cmd.Parameters.AddWithValue("@BloodGroupID", bloodGroupID.Value);
                    cmd.Parameters.AddWithValue("@QuantityChange", bloodStockModel.Quantity);
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{ex.Message}");
                    }
                }
            }
            return result > 0;
        }
        #endregion

        #region Delete
        public bool Delete(int stockId)
        {
            int result = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("PR_BloodStock_Delete", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StockID", stockId);
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{ex.Message}");
                    }
                }
            }
            return result > 0;
        }
        #endregion

        #region GetBloodStockAvailability
        public async Task<BloodAvailabilityViewModel> GetBloodAvailabilityAsync(string bloodGroupName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(
                    "PR_StockAndDonors_FindByBloodGroup",
                    new { BloodGroupName = bloodGroupName },
                    commandType: CommandType.StoredProcedure))
                {
                    // First result set: List of donors.
                    var donors = (await multi.ReadAsync<BloodDonorModel>()).ToList();

                    // Second result set: Blood stock details.
                    var stockDetails = await multi.ReadSingleAsync<BloodStockDetailsModel>();

                    // Build an availability message based on blood stock quantity.
                    string availabilityMessage = stockDetails.TotalBloodStock == 0
                        ? "Blood not available or out of stock."
                        : "Blood available.";

                    return new BloodAvailabilityViewModel
                    {
                        StockDetails = stockDetails,
                        Donors = donors,
                        AvailabilityMessage = availabilityMessage
                    };
                }
            }
        }
        #endregion
    }
}
