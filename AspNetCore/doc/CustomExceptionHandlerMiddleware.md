# Class Name: CustomExceptionHandlerMiddleware

**Namespace:** `Boutquin.AspNetCore`

ASP.NET Core middleware that catches unhandled exceptions and maps them to RFC 7807 ProblemDetails responses.

## Design Rationale

- **Middleware over exception filters:** Filters only catch exceptions within the MVC pipeline. Middleware wraps the entire request pipeline — routing, authentication, other middleware, minimal APIs, and MVC — providing complete coverage.
- **RFC 7807 ProblemDetails:** Provides a standard error envelope (`type`, `title`, `status`, `detail`, `instance`) with the `application/problem+json` content type, enabling generic error handling on the client side.
- **Self-describing exceptions:** `DomainException` subclasses carry their own `StatusCode` and `Title`, so the middleware doesn't need a growing switch statement.

## Constructor

### `CustomExceptionHandlerMiddleware(RequestDelegate next)`

**Parameters:**
- `next` (RequestDelegate): The next middleware in the pipeline.

**Exceptions:** `ArgumentNullException` when `next` is null.

## Methods

### `Task InvokeAsync(HttpContext context)`

Invokes the next middleware in a try/catch. On exception, maps the exception to a ProblemDetails response.

**Exception Mapping:**

| Exception Type | Status Code | Behavior |
|---------------|-------------|----------|
| `ValidationException` | 400 | Includes grouped validation errors in `extensions["errors"]` |
| `DomainException` | (from exception) | Uses the exception's `StatusCode` and `Title` |
| Any other `Exception` | 500 | Generic "An unexpected error occurred." (prevents information disclosure) |

## Registration Extension

### `CustomExceptionHandlerMiddlewareExtensions`

**Namespace:** `Boutquin.AspNetCore`

```csharp
public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
```

Registers the middleware in the pipeline.

**Exceptions:** `ArgumentNullException` when `app` is null.

## Usage

```csharp
// In Program.cs or Startup.Configure:
if (!env.IsDevelopment())
{
    app.UseCustomExceptionHandler();
}
```

## Response Example

```json
{
  "type": null,
  "title": "Not Found",
  "status": 404,
  "detail": "Order 42 was not found.",
  "instance": "/api/orders/42"
}
```

## See Also

- [DomainExceptions](../../Domain/doc/DomainExceptions.md) — The exception hierarchy this middleware consumes.
