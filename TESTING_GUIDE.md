# MyCloset API Testing Guide

## Overview
This guide explains how to test the MyCloset API using test users without requiring OAuth authentication setup.

## Deployed API
- **Base URL**: https://app-euz4k3e3dqvkq.azurewebsites.net
- **Swagger UI**: https://app-euz4k3e3dqvkq.azurewebsites.net/swagger
- **Environment**: Staging (dev endpoints enabled)

## Test Users Created

Two test users have been created in the Cosmos DB:

### Alice Test
- **User ID**: `c791f12f-e393-4f30-a3f6-f73b8cc2ae3e`
- **Email**: alice@test.com
- **Display Name**: Alice Test
- **Account Provider**: dev-test

### Bob Demo
- **User ID**: `c53abbce-5feb-46af-8619-67c2be9d98d0`
- **Email**: bob@test.com
- **Display Name**: Bob Demo
- **Account Provider**: dev-test

## Authentication Bypass for Testing

In Development and Staging environments, the API supports authentication bypass using a custom header.

### Header: `X-Test-User-Id`
Add this header to any API request with a test user's GUID to authenticate as that user.

### Example: cURL
```bash
# Test as Alice
curl -X POST \
  -H "X-Test-User-Id: c791f12f-e393-4f30-a3f6-f73b8cc2ae3e" \
  -H "Content-Type: application/json" \
  https://app-euz4k3e3dqvkq.azurewebsites.net/api/Closet/GetCloset

# Test as Bob
curl -X POST \
  -H "X-Test-User-Id: c53abbce-5feb-46af-8619-67c2be9d98d0" \
  -H "Content-Type: application/json" \
  https://app-euz4k3e3dqvkq.azurewebsites.net/api/Closet/GetCloset
```

### Example: Postman
1. Open Postman
2. Create a new request
3. Add header:
   - **Key**: `X-Test-User-Id`
   - **Value**: `c791f12f-e393-4f30-a3f6-f73b8cc2ae3e`
4. Make your API request

### Example: JavaScript/Fetch
```javascript
const response = await fetch('https://app-euz4k3e3dqvkq.azurewebsites.net/api/Closet/GetCloset', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'X-Test-User-Id': 'c791f12f-e393-4f30-a3f6-f73b8cc2ae3e'
  }
});
```

## Dev Endpoints Available

The API includes several development-only endpoints for managing test users:

### 1. Get All Test Users
```bash
GET /api/Dev/users
curl https://app-euz4k3e3dqvkq.azurewebsites.net/api/Dev/users
```

### 2. Get User by Email
```bash
GET /api/Dev/user/{email}
curl https://app-euz4k3e3dqvkq.azurewebsites.net/api/Dev/user/alice@test.com
```

### 3. Seed Test Users (Create Alice & Bob)
```bash
POST /api/Dev/seed-users
curl -X POST https://app-euz4k3e3dqvkq.azurewebsites.net/api/Dev/seed-users
```

### 4. Create Custom Test User
```bash
POST /api/Dev/create-user
curl -X POST \
  -H "Content-Type: application/json" \
  -d '{"email":"charlie@test.com","displayName":"Charlie Test"}' \
  https://app-euz4k3e3dqvkq.azurewebsites.net/api/Dev/create-user
```

### 5. Get Authentication Help
```bash
GET /api/Dev/auth-help
curl https://app-euz4k3e3dqvkq.azurewebsites.net/api/Dev/auth-help
```

### 6. Cleanup Test Users
```bash
DELETE /api/Dev/cleanup
curl -X DELETE https://app-euz4k3e3dqvkq.azurewebsites.net/api/Dev/cleanup
```

## API Endpoints to Test

### User Endpoints
- `POST /api/User/AccountDetails` - Get user account details
- `POST /api/User/AddAccount` - Add account (not implemented)
- `PUT /api/User/EditAccount` - Edit account (not implemented)
- `DELETE /api/User/DeleteAccount` - Delete account (not implemented)

### Closet Endpoints
- `POST /api/Closet/GetCloset` - Get user's clothing items
- `POST /api/Closet/GetItem` - Get specific clothing item
- `POST /api/Closet/AddItem` - Add clothing item
- `POST /api/Closet/EditItem` - Edit clothing item
- `DELETE /api/Closet/DeleteItem` - Delete clothing item

### Outfit Endpoints
- `POST /api/Closet/GetOutfits` - Get user's outfits
- `POST /api/Closet/CreateOutfit` - Create new outfit
- `PUT /api/Closet/UpdateOutfit` - Update outfit
- `DELETE /api/Closet/DeleteOutfit` - Delete outfit

