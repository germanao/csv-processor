# CSV Processor Exercises

## Parsing

- Add tests proving that `NaiveCsvReader` fails for quoted commas and embedded newlines.
- Replace `string.Split(',')` with a parser that tracks quote state.
- Decide where to report malformed rows: parser diagnostics, validator errors, or both.

## Validation

- Add a row-width validation error.
- Decide how nullable optional fields should interact with format validators.
- Replace stringly typed error codes with constants or an enum.

## Transformation

- Create a transformation abstraction such as `ICsvTransformationRule`.
- Add culture-aware decimal normalization.
- Add transformations that preserve raw values for auditability.

## Writing

- Implement RFC 4180-style escaping.
- Stream records instead of buffering all accepted rows.
- Add a rejected-row output file.

## Production hardening

- Add structured logging.
- Add cancellation tests.
- Add metrics for throughput and error rates.
- Benchmark large files and decide when batching is useful.
