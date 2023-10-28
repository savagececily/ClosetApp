using System;
using System.Collections.Generic;

namespace MyCloset.Models.DBModels;

public partial class Friendship
{
    public Guid FriendshipId { get; set; }

    public Guid Requestor { get; set; }

    public Guid Requested { get; set; }

    public int RequestStatus { get; set; }

    public virtual FriendRequestStatus RequestStatusNavigation { get; set; } = null!;

    public virtual User RequestedNavigation { get; set; } = null!;

    public virtual User RequestorNavigation { get; set; } = null!;
}
