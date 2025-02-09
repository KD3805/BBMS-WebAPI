using BBMS_WebAPI.Data;
using BBMS_WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace BBMS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodStockController : ControllerBase
    {
        private readonly BloodStockRepository _bloodStockRepository;

        public BloodStockController(IConfiguration configuration)
        {
            _bloodStockRepository = new BloodStockRepository(configuration);
        }

        #region GetAll
        [HttpGet]
        public IActionResult GetAllBloodStocks()
        {
            var bloodStocks = _bloodStockRepository.GetAll();
            if (bloodStocks == null || bloodStocks.Count == 0)
                return NotFound("No blood stocks found.");
            return Ok(bloodStocks);
        }
        #endregion

        #region GetById
        [HttpGet("{id:int}")]
        public IActionResult GetBloodStockById(int id)
        {
            var bloodStock = _bloodStockRepository.GetById(id);
            if (bloodStock == null)
                return NotFound($"Blood stock with ID {id} not found.");
            return Ok(bloodStock);
        }
        #endregion

        #region Insert
        [HttpPost]
        public IActionResult InsertBloodStock(BloodStockModel bloodStock)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var inserted = _bloodStockRepository.Insert(bloodStock);
            if (inserted)
                return CreatedAtAction(nameof(GetBloodStockById), new { id = bloodStock.StockID }, bloodStock);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error inserting blood stock.");
        }
        #endregion

        #region Update
        [HttpPut("{id:int}")]
        public IActionResult UpdateBloodStock(int id, BloodStockModel bloodStock)
        {
            // Override any conflicting StockID in the payload with the route id.
            bloodStock.StockID = id;

            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values
                                                  .SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                return BadRequest(new { message = "Validation failed", errors = validationErrors });
            }

            var stockByID = _bloodStockRepository.GetById(id);
            if (stockByID == null)
                return NotFound($"Blood stock with ID {id} not found.");

            // If LastUpdated is not provided, set it to the current date/time.
            if (bloodStock.LastUpdated == null)
                bloodStock.LastUpdated = DateTime.Now;

            var updated = _bloodStockRepository.Update(bloodStock);
            if (updated)
                return NoContent();

            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating blood stock.");
        }
        #endregion

        #region Delete
        [HttpDelete("{id:int}")]
        public IActionResult DeleteBloodStock(int id)
        {
            var bloodStock = _bloodStockRepository.GetById(id);
            if (bloodStock == null)
                return NotFound($"Blood stock with ID {id} not found.");
            var deleted = _bloodStockRepository.Delete(id);
            if (deleted)
                return NoContent();
            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting blood stock.");
        }
        #endregion
    }
}
