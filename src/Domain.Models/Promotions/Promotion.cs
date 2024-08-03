using Domain.Core.Commands;
using Domain.Models.Bags;
using Domain.Models.Products;
using Domain.Models.Promotions.Strategies;

namespace Domain.Models.Promotions
{
    public class Promotion
    {
        public Promotion() { }  

        public Promotion(ICommandDispatcher commandDispatcher) 
        { 
            this.CommandDispatcher = commandDispatcher; 
        }

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<Promotion> DependsOn { get; set; } = [];

        public IEnumerable<Product> For { get; set; } = [];

        protected ICommandDispatcher CommandDispatcher { get; }

        public virtual void Apply(Guid bagId, BagItem bagItem)
        {
            if (bagItem != null)
            {
                bagItem.AppliedPromotions.Add(this);
            }
        }
    }
}
