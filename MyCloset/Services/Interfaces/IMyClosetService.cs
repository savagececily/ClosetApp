using System;
using MyCloset.Models;
using MyCloset.Models.RequestModels;

namespace MyCloset.Services.Interfaces
{
    public interface IMyClosetService
    { 
        public Task<ClosetActionResult> GetClosetItems(Guid userId);
        public Task<ClosetActionResult> GetClosetItem(Guid clothingItemId);
        public Task<ClosetActionResult> GetOutfits(Guid userId);
        public Task<ClosetActionResult> GetOutfit(Guid outfitId);
        public Task<ClosetActionResult> SaveClothingItem(ClothingItemRequest clothingItem);
        public Task<ClosetActionResult> SaveOutfit(OutfitRequest newOutfit);
        public Task<ClosetActionResult> DeleteClothingItems(List<Guid> clothingItemIds, Guid currentUser);
        public Task<ClosetActionResult> DeleteOutfits(List<Guid> outfitIds, Guid currentUser);
    }
}

