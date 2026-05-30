using CsvProcessor.Core.Processing;

// TODO(API-01): Move service registration into extension methods once dependencies grow.
// TODO(API-02): Add request-size limits, content-type validation, and ProblemDetails responses.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(CsvCustomerBatchProcessor.CreateDefault());

var app = builder.Build();

app.MapPost("/ingestions/customers", async (HttpRequest request, CsvCustomerBatchProcessor processor, CancellationToken cancellationToken) =>
{
    // TODO(API-03): Validate Content-Type and stream large files instead of buffering the whole body.

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
