# Class Name: ValidationException

**Namespace:** `Boutquin.Validation.Exceptions`
**Inherits:** `Exception`

Exception thrown when FluentValidation validation fails. Wraps the collection of `ValidationFailure` objects and builds a concatenated error message.

## Properties

### `IEnumerable<ValidationFailure> Errors { get; }`

The collection of validation failures, each containing:
- `PropertyName` — The name of the property that failed validation.
- `ErrorMessage` — The human-readable error message.

## Constructor

### `ValidationException(IEnumerable<ValidationFailure> failures)`

Creates a new `ValidationException` from a collection of validation failures. The `Message` property is automatically built by concatenating all error messages with newlines.

**Parameters:**
- `failures` (IEnumerable&lt;ValidationFailure&gt;): The validation failures.

**Exceptions:** `ArgumentNullException` when `failures` is null.

## Integration with Middleware

When caught by `CustomExceptionHandlerMiddleware`, the errors are grouped by `PropertyName` and surfaced in the ProblemDetails `extensions["errors"]` dictionary. This allows API clients to map errors back to specific form fields.

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Name must not be empty.\nEmail must be valid.",
  "instance": "/api/users",
  "errors": {
    "Name": ["Name must not be empty."],
    "Email": ["Email must be valid."]
  }
}
```

## Usage

```csharp
var validator = new UserValidator();
var result = validator.Validate(user);
if (!result.IsValid)
{
    throw new ValidationException(result.Errors);
}
```

## See Also

- [CustomExceptionHandlerMiddleware](../../AspNetCore/doc/CustomExceptionHandlerMiddleware.md) — Catches this exception and produces ProblemDetails responses.
