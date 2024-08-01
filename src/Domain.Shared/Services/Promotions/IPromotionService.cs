using Domain.Models.Bags;
using Domain.Models.Promotions;

namespace Domain.Shared.Services.Promotions
{
    public interface IPromotionService
    {
        Task ApplyPromotions(Bag bag, BagItem bagItem);

        Task<IEnumerable<Promotion>> FindAsync(Promotion promotion);
    }
}
