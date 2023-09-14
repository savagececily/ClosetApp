using System;
using System.Collections.Generic;

namespace MyCloset.Models;

public partial class Outfit
{
    public Guid OutfitId { get; set; }

    public string Title { get; set; }

    public Guid UserId { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();
}
