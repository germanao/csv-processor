# CSV Processor Guided PoC for C#/.NET Data Challenges

This branch intentionally contains an **incomplete learning version** of a CSV processor. It keeps the architecture and vocabulary of the full implementation, but many methods are stubs, mocks, or deliberately naive implementations with `TODO` comments.

Use this repository as a reconstruction exercise: read the tests, run the sample inputs, follow the comments, and complete each concept one at a time until the project behaves like a production-ready CSV ingestion pipeline.

## Learning objective

By completing the TODOs, you should be able to explain and implement common .NET data-processing concepts:

- CSV parsing, including quoted values and escaped quotes.
- Header normalization and schema drift detection.
- Row normalization, validation, and quarantine of bad records.
- Deterministic deduplication by business key.
- Data-quality summaries and invalid-reason metrics.
- Watermark-based incremental processing.
- Idempotency guards for batch processing.
- Minimal API ingestion endpoints.
- Background workers, channels, cancellation, and bounded async fan-out.

## Important branch note

This branch is a **PoC / guide branch**, not the complete implementation. Some code is intentionally wrong or incomplete. The tests are left as executable requirements so you can use failing assertions as a study checklist.

## Project structure

| Path | Purpose | What you should complete |
| --- | --- | --- |
| `src/CsvProcessor.Core/Csv` | CSV parsing | Replace the naive comma splitter with a parser that understands quoted fields and record boundaries. |
| `src/CsvProcessor.Core/Validation` | Schema drift and row mapping | Implement normalization, required fields, type parsing, status mapping, and rename hints. |
| `src/CsvProcessor.Core/Processing` | Batch orchestration, dedupe, watermarking, idempotency | Implement the real processing order, deterministic duplicate winner rules, watermark filtering, and replay protection. |
| `src/CsvProcessor.Core/Quality` | Data-quality metrics | Count accepted, invalid, duplicate, blank, and invalid-reason metrics correctly. |
| `src/CsvProcessor.Console` | File input entry point | Add argument validation, error handling, and useful exit codes. |
| `src/CsvProcessor.Api` | Minimal API endpoint | Add request validation, problem details, cancellation, and service registration patterns. |
| `src/CsvProcessor.Worker` | Background ingestion worker | Add retry/error handling, idempotency semantics, graceful cancellation, and persistence boundaries. |
| `tests/CsvProcessor.Tests` | xUnit requirements | Treat these tests as the first acceptance criteria to make pass. |
| `samples/messy-customers.csv` | Dirty input sample | Use this to test normalization and quarantine behavior. |
| `docs/preparation-bank.md` | Study guide | Use the practice bank to rebuild features from scratch. |

## Suggested exercise flow

1. Run the tests and observe the failures.
2. Complete `CsvReader` until it parses the sample file correctly.
3. Complete `SchemaDriftDetector` before mapping rows.
4. Complete `CustomerRowMapper` and make invalid-row quarantine deterministic.
5. Complete `CustomerDedupeService` with the documented tie-break rule.
6. Complete `WatermarkProcessor` and `DataQualityService`.
7. Wire API/worker edge cases after the core library is reliable.

## Commands

```bash
dotnet restore
dotnet test
dotnet run --project src/CsvProcessor.Console -- samples/messy-customers.csv
```

## Core CSV challenge flow to rebuild

```text
CSV text
  -> read header and rows
  -> detect schema drift
  -> normalize and validate rows
  -> quarantine invalid rows
  -> deduplicate valid rows by email
  -> filter by previous watermark
  -> calculate next watermark
  -> return accepted records, invalid rows, duplicate report, and quality summary
```

## Completion rule of thumb

A feature is not finished until you can answer these questions:

- What invalid input does this code reject?
- What valid-but-messy input does it normalize?
- Is the output deterministic?
- Does it preserve enough source context for debugging?
- Does it behave safely when the same batch is submitted twice?
