using System;
using System.Collections.Generic;

namespace MyCloset.Models;

public partial class ClothingItem
{
    public Guid ClothingItemId { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public string Category { get; set; }

    public string LinkToPhoto { get; set; }

    public Guid UserId { get; set; }

    public string Occasions { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();
}
