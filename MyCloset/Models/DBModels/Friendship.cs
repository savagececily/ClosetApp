using System;
using System.Collections.Generic;

namespace MyCloset.Models.DBModels;

public partial class Friendship
{
    public Guid FriendshipId { get; set; }

    public Guid User1 { get; set; }

    public Guid User2 { get; set; }

    public int RequestStatus { get; set; }

    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// User that initiated the friendship request 
    /// </summary>
    public Guid CreatedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    /// <summary>
    /// User that made the most recent change to the relationship
    /// </summary>
    public Guid ModifiedBy { get; set; }

    public virtual FriendRequestStatus RequestStatusNavigation { get; set; } = null!;

    public virtual User RequestedNavigation { get; set; } = null!;

    public virtual User RequestorNavigation { get; set; } = null!;
}
