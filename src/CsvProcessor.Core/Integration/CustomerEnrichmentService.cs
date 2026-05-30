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
        ArgumentOutOfRangeException.ThrowIfLessThan(maxConcurrency, 1);
        using var gate = new SemaphoreSlim(maxConcurrency);

        var tasks = customers.Select(async customer =>
        {
            await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var risk = await riskClient.GetRiskAsync(idSelector(customer), cancellationToken).ConfigureAwait(false);
                return new EnrichedCustomer<TCustomer>(customer, risk, null);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                return new EnrichedCustomer<TCustomer>(customer, null, exception.Message);
            }
            finally
            {
                gate.Release();
            }
        });

        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
