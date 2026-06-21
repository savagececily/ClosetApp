targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment (e.g., dev, test, prod)')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string

@description('Name of the App Service')
param appServiceName string = ''

@description('Name of the CosmosDB account')
param cosmosDbAccountName string = ''

@description('Name of the Storage Account')
param storageAccountName string = ''

@description('Name of the App Configuration')
param appConfigName string = ''

@description('Name of the Key Vault')
param keyVaultName string = ''

@description('Name of the Azure OpenAI account')
param openAiAccountName string = ''

@description('Id of the principal to grant Key Vault access')
param principalId string = ''

// Tags to apply to all resources
var tags = {
  'azd-env-name': environmentName
  'app': 'MyCloset'
}

// Organize resources in a resource group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}

// CosmosDB for NoSQL document database
module cosmosDb './modules/cosmos-db.bicep' = {
  name: 'cosmosdb-deployment'
  scope: rg
  params: {
    accountName: !empty(cosmosDbAccountName) ? cosmosDbAccountName : 'cosmos-${uniqueString(rg.id)}'
    location: location
    tags: tags
  }
}

// Storage Account for blob storage (clothing images)
module storage './modules/storage-account.bicep' = {
  name: 'storage-deployment'
  scope: rg
  params: {
    storageAccountName: !empty(storageAccountName) ? storageAccountName : 'st${uniqueString(rg.id)}'
    location: location
    tags: tags
  }
}

// App Configuration for centralized configuration
module appConfig './modules/app-configuration.bicep' = {
  name: 'appconfig-deployment'
  scope: rg
  params: {
    appConfigName: !empty(appConfigName) ? appConfigName : 'appconfig-${uniqueString(rg.id)}'
    location: location
    tags: tags
  }
}

// Key Vault for secrets
module keyVault './modules/key-vault.bicep' = {
  name: 'keyvault-deployment'
  scope: rg
  params: {
    keyVaultName: !empty(keyVaultName) ? keyVaultName : 'kv-${uniqueString(rg.id)}'
    location: location
    tags: tags
    principalId: principalId
  }
}

// Azure OpenAI for AI-powered features
module openAi './modules/azure-openai.bicep' = {
  name: 'openai-deployment'
  scope: rg
  params: {
    accountName: !empty(openAiAccountName) ? openAiAccountName : 'openai-${uniqueString(rg.id)}'
    location: location
    tags: tags
  }
}

// App Service Plan and App Service for hosting the API
module appService './modules/app-service.bicep' = {
  name: 'appservice-deployment'
  scope: rg
  params: {
    appServiceName: !empty(appServiceName) ? appServiceName : 'app-${uniqueString(rg.id)}'
    location: location
    tags: tags
    appConfigEndpoint: appConfig.outputs.endpoint
    cosmosDbEndpoint: cosmosDb.outputs.endpoint
    cosmosDbDatabaseName: cosmosDb.outputs.databaseName
    storageAccountName: storage.outputs.name
    openAiEndpoint: openAi.outputs.endpoint
    keyVaultName: keyVault.outputs.name
  }
}

// Grant App Service managed identity access to resources
module appServiceRoleAssignments './modules/role-assignments.bicep' = {
  name: 'appservice-roles-deployment'
  scope: rg
  params: {
    appServicePrincipalId: appService.outputs.managedIdentityPrincipalId
    appConfigName: appConfig.outputs.name
    cosmosDbAccountName: cosmosDb.outputs.accountName
    storageAccountName: storage.outputs.name
    openAiAccountName: openAi.outputs.accountName
    keyVaultName: keyVault.outputs.name
  }
}

// Outputs for the deployment
output AZURE_LOCATION string = location
output AZURE_TENANT_ID string = tenant().tenantId
output AZURE_RESOURCE_GROUP string = rg.name

output APP_SERVICE_NAME string = appService.outputs.name
output APP_SERVICE_URI string = appService.outputs.uri
output APP_SERVICE_IDENTITY_PRINCIPAL_ID string = appService.outputs.managedIdentityPrincipalId

output AZURE_COSMOS_DB_ENDPOINT string = cosmosDb.outputs.endpoint
output AZURE_COSMOS_DB_DATABASE_NAME string = cosmosDb.outputs.databaseName

output AZURE_STORAGE_ACCOUNT_NAME string = storage.outputs.name
output AZURE_STORAGE_BLOB_ENDPOINT string = storage.outputs.blobEndpoint

output AZURE_APP_CONFIG_ENDPOINT string = appConfig.outputs.endpoint

output AZURE_OPENAI_ENDPOINT string = openAi.outputs.endpoint

output AZURE_KEY_VAULT_NAME string = keyVault.outputs.name
output AZURE_KEY_VAULT_ENDPOINT string = keyVault.outputs.endpoint
