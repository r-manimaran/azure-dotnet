namespace AzFileProcessing.Common;

public class Event
{
    public Guid Id { get; set; }
    public string ProcessReferenceId { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessStartTime { get; set; }
    public DateTime? ProcessEndTime { get; set; }
}
