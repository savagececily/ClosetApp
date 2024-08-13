using System;
using System.Collections.Generic;

namespace MyCloset.Models.DBModels;

public partial class User
{
    public Guid UserId { get; set; }

    public string DisplayName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string AccountProvider { get; set; } = null!;

    public bool IsPublic { get; set; } = true;

    public DateTime DateAdded { get; set; }

    public DateTime LastModified { get; set; }

    public DateTime LastLogin { get; set; }

    public virtual ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();

    public virtual ICollection<Friendship> FriendshipRequestedNavigations { get; set; } = new List<Friendship>();

    public virtual ICollection<Friendship> FriendshipRequestorNavigations { get; set; } = new List<Friendship>();

    public virtual ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();
}
