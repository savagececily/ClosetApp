using System;
using System.Collections.Generic;

namespace MyCloset.Models;

public partial class UserFriend
{
    public Guid FriendshipId { get; set; }

    public Guid UserId1 { get; set; }

    public Guid UserId2 { get; set; }
    public bool Status { get; set; }

    public virtual User? UserId1Navigation { get; set; }

    public virtual User? UserId2Navigation { get; set; }
}
