using System.Net;
using Microsoft.EntityFrameworkCore;
using MyCloset.Models;
using MyCloset.Models.DBModels;
using MyCloset.Models.RequestModels;
using MyCloset.Services.Interfaces;
using MyCloset.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                List<Friendship> friendships = await _dbContext.Friendships
                    .Where(x => (x.User1 == userId || x.User2 == userId)
                        && x.RequestStatus == (int)FriendRequestType.ACCEPTED
                        && x.ModifiedBy == userId)
                    .ToListAsync();

                if(friendships == null || friendships.Count == 0)
                {
                    _logger.LogInformation($"User {userId} has no friends to return.");
                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Data = new List<User>()
                    };
                }

                List<Guid> friendIds = friendships.Select(x =>
                {
                    return x.User1 != userId ? x.User1 : x.User2;
                }).ToList();

                List<User> users = await _dbContext.Users.Where(x => friendIds.Contains(x.UserId)).ToListAsync();

                _logger.LogInformation($"Returning {users.Count} friends for user {userId}.");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = users
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown during {this.GetType().Name}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while fetching your friend list."
                };
            }
        }

        public async Task<ClosetActionResult> GetFriendRequests(Guid userId)
        {
            try
            {
                _logger.LogInformation($"Getting friend requests for user {userId}");

                List<Friendship> pendingFriendships = await _dbContext.Friendships
                    .Where(x => (x.User1 == userId || x.User2 == userId)
                        && x.RequestStatus == (int)FriendRequestType.CREATED
                        && x.ModifiedBy != userId)
                    .ToListAsync();

                List<User> requestors = await _dbContext.Users
                    .Where(u => pendingFriendships.Select(x => x.User1).Contains(u.UserId)
                        || pendingFriendships.Select(x => x.User2).Contains(u.UserId))
                    .Distinct()
                    .ToListAsync();

                string responseMessage = string.Empty;

                if(pendingFriendships == null || pendingFriendships.Count() == 0)
                {
                    _logger.LogInformation($"No pending friend requests found for user {userId}.");
                    responseMessage = "No friend requests.";
                }
                else
                {
                    _logger.LogInformation($"Returning {requestors.Count()} requests for user {userId}.");
                    responseMessage = $"You have {requestors.Count()} friend requests.";
                }

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = responseMessage,
                    Data = requestors
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown during {this.GetType().Name}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while fetching your friend requests."
                };
            }
        }

        public async Task<ClosetActionResult> GetBlockedList(Guid userId)
        {
            try
            {
                List<Friendship> blockedFriendships = await _dbContext.Friendships
                    .Where(x => (x.User1 == userId || x.User2 == userId)
                        && x.RequestStatus == (int)FriendRequestType.BLOCKED
                        && x.ModifiedBy == userId)
                    .ToListAsync();

                List<Guid> blockedUserIds = blockedFriendships.Select(x =>
                {
                    return x.User1 != userId ? x.User1 : x.User2;
                }).ToList();

                List<User> users = await _dbContext.Users.Where(x => blockedUserIds.Contains(x.UserId)).ToListAsync();

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = users
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown during {this.GetType().Name}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while fetching your blocked list."
                };
            }
        }

        public async Task<ClosetActionResult> EditFriendship(FriendshipRequest friendshipRequest)
        {
            switch (friendshipRequest.RequestType)
            {
                case FriendRequestType.CREATED:
                        return await InitiateFriendRequest(friendshipRequest);
                case FriendRequestType.ACCEPTED:
                    return await AcceptFriendRequest(friendshipRequest);
                case FriendRequestType.DECLINED:
                    return await DeclineFriendRequest(friendshipRequest);
                case FriendRequestType.BLOCKED:
                    return await BlockUser(friendshipRequest);
                case FriendRequestType.REMOVED:
                    return await RemoveFriend(friendshipRequest);
                // TODO: Add Cancelled as a request type - same as removed but different response 
                default:
                    {
                        string message = $"{friendshipRequest.RequestType} does not update relationship between User1: ({friendshipRequest.User1}) and User2: ({friendshipRequest.User2}).";
                        _logger.LogWarning(message);
                        return new ClosetActionResult
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Message = message
                        };
                    }
            }

        }

        /// <summary>
        /// Remove user from friend list
        /// </summary>
        /// <param name="friendshipRequest"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task<ClosetActionResult> RemoveFriend(FriendshipRequest friendshipRequest)
        {
            Guid currentUser = friendshipRequest.User1.Value;
            Guid removedFriend = friendshipRequest.User2.Value;

            User? currentUserModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == currentUser);
            User? deletedFriendUserModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == removedFriend);

            if (currentUserModel == null)
            {
                _logger.LogError($"The current user {currentUser} does not exist.");
                throw new ArgumentException($"User {currentUser} does not exist.");
            }

            if (removedFriend == null)
            {
                _logger.LogError($"User {removedFriend} does not exist.");
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Could not remove friend. User not found."
                };
            }

            try
            {
                _logger.LogInformation($"Removing user {removedFriend} from user {currentUser} friend list.");

                Friendship? friendship = await _dbContext.Friendships
                    .FirstOrDefaultAsync(x => (x.User1 == currentUser && x.User2 == removedFriend) || (x.User1 == removedFriend && x.User2 == currentUser));

                if (friendship != null)
                {
                    friendship.RequestStatus = (int)FriendRequestType.NOT_APPLICABLE;
                    friendship.ModifiedBy = friendshipRequest.User1.Value;
                    friendship.ModifiedOn = DateTime.UtcNow;

                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"User: {currentUser} successfully removed user: {removedFriend} from their friend list.");

                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = $"{deletedFriendUserModel?.DisplayName} was successfully removed from your friend list."
                    };
                }
                else
                {
                    _logger.LogWarning($"Cannot remove friendship from {currentUser} and {removedFriend} because it does not exist.");
                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = $"{deletedFriendUserModel?.DisplayName} cannot be removed because there is no existing friendship."
                    };
                }

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

        /// <summary>
        /// Block user.
        /// </summary>
        /// <param name="friendshipRequest"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> Thrown if the guid for the current user is invalid. </exception>
        private async Task<ClosetActionResult> BlockUser(FriendshipRequest friendshipRequest)
        {

            Guid currentUser = friendshipRequest.User1.Value;
            Guid blockedUser = friendshipRequest.User2.Value;

            User? currentUserModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == currentUser);
            User? deletedFriendUserModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == blockedUser);

            if (currentUserModel == null)
            {
                _logger.LogError($"The current user {currentUser} does not exist.");
                throw new ArgumentException($"User {currentUser} does not exist.");
            }

            if (blockedUser == null)
            {
                _logger.LogError($"User {blockedUser} does not exist.");
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Could not block user. User not found."
                };
            }

            try
            {
                _logger.LogInformation($"Blocking user {blockedUser} for {currentUser}");

                Friendship? friendship = await _dbContext.Friendships
                    .FirstOrDefaultAsync(x => (x.User1 == currentUser && x.User2 == blockedUser) || (x.User1 == blockedUser && x.User2 == currentUser));

                if (friendship != null)
                {
                    // Modify friendship
                    friendship.RequestStatus = ((int)friendshipRequest.RequestType);
                    friendship.ModifiedBy = friendshipRequest.User1.Value;
                    friendship.ModifiedOn = DateTime.UtcNow;
                }
                else
                {
                    // Create friendship with blocked status 
                    Friendship blockedFriendship = new Friendship
                    {
                        User1 = currentUser,
                        User2 = blockedUser,
                        RequestStatus = (int)FriendRequestType.BLOCKED,
                        CreatedBy = currentUser,
                        CreatedOn = DateTime.UtcNow,
                        ModifiedBy = currentUser,
                        ModifiedOn = DateTime.UtcNow
                    };

                    _ = await _dbContext.Friendships.AddAsync(blockedFriendship);
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User: {currentUser} successfully blocked user: {blockedUser} from their friend list.");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"{deletedFriendUserModel?.DisplayName} was successfully blocked."
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown during {this.GetType().Name}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while blocking {deletedFriendUserModel?.DisplayName}."
                };
            }
        }

        /// <summary>
        /// Unblock user.
        /// </summary>
        /// <param name="friendshipRequest"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> Thrown if the guid for the current user is invalid. </exception>
        private async Task<ClosetActionResult> UnblockUser(FriendshipRequest friendshipRequest)
        {
            Guid currentUser = friendshipRequest.User1.Value;
            Guid unblockedUser = friendshipRequest.User2.Value;

            User? currentUserModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == currentUser);
            User? deletedFriendUserModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == unblockedUser);

            if (currentUserModel == null)
            {
                _logger.LogError($"The current user {currentUser} does not exist.");
                throw new ArgumentException($"User {currentUser} does not exist.");
            }

            if (unblockedUser == null)
            {
                _logger.LogError($"User {unblockedUser} does not exist.");
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Could not unblock user. User not found."
                };
            }

            try
            {
                _logger.LogInformation($"Unblocking user {unblockedUser} for {currentUser}");

                Friendship? friendship = await _dbContext.Friendships
                    .FirstOrDefaultAsync(x => (x.User1 == currentUser && x.User2 == unblockedUser) || (x.User1 == unblockedUser && x.User2 == currentUser));

                if (friendship != null)
                {
                    // Modify friendship
                    friendship.RequestStatus = (int)FriendRequestType.UNBLOCK;
                    friendship.ModifiedBy = friendshipRequest.User1.Value;
                    friendship.ModifiedOn = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"User: {currentUser} successfully unblocked user: {unblockedUser} from their content.");

                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = $"{deletedFriendUserModel?.DisplayName} was successfully blocked."
                    };
                }
                else
                {
                    _logger.LogWarning($"Cannot unblock {unblockedUser} for user {currentUser} because friendship does not exist.");
                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = $"{unblockedUser} was not already blocked by you."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown during {this.GetType().Name}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while unblocking {deletedFriendUserModel?.DisplayName}."
                };
            }
        }

        /// <summary>
        /// Decline friend request.
        /// </summary>
        /// <param name="friendshipRequest"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task<ClosetActionResult> DeclineFriendRequest(FriendshipRequest friendshipRequest)
        {
            Guid currentUser = friendshipRequest.User1.Value;
            Guid declinedFriend = friendshipRequest.User2.Value;

            User? currentUserModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == currentUser);
            User? declinedFriendModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == declinedFriend);

            if (currentUserModel == null)
            {
                _logger.LogError($"The current user {currentUser} does not exist.");
                throw new ArgumentException($"User {currentUser} does not exist.");
            }

            if (declinedFriendModel == null)
            {
                _logger.LogError($"User {declinedFriend} does not exist.");
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Could not decline friend. User not found."
                };
            }

            try
            {
                Friendship? friendship = await _dbContext.Friendships
                    .FirstOrDefaultAsync(x => (x.User1 == currentUser && x.User2 == declinedFriend) || (x.User1 == declinedFriend && x.User2 == currentUser));

                if (friendship != null)
                {
                    _logger.LogInformation($"Declining friendship request from user {friendship.CreatedBy} to user {currentUser}.");

                    if(currentUser != friendship.CreatedBy)
                    {
                        return new ClosetActionResult
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Message = $"Cannot decline a request you initiated."
                        };
                    }

                    friendship.RequestStatus = (int)FriendRequestType.DECLINED;
                    friendship.ModifiedBy = friendshipRequest.User1.Value;
                    friendship.ModifiedOn = DateTime.UtcNow;

                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"User: {currentUser} successfully declined friend request from user: {declinedFriend}.");

                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = $"You successfully declined {declinedFriendModel?.DisplayName} as a friend."
                    };
                }
                else
                {
                    _logger.LogWarning($"Cannot decline friendship from {currentUser} and {declinedFriend} because it does not exist.");
                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = $"{declinedFriendModel?.DisplayName} cannot be declined as a friend because there is no existing friendship."
                    };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown during {this.GetType().Name}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while declining {declinedFriendModel?.DisplayName} from your friends."
                };
            }
        }

        /// <summary>
        /// Accept friend request.
        /// </summary>
        /// <param name="friendshipRequest"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task<ClosetActionResult> AcceptFriendRequest(FriendshipRequest friendshipRequest)
        {
            Guid currentUser = friendshipRequest.User1.Value;
            Guid acceptedFriend = friendshipRequest.User2.Value;

            User? currentUserModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == currentUser);
            User? acceptedFriendModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == acceptedFriend);

            if (currentUserModel == null)
            {
                _logger.LogError($"The current user {currentUser} does not exist.");
                throw new ArgumentException($"User {currentUser} does not exist.");
            }

            if (acceptedFriendModel == null)
            {
                _logger.LogError($"User {acceptedFriend} does not exist.");
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Could not accept friend. User not found."
                };
            }

            try
            {
                Friendship? friendship = await _dbContext.Friendships
                    .FirstOrDefaultAsync(x => (x.User1 == currentUser && x.User2 == acceptedFriend) || (x.User1 == acceptedFriend && x.User2 == currentUser));

                if (friendship != null)
                {
                    _logger.LogInformation($"Accpeting friendship request from user {friendship.CreatedBy} to user {currentUser}.");

                    if (currentUser == friendship.CreatedBy)
                    {
                        return new ClosetActionResult
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Message = $"Cannot accept a request you initiated."
                        };
                    }

                    friendship.RequestStatus = (int)FriendRequestType.ACCEPTED;
                    friendship.ModifiedBy = friendshipRequest.User1.Value;
                    friendship.ModifiedOn = DateTime.UtcNow;

                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"User: {currentUser} successfully accepted friend request from user: {acceptedFriend}.");

                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = $"You successfully accepted {acceptedFriendModel?.DisplayName} as a friend."
                    };
                }
                else
                {
                    _logger.LogWarning($"Cannot begin friendship between {currentUser} and {acceptedFriend} because it does not exist.");
                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = $"{acceptedFriendModel?.DisplayName} cannot be accept as a friend because there is no existing friendship request."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown during {this.GetType().Name}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while accepting {acceptedFriendModel?.DisplayName} as your friend."
                };
            }
        }

        /// <summary>
        /// Intiate friend request.
        /// </summary>
        /// <param name="friendshipRequest"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task<ClosetActionResult> InitiateFriendRequest(FriendshipRequest friendshipRequest)
        {
            Guid currentUser = friendshipRequest.User1.Value;
            Guid requestedUser = friendshipRequest.User2.Value;

            User? currentUserModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == currentUser);
            User? requestedFriendModel = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == requestedUser);

            if (currentUserModel == null)
            {
                _logger.LogError($"The current user {currentUser} does not exist.");
                throw new ArgumentException($"User {currentUser} does not exist.");
            }

            if (requestedFriendModel == null)
            {
                _logger.LogError($"User {requestedUser} does not exist.");
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Could not block user. User not found."
                };
            }

            try
            {
                _logger.LogInformation($"Creating a friend request between user {currentUser} for {requestedUser}");

                Friendship? friendship = await _dbContext.Friendships
                    .FirstOrDefaultAsync(x => (x.User1 == currentUser && x.User2 == requestedUser) || (x.User1 == requestedUser && x.User2 == currentUser));

                if (friendship != null)
                {
                    _logger.LogWarning($"Friendship request between {currentUser} and {requestedUser} already exists.");
                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = $"Friend request already exists."
                    };
                }
                else
                {
                    // Create friendship with created status 
                    Friendship createdFriendship = new Friendship
                    {
                        User1 = currentUser,
                        User2 = requestedUser,
                        RequestStatus = (int)FriendRequestType.CREATED,
                        CreatedBy = currentUser,
                        CreatedOn = DateTime.UtcNow,
                        ModifiedBy = currentUser,
                        ModifiedOn = DateTime.UtcNow
                    };

                    _ = await _dbContext.Friendships.AddAsync(createdFriendship);
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User: {currentUser} successfully requested user: {requestedUser} to join their friend list.");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"{requestedFriendModel?.DisplayName} was successfully requested to join your friend list."
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception was thrown during {this.GetType().Name}. {ex.Message} ");

                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = $"Oops! Something went wrong while requesting {requestedFriendModel?.DisplayName} to join your friend list."
                };
            }
        }

    }
}

