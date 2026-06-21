using System;

namespace MyCloset.Models.DBModels;

/// <summary>
/// NOTE: AIImageAnalysis is now EMBEDDED in ClothingItem documents as AIAnalysis property.
/// This class kept for backward compatibility but should use ClothingItem.AIAnalysis instead.
/// For new implementations, use the EmbeddedAIAnalysis class embedded in ClothingItem.
/// </summary>
[Obsolete("Use ClothingItem.AIAnalysis embedded property for better performance")]
public partial class AIImageAnalysis
{
    public Guid AnalysisId { get; set; }
    public Guid ClothingItemId { get; set; }
    public string GeneratedTags { get; set; } = null!;
    public string Colors { get; set; } = null!;
    public string? Patterns { get; set; }
    public string? SuggestedCategory { get; set; }
    public string? Description { get; set; }
    public string? StyleTags { get; set; }
    public int ConfidenceScore { get; set; }
    public DateTime DateAnalyzed { get; set; }
}
