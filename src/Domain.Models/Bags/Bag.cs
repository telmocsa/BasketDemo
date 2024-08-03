using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Bags
{
    public class Bag
    {
        public Guid Id { get; set; }

        public int OwnerId { get; set; }

        public List<BagItem> Items { get; set; } = [];

        public double TotalPriceWithouDiscount { get { return Items.Sum(item => item.Product.Price.BasePrice); } }

        public double TotalDiscounts { get { return Items.Sum(item => item.Product.Price.BasePrice - item.Product.Price.DiscountPrice); } }

        public double FinalPrice { get { return TotalPriceWithouDiscount - TotalDiscounts; } }
    }
}
