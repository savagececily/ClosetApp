using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCloset.Models.DBModels;

public class ClothingItem
{
    [Key]
    public Guid ClothingItemID { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    [Required]
    [MaxLength(255)]
    public string Description { get; set; }

    [Required]
    [MaxLength(255)]
    public string Category { get; set; }

    [Required]
    [MaxLength(255)]
    public string LinkToPhoto { get; set; }

    [Required]
    public Guid UserID { get; set; }

    [Required]
    public string Tags { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime LastModified { get; set; }

    [ForeignKey("UserID")]
    public User User { get; set; }

    public List<OutfitClothingItem> OutfitClothingItems { get; set; }
}
