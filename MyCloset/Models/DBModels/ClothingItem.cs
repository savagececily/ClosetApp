using System;
using System.Collections.Generic;

namespace MyCloset.Models.DBModels;

public partial class ClothingItem
{
    public Guid ClothingItemId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string LinkToPhoto { get; set; } = null!;

    public Guid UserId { get; set; }

    public string Tags { get; set; } = null!;

    public DateTime DateAdded { get; set; }

    public DateTime LastModified { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();
}
