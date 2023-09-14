using System;
using System.Collections.Generic;

namespace MyCloset.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string DisplayName { get; set; }

    public string Email { get; set; }

    public string AccountProvider { get; set; }

    public virtual ICollection<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();

    public virtual ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();

    public virtual ICollection<UserFriend> UserFriendUserId1Navigations { get; set; } = new List<UserFriend>();

    public virtual ICollection<UserFriend> UserFriendUserId2Navigations { get; set; } = new List<UserFriend>();
}
