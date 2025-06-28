using Azure;
using Azure.Data.Tables;

public class TableStorageService
{
    private readonly TableClient _tableClient;

    public TableStorageService(IConfiguration config)
    {
        string connectionString = config["AzureTable:ConnectionString"];
        string tableName = config["AzureTable:TableName"];
        _tableClient = new TableClient(connectionString, tableName);
        _tableClient.CreateIfNotExists();
    }

    public IEnumerable<VehicleEntity> GetAllVehicles()
    {
        return _tableClient.Query<VehicleEntity>().ToList();
    }

    public async Task AddVehicleAsync(VehicleEntity vehicle)
    {
        await _tableClient.AddEntityAsync(vehicle);
    }

    public async Task UpdateVehicleAsync(VehicleEntity vehicle)
    {
        vehicle.ETag = ETag.All;
    await _tableClient.UpdateEntityAsync(vehicle, vehicle.ETag, TableUpdateMode.Replace);
    }

}
