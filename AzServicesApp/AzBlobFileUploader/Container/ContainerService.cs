namespace AzBlobFileUploader.Container;

public class ContainerService : IContainerService
{
    private readonly ILogger<ContainerService> _logger;

    public ContainerService(ILogger<ContainerService> logger)
    {
        _logger = logger;
    }
}
