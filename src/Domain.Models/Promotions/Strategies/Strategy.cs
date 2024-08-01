using Domain.Core.Commands;
using Domain.Models.Bags;

namespace Domain.Models.Promotions.Strategies
{
    public abstract class Strategy
    {
        protected Strategy(ICommandDispatcher commandDispatcher) { this.CommandDispatcher = commandDispatcher; }

        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        protected ICommandDispatcher CommandDispatcher { get; }

        public abstract void Apply(Guid bagId, BagItem bagItem);
    }
}