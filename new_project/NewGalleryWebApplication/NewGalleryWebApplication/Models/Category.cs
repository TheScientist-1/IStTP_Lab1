using System;
using System.Collections.Generic;

namespace NewGalleryWebApplication;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Info { get; set; }

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
