using System.Net;
using Microsoft.EntityFrameworkCore;
using MyCloset.Models;
using MyCloset.Models.DBModels;
using MyCloset.Services.Interfaces;
using MyCloset.Utilities;

namespace MyCloset.Services.Implementation
{
    public class FriendService : IFriendService
    {
        private readonly MyClosetAppDbContext _dbContext;
        private readonly ILogger<FriendService> _logger;

        public FriendService(ILogger<FriendService> logger, MyClosetAppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<ClosetActionResult> GetFriends(Guid userId)
        {
            try
            {
                // TODO: Select Users that are friends of the current user 
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }

        public async Task<ClosetActionResult> FriendRequestResponse(Guid friendshipId, bool accepted)
        {
            try
            {
                Friendship? friendship = await _dbContext.Friendships
                    .FirstOrDefaultAsync(x => x.FriendshipId == friendshipId);
                // TODO: redo method, the following code is not how this should be working

                if (friendship != null)
                {
                    _dbContext.Friendships.Remove(friendship);
                }

                await _dbContext.SaveChangesAsync();

                // TODO: Add success info log and status message to action result
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $""
                };

            }
            catch (Exception ex)
            {
                // TODO: Add Logging and Exception Handling 
                throw new NotImplementedException();
            }
        }

        public async Task<ClosetActionResult> DeleteFriend(Guid currentUser, Guid deletedFriend)
        {
            User? currentUserModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == currentUser);
            User? deletedFriendUserModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == deletedFriend);

            try
            {
                Friendship? friendship = await _dbContext.Friendships
                    .FirstOrDefaultAsync(x => (x.Requestor == currentUser && x.Requested == deletedFriend) || (x.Requestor == deletedFriend && x.Requested == currentUser));

                if (friendship != null)
                {
                    _dbContext.Friendships.Remove(friendship);
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User: {currentUser} successfully removed user: {deletedFriend} from their friend list.");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"{deletedFriendUserModel?.DisplayName} was successfully removed from your friend list."
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown during {this.GetType().Name}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while removing {deletedFriendUserModel?.DisplayName} from your friends."
                };
            }
        }

        public async Task<ClosetActionResult> RequestFriend(Guid currentUser, Guid requestedFriend)
        {
            try
            {
                bool friendshipExists = await _dbContext.Friendships
                    .AnyAsync(x => (x.Requestor == currentUser && x.Requested == requestedFriend) || (x.Requestor == requestedFriend && x.Requested == currentUser));

                if (friendshipExists)
                {
                    string errorMessage = "Friendship already exists.";
                    _logger.LogError($"Friendship between user: {currentUser} and user: {requestedFriend} cannot be requested again. {errorMessage}.");
                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = errorMessage
                    };
                }

                await _dbContext.Friendships.AddAsync(new Friendship
                {
                    Requestor = currentUser,
                    Requested = requestedFriend,
                    RequestStatus = (int)FriendRequestType.Requested
                });

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User: {currentUser} successfully requested user: {requestedFriend} to be friends.");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Request submitted successfully."
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown iniializing friend request between user: {currentUser} and user: {requestedFriend}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Oops! Something went wrong during your friend request."
                };
            }
        }
    }
}

