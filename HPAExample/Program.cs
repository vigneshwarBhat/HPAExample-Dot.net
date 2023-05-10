using HPAExample;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//IDisposable collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Build a resource configuration action to set service information.
Action<ResourceBuilder> configureResource = r => r.AddService(
    serviceName: builder.Configuration.GetValue<string>("ServiceName"),
    serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown",
    serviceInstanceId: Environment.MachineName);
builder.Host
    .ConfigureLogging((_, loggingBuilder) => loggingBuilder.ClearProviders())
    .UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddSingleton<Instrumentation>();
//builder.Services.AddHttpClient("test").UseHttpClientMetrics();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(configureResource)
    .WithMetrics(opts => opts
        .AddMeter(Instrumentation.MeterName)
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter());

builder.Services.AddHttpClient("test");
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy
            .SetIsOriginAllowed((host) => true)
            .AllowAnyMethod()
        .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
//app.UseHttpMetrics(options =>
//{
//    options.AddCustomLabel("appName", _ => "HPAExample");
//});
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
//app.UseMetricServer();
app.Run();