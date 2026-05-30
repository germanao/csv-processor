# CSV Processor PoC / Guided Learning Project

This repository intentionally contains an **incomplete** CSV processing implementation.
It is designed as a study scaffold for C#/.NET data processing concepts rather than a production-ready processor.

The original full pull request was not available in this local checkout: the repository only contained the initial `.gitkeep` commit and no remotes or additional branches. This scaffold therefore captures the typical ideas expected in a CSV processor PR:

- CLI entry point and options parsing.
- Streaming-oriented CSV reading.
- Schema definition and validation.
- Row transformation.
- Error collection and processing statistics.
- CSV writing.
- Clear TODO/FIXME comments where you should complete or correct the design.

## Learning objectives

Use this project to practice:

1. Designing a .NET console application.
2. Modeling data processing stages as small services.
3. Understanding CSV edge cases: quoted fields, escaped quotes, delimiters, headers, newlines, encodings, empty values, and malformed rows.
4. Separating parsing, validation, transformation, and output concerns.
5. Building reliable diagnostics around partial failures.
6. Refactoring mocks into tested production implementations.

## Suggested completion path

1. Make the project compile with your installed .NET SDK.
2. Replace `NaiveCsvReader` with a quote-aware parser or an integration with a mature CSV library.
3. Make `CsvValidator` enforce all declared schema rules.
4. Fix `CsvTransformer` so it applies configured mappings rather than hard-coded behavior.
5. Implement `CsvWriter` escaping correctly.
6. Add unit tests for parsing, validation, transformation, and error reporting.
7. Add integration tests using `samples/input.csv`.

## Example command

```bash
dotnet run --project src/CsvProcessor.Poc -- --input samples/input.csv --output out/normalized.csv
```

The command above is intentionally aspirational until you complete the TODOs.
