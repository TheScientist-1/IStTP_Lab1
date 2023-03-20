using System;
using System.Collections.Generic;

namespace NewGalleryWebApplication;

public partial class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int StatusId { get; set; }

    public string? Info { get; set; }

    public DateTime Date { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; } = new List<OrderProduct>();

    public virtual Status Status { get; set; } = null!;
}
