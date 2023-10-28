using System;
using System.Net;
using Microsoft.EntityFrameworkCore;
using MyCloset.Models;
using MyCloset.Models.DBModels;
using MyCloset.Services.Interfaces;

namespace MyCloset.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly MyClosetAppDbContext _dbContext;

        public UserService(ILogger<UserService> logger, MyClosetAppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<ClosetActionResult> GetUserDetails(Guid userId)
        {
            try
            {
                User? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

                if(user == null)
                {
                    _logger.LogWarning($"User: {userId} does not exist.");

                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = $"The user you are looking for does not exist."
                    };
                }

                // TODO: Return a message?
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = user
                };
            }
            catch(Exception ex){
                _logger.LogError(ex, $"Something went wrong while getting account details for User: {userId}.");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while getting account details."
                };
            }
        }

        // TODO: Create an UserRequest Model
        public Task<ClosetActionResult> CreateUser(string email, string provider, string displayName)
        {
            throw new NotImplementedException();
        }

        // TODO: Add more account settings - private, seller, verified, etc.
        public Task<ClosetActionResult> EditUserDetails(string displayName)
        {
            throw new NotImplementedException();
        }

        public async Task<ClosetActionResult> DeleteUser(Guid userId, bool isPermenant)
        {
            try
            {
                User? userToDelete = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

                if(userToDelete != null)
                {
                    if (isPermenant)
                    {
                        _dbContext.Users.Remove(userToDelete);
                    }
                    else
                    {
                        // TODO: Create SoftDelete Column to DB
                        // TODO: Change SoftDelete Column to True
                        // TODO: Create Function to clean up DB
                    }
                }
                
                await _dbContext.SaveChangesAsync();
                string accountRecoverableMessage = isPermenant ? "Account is recoverable for 365 days." : string.Empty;

                _logger.LogInformation($"User: {userId} account deleted successfully. {accountRecoverableMessage}");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"Your account has been deleted. {accountRecoverableMessage}"
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while deleting User: {userId}.");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while deleting your account."
                };
            }
        }
    }
}

