using System;
using System.Net;
using Microsoft.EntityFrameworkCore;
using MyCloset.Models;
using MyCloset.Models.DBModels;
using MyCloset.Models.RequestModels;
using MyCloset.Models.ResponseModels;
using MyCloset.Services.Interfaces;
using Newtonsoft.Json;

namespace MyCloset.Services.Implementation
{
    public class MyClosetService : IMyClosetService
    {
        private readonly MyClosetAppDbContext _dbContext;
        private readonly ILogger<MyClosetService> _logger;
        private readonly IBlobStorageService _blobStorageService;

        public MyClosetService(ILogger<MyClosetService> logger, MyClosetAppDbContext dbContext, IBlobStorageService blobStorageService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _blobStorageService = blobStorageService;
        }

        public async Task<ClosetActionResult> SaveClothingItem(ClothingItemRequest clothingItem)
        {

            if (clothingItem.Id.HasValue)
            {
                ClothingItem? existingClothingItem = await _dbContext.ClothingItems
.                   FirstOrDefaultAsync(x => x.ClothingItemId == clothingItem.Id);
                return await UpdateClothingItem(clothingItem, existingClothingItem);
            }
            else
            {
                return await AddClothingItem(clothingItem);

            }
        }

        private async Task<ClosetActionResult> AddClothingItem(ClothingItemRequest newClothingItem)
        {
            try
            {
                if (await _dbContext.ClothingItems.AnyAsync(x => x.Title == newClothingItem.Title && x.UserId == newClothingItem.UserId))
                {
                    _logger.LogError($"User: {newClothingItem.UserId} tried to add clothing item: {newClothingItem.Title} but this title already exists.");

                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = $"The title, \'{newClothingItem.Title}\', already exists."
                    };
                }

                ClothingItem clothingItemToAdd = new ClothingItem
                {
                    id = Guid.NewGuid().ToString(),
                    ClothingItemId = Guid.NewGuid(),
                    Title = newClothingItem.Title,
                    Description = newClothingItem.Description,
                    UserId = newClothingItem.UserId.HasValue ? newClothingItem.UserId.Value : throw new Exception("User Id was not provided during add new clothing item."),
                    Category = newClothingItem.Category,
                    Tags = newClothingItem.Tags ?? new List<string>(),
                    LinkToPhoto = await _blobStorageService.UploadImageAsync(newClothingItem.UserId.Value, newClothingItem.Image)
                };

                await _dbContext.ClothingItems.AddAsync(clothingItemToAdd);

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User: {newClothingItem.UserId} successfully saved clothing item: {newClothingItem.Title}.");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"\'{newClothingItem.Title}\' was successfully added to your closet."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown while adding a new item to closet for user: {newClothingItem.UserId}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while adding \'{newClothingItem.Title}\' to your closet."
                };
            }

        }

        private async Task<ClosetActionResult> UpdateClothingItem(ClothingItemRequest updatedClothingItem, ClothingItem existingClothingItem)
        {
            try
            {
                if (existingClothingItem == null)
                {
                    _logger.LogError($"User: {updatedClothingItem.UserId} tried to update clothing item: {updatedClothingItem.Id} but it was not found.");

                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = $"{updatedClothingItem.Title} was not found."
                    };
                }

                if (existingClothingItem.UserId != updatedClothingItem.UserId)
                {
                    _logger.LogError($"User: {updatedClothingItem.UserId} tried to update clothing item: {updatedClothingItem.Id} but this item belongs to user: {existingClothingItem.UserId}.");

                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        Message = $"\'{existingClothingItem.Title}\' is not part of your closet."
                    };
                }

                if (updatedClothingItem.Title != existingClothingItem.Title)
                {
                    _logger.LogError($"User: {updatedClothingItem.UserId} tried to update clothing item: {updatedClothingItem.Id} but title cannot be changed.");

                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = $"The title for \'{existingClothingItem.Title}\' cannot be changed."
                    };
                }

                existingClothingItem.Description = updatedClothingItem.Description;
                existingClothingItem.Category = updatedClothingItem.Category;

                // Merge tags
                var existingTags = existingClothingItem.Tags ?? new List<string>();
                if (updatedClothingItem.Tags != null && updatedClothingItem.Tags.Any())
                {
                    existingTags.AddRange(updatedClothingItem.Tags);
                    existingClothingItem.Tags = existingTags.Distinct().ToList();
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User: {updatedClothingItem.UserId} successfully saved clothing item: {updatedClothingItem.Title}.");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"\'{updatedClothingItem.Title}\' was successfully updated in your closet."
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown while updating clothing item {existingClothingItem.ClothingItemId}. {ex.Message} ");
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while updating {existingClothingItem.ClothingItemId}."
                };
            }
        }

        // TODO: Implement Save Outfit service method
        public async Task<ClosetActionResult> SaveOutfit(OutfitRequest outfit)
        {
            throw new NotImplementedException();
        }

        // TODO: update method to have closet action result
        public async Task<ClosetActionResult> DeleteClothingItems(List<Guid> clothingItemIds, Guid currentUser)
        {
            try
            {
                IQueryable<ClothingItem> clothingItemsToDelete = _dbContext.ClothingItems
                    .Where(x => clothingItemIds.Contains(x.ClothingItemId));


                if(clothingItemsToDelete.Any(x => x.UserId != currentUser))
                {
                    // TODO: Log error 
                    _logger.LogError($"");
                    // TODO: add list of items that were not accessible by the current user
                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        Message = $"You do not have access to delete: .",
                    };
                }

                _dbContext.ClothingItems.RemoveRange(clothingItemsToDelete);

                await _dbContext.SaveChangesAsync();

                // TODO: Add Logging and Result
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown during {this.GetType().Name}. {ex.Message} ");
                // TODO: List clothing items by name
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while deleting the following clothing items from your closet: ."
                };
            }
        }

        // TODO: update method to have closet action result
        public async Task<ClosetActionResult> DeleteOutfits(List<Guid> outfitIds, Guid currentUser)
        {
            throw new NotImplementedException();
            // TODO: Review Cascaded items
            // TODO: validate current user has permission to delete 
            try
            {
                IQueryable<Outfit> outfitsToDelete = _dbContext.Outfits
                .Where(x => outfitIds.Contains(x.OutfitId));

                _dbContext.Outfits.RemoveRange(outfitsToDelete);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        // TODO: Modify to have a ClosetActionResult
        public async Task<ClosetActionResult> GetClosetItem(Guid clothingItemId)
        {
            try
            {
                ClothingItem? clothingItem = await _dbContext.ClothingItems.FirstOrDefaultAsync(x => x.ClothingItemId == clothingItemId);

                throw new NotImplementedException(); 
            }
            catch (Exception ex)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        // TODO: update method to return closet action result
        public async Task<ClosetActionResult> GetClosetItems(Guid userId)
        {
            try
            {
                List<ClothingItem> clothingItems = await _dbContext.ClothingItems.Where(x => x.UserId == userId).ToListAsync();


                List<Task<ClothingItemResponse>> tasks = clothingItems.Select(async x =>
                {

                    var response = new ClothingItemResponse
                    {
                        ClothingItemId = x.ClothingItemId,
                        Title = x.Title,
                        Description = x.Description,
                        Category = x.Category,
                        Tags = x.Tags ?? new List<string>(),
                        UserId = x.UserId,
                        Image = Convert.ToBase64String(await _blobStorageService.GetImageAsync(x.LinkToPhoto))
                    };

                    return response;
                }).ToList();

                await Task.WhenAll(tasks);

                List<ClothingItemResponse> clothingItemResponses = tasks.Select(task => task.Result).ToList();

                _logger.LogInformation($"Found {clothingItems.Count()} closet items for user {userId}.");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = clothingItemResponses
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown while fetching closet items for: {userId}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while getting your closet items.."
                };
            }
        }

        // TODO: update method to return closet action result
        public async Task<ClosetActionResult> GetOutfit(Guid outfitId)
        {
            try
            {

                Outfit? outfit = await _dbContext.Outfits.FirstOrDefaultAsync(x => x.OutfitId == outfitId);
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        public async Task<ClosetActionResult> GetOutfits(Guid userId)
        {
            try
            {
                _logger.LogInformation($"Getting outfits for user: {userId}");

                List<Outfit> Outfits = await _dbContext.Outfits.Where(x => x.UserId == userId).ToListAsync();
                //TODO: evaluate creating an additional return type to include all data for each item

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                // TODO: Add more context to log
                _logger.LogError(ex, ex.Message);
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }
    }
}

