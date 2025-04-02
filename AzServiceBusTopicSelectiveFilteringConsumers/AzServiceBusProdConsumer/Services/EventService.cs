using AzServiceBusProdConsumer.Models;

namespace AzServiceBusProdConsumer.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<EventService> _logger;

    public EventService(AppDbContext dbContext, ILogger<EventService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task CreateEvent(Event eventItem)
    {
        _logger.LogInformation("Adding the new Event Item of type:{EventType}", eventItem.Type);
        _dbContext.Events.Add(eventItem);
        await _dbContext.SaveChangesAsync();
    }

    public Task UpdateEvent(Event updateEventItem)
    {
        throw new NotImplementedException();
    }
}
