using Azure.Data.Tables;

public class TableStorageService
{
    private readonly TableClient _tableClient;

    public TableStorageService(IConfiguration config)
    {
        var connectionString = config["AzureStorage:ConnectionString"];
        var tableName = config["AzureStorage:TableName"] ?? "Devices";

        _tableClient = new TableClient(connectionString, tableName);
        _tableClient.CreateIfNotExists();
    }

    public List<DeviceEntity> GetDevices()
    {
        return _tableClient.Query<DeviceEntity>().ToList();
    }

    public async Task AddDeviceAsync(DeviceEntity device)
    {
        await _tableClient.AddEntityAsync(device);
    }

    public async Task DeleteDeviceAsync(string rowKey)
    {
        await _tableClient.DeleteEntityAsync("Devices", rowKey);
    }
}
