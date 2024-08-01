namespace Domain.Models.Products
{
    public class Product
    {
        public int Id { get; set; } 

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Price Price { get; set; } = new Price();
    }
}
