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
        public string ProductId { get; set; } 
        public List<decimal> PricesNegotiated { get; set; }
        [Required]
        public decimal Price { get; set; }
        public NegotiationStatus Status { get; set; } = NegotiationStatus.Pending;
        public DateTime NegotiationDate { get; set; } = DateTime.UtcNow;

    }
    public class NegotiationDto
    {
        [Required]
        public string ProductId { get; set; }
        [Required]
        public decimal Price { get; set; }

    }
    public enum NegotiationStatus
    {
        Pending,
        Accepted,
        Rejected,
        Cancelled
    }
}
