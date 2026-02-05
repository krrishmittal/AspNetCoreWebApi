using Microsoft.EntityFrameworkCore;
using Serilog;
using ValidationForDataModels.Models;
using ValidationForDataModels.Service;
using ValidationForDataModels.Repository;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .AddJsonFile("serilogsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();
// Add services to the container.

Log.Logger=new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddDbContext<AppdbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("default")));

// Register UserService and UserRepo
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddAutoMapper(typeof(Program));

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add Controllers with custom validation error response
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var messages = context.ModelState
                .SelectMany(message => message.Value.Errors)
                .Select(error => error.ErrorMessage)
                .ToList();

            var errorMessage = string.Join("\n", messages);
            return new BadRequestObjectResult(errorMessage);
        };
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



try
{
    Log.Information("Web server started");
    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "Application Failder to startup");
}
finally
{
    Log.CloseAndFlush();
}
