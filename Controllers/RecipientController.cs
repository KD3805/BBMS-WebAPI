using BBMS_WebAPI.Data;
using BBMS_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BBMS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipientController : ControllerBase
    {   
        private readonly RecipientRepository _recipientRepository;

        public RecipientController(IConfiguration configuration)
        {
            _recipientRepository = new RecipientRepository(configuration);
        }

        #region GetAll
        [HttpGet]
        public IActionResult GetAllRecipients()
        {
            var recipients = _recipientRepository.GetAll();
            if (recipients == null || recipients.Count == 0)
                return NotFound("No recipients found.");

            return Ok(recipients);
        }
        #endregion

        #region GetById
        [HttpGet("{id:int}")]
        public IActionResult GetRecipientById(int id)
        {
            var recipient = _recipientRepository.GetById(id);
            if (recipient == null)
                return NotFound($"Recipient with ID {id} not found.");

            return Ok(recipient);
        }
        #endregion

        #region Recipient Profile
        [HttpGet("Profile")]
        public IActionResult GetRecipientProfile()
        {
            var userId = HttpContext.Items["UserId"];

            if (userId == null)
            {
                Console.WriteLine("Unauthorized: Token validation failed or user not attached to context.");
                return Unauthorized("Invalid or expired token.");
            }

            var recipient = _recipientRepository.GetById((int)userId);
            if (recipient == null)
                return NotFound("Recipient not found.");

            return Ok(new
            {
                recipient.RecipientID,
                recipient.Name,
                recipient.DOB,
                recipient.Age,
                recipient.Gender,
                recipient.BloodGroupName,
                recipient.Phone,
                recipient.Email,
                recipient.Address
            });
        }
        #endregion

        #region Insert
        [HttpPost]
        public IActionResult InsertRecipient([FromBody] RecipientModel recipientModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isInserted = _recipientRepository.Insert(recipientModel);
            if (isInserted)
                return CreatedAtAction(nameof(GetRecipientById), new { id = recipientModel.RecipientID }, recipientModel);

            return StatusCode(StatusCodes.Status500InternalServerError, "Error inserting recipient.");
        }
        #endregion

        #region Update
        [HttpPut("{id:int}")]
        public IActionResult UpdateRecipient(int id, [FromBody] RecipientModel recipientModel)
        {
            if (id != recipientModel.RecipientID)
                return BadRequest("Recipient ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var recipient = _recipientRepository.GetById(id);
            if (recipient == null)
                return NotFound($"Recipient with ID {id} not found.");

            var isUpdated = _recipientRepository.Update(recipientModel);
            if (isUpdated)
                return NoContent();

            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating recipient.");
        }
        #endregion

        #region Delete
        [HttpDelete("{id:int}")]
        public IActionResult DeleteRecipient(int id)
        {
            var recipient = _recipientRepository.GetById(id);
            if (recipient == null)
                return NotFound($"Recipient with ID {id} not found.");

            var isDeleted = _recipientRepository.Delete(id);
            if (isDeleted)
                return NoContent();

            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting recipient.");
        }
        #endregion

        #region GetByEmail
        [HttpPost("Email")]
        public IActionResult GetRecipientByEmail([FromBody] LoginRequestModel rq)
        {
            if (string.IsNullOrWhiteSpace(rq.Email))
                return BadRequest("Email is required.");

            var recipient = _recipientRepository.GetByEmail(rq.Email);
            if (recipient == null)
            {
                return Ok(new
                {
                    exists = false,
                    message = "Recipient with the provided email not found."
                });
            }

            return Ok(new
            {
                message = "Recipient with the given email already exists.",
                exists = true
            });
        }
        #endregion

        #region DropDown
        [HttpGet("dropdown")]
        public IActionResult GetRecipientDropDown()
        {
            var recipients = _recipientRepository.GetRecipientDropDown();
            if (recipients == null || recipients.Count == 0)
                return NotFound("No recipients found for dropdown.");

            return Ok(recipients);
        }
        #endregion
    }
}
