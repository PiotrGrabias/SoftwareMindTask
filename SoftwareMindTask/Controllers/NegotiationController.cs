using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoftwareMindTask.Entities;
using SoftwareMindTask.Services;

namespace SoftwareMindTask.Controllers
{
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNegotiation(string id)
        {
            var negotiation = await _negotiationsService.GetAsync(id);
            if (negotiation == null) return NotFound();
            return Ok(negotiation);
        }
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
        
        [HttpPost("start-negotiation")]
        public async Task<IActionResult> StartNegotiation(NegotiationDto newNegotiation)
        {
            if (newNegotiation.Price <= 0)
            {
                return BadRequest("You must start with an initial proposed price.");
            }
            var negotiation = new Negotiation
            {
                ProductId = newNegotiation.ProductId,
                Price = newNegotiation.Price,
                PricesNegotiated = new List<decimal> { newNegotiation.Price },
                Status = NegotiationStatus.Pending,
                NegotiationDate = DateTime.UtcNow
            };

            await _negotiationsService.CreateAsync(negotiation);

            return CreatedAtAction(nameof(GetNegotiation), new { id = negotiation.NegotiationId }, negotiation);
        }
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
                (DateTime.UtcNow - negotiation.NegotiationDate).TotalDays > 7)
            {
                negotiation.Status = NegotiationStatus.Cancelled;
                await _negotiationsService.UpdateAsync(id, negotiation);
                return BadRequest("Negotiation expired. No new price was proposed within 7 days.");
            }

            negotiation.PricesNegotiated.Add(newPrice);
            negotiation.Status = NegotiationStatus.Pending;
            negotiation.NegotiationDate = DateTime.UtcNow;
            await _negotiationsService.UpdateAsync(id, negotiation);

            return Ok(negotiation);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptNegotiation(string id)
        {
            var negotiation = await _negotiationsService.GetAsync(id);
            if (negotiation == null) return NotFound();
            negotiation.Status = NegotiationStatus.Accepted;
            var finalPrice = negotiation.PricesNegotiated.LastOrDefault();
            await _productsService.UpdatePriceAsync(negotiation.ProductId, finalPrice);
            await _negotiationsService.UpdateAsync(id, negotiation);
            return Ok("Negotiation accepted.");
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectNegotiation(string id)
        {
            var negotiation = await _negotiationsService.GetAsync(id);
            if (negotiation.Status == NegotiationStatus.Accepted || negotiation.Status == NegotiationStatus.Cancelled)
                return BadRequest("This negotiation has already been completed or cancelled.");
            if (negotiation == null) return NotFound();

            if (negotiation.PricesNegotiated.Count >= 3)
            {
                negotiation.Status = NegotiationStatus.Cancelled;
                await _negotiationsService.UpdateAsync(id, negotiation);
                return Ok("Maximum number of attempts reached.");
            }

            negotiation.Status = NegotiationStatus.Rejected;
            negotiation.NegotiationDate = DateTime.UtcNow;
            await _negotiationsService.UpdateAsync(id, negotiation);

            return Ok("Price rejected. Awaiting new proposal for 7 days.");
        }
    }

}
