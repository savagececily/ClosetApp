@description('Name of the Azure OpenAI account')
param accountName string

@description('Location for the Azure OpenAI account')
param location string = resourceGroup().location

@description('Tags to apply to the resource')
param tags object = {}

@description('SKU of the Azure OpenAI account')
param sku string = 'S0'

resource openAiAccount 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: accountName
  location: location
  tags: tags
  kind: 'OpenAI'
  sku: {
    name: sku
  }
  properties: {
    customSubDomainName: accountName
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
}

// Deploy GPT-4o-mini model (includes both text and vision capabilities, cost-effective)
resource gpt4oMiniDeployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  parent: openAiAccount
  name: 'gpt-4o-mini'
  sku: {
    name: 'Standard'
    capacity: 10
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4o-mini'
      version: '2024-07-18'
    }
  }
}

output id string = openAiAccount.id
output accountName string = openAiAccount.name
output endpoint string = openAiAccount.properties.endpoint
