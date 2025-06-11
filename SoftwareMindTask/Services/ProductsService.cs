using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SoftwareMindTask.Entities;

namespace SoftwareMindTask.Services
{
    public class ProductsService
    {
        private readonly IMongoCollection<Product> _products;
        public ProductsService(
            IOptions<DatabaseSettings> productStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                productStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(
                productStoreDatabaseSettings.Value.DatabaseName);
            _products = mongoDatabase.GetCollection<Product>("Products");
        }
        public async Task<List<Product>> GetAll() =>
            await _products.Find(_ => true).ToListAsync();

        public async Task<Product?> GetAsync(string id) =>
            await _products.Find(x => x.ProductId == id).FirstOrDefaultAsync();
        public async Task CreateAsync(Product newProduct) =>
            await _products.InsertOneAsync(newProduct);
        public async Task UpdateAsync(string id, Product updatedProduct) =>
            await _products.ReplaceOneAsync(x => x.ProductId == id, updatedProduct);

        public async Task RemoveAsync(string id) =>
            await _products.DeleteOneAsync(x => x.ProductId == id);
        public async Task UpdatePriceAsync(string productId, decimal newPrice)
        {
            var product = await _products.Find(x => x.ProductId == productId).FirstOrDefaultAsync();
            if (product == null) throw new Exception("Product not found");

            await _products.ReplaceOneAsync(x => x.ProductId == productId, product);
        }
    }
}
