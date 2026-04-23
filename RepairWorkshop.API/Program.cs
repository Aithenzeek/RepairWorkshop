using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RepairWorkshop.API.Authorization;
using RepairWorkshop.API.Exceptions;
using RepairWorkshop.API.Extensions;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkshop.BLL.Services;
using RepairWorkShop.DAL;
using System.Text;

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
