using Microsoft.AspNetCore.Mvc;
using MyCloset.Models;
using MyCloset.Models.DBModels;
using MyCloset.Models.RequestModels;
using MyCloset.Services.Interfaces;
using MyCloset.Utilities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCloset.Controllers
{
    public class FriendsController : BaseController
    {
        private readonly ILogger<FriendsController> _logger;
        private readonly IFriendService _friendService;

        public FriendsController(ILogger<FriendsController> logger, IFriendService friendService)
        {
            _logger = logger;
            _friendService = friendService;
        }

        /// <summary>
        /// Get friends list for the current user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("GetFriends")]
        public async Task<IActionResult> GetFriends(Guid userId)
        {
            try
            {
                ClosetActionResult result = await _friendService.GetFriends(userId);
                return ResultHelper(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred while getting friend list for user {userId}. {ex.Message}");

                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while fetching your friend list."
                });
            }

        }

        /// <summary>
        /// Create, Accept/Decline, Block/Remove friendship
        /// </summary>
        /// <param name="friendshipRequest"></param>
        /// <returns></returns>
        [HttpPost("EditFriendship")]
        public async Task<IActionResult> EditFriendship([FromBody] FriendshipRequest friendshipRequest)
        {
            try
            {
                ClosetActionResult result = await _friendService.EditFriendship(friendshipRequest);
                return ResultHelper(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred while get updating friendship for user {friendshipRequest.User1}. {ex.Message}");

                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while {friendshipRequest.RequestType.ToActionString()}."
                });
            }
        }

        /// <summary>
        /// Get blocked list
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("BlockedList")]
        public async Task<IActionResult> GetBlockedList(Guid userId)
        {
            try
            {
                ClosetActionResult result = await _friendService.GetBlockedList(userId);
                return ResultHelper(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred while getting blocked list for user {userId}. {ex.Message}");

                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while fetching your blocked list."
                });
            }
        }

        /// <summary>
        /// Get pending friendship requests
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("PendingRequests")]
        public async Task<IActionResult> GetFriendRequests(Guid userId)
        {
            try
            {
                ClosetActionResult result = await _friendService.GetFriendRequests(userId);
                return ResultHelper(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred while getting friend list for user {userId}. {ex.Message}");

                return ResultHelper(new ClosetActionResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = $"Oops! Something went wrong while fetching your pending friend requests."
                });
            }
        }
    }
}

