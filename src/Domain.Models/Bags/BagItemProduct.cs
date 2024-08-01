namespace Domain.Models.Bags
{
    public class BagItemProduct
    {
        public int Id { get; set; } 

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public BagItemProductPrice Price { get; set; } = new BagItemProductPrice();
    }
}
