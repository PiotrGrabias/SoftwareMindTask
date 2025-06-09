using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoftwareMindTask.Entities
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        public void ChangePrice(decimal price)
        {
            if(price < 0) throw new ArgumentException("Price cannot be negative.");
            this.Price = price;
        }
    }
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
