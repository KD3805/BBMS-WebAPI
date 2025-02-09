using BBMS_WebAPI.Data;
using BBMS_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BBMS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodRequestController : ControllerBase
    {
        private readonly BloodRequestRepository _bloodRequestRepository;

        public BloodRequestController(IConfiguration configuration)
        {
            _bloodRequestRepository = new BloodRequestRepository(configuration);
        }

        #region GetAll
        [HttpGet]
        public IActionResult GetAllBloodRequests()
        {
            try
            {
                var bloodRequests = _bloodRequestRepository.GetAll();
                if (bloodRequests == null || bloodRequests.Count == 0)
                    return NotFound("No blood requests found.");

                return Ok(bloodRequests);
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
        }
        #endregion


        #region GetById
        [HttpGet("{id:int}")]
        public IActionResult GetBloodRequestById(int id)
        {
            try
            {
                var bloodRequest = _bloodRequestRepository.GetById(id);
                if (bloodRequest == null)
                    return NotFound($"BloodRequest with ID {id} not found.");

                return Ok(bloodRequest);
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
        }
        #endregion


        #region BloodRequest History By RecipientID
        [HttpGet("History/{recipientID:int}")]
        public IActionResult GetBloodRequestHistoryByRecipientID(int recipientID)
        {
            try
            {
                var bloodRequestHistory = _bloodRequestRepository.GetBloodRequestHistoryByRecipientID(recipientID);
                if (bloodRequestHistory == null || bloodRequestHistory.Count == 0)
                    return NotFound($"No blood request history found for recipient with ID {recipientID}.");

                return Ok(bloodRequestHistory);
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
        }
        #endregion


        #region  Recipient Request Report By RecipientID
        [HttpGet("Report/{recipientID:int}")]
        public IActionResult GetRecipientRequestReportByRecipientID(int recipientID)
        {
            try
            {
                var requestReport = _bloodRequestRepository.GetRecipientRequestReportByRecipientID(recipientID);
                if (requestReport == null)
                    return NotFound($"No request report found for recipient with ID {recipientID}.");

                return Ok(requestReport);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error occurred: {ex.Message}");
            }
        }
        #endregion


        #region Insert
        [HttpPost]
        public IActionResult InsertBloodRequest([FromBody] BloodRequestModel bloodRequestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var isInserted = _bloodRequestRepository.Insert(bloodRequestModel);
                if (isInserted)
                    return CreatedAtAction(nameof(GetBloodRequestById), new { id = bloodRequestModel.RequestID }, bloodRequestModel);

                return StatusCode(StatusCodes.Status500InternalServerError, "Error inserting blood request.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
        }
        #endregion


        #region Update
        [HttpPut("{id:int}")]
        public IActionResult UpdateBloodRequest(int id, [FromBody] BloodRequestModel bloodRequestModel)
        {
            if (id != bloodRequestModel.RequestID)
                return BadRequest("BloodRequest ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var isUpdated = _bloodRequestRepository.Update(bloodRequestModel);
                if (isUpdated)
                    return NoContent();

                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating blood request.");
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
        }
        #endregion


        #region UpdateStatus
        [HttpPut("UpdateStatus/{id:int}")]
        public IActionResult UpdateBloodRequestStatusStock(int id, [FromBody] BloodRequestUpdateStatusModel statusModel)
        {
            if (id != statusModel.RequestID)
                return BadRequest("BloodRequest ID mismatch.");

            try
            {
                var isUpdated = _bloodRequestRepository.UpdateStatusAndStock(statusModel);
                if (isUpdated)
                    return NoContent();

                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating blood request status or stock.");
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occurred.");
            }
        }
        #endregion


        #region Delete
        [HttpDelete("{id:int}")]
        public IActionResult DeleteBloodRequest(int id)
        {
            try
            {
                var isDeleted = _bloodRequestRepository.Delete(id);
                if (isDeleted)
                    return NoContent();

                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting blood request.");
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"{ex.Message}");
            }
        }
        #endregion


        #region DropDown
        [HttpGet("dropdown")]
        public IActionResult GetBloodRequestDropDown()
        {
            var bloodRequests = _bloodRequestRepository.GetAll()
                .Select(d => new BloodRequestDropDownModel
                {
                    RequestID = d.RequestID,
                    RecipientName = d.RecipientName,
                    BloodGroupName = d.BloodGroupName
                })
                .ToList();

            if (bloodRequests == null || bloodRequests.Count == 0)
                return NotFound("No bloodRequests found for dropdown.");

            return Ok(bloodRequests);
        }
        #endregion
    
    }
}
