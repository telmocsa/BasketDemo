using Domain.Models.Promotions;

namespace Domain.Shared.Repositories.Promotions
{
    public interface IPromotionRepository
    {
        Task<IEnumerable<Promotion>> Find(Promotion promotion, DateTime dateTime = default);
    }
}