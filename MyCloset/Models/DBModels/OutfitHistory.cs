using System;
using Newtonsoft.Json;

namespace MyCloset.Models.DBModels;

/// <summary>
/// NOTE: OutfitHistory is now EMBEDDED in Outfit documents as WornHistory array.
/// This class kept for backward compatibility but should use Outfit.WornHistory instead.
/// For analytics queries, use the embedded data in Outfit documents.
/// </summary>
[Obsolete("Use Outfit.WornHistory embedded array for better performance")]
public partial class OutfitHistory
{
    public Guid OutfitHistoryId { get; set; }
    public Guid OutfitId { get; set; }
    public Guid UserId { get; set; }
    public DateTime DateWorn { get; set; }
    public string? Location { get; set; }
    public string? Occasion { get; set; }
    public string? Notes { get; set; }
    public Guid? SocialMediaPostId { get; set; }
    public DateTime DateAdded { get; set; }
}
