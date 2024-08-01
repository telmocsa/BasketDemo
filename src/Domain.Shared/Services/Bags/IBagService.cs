using Domain.Models.Bags;
using Domain.Models.Users;

namespace Domain.Shared.Services.Bags
{
    public interface IBagService
    {
        Task<Bag> Create(User user);
        
        Task<Bag> Find(Bag bag);

        Task<bool> AddItem(Bag bag, BagItem bagItem);

        Task<bool> UpdateItem(Bag bag, BagItem bagItem);

        Task<bool> RemoveItem(Bag bag, BagItem bagItem);
    }
}
