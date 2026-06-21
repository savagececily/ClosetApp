@description('Name of the CosmosDB account')
param accountName string

@description('Location for the CosmosDB account')
param location string = resourceGroup().location

@description('Tags to apply to the resource')
param tags object = {}

@description('Enable serverless mode (true for dev/test, false for production)')
param enableServerless bool = true

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: accountName
  location: location
  tags: tags
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: false
    enableMultipleWriteLocations: false
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    capabilities: enableServerless ? [
      {
        name: 'EnableServerless'
      }
    ] : []
    backupPolicy: {
      type: 'Periodic'
      periodicModeProperties: {
        backupIntervalInMinutes: 240
        backupRetentionIntervalInHours: 720
        backupStorageRedundancy: 'Local'
      }
    }
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  parent: cosmosDbAccount
  name: 'MyClosetDB'
  properties: {
    resource: {
      id: 'MyClosetDB'
    }
  }
}

// Users container - partition key: /id
resource usersContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: database
  name: 'Users'
  properties: {
    resource: {
      id: 'Users'
      partitionKey: {
        paths: [
          '/id'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
    }
  }
}

// ClothingItems container - partition key: /userId
resource clothingItemsContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: database
  name: 'ClothingItems'
  properties: {
    resource: {
      id: 'ClothingItems'
      partitionKey: {
        paths: [
          '/userId'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
    }
  }
}

// Outfits container - partition key: /userId
resource outfitsContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: database
  name: 'Outfits'
  properties: {
    resource: {
      id: 'Outfits'
      partitionKey: {
        paths: [
          '/userId'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
    }
  }
}

// OutfitRecommendations container - partition key: /userId
resource outfitRecommendationsContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: database
  name: 'OutfitRecommendations'
  properties: {
    resource: {
      id: 'OutfitRecommendations'
      partitionKey: {
        paths: [
          '/userId'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
    }
  }
}

// Friendships container - partition key: /user1
resource friendshipsContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: database
  name: 'Friendships'
  properties: {
    resource: {
      id: 'Friendships'
      partitionKey: {
        paths: [
          '/user1'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
    }
  }
}

// SocialMediaData container - partition key: /userId (shared container with type discriminator)
resource socialMediaDataContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: database
  name: 'SocialMediaData'
  properties: {
    resource: {
      id: 'SocialMediaData'
      partitionKey: {
        paths: [
          '/userId'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
    }
  }
}

output endpoint string = cosmosDbAccount.properties.documentEndpoint
output accountName string = cosmosDbAccount.name
output databaseName string = database.name
output id string = cosmosDbAccount.id
