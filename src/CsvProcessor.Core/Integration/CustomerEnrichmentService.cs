namespace CsvProcessor.Core.Integration;

public interface ICustomerRiskClient
{
    Task<CustomerRisk> GetRiskAsync(string customerId, CancellationToken cancellationToken);
}

public sealed record CustomerRisk(string CustomerId, int Score, string Tier);
public sealed record EnrichedCustomer<TCustomer>(TCustomer Customer, CustomerRisk? Risk, string? Error);

public sealed class CustomerEnrichmentService(ICustomerRiskClient riskClient)
{
    public async Task<IReadOnlyList<EnrichedCustomer<TCustomer>>> EnrichAsync<TCustomer>(
        IReadOnlyList<TCustomer> customers,
        Func<TCustomer, string> idSelector,
        int maxConcurrency,
        CancellationToken cancellationToken)
    {
        // TODO(ASYNC-01): Validate maxConcurrency and use SemaphoreSlim for bounded fan-out.
        // TODO(ASYNC-02): Preserve input order while allowing concurrent requests.
        // TODO(ASYNC-03): Let OperationCanceledException flow but convert per-customer service failures to Error.
        // MOCK/INCOMPLETE: this is sequential and ignores maxConcurrency.
        var enriched = new List<EnrichedCustomer<TCustomer>>();
        foreach (var customer in customers)
        {
            try
            {
                var risk = await riskClient.GetRiskAsync(idSelector(customer), cancellationToken).ConfigureAwait(false);
                enriched.Add(new EnrichedCustomer<TCustomer>(customer, risk, null));
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                enriched.Add(new EnrichedCustomer<TCustomer>(customer, null, exception.Message));
            }
        }

        return enriched;
    }
}
