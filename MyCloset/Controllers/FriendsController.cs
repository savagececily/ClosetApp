using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCloset.Services.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCloset.Controllers
{
    public class FriendsController : BaseController
    {
        private readonly ILogger<FriendsController> _logger;
        private readonly IFriendService _friendService;

        // TODO: Add friend service via DI
        public FriendsController(ILogger<FriendsController> logger, IFriendService friendService)
        {
            _logger = logger;
            _friendService = friendService;
        }

        // TODO: Implement Get Friends Controller
        /// <summary>
        /// Get friends list for the current user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet]
        [Route("GetFriends")]
        public async Task<IActionResult> GetFriends()
        {
            throw new NotImplementedException();
        }

        // TODO: Implement Request Freind Controller
        /// <summary>
        /// Request friend for the current user
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> RequestFriend(Guid requestedFriend)
        {
            throw new NotImplementedException();
        }

        // TODO: Implement Friend Request Response Controller
        /// <summary>
        /// Process response to friend request
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("FriendRequestResponse")]
        public async Task<IActionResult> FriendRequestResponse()
        {
            throw new NotImplementedException();
        }

        // TODO: Implement Remove Friend Controller
        /// <summary>
        /// Remove friend from the current users list
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("RemoveFriend")]
        public async Task<IActionResult> RemoveFriend(Guid deletedFriend)
        {
            throw new NotImplementedException();
        }
    }
}

