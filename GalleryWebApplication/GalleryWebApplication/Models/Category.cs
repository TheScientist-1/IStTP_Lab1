using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GalleryWebApplication;

public partial class Category
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Категорія")]

    public string Name { get; set; } = null!;
    [Display(Name = "Інформація про категорію")]

    public string? Info { get; set; }

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
