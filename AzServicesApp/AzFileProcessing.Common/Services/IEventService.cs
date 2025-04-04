using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzFileProcessing.Common.Services;

public interface IEventService
{
    Task SaveEvent<T>(T @event) where T : Event;
    Task<IEnumerable<Event>> GetEvents(string processReferenceId);
    Task IngestEvent(Guid eventId);
    Task<IEnumerable<Event>> GetUnProcessedEvents(string processReferenceId);
}
