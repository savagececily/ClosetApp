@description('Name of the App Service')
param appServiceName string

@description('Location for the App Service')
param location string = resourceGroup().location

@description('Tags to apply to the resource')
param tags object = {}

@description('App Configuration endpoint')
param appConfigEndpoint string

@description('CosmosDB endpoint')
param cosmosDbEndpoint string

@description('CosmosDB database name')
param cosmosDbDatabaseName string

@description('Storage account name')
param storageAccountName string

@description('Azure OpenAI endpoint')
param openAiEndpoint string

@description('Key Vault name')
param keyVaultName string

@description('App Service Plan SKU (B1 for dev, S1 for prod)')
param appServicePlanSku string = 'B1'

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: '${appServiceName}-plan'
  location: location
  tags: tags
  sku: {
    name: appServicePlanSku
    tier: appServicePlanSku == 'B1' ? 'Basic' : 'Standard'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

// App Service
resource appService 'Microsoft.Web/sites@2022-09-01' = {
  name: appServiceName
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: appServicePlanSku != 'B1' // Always On not available on Basic tier
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'AZURE_APP_CONFIG_ENDPOINT'
          value: appConfigEndpoint
        }
        {
          name: 'CosmosDb__Endpoint'
          value: cosmosDbEndpoint
        }
        {
          name: 'CosmosDb__DatabaseName'
          value: cosmosDbDatabaseName
        }
        {
          name: 'BlobStorage__AccountName'
          value: storageAccountName
        }
        {
          name: 'AzureOpenAI__Endpoint'
          value: openAiEndpoint
        }
        {
          name: 'KeyVault__Name'
          value: keyVaultName
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
      ]
      cors: {
        allowedOrigins: [
          'https://portal.azure.com'
        ]
        supportCredentials: false
      }
    }
  }
}

output id string = appService.id
output name string = appService.name
output uri string = 'https://${appService.properties.defaultHostName}'
output managedIdentityPrincipalId string = appService.identity.principalId
