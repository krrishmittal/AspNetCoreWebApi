using LoggingWithSerilog.Mapping;
using LoggingWithSerilog.Models;
using LoggingWithSerilog.Repositories;
using LoggingWithSerilog.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration=new ConfigurationBuilder()
    .AddJsonFile("serilogsettings.json",optional:true)
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentUserName()
    .CreateLogger();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DumContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(EmployeeMapping));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.

try
{
    Log.Information("Web server started");
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI();
        app.UseSwagger();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}