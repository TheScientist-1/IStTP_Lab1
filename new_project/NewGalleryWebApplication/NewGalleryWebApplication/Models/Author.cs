using System;
using System.Collections.Generic;

namespace NewGalleryWebApplication;

public partial class Author
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Contacts { get; set; }

    public virtual ICollection<AuthorsProduct> AuthorsProducts { get; } = new List<AuthorsProduct>();
}
