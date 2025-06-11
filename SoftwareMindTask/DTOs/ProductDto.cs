using System.ComponentModel.DataAnnotations;

namespace SoftwareMindTask.DTOs
{
    public class ProductDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Range(0.01, double.MaxValue)]
        public decimal Price
        {
            get; set;
        }
    }
}
