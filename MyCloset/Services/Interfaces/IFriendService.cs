using System;
using MyCloset.Models;
using MyCloset.Models.RequestModels;

namespace MyCloset.Services.Interfaces
{
    public interface IFriendService
    {
        public Task<ClosetActionResult> GetFriends(Guid userId);
        public Task<ClosetActionResult> GetFriendRequests(Guid userId);
        public Task<ClosetActionResult> GetBlockedList(Guid userId);
        public Task<ClosetActionResult> EditFriendship(FriendshipRequest friendshipRequest);
    }
}