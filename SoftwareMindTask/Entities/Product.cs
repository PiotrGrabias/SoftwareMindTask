using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace SoftwareMindTask.Entities
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
        public string Name { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price can't be negative")]
        public decimal Price { get; set; }
    }
}
