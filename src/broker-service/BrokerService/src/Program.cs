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
builder.Services.AddBrokerServiceDependencyGroup();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    });

builder.Services.AddCors(services =>
    services.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "EasyTradeBrokerService" });
    var xmlDocumentation = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlDocumentation));
});

builder.Services.AddDbContext<BrokerDbContext>(options =>
{
    var connString = builder.Configuration[Constants.MySqlConnectionString];
    options.UseMySql(connString, ServerVersion.AutoDetect(connString));
});

builder.Logging.ClearProviders();
builder.Logging.AddCustomLogger(options =>
{
    options.SkipString = "EasyTrade.BrokerService.";
    options.MinimumMessageLength = 100;
});

builder.Services.AddBrokerServiceDependencyGroup();
builder.Services.AddScoped<ITradeRepository, TradeRepository>();
builder.Services.AddTransient<HighCpuUsageMiddleware>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    var proxyPrefix = builder.Configuration[Constants.ProxyPrefix] ?? string.Empty;

    app.UseSwagger(options =>
    {
        if (!string.IsNullOrEmpty(proxyPrefix))
        {
            options.PreSerializeFilters.Add((swagger, request) =>
            {
                var url = $"{request.Scheme}://{request.Host}/{proxyPrefix}";
                swagger.Servers = new[] { new OpenApiServer { Url = url } };
            });
        }
    });

    app.UseSwaggerUI(options =>
    {
        var swaggerJsonPath = string.IsNullOrEmpty(proxyPrefix)
            ? "/swagger/v1/swagger.json"
            : $"/{proxyPrefix}/swagger/v1/swagger.json";

        options.SwaggerEndpoint(swaggerJsonPath, "EasyTradeBrokerService v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<HighCpuUsageMiddleware>();
app.MapControllers();
app.Run();
