using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCloset.Models.DBModels;

public class Friendship
{
    [Key]
    public Guid FriendshipID { get; set; }

    [Required]
    public Guid Requestor { get; set; }

    [Required]
    public Guid Requested { get; set; }

    [Required]
    public int RequestStatus { get; set; }

    [ForeignKey("Requestor")]
    public User RequestorUser { get; set; }

    [ForeignKey("Requested")]
    public User RequestedUser { get; set; }

    [ForeignKey("RequestStatus")]
    public FriendRequestStatus Status { get; set; }
}
