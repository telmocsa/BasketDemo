using Domain.Models.Promotions;

namespace Domain.Models.Bags
{
    public class BagItem
    {
        public Guid Id { get; set; }

        public BagItemProduct Product { get; set; } = new BagItemProduct();

        public int Quantity { get; set; }

        public List<Promotion> AppliedPromotions { get; set; } = [];
    }
}
