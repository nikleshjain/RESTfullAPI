using Microsoft.EntityFrameworkCore;
using RESTfullAPI.Domain.Entities;

namespace RESTfullAPI.Infrastructure.Data.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var totalCount = await _dbSet.CountAsync();
        var products = await _dbSet
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (products, totalCount);
    }
    
    public async Task<Product?> GetByIdWithItemsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}



