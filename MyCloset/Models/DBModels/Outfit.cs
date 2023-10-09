using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCloset.Models.DBModels;

public partial class Outfit
{
    [Key]
    public Guid OutfitID { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    [Required]
    public Guid UserID { get; set; }

    [Required]
    public string Tags { get; set; }

    [Required]
    public DateTime DateAdded { get; set; }

    [Required]
    public DateTime LastModified { get; set; }

    [ForeignKey("UserID")]
    public User User { get; set; }

    public List<OutfitClothingItem> OutfitClothingItems { get; set; }
}
