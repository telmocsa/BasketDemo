using Domain.Models.Bags;
using Domain.Models.Products;
using Domain.Models.Promotions;
using Domain.Shared.Repositories.Promotions;
using Domain.Shared.Services.Promotions;

namespace Domain.Services.Promotions
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository promotionRepository;

        public PromotionService(IPromotionRepository promotionRepository)
        {
            this.promotionRepository = promotionRepository;
        }

        public async Task ApplyPromotions(Bag bag, BagItem bagItem)
        {
            var promotions = await promotionRepository.Find(new Promotion { For = new[] { new Product { Id = bagItem.Product.Id } } }, DateTime.UtcNow);
            foreach (var promo in promotions)
            {
                if (promo.DependsOn.All(dep => VerifyDependency(bag, promo)))
                {
                    promo.Apply(bag.Id, bagItem);
                }
            }
        }

        public async Task<IEnumerable<Promotion>> FindAsync(Promotion promotion)
        {
            return await promotionRepository.Find(promotion);
        }
        
        private bool VerifyDependency(Bag bag, Promotion promotion)
        {
            return bag.Items.Any(item => item.AppliedPromotions.Any(promo => promo == promotion));
        }
    }
}
