
namespace FileProcessor;

public class FileProcessorJob : BackgroundService
{
    private readonly ILogger<FileProcessorJob> _logging;

    public FileProcessorJob(ILogger<FileProcessorJob> logging)
    {
        _logging = logging;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
       
        while(!stoppingToken.IsCancellationRequested)
        {

        }
    }
}
