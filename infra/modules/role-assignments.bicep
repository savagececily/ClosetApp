@description('Principal ID of the App Service managed identity')
param appServicePrincipalId string

@description('Name of the CosmosDB account')
param cosmosDbAccountName string

@description('Name of the Storage account')
param storageAccountName string

@description('Name of the Azure OpenAI account (optional)')
param openAiAccountName string = ''

@description('Name of the Key Vault')
param keyVaultName string

// Reference existing resources
resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: cosmosDbAccountName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource openAiAccount 'Microsoft.CognitiveServices/accounts@2023-05-01' existing = if (!empty(openAiAccountName)) {
  name: openAiAccountName
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
}

// CosmosDB Built-in Data Contributor role for App Service
// Note: Cosmos DB uses account-scoped role definitions, not subscription-level
resource cosmosDbDataContributorRole 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2023-04-15' = {
  name: guid(cosmosDbAccount.id, appServicePrincipalId, 'CosmosDBDataContributor')
  parent: cosmosDbAccount
  properties: {
    roleDefinitionId: resourceId('Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions', cosmosDbAccountName, '00000000-0000-0000-0000-000000000002') // Cosmos DB Built-in Data Contributor
    principalId: appServicePrincipalId
    scope: cosmosDbAccount.id
  }
}

// Storage Blob Data Contributor role for App Service
resource storageBlobDataContributorRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, appServicePrincipalId, 'StorageBlobDataContributor')
  scope: storageAccount
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe') // Storage Blob Data Contributor
    principalId: appServicePrincipalId
    principalType: 'ServicePrincipal'
  }
}

// Cognitive Services OpenAI User role for App Service (optional)
resource openAiUserRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (!empty(openAiAccountName)) {
  name: guid(openAiAccount.id, appServicePrincipalId, 'CognitiveServicesOpenAIUser')
  scope: openAiAccount
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd') // Cognitive Services OpenAI User
    principalId: appServicePrincipalId
    principalType: 'ServicePrincipal'
  }
}

// Key Vault Secrets User role for App Service
resource keyVaultSecretsUserRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, appServicePrincipalId, 'KeyVaultSecretsUser')
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // Key Vault Secrets User
    principalId: appServicePrincipalId
    principalType: 'ServicePrincipal'
  }
}

output cosmosDbRoleId string = cosmosDbDataContributorRole.id
output storageRoleId string = storageBlobDataContributorRole.id
output openAiRoleId string = !empty(openAiAccountName) ? openAiUserRole.id : ''
output keyVaultRoleId string = keyVaultSecretsUserRole.id
