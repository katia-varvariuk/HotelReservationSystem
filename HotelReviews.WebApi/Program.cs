using FluentValidation;
using HotelReviews.Application.Behaviors;
using HotelReviews.Application.Mappings;
using HotelReviews.Infrastructure;
using HotelReviews.WebApi.Middleware;
using MediatR;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "HotelReviews.WebApi")
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        "logs/hotelreviews-.log",
        rollingInterval: RollingInterval.Day,
        encoding: System.Text.Encoding.UTF8,  
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddControllers();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        Assembly.Load("HotelReviews.Application"));
});

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
builder.Services.AddValidatorsFromAssembly(
    Assembly.Load("HotelReviews.Application"),
    includeInternalTypes: false);
builder.Services.AddAutoMapper(
    Assembly.Load("HotelReviews.Application"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Hotel Reviews API",
        Version = "v1",
        Description = "Clean Architecture API for managing hotel reviews and requests",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Hotel Reviews Team",
            Email = "support@hotelreviews.com"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddHealthChecks()
    .AddCheck("MongoDB", () =>
    {
        try
        {
            var context = builder.Services.BuildServiceProvider()
                .GetRequiredService<HotelReviews.Infrastructure.Persistence.MongoDbContext>();

            var isConnected = context.CheckConnectionAsync().GetAwaiter().GetResult();

            return isConnected
                ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("MongoDB connected")
                : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("MongoDB not connected");
        }
        catch (Exception ex)
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy(
                "MongoDB connection error", ex);
        }
    });
var app = builder.Build();
try
{
    Log.Information("Initializing MongoDB...");
    await app.Services.InitializeMongoDbAsync();
    Log.Information("MongoDB initialized successfully");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to initialize MongoDB");
    throw;
}
app.UseGlobalExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Reviews API v1");
        options.RoutePrefix = string.Empty; 
    });
}
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseSerilogRequestLogging();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
try
{
    Log.Information("Starting Hotel Reviews API...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}