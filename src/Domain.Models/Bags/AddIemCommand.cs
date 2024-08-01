using Domain.Core.Commands;

namespace Domain.Models.Bags
{
    public class AddItemCommand : ICommand
    {
        public BagItem BagItem{ get; }

        public Guid BagId { get; }

        public AddItemCommand(Guid bagId, BagItem bagItem)
        {
            this.BagItem = bagItem;
            this.BagId = bagId; 
        }
    }
}
