using Domain.Core.Commands;
using Domain.Models.Bags;
using Domain.Models.Products;
using Domain.Models.Promotions;
using Domain.Models.Promotions.Strategies;
using Domain.Services.Promotions;
using Domain.Shared.Repositories.Promotions;
using Moq;
using System.Reflection.Metadata.Ecma335;

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

        [Fact]
        public async Task ApplyPromotions_BagItem_HasPromotionsWhithDependenciesConditionsMet_PromotionsAreApplied()
        {
            //arrange
            double discountToApply = 0.1;
            double productBasePrice = 10;
            int mainPromotion = 1;
            int dependentPromotion = 2;
            int promoQuantity = 2;

            var commandDispatcher = new Mock<ICommandDispatcher>();

            var promoBuyXGetY = new Mock<Promotion>();
            promoBuyXGetY.Setup(instance => instance.DependsOn.All(It.IsAny<Func<Promotion, bool>>())).Returns(true);
            promoBuyXGetY.Setup(instance => instance.Apply(It.IsAny<Guid>(), It.IsAny<BagItem>()));

            var promotionsRepository = new Mock<IPromotionRepository>();
            promotionsRepository
                .Setup(pr => pr.Find(It.IsAny<Promotion>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Promotion>
                {
                    new BuyXGetY (commandDispatcher.Object)
                    {
                        Id = mainPromotion,
                        For = new [] { new Product { Id = productId } },
                        BuyX = promoQuantity,
                        GetY = relatedProduct
                    },
                    new PercentOff (commandDispatcher.Object)
                    {
                        Id = dependentPromotion,
                        For = new [] { new Product { Id = relatedProduct} },
                        Discount = discountToApply,
                        DependsOn = new [] { new Promotion { Id = mainPromotion } }
                    } 
                });

            var bag = new Bag();
            var bagItem = new BagItem { Product = new BagItemProduct { Id = productId, Price = new BagItemProductPrice { BasePrice = productBasePrice } }, Quantity = promoQuantity };
            bag.Items.Add(bagItem);

            //act
            var sut = new PromotionService(promotionsRepository.Object);
            await sut.ApplyPromotions(bag, bagItem);

            //assert
            Assert.True(bagItem.AppliedPromotions.Any());
            Assert.True(bagItem.Product.Price.DiscountPrice == productBasePrice - (productBasePrice * discountToApply));
            Assert.True(bag.Items.Count == 2);
            Assert.Contains(bag.Items, item => item.Product.Id == relatedProduct);
            Assert.True(bag.Items.Single(item => item.Product.Id == relatedProduct).AppliedPromotions.Any());
            Assert.True(bag.Items.Single(item => item.Product.Id == relatedProduct).AppliedPromotions.Single().Id == dependentPromotion);

        }
    }
}