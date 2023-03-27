using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GalleryWebApplication;
using GalleryWebApplication.Models.GalleryViewModels;

namespace GalleryWebApplication.Controllers
{
    public class TestController : Controller
    {


        public TestController()
        {

        }


        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UploadPhoto()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UploadPhoto(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("Invalid file");
            }
            
            var fileName = Guid.NewGuid().ToString() + Path.GetFileName(photo.FileName);
            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                photo.CopyTo(stream);
            }

            return RedirectToAction("Index");
        }

    }
}
