var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register our product service - we'll define interfaces and classes in the same files
builder.Services.AddSingleton<IProductService, ProductService>();

// Add CORS for Claude MCP integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClaude", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowClaude");
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

// MCP endpoint to describe available tools
app.MapGet("/mcp/tools", () => new
{
    tools = new object[]
    {
        new
        {
            name = "get_products",
            description = "Get all products or search products by query",
            input_schema = new
            {
                type = "object",
                properties = new
                {
                    search = new { type = "string", description = "Optional search query to filter products" }
                }
            }
        },
        new
        {
            name = "get_product",
            description = "Get a specific product by ID",
            input_schema = new
            {
                type = "object",
                properties = new
                {
                    id = new { type = "integer", description = "Product ID" }
                },
                required = new[] { "id" }
            }
        },
        new
        {
            name = "create_product",
            description = "Create a new product",
            input_schema = new
            {
                type = "object",
                properties = new
                {
                    name = new { type = "string", description = "Product name" },
                    description = new { type = "string", description = "Product description" },
                    price = new { type = "number", description = "Product price" },
                    category = new { type = "string", description = "Product category" },
                    stockQuantity = new { type = "integer", description = "Stock quantity" }
                },
                required = new[] { "name", "price", "category", "stockQuantity" }
            }
        },
        new
        {
            name = "update_product",
            description = "Update an existing product",
            input_schema = new
            {
                type = "object",
                properties = new
                {
                    id = new { type = "integer", description = "Product ID" },
                    name = new { type = "string", description = "Product name" },
                    description = new { type = "string", description = "Product description" },
                    price = new { type = "number", description = "Product price" },
                    category = new { type = "string", description = "Product category" },
                    stockQuantity = new { type = "integer", description = "Stock quantity" }
                },
                required = new[] { "id" }
            }
        },
        new
        {
            name = "delete_product",
            description = "Delete a product by ID",
            input_schema = new
            {
                type = "object",
                properties = new
                {
                    id = new { type = "integer", description = "Product ID" }
                },
                required = new[] { "id" }
            }
        }
    }
});

app.Run();

