# CSV Processor Guided PoC

This repository is intentionally **not a finished product**. It is a study scaffold for consolidating C#/.NET and data-processing knowledge through a realistic CSV-processing pipeline.

> Note: the working tree available to the agent did not contain the referenced open PR or any remote branches. The implementation below recreates the likely ideas of a full CSV processor as an intentionally incomplete guide: comments, mock implementations, and deliberately naive code point to what should be completed.

## Learning objectives

Use this project to practice:

- C# project structure, records, async streams, dependency boundaries, and cancellation tokens.
- CSV-specific concerns: delimiters, quoted fields, escaped quotes, headers, schema mapping, culture-aware parsing, and malformed rows.
- Data-processing concerns: streaming vs buffering, back pressure, batching, validation, quarantine files, observability, and idempotent outputs.
- Testing strategy: parser golden cases, large-file behavior, invalid encodings, and pipeline integration tests.

## Suggested exercises

1. Replace the naive CSV parser with a correct RFC-4180-aware parser or wrap a production library.
2. Implement schema inference and column mapping in `ValidationStage`.
3. Make transformations deterministic and culture-aware.
4. Add quarantine output for failed rows without stopping the entire pipeline.
5. Add tests for quoted fields, escaped quotes, embedded newlines, missing headers, and large files.
6. Add structured logging and metrics for throughput, bad-row count, and latency.

## Current behavior

The console app accepts an input path and an output path:

```bash
dotnet run --project src/CsvProcessor -- samples/input.csv samples/output.csv
```

The current implementation is intentionally incomplete and contains TODO comments where a learner should implement the missing production behavior.
