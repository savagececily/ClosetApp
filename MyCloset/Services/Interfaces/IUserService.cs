using System;
using MyCloset.Models;

namespace MyCloset.Services.Interfaces
{
    public interface IUserService
    {
        public Task<ClosetActionResult> CreateUser(string email, string provider, string displayName);
        public Task<ClosetActionResult> GetUserDetails(Guid userId);
        public Task<ClosetActionResult> DeleteUser(Guid userId);
    }
}

