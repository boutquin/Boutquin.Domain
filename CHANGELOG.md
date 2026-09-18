# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-09-17

> This release includes the work previously recorded as 0.8.0 and 0.9.0 development checkpoints.
> Breaking changes and migration guidance are listed under **Changed**.
> Version 1.0.0 establishes the stable public API baseline for all three packages.

### Added

- Public API compatibility baselines for all three packages, enforced by
  `Microsoft.CodeAnalysis.PublicApiAnalyzers` during normal builds.
- Package-specific NuGet READMEs for `Boutquin.Domain`, `Boutquin.AspNetCore`, and
  `Boutquin.Validation`.
- `Money` operators completing the arithmetic group: `operator -(decimal, Money)` and unary
  `operator -(Money)`, symmetric with the existing `decimal + Money` / `Money - decimal` operators.
- `ResultExtensions.MatchAsync`/`TapAsync` overloads accepting a `Task<Result<T>>` (and, for
  `MatchAsync`, a non-generic `Task<Result>`) source, mirroring the existing `BindAsync` pair — a
  caller holding a `Task`-typed result no longer needs an `.AsTask()`/wrap hop.
- `CustomExceptionHandlerMiddleware` accepts an optional `ILogger<CustomExceptionHandlerMiddleware>`
  constructor parameter (resolved automatically via DI through `UseMiddleware`); unmapped exceptions
  and internal-server faults are logged via `LogError` when a logger is available, and construction
  without one keeps compiling and behaving exactly as before.
- `Money` value object (`Boutquin.Domain.ValueObjects`) — immutable `readonly record struct` with currency-safe arithmetic, ordering, and rounding. Structural equality never throws across currencies; `+`/`-`/ordering between differing currencies throw `CurrencyMismatchException`.
- `Currency` enumeration (`Boutquin.Domain.Enumerations`) — ISO 4217 alphabetic codes backed by their ISO 4217 numeric values, with per-member names exposed via `EnumExtensions.GetDescription`. Reflects the current active register: withdrawn codes (HRK, MRO, STD, VEF, SLL, CUC, ZWL) are excluded and their successors (MRU, STN, VES, SLE, ZWG) included.
- `CurrencyMismatchException` (`Boutquin.Domain.Exceptions`) — thrown by currency-mixing `Money` operations; exposes the `Expected`/`Actual` currencies.
- `CurrencyExtensions.Amount` — fluent `Money` factory (`Currency.USD.Amount(19.99m)`).
- `ErrorType.Cancelled` (499) and `Error.Cancelled(code, description)` — a dedicated category for caller-initiated cancellation, distinct from `RequestTimeout` (408).
- `ResultExtensions.BindAsync` overload accepting a `ValueTask<Result<T>>` source (mirrors `TapAsync`, removing the need for an `.AsTask()` hop).
- `Currency.UYW` (Unidad previsional, ISO 4217 numeric `927`) added to the catalogue.

### Changed

- **Breaking:** `Money` now rejects `Currency.Unspecified` and undefined currency values at
  construction. Its `Amount` and `Currency` properties are get-only so `with` expressions cannot
  bypass that invariant. Currency-dependent operations on `default(Money)` now throw
  `InvalidOperationException`; structural equality and hashing remain non-throwing.
- `CurrencyExtensions.Amount` now rejects undefined currency values as well as
  `Currency.Unspecified`.
- `DecimalArrayExtensions.Variance` and `StandardDeviation` now reject undefined
  `CalculationType` values with `ArgumentOutOfRangeException`.
- The message-less constructors of `BadRequestException`, `UnauthorizedException`,
  `ForbiddenException`, `NotFoundException`, `ConflictException`, `UnsupportedMediaTypeException`,
  `UnprocessableEntityException`, and `TooManyRequestsException` now set a human-readable default
  `Message` (exposed as constants on `ExceptionMessages`) instead of leaving it at the framework
  default. Behavior change: `new NotFoundException().Message` is now
  `"The requested resource was not found."` rather than the framework-generated
  `"Exception of type '…' was thrown."`. Additive to public API (new `ExceptionMessages` constants);
  no signatures changed.
- **Breaking:** renamed `Guard.GuardCondition.With<TException>(params object[] args)` to
  `WithArgs<TException>(params object[] args)`. The former overload's params-only shape could never be
  selected once the `With<TException>(string, params object[])` overload existed — the compiler always
  preferred the more specific string-first overload, so the multi-argument-constructor call pattern its
  own doc example showed silently misrouted. **Migration:** replace
  `.With<TException>(arg1, arg2, ...)` with `.WithArgs<TException>(arg1, arg2, ...)`.
- **Breaking:** `EmptyOrNullArrayException`, `EmptyOrNullCollectionException`, and
  `EmptyOrNullDictionaryException` now derive from `ArgumentException` (previously `Exception`
  directly), so a single `catch (ArgumentException)` now catches every empty/null-container guard
  failure alongside the rest of the `Guard` family's rejections. Message-only constructors are
  unchanged. **Migration:** callers catching `Exception` are unaffected; a caller that relied on these
  types *not* being an `ArgumentException` should narrow its catch accordingly.
- **Breaking:** `Guard.AgainstNullOrDefault` for a `Nullable<T>` now compares the present value against
  the *underlying* type's default, so `Guard.AgainstNullOrDefault(() => (int?)0)` now throws
  `ArgumentException` (previously only a `null` `Nullable<T>` threw; `(int?)0` passed silently).
  **Migration:** a caller relying on `(int?)0` passing this guard must stop doing so — the value is now
  rejected as the documented contract requires.
- `CustomExceptionHandlerMiddleware`: 5xx message suppression now keys on the `StatusCode >= 500`
  *class* rather than the single `InternalServerErrorException` type, so `ServiceUnavailableException`
  (503) and any future 5xx domain exception no longer echo their message to the client; a downstream
  component's buffered-but-not-yet-started response body is now cleared before the ProblemDetails body
  is written; an `OperationCanceledException` raised because the client aborted the request
  (`context.RequestAborted`) is now rethrown instead of converted into a wasted 500 write; a
  `ValidationException.Errors` entry with a `null` `PropertyName` (a model-level FluentValidation
  failure) is now grouped under the empty-string key instead of throwing from the handler itself.
- `Result.Map` now routes a projection that returns `null` through `Result.Create`, producing a failed
  result carrying `Error.NullValue` instead of throwing `ArgumentNullException` — `Map` is now total
  for any well-formed projection.
- `Result.Cancelled`'s human-readable message parameter renamed `name` → `description`, matching the
  sibling `Error` factories' parameter naming.
