using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyCloset.Models.DBModels;

public class User
{
    [Key]
    public Guid UserID { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [MaxLength(255)]
    public string DisplayName { get; set; }

    [Required]
    [MaxLength(255)]
    public string AccountProvider { get; set; }

    [Required]
    public DateTime DateAdded { get; set; }

    [Required]
    public DateTime LastModified { get; set; }

    [Required]
    public DateTime LastLogin { get; set; }

    public List<Friendship> SentFriendRequests { get; set; }
    public List<Friendship> ReceivedFriendRequests { get; set; }
}
