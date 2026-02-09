using FluentValidation;
using FluentValidation.AspNetCore;
using GoogleLogin.Models;
using GoogleLogin.Repository;
using GoogleLogin.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

builder.Configuration.AddJsonFile("serilogsettings.json", optional: false, reloadOnChange: true);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Configuration.AddEnvironmentVariables();

// ✅ FIX: Configure BOTH Cookie and JWT in ONE call
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
    options.SlidingExpiration = true;
    options.LoginPath = "/Login"; 
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/login";
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Client_ID"];
    options.ClientSecret = builder.Configuration["Client_Secret"];
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
        ValidAudience = builder.Configuration["Jwt:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            Log.Warning(
                "401 Unauthorized. Path: {Path}, Error: {Error}, Description: {Description}",
                context.Request.Path,
                context.Error,
                context.ErrorDescription
            );

            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            Log.Error(
                context.Exception,
                "JWT authentication failed. Path: {Path}",
                context.Request.Path
            );

            return Task.CompletedTask;
        },

        OnForbidden = context =>
        {
            Log.Warning(
                "403 Forbidden. Path: {Path}",
                context.Request.Path
            );

            return Task.CompletedTask;
        }
    };
});

// Add services to the container.
builder.Services.AddDbContext<AppdbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add Session Support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<EncryptionService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => new
                    {
                        error = e.Exception?.Data["ErrorCode"]?.ToString() ??
                                context.ModelState[kvp.Key]?.ValidationState.ToString() ??
                                "validation error",
                        message = e.ErrorMessage
                    }).ToArray()
                );

            var response = new
            {
                title = "One or more validation errors occurred.",
                status = 400,
                errors = errors
            };

            return new BadRequestObjectResult(response);
        };
    });

var Client_Id = builder.Configuration["Client_ID"];
var Client_Secret = builder.Configuration["Client_SECRET"];

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


try
{
    Log.Information("Web server started");
    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseSession();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application Failed to startup");
}
finally
{
    Log.CloseAndFlush();
}