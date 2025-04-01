using System.Reflection;
using EasyTrade.BrokerService;
using EasyTrade.BrokerService.Helpers;
using EasyTrade.BrokerService.Helpers.Logging;
using EasyTrade.BrokerService.ProblemPatterns.HighCpuUsage;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using EasyTrade.BrokerService.Entities.Trades.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());  // Add the custom DateTimeConverter here
    });

// Default CORS policy allowing every connection
builder.Services.AddCors(services =>
    services.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "EasyTradeBrokerService" });
    // Turn on generating Swagger documentation from comments
    var xmlDocumentation = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlDocumentation));
});

// Connect to MySQL database using Pomelo.EntityFrameworkCore.MySql
builder.Services.AddDbContext<BrokerDbContext>(options =>
    options.UseMySql(builder.Configuration[Constants.MySqlConnectionString], 
        ServerVersion.AutoDetect(builder.Configuration[Constants.MySqlConnectionString])) // Auto detect MySQL server version
        // .EnableSensitiveDataLogging() // Enables logging of sensitive data for debugging (e.g., SQL queries)
        // .LogTo(Console.WriteLine, LogLevel.Information) // Logs SQL queries to the console (or another logging provider)
);

// Clear default logging providers and add custom ones
builder.Logging.ClearProviders();
builder.Logging.AddCustomLogger(options =>
{
    options.SkipString = "EasyTrade.BrokerService.";
    options.MinimumMessageLength = 100;
});

// Add dependency injection group for services
builder.Services.AddBrokerServiceDependencyGroup();
builder.Services.AddScoped<ITradeRepository, TradeRepository>();
builder.Services.AddTransient<HighCpuUsageMiddleware>();

// Add HTTP client used to connect to other services
builder.Services.AddHttpClient();

// Create the app instance
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Add and configure Swagger
    var proxyPrefix = builder.Configuration[Constants.ProxyPrefix] ?? string.Empty;
    app.UseSwagger(options =>
    {
        if (!string.IsNullOrEmpty(proxyPrefix))
        {
            options.PreSerializeFilters.Add(
                (swagger, request) =>
                {
                    var url = $"{request.Scheme}://{request.Host}/{proxyPrefix}";
                    swagger.Servers = new[] { new OpenApiServer { Url = url } }; // Ensure this line is properly forming the servers array
                }
            );
        }
    });
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyTradeBrokerService v1") // Ensure the correct endpoint path
    );
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<HighCpuUsageMiddleware>();

app.MapControllers();

app.Run();
