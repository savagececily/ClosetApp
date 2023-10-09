using System;
using MyCloset.Models;

namespace MyCloset.Services.Interfaces
{
    public interface IFriendService
    {
        public Task<ClosetActionResult> GetFriends(Guid userId);
        public Task<ClosetActionResult> RequestFriend(Guid currentUser, Guid requestedFriend);
        public Task<ClosetActionResult> FriendRequestResponse(Guid friendshipId, bool accepted);
        public Task<ClosetActionResult> DeleteFriend(Guid currentUser, Guid deletedFriend);

    }
}