using Domain.Core.Commands;
using Domain.Models.Bags;
using Domain.Models.Products;
using Domain.Shared.Services.Bags;
using Domain.Shared.Services.Products;

namespace Domain.Services.Bags.CommandHandlers 
{
    public class AddItemCommandHandler : ICommandHandler<AddItemCommand>
    {
        private readonly IBagService bagService;
        private readonly IProductService productService;

        public AddItemCommandHandler(IBagService bagService, IProductService productService) 
        {
            this.bagService = bagService;
            this.productService = productService;
        }

        public async Task Handle(AddItemCommand command)
        {
            var bag = await bagService.Find(new Bag { Id = command.BagId });
            var product = await productService.Find( new Product { Id = command.BagItem.Product.Id });

            await bagService.AddItem(
                bag, 
                new BagItem 
                { 
                    Id = command.BagItem.Id,
                    Product = CreateBagItemProduct(product.FirstOrDefault()),
                    Quantity = command.BagItem.Quantity                    
                });
        }

        private BagItemProduct CreateBagItemProduct(Product product)
        {
            return new BagItemProduct { Id = product.Id, Description = product.Description, Name = product.Name, Price = CreateBagItemProductPrice(product.Price) };
        }

        private BagItemProductPrice CreateBagItemProductPrice(Price price)
        {
            return new BagItemProductPrice { Id = price.Id, BasePrice = price.BasePrice};
        }
    }
}
