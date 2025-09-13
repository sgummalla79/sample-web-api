using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? search = null)
    {
        try
        {
            IEnumerable<Product> products;
            
            if (!string.IsNullOrEmpty(search))
            {
                products = await _productService.SearchProductsAsync(search);
            }
            else
            {
                products = await _productService.GetAllProductsAsync();
            }

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving products", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }
            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the product", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(CreateProductRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the product", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> UpdateProduct(int id, UpdateProductRequest request)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, request);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }
            return Ok(product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the product", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        try
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the product", error = ex.Message });
        }
    }
}