using AzServiceBusProdConsumer.Models;

namespace AzServiceBusProdConsumer.Services
{
    public interface IEventService
    {
        Task CreateEvent(Event eventItem);
        Task UpdateEvent(Event updateEventItem);
    }
}
