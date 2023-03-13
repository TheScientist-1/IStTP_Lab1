using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GalleryWebApplication;

public partial class Author
{
    public int Id { get; set; }
    [Required(ErrorMessage = "The field should not be empty")]
    public string Name { get; set; } = null!;

    public string? Contacts { get; set; }


    public virtual ICollection<AuthorsProduct> AuthorsProducts { get; } = new List<AuthorsProduct>();
}
