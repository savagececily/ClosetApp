@description('Name of the storage account')
param storageAccountName string

@description('Location for the storage account')
param location string = resourceGroup().location

@description('Tags to apply to the resource')
param tags object = {}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
    supportsHttpsTrafficOnly: true
    encryption: {
      services: {
        blob: {
          enabled: true
        }
        file: {
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
    accessTier: 'Hot'
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    deleteRetentionPolicy: {
      enabled: true
      days: 7
    }
  }
}

// Container for clothing item images
resource clothingImagesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: 'clothing-images'
  properties: {
    publicAccess: 'None'
  }
}

// Container for outfit images
resource outfitImagesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: 'outfit-images'
  properties: {
    publicAccess: 'None'
  }
}

// Container for user profile photos
resource profilePhotosContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: 'profile-photos'
  properties: {
    publicAccess: 'None'
  }
}

output id string = storageAccount.id
output name string = storageAccount.name
output blobEndpoint string = storageAccount.properties.primaryEndpoints.blob
