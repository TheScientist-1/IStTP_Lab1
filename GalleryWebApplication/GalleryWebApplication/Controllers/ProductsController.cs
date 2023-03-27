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
    public class ProductsController : Controller
    {
        private readonly DbgalleryContext _context;

        public ProductsController(DbgalleryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Smth(int id)
        {
            var author = await _context.Authors
             .Include(a => a.AuthorsProducts)
             .ThenInclude(ap => ap.Product)
                 .ThenInclude(p => p.Category)
             .FirstOrDefaultAsync(a => a.Id == id);

            var authorProductIds = author.AuthorsProducts
                .Select(ap => ap.ProductId)
                .ToList();

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => authorProductIds.Contains(p.Id))
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int id, string name)
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";


            ViewBag.CategoryId = id;
            ViewBag.CategoryName = name;

            ViewData["CurrentFilter"] = searchString;
            var dbgalleryContext = _context.Products.Where(p => p.CategoryId == id).Include(p => p.Category);
            var products = from p in _context.Products.Where(a => a.CategoryId == id).Include(a => a.Category)
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;

                case "Price":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            return View(await products.AsNoTracking().ToListAsync());
        }






        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        //public IActionResult Create(int categoryId)
        //{
        //    //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        //    //ViewBag.CategoryId = categoryId;
        //    //try
        //    //{
        //    //    ViewBag.CategoryName = _context.Categories.Where(p => p.Id == categoryId).FirstOrDefault().Name;
        //    //}
        //    //catch
        //    //{
        //    //    ViewBag.CategoryName = String.Empty;
        //    //}


        //    return View();
        //}

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }


        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int categoryId, [Bind("Name,CategoryId,Price,Info,Photo")] Product product)
        {
            Console.WriteLine("==============DTO===============");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(product));
            Console.WriteLine("===============================");
            // var product = new Product(product);
            Console.WriteLine("==============Product=================");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(product));
            Console.WriteLine("===============================");

            if (_context.Products.Any(i => i.Name == product.Name))
            {
                ModelState.AddModelError(nameof(product.Name), "This name is already in use.");
            }
            product.CategoryId = categoryId;
            if (ModelState.IsValid)
            {
                if (product.Photo != null)
                {
                    var photoFileName = UploadPhotoAndReturnItsName(product.Photo);
                    product.PhotoPath = photoFileName;
                }



                _context.Add(product);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Products", new { id = categoryId, name = _context.Categories.Where(p => p.Id == categoryId).FirstOrDefault().Name });
            }
            //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            //return View(product);
            return RedirectToAction("Index", "Products", new { id = categoryId, name = _context.Categories.Where(p => p.Id == categoryId).FirstOrDefault().Name });

        }



        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);

            return View(product);

        }


        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CategoryId,Price,Info,PhotoPath,Photo")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (product.Photo != null)
                {
                    Console.WriteLine(11111111111111111);
                    var photoFileName = UploadPhotoAndReturnItsName(product.Photo);
                    product.PhotoPath = photoFileName;
                }

                try
                {
                    Console.WriteLine($"\n\n\n {product.PhotoPath}  \n\n\n");
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Products", new { id = product.CategoryId, name = _context.Categories.Where(p => p.Id == product.CategoryId).FirstOrDefault().Name });
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return RedirectToAction("Index", "Products", new { id = product.CategoryId, name = _context.Categories.Where(p => p.Id == product.CategoryId).FirstOrDefault().Name });

        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'DbgalleryContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Index", "Products", new { id = product.CategoryId, name = _context.Categories.Where(p => p.Id == product.CategoryId).FirstOrDefault().Name });

        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public string UploadPhotoAndReturnItsName(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                // return BadRequest("Invalid file");
            }

            var oldfileName = Path.GetFileName(photo.FileName);
            // var fileName = Guid.NewGuid().ToString() + Path.GetFileName(photo.FileName);

            var fileParts = oldfileName.Split(".");

            var fileExtension = fileParts[fileParts.Length - 1];

            var fileName = Guid.NewGuid().ToString() + "." + fileExtension;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                photo.CopyTo(stream);
            }

            return fileName;
        }
    }
}