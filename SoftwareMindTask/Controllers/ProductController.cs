using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using SoftwareMindTask.Entities;
using SoftwareMindTask.Services;

namespace SoftwareMindTask.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ProductsService _productsService;
        public ProductController(ProductsService productsService)
        {
            _productsService = productsService;
        }
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Product>> Get(string id)
        {
            var product = await _productsService.GetAsync(id);
            if(product is null)
            {
                return NotFound();
            }
            return product;
        }
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAll()
        {
            var product = await _productsService.GetAll();
            if (product is null)
            {
                return NotFound();
            }
            return product;
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<IActionResult> Post(ProductDto newProduct)
        {
            var product = new Product
            {
                Name = newProduct.Name,
                Price = newProduct.Price
            };

            await _productsService.CreateAsync(product);

            return CreatedAtAction(nameof(Get), new { id = product.ProductId }, newProduct);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, ProductDto updated)
        {
            var product = await _productsService.GetAsync(id);
            var updatedProduct = new Product
            {
                Name = updated.Name,
                Price = updated.Price
            };
            if (product is null)
            {
                return NotFound();
            }

            updatedProduct.ProductId = product.ProductId;

            await _productsService.UpdateAsync(id, updatedProduct);

            return NoContent();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _productsService.GetAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            await _productsService.RemoveAsync(id);

            return NoContent();
        }

    }
}
