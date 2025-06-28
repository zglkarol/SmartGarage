using Azure;
using Azure.Data.Tables;

public class DeviceEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "Devices"; // możesz zmodyfikować
    public required string RowKey { get; set; } // np. GUID lub ID
    public required string Name { get; set; }
    public required string Type { get; set; }

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
