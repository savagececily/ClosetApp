using System;
using System.Collections.Generic;

namespace MyCloset.Models.DBModels;

public partial class Outfit
{
    public Guid OutfitId { get; set; }

    public string Title { get; set; } = null!;

    public Guid UserId { get; set; }

    public string Tags { get; set; } = null!;

    public DateTime DateAdded { get; set; }

    public DateTime LastModified { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();
}
