$res_group ="rgContSecretDemo"
$loc = "eastus"
$environment="contenv"

# creating resource group
az group create --name $res_group --location $loc

# creating container environment
az containerapp env create --name $environment --resource-group $res_group --internal-only false --location $loc

# creating the container app
az containerapp create `
 --name secrets-demo-app `
 --resource-group rgContSecretDemo `
 --environment contenv1 `
 --image rmanimaran/containerenvapp:latest `
 --target-port 8080 `
 --ingress 'external' `
 --env-vars ASPNETCORE_ENVIRONMENT=Production ASPNETCORE_HTTP_PORTS=8080 ASPNETCORE_URLS=http://+:8080 `
 --secrets key1=12345 key2='AnotherSecret'



 # updating container app with new revision after chaning the value in the environment variable
 az containerapp revision `
  --name secrets-demo-app `
  --resource-group $res_group `
  --revision secrets-demo-app--v2

  az group delete --resource-group $res_group --yes
