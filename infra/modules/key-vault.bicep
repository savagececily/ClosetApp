@description('Name of the Key Vault')
param keyVaultName string

@description('Location for the Key Vault')
param location string = resourceGroup().location

@description('Tags to apply to the resource')
param tags object = {}

@description('Principal ID to grant access to Key Vault')
param principalId string

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: null
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

// Grant current principal Key Vault Secrets Officer role (for initial setup)
resource keyVaultSecretsOfficerRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (!empty(principalId)) {
  name: guid(keyVault.id, principalId, 'KeyVaultSecretsOfficer')
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7') // Key Vault Secrets Officer
    principalId: principalId
    principalType: 'User'
  }
}

output id string = keyVault.id
output name string = keyVault.name
output endpoint string = keyVault.properties.vaultUri