### AI Endpoints
- `POST /api/AI/AnalyzeImage` - Analyze clothing image
- `POST /api/AI/RecommendOutfits` - Get AI outfit recommendations
- `POST /api/AI/SuggestPairings` - Get AI pairing suggestions

### Friends Endpoints
- `GET /api/Friends/GetFriendRequests` - Get friend requests
- `POST /api/Friends/SendRequest` - Send friend request
- `POST /api/Friends/AcceptRequest` - Accept friend request
- `POST /api/Friends/RejectRequest` - Reject friend request

## Testing Workflow

### 1. Verify API is Running
```bash
curl https://app-euz4k3e3dqvkq.azurewebsites.net/swagger
```

### 2. Check Test Users Exist
```bash
curl https://app-euz4k3e3dqvkq.azurewebsites.net/api/Dev/users | jq
```

### 3. Test Authenticated Endpoint
```bash
curl -X POST \
  -H "X-Test-User-Id: c791f12f-e393-4f30-a3f6-f73b8cc2ae3e" \
  -H "Content-Type: application/json" \
  https://app-euz4k3e3dqvkq.azurewebsites.net/api/Closet/GetCloset | jq
```

### 4. Test Adding Clothing Item
```bash
curl -X POST \
  -H "X-Test-User-Id: c791f12f-e393-4f30-a3f6-f73b8cc2ae3e" \
  -H "Content-Type: application/json" \
  -d '{"name":"Blue Jeans","category":"Pants","color":"Blue"}' \
  https://app-euz4k3e3dqvkq.azurewebsites.net/api/Closet/AddItem | jq
```

## Mobile App Configuration

To configure the MAUI mobile app to use the deployed API:

### Update ApiService Base URL
In `MyCloset.Mobile/Services/ApiService.cs` or configuration:
```csharp
private const string BaseUrl = "https://app-euz4k3e3dqvkq.azurewebsites.net";
```

### Add Test User Header
Add the `X-Test-User-Id` header to all HTTP requests:
```csharp
httpClient.DefaultRequestHeaders.Add("X-Test-User-Id", "c791f12f-e393-4f30-a3f6-f73b8cc2ae3e");
```

## Security Notes

⚠️ **Important**: The authentication bypass is **only enabled** in Development and Staging environments.

- The `X-Test-User-Id` header is ignored in Production
- Dev endpoints return 404 in Production
- This is intended for testing purposes only

## Troubleshooting

### Health Check Returns 503
The `/health` endpoint may return 503 if:
- Cosmos DB RBAC permissions are still propagating (can take 10-15 minutes)
- However, test users were created successfully, so write permissions are working
- The health check specifically tests DbContext connection

### Test Users Not Found
If test users are missing, recreate them:
```bash
curl -X POST https://app-euz4k3e3dqvkq.azurewebsites.net/api/Dev/seed-users
```

### Authentication Not Working
Verify:
1. App Service environment is set to "Staging" (check in Azure Portal)
2. You're using the correct header name: `X-Test-User-Id`
3. User ID is a valid GUID from the test users list

## Next Steps

1. **Test Basic Endpoints**: Use cURL or Postman to test each endpoint with test user authentication
2. **Configure Mobile App**: Update mobile app to point to deployed API and add test user header
3. **Test End-to-End Flow**: Test the full user journey from mobile app to API to database
4. **Implement Missing Endpoints**: Complete UserController methods that throw NotImplementedException
5. **Add Production Authentication**: Implement proper OAuth flow for production use

## Resources

- **Azure Portal**: https://portal.azure.com/#@/resource/subscriptions/a7c4f882-34af-44dc-9bd7-ccac4f1ec402/resourceGroups/rg-mycloset-dev/overview
- **App Service**: app-euz4k3e3dqvkq
- **Cosmos DB**: cosmos-euz4k3e3dqvkq
- **Storage Account**: steuz4k3e3dqvkq
- **Key Vault**: kv-euz4k3e3dqvkq

## Summary of Changes

### Infrastructure
- ✅ Deployed to Azure App Service (Linux, .NET 9.0)
- ✅ Cosmos DB with all 6 containers created
- ✅ Managed identity authentication with RBAC
- ✅ Environment set to "Staging" for dev endpoints
- ✅ Removed Azure App Configuration to reduce costs

### Code Changes
- ✅ Created `DevController` with test user management endpoints
- ✅ Implemented authentication bypass via `X-Test-User-Id` header in `BaseController`
- ✅ Fixed DbContext configuration for obsolete entities (AIImageAnalysis, OutfitHistory)
- ✅ All Cosmos DB calls now use async methods

### Test Data
- ✅ 2 test users created (Alice Test, Bob Demo)
- ✅ Users have valid GUIDs for testing
- ✅ Users marked with "dev-test" provider for easy cleanup
