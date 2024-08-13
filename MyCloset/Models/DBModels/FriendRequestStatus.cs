using System;
using System.Collections.Generic;

namespace MyCloset.Models.DBModels;

public partial class FriendRequestStatus
{
    public int StatusId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
}
