using RESTfullAPI.Domain.Entities;

namespace RESTfullAPI.Infrastructure.Data.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    Task<Product?> GetByIdWithItemsAsync(int id);
}



