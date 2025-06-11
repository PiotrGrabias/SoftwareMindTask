using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace SoftwareMindTask.Entities
{
    public class Negotiation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string NegotiationId { get; set; }
        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } 
        public List<decimal> PricesNegotiated { get; set; } = new List<decimal>();
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price can't be negative")]
        public decimal Price { get; set; }
        public NegotiationStatus Status { get; set; } = NegotiationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    }
    public enum NegotiationStatus
    {
        Pending,
        Accepted,
        Rejected,
        Cancelled
    }
}
