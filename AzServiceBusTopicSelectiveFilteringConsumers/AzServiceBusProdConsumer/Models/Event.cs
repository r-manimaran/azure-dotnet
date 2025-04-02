using System.Text.Json;

namespace AzServiceBusProdConsumer.Models;

public class Event
{
    public Guid Id { get; set; }
    public Guid UniqueId { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessStartTime { get; set; }
    public DateTime? ProcessEndTime { get; set; }
}
