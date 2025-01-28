using BBMS_WebAPI.Data;
using BBMS_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BBMS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonorController : ControllerBase
    {
        private readonly DonorRepository _donorRepository;

        public DonorController(IConfiguration configuration)
        {
            _donorRepository = new DonorRepository(configuration);
        }

        #region GetAll
        [HttpGet]
        public IActionResult GetAllDonors()
        {
            var donors = _donorRepository.GetAll();
            if (donors == null || donors.Count == 0)
                return NotFound("No donors found.");

            return Ok(donors);
        }
        #endregion

        #region GetById
        [HttpGet("{id:int}")]
        public IActionResult GetDonorById(int id)
        {
            var donor = _donorRepository.GetById(id);
            if (donor == null)
                return NotFound($"Donor with ID {id} not found.");

            return Ok(donor);
        }
        #endregion

        #region Donor Profile
        [HttpGet("Profile")]
        public IActionResult GetDonorProfile()
        {
            var userId = HttpContext.Items["UserId"];

            if (userId == null)
            {
                Console.WriteLine("Unauthorized: Token validation failed or user not attached to context.");
                return Unauthorized("Invalid or expired token.");
            }

            var donor = _donorRepository.GetById((int)userId);
            if (donor == null)
            {
                Console.WriteLine($"Donor with ID {userId} not found.");
                return NotFound("Donor not found.");
            }

            Console.WriteLine($"Donor Profile Retrieved: {donor.Name}");
            return Ok(new
            {
                donor.DonorID,
                donor.Name,
                donor.DOB,
                donor.Age,
                donor.Gender,
                donor.BloodGroupName,
                donor.Phone,
                donor.Email,
                donor.Address
            });
        }
        #endregion

        #region Insert
        [HttpPost]
        public IActionResult InsertDonor([FromBody] DonorModel donorModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isInserted = _donorRepository.Insert(donorModel);
            if (isInserted)
                return CreatedAtAction(nameof(GetDonorById), new { id = donorModel.DonorID }, donorModel);

            return StatusCode(StatusCodes.Status500InternalServerError, "Error inserting donor.");
        }
        #endregion

        #region Update
        [HttpPut("{id:int}")]
        public IActionResult UpdateDonor(int id, [FromBody] DonorModel donorModel)
        {
            if (id != donorModel.DonorID)
                return BadRequest("Donor ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var donor = _donorRepository.GetById(id);
            if (donor == null)
                return NotFound($"Donor with ID {id} not found.");

            var isUpdated = _donorRepository.Update(donorModel);
            if (isUpdated)
                return NoContent();

            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating donor.");
        }
        #endregion

        #region Delete
        [HttpDelete("{id:int}")]
        public IActionResult DeleteDonor(int id)
        {
            var donor = _donorRepository.GetById(id);
            if (donor == null)
                return NotFound($"Donor with ID {id} not found.");

            var isDeleted = _donorRepository.Delete(id);
            if (isDeleted)
                return NoContent();

            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting donor.");
        }
        #endregion

        #region GetByEmail
        [HttpPost("Email")]
        public IActionResult GetDonorByEmail([FromBody] LoginRequestModel rq)
        {
            if (string.IsNullOrWhiteSpace(rq.Email))
                return BadRequest("Email is required.");

            var donor = _donorRepository.GetByEmail(rq.Email);
            if (donor == null)
            {
                return Ok(new
                {
                    exists = false,
                    message = "Donor with the provided email not found."
                });
            }

            return Ok(new
            {
                message = "Donor with the given email already exists.",
                exists = true
            });
        }
        #endregion

        #region DropDown
        [HttpGet("DropDown")]
        public IActionResult GetDonorDropDown()
        {
            var donors = _donorRepository.GetDonorDropDown();
            if (donors == null || donors.Count == 0)
                return NotFound("No donors found for dropdown.");

            return Ok(donors);
        }
        #endregion
    }
}
