using System;
namespace MyCloset.Models.RequestModels
{
    public class ClothingItemRequest
    {
        public Guid? Id { get; set; }
        public required string Title { get; set; }

        public required string Description { get; set; }

        public required string Category { get; set; }

        public required string Image { get; set; }

        public Guid? UserId { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}

