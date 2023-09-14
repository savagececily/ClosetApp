using System;
using MyCloset.Models;
using MyCloset.RequestModels;

namespace MyCloset
{
    public interface IMyClosetService
    {
        public Task<List<ClothingItem>> GetClosetItems(Guid userId);
        public Task<ClothingItem?> GetClosetItem(Guid clothingItemId);
        public Task<List<Outfit>> GetOutfits(Guid userId);
        public Task<Outfit?> GetOutfit(Guid outfitId);
        public Task SaveClothingItem(ClothingItemRequest clothingItem);
        public Task SaveOutfit(OutfitRequest newOutfit);
        public Task DeleteClothingItems(List<Guid> clothingItemIds);
        public Task DeleteOutfits(List<Guid> outfitIds);
        public Task<List<User>> GetFriends(Guid currentUserId);
        public Task RequestFriend(Guid currentUser, Guid requestedFriend);
        public Task FriendRequestResponse(Guid friendshipId, bool accepted);
        public Task DeleteFriend(Guid currentUser, Guid deletedFriend);
    }
}

