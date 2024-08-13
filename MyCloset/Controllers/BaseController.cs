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
            // TODO: Get the guid from the db using the logged in users email address
            //return new Guid();
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
