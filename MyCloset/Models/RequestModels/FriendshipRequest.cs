using System;
using MyCloset.Utilities;

namespace MyCloset.Models.RequestModels
{
    public class FriendshipRequest
    {
        /// <summary>
        /// User initiating the current request 
        /// </summary>
        public Guid? User1 { get; set; }
        /// <summary>
        /// User to respond
        /// </summary>
        public Guid? User2 { get; set; }
        /// <summary>
        /// Status of friendship
        /// </summary>
        public FriendRequestType RequestType { get; set; } = FriendRequestType.NOT_APPLICABLE;
    }
}

