using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCloset.Models;
using MyCloset.Services.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCloset.Controllers
{
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        // TODO: Implement Add Account Controller
        /// <summary>
        /// Get User Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AccountDetails")]
        public async Task<IActionResult> GetUserDetails(Guid userId)
        {
            try
            {
                if (userId == null)
                {

                    ClosetActionResult? result = await _userService.GetUserDetails(userId);
                }
            }
            catch(Exception ex)
            {

            }
            throw new NotImplementedException();
        }

        // TODO: Implement Add Account Controller
        /// <summary>
        /// Set up a new user account in the application
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddAccount")]
        public async Task<IActionResult> AddAccount()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Edit the current users account
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("EditAccount")]
        public async Task<IActionResult> EditAccount()
        {
            throw new NotImplementedException();
        }

        // TODO: Implement Delete Account Controller
        /// <summary>
        /// Delete the current users account from the application
        /// </summary>
        /// <param name="isPermenant"> True, delete all data. False, save data for 1 year. </param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete]
        [Route("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount(bool isPermanent)
        {
            ClosetActionResult result = await _userService.DeleteUser(await GetCurrentUserGuid(), isPermanent);
            throw new NotImplementedException();
        }
    }
}

