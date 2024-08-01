using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Bags
{
    public class Bag
    {
        public Guid Id { get; set; }

        public int OwnerId { get; set; }

        public List<BagItem> Items { get; set; } = [];
    }
}
