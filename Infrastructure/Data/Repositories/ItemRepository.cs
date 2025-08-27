using Microsoft.EntityFrameworkCore;
using RESTfullAPI.Domain.Entities;

namespace RESTfullAPI.Infrastructure.Data.Repositories;

public class ItemRepository : GenericRepository<Item>, IItemRepository
{
    public ItemRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Item>> GetByProductIdAsync(int productId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.ProductId == productId)
            .ToListAsync();
    }
    
    public async Task<Item?> GetByIdWithProductAsync(int id)
    {
        return await _dbSet
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == id);
    }
}



