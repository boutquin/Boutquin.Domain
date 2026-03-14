# Boutquin.Domain

.NET 10 / C# 14 domain-driven design library. Three NuGet packages published from one solution.

## Solution Structure

| Project | Package | Description |
|---------|---------|-------------|
| `Domain/` | `Boutquin.Domain` | Entity base, Result pattern, Guard clauses, strongly typed IDs, domain exceptions, JSON converters, extensions |
| `AspNetCore/` | `Boutquin.AspNetCore` | RFC 7807 exception middleware, modular app registration (`IModule`) |
| `Validation/` | `Boutquin.Validation` | FluentValidation integration, `ValidationException` |
| `UnitTests/` | — | xUnit 2.9.3 + FluentAssertions 8.8.0 + coverlet 8.0.0 |

## Key Design Decisions

- **Entity equality** — Identity-based (not structural). `Entity<TEntityId>` uses `Id` for `Equals`/`GetHashCode`. Transient entities (default Id) throw on comparison.
- **Result pattern** — `Result<TValue>` with implicit conversions. No exceptions for expected failures. `Error` is a value object with `Code`/`Description`.
- **Guard clauses** — Two API families: `CallerArgumentExpression` (C# 10+, auto-captures param name) and Expression-based (legacy). Both are internal with `InternalsVisibleTo` for tests.
- **Exception middleware** — Maps `ValidationException` → 400, `DomainException` hierarchy → appropriate HTTP status, unknown → 500. Uses RFC 7807 `ProblemDetails`.
- **Module system** — `IModule.RegisterModule()` with `Func<Assembly>` injection for testability (avoids untestable `Assembly.GetEntryAssembly()`).
- **DateOnly dictionary keys** — Custom `JsonConverterFactory` because `System.Text.Json` cannot serialize non-string dictionary keys. ISO 8601 format.

## Build & Test

```bash
dotnet build
dotnet test --collect:"XPlat Code Coverage"
```

## Versioning & Publishing

- MinVer reads git tags: `git tag v0.6.0-beta.2 && git push --tags`
- GitHub Actions workflow `.github/workflows/nuget-publish.yml` auto-publishes on `v*` tag push.
- NuGet API key: GitHub secret `NUGET_API_KEY` (local copy in `CLAUDE.local.md`, gitignored).

## Branch

- Default branch: `main`
- CI triggers: `main` (in both `pr-verify.yml` and `nuget-publish.yml`)
