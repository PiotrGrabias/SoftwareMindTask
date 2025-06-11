using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftwareMindTask.DTOs;
using SoftwareMindTask.Entities;
using SoftwareMindTask.Services;

namespace SoftwareMindTask.Controllers
{
    /// <summary>
    /// Handles negotiation operations
    /// </summary>
    [ApiController]
    [Route("api/negotiations")]
    public class NegotiationController : ControllerBase
    {

        private readonly NegotiationsService _negotiationsService;
        private readonly ProductsService _productsService;
        public NegotiationController(NegotiationsService negotiationsService, ProductsService productsService)
        {
            _productsService = productsService;
            _negotiationsService = negotiationsService;
        }
        /// <summary>
        /// Gets a negotiation by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNegotiation(string id)
        {
            var negotiation = await _negotiationsService.GetAsync(id);
            if (negotiation == null) return NotFound();
            return Ok(negotiation);
        }
        /// <summary>
        /// Gets all negotiations
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Negotiation>>> GetAll()
        {
            var negotiations = await _negotiationsService.GetAll();
            if (negotiations is null)
            {
                return NotFound();
            }
            return negotiations;
        }
        /// <summary>
        /// Validates the price and starts a new negotiation from the NegotiationDto
        /// </summary>
        [HttpPost("start-negotiation")]
        public async Task<IActionResult> StartNegotiation(NegotiationDto newNegotiation)
        {
            var negotiation = new Negotiation
            {
                ProductId = newNegotiation.ProductId,
                Price = newNegotiation.Price,
                PricesNegotiated = new List<decimal> { newNegotiation.Price },
                Status = NegotiationStatus.Pending,
                UpdatedAt = DateTime.UtcNow
            };
            await _negotiationsService.CreateAsync(negotiation);

            return Ok("Negotiation started");
        }
        /// <summary>
        /// Proposes a new price in a negotiation
        /// </summary>
        [HttpPost("{id}/propose")]
        public async Task<IActionResult> ProposePrice(string id, decimal newPrice)
        {
            var negotiation = await _negotiationsService.GetAsync(id);
            if (negotiation == null) return NotFound();

            if (negotiation.Status == NegotiationStatus.Accepted || negotiation.Status == NegotiationStatus.Cancelled)
                return BadRequest("This negotiation has already been completed or cancelled.");

            if (negotiation.PricesNegotiated.Count >= 3)
                return BadRequest("Maximum number of negotiation attempts reached.");

            if (negotiation.Status == NegotiationStatus.Rejected &&
                (DateTime.UtcNow - negotiation.UpdatedAt).TotalDays > 7)
            {
                negotiation.Status = NegotiationStatus.Cancelled;
                await _negotiationsService.UpdateAsync(id, negotiation);
                return BadRequest("Negotiation expired. No new price was proposed within 7 days.");
            }
            negotiation.PricesNegotiated.Add(newPrice);
            negotiation.Status = NegotiationStatus.Pending;
            negotiation.UpdatedAt = DateTime.UtcNow;
            await _negotiationsService.UpdateAsync(id, negotiation);

            return Ok(negotiation);
        }
        /// <summary>
        /// Accepts a negotiation, changes the negotiation status to accepted
        /// </summary>
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptNegotiation(string id)
        {
            var negotiation = await _negotiationsService.GetAsync(id);
            if (negotiation == null) return NotFound();
            if (negotiation.Status == NegotiationStatus.Accepted || negotiation.Status == NegotiationStatus.Cancelled)
                return BadRequest("This negotiation has already been completed or cancelled.");
            if (negotiation.PricesNegotiated.Count >= 3)
            {
                negotiation.Status = NegotiationStatus.Cancelled;
                await _negotiationsService.UpdateAsync(id, negotiation);
                return Ok("Maximum number of attempts reached.");
            }
            negotiation.Status = NegotiationStatus.Accepted;
            var finalPrice = negotiation.PricesNegotiated.LastOrDefault();
            await _productsService.UpdatePriceAsync(negotiation.ProductId, finalPrice);
            await _negotiationsService.UpdateAsync(id, negotiation);
            return Ok("Negotiation accepted.");
        }
        /// <summary>
        /// Rejects a negotiation, sets the negotiation status to rejected
        /// </summary>
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectNegotiation(string id)
        {
            var negotiation = await _negotiationsService.GetAsync(id);
            if (negotiation == null) return NotFound();
            if (negotiation.Status == NegotiationStatus.Accepted || negotiation.Status == NegotiationStatus.Cancelled)
                return BadRequest("This negotiation has already been completed or cancelled.");
            if (negotiation.PricesNegotiated.Count >= 3)
            {
                negotiation.Status = NegotiationStatus.Cancelled;
                await _negotiationsService.UpdateAsync(id, negotiation);
                return Ok("Maximum number of attempts reached.");
            }
            negotiation.Status = NegotiationStatus.Rejected;
            negotiation.UpdatedAt = DateTime.UtcNow;
            await _negotiationsService.UpdateAsync(id, negotiation);
            return Ok("Price rejected. Awaiting new proposal for 7 days.");
        }
    }

}
