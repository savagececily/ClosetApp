using System;
using MyCloset.Models;

namespace MyCloset.Models.RequestModels
{
    public class OutfitRequest
    {
        public Guid? OutfitId { get; set; }

        public string Title { get; set; }

        public Guid UserId { get; set; }

        public List<Guid> ClothingItemIds { get; set; } = new List<Guid>();
    }
}

