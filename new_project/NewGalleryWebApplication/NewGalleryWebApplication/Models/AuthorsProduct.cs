using System;
using System.Collections.Generic;

namespace NewGalleryWebApplication;

public partial class AuthorsProduct
{
    public int AuthorId { get; set; }

    public int ProductId { get; set; }

    public int Id { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
