using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GalleryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly DbgalleryContext _context;

        public ChartController(DbgalleryContext context)
        {
            _context = context;
        }
        [HttpGet("JsonData")]

        public JsonResult JsonData()
        {
            var categories = _context.Categories.Include(c => c.Products).ToList();
            List<object> catProduct = new List<object>();
            catProduct.Add(new object[] { "Category", "Number of artworks" });
            foreach (var c in categories)
            {
                catProduct.Add(new object[] { c.Name, c.Products.Count() });
            }
            return new JsonResult(catProduct);
        }

        [HttpGet("AuthorProducts")]
        public JsonResult AuthorProducts()
        {
            var authors = _context.Authors.Include(a => a.AuthorsProducts).ThenInclude(ap => ap.Product).ToList();

            List<object> authorProd = new List<object>();
            authorProd.Add(new object[] { "Author", "Number of artworks" });
            foreach (var a in authors)
            {
                int productCount = a.AuthorsProducts.Sum(ap => ap.Product != null ? 1 : 0);

                authorProd.Add(new object[] { a.Name, productCount });
            }

            return new JsonResult(authorProd);
        }

        [HttpGet("AuthorProductsLineChart")]
        public JsonResult AuthorProductsLineChart()
        {
            var authors = _context.Authors.Include(a => a.AuthorsProducts).ThenInclude(ap => ap.Product).ToList();

            var chartData = new List<object>();
            chartData.Add(new object[] { "Author", "Number of artworks" });

            foreach (var a in authors)
            {
                int productCount = a.AuthorsProducts.Sum(ap => ap.Product != null ? 1 : 0);

                chartData.Add(new object[] { a.Name, productCount });
            }

            return new JsonResult(chartData);
        }

    }
}
