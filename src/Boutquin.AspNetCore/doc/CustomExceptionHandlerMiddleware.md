# CustomExceptionHandlerMiddleware

**Namespace:** `Boutquin.AspNetCore`

ASP.NET Core middleware that translates unhandled exceptions into RFC 7807
`ProblemDetails` with the `application/problem+json` content type.

## Register it

Place the middleware before endpoints and other components whose exceptions it should
handle.

```csharp
var app = builder.Build();
app.UseCustomExceptionHandler();
app.MapEndpoints();
```

## Exception mapping

| Exception | Response |
|---|---|
| `ValidationException` | 400, title `Validation Error`, detail `One or more validation errors occurred.`, and `extensions["errors"]` grouped by property name (a null property name uses the empty key). |
| `DomainException` | Its `StatusCode` and `Title`. For 4xx errors the exception message is the detail; for 5xx errors the detail is a generic server-error message. |
| Any other exception | Generic 500 response; the exception is logged when an `ILogger<CustomExceptionHandlerMiddleware>` is available. |

An `InternalServerErrorException` is also logged when a logger is available. The
middleware clears an unstarted response before writing the problem response. It
rethrows an exception when the response has already started, and it lets an
`OperationCanceledException` caused by `HttpContext.RequestAborted` propagate.
