# MyCloset Backend - AI & Social Media Features

## Overview

This document outlines the new AI-powered and social media integration features added to your MyCloset backend application.

## ✨ New Features

### 1. **AI-Powered Outfit Recommendations**
- Get personalized outfit suggestions based on occasion, weather, and season
- AI analyzes your closet items to create harmonious combinations
- Avoids suggesting recently worn outfits

### 2. **Image Analysis with AI**
- Automatically tag clothing items when uploaded
- Detect colors, patterns, and style attributes
- Get AI-suggested categories and descriptions

### 3. **Outfit Completion Suggestions**
- Start building an outfit and get AI suggestions to complete it
- Smart pairing based on colors, styles, and occasion

### 4. **Social Media Integration**
- Link Instagram, Facebook, and other social media accounts
- Track when and where outfits were worn
- Import outfit images from social media posts

### 5. **Outfit History & Analytics**
- Record when outfits are worn
- Track locations and occasions
- View analytics: most worn outfits, wearing patterns, etc.

### 6. **Wardrobe Gap Analysis**
- AI identifies missing wardrobe essentials
- Get suggestions for items to add to your closet

### 7. **Styling Tips**
- Get personalized styling advice for individual items
- See pairing suggestions from your existing closet

## 📁 New Files Added

### Database Models (`Models/DBModels/`)
- `OutfitHistory.cs` - Tracks when outfits were worn
- `SocialMediaPost.cs` - Links to social media posts
- `OutfitRecommendation.cs` - Stores AI-generated recommendations
- `AIImageAnalysis.cs` - Stores AI analysis results for images

### Services (`Services/`)
- **Interfaces:**
  - `IAIService.cs` - AI service interface
  - `ISocialMediaService.cs` - Social media service interface
  
- **Implementations:**
  - `AIService.cs` - AI functionality implementation
  - `SocialMediaService.cs` - Social media tracking implementation

### Controllers (`Controllers/`)
- `AIController.cs` - AI-powered endpoints
- `SocialMediaController.cs` - Social media and history endpoints

### Request Models (`Models/RequestModels/`)
- `AIAndSocialMediaRequests.cs` - Request DTOs for new features

## 🔧 Setup Instructions

### 1. Install Required NuGet Packages

```bash
dotnet add package Azure.AI.OpenAI --version 1.0.0-beta.12
dotnet add package Azure.AI.Vision.ImageAnalysis --version 1.0.0-beta.2
```

### 2. Update Database

You need to create and run Entity Framework migrations for the new tables:

```bash
# Create migration
dotnet ef migrations add AddAIAndSocialMediaFeatures

# Update database
dotnet ef database update
```

### 3. Configure Azure Services

Add these configuration values to your Azure App Configuration or `appsettings.json`:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-openai-resource.openai.azure.com/",
    "Key": "your-azure-openai-key",
    "DeploymentName": "gpt-4"
  },
  "AzureComputerVision": {
    "Endpoint": "https://your-vision-resource.cognitiveservices.azure.com/",
    "Key": "your-computer-vision-key"
  }
}
```

### 4. Set Up Azure OpenAI

1. Create an Azure OpenAI resource in Azure Portal
2. Deploy a GPT-4 or GPT-4 Vision model
3. Copy the endpoint and key to your configuration

### 5. Set Up Azure Computer Vision (Optional)

1. Create an Azure Computer Vision resource
2. Copy the endpoint and key to your configuration
3. This is used for additional image analysis capabilities

## 📡 New API Endpoints

### AI Controller (`/api/AI`)

#### POST `/api/AI/AnalyzeClothingImage`
Analyze a clothing item image with AI
```json
{
  "clothingItemId": "guid",
  "imageUrl": "https://..."
}
```

#### POST `/api/AI/GetOutfitRecommendations`
Get outfit recommendations
```json
{
  "occasion": "casual|business|formal|date",
  "weather": "sunny|rainy|cold|warm",
  "season": "spring|summer|fall|winter",
  "excludeItemIds": ["guid1", "guid2"]
}
```

#### POST `/api/AI/GetOutfitCompletionSuggestions`
Get suggestions to complete a partial outfit
```json
{
  "selectedItemIds": ["guid1", "guid2"],
  "occasion": "casual"
}
```

#### POST `/api/AI/AnalyzeOutfitImage`
Analyze a full outfit image (e.g., from social media)
```json
{
  "imageUrl": "https://..."
}
```

#### GET `/api/AI/GetStylingTips/{clothingItemId}`
Get styling tips for an item

#### GET `/api/AI/IdentifyWardrobeGaps`
Get wardrobe gap analysis

### Social Media Controller (`/api/SocialMedia`)

#### POST `/api/SocialMedia/LinkAccount`
Link a social media account
```json
{
  "platform": "Instagram|Facebook|Twitter",
  "accessToken": "oauth-token"
}
```

#### POST `/api/SocialMedia/SyncPosts`
Sync posts from linked accounts
- Query param: `platform` (optional)

#### POST `/api/SocialMedia/AddPost`
Manually add a social media post
```json
{
  "postUrl": "https://...",
  "platform": "Instagram",
  "outfitId": "guid (optional)"
}
```

#### GET `/api/SocialMedia/OutfitHistory`
Get outfit history
- Query params: `startDate`, `endDate` (optional)

#### POST `/api/SocialMedia/RecordOutfitWorn`
Record when an outfit was worn
```json
{
  "outfitId": "guid",
  "dateWorn": "2024-03-15",
  "location": "Office",
  "occasion": "Work meeting",
  "notes": "Felt great!",
  "socialMediaPostId": "guid (optional)"
}
```

#### GET `/api/SocialMedia/PostsForOutfit/{outfitId}`
Get posts featuring an outfit

#### GET `/api/SocialMedia/Analytics`
Get outfit wearing analytics

#### POST `/api/SocialMedia/FindSimilarOutfits`
Find similar outfits in closet
```json
{
  "imageUrl": "https://..."
}
```

## 🎯 Next Steps

### Immediate Actions

1. **Install NuGet packages** (see Setup Instructions #1)
2. **Run database migrations** (see Setup Instructions #2)
3. **Configure Azure services** (see Setup Instructions #3-5)

### Frontend Integration

You'll need to update your React frontend (`ClientApp/`) to:
1. Add UI for AI outfit recommendations
2. Create outfit history tracking pages
3. Add social media linking functionality
4. Display AI styling tips and suggestions

### Testing

Test the new endpoints using:
- Swagger UI: `https://localhost:[port]/swagger`
- Postman or similar API testing tool

