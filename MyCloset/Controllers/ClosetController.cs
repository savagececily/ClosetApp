using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCloset.Controllers
{
    public class ClosetController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
#region Closet
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMyCloset()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult AddClosetContent()
        {
            return Created("", "");
        }

        [HttpPut]
        public IActionResult EditClosetContent()
        {
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteClosetContent()
        {
            return NoContent();
        }
#endregion

#region Style Factory
        [HttpGet]
        public IActionResult GetStyleFactory()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult AddOutfitToStyleFactory()
        {
            return Created("", "");
        }

        [HttpPut]
        public IActionResult EditOutfitInStyleFactory()
        {
            return Ok(); 
        }

        [HttpDelete]
        public IActionResult DeleteOutfitInStyleFactory()
        {
            return NoContent();
        }
#endregion
    }
}

