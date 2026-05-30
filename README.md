# CSV Processor PoC for C#/.NET Data Challenges

This repository is a learning-oriented .NET 8 / C# 12 proof of concept for common assessment tasks around CSV ingestion, low-quality data, validation, deduplication, data-quality reporting, APIs, background workers, and async integration.

The code intentionally separates concepts into small classes so you can study or copy individual patterns during timed practice.

## Project structure

| Path | Purpose | Assessment topic |
| --- | --- | --- |
| `src/CsvProcessor.Core` | CSV parsing, schema drift detection, row mapping, validation, dedupe, watermarking, idempotency, quality metrics, async enrichment | Single-file and library-style coding challenges |
| `src/CsvProcessor.Console` | File input entry point that prints a JSON processing report | HackerRank-style stdin/file transformation practice |
| `src/CsvProcessor.Api` | Minimal API endpoint for CSV ingestion | REST API, validation, error surface |
| `src/CsvProcessor.Worker` | Bounded channel and `BackgroundService` ingestion worker | background processing, cancellation, idempotency |
| `tests/CsvProcessor.Tests` | xUnit examples for invalid rows, duplicates, and schema drift | automated test scoring |
| `samples/messy-customers.csv` | Small dirty input file to practice with | data-quality scenarios |
| `docs/preparation-bank.md` | Study guide and practice bank based on your prompt | interview preparation |

## Requirements

Use .NET 8 and C# 12. The project is pinned through `Directory.Build.props` with nullable references, implicit usings, latest analysis, and warnings as errors.

## Run locally

```bash
dotnet restore
dotnet test
dotnet run --project src/CsvProcessor.Console -- samples/messy-customers.csv
```

Start the API:

```bash
dotnet run --project src/CsvProcessor.Api
curl -X POST http://localhost:5000/ingestions/customers --data-binary @samples/messy-customers.csv
```

## Core CSV challenge flow

1. Read headers and data rows with a lightweight CSV reader that supports quoted fields.
2. Normalize headers and raw values.
3. Detect schema drift before mapping data.
4. Map raw rows into canonical `CustomerRecord` values.
5. Quarantine invalid rows instead of failing the whole batch.
6. Deduplicate by business key (`email`) using latest `updated_at`, then highest balance, then lowest source row.
7. Apply an optional watermark for incremental processing.
8. Return accepted records, invalid rows, duplicate report, quality metrics, and next watermark.

## Practice prompts implemented in code

- C1: Normalize messy legacy CSV rows into canonical DTOs.
- C4: Deduplicate by business key and latest timestamp.
- C5: Compute data-quality summary per batch.
- C6: Detect missing and unexpected columns.
- C7: Continue processing valid rows while quarantining invalid rows.
- C8: Watermark-based incremental processing.
- C9: In-memory idempotency guard.
- C10: Bounded async fan-out enrichment.
- C11: Background worker with bounded queue and graceful cancellation.
- C13: POST endpoint for ingestion.

## Tips for timed assessments

- Write the expected input and output shape first.
- Normalize values before validation, but preserve source rows for error reporting.
- Always define the dedupe tie-break rule explicitly.
- Prefer deterministic ordering in outputs and tests.
- For partial-success tasks, return both accepted data and rejected-row details.
- For async tasks, propagate `CancellationToken` and set a clear concurrency limit.
