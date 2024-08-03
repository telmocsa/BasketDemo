using Domain.Models.Products;

namespace Domain.Shared.Repositories.Products
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> Find(Product product);
    }
}