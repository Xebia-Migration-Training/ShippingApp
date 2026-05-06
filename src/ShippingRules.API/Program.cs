using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Serilog;
using ShippingRules.API.Middleware;
using ShippingRules.Application.Interfaces;
using ShippingRules.Application.Services;
using ShippingRules.Infrastructure.Data;
using ShippingRules.Infrastructure.Repositories;
using System.Reflection;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/shipping-rules-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Database configuration - Using In-Memory for demo
    builder.Services.AddDbContext<ShippingRulesDbContext>(options =>
        options.UseInMemoryDatabase("ShippingRulesDb"));

    // Register MediatR with validation pipeline behavior
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
        Assembly.Load("ShippingRules.Application")));
    builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ShippingRules.Application.Behaviors.ValidationBehavior<,>));

    // Register FluentValidation
    builder.Services.AddValidatorsFromAssembly(Assembly.Load("ShippingRules.Application"));

    // Register Repositories
    builder.Services.AddScoped<IShippingRuleRepository, ShippingRuleRepository>();
    builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();

    // Register Services
    builder.Services.AddScoped<RulePrecedenceService>();
    builder.Services.AddScoped<ExchangeRateService>();

    // Add CORS - restrict origins in production
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowConfigured", policy =>
        {
            if (allowedOrigins is { Length: > 0 })
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
            else
            {
                // Fallback for development only
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
        });
    });

    var app = builder.Build();

    // Global exception handling - must be first in pipeline
    app.UseMiddleware<GlobalExceptionHandler>();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Shipping Rules API V1");
            options.RoutePrefix = string.Empty; // Swagger at root
        });
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    app.UseCors("AllowConfigured");
    app.UseAuthorization();
    app.MapControllers();

    // Initialize database
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ShippingRulesDbContext>();
        dbContext.Database.EnsureCreated();
        Log.Information("Database initialized");
    }

    Log.Information("Starting Shipping Rules API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    Log.CloseAndFlush();
}
