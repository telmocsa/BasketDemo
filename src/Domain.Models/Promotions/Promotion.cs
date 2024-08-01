using Domain.Models.Products;
using Domain.Models.Promotions.Strategies;

namespace Domain.Models.Promotions
{
    public class Promotion
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Strategy Strategy { get; set; }

        public IEnumerable<Promotion> DependsOn { get; set; } = [];

        public IEnumerable<Product> For { get; set; } = [];
    }
}
