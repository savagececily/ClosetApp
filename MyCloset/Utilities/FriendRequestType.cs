using System;
namespace MyCloset.Utilities
{
    public enum FriendRequestType
    {
        NOT_APPLICABLE = 0,
        CREATED = 1,
        ACCEPTED = 2,
        DECLINED = 3 ,
        REMOVED = 4,
        BLOCKED = 5,
        UNBLOCK = 6
    }

    public static class FriendRequestTypeExtentions
    {
        public static string ToActionString(this FriendRequestType type)
        {
            return type switch
            {
                FriendRequestType.CREATED => "initiating",
                FriendRequestType.ACCEPTED => "accepting",
                FriendRequestType.DECLINED => "declining",
                FriendRequestType.REMOVED => "removing",
                FriendRequestType.BLOCKED => "blocking",
                FriendRequestType.UNBLOCK => "unblocking",
                _ => "unknown action"
            };
        }
    }
}

