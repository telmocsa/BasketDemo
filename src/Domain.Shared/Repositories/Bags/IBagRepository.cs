using Domain.Models.Bags;

namespace Domain.Shared.Repositories.Bags
{
    public interface IBagRepository    {
        Task<Bag> Find(Bag bag);

        Task<bool> Update(Bag bag);

        Task<bool> Create(Bag bag);
    }
}
