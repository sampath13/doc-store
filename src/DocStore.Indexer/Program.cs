using DocStore.Analyzer.Net.Search;
using DocStore.Analyzer.Net.Services.Queue;
using DocStore.Data;
using DocStore.Indexer;
using Microsoft.Extensions.Options;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton<AppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ISearchClient, SearchClient>();
builder.Services.AddSerilog()
    .AddPostgresDb(builder.Configuration);

var host = builder.Build();
host.Run();