using AutoMapper;
using RESTfullAPI.Application.DTOs;
using RESTfullAPI.Application.Interfaces;
using RESTfullAPI.Domain.Entities;
using RESTfullAPI.Domain.Exceptions;
using RESTfullAPI.Infrastructure.Data.Repositories;

namespace RESTfullAPI.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    
    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    
    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdWithItemsAsync(id);
        return _mapper.Map<ProductDto>(product);
    }
    
    public async Task<ProductListDto> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        var (products, totalCount) = await _productRepository.GetPagedAsync(pageNumber, pageSize);
        var productDtos = _mapper.Map<List<ProductDto>>(products);
        
        return new ProductListDto
        {
            Products = productDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }
    
    public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
    {
        var product = _mapper.Map<Product>(createProductDto);
        var createdProduct = await _productRepository.AddAsync(product);
        return _mapper.Map<ProductDto>(createdProduct);
    }
    
    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateProductDto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            throw new DomainException($"Product with ID {id} not found");
        }
        
        _mapper.Map(updateProductDto, existingProduct);
        var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
        return _mapper.Map<ProductDto>(updatedProduct);
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return false;
        }
        
        await _productRepository.DeleteAsync(product);
        return true;
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
        return await _productRepository.ExistsAsync(id);
    }
}



