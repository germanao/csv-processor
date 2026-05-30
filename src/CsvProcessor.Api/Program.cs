using CsvProcessor.Core.Processing;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(CsvCustomerBatchProcessor.CreateDefault());

var app = builder.Build();

app.MapPost("/ingestions/customers", async (HttpRequest request, CsvCustomerBatchProcessor processor, CancellationToken cancellationToken) =>
{
    using var reader = new StreamReader(request.Body);
    var csvText = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
    if (string.IsNullOrWhiteSpace(csvText))
    {
        return Results.BadRequest(new { error = "Request body must contain CSV content." });
    }

    var previousWatermark = request.Query.TryGetValue("previousWatermark", out var values) && DateTimeOffset.TryParse(values.ToString(), out var parsed)
        ? parsed
        : (DateTimeOffset?)null;
    var result = processor.Process(csvText, previousWatermark);
    return Results.Ok(result);
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
