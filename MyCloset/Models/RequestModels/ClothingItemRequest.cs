using System;
namespace MyCloset.Models.RequestModels
{
    public class ClothingItemRequest
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string Image { get; set; } // For storing image as base64 string

        public Guid? UserId { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}

