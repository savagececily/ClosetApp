using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCloset.Models;
using MyCloset.Models.RequestModels;
using MyCloset.Services.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCloset.Controllers
{
    public class ClosetController : BaseController
    {
        private readonly ILogger<ClosetController> _logger;
        private readonly IMyClosetService _closetService;

        public ClosetController(ILogger<ClosetController> logger, IMyClosetService closetService)
        {
            _logger = logger;
            _closetService = closetService;
        }

#region Closet
        /// <summary>
        ///  Get closet items for the current user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMyCloset")]
        public async Task<IActionResult> GetCloset()
        {
            Guid currentUser = await GetCurrentUserGuid();
            try
            {
                ClosetActionResult result = await _closetService.GetClosetItems(currentUser);

                return ResultHelper(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred while fetching closet items for user: {currentUser}. {ex.Message}");

                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while fetching your clothing items."
                });
            }
        }

        /// <summary>
        ///  Get closet items for a specific user
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCloset")]
        public async Task<IActionResult> GetCloset(Guid userId)
        {
            try
            {
                ClosetActionResult result = await _closetService.GetClosetItems(userId);

                return ResultHelper(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred while fetching closet items for user: {userId}. {ex.Message}");

                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while fetching your clothing items."
                });
            }
        }

        /// <summary>
        /// Save clothing item details
        /// </summary>
        /// <param name="clothingItem"></param>
        /// <returns></returns>
        [HttpPost("SaveClothingItem")]
        public async Task<IActionResult> SaveClothingItem([FromForm] ClothingItemRequest clothingItem)
        {
            try
            {
                clothingItem.UserId = await GetCurrentUserGuid();
                ClosetActionResult result = await _closetService.SaveClothingItem(clothingItem);

                return ResultHelper(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred during add/update of clothing item \'{clothingItem.Title}\'. {ex.Message}");
                
                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while updating your closet with clothing item \'{clothingItem.Title}\'."
                });
            }
        }

        /// <summary>
        /// Delete clothing items from closet
        /// </summary>
        /// <param name="clothingItemIds"></param>
        /// <returns></returns>
        [HttpDelete("DeleteClothingItems")]
        public async Task<IActionResult> DeleteClothingItemsAsync(List<Guid> clothingItemIds)
        {
            try
            {
                ClosetActionResult result = await _closetService.DeleteClothingItems(clothingItemIds, await GetCurrentUserGuid());
                return ResultHelper(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred while deleting the following clothing items: {string.Join(',', clothingItemIds)}. {ex.Message}");

                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while deleting items from your closet."
                });
            }
        }
#endregion

#region Outfits
        // TODO: Add GetOutfits by user id
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOutfits")]
        public async Task<IActionResult> GetOutfits()
        {
            Guid currentUser = await GetCurrentUserGuid();
            try
            {
                ClosetActionResult result = await _closetService.GetOutfits(currentUser);

                return ResultHelper(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred while fetching outfits for user: {currentUser}. {ex.Message}");

                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while fetching your outfits."
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOutfits/userId")]
        public async Task<IActionResult> GetOutfits(Guid userId)
        {
            try
            {
                ClosetActionResult result = await _closetService.GetOutfits(userId);

                return ResultHelper(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred while fetching outfits for user: {userId}. {ex.Message}");

                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while fetching your outfits."
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outfit"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveOutfit")]
        public async Task<IActionResult> SaveOutfit([FromForm] OutfitRequest outfit)
        {
            try
            {
                outfit.UserId = await GetCurrentUserGuid();
                ClosetActionResult result = await _closetService.SaveOutfit(outfit);

                return ResultHelper(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred during add/update of clothing item \'{outfit.Title}\'. {ex.Message}");

                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while updating your closet with clothing item \'{outfit.Title}\'."
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outfitIds"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteOutfits")]
        public async Task<IActionResult> DeleteOutfits(List<Guid> outfitIds)
        {
            try
            {
                ClosetActionResult result = await _closetService.DeleteOutfits(outfitIds, await GetCurrentUserGuid());
                return ResultHelper(result);
            }
            catch(Exception ex)
            {
                // TODO: List items by id in error log
                _logger.LogError(ex, $"An exception occurred while deleting the following outfits: . {ex.Message}");

                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while deleting items from your closet."
                });
            }
        }
#endregion
    }
}

