# Preparation bank for .NET data-processing assessments

## Seven-day plan

| Day | Focus | Outcome |
| --- | --- | --- |
| 1 | C# 12 syntax, records, nullable references, collections | Implement DTOs and row mappers without looking up syntax |
| 2 | CSV parsing, normalization, validation, dedupe | Solve C1, C4, C5, and C7 variants under 30 minutes |
| 3 | LINQ, grouping, deferred execution, deterministic ordering | Explain `IEnumerable<T>` vs `IQueryable<T>` and avoid repeated enumeration |
| 4 | Minimal APIs, model validation, error payloads | Build a small POST ingestion endpoint from scratch |
| 5 | Async, cancellation, `Task.WhenAll`, bounded fan-out | Implement enrichment with partial failures and cancellation propagation |
| 6 | Background services, channels, idempotency, watermarks | Build a retry-safe ingestion worker with a bounded queue |
| 7 | Mock assessment | Rebuild the core CSV processor from a blank file, then add tests |

## High-probability coding drills

1. Normalize CSV rows into a canonical DTO.
2. Quarantine invalid rows while continuing valid-row processing.
3. Deduplicate by business key with a deterministic tie-break rule.
4. Produce data-quality metrics for invalid values, blanks, duplicates, and total counts.
5. Compare expected and observed schemas and classify drift.
6. Merge a full snapshot with a delta feed.
7. Apply a watermark for incremental loading and protect replay behavior.
8. Implement idempotency by batch ID or request ID.
9. Fan out HTTP calls with bounded concurrency and aggregate results.
10. Add a REST endpoint that accepts CSV content and returns a processing summary.

## MCQ topics to memorize

- Nullable reference type annotations communicate intent at compile time; they do not add runtime null checks by themselves.
- Records have value-based equality by default and are useful for immutable DTO-like values.
- LINQ queries are usually deferred until enumerated; materialize once when the source is expensive or mutable.
- `IEnumerable<T>` executes in memory; `IQueryable<T>` builds provider-translated expressions.
- Async is about non-blocking waiting; it is not automatically CPU parallelism.
- `Task.WhenAll` waits for all tasks and surfaces failures after completion.
- `CancellationToken` should be accepted by async APIs and propagated to downstream calls.
- Hosted services are singleton-like; create scopes when you need scoped services.
- `IHttpClientFactory` centralizes configuration and handler lifetime management.
- `[ApiController]` improves binding and validation behavior for controller-based APIs.

## Visual mental model

```text
CSV bytes/string
    |
    v
headers -----> schema drift report
    |
    v
raw rows -> normalize -> validate/map -> valid rows + invalid quarantine
                                      |
                                      v
                                dedupe by key
                                      |
                                      v
                              watermark filter
                                      |
                                      v
accepted records + duplicate report + quality summary + next watermark
```
