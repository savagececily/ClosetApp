# Azure Deployment Plan - MyCloset

**Status**: Ready for Deployment  
**Created**: 2026-06-21  
**Updated**: 2026-06-21 (Infrastructure generated, ready for azd up)  
**Mode**: MODIFY - Adding Azure infrastructure to existing application

---

## Project Overview

**Application**: MyCloset - Wardrobe management application  
**Tech Stack**: ASP.NET Core 8.0 Web API with .NET MAUI mobile client  
**Database**: Azure Cosmos DB (NoSQL)  
**.NET Version**: .NET 8 (LTS, supported until November 2026)  

---

## Current State

- ✅ ASP.NET Core 8.0 Web API
- ✅ Upgraded to .NET 8 LTS (from .NET 7)
- ✅ CosmosDB models fully converted
- ✅ Service layer updated for CosmosDB
- ✅ Application builds successfully (0 errors)
- ✅ Azure.Identity 1.13.1 (security vulnerabilities fixed)
- ✅ **Bicep infrastructure templates generated**
- ✅ **azure.yaml created for azd**
- ✅ **Program.cs updated for cloud configuration**
- ✅ **Deployment documentation complete**
- 🎯 **Ready to deploy with `azd up`**

---

## Goals

1. Add Azure App Configuration support for centralized configuration management
2. Create Bicep templates for infrastructure as code
3. Prepare application for Azure deployment

---

## Analysis (Completed)

