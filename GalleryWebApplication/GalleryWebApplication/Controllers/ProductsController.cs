using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GalleryWebApplication;

namespace GalleryWebApplication.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DbgalleryContext _context;

        public ProductsController(DbgalleryContext context)
        {
            _context = context;
        }

        // GET: Products
        ////public async Task<IActionResult> Index(int id, string name)
        ////{
        ////    if (id == null) return RedirectToAction("Categories", "Index");
        ////    ViewBag.CategoryId = id;
        ////    ViewBag.CategoryName = name;
        ////    var dbgalleryContext = _context.Products.Where(p => p.CategoryId == id).Include(p => p.Category);

        ////    return View(await dbgalleryContext.ToListAsync());
        ////}
        public async Task<IActionResult> Index()
        {
            var dbgalleryContext = _context.Products.Include(p => p.Category);
            return View(await dbgalleryContext.ToListAsync());
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

        //public async Task<IActionResult> AddProduct(int? id)
        //{
        //    if (id == null || _context.Products == null)
        //    {
        //        return NotFound();
        //    }
        //    var products = await _context.Products.ToListAsync();
        //    ViewBag.CategoryId = id;
        //    return View(products);
        //}
        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int categoryId, [Bind("Id,Name,CategoryId,Price,Info")] Product product)
        {
            product.CategoryId= categoryId;
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                //return RedirectToAction("Index", "Products", new { id = categoryId, name = _context.Categories.Where(p => p.Id == categoryId).FirstOrDefault().Name} );
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
              //return RedirectToAction("Index", "Products", new { id = categoryId, name = _context.Categories.Where(p => p.Id == categoryId).FirstOrDefault().Name} );

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

        public async Task<IActionResult> Buy(int? productId, int? categoryId)
        {
            if (categoryId == null || productId == null)
            {
                return NotFound();
            }
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == productId);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = categoryId;
            return View(product);

        }



        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CategoryId,Price,Info")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
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
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
