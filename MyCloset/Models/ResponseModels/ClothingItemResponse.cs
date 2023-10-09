using System;
namespace MyCloset.Models.ResponseModels
{
	public class ClothingItemResponse
	{

        public Guid? ClothingItemId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public byte[] Image { get; set; }

        public Guid UserId { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}

