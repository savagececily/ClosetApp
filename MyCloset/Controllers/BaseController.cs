using Microsoft.AspNetCore.Mvc;
using MyCloset.Models;
using MyCloset.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace MyCloset.Controllers
{
    public class BaseController : ControllerBase
    {
        protected async Task<Guid> GetCurrentUserGuid()
        {
            // Check for test user header in Development/Staging environments
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == "Development" || environment == "Staging")
            {
                if (Request.Headers.TryGetValue("X-Test-User-Id", out var testUserId))
                {
                    if (Guid.TryParse(testUserId, out var userId))
                    {
                        return userId;
                    }
                }
            }

            // TODO: Get the guid from authentication claims
            // For now, return hardcoded GUID for backward compatibility
            return Guid.Parse("e85865f7-3c93-4edf-be81-c9dd8c048008");
        }

        protected IActionResult ResultHelper(ClosetActionResult closetActionResult)
        {
            return StatusCode((int)closetActionResult.StatusCode,
                new
                {
                    Message = closetActionResult.Message,
                    Data = closetActionResult.Data
                });
        }
    }
}
