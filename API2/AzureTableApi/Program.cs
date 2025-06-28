using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Dodaj Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient",
        policy => policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddSingleton<TableStorageService>();

var app = builder.Build();

// Włącz Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularDevClient");

app.MapGet("/vehicles", async ([FromServices] TableStorageService service) =>
{
    return Results.Ok(service.GetAllVehicles());
})
.WithName("GetVehicles")
.WithTags("Vehicles");

app.MapPost("/vehicles", async ([FromServices] TableStorageService service, [FromBody] VehicleEntity vehicle) =>
{
    await service.AddVehicleAsync(vehicle);
    return Results.Created($"/vehicles/{vehicle.RowKey}", vehicle);
})
.WithName("AddVehicle")
.WithTags("Vehicles");

app.MapPut("/vehicles/{rowKey}", async ([FromServices] TableStorageService service, string rowKey, [FromBody] VehicleEntity vehicle) =>
{
    if (rowKey != vehicle.RowKey)
        return Results.BadRequest("RowKey mismatch");

    await service.UpdateVehicleAsync(vehicle);
    return Results.NoContent();
})
.WithName("UpdateVehicle")
.WithTags("Vehicles");

app.Run();
