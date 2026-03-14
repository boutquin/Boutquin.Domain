# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- `Boutquin.AspNetCore` library — `CustomExceptionHandlerMiddleware` (RFC 7807 ProblemDetails) and `ModuleExtensions` (modular monolith registration).
- `Boutquin.Validation` library — `ValidationBehavior<,>` (MediatR pipeline) and `ValidationException`.
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
