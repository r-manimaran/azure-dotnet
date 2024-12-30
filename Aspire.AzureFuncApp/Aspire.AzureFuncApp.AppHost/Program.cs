using Aspire.Hosting.Azure;
using Azure.Provisioning;
using Azure.Provisioning.Storage;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("mystorage").RunAsEmulator()
                .ConfigureInfrastructure(infrastructure =>
                {
                    //Can do Role assignment instead of doing in Azure

                    var principalTypeParameter = new ProvisioningParameter(
                        AzureBicepResource.KnownParameters.PrincipalType, typeof(string) );
                    var principalIdParameter = new ProvisioningParameter(
                        AzureBicepResource.KnownParameters.PrincipalId, typeof(string) );

                    var storageAccount = infrastructure.GetProvisionableResources()
                        .OfType<StorageAccount>()
                        .FirstOrDefault(r => r.BicepIdentifier == "mystorage") ?? throw new InvalidOperationException
                        ("Could not find target storage Account.");

                    infrastructure.Add(storageAccount.CreateRoleAssignment(
                        StorageBuiltInRole.StorageBlobDataOwner, principalTypeParameter, principalIdParameter));
                    infrastructure.Add(storageAccount.CreateRoleAssignment(
                        StorageBuiltInRole.StorageAccountContributor, principalTypeParameter, principalIdParameter));

                });
var blobs = storage.AddBlobs("blobs");

builder.AddAzureFunctionsProject<Projects.AspireImageResizer>("aspireimageresizer")
        .WithReference(blobs)
        .WaitFor(blobs)
        .WithHostStorage(storage)
        .WithExternalHttpEndpoints();

builder.Build().Run();
