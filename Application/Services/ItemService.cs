using AutoMapper;
using RESTfullAPI.Application.DTOs;
using RESTfullAPI.Application.Interfaces;
using RESTfullAPI.Domain.Entities;
using RESTfullAPI.Domain.Exceptions;
using RESTfullAPI.Infrastructure.Data.Repositories;

namespace RESTfullAPI.Application.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    
    public ItemService(IItemRepository itemRepository, IProductRepository productRepository, IMapper mapper)
    {
        _itemRepository = itemRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }
    
    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        var item = await _itemRepository.GetByIdWithProductAsync(id);
        return _mapper.Map<ItemDto>(item);
    }
    
    public async Task<List<ItemDto>> GetByProductIdAsync(int productId)
    {
        var items = await _itemRepository.GetByProductIdAsync(productId);
        return _mapper.Map<List<ItemDto>>(items);
    }
    
    public async Task<ItemDto> CreateAsync(CreateItemDto createItemDto)
    {
        // Validate that the product exists
        var productExists = await _productRepository.ExistsAsync(createItemDto.ProductId);
        if (!productExists)
        {
            throw new DomainException($"Product with ID {createItemDto.ProductId} not found");
        }
        
        var item = _mapper.Map<Item>(createItemDto);
        var createdItem = await _itemRepository.AddAsync(item);
        
        // Get the created item with product information
        var itemWithProduct = await _itemRepository.GetByIdWithProductAsync(createdItem.Id);
        return _mapper.Map<ItemDto>(itemWithProduct);
    }
    
    public async Task<ItemDto> UpdateAsync(int id, UpdateItemDto updateItemDto)
    {
        var existingItem = await _itemRepository.GetByIdAsync(id);
        if (existingItem == null)
        {
            throw new DomainException($"Item with ID {id} not found");
        }
        
        _mapper.Map(updateItemDto, existingItem);
        var updatedItem = await _itemRepository.UpdateAsync(existingItem);
        
        // Get the updated item with product information
        var itemWithProduct = await _itemRepository.GetByIdWithProductAsync(updatedItem.Id);
        return _mapper.Map<ItemDto>(itemWithProduct);
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        if (item == null)
        {
            return false;
        }
        
        await _itemRepository.DeleteAsync(item);
        return true;
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
        return await _itemRepository.ExistsAsync(id);
    }
}



