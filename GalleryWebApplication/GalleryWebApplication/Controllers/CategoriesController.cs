using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GalleryWebApplication;
using ClosedXML.Excel;
using System.Security.Claims;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;

namespace GalleryWebApplication.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DbgalleryContext _context;

        public CategoriesController(DbgalleryContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return _context.Categories != null ?
                        View(await _context.Categories.ToListAsync()) :
                        Problem("Entity set 'DbgalleryContext.Categories'  is null.");
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            //return View(category);
            return RedirectToAction("Index", "Products", new { id = category.Id, name = category.Name });

        }

        // GET: Categories/Create
        [Authorize(Roles = "admin, superAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, superAdmin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Info")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "admin, superAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, superAdmin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Info")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "admin, superAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, superAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'DbgalleryContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, superAdmin")]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);

                        var workBook = new XLWorkbook(stream);

                        foreach (IXLWorksheet worksheet in workBook.Worksheets)
                        {
                            var categoryName = worksheet.Name;
                            var category = await GetOrCreateCategory(categoryName);

                            foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                            {
                                try
                                {
                                    var product = await CreateProductFromRow(row, category);
                                    await AddAuthorsToProduct(product, row);
                                }
                                catch (Exception e)
                                {
                                    return View("ImportError");
                                }
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<Category> GetOrCreateCategory(string categoryName)
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.Contains(categoryName));

            if (existingCategory != null)
            {
                return existingCategory;
            }

            var newCategory = new Category
            {
                Name = categoryName,
                Info = "from EXCEL"
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return newCategory;
        }

        private async Task<Product> CreateProductFromRow(IXLRow row, Category category)
        {
            var product = new Product
            {
                Name = row.Cell(1).Value.ToString(),
                PhotoPath = row.Cell(2).Value.ToString(),
                Price = decimal.Parse(row.Cell(3).Value.ToString()),
                Info = row.Cell(4).Value.ToString(),
                Category = category
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        private async Task AddAuthorsToProduct(Product product, IXLRow row)
        {
            for (int i = 6; i <= 20; i++)
            {
                var authorName = row.Cell(i).Value.ToString();

                if (string.IsNullOrWhiteSpace(authorName))
                {
                    continue;
                }

                var author = await GetOrCreateAuthor(authorName);

                var authorsProduct = new AuthorsProduct
                {
                    ProductId = product.Id,
                    AuthorId = author.Id
                };

                _context.AuthorsProducts.Add(authorsProduct);
            }

            await _context.SaveChangesAsync();
        }

        private async Task<Author> GetOrCreateAuthor(string authorName)
        {
            var existingAuthor = await _context.Authors
                .FirstOrDefaultAsync(a => a.Name.Contains(authorName));

            if (existingAuthor != null)
            {
                return existingAuthor;
            }

            var newAuthor = new Author
            {
                Name = authorName,
                Contacts = "from EXCEL"
            };

            _context.Authors.Add(newAuthor);
            await _context.SaveChangesAsync();

            return newAuthor;
        }


        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream))
                        {
                            //перегляд усіх листів (в даному випадку категорій)
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                //worksheet.Name - назва категорії. Пробуємо знайти в БД, якщо відсутня, то створюємо нову
                                Category newcat;
                                var c = (from cat in _context.Categories
                                         where cat.Name.Contains(worksheet.Name)
                                         select cat).ToList();
                                if (c.Count > 0)
                                {
                                    newcat = c[0];
                                }
                                else
                                {
                                    newcat = new Category();
                                    newcat.Name = worksheet.Name;
                                    newcat.Info = "from EXCEL";
                                    //додати в контекст
                                    _context.Categories.Add(newcat);
                                }
                                //перегляд усіх рядків                    
                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    try
                                    {
                                        Product product = new Product();
                                        product.Name = row.Cell(1).Value.ToString();
                                        product.PhotoPath = row.Cell(2).Value.ToString();
                                        product.Price = decimal.Parse(row.Cell(3).Value.ToString());
                                        product.Info = row.Cell(4).Value.ToString();
                                        product.Category = newcat;
                                        _context.Products.Add(product);
                                        _context.SaveChanges();

                                        //у разі наявності автора знайти його, у разі відсутності - додати
                                        for (int i = 6; i <= 20; i++)
                                        {
                                            if (row.Cell(i).Value.ToString().Length > 0)
                                            {
                                                Author author;
                                                var a = (from aut in _context.Authors
                                                         where aut.Name.Contains(row.Cell(i).Value.ToString())
                                                         select aut).ToList();
                                                if (a.Count > 0)
                                                {
                                                    author = a[0];
                                                }
                                                else
                                                {
                                                    author = new Author();
                                                    author.Name = row.Cell(i).Value.ToString();
                                                    author.Contacts = "from EXCEL";
                                                    //додати в контекст
                                                    _context.Add(author);
                                                    _context.SaveChanges();
                                                }
                                                AuthorsProduct ap = new AuthorsProduct();
                                                ap.ProductId = product.Id;
                                                ap.AuthorId = author.Id;
                                                _context.AuthorsProducts.Add(ap);
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        //logging самостійно :)

                                    }
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        */

        [Authorize(Roles = "admin, superAdmin")]
        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var categories = _context.Categories.Include(c => c.Products).ToList();
                foreach (var category in categories)
                {
                    var worksheet = workbook.Worksheets.Add(category.Name);

                    var headers = new List<string> { "Name", "Photo Path", "Price", "Info", "Category", "Authors" };
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                        worksheet.Row(1).Style.Font.Bold = true;
                    }

                    var products = category.Products.ToList();
                    for (int i = 0; i < products.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = products[i].Name;
                        worksheet.Cell(i + 2, 2).Value = products[i].PhotoPath;
                        worksheet.Cell(i + 2, 3).Value = products[i].Price;
                        worksheet.Cell(i + 2, 4).Value = products[i].Info;
                        worksheet.Cell(i + 2, 5).Value = category.Name;

                        var authors = _context.Products
                            .Where(p => p.Id == products[i].Id)
                            .SelectMany(p => p.AuthorsProducts)
                            .Select(ap => ap.Author)
                            .ToList();
                        int j = 0;
                        foreach (var a in authors)
                        {
                            if (j < authors.Count())
                            {
                                worksheet.Cell(i + 2, j + 6).Value = a.Name;
                                j++;
                            }

                        }
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();
                    return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"gallery_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }














        public ActionResult ExportCat(int id)
        {
            try
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var category = _context.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == id);
                    if (category == null)
                    {
                        return NotFound();
                    }

                    var prod = category.Products.ToList();

                    var worksheet = workbook.Worksheets.Add(category.Name);

                    var headers = new List<string> { "Name", "Photo Path", "Price", "Info", "Category", "Authors" };
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                        worksheet.Row(1).Style.Font.Bold = true;
                    }

                    for (int i = 0; i < prod.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = prod[i].Name;
                        worksheet.Cell(i + 2, 2).Value = prod[i].PhotoPath;
                        worksheet.Cell(i + 2, 3).Value = prod[i].Price;
                        worksheet.Cell(i + 2, 4).Value = prod[i].Info;
                        worksheet.Cell(i + 2, 5).Value = category.Name;

                        var authors = _context.Products
                            .Where(p => p.Id == prod[i].Id)
                            .SelectMany(p => p.AuthorsProducts)
                            .Select(ap => ap.Author)
                            .ToList();

                        int j = 0;
                        foreach (var a in authors)
                        {
                            if (j < authors.Count())
                            {
                                worksheet.Cell(i + 2, j + 6).Value = a.Name;
                                j++;
                            }

                        }
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        string name = _context.Categories.Where(p => p.Id == id).FirstOrDefault().Name;
                        return new FileContentResult(stream.ToArray(),
                                             "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                        {
                            FileDownloadName = $"{name}_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }
}
