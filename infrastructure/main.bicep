// targetScope = 'subscription' // If deploying at sub level, but we will deploy at RG level

param location string = resourceGroup().location
param acrName string = 'acrsoftcloud${uniqueString(resourceGroup().id)}'

// Create an Azure Container Registry (Basic tier is the cheapest)
resource acr 'Microsoft.ContainerRegistry/registries@2023-01-01-preview' = {
  name: acrName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true // We enable this just for easy learning/testing, though Managed Identity is better for prod
  }
}

// Output the login server so we can use it in docker commands
output acrLoginServer string = acr.properties.loginServer
output acrName string = acr.name
