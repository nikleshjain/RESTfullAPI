using RESTfullAPI.Domain.Entities;

namespace RESTfullAPI.Infrastructure.Data.Repositories;

public interface IItemRepository : IGenericRepository<Item>
{
    Task<IEnumerable<Item>> GetByProductIdAsync(int productId);
    Task<Item?> GetByIdWithProductAsync(int id);
}



