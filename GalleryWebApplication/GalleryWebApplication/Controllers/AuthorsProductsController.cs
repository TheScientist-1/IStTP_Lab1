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
    public class AuthorsProductsController : Controller
    {
        private readonly DbgalleryContext _context;

        public AuthorsProductsController(DbgalleryContext context)
        {
            _context = context;
        }

        // GET: AuthorsProducts
        public async Task<IActionResult> Index()
        {
            var dbgalleryContext = _context.AuthorsProducts.Include(a => a.Author).Include(a => a.Product);
            return View(await dbgalleryContext.ToListAsync());
        }

        // GET: AuthorsProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AuthorsProducts == null)
            {
                return NotFound();
            }

            var authorsProduct = await _context.AuthorsProducts
                .Include(a => a.Author)
                .Include(a => a.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authorsProduct == null)
            {
                return NotFound();
            }

            return View(authorsProduct);
        }

        // GET: AuthorsProducts/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: AuthorsProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorId,ProductId,Id")] AuthorsProduct authorsProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(authorsProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Id", authorsProduct.AuthorId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", authorsProduct.ProductId);
            return View(authorsProduct);
        }

        // GET: AuthorsProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AuthorsProducts == null)
            {
                return NotFound();
            }

            var authorsProduct = await _context.AuthorsProducts.FindAsync(id);
            if (authorsProduct == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Id", authorsProduct.AuthorId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", authorsProduct.ProductId);
            return View(authorsProduct);
        }

        // POST: AuthorsProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorId,ProductId,Id")] AuthorsProduct authorsProduct)
        {
            if (id != authorsProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(authorsProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorsProductExists(authorsProduct.Id))
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
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Id", authorsProduct.AuthorId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", authorsProduct.ProductId);
            return View(authorsProduct);
        }

        // GET: AuthorsProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AuthorsProducts == null)
            {
                return NotFound();
            }

            var authorsProduct = await _context.AuthorsProducts
                .Include(a => a.Author)
                .Include(a => a.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authorsProduct == null)
            {
                return NotFound();
            }

            return View(authorsProduct);
        }

        // POST: AuthorsProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AuthorsProducts == null)
            {
                return Problem("Entity set 'DbgalleryContext.AuthorsProducts'  is null.");
            }
            var authorsProduct = await _context.AuthorsProducts.FindAsync(id);
            if (authorsProduct != null)
            {
                _context.AuthorsProducts.Remove(authorsProduct);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorsProductExists(int id)
        {
          return _context.AuthorsProducts.Any(e => e.Id == id);
        }
    }
}
