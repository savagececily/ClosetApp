# MyCloset Azure Deployment Guide

This guide walks you through deploying the MyCloset application to Azure using Azure Developer CLI (azd).

## Prerequisites

- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) installed
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed
- An Azure subscription with appropriate permissions
- Azure CLI logged in: `az login`

## Architecture Overview

The deployment creates the following Azure resources:

| Resource | Purpose | SKU/Tier |
|----------|---------|----------|
| **App Service** | Hosts the ASP.NET Core Web API | Basic B1 (dev) / Standard S1 (prod) |
| **CosmosDB (NoSQL)** | Document database with 6 containers | Serverless (dev) / Provisioned (prod) |
| **Storage Account** | Blob storage for images | Standard LRS |
| **App Configuration** | Centralized configuration management | Standard |
| **Azure OpenAI** | AI-powered outfit recommendations | Pay-as-you-go |
| **Key Vault** | Secure storage for OAuth secrets | Standard |

All resources use **Managed Identity** for secure authentication (no connection strings in code).

## Quick Start

### 1. Initialize Azure Developer CLI

```bash
azd auth login
azd init
```

When prompted:
- **Environment Name**: Choose a name (e.g., `mycloset-dev` or `mycloset-prod`)
- **Region**: Select your preferred Azure region (e.g., `eastus`)

### 2. Provision & Deploy

Deploy everything with a single command:

```bash
azd up
```

This command will:
1. ✅ Provision all Azure resources (5-10 minutes)
2. ✅ Build the .NET application
3. ✅ Deploy the API to App Service
4. ✅ Configure managed identity and RBAC roles
5. ✅ Output the API endpoint

### 3. Configure OAuth Secrets

After deployment, add your OAuth provider secrets to Key Vault:

```bash
# Get Key Vault name
KEY_VAULT_NAME=$(azd env get-values | grep AZURE_KEY_VAULT_NAME | cut -d '=' -f 2)

# Add Google OAuth secrets
az keyvault secret set --vault-name $KEY_VAULT_NAME --name "GoogleClientId" --value "YOUR_GOOGLE_CLIENT_ID"
az keyvault secret set --vault-name $KEY_VAULT_NAME --name "GoogleClientSecret" --value "YOUR_GOOGLE_CLIENT_SECRET"

# Add Facebook OAuth secrets
az keyvault secret set --vault-name $KEY_VAULT_NAME --name "FacebookAppId" --value "YOUR_FACEBOOK_APP_ID"
az keyvault secret set --vault-name $KEY_VAULT_NAME --name "FacebookAppSecret" --value "YOUR_FACEBOOK_APP_SECRET"

# Add Microsoft OAuth secrets
az keyvault secret set --vault-name $KEY_VAULT_NAME --name "MicrosoftClientId" --value "YOUR_MICROSOFT_CLIENT_ID"
az keyvault secret set --vault-name $KEY_VAULT_NAME --name "MicrosoftClientSecret" --value "YOUR_MICROSOFT_CLIENT_SECRET"
```

### 4. Verify Deployment

```bash
# Get the API endpoint
API_URL=$(azd env get-values | grep APP_SERVICE_URI | cut -d '=' -f 2)

# Test the API
curl $API_URL/api/health
```

## Manual Commands

### Deploy Code Only (after infrastructure is provisioned)

```bash
azd deploy
```

### Provision Infrastructure Only (without deploying code)

```bash
azd provision
```

### View Environment Variables

```bash
azd env get-values
```

### Monitor Application

```bash
azd monitor
```

## Local Development Setup

### 1. Configure Local Settings

Create `appsettings.Development.json` with:

```json
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
  },
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  }
}
```

### 2. Run Locally

```bash
cd MyCloset
dotnet restore
dotnet run
```

The API will be available at `https://localhost:5001`

## CosmosDB Structure

The deployment creates 6 containers:

| Container | Partition Key | Purpose |
|-----------|--------------|---------|
| Users | `/id` | User accounts and profiles |
| ClothingItems | `/userId` | Clothing items in user wardrobes |
| Outfits | `/userId` | User outfit combinations |
| OutfitRecommendations | `/userId` | AI-generated recommendations |
| Friendships | `/user1` | User friend relationships |
| SocialMediaData | `/userId` | Social media connections & posts |

## Security Features

✅ **Managed Identity** - No credentials in code  
✅ **RBAC** - Least privilege access to all resources  
✅ **Key Vault** - Secrets stored securely  
✅ **HTTPS Only** - TLS 1.2 minimum  
✅ **Private Blobs** - No public access to images

## Cost Estimates

### Development Environment
- App Service (Basic B1): ~$13/month
- CosmosDB (Serverless): ~$5-10/month
- Storage: ~$1/month
- App Configuration: ~$1/month
- Azure OpenAI: Pay-per-token (~$5-20/month)
- Key Vault: ~$0.30/month
- **Total**: ~$25-50/month

### Production Environment
- App Service (Standard S1): ~$70/month
- CosmosDB (Provisioned 400 RU/s): ~$25/month
- Storage: ~$5/month
- App Configuration: ~$1/month
- Azure OpenAI: Pay-per-token (~$50-100/month)
- Key Vault: ~$0.30/month
- **Total**: ~$150-200/month

## Troubleshooting

### Deployment Fails with "Resource already exists"

```bash
azd down
azd up
```

### CosmosDB Connection Issues

Verify managed identity has correct role:
```bash
az cosmosdb sql role assignment list --account-name YOUR_COSMOS_ACCOUNT --resource-group YOUR_RG
```

### App Service Not Starting

Check logs:
```bash
az webapp log tail --name YOUR_APP_NAME --resource-group YOUR_RG
```

### Key Vault Access Denied

Grant yourself secrets officer role:
```bash
az role assignment create --role "Key Vault Secrets Officer" --assignee YOUR_USER_ID --scope /subscriptions/YOUR_SUB/resourceGroups/YOUR_RG/providers/Microsoft.KeyVault/vaults/YOUR_KV
```

## Cleanup

To delete all Azure resources:

```bash
azd down
```

## Additional Resources

- [Azure Developer CLI Documentation](https://learn.microsoft.com/azure/developer/azure-developer-cli/)
- [CosmosDB Best Practices](../COSMOSDB_ARCHITECTURE.md)
- [App Service Documentation](https://learn.microsoft.com/azure/app-service/)
- [Managed Identity Documentation](https://learn.microsoft.com/azure/active-directory/managed-identities-azure-resources/)

## Support

For issues or questions:
- Check the [Deployment Plan](../.azure/deployment-plan.md)
- Review [CosmosDB Architecture](../COSMOSDB_ARCHITECTURE.md)
- Open an issue in the repository
