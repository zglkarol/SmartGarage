using Azure;
using Azure.Data.Tables;

public class VehicleEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "Vehicles";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Owner { get; set; }
    public bool Status { get; set; }
    public bool IsCar { get; set; }
}