### Step 1: Workspace Analysis8.0 API
- Azure App Configuration **already integrated** in code (needs infrastructure)
- Primary project: MyCloset.csproj (ASP.NET Core 8
- Azure App Configuration **already integrated** in code (needs infrastructure)
- Primary project: MyCloset.csproj (ASP.NET Core 7.0 Web API)
- Additional project: MyCloset.Mobile.csproj (.NET MAUI - mobile client)

### Step 2: Requirements Gathering
- **Deployment Target**: Azure App Service (recommended for ASP.NET Core)
- **Scale**: Testing → Small Production (<1000 users)
- **Region**: East US
- **Tier**: Start with Basic B1 ($~13/month), upgrade to Standard S1 for production
- **Infrastructure**: Azure Developer CLI (azd) with Bicep templates

### Step 3: Codebase Scan

**Existing Azure Services (detected in code):**
- ✅ Azure App Configuration - Code integration exists, needs infrastructure
- ✅ CosmosDB - Using EF Core Cosmos provider 7.0.13
- ✅ Azure Blob Storage - BlobStorageService implemented
- ✅ Azure AI OpenAI - AIService with Azure.AI.OpenAI 2.1.0
- ✅ OAuth Authentication - Google, Facebook, Microsoft providers configured
- ✅ Managed Identity - Using DefaultAzureCredential

**Dependencies:**
- Microsoft.EntityFrameworkCore.Cosmos 8.0.22
- Azure.Storage.Blobs 12.18.0
- Azure.AI.OpenAI 2.1.0
- Microsoft.Extensions.Configuration.AzureAppConfiguration 6.1.1
- Azure.Identity 1.13.1 (latest, security fixes)

### Step 4: Recipe Selection
**Selected: Azure Developer CLI (azd) with Bicep**
- Fast scaffolding and deployment
- Bicep templates for infrastructure as code
- Built-in environment management
- Easy local → Azure workflow

### Step 5: Architecture Planning

**Azure Resources to Provision:**

1. **Azure App Service** (Linux, .NET 8)
   - Hosts the ASP.NET Core Web API
   - Managed identity enabled for secure access to other services
   - Basic B1 tier (testing), upgrade to Standard S1 (production)

2. **Azure App Configuration**
   - Centralized configuration management
   - Stores connection strings and app settings securely
   - Feature flags support for future use
   - Standard tier

3. **Azure Cosmos DB (NoSQL)**
   - Database for user data, clothing items, outfits
   - Serverless mode for cost optimization during testing
   - Provisioned throughput for production
   - Containers: Users, ClothingItems, Outfits, Friendships, SocialMediaData, OutfitRecommendations

4. **Azure Storage Account**
   - Blob storage for clothing item images
   - Standard LRS (locally redundant)
   - Private endpoint for secure access (production)

5. **Azure OpenAI**
   - AI-powered outfit recommendations
   - Image analysis for clothing items
   - Model: GPT-4 (configurable deployment name)

6. **Azure Key Vault** (recommended)
   - Secure storage for OAuth secrets
   - Connection strings
   - API keys

**Service Communication:**
```
┌─────────────────┐
│  Mobile Client  │
│   (.NET MAUI)   │
└────────┬────────┘
         │ HTTPS
         ↓
┌─────────────────────────────────────┐
│     Azure App Service (API)         │
│  ┌──────────────────────────────┐  │
│  │   ASP.NET Core 7.0 Web API   │  │
│  │   + Managed Identity         │  │
│  └──────────────────────────────┘  │
└─────┬──────┬──────┬──────┬─────────┘
      │      │      │      │
      ↓      ↓      ↓      ↓
┌──────────┐┌─────┐┌──────┐┌────────┐
│ CosmosDB ││App  ││Blob  ││OpenAI  │
│  (NoSQL) ││Config│Storage│Service │
└──────────┘└─────┘└──────┘└────────┘
             ↓
        ┌─────────┐
        │Key Vault│
        └─────────┘
```

**Configuration Strategy:**
- App Configuration stores: CosmosDB connection, Storage account name, OpenAI endpoint
- Key Vault stores: OAuth secrets, API keys
- Managed Identity: App Service → App Configuration → Key Vault (no secrets in code)
- Environment-based labels in App Configuration (Development, Staging, Production)

---

## Implementation Plan

### Phase 2: Execution Tasks

**1. Generate Azure Infrastructure (Bicep)**
   - Create `./infra/main.bicep` - Main infrastructure template
   - Create `./infra/app-service.bicep` - App Service module
   - Create `./infra/cosmos-db.bicep` - CosmosDB with 6 containers
   - Create `./infra/app-config.bicep` - App Configuration resource
   - Create `./infra/storage.bicep` - Blob Storage account
   - Create `./infra/openai.bicep` - Azure OpenAI service
   - Create `./infra/key-vault.bicep` - Key Vault for secrets
   - Create `./infra/main.parameters.json` - Environment parameters

**2. Create azd Configuration**
   - Create `azure.yaml` - Azure Developer CLI project config
   - Define service mapping (API → App Service)
   - Configure hooks for build and deployment

**3. Update Application Code**
   - Update `Program.cs` - Read App Configuration URL from environment
   - Update `appsettings.json` - Remove hardcoded values
   - Add connection string configuration for local development
   - Add deployment documentation

**4. Security Hardening**
   - Configure managed identity for App Service
   - Grant RBAC permissions (App Config Data Reader, Storage Blob Contributor)
   - Key Vault access policies for secrets
   - Enable HTTPS only on App Service

**5. CosmosDB Container Setup**
   - Create containers with proper partition keys:
     - Users → partition key: `/id`
     - ClothingItems → partition key: `/userId`
     - Outfits → partition key: `/userId`
     - OutfitRecommendations → partition key: `/userId`
     - Friendships → partition key: `/user1`
     - SocialMediaData → partition key: `/userId`

---

## Artifacts to Generate

```
MyCloset/
├── .azure/
│   └── deployment-plan.md (this file)
├── azure.yaml (azd config)
└── infra/
    ├── main.bicep (main orchestrator)
    ├── main.parameters.json (environment params)
    ├── modules/
    │   ├── app-service.bicep
    │   ├── app-configuration.bicep
    │   ├── cosmos-db.bicep
    │   ├── storage-account.bicep
    │   ├── azure-openai.bicep
    │   └── key-vault.bicep
    └── README.md (deployment guide)
```

---

## Cost Estimates (East US, monthly)

**Testing/Development:**
- App Service (Basic B1): ~$13
- CosmosDB (Serverless): ~$1-10 (usage-based)
- Storage Account (Standard LRS): ~$0.50
- App Configuration (Standard): ~$1.20
- Azure OpenAI (Pay-as-you-go): Variable, ~$10-50/month
- **Total: ~$25-75/month**

**Small Production (<1000 users):**
- App Service (Standard S1): ~$70
- CosmosDB (Provisioned 400 RU/s): ~$24
- Storage Account: ~$2
- App Configuration: ~$1.20
- Azure OpenAI: ~$50-100/month
- Key Vault: ~$0.50
- **Total: ~$150-200/month**

---

## Security Considerations

1. **Managed Identity**: App Service uses system-assigned managed identity
2. **No Secrets in Code**: All secrets in Key Vault, references in App Config
3. **RBAC**: Least-privilege access roles
4. **Private Endpoints**: Consider for production (CosmosDB, Storage)
5. **CORS**: Configure allowed origins for mobile client
6. **OAuth Secrets**: Store in Key Vault, reference via App Configuration

---

## Deployment Workflow

```bash
# 1. Initialize azd
azd init

# 2. Set environment variables
azd env set AZURE_LOCATION eastus

# 3. Provision infrastructure
azd provision

# 4. Deploy application
azd deploy

# 5. Configure App Configuration with secrets
# (Manual step or via post-provision script)
```

---

## Validation Checklist

Before deployment:
- [ ] Bicep templates lint successfully
- [ ] Parameter files configured for environment
- [ ] Managed identity permissions configured
- [ ] CosmosDB partition keys validated
- [ ] Application builds successfully (already ✅)
- [ ] Connection strings configured
- [ ] OAuth providers registered with Azure URLs

After deployment:
- [ ] App Service is running
- [ ] CosmosDB containers created with correct partition keys
- [ ] App Configuration accessible via managed identity
- [ ] Storage account contains container for images
- [ ] API endpoints respond correctly
- [ ] Mobile client can connect to API

---

1. Complete requirements gathering
2. Scan codebase for services and dependencies
3. Select deployment recipe (AZD/Bicep/Terraform)
4. Map services to Azure resources
5. Generate infrastructure code
6. Add Azure App Configuration integration
7. Validate and deploy

---

## Notes

- Application already uses CosmosDB (connection configured in appsettings.json)
- Existing services: BlobStorageService, Azure AI OpenAI
- OAuth authentication already configured (Google, Facebook, Microsoft)
