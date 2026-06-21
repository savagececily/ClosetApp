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

// Deploy GPT-4 model for outfit recommendations and AI analysis
resource gpt4Deployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  parent: openAiAccount
  name: 'gpt-4'
  sku: {
    name: 'Standard'
    capacity: 10
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4'
      version: 'turbo-2024-04-09'
    }
  }
}

// Deploy GPT-4 Vision model for image analysis
resource gpt4VisionDeployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  parent: openAiAccount
  name: 'gpt-4-vision'
  sku: {
    name: 'Standard'
    capacity: 10
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4'
      version: 'vision-preview'
    }
  }
  dependsOn: [
    gpt4Deployment
  ]
}

output id string = openAiAccount.id
output accountName string = openAiAccount.name
output endpoint string = openAiAccount.properties.endpoint
