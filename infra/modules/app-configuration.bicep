@description('Name of the App Configuration')
param appConfigName string

@description('Location for the App Configuration')
param location string = resourceGroup().location

@description('Tags to apply to the resource')
param tags object = {}

@description('SKU of the App Configuration (Free, Standard)')
param sku string = 'Standard'

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2023-03-01' = {
  name: appConfigName
  location: location
  tags: tags
  sku: {
    name: sku
  }
  properties: {
    enablePurgeProtection: false
    publicNetworkAccess: 'Enabled'
  }
}

output id string = appConfig.id
output name string = appConfig.name
output endpoint string = appConfig.properties.endpoint
