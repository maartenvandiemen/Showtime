param location string
param keyVaultName string
param applicationname string

param applicationInsightsConnectionString string

var appServiceAppName = 'site-${applicationname}-${uniqueString(resourceGroup().id)}'
var appServicePlanName = 'sitePlan-${applicationname}-${uniqueString(resourceGroup().id)}'

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'S1'
  }
  properties:{
    reserved: true
  }
  kind: 'windows'
}


resource appServiceApp 'Microsoft.Web/sites@2023-01-01' = {
  name: appServiceAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
     acrUseManagedIdentityCreds: false
     linuxFxVersion: 'DOTNETCORE|8.0'
     connectionStrings: [
      { 
        name: 'Database'
        connectionString: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=ConnectionString)'
        type: 'SQLAzure'
      }
     ]
     appSettings: [
      {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsightsConnectionString
      }]
    }
  }  
  identity: {
    type: 'SystemAssigned'
 }
}

output principalId string = appServiceApp.identity.principalId
output webAppName string = appServiceApp.name
