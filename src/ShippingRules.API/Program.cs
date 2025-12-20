using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Serilog;
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

    // Register MediatR
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
        Assembly.Load("ShippingRules.Application")));

    // Register FluentValidation
    builder.Services.AddValidatorsFromAssembly(Assembly.Load("ShippingRules.Application"));

    // Register Repositories
    builder.Services.AddScoped<IShippingRuleRepository, ShippingRuleRepository>();
    builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();

    // Register Services
    builder.Services.AddScoped<RulePrecedenceService>();
    builder.Services.AddScoped<ExchangeRateService>();

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

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
    app.UseCors("AllowAll");
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
