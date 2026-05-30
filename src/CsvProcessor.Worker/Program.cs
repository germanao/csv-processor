using CsvProcessor.Core.Processing;
using CsvProcessor.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton(CsvCustomerBatchProcessor.CreateDefault());
builder.Services.AddSingleton<IIdempotencyStore, InMemoryIdempotencyStore>();
builder.Services.AddSingleton<IngestionQueue>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