### Optional Enhancements

1. **Social Media OAuth Integration**: Implement proper OAuth flows for Instagram, Facebook, etc.
2. **Image Upload Enhancement**: Add drag-and-drop image uploads
3. **Notifications**: Add notifications when AI finds new outfit combinations
4. **Recommendation Feedback**: Allow users to rate recommendations to improve AI suggestions

## 🔐 Security Notes

- Store Azure OpenAI and Computer Vision keys in Azure Key Vault (not in appsettings.json)
- Use managed identities when deploying to Azure
- Implement rate limiting for AI endpoints to control costs
- Validate all image URLs before processing

## 💰 Cost Considerations

### Azure OpenAI
- GPT-4 costs approximately $0.03-0.06 per 1K tokens
- Estimate: ~500-1000 tokens per outfit recommendation
- Consider using GPT-3.5 Turbo for cost savings

### Azure Computer Vision
- Image analysis: ~$1 per 1,000 images
- Consider caching analysis results

### Optimization Tips
1. Cache AI responses for common requests
2. Batch image analysis when possible
3. Use GPT-3.5 Turbo instead of GPT-4 where appropriate
4. Set token limits on API calls

## 📊 Database Schema Changes

### New Tables
- `OutfitHistory` - Tracks outfit wearing history
- `SocialMediaPost` - Links to external social media posts
- `OutfitRecommendation` - Stores AI recommendations
- `AIImageAnalysis` - Stores image analysis results

### Updated Tables
- `User` - Added navigation properties for new collections
- `Outfit` - Added navigation properties for history and social posts
- `ClothingItem` - Added navigation property for AI analysis

## 🐛 Troubleshooting

### "Unable to connect to Azure OpenAI"
- Verify endpoint and key in configuration
- Check if deployment name matches your Azure OpenAI deployment
- Ensure network connectivity to Azure

### "AI Service returns empty results"
- Check Azure OpenAI quota and rate limits
- Verify model deployment is active
- Review logs for detailed error messages

### "Image analysis fails"
- Ensure image URLs are publicly accessible
- Check image format (JPG, PNG supported)
- Verify Computer Vision endpoint and key

## 📚 Resources

- [Azure OpenAI Documentation](https://learn.microsoft.com/azure/cognitive-services/openai/)
- [Azure Computer Vision Documentation](https://learn.microsoft.com/azure/cognitive-services/computer-vision/)
- [Entity Framework Core Migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/)

## 🤝 Support

For questions or issues with the new features, please review:
1. This documentation
2. Swagger API documentation at `/swagger`
3. Application logs in Azure App Insights

---

**Ready to get started?** Follow the Setup Instructions above and test your new AI-powered closet! 🎉
