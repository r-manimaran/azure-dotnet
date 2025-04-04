using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzFileProcessing.Common.Services;

public class EventService : IEventService
{
    private readonly ILogger<EventService> _logger;

    public EventService(ILogger<EventService> logger)
    {
        _logger = logger;
    }
    public Task<IEnumerable<Event>> GetEvents(string processReferenceId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Event>> GetUnProcessedEvents(string processReferenceId)
    {
        throw new NotImplementedException();
    }

    public Task IngestEvent(Guid eventId)
    {
        _logger.LogInformation("Ingested for Event :{ Event}",JsonSerializer.Serialize(eventId));

        return Task.CompletedTask;
       
    }

    public Task SaveEvent<T>(T @event) where T : Event
    {
        _logger.LogInformation("Saving Event: {Event}",JsonSerializer.Serialize(@event));

        return Task.CompletedTask;
    }
}
