using System;
using System.ComponentModel.DataAnnotations;

namespace MyCloset.Models.DBModels
{
    public class FriendRequestStatus
    {
        [Key]
        public int StatusID { get; set; }
        public string Status { get; set; }
    }
}

