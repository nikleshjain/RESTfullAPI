using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTfullAPI.Application.DTOs;
using RESTfullAPI.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RESTfullAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[SwaggerTag("Product management operations")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;
    
    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all products with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get all products", Description = "Retrieves a paginated list of all products")]
    [SwaggerResponse(200, "Successfully retrieved products", typeof(ProductListDto))]
    [SwaggerResponse(400, "Bad request")]
    public async Task<ActionResult<ProductListDto>> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Page number must be greater than 0 and page size must be between 1 and 100");
            }
            
            var products = await _productService.GetAllAsync(pageNumber, pageSize);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, "An error occurred while retrieving products");
        }
    }
    
    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get product by ID", Description = "Retrieves a specific product by its ID")]
    [SwaggerResponse(200, "Successfully retrieved product", typeof(ProductDto))]
    [SwaggerResponse(404, "Product not found")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found");
            }
            
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
            return StatusCode(500, "An error occurred while retrieving the product");
        }
    }
    
    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="createProductDto">Product creation data</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [Authorize]
    [SwaggerOperation(Summary = "Create product", Description = "Creates a new product")]
    [SwaggerResponse(201, "Product created successfully", typeof(ProductDto))]
    [SwaggerResponse(400, "Bad request")]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
    {
        try
        {
            var createdProduct = await _productService.CreateAsync(createProductDto);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, "An error occurred while creating the product");
        }
    }
    
    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="updateProductDto">Product update data</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Update product", Description = "Updates an existing product")]
    [SwaggerResponse(200, "Product updated successfully", typeof(ProductDto))]
    [SwaggerResponse(404, "Product not found")]
    [SwaggerResponse(400, "Bad request")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto updateProductDto)
    {
        try
        {
            var updatedProduct = await _productService.UpdateAsync(id, updateProductDto);
            return Ok(updatedProduct);
        }
        catch (Exception ex) when (ex.Message.Contains("not found"))
        {
            return NotFound($"Product with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {ProductId}", id);
            return StatusCode(500, "An error occurred while updating the product");
        }
    }
    
    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Delete product", Description = "Deletes a product")]
    [SwaggerResponse(204, "Product deleted successfully")]
    [SwaggerResponse(404, "Product not found")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Product with ID {id} not found");
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID {ProductId}", id);
            return StatusCode(500, "An error occurred while deleting the product");
        }
    }
}


