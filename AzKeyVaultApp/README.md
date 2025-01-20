
### Build and push the images to Azure Container registry
```bash
> chmod +x build-and-push.sh
> ./build-and-push.sh
> az acr list --output table

```
## Azure container registry CLI operation:

```bash
# list all repositories
az acr repository list --name maranacr --output table

# list repositories with details
az acr repository list --name maranacr --output json

# list tags for a specific repository
az acr repository show-tags --name maranacr --repository userregistration --output table

# list repository list with their sizes and last updated times
az acr repository list --name maranacr \
   --query "[].[name, lastUpdatedTime]" \
   --output table
