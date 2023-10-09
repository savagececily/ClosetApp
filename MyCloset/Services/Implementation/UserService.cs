using System;
using MyCloset.Models;
using MyCloset.Services.Interfaces;

namespace MyCloset.Services.Implementation
{
    public class UserService : IUserService
    {
        public Task<ClosetActionResult> CreateUser(string email, string provider, string displayName)
        {
            throw new NotImplementedException();
        }

        public Task<ClosetActionResult> DeleteUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ClosetActionResult> GetUserDetails(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}

