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
            return RedirectToAction("Index", "Products", new { id=category.Id, name=category.Name});

        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    try
                    {
                        using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                        {
                            await fileExcel.CopyToAsync(stream);
                            var workBook = GetWorkBook(stream);
                            await ImportData(workBook);
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        // logging and error handling
                        return View("ImportError");
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private XLWorkbook GetWorkBook(Stream stream)
        {
            return new XLWorkbook(stream);
        }

        private async Task ImportData(XLWorkbook workBook)
        {
            foreach (IXLWorksheet worksheet in workBook.Worksheets)
            {
                var category = await GetOrCreateCategory(worksheet.Name);
                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                {
                    var product = await CreateProduct(row, category);
                    await AddProductToDb(product);
                }
            }
        }

        private async Task<Category> GetOrCreateCategory(string name)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name.Contains(name));
            if (category == null)
            {
                category = new Category();
                category.Name = name;
                category.Info = "from EXCEL";
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            return category;
        }

        private async Task<Product> CreateProduct(IXLRow row, Category category)
        {
            var product = new Product();
            product.Name = row.Cell(1).Value.ToString();
            product.PhotoPath = row.Cell(2).Value.ToString();
            product.Price = decimal.Parse(row.Cell(3).Value.ToString());
            product.Info = row.Cell(4).Value.ToString();
            product.CategoryId = category.Id;
           // _context.Products.Add(product);
            return product;

            //for (int i = 6; i <= 10; i++)
            //{
            //    if (row.Cell(i).Value.ToString().Length > 0)
            //    {
            //        var author = await GetOrCreateAuthor(row.Cell(i).Value.ToString());
            //        //product.AuthorsProducts.Add(new AuthorsProduct { ProductId = product.Id, AuthorId = author.Id });
            //    }
            //}
        }

        private async Task<Author> GetOrCreateAuthor(string name)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name.Contains(name));
            if (author == null)
            {
                author = new Author();
                author.Name = name;
                author.Contacts = "from EXCEL";
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
            }
            return author;
        }

        private async Task AddProductToDb(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }



        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var categories = _context.Categories.Include("Products").ToList();
                //тут, для прикладу ми пишемо усі книжки з БД, в своїх проєктах ТАК НЕ РОБИТИ (писати лише вибрані)
                foreach (var c in categories)
                {
                    var worksheet = workbook.Worksheets.Add(c.Name);

                    worksheet.Cell("A1").Value = "Name";
                    worksheet.Cell("B1").Value = "Photo Path";
                    worksheet.Cell("C1").Value = "Price";
                    worksheet.Cell("D1").Value = "Info";
                    worksheet.Cell("E1").Value = "Category";
                    worksheet.Cell("F1").Value = "Authors";
                    worksheet.Row(1).Style.Font.Bold = true;
                    var products = c.Products.ToList();

                    //нумерація рядків/стовпчиків починається з індекса 1 (не 0)
                    for (int i = 0; i < products.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = products[i].Name;
                        worksheet.Cell(i + 2, 2).Value = products[i].PhotoPath;
                        worksheet.Cell(i + 2, 3).Value = products[i].Price;
                        worksheet.Cell(i + 2, 4).Value = products[i].Info;
                        var category = _context.Products
                         .Where(p => p.Id == products[i].Id)
                         .Select(p => p.Category)
                         .FirstOrDefault();

                        worksheet.Cell(i + 2, 5).Value = category.Name;



                        var authors = _context.Products
                            .Where(p => p.Id == products[i].Id)
                            .SelectMany(p => p.AuthorsProducts)
                            .Select(ap => ap.Author)
                            .ToList();


                        //var ab = _context.AuthorsProducts.Where(a => a.ProductId == products[i].Id).Include("Author").ToList();


                        //більше 4-ох нікуди писати
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

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        //змініть назву файла відповідно до тематики Вашого проєкту

                        FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

    }
}
