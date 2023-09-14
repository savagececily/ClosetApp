using System;
using Microsoft.EntityFrameworkCore;
using MyCloset.Models;
using MyCloset.RequestModels;
using Newtonsoft.Json;

namespace MyCloset
{
    public class MyClosetService : IMyClosetService
    {
        private readonly MyClosetAppDbContext _dbContext;
        private readonly ILogger<MyClosetService> _logger;

        public MyClosetService(ILogger<MyClosetService> logger, MyClosetAppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task SaveClothingItem(ClothingItemRequest clothingItem)
        {
            try
            {
                if (clothingItem.ClothingItemId.HasValue)
                {
                    // TODO: Validate Current User

                    ClothingItem? existingClothingItem = await _dbContext.ClothingItems
                        .FirstOrDefaultAsync(x => x.ClothingItemId == clothingItem.ClothingItemId);

                    if(existingClothingItem == null)
                    {
                        // TODO: Throw Validation Error, Not Found
                        return;
                    }

                    if(clothingItem.Title != existingClothingItem.Title)
                    {
                        // TODO: Throw Validadtion Error, Title cannot be changed
                    }

                    existingClothingItem.Description = clothingItem.Description;
                    existingClothingItem.Category = clothingItem.Category;
                    // TODO: Update Occasion List 
                }
                else
                {
                    if(!await _dbContext.ClothingItems.AnyAsync(x => x.Title == clothingItem.Title))
                    {
                        // TODO: Throw Validation Error
                        return;
                    }

                    await _dbContext.ClothingItems.AddAsync(new ClothingItem
                    {
                        Title = clothingItem.Title,
                        Description = clothingItem.Description,
                        UserId = clothingItem.UserId,
                        Category = clothingItem.Category,
                        Occasions = JsonConvert.SerializeObject(clothingItem.Occasions)
                        // TODO: Get Generated Occasions append to submitted occasions
                        // TODO: Create & Get Photo Link
                    }); ; ;
                }

                await _dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        public Task SaveOutfit(OutfitRequest newOutfit)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteClothingItems(List<Guid> clothingItemIds)
        {
            try
            {
                IQueryable<ClothingItem> clothingItemsToDelete = _dbContext.ClothingItems
                    .Include(x => x.Outfits)
                    .Where(x => clothingItemIds.Contains(x.ClothingItemId));

                _dbContext.ClothingItems.RemoveRange(clothingItemsToDelete);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        public async Task DeleteFriend(Guid currentUser, Guid deletedFriend)
        {
                try
                {
                    UserFriend? friendship = await _dbContext.UserFriends
                        .FirstOrDefaultAsync(x => (x.UserId1 == currentUser && x.UserId2 == deletedFriend) || (x.UserId1 == deletedFriend && x.UserId2 == currentUser));

                    if (friendship != null)
                    {
                        _dbContext.UserFriends.Remove(friendship);
                    }

                    await _dbContext.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    // TODO: Add Logging and Exception Handling 
                    throw new NotImplementedException();
                }
         }

        public async Task DeleteOutfits(List<Guid> outfitIds)
        {
            try
            {
                IQueryable<Outfit> outfitsToDelete = _dbContext.Outfits
                .Where(x => outfitIds.Contains(x.OutfitId));

                _dbContext.Outfits.RemoveRange(outfitsToDelete);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        public async Task FriendRequestResponse(Guid friendshipId, bool accepted)
        {
            try
            {
                UserFriend? friendship = await _dbContext.UserFriends
                    .FirstOrDefaultAsync(x => x.FriendshipId == friendshipId);

                if (friendship != null)
                {
                    _dbContext.UserFriends.Remove(friendship);
                }

                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        public async Task<ClothingItem?> GetClosetItem(Guid clothingItemId)
        {
            try
            {
                return await _dbContext.ClothingItems.FirstOrDefaultAsync(x => x.ClothingItemId == clothingItemId);
            }
            catch (Exception ex)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        public async Task<List<ClothingItem>> GetClosetItems(Guid userId)
        {
            try
            {
                return await _dbContext.ClothingItems.Where(x => x.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        public async Task<List<User>> GetFriends(Guid currentUserId)
        {
            try
            {
                 // TODO: Select Users that are friends of the current user 
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        public async Task<Outfit?> GetOutfit(Guid outfitId)
        {
            try
            {
                return await _dbContext.Outfits.FirstOrDefaultAsync(x => x.OutfitId == outfitId);
            }
            catch (Exception ex)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        public async Task<List<Outfit>> GetOutfits(Guid userId)
        {
            try
            {
                _logger.LogInformation($"Getting outfits for user: {userId}");

                return await _dbContext.Outfits.Where(x => x.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        public async Task RequestFriend(Guid currentUser, Guid requestedFriend)
        {
            try
            {
                bool friendshipExists = await _dbContext.UserFriends
                    .AnyAsync(x => (x.UserId1 == currentUser && x.UserId2 == requestedFriend) || (x.UserId1 == requestedFriend && x.UserId2 == currentUser));

                if (friendshipExists)
                {
                    // TODO: Throw exception - cannot request friend where frienship already exists
                    return;
                }

                await _dbContext.UserFriends.AddAsync(new UserFriend
                {
                    UserId1 = currentUser,
                    UserId2 = requestedFriend,
                    Status = false
                });

                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }
    }
}

