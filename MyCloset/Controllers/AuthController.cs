using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCloset.Models;
using MyCloset.Services.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCloset.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;

        public AuthController(ILogger<AuthController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        [Route("/login/{provider}")]
        public IActionResult Login(string provider)
        {
            // Redirect to the external provider's authentication page
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, provider);
        }

        [HttpPost]
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        [HttpPost()]
        [Route("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            // Get user information from the external authentication provider
            AuthenticateResult externalUserInfo = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            // Check if the user already exists in your database
            ClosetActionResult userExists = await _userService.GetUserDetails(await GetCurrentUserGuid());

            if (userExists != null)
            {
                // TODO: implement response 
                return Redirect("/"); // Redirect to the dashboard after login
            }
            else
            {
                // TODO: Redirect to complete sign up
                return RedirectToAction("CompleteSignup");
            }
        }

        [HttpPost]
        [Route("SaveDetails")]
        public async Task<IActionResult> SaveDetails(string displayName)
        {
            // Get user information from the external authentication provider
            AuthenticateResult externalUserInfo = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (externalUserInfo?.Principal == null)
            {
                return BadRequest("Authentication failed.");
            }

            string? email = externalUserInfo.Principal.FindFirstValue(ClaimTypes.Email);
            string? provider = externalUserInfo.Properties?.Items[".AuthScheme"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(provider))
            {
                return BadRequest("Unable to retrieve user information.");
            }

            // Check if the user already exists in your database
            ClosetActionResult result = await _userService.CreateUser(email, provider, displayName);

            // TODO: Add return value
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("DeleteUser/permanent")]
        public async Task<IActionResult> DeleteUser(Guid userId, bool permanent)
        {
            AuthenticateResult externalUserInfo = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            
            if (externalUserInfo?.Principal == null)
            {
                return BadRequest("Authentication failed.");
            }
            
            string? email = externalUserInfo.Principal.FindFirstValue(ClaimTypes.Email);

            // TODO: Get user details from user id
            // TODO: Check that emails match

            ClosetActionResult result = await _userService.DeleteUser(await GetCurrentUserGuid());

            // TODO: return something]
            throw new NotImplementedException();
        }
    }
}

