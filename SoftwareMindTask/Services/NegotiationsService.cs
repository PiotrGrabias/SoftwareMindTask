using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SoftwareMindTask.Entities;

namespace SoftwareMindTask.Services
{
    public class NegotiationsService
    {
        private readonly IMongoCollection<Negotiation> _negotiations;
        public NegotiationsService(
            IOptions<DatabaseSettings> productStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                productStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(
                productStoreDatabaseSettings.Value.DatabaseName);
            _negotiations = mongoDatabase.GetCollection<Negotiation>("Negotiations");
        }
        public async Task<List<Negotiation>> GetAll() =>
            await _negotiations.Find(_ => true).ToListAsync();
        public async Task<Negotiation?> GetAsync(string id) =>
            await _negotiations.Find(x => x.NegotiationId == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Negotiation newProduct) =>
            await _negotiations.InsertOneAsync(newProduct);
        public async Task UpdateAsync(string id, Negotiation updatedNegotiation) =>
            await _negotiations.ReplaceOneAsync(x => x.NegotiationId == id, updatedNegotiation);

        public async Task RemoveAsync(string id) =>
            await _negotiations.DeleteOneAsync(x => x.NegotiationId == id);

    }
}
