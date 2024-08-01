using Domain.Models.Products;

namespace Domain.Shared.Services.Products
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> Find(Product product); 
    }
}
