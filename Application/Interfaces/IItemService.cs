using RESTfullAPI.Application.DTOs;

namespace RESTfullAPI.Application.Interfaces;

public interface IItemService
{
    Task<ItemDto?> GetByIdAsync(int id);
    Task<List<ItemDto>> GetByProductIdAsync(int productId);
    Task<ItemDto> CreateAsync(CreateItemDto createItemDto);
    Task<ItemDto> UpdateAsync(int id, UpdateItemDto updateItemDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
