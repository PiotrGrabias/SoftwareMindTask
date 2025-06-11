using System.ComponentModel.DataAnnotations;

namespace SoftwareMindTask.DTOs
{
    public class NegotiationDto
    {
        [Required]
        public string ProductId { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

    }
}
