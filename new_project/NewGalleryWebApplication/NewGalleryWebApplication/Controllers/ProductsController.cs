using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewGalleryWebApplication;

namespace NewGalleryWebApplication.Controllers
{
    
    
    public class ProductsController : Controller
    {
        private readonly DbgalleryContext _context;

        public ProductsController(DbgalleryContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(int? id,string sortOrder, string searchString)
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";


            ViewBag.CategoryId = id;
            var test = _context.Categories.Where((a => a.Id == id)).FirstOrDefault();
            if (test==null)
            {
                return NotFound();
            }
            ViewBag.CategoryName = test.Name;

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


        //public async Task<IActionResult> Index(string sortOrder, string searchString)
        //{

        //    ViewData["CurrentSort"] = sortOrder;
        //    ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        //    ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";


        //    ViewBag.CategoryId = -1;

        //    ViewBag.CategoryName = "All categories";


        //    var products = _context.Products;

            

        //    //switch (sortOrder)
        //    //{
        //    //    case "name_desc":
        //    //        products = products.OrderByDescending(p => p.Name);
        //    //        break;

        //    //    case "Price":
        //    //        products = products.OrderBy(p => p.Price);
        //    //        break;
        //    //    case "price_desc":
        //    //        products = products.OrderByDescending(p => p.Price);
        //    //        break;
        //    //    default:
        //    //        products = products.OrderBy(p => p.Name);
        //    //        break;
        //    //}

        //    return View(await products.AsNoTracking().ToListAsync());
        //}

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

        //// GET: Products/Create
        //public IActionResult Create(int categoryId)
        //{
        //    //ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        //    ViewBag.CategoryId = categoryId;
        //    ViewBag.CategoryName = _context.Categories.Where(p => p.Id == categoryId).FirstOrDefault().Name;
        //    return View();
        //}


        // GET: Products/Create
        public IActionResult Create(int categoryId)
        {
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = _context.Categories.Where(p => p.Id == categoryId).FirstOrDefault().Name;
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int categoryId, [Bind("Name,CategoryId,Price,Info")] Product product)
        {
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = _context.Categories.Where(p => p.Id == categoryId).FirstOrDefault().Name;
            product.CategoryId = categoryId;
            var errors = ModelState
             .Where(x => x.Value.Errors.Count > 0)
              .Select(x => new { x.Key, x.Value.Errors })
                     .ToArray();
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction("Index", "Products", new { id = categoryId, name = _context.Categories.Where(p => p.Id == categoryId).FirstOrDefault().Name });
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
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
            // ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            // List<SelectListItem> categoryList = new List<SelectListItem>();
            // foreach (var cat in _context.Categories) {
            //     categoryList.Add(new SelectListItem {Selected = true, Text = cat.Name, Value = cat.Id.ToString()});
            // }
            // ViewData["CategoryId"] = new SelectList(categoryList);
            // ViewData["Categories"] = _context.Categories.ToList();
            ViewData["Categories"] = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
            return View(new ProductViewModel(product));
        }


        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel productvm)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products.FindAsync(id);
                    product.Name  =  productvm.Name;
                    product.Price = productvm.Price;
                    product.CategoryId = productvm.CategoryId;
                    product.Info = productvm.Info;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(productvm.Id))
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
            ViewData["Categories"] = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));


            return View(productvm);
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
