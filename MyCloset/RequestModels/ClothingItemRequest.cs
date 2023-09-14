using System;
namespace MyCloset.RequestModels
{
    public class ClothingItemRequest
    {
        public Guid? ClothingItemId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string Photo { get; set; }

        public Guid UserId { get; set; }

        public List<string> Occasions { get; set; } = new();
    }
}

