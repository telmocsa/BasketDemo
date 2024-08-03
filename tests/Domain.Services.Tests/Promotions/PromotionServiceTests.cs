using Domain.Core.Commands;
using Domain.Models.Bags;
using Domain.Models.Products;
using Domain.Models.Promotions;
using Domain.Models.Promotions.Strategies;
using Domain.Services.Promotions;
using Domain.Shared.Repositories.Promotions;
using Moq;

namespace Domain.Services.Tests.Promotions
{
    public class PromotionServiceTests
    {
        private const int productId = 123123;
        private const int relatedProduct = 456456;

        [Fact]
        public async Task ApplyPromotions_BagItem_HasNoPromotions_NoneIsApplied()
        {
            //arrange
            var promotionsRepository = new Mock<IPromotionRepository>();
            promotionsRepository
                .Setup(pr => pr.Find(It.IsAny<Promotion>(), It.IsAny<DateTime>()))
                .ReturnsAsync([]);

            var bag = new Bag();
            var bagItem = new BagItem { Product = new BagItemProduct { Id = productId } };

            //act
            var sut = new PromotionService(promotionsRepository.Object);
            await sut.ApplyPromotions(bag, bagItem);

            //assert
            Assert.False(bagItem.AppliedPromotions.Any());
        }

        [Fact]
        public async Task ApplyPromotions_BagItem_HasPromotionsWhithOutDependencies_PromotionIsApplied()
        {
            //arrange
            double discountToApply = 0.1;
            double basePrice = 10;

            var commandDispatcher = new Mock<ICommandDispatcher>();

            var promotionsRepository = new Mock<IPromotionRepository>();
            promotionsRepository
                .Setup(pr => pr.Find(It.IsAny<Promotion>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new[] 
                {
                    new PercentOff (commandDispatcher.Object)
                    {
                        For = new [] { new Product { Id = productId} },
                        Discount = discountToApply,
                    }
                });

            var bag = new Bag();
            var bagItem = new BagItem { Product = new BagItemProduct { Id = productId, Price = new BagItemProductPrice { BasePrice = basePrice} } };

            //act
            var sut = new PromotionService(promotionsRepository.Object);
            await sut.ApplyPromotions(bag, bagItem);

            //assert
            Assert.True(bagItem.AppliedPromotions.Any());
            Assert.True(bagItem.Product.Price.DiscountPrice == basePrice - (basePrice * discountToApply));
        }
    }
}