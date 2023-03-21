using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GalleryWebApplication;

public partial class Product
{
    public Product()
    {
        OrderProducts = new HashSet<OrderProduct>();
    }
    public int Id { get; set; }

    [Required(ErrorMessage = "The field should not be empty")]
    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    [Required(ErrorMessage = "The field should not be empty")]

    public decimal Price { get; set; }
    [Display(Name = "Product information")]
    public string? Info { get; set; }

    public virtual ICollection<AuthorsProduct> AuthorsProducts { get; } = new List<AuthorsProduct>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; } = new List<OrderProduct>();
}
