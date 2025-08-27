using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTfullAPI.Application.DTOs;
using RESTfullAPI.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RESTfullAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[SwaggerTag("Item management operations")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly ILogger<ItemsController> _logger;
    
    public ItemsController(IItemService itemService, ILogger<ItemsController> logger)
    {
        _itemService = itemService;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all items for a specific product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>List of items for the product</returns>
    [HttpGet("product/{productId}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get items by product ID", Description = "Retrieves all items for a specific product")]
    [SwaggerResponse(200, "Successfully retrieved items", typeof(List<ItemDto>))]
    [SwaggerResponse(400, "Bad request")]
    public async Task<ActionResult<List<ItemDto>>> GetItemsByProduct(int productId)
    {
        try
        {
            var items = await _itemService.GetByProductIdAsync(productId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving items for product {ProductId}", productId);
            return StatusCode(500, "An error occurred while retrieving items");
        }
    }
    
    /// <summary>
    /// Get a specific item by ID
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <returns>Item details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get item by ID", Description = "Retrieves a specific item by its ID")]
    [SwaggerResponse(200, "Successfully retrieved item", typeof(ItemDto))]
    [SwaggerResponse(404, "Item not found")]
    public async Task<ActionResult<ItemDto>> GetItem(int id)
    {
        try
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound($"Item with ID {id} not found");
            }
            
            return Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving item with ID {ItemId}", id);
            return StatusCode(500, "An error occurred while retrieving the item");
        }
    }
    
    /// <summary>
    /// Create a new item
    /// </summary>
    /// <param name="createItemDto">Item creation data</param>
    /// <returns>Created item</returns>
    [HttpPost]
    [Authorize]
    [SwaggerOperation(Summary = "Create item", Description = "Creates a new item")]
    [SwaggerResponse(201, "Item created successfully", typeof(ItemDto))]
    [SwaggerResponse(400, "Bad request")]
    public async Task<ActionResult<ItemDto>> CreateItem(CreateItemDto createItemDto)
    {
        try
        {
            var createdItem = await _itemService.CreateAsync(createItemDto);
            return CreatedAtAction(nameof(GetItem), new { id = createdItem.Id }, createdItem);
        }
        catch (Exception ex) when (ex.Message.Contains("not found"))
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating item");
            return StatusCode(500, "An error occurred while creating the item");
        }
    }
    
    /// <summary>
    /// Update an existing item
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <param name="updateItemDto">Item update data</param>
    /// <returns>Updated item</returns>
    [HttpPut("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Update item", Description = "Updates an existing item")]
    [SwaggerResponse(200, "Item updated successfully", typeof(ItemDto))]
    [SwaggerResponse(404, "Item not found")]
    [SwaggerResponse(400, "Bad request")]
    public async Task<ActionResult<ItemDto>> UpdateItem(int id, UpdateItemDto updateItemDto)
    {
        try
        {
            var updatedItem = await _itemService.UpdateAsync(id, updateItemDto);
            return Ok(updatedItem);
        }
        catch (Exception ex) when (ex.Message.Contains("not found"))
        {
            return NotFound($"Item with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating item with ID {ItemId}", id);
            return StatusCode(500, "An error occurred while updating the item");
        }
    }
    
    /// <summary>
    /// Delete an item
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Delete item", Description = "Deletes an item")]
    [SwaggerResponse(204, "Item deleted successfully")]
    [SwaggerResponse(404, "Item not found")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        try
        {
            var deleted = await _itemService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Item with ID {id} not found");
            }
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item with ID {ItemId}", id);
            return StatusCode(500, "An error occurred while deleting the item");
        }
    }
}
