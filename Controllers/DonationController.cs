using BBMS_WebAPI.Data;
using BBMS_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BBMS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationController : ControllerBase
    {
        private readonly DonationRepository _donationRepository;

        public DonationController(IConfiguration configuration)
        {
            _donationRepository = new DonationRepository(configuration);
        }

        #region GetAll
        [HttpGet]
        public IActionResult GetAllDonations()
        {
            var donations = _donationRepository.GetAll();
            if (donations == null || donations.Count == 0)
                return NotFound("No donations found.");

            return Ok(donations);
        }
        #endregion

        #region GetById
        [HttpGet("{id:int}")]
        public IActionResult GetDonationById(int id)
        {
            var donation = _donationRepository.GetById(id);
            if (donation == null)
                return NotFound($"Donation with ID {id} not found.");

            return Ok(donation);
        }
        #endregion

        #region Donation History By DonorID
        [HttpGet("history/{donorID:int}")]
        public IActionResult GetDonationHistoryByDonorID(int donorID)
        {
            try
            {
                var donationHistory = _donationRepository.GetDonationHistoryByDonorID(donorID);
                if (donationHistory == null || donationHistory.Count == 0)
                    return NotFound($"No donation history found for donor with ID {donorID}.");

                return Ok(donationHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error occurred: {ex.Message}");
            }
        }
        #endregion

        #region Insert
        [HttpPost]
        public IActionResult InsertDonation([FromBody] DonationModel donationModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var isInserted = _donationRepository.Insert(donationModel);
                if (isInserted)
                    return CreatedAtAction(nameof(GetDonationById), new { id = donationModel.DonationID }, donationModel);

                return StatusCode(StatusCodes.Status500InternalServerError, "Error inserting donation.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occurred.");
            }
        }
        #endregion

        #region Update
        [HttpPut("{id:int}")]
        public IActionResult UpdateDonation(int id, [FromBody] DonationModel donationModel)
        {
            if (id != donationModel.DonationID)
                return BadRequest("Donation ID mismatch.");                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var donation = _donationRepository.GetById(id);
            if (donation == null)
                return NotFound($"Donation with ID {id} not found.");

            try
            {
                var isUpdated = _donationRepository.Update(donationModel);
                if (isUpdated)
                    return NoContent();

                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating donation.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occurred.");
            }
        }
        #endregion

        #region UpdateStatus
        [HttpPut("status/{id:int}")]
        public IActionResult UpdateDonationStatusStock(int id, [FromBody] DonationUpdateStatusModel statusModel)
        {
            if (id != statusModel.DonationID)
                return BadRequest("Donation ID mismatch.");

            try
            {
                var isUpdated = _donationRepository.UpdateStatusAndStock(statusModel);
                if (isUpdated)
                    return NoContent();

                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating donation status or stock.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occurred.");
            }
        }
        #endregion

        #region Delete
        [HttpDelete("{id:int}")]
        public IActionResult DeleteDonation(int id)
        {
            var donation = _donationRepository.GetById(id);
            if (donation == null)
                return NotFound($"Donation with ID {id} not found.");

            var isDeleted = _donationRepository.Delete(id);
            if (isDeleted)
                return NoContent();

            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting donation.");
        }
        #endregion

        //#region DetermineEligibility
        //[HttpGet("eligibility/{donorName}")]
        //public IActionResult CheckEligibility(string donorName)
        //{
        //    try
        //    {
        //        var isEligible = _donationRepository.DetermineEligibility(donorName);
        //        return Ok(new
        //        {
        //            message = isEligible ? "Donor is eligible for donation." : "Donor is not eligible for donation.",
        //            isEligible
        //        });
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occurred.");
        //    }
        //}
        //#endregion

        #region DropDown
        [HttpGet("dropdown")]
        public IActionResult GetDonationDropDown()
        {
            var donations = _donationRepository.GetAll()
                .Select(d => new DonationDropDownModel
                {
                    DonationID = d.DonationID,
                    DonorName = d.DonorName,
                    BloodGroupName = d.BloodGroupName
                })
                .ToList();

            if (donations == null || donations.Count == 0)
                return NotFound("No donations found for dropdown.");

            return Ok(donations);
        }
        #endregion
    }
}
