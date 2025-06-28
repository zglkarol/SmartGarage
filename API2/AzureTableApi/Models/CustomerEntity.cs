using Azure;
using Azure.Data.Tables;

public class CustomerEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "Customers";
    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // Custom properties
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
