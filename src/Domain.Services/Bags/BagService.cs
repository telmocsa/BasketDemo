using Domain.Models.Bags;
using Domain.Models.Users;
using Domain.Shared.Repositories.Bags;
using Domain.Shared.Services.Bags;
using Domain.Shared.Services.Promotions;

namespace Domain.Services.Bags
{
    public class BagService : IBagService
    {
        private readonly IBagRepository bagRepository;
        private readonly IPromotionService promotionService;

        public BagService(IBagRepository bagRepository, IPromotionService promotionService) { 
            this.bagRepository = bagRepository;
            this.promotionService = promotionService;
        }

        public async Task<Bag> Create(User user)
        {
            var bag = await bagRepository.Find(new Bag { OwnerId = user.Id });

            if (bag != null)
            {
                return bag;
            }
            
            bag = new Bag { Id = Guid.NewGuid(), OwnerId = user.Id };
            await bagRepository.Create(bag);
         
            return bag;
        }

        public async Task<bool> AddItem(Bag bag, BagItem bagItem)
        {
            bag.Items.Add(bagItem);
            var result = await bagRepository.Update(bag);
            
            await promotionService.ApplyPromotions(bag.Id, bagItem);

            return result;
        }

        public async Task<bool> RemoveItem(Bag bag, BagItem bagItem)
        {
            bag.Items.Remove(bagItem);
            var result = await bagRepository.Update(bag);
            return result;
        }

        public async Task<bool> UpdateItem(Bag bag, BagItem bagItem)
        {
            await promotionService.ApplyPromotions(bag.Id, bagItem);

            var changedItem = bag.Items.IndexOf(bagItem);
            
            if (changedItem == -1)
            {
                return false;
            }

            bag.Items[changedItem] = bagItem;

            var result = await bagRepository.Update(bag);
            return result;
        }

        public async Task<Bag> Find(Bag bag) 
        { 
            return await bagRepository.Find(bag);
        }
    }
}
