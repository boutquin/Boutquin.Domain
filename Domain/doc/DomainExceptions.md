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
// Parameterless — uses default Exception message
throw new NotFoundException();

// With message
throw new NotFoundException("Order 42 was not found.");

// With message and inner exception
throw new NotFoundException("Order 42 was not found.", originalException);
```

## Non-HTTP Exceptions

These exceptions are not mapped to HTTP status codes — they represent domain-level data validation errors.

| Exception Class | Default Message |
|----------------|-----------------|
| `EmptyOrNullArrayException` | `ExceptionMessages.EmptyOrNullArray` |
| `EmptyOrNullCollectionException` | (built-in default) |
| `EmptyOrNullDictionaryException` | (built-in default) |
| `InsufficientDataException` | (no default — message required) |

All non-HTTP exceptions extend `Exception` directly and follow the same three-constructor pattern (parameterless, message, message + inner), except `InsufficientDataException` which requires a message.

## ExceptionMessages

Static class containing string constants for default exception messages:

- `EmptyOrNullArray` — "Input array must not be empty or null."
- `InsufficientDataForSampleCalculation` — "Input array must have at least two elements for sample calculation."

## Integration with Middleware

These exceptions are caught by `CustomExceptionHandlerMiddleware`, which maps them to RFC 7807 ProblemDetails responses. See [CustomExceptionHandlerMiddleware](../../AspNetCore/doc/CustomExceptionHandlerMiddleware.md).
