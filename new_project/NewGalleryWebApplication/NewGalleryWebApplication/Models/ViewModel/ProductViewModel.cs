namespace NewGalleryWebApplication
{
    public class ProductViewModel
    {
        public ProductViewModel(Product product) 
        {
            Name = product.Name;
            Price = product.Price;
            CategoryId = product.CategoryId;
            Info = product.Info;
        }

        public ProductViewModel()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public string? Info { get; set; }
    }
    
}
