using Microsoft.EntityFrameworkCore;
using RepairWorkshop.API.Extensions;
using RepairWorkShop.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddProblemDetails();

builder.Services.AddAuthorization();

builder.Services
    .AddApplicationServices()
    .AddDatabase()
    .AddExceptions()
    .AddSwagger()
    .AddAuth()
    .AddPermission();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "app.db");

    options.UseSqlite($"Data Source={path}");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
