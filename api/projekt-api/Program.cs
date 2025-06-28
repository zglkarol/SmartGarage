using Microsoft.EntityFrameworkCore;
using projekt_api.Data;

var builder = WebApplication.CreateBuilder(args);

// Dodanie DB contextu
// builder.Services.AddDbContext<ApiContext>(options =>
//     options.UseInMemoryDatabase("DevicesDb"));

var app = builder.Build();
builder.Services.AddSingleton<TableStorageService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowAngularFrontend");

// app.UseAuthorization();

app.MapControllers();

app.Run();
