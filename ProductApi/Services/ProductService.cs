public class ProductService : IProductService
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;

    public ProductService()
    {
        // Seed with sample data
        _products.AddRange(new[]
        {
            new Product { Id = _nextId++, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, Category = "Electronics", StockQuantity = 10 },
            new Product { Id = _nextId++, Name = "Coffee Mug", Description = "Ceramic coffee mug", Price = 12.99m, Category = "Kitchenware", StockQuantity = 50 },
            new Product { Id = _nextId++, Name = "Book", Description = "Programming guide", Price = 39.99m, Category = "Books", StockQuantity = 25 },
        });
    }

    public Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return Task.FromResult<IEnumerable<Product>>(_products);
    }

    public Task<Product?> GetProductByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task<Product> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = _nextId++,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            StockQuantity = request.StockQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<Product?> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null) return Task.FromResult<Product?>(null);

        if (!string.IsNullOrEmpty(request.Name)) product.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Description)) product.Description = request.Description;
        if (request.Price.HasValue) product.Price = request.Price.Value;
        if (!string.IsNullOrEmpty(request.Category)) product.Category = request.Category;
        if (request.StockQuantity.HasValue) product.StockQuantity = request.StockQuantity.Value;
        
        product.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Product?>(product);
    }

    public Task<bool> DeleteProductAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null) return Task.FromResult(false);

        _products.Remove(product);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        var results = _products.Where(p => 
            p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Category.Contains(query, StringComparison.OrdinalIgnoreCase)
        );
        return Task.FromResult(results);
    }
}