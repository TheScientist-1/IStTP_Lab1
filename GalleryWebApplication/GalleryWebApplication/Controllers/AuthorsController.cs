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
    public class AuthorsController : Controller
    {
        private readonly DbgalleryContext _context;

        public AuthorsController(DbgalleryContext context)
        {
            _context = context;
        }

        // GET: Authors
        //public async Task<IActionResult> Index()
        //{
        //      return View(await _context.Authors.ToListAsync());
        //}

        public async Task<IActionResult> Index(int? id, int? productId)
        {
            var viewModel = new InstructorIndexData();
            viewModel.Authors = await _context.Authors
                  .Include(i => i.AuthorsProducts)
                    .ThenInclude(i => i.Product)
                  .Include(i => i.AuthorsProducts)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(i => i.Category)
                  .AsNoTracking()
                  .OrderBy(i => i.Name)
                  .ToListAsync();

            if (id != null)
            {
                ViewData["AuthorId"] = id.Value;
                Author author = viewModel.Authors.Where(
                    i => i.Id == id.Value).Single();
                viewModel.Products = author.AuthorsProducts.Select(s => s.Product);
            }

            
            return View(viewModel);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
           
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
           
            if (author == null)
            {
                return NotFound();
            }
            //return View(author);
            //return RedirectToAction("Index", "AuthorsProducts", new { id = author.Id, name = author.Name });
            return RedirectToAction("Smth", "Products", new { id = author.Id });

        }



        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Contacts")] Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        /*public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }*/

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .Include(i => i.AuthorsProducts).ThenInclude(i => i.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }
            PopulateAuthorsProductsData(author);
            return View(author);
        }

        private void PopulateAuthorsProductsData(Author author)
        {
            var allProducts = _context.Products;
            var authorProducts = new HashSet<int>(author.AuthorsProducts.Select(c => c.ProductId));
            var viewModel = new List<FoundAuthorProducts>();
            foreach (var product in allProducts)
            {
                viewModel.Add(new FoundAuthorProducts
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Available = authorProducts.Contains(product.Id)

                });
            }
            ViewData["Products"] = viewModel;
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        /*public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Contacts")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
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
            return View(author);
        }*/
        
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Contacts")] Author author, string[] selectedProducts)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorToUpdate = await _context.Authors
                .Include(i => i.AuthorsProducts)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (await TryUpdateModelAsync<Author>(
                authorToUpdate,
                "",
                i => i.Name, i => i.Contacts ))
            {
                
                UpdateAuthorProducts(selectedProducts, authorToUpdate);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            UpdateAuthorProducts(selectedProducts, authorToUpdate);
            PopulateAuthorsProductsData(authorToUpdate);
            return View(authorToUpdate);
        }


        private void UpdateAuthorProducts(string[] selectedProducts, Author authorToUpdate)
        {
            /*if (selectedProducts == null)
            {
                authorToUpdate.AuthorsProducts = new List<AurhorsProducts>();
                return;
            }*/

            var selectedProductsHH = new HashSet<string>(selectedProducts);
            var authorProducts = new HashSet<int>
                (authorToUpdate.AuthorsProducts.Select(c => c.Product.Id));
            foreach (var product in _context.Products)
            {
                if (selectedProductsHH.Contains(product.Id.ToString()))
                {
                    if (!authorProducts.Contains(product.Id))
                    {
                        authorToUpdate.AuthorsProducts.Add(new AuthorsProduct { AuthorId = authorToUpdate.Id, ProductId = product.Id });
                    }
                }
                else
                {

                    if (authorProducts.Contains(product.Id))
                    {
                        AuthorsProduct productToRemove = authorToUpdate.AuthorsProducts.FirstOrDefault(i => i.ProductId == product.Id);
                        _context.Remove(productToRemove);
                    }
                }
            }
        }


        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Authors == null)
            {
                return Problem("Entity set 'DbgalleryContext.Authors'  is null.");
            }
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
          return _context.Authors.Any(e => e.Id == id);
        }
    }
}
