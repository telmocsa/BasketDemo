using Domain.Core.Commands;
using Domain.Models.Bags;
using Domain.Models.Products;

namespace Domain.Models.Promotions.Strategies
{
    public class BuyXGetY : Promotion
    {
        public BuyXGetY(ICommandDispatcher commandDispatcher) : base(commandDispatcher)
        {
        }

        public int BuyX { get; set; }

        public int GetY { get; set; }

        public override void Apply(Guid bagId, BagItem bagItem)
        {
            if(bagItem.Quantity >= BuyX)
            {
                base.Apply(bagId, bagItem);

                CommandDispatcher.Dispatch(
                    new AddItemCommand(
                        bagId, 
                        new BagItem
                        { 
                            Id = Guid.NewGuid(),
                            Product = new BagItemProduct { Id = GetY }, 
                            Quantity = 1
                        }));
            }
        }
    }
}
