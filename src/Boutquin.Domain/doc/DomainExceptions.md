# Domain Exceptions

**Namespace:** `Boutquin.Domain.Exceptions`

The exception hierarchy provides typed domain exceptions that carry their own HTTP status code and title. This self-describing pattern allows middleware to map any `DomainException` to a ProblemDetails response without a per-type switch statement.

## Abstract Base Class

### `DomainException : Exception`

Base class for all HTTP-mapped domain exceptions.

**Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `StatusCode` | `int` | The HTTP status code (e.g., 404, 500). |
| `Title` | `string` | The human-readable title (e.g., "Not Found"). |

**Constructors:**
- `DomainException(int statusCode, string title)`
- `DomainException(int statusCode, string title, string message)`
- `DomainException(int statusCode, string title, string message, Exception inner)`

## HTTP Exception Types

All HTTP exception types follow the same constructor pattern: parameterless, message-only, and message + inner exception.

| Exception Class | Status Code | Title |
|----------------|-------------|-------|
| `BadRequestException` | 400 | Bad Request |
| `UnauthorizedException` | 401 | Unauthorized |
| `ForbiddenException` | 403 | Forbidden |
| `NotFoundException` | 404 | Not Found |
| `ConflictException` | 409 | Conflict |
| `UnsupportedMediaTypeException` | 415 | Unsupported Media Type |
| `UnprocessableEntityException` | 422 | Unprocessable Entity |
| `TooManyRequestsException` | 429 | Too Many Requests |
| `InternalServerErrorException` | 500 | Internal Server Error |
| `ServiceUnavailableException` | 503 | Service Unavailable |

### Constructor Pattern (all types)

```csharp
// Parameterless — uses the exception type's safe default message
throw new NotFoundException();

// With message
throw new NotFoundException("Order 42 was not found.");

// With message and inner exception
throw new NotFoundException("Order 42 was not found.", originalException);
```

## Non-HTTP Exceptions

These exceptions are not `DomainException` subclasses and are not mapped by the HTTP middleware. They represent argument and data-validation failures.

| Exception Class | Default Message |
|----------------|-----------------|
| `EmptyOrNullArrayException` | `ExceptionMessages.EmptyOrNullArray` |
| `EmptyOrNullCollectionException` | `ExceptionMessages.EmptyOrNullCollection` |
| `EmptyOrNullDictionaryException` | `ExceptionMessages.EmptyOrNullDictionary` |
| `InsufficientDataException` | (no default — message required) |
| `CurrencyMismatchException` | (no default — currency constructor reports the mismatch) |

`EmptyOrNullArrayException`, `EmptyOrNullCollectionException`, and
`EmptyOrNullDictionaryException` derive from `ArgumentException`. `InsufficientDataException`
and `CurrencyMismatchException` derive directly from `Exception`; the former requires a
message, while the latter also has a `(Currency expected, Currency actual)` constructor
and exposes those values as properties.

## ExceptionMessages

Static class containing string constants for default exception messages:

- `EmptyOrNullArray` — "Input array must not be empty or null."
- `EmptyOrNullCollection` — "Input collection must not be empty or null."
- `EmptyOrNullDictionary` — "Input dictionary must not be empty or null."
- `InsufficientDataForSampleCalculation` — "Input array must have at least two elements for sample calculation."

## Integration with Middleware

`CustomExceptionHandlerMiddleware` maps `DomainException` subclasses to their
configured RFC 7807 status and title. The non-HTTP exceptions above do not receive a
special mapping and should normally be handled at the application boundary. See
[CustomExceptionHandlerMiddleware](../../Boutquin.AspNetCore/doc/CustomExceptionHandlerMiddleware.md).
