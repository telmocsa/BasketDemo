using Domain.Core.Commands;
using Domain.Models.Bags;
using Domain.Models.Products;
using Domain.Models.Promotions;
using Domain.Models.Promotions.Strategies;
using Domain.Services.Bags;
using Domain.Services.Bags.CommandHandlers;
using Domain.Services.Promotions;
using Domain.Shared.Repositories.Promotions;
using Domain.Shared.Services.Bags;
using Domain.Shared.Services.Products;
using Moq;

namespace Domain.Services.Integration.Tests.Promotions
{
    public class PromotionServiceTests
    {
        [Fact]
        public async Task ApplyPromotions_BagItems_WithConditionsFor_PromotionsBuyXGetYWithPromotionZ_PromotionsAreApplied()
        {
            //arrange
            int productId = 123123;
            int productIdQty = 2;
            int relatedProduct = 456456;
            double discountToApply = 0.1;
            double productBasePrice = 10;
            int mainPromotion = 1;
            int dependentPromotion = 2;
            int promoTargetQuantity = 2;
            int promoAttributedQuantity = 1;
            int relatedProductPriceId = 1;
            double relatedProductBasePrice = 10;

            var bag = new Bag { Id = Guid.NewGuid() };
            var bagItem = new BagItem 
            { 
                Product = new BagItemProduct 
                { 
                    Id = productId, 
                    Price = new BagItemProductPrice 
                    { 
                        BasePrice = productBasePrice 
                    } 
                }, 
                Quantity = productIdQty
            };

            bag.Items.Add(bagItem);

            var promotionToAddBagIem = new BagItem 
            { 
                Quantity = promoAttributedQuantity, 
                Product = new BagItemProduct 
                { 
                    Id = relatedProduct, 
                    Price = new BagItemProductPrice 
                    { 
                        Id = relatedProductPriceId, 
                        BasePrice = relatedProductBasePrice 
                    } 
                } 
            };

            var promotionToAddProduct = new Product
            {
                Id = relatedProduct,
                Description = "somedescr",
                Name = "somename",
                Price = new Price
                {
                    Id = relatedProductPriceId,
                    BasePrice = relatedProductBasePrice
                }
            };

            var bagService = new Mock<IBagService>();
            var productService = new Mock<IProductService>();

            var addItemCommandHandler = new AddItemCommandHandler(bagService.Object, productService.Object);

            var commandDispatcher = new Mock<ICommandDispatcher>();
            var addItemCommand = new AddItemCommand(
                bag.Id,
                new BagItem
                {
                    Id = Guid.NewGuid(),
                    Product = new BagItemProduct { Id = relatedProduct },
                    Quantity = promoAttributedQuantity
                });

            commandDispatcher
                .Setup(instance => instance.Dispatch(It.IsAny<ICommand>()))
                .Callback(async () => await addItemCommandHandler.Handle(addItemCommand));


            var promotionsRepository = new Mock<IPromotionRepository>();
            
            promotionsRepository.Setup(pr => pr.Find(It.Is<Promotion>(instance => instance.For.Any(x => x.Id == productId)), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Promotion>
                {
                    new BuyXGetY (commandDispatcher.Object)
                    {
                        Id = mainPromotion,
                        For = new [] { new Product { Id = productId } },
                        BuyX = promoTargetQuantity,
                        GetY = relatedProduct
                    }
                });

            promotionsRepository
                .Setup(pr => pr.Find(It.Is<Promotion>(promo => promo.For.Any(x => x.Id == relatedProduct)), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Promotion>
                {
                    new PercentOff (commandDispatcher.Object)
                    {
                        Id = dependentPromotion,
                        For = new [] { new Product { Id = relatedProduct} },
                        Discount = discountToApply,
                        DependsOn = new [] { new Promotion { Id = mainPromotion } }
                    }
                });

            bagService.Setup(instance => instance.Find(It.IsAny<Bag>())).ReturnsAsync(bag);
            productService.Setup(instance => instance.Find(It.IsAny<Product>())).ReturnsAsync(new List<Product> { promotionToAddProduct });
            var sut = new PromotionService(promotionsRepository.Object);

            bagService.Setup(instance => instance.AddItem(It.IsAny<Bag>(), It.IsAny<BagItem>()))
                .ReturnsAsync(true)
                .Callback( async () =>
                {
                    bag.Items.Add(promotionToAddBagIem);
                    await sut.ApplyPromotions(bag, promotionToAddBagIem);
                });

            //act
            await sut.ApplyPromotions(bag, bagItem);

            //assert
            Assert.True(bagItem.AppliedPromotions.Any());
            Assert.True(promotionToAddBagIem.Product.Price.DiscountPrice == productBasePrice - (productBasePrice * discountToApply));
            Assert.True(bag.Items.Count == 2);
            Assert.Contains(bag.Items, item => item.Product.Id == productId);
            Assert.Contains(bag.Items, item => item.Product.Id == relatedProduct);
            Assert.True(bag.Items.Single(item => item.Product.Id == productId).AppliedPromotions.Single().Id == mainPromotion);
            Assert.True(bag.Items.Single(item => item.Product.Id == relatedProduct).AppliedPromotions.Single().Id == dependentPromotion);
        }

        [Fact]
        public async Task ApplyPromotions_BagItems_WithoutConditionsFor_PromotionsBuyXGetYWithPromotionZ_PromotionsAreNotApplied()
        {
            //arrange
            int productId = 123123;
            int productIdQty = 1;
            int relatedProduct = 456456;

            double discountToApply = 0.1;
            double productBasePrice = 10;
            int mainPromotion = 1;
            int dependentPromotion = 2;
            int promoTargetQuantity = 2;
            int promoAttributedQuantity = 1;
            int relatedProductPriceId = 1;
            double relatedProductBasePrice = 10;

            var bag = new Bag { Id = Guid.NewGuid() };
            var bagItem = new BagItem
            {
                Product = new BagItemProduct
                {
                    Id = productId,
                    Price = new BagItemProductPrice
                    {
                        BasePrice = productBasePrice
                    }
                },
                Quantity = productIdQty
            };

            bag.Items.Add(bagItem);

            var promotionToAddBagIem = new BagItem
            {
                Quantity = promoAttributedQuantity,
                Product = new BagItemProduct
                {
                    Id = relatedProduct,
                    Price = new BagItemProductPrice
                    {
                        Id = relatedProductPriceId,
                        BasePrice = relatedProductBasePrice
                    }
                }
            };

            var promotionToAddProduct = new Product
            {
                Id = relatedProduct,
                Description = "somedescr",
                Name = "somename",
                Price = new Price
                {
                    Id = relatedProductPriceId,
                    BasePrice = relatedProductBasePrice
                }
            };

            var bagService = new Mock<IBagService>();
            var productService = new Mock<IProductService>();

            var addItemCommandHandler = new AddItemCommandHandler(bagService.Object, productService.Object);

            var commandDispatcher = new Mock<ICommandDispatcher>();
            var addItemCommand = new AddItemCommand(
                bag.Id,
                new BagItem
                {
                    Id = Guid.NewGuid(),
                    Product = new BagItemProduct { Id = relatedProduct },
                    Quantity = promoAttributedQuantity
                });

            commandDispatcher
                .Setup(instance => instance.Dispatch(It.IsAny<ICommand>()))
                .Callback(async () => await addItemCommandHandler.Handle(addItemCommand));


            var promotionsRepository = new Mock<IPromotionRepository>();

            promotionsRepository.Setup(pr => pr.Find(It.Is<Promotion>(instance => instance.For.Any(x => x.Id == productId)), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Promotion>
                {
                    new BuyXGetY (commandDispatcher.Object)
                    {
                        Id = mainPromotion,
                        For = new [] { new Product { Id = productId } },
                        BuyX = promoTargetQuantity,
                        GetY = relatedProduct
                    }
                });

            promotionsRepository
                .Setup(pr => pr.Find(It.Is<Promotion>(promo => promo.For.Any(x => x.Id == relatedProduct)), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Promotion>
                {
                    new PercentOff (commandDispatcher.Object)
                    {
                        Id = dependentPromotion,
                        For = new [] { new Product { Id = relatedProduct} },
                        Discount = discountToApply,
                        DependsOn = new [] { new Promotion { Id = mainPromotion } }
                    }
                });

            bagService.Setup(instance => instance.Find(It.IsAny<Bag>())).ReturnsAsync(bag);
            productService.Setup(instance => instance.Find(It.IsAny<Product>())).ReturnsAsync(new List<Product> { promotionToAddProduct });
            var sut = new PromotionService(promotionsRepository.Object);

            bagService.Setup(instance => instance.AddItem(It.IsAny<Bag>(), It.IsAny<BagItem>()))
                .ReturnsAsync(true)
                .Callback(async () =>
                {
                    bag.Items.Add(promotionToAddBagIem);
                    await sut.ApplyPromotions(bag, promotionToAddBagIem);
                });

            //act
            await sut.ApplyPromotions(bag, bagItem);

            //assert
            Assert.False(bagItem.AppliedPromotions.Any());
            Assert.True(bag.Items.Count == 1);
            Assert.Contains(bag.Items, item => item.Product.Id == productId);
            Assert.DoesNotContain(bag.Items, item => item.Product.Id == relatedProduct);
        }

        [Fact]
        public async Task ApplyPromotions_BagItems_WithConditionsFor_PercentageOff_PromotionsAreApplied()
        {
            //arrange
            int productId = 123123;
            int productIdQty = 1;
            int sndProductId = 456456;

            double discountToApply = 0.1;
            double sndItemDiscountToApply = 0.05;
            double productBasePrice = 10;
            int mainPromotion = 1;
            int sndItemPromotion = 2;
            int relatedProductPriceId = 1;
            double relatedProductBasePrice = 10;

            var bag = new Bag { Id = Guid.NewGuid() };
            var bagItem = new BagItem
            {
                Product = new BagItemProduct
                {
                    Id = productId,
                    Price = new BagItemProductPrice
                    {
                        BasePrice = productBasePrice
                    }
                },
                Quantity = productIdQty
            };

            var sndBagItem = new BagItem
            {
                Product = new BagItemProduct
                {
                    Id = sndProductId,
                    Price = new BagItemProductPrice
                    {
                        Id = relatedProductPriceId,
                        BasePrice = relatedProductBasePrice
                    }
                },
                Quantity = productIdQty
            };

            bag.Items.Add(bagItem);
            bag.Items.Add(sndBagItem);

            var sndProduct = new Product
            {
                Id = sndProductId,
                Description = "somedescr",
                Name = "somename",
                Price = new Price
                {
                    Id = relatedProductPriceId,
                    BasePrice = relatedProductBasePrice
                }
            };

            var bagService = new Mock<IBagService>();
            var productService = new Mock<IProductService>();

            var addItemCommandHandler = new AddItemCommandHandler(bagService.Object, productService.Object);

            var commandDispatcher = new Mock<ICommandDispatcher>();

            var promotionsRepository = new Mock<IPromotionRepository>();

            promotionsRepository.Setup(pr => pr.Find(It.Is<Promotion>(instance => instance.For.Any(x => x.Id == productId)), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Promotion>
                {
                    new PercentOff (commandDispatcher.Object)
                    {
                        Id = mainPromotion,
                        For = new [] { new Product { Id = productId } },
                        Discount = discountToApply
                    }
                });

            promotionsRepository
                .Setup(pr => pr.Find(It.Is<Promotion>(promo => promo.For.Any(x => x.Id == sndProductId)), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Promotion>
                {
                    new PercentOff (commandDispatcher.Object)
                    {
                        Id = sndItemPromotion,
                        For = new [] { new Product { Id = sndProductId} },
                        Discount = sndItemDiscountToApply,
                        DependsOn = new [] { new Promotion { Id = mainPromotion } }
                    }
                });

            bagService.Setup(instance => instance.Find(It.IsAny<Bag>())).ReturnsAsync(bag);
            productService.Setup(instance => instance.Find(It.IsAny<Product>())).ReturnsAsync(new List<Product> { sndProduct });

            //act
            var sut = new PromotionService(promotionsRepository.Object);
            await sut.ApplyPromotions(bag, bagItem);
            await sut.ApplyPromotions(bag, sndBagItem);

            //assert
            Assert.True(bag.Items.Single(item => item.Product.Id == productId).AppliedPromotions.Single().Id == mainPromotion);
            Assert.True(bagItem.Product.Price.DiscountPrice == productBasePrice - (productBasePrice * discountToApply));

            Assert.True(bag.Items.Single(item => item.Product.Id == sndProductId).AppliedPromotions.Single().Id == sndItemPromotion);
            Assert.True(sndBagItem.Product.Price.DiscountPrice == productBasePrice - (productBasePrice * sndItemDiscountToApply));

        }

        [Fact]
        public async Task ApplyPromotions_BagItems_WithConditionsFor_BuyXGetYWithPromotionZ_And_PercentageOff_PromotionsAreApplied()
        {
            //arrange
            int productId = 123123;
            int productIdQty = 2;
            double productBasePrice = 10;

            int relatedProduct = 456456;
            double relatedProductBasePrice = 10;
            double discountToApply = 0.1;
            int promoTargetQuantity = 2;
            int promoAttributedQuantity = 1;
            int relatedProductPriceId = 1;

            int sndProductId = 989989;
            int sndProductIdQty = 1;
            int sndProductBasePrice = 10;
            double sndProductPromotion = 0.2;

            int mainPromotion = 1;
            int dependentPromotion = 2;
            int sndProdPromotionId = 3; 

            var bag = new Bag { Id = Guid.NewGuid() };
            var bagItem = new BagItem
            {
                Product = new BagItemProduct
                {
                    Id = productId,
                    Price = new BagItemProductPrice
                    {
                        BasePrice = productBasePrice
                    }
                },
                Quantity = productIdQty
            };

            var sndBagItem = new BagItem
            {
                Product = new BagItemProduct
                {
                    Id = sndProductId,
                    Price = new BagItemProductPrice
                    {
                        BasePrice = sndProductBasePrice
                    }
                },
                Quantity = sndProductIdQty
            };

            bag.Items.Add(bagItem);
            bag.Items.Add(sndBagItem);

            var promotionToAddBagIem = new BagItem
            {
                Quantity = promoAttributedQuantity,
                Product = new BagItemProduct
                {
                    Id = relatedProduct,
                    Price = new BagItemProductPrice
                    {
                        Id = relatedProductPriceId,
                        BasePrice = relatedProductBasePrice
                    }
                }
            };

            var promotionToAddProduct = new Product
            {
                Id = relatedProduct,
                Description = "somedescr",
                Name = "somename",
                Price = new Price
                {
                    Id = relatedProductPriceId,
                    BasePrice = relatedProductBasePrice
                }
            };

            var bagService = new Mock<IBagService>();
            var productService = new Mock<IProductService>();

            var addItemCommandHandler = new AddItemCommandHandler(bagService.Object, productService.Object);

            var commandDispatcher = new Mock<ICommandDispatcher>();
            var addItemCommand = new AddItemCommand(
                bag.Id,
                new BagItem
                {
                    Id = Guid.NewGuid(),
                    Product = new BagItemProduct { Id = relatedProduct },
                    Quantity = promoAttributedQuantity
                });

            commandDispatcher
                .Setup(instance => instance.Dispatch(It.IsAny<ICommand>()))
                .Callback(async () => await addItemCommandHandler.Handle(addItemCommand));


            var promotionsRepository = new Mock<IPromotionRepository>();

            promotionsRepository.Setup(pr => pr.Find(It.Is<Promotion>(instance => instance.For.Any(x => x.Id == productId)), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Promotion>
                {
                    new BuyXGetY (commandDispatcher.Object)
                    {
                        Id = mainPromotion,
                        For = new [] { new Product { Id = productId } },
                        BuyX = promoTargetQuantity,
                        GetY = relatedProduct
                    }
                });

            promotionsRepository
                .Setup(pr => pr.Find(It.Is<Promotion>(promo => promo.For.Any(x => x.Id == relatedProduct)), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Promotion>
                {
                    new PercentOff (commandDispatcher.Object)
                    {
                        Id = dependentPromotion,
                        For = new [] { new Product { Id = relatedProduct} },
                        Discount = discountToApply,
                        DependsOn = new [] { new Promotion { Id = mainPromotion } }
                    }
                });

            promotionsRepository
                .Setup(pr => pr.Find(It.Is<Promotion>(promo => promo.For.Any(x => x.Id == sndProductId)), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Promotion>
                {
                    new PercentOff (commandDispatcher.Object)
                    {
                        Id = sndProdPromotionId,
                        For = new [] { new Product { Id = sndProductId} },
                        Discount = sndProductPromotion,
                    }
                });

            bagService.Setup(instance => instance.Find(It.IsAny<Bag>())).ReturnsAsync(bag);
            productService.Setup(instance => instance.Find(It.IsAny<Product>())).ReturnsAsync(new List<Product> { promotionToAddProduct });

            var sut = new PromotionService(promotionsRepository.Object);

            bagService.Setup(instance => instance.AddItem(It.IsAny<Bag>(), It.IsAny<BagItem>()))
                .ReturnsAsync(true)
                .Callback(async () =>
                {
                    bag.Items.Add(promotionToAddBagIem);
                    await sut.ApplyPromotions(bag, promotionToAddBagIem);
                });

            //act
            await sut.ApplyPromotions(bag, bagItem);
            await sut.ApplyPromotions(bag, sndBagItem);

            //assert
            Assert.True(bagItem.AppliedPromotions.Any());
            Assert.True(promotionToAddBagIem.Product.Price.DiscountPrice == productBasePrice - (productBasePrice * discountToApply));
            Assert.True(bag.Items.Count == 3);
            Assert.Contains(bag.Items, item => item.Product.Id == productId);
            Assert.Contains(bag.Items, item => item.Product.Id == relatedProduct);
            Assert.Contains(bag.Items, item => item.Product.Id == sndProductId);

            Assert.True(bag.Items.Single(item => item.Product.Id == productId).AppliedPromotions.Single().Id == mainPromotion);
            Assert.True(bag.Items.Single(item => item.Product.Id == relatedProduct).AppliedPromotions.Single().Id == dependentPromotion);
            Assert.True(bag.Items.Single(item => item.Product.Id == sndProductId).AppliedPromotions.Single().Id == sndProdPromotionId);
        }
    }
}