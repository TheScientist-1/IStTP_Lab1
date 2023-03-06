using System;
using System.Collections.Generic;

namespace GalleryWebApplication;

public partial class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Contacts { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
