$rgrp ="rgStorageMountDemo"
$loc ="eastus"

# Create a resource group
az group create --name $rgrp --location $loc

# deploy the template
az deployment group create --resource-group $rgrp --template-file storageMountDemo.json


# final cleanup
az group delete --name $rgrp --yes