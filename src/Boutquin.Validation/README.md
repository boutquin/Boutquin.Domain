# Boutquin.Validation

FluentValidation integration for Boutquin.Domain. The package provides
`ValidationException`, which carries structured `ValidationFailure` instances for
application and HTTP-boundary handling.

## Install

```bash
dotnet add package Boutquin.Validation
```

## Use validation failures

```csharp
using Boutquin.Validation.Exceptions;

var validationResult = await validator.ValidateAsync(command, cancellationToken);
if (!validationResult.IsValid)
    throw new ValidationException(validationResult.Errors);
```

`ValidationException.Errors` is an `IReadOnlyList<ValidationFailure>` materialized
once by the constructor. Its message is the validation error messages joined with
new lines. For an RFC 7807 HTTP response, add `Boutquin.AspNetCore`; its exception
middleware maps this exception to a 400 response and groups errors by property name.

## Documentation

See the [ValidationException guide](https://github.com/boutquin/Boutquin.Domain/blob/main/src/Boutquin.Validation/doc/ValidationException.md).

## License

Apache-2.0. See the [repository license](https://github.com/boutquin/Boutquin.Domain/blob/main/LICENSE.txt).
