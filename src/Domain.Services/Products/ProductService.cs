using Domain.Models.Products;
using Domain.Shared.Repositories.Products;
using Domain.Shared.Services.Products;

namespace Domain.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> Find(Product product)
        {
            return await productRepository.Find(product);
        }
    }
}
