using Domain.Models.Bags;
using Domain.Models.Products;
using Domain.Models.Users;
using Domain.Services.Bags;
using Domain.Services.Products;
using Domain.Services.Promotions;
using Domain.Shared.Repositories.Bags;
using Domain.Shared.Repositories.Products;
using Domain.Shared.Services.Promotions;
using Moq;

namespace Domain.Services.Tests.Bags
{
    public class BagsServiceTests
    {
        private const int userId = 123123;

        [Fact]
        public async Task User_HasNoBag_CreateBag()
        {
            //arrange
            var bagRepository = new Mock<IBagRepository>();
            bagRepository.Setup(bag => bag.Find(It.IsAny<Bag>())).ReturnsAsync(default(Bag));
            bagRepository.Setup(bag => bag.Create(It.IsAny<Bag>())).ReturnsAsync(true);

            var promotionService = new Mock<IPromotionService>();

            //act
            var sut = new BagService(bagRepository.Object, promotionService.Object);
            var result = await sut.Create(new User { Id = userId });

            //assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.OwnerId);
            bagRepository.Verify(bag => bag.Find(It.IsAny<Bag>()), Times.Once);
            bagRepository.Verify(bag => bag.Create(It.IsAny<Bag>()), Times.Once);
        }

        [Fact]
        public async Task User_HasBag_ReturnBag()
        {
            //arrange
            var bagRepository = new Mock<IBagRepository>();
            bagRepository.Setup(bag => bag.Find(It.IsAny<Bag>())).ReturnsAsync(new Bag { Id = Guid.NewGuid(), OwnerId = userId });

            var promotionService = new Mock<IPromotionService>();

            //act
            var sut = new BagService(bagRepository.Object, promotionService.Object);
            var result = await sut.Create(new User { Id = userId });

            //assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.OwnerId);
            bagRepository.Verify(bag => bag.Find(It.IsAny<Bag>()), Times.Once);
            bagRepository.Verify(bag => bag.Create(It.IsAny<Bag>()), Times.Never);
        }
    }
}