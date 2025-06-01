
using DocStore.Analyzer.Net.Search;
using DocStore.Analyzer.Net.Services.Queue;
using DocStore.Analyzer.Net.Services.Search;
using DocStore.Data;
using Microsoft.Extensions.Options;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services
    .AddSerilog()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMessageProducer()
    .AddTransient<IndexCheck>()
    .AddScoped<ISearchClient, SearchClient>()
    .Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"))
    .AddSingleton<AppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value)
    .AddMediatR(configuration =>
    {
        configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
        configuration.RegisterServicesFromAssembly(typeof(ISearchClient).Assembly);
    })
    .AddCors(options => options.AddPolicy("AllowAll", policy => policy.AllowAnyHeader().AllowAnyMethod()))
    .AddPostgresDb(builder.Configuration);

var app = builder.Build();

app.UseCors(x =>
    x.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());

app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    Log.Information("Checking index");
    var indexCheck = scope.ServiceProvider.GetService<IndexCheck>()!;
    indexCheck.CreateIfNotExistsAsync(CancellationToken.None).GetAwaiter().GetResult();
    Log.Information("index check completed");
}

app.Run();
