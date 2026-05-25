using RepairWorkshop.API.Extensions;
using RepairWorkshop.API.Services;
using RepairWorkShop.DAL.Interceptors;
using RepairWorkShop.DAL.Interfaces;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<AuditableInterceptor>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
