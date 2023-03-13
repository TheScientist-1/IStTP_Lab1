using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GalleryWebApplication;

public partial class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "The field should not be empty")]
    [Display(Name = "Category")]
    public string Name { get; set; } = null!;

    [Display(Name = "Category information")]
    public string? Info { get; set; }

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