- The unsorted `DateOnly`-keyed dictionary JSON converter now validates that the payload begins with a
  JSON object-start token, so a bare-scalar payload throws `JsonException` instead of silently
  deserializing to an empty dictionary (matching the sorted-dictionary converter's existing behavior).
- `DateOnlyConverter.Read` now rejects a non-string JSON token with `JsonException` instead of an
  opaque `InvalidOperationException`.
- `EnumExtensions.GetDescription` now keys its reflection lookup on the value's runtime type
  (`enumValue.GetType()`) rather than the generic type parameter, so a `System.Enum`-typed receiver
  resolves a defined member correctly instead of throwing.
- `StringExtensions.IsNullOrEmpty`/`IsNullOrWhiteSpace`/`Compare`/`CompareOrdinal` now accept `string?`
  parameters, matching their actual null-tolerant behavior; `IsNullOrEmpty`/`IsNullOrWhiteSpace` carry
  `[NotNullWhen(false)]` so callers no longer need to suppress CS8604 at the call site.
- `DateTimeExtensions.ConvertTimeZone` XML docs now document the DST spring-forward-gap
  `ArgumentException` and the fall-back ambiguous-hour resolution (standard-time offset), instead of
  leaving the throw undocumented.
- `StronglyTypedId<TValue>.ToString` now formats under `CultureInfo.InvariantCulture` instead of the
  ambient current culture, so the rendered id is stable across threads/locales.
- Repo restructured to the standard `src/` + `tests/` layout; legacy `Boutquin.sln` migrated to `Boutquin.Domain.slnx` (`.slnx`-only convention). No public package IDs changed.
- Project folders under the new `src/` root keep their full package names — `src/Boutquin.Domain/`, `src/Boutquin.AspNetCore/`, `src/Boutquin.Validation/` — alongside `tests/Boutquin.UnitTests/`.
- `StringExtensions.ToUpperCaseFirst`/`ToLowerCaseFirst` now use the invariant culture by default (avoiding the Turkish dotted/dotless-I surprise); added `CultureInfo`-accepting overloads for explicit culture control, plus a `Format(string, IFormatProvider, object[])` overload for locale-stable formatting.
- `Entity<TEntityId>` equality now distinguishes different concrete entity types that share the same id type — `Order(42)` no longer equals `Customer(42)` — matching the documented contract. `TEntityId` is now constrained to `notnull`.
- **Breaking:** renamed `Error.Name` → `Error.Description` (the positional record member and the factory parameters). The property always carried the human-readable description, not a short identifier — the identifier role is played by `Code`.
- **Breaking:** renamed `StringExtensions.ToUppercaseFirst` → `ToUpperCaseFirst` for casing consistency with `ToLowerCaseFirst`.
- **Breaking:** `Result.Cancelled()` now produces an `ErrorType.Cancelled` (499) error instead of `ErrorType.RequestTimeout` (408), so a caller-initiated cancellation is no longer conflated with a server-side timeout.
- **Breaking:** `ValidationException.Errors` is now `IReadOnlyList<ValidationFailure>` (was `IEnumerable<>`); the failures are materialized once at construction and a null `failures` argument now throws `ArgumentNullException`.
- `CustomExceptionHandlerMiddleware`: validation responses now use a short RFC 7807 `title` (`"Validation Error"`) with the sentence moved to `detail`; `InternalServerErrorException` messages are no longer echoed to clients (server-fault detail is generic); serializer options are cached.
- `DateRange` equality now treats all empty ranges as equal (set semantics), with a matching `GetHashCode`.
- `EnumExtensions.GetDescription` now memoizes its reflection result.
- `Money` now defines a commutative `decimal * Money` operator.

### Fixed

- `Guard.AgainstNullOrDefault` now compares nullable structs against their zero-initialized
  underlying default without invoking a parameterless constructor. It rejects a present default
  value and accepts a constructor-initialized non-default value.
- `DateOnlyDictionaryConverterFactory` now round-trips both supported dictionary types when
  registered by itself. Key parsing uses the DateOnly converter's `ReadAsPropertyName` contract;
  a separate `DateOnlyConverter` registration is optional.
- Package verification now compares complete stream blocks before checking README bytes.
  Compressed entries that return short reads no longer fail verification when their content
  is identical to the committed README.
- `Result.Success<TValue>(null)` now throws `ArgumentNullException` instead of producing a "successful" result whose `Value` returns null (which violated the non-null contract); use `Result.Create` for null-tolerant construction.
- `DateOnlyConverter` now reads dates with `TryParseExact` against its canonical `yyyy-MM-dd` format (under `CultureInfo.InvariantCulture`), so JSON round-trips are lossless and a non-canonical input is rejected with a clear `JsonException` rather than silently rewritten.
- The `DateOnly`-keyed dictionary converters now validate that the payload begins with an object-start token and that each entry begins with a JSON property-name token, surfacing malformed input as a clear `JsonException` instead of an opaque `InvalidOperationException`.
- `DateTimeExtensions.ConvertTimeZone` no longer throws when the input's `DateTimeKind` contradicts the source zone (e.g. a `Utc`-kind value with a non-UTC source); the value is interpreted as a wall-clock time in the source zone.
- `JsonElementExtensions.ToObject<T>` now rejects a `JsonValueKind.Undefined` element with a clear `ArgumentException`.
- `CustomExceptionHandlerMiddleware` now re-throws (preserving the original stack trace) when the response has already started, instead of corrupting a committed response with a "headers already sent" failure.
- `Guard.With<TException>(string, params object[])` no longer formats the message (and so no longer risks a `FormatException`) when the guard condition is not met; constructor exceptions raised while building the thrown exception now surface with their original stack trace preserved.
- `ModuleExtensions` module discovery tolerates assemblies that fail to fully load (`ReflectionTypeLoadException`), contributing their loadable types instead of aborting the whole scan.
- `EmptyOrNullCollectionException` and `EmptyOrNullDictionaryException` parameterless constructors now carry a meaningful default message (consistent with `EmptyOrNullArrayException`).
- The synchronous `ResultExtensions.Match` overloads now validate their arguments with `ArgumentNullException`, matching the async/`Map`/`Bind` overloads.
- `ValidationException` now throws `ArgumentNullException` for a null `failures` argument (previously a `NullReferenceException` from the message builder) and enumerates the source sequence exactly once.
- `ModuleExtensions` now skips concrete `IModule` implementations without a public parameterless constructor (previously aborted the whole scan with `MissingMethodException`), and repeated `RegisterModules` calls now accumulate their modules instead of silently dropping all but the last.
- `StringExtensions.ToUpperCaseFirst`/`ToLowerCaseFirst` now case the first Unicode scalar, correctly handling a leading supplementary character (surrogate pair) instead of mangling or ignoring it.
- `Result.Failure(...)` overloads now reject a null `Error` with `ArgumentNullException`; `Entity.RaiseDomainEvent` rejects a null event; `Entity.GetDomainEvents()` returns an immutable snapshot safe to iterate during dispatch.
- `Guard.AgainstEmptyOrNullEnumerable` now names the offending parameter in its message, and `Guard.AgainstOverflow` rejects a null string with `ArgumentException` instead of a `NullReferenceException`. `Guard.AgainstNullOrEmptyArray` was renamed to `Guard.AgainstEmptyOrNullArray` (internal API).
- `CurrencyExtensions.Amount` now rejects `Currency.Unspecified` (the default sentinel, not a real denomination).
- `DateTimeExtensions.ConvertTimeZone` now rejects a blank time-zone id with a clear `ArgumentException` instead of an opaque `TimeZoneNotFoundException`.
- Currency catalogue: corrected `PEN` description casing (“Peruvian sol”) and gave `Unspecified` an explicit description.

### Security

- Closed an information-disclosure edge in `CustomExceptionHandlerMiddleware`: a 4xx
  `DomainException` thrown through its message-less constructor (e.g. `new NotFoundException()`) had
  no caller message, so `Exception.Message` fell back to the .NET framework default —
  `"Exception of type 'Boutquin.Domain.Exceptions.NotFoundException' was thrown."` — which the
  middleware forwarded verbatim into the RFC 7807 `detail`, leaking the internal fully-qualified
  type name to API consumers. The eight client-error (4xx) exception types now carry a safe, generic
  default message on their message-less constructor, so the response body never exposes internal
  type names. Genuine caller-supplied 4xx messages are still forwarded unchanged, and server-side
  (≥500) messages remain suppressed as before.

## [0.7.0] - 2026-03-16

### Added

- `Boutquin.AspNetCore` library — `CustomExceptionHandlerMiddleware` (RFC 7807 ProblemDetails) and `ModuleExtensions` (modular monolith registration).
- `Boutquin.Validation` library — `ValidationException`, which carries structured FluentValidation failures.
- Comprehensive API documentation in `Domain/doc/`, `AspNetCore/doc/`, and `Validation/doc/`.
- GitHub Actions CI workflow (`pr-verify.yml`) with build, test, coverage, and format checks.
- SourceLink and deterministic build support via `Directory.Build.props`.

### Changed

- Migrated from `CommonAssemblyInfo.props` (explicit `<Import>`) to `Directory.Build.props` (MSBuild auto-import convention).
- Renamed project folders: `Boutquin.Validation/` → `Validation/`, `Boutquin.AspNetCore/` → `AspNetCore/`.
- Updated `README.md` with complete solution structure, API reference links, and quick-start examples.
- Upgraded to .NET 10 / C# 14.

### Removed

- Deprecated `PackageIconUrl` property (replaced by `PackageIcon`).
- Explicit `<Import>` directives from all `.csproj` files (no longer needed with `Directory.Build.props`).
