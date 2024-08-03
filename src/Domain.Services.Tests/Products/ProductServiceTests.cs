using Domain.Models.Products;
using Domain.Services.Products;
using Domain.Shared.Repositories.Products;
using Moq;

namespace Domain.Services.Tests.Products
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task Find_InvalidArgument_ReturnsEmpty()
        {
            //arrange
            var productRepository = new Mock<IProductRepository>();
            productRepository.Setup(pr => pr.Find(It.IsAny<Product>())).ReturnsAsync([]);

            //act
            var sut = new ProductService(productRepository.Object);
            var result = await sut.Find(new Product());

            //assert
            Assert.Empty(result);
        }
    }
}