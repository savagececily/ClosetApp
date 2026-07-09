using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCloset.Models.DBModels
{
    public class OutfitClothingItem
    {
        [Key]
        public Guid OutfitID { get; set; }

        [Key]
        public Guid ClothingItemID { get; set; }

        [ForeignKey("OutfitID")]
        public required Outfit Outfit { get; set; }

        [ForeignKey("ClothingItemID")]
        public required ClothingItem ClothingItem { get; set; }
    }
}

