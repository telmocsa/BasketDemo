using Domain.Core.Commands;
using Domain.Models.Bags;
using Domain.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Promotions.Strategies
{
    public class PercentOff : Strategy
    {
        public PercentOff(ICommandDispatcher commandDispatcher) : base(commandDispatcher)
        {
        }

        public double Discount { get; set; }

        public override void Apply(Guid bagId, BagItem bagItem)
        {
            bagItem.Product.Price.DiscountPrice = bagItem.Product.Price.BasePrice - (bagItem.Product.Price.BasePrice * Discount);
        }
    }
}
