# ValidationException

**Namespace:** `Boutquin.Validation.Exceptions`
**Inherits:** `Exception`

`ValidationException` represents FluentValidation failures at an application boundary.

```csharp
var result = await validator.ValidateAsync(command, cancellationToken);
if (!result.IsValid)
    throw new ValidationException(result.Errors);
```

Its constructor rejects a null sequence with `ArgumentNullException` and materializes
the sequence exactly once. `Errors` is an `IReadOnlyList<ValidationFailure>`, so it
can be enumerated repeatedly without observing later mutation of the caller's source.
`Message` is the individual failure messages joined with `Environment.NewLine`.

When `Boutquin.AspNetCore`'s `CustomExceptionHandlerMiddleware` handles this
exception, it produces a 400 problem response with title `Validation Error`, detail
`One or more validation errors occurred.`, and an `errors` extension grouped by
`ValidationFailure.PropertyName`. Model-level failures with a null property name are
grouped under the empty-string key.
