# MyCloset - AI-Powered Wardrobe Management

A full-stack wardrobe management application with AI-powered outfit recommendations, built with ASP.NET Core 8.0 and .NET MAUI.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Azure](https://img.shields.io/badge/Azure-Ready-0078D4?logo=microsoft-azure)](https://azure.microsoft.com)
[![CosmosDB](https://img.shields.io/badge/CosmosDB-NoSQL-2D9CDB)](https://azure.microsoft.com/services/cosmos-db/)

## 🌟 Overview

MyCloset helps you organize your wardrobe, create outfits, and get AI-powered styling recommendations. Connect with friends, track your outfit history, and leverage Azure OpenAI for intelligent fashion insights.

### Key Features

- 👕 **Digital Closet** - Organize clothing items with photos, categories, colors, and custom tags
- 🎨 **Outfit Builder** - Create, save, and track outfit combinations
- 🤖 **AI Recommendations** - Get personalized outfit suggestions powered by GPT-4
- 🔍 **Smart Image Analysis** - Automatic tagging, color detection, and style recognition
- 📱 **Mobile Apps** - Native iOS and Android apps with .NET MAUI
- 👥 **Social Features** - Connect with friends, share outfits, track wear history
- 📊 **Wardrobe Analytics** - Identify gaps, track most-worn items, and optimize your closet

## 📂 Repository Structure

\`\`\`
MyCloset/
├── MyCloset/                    # ASP.NET Core 8.0 Web API
│   ├── Controllers/             # REST API endpoints
│   ├── Services/                # Business logic layer
│   ├── Models/                  # CosmosDB document models
│   └── Utilities/               # Helper classes
├── MyCloset.Mobile/             # .NET MAUI mobile app (iOS, Android)
│   ├── Pages/                   # XAML UI pages
│   ├── ViewModels/              # MVVM view models
│   └── Services/                # HTTP API client
├── infra/                       # Azure infrastructure (Bicep)
│   ├── main.bicep              # Main deployment template
│   └── modules/                # Resource modules
├── docs/                        # Technical documentation
└── azure.yaml                   # Azure Developer CLI config
\`\`\`

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure subscription](https://azure.microsoft.com/free/) (for deployment)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) (for deployment)

### Local Development

1. **Clone the repository**
   \`\`\`bash
   git clone https://github.com/savagececily/ClosetApp.git
   cd MyCloset
   \`\`\`

2. **Configure local settings**
   
   Create \`MyCloset/appsettings.Development.json\`:
   \`\`\`json
   {
     "CosmosDb": {
       "ConnectionString": "YOUR_COSMOS_CONNECTION_STRING",
       "DatabaseName": "MyClosetDB"
     },
     "BlobStorage": {
       "ConnectionString": "YOUR_STORAGE_CONNECTION_STRING"
     },
     "AzureOpenAI": {
       "Endpoint": "YOUR_OPENAI_ENDPOINT",
       "ApiKey": "YOUR_OPENAI_KEY"
     }
   }
   \`\`\`

3. **Run the API**
   \`\`\`bash
   cd MyCloset
   dotnet restore
   dotnet run
   \`\`\`
   
   API available at \`https://localhost:5001\`

### Deploy to Azure

Deploy the entire application to Azure with a single command:

\`\`\`bash
# Login and initialize
azd auth login
azd init

# Deploy everything (infrastructure + code)
azd up
\`\`\`

This deploys:
- ✅ App Service (Linux, .NET 8)
- ✅ CosmosDB (NoSQL, 6 containers)
- ✅ Storage Account (blob storage)
- ✅ App Configuration
- ✅ Azure OpenAI (GPT-4)
- ✅ Key Vault (for secrets)

**Cost**: ~$25-50/month (dev), ~$150-200/month (production)

See [Deployment Guide](infra/README.md) for detailed instructions.

## 🏗️ Architecture

### Backend API (.NET 8)
- **Framework**: ASP.NET Core 8.0 Web API
- **Database**: Azure CosmosDB NoSQL (6 containers)
- **Storage**: Azure Blob Storage (images)
- **AI**: Azure OpenAI (GPT-4, GPT-4 Vision)
- **Auth**: OAuth 2.0 (Google, Facebook, Microsoft)
- **Configuration**: Azure App Configuration
- **Security**: Managed Identity, RBAC, Key Vault

### Mobile App (.NET MAUI)
- **Platforms**: iOS, Android, macOS Catalyst, Windows
- **Architecture**: MVVM with CommunityToolkit.Mvvm
- **HTTP Client**: REST API integration
- **Target**: .NET 7.0 (planned upgrade to .NET 8)

### Database Schema

CosmosDB containers with optimized partition keys:

| Container | Partition Key | Purpose |
|-----------|--------------|---------|
| Users | \`/id\` | User profiles and settings |
| ClothingItems | \`/userId\` | User's clothing inventory |
| Outfits | \`/userId\` | Saved outfit combinations |
| OutfitRecommendations | \`/userId\` | AI-generated suggestions |
| Friendships | \`/user1\` | Friend relationships |
| SocialMediaData | \`/userId\` | Social media connections & posts |

See [CosmosDB Architecture](docs/COSMOSDB_ARCHITECTURE.md) for details.

## 📚 Documentation

- **[Deployment Guide](infra/README.md)** - Complete Azure deployment walkthrough
- **[CosmosDB Architecture](docs/COSMOSDB_ARCHITECTURE.md)** - Database schema, queries, RU costs
- **[CosmosDB Migration](docs/COSMOSDB_MIGRATION_SUMMARY.md)** - Migration guide from SQL Server
- **[AI Features](docs/AI_FEATURES.md)** - GPT-4 integration and capabilities

## 🛡️ Security Features

- ✅ **Managed Identity** - No credentials in code
- ✅ **RBAC** - Least privilege access to all Azure resources
- ✅ **Key Vault** - Secure storage for OAuth secrets
- ✅ **HTTPS Only** - TLS 1.2 minimum
- ✅ **Private Containers** - No public blob access
- ✅ **Soft Delete** - Enabled on Key Vault and Storage

## 🧪 Development Status

| Component | Status | Version |
|-----------|--------|---------|
| Backend API | ✅ Ready | .NET 8.0 LTS |
| CosmosDB Models | ✅ Optimized | Partition keys configured |
| Azure Infrastructure | ✅ Ready | Bicep templates complete |
| Mobile App | 🚧 In Progress | .NET MAUI 7.0 |
| Deployment | ✅ Ready | azd up works |

**Build Status**: ✅ 0 errors, 90 warnings (nullable reference warnings)

## 🎯 Roadmap

- [ ] Upgrade Mobile app to .NET 8
- [ ] Add Application Insights telemetry
- [ ] Implement zone redundancy for production
- [ ] Add health endpoints for monitoring
- [ ] Implement image optimization (thumbnails)
- [ ] Add offline mode for mobile app
- [ ] Implement push notifications

## 🤝 Contributing

This is a personal project, but suggestions and feedback are welcome! Open an issue or submit a PR.

## 📄 License

Copyright © 2026 Cecily Savage. All rights reserved.

## 🔗 Links

- **Repository**: [savagececily/ClosetApp](https://github.com/savagececily/ClosetApp)
- **Azure OpenAI**: [Documentation](https://learn.microsoft.com/azure/ai-services/openai/)
- **CosmosDB**: [Best Practices](https://learn.microsoft.com/azure/cosmos-db/)
- **Azure Developer CLI**: [Get Started](https://learn.microsoft.com/azure/developer/azure-developer-cli/)

---

**Questions?** Check the [documentation](docs/) or open an issue.
