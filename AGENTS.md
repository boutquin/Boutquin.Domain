# Boutquin.Domain

> **Language conventions:** `~/.claude/conventions/dotnet-conventions.md`

.NET 10 / C# 14 domain-driven design library. Three NuGet packages published from one solution.

## Solution Structure

| Project | Package | Description |
|---------|---------|-------------|
| `src/Boutquin.Domain/` | `Boutquin.Domain` | Entity base, Result pattern, Guard clauses, strongly typed IDs, value objects (`DateRange`, `Money`/`Currency`), domain exceptions, JSON converters, extensions |
| `src/Boutquin.AspNetCore/` | `Boutquin.AspNetCore` | RFC 7807 exception middleware, modular app registration (`IModule`) |
| `src/Boutquin.Validation/` | `Boutquin.Validation` | FluentValidation integration, `ValidationException` |
| `tests/Boutquin.UnitTests/` | — | xUnit 2.9.3 + FluentAssertions 8.8.0 + coverlet 8.0.0 |

## Key Design Decisions

- **Entity equality** — Identity-based (not structural). `Entity<TEntityId>` uses `Id` for `Equals`/`GetHashCode` and requires the same concrete runtime type. Distinct transient entities (default Id) compare unequal; a transient entity equals only itself by reference.
- **Result pattern** — `Result<TValue>` with implicit conversions. No exceptions for expected failures. `Error` is a value object with `Code`/`Description`.
- **Money/Currency** — `Money` is an immutable `readonly record struct` (amount + ISO 4217 `Currency`). Constructed values require a defined currency other than `Currency.Unspecified`; `default(Money)` remains structurally comparable, but currency-dependent operations on it throw. Structural equality never throws across currencies; `+`/`-`/ordering between differing currencies throw `CurrencyMismatchException` (currency mixing is a programming error, not a `Result` failure). `decimal`-scaling and rounding preserve the currency. Ported/modernized from the legacy `Boutquin.Core` archive.
- **Guard clauses** — Two API families: `CallerArgumentExpression` (C# 10+, auto-captures param name) and Expression-based (legacy). `Guard` is public API (no `InternalsVisibleTo` scoping in Boutquin.Domain) — both families are ordinary public static methods any consumer can call directly.
- **Exception middleware** — Maps `ValidationException` → 400, `DomainException` hierarchy → appropriate HTTP status, unknown → 500. Uses RFC 7807 `ProblemDetails`.
- **Module system** — `IModule.RegisterModule()` with `Func<Assembly>` injection for testability (avoids untestable `Assembly.GetEntryAssembly()`).
- **DateOnly dictionary keys** — .NET 10 supports these keys natively. The custom `JsonConverterFactory` writes ISO 8601 keys and rejects null dictionary values on read. Factory-only registration works; keys use the registered DateOnly converter's `ReadAsPropertyName` contract.
- **XML-doc quality bar** — public-API XML documentation follows the repository's documentation style guide (`documentation-style.md`, a dev-team planning doc kept out of the shipped package). The guide is calibrated against the `Boutquin.Storage` reference standard, adapted to this library's DDD building-block domain. Read it before editing public XML docs.

## Build & Test

```bash
dotnet build
dotnet test --collect:"XPlat Code Coverage"
```

## Versioning, Package Gates & Publishing

### Version policy

- MinVer reads `v*` git tags. The release workflow is `.github/workflows/publish.yml`; it publishes only after building, testing, formatting, packing once into a fresh directory, and passing `tools/Boutquin.PackageVerification`.
- Pre-1.0 assembly compatibility follows the minor line: every `0.9.x` package has `AssemblyVersion=0.9.0.0`, `0.10.x` has `0.10.0.0`, and so on. At 1.0 and later it follows the major line (`1.x → 1.0.0.0`). `FileVersion` remains the full `{major}.{minor}.{patch}.0`.
- Internal package dependencies are exact candidate ranges. `Boutquin.Validation` pins `Boutquin.Domain`; `Boutquin.AspNetCore` pins both `Boutquin.Domain` and `Boutquin.Validation`. The verifier rejects a bare minimum-version dependency.
- Use `MinVerVersionOverride` for a local or release-candidate version. It keeps MinVer active, so PackageVersion, FileVersion, InformationalVersion, and the committed AssemblyVersion policy remain coherent. `MinVerSkip=true` disables MinVer entirely and also makes `MinVerVersionOverride` inert; reserve it for genuinely non-versioned projects. If a special build must skip MinVer, provide the complete replacement set (`Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion`).
- Do not pass an explicit `InformationalVersion`. SourceLink appends the full repository SHA; supplying a value that already contains the SHA can duplicate it. The verifier requires exactly `<PackageVersion>+<full-SHA>`.
- The NuGet API key is the GitHub secret `NUGET_API_KEY` (local copy in `CLAUDE.local.md`, gitignored).

### Safe local-package and consumer gate

Every attempt gets a fresh prerelease version, feed, build cache, consumer cache, and hash manifest. Never reuse a version or feed directory, and never clear the shared global package cache or a shared local feed. From a clean committed checkout:

```bash
set -euo pipefail
test -z "$(git status --porcelain --untracked-files=all)"
VERSION="1.0.0-rc.1.gate.$(date -u +%Y%m%d%H%M%S).1"
GATE_ROOT="$(mktemp -d /tmp/boutquin-domain-gate.XXXXXX)"
FEED="$GATE_ROOT/feed"
BUILD_CACHE="$GATE_ROOT/build-packages"
CONSUMER_CACHE="$GATE_ROOT/consumer-packages"
HASHES="$FEED/package-hashes.json"
mkdir -p "$FEED" "$BUILD_CACHE" "$CONSUMER_CACHE"
test -z "$(find "$FEED" -mindepth 1 -print -quit)"

NUGET_PACKAGES="$BUILD_CACHE" dotnet restore Boutquin.Domain.slnx --packages "$BUILD_CACHE"
NUGET_PACKAGES="$BUILD_CACHE" dotnet build Boutquin.Domain.slnx \
  --configuration Release --no-restore --no-incremental \
  -p:GeneratePackageOnBuild=false -p:MinVerVersionOverride="$VERSION"

for project in \
  src/Boutquin.Domain/Boutquin.Domain.csproj \
  src/Boutquin.Validation/Boutquin.Validation.csproj \
  src/Boutquin.AspNetCore/Boutquin.AspNetCore.csproj
do
  NUGET_PACKAGES="$BUILD_CACHE" dotnet pack "$project" \
    --configuration Release --no-build --no-restore \
    -p:MinVerVersionOverride="$VERSION" --output "$FEED"
done

COMMIT="$(git rev-parse HEAD)"
dotnet run --project tools/Boutquin.PackageVerification --configuration Release --no-build -- \
  verify --spec eng/package-verification.json --artifacts "$FEED" \
  --version "$VERSION" --commit "$COMMIT"
dotnet run --project tools/Boutquin.PackageVerification --configuration Release --no-build -- \
  record-hashes --spec eng/package-verification.json --artifacts "$FEED" \
  --version "$VERSION" --commit "$COMMIT" --output "$HASHES"
dotnet run --project tools/Boutquin.PackageVerification --configuration Release --no-build -- \
  verify --spec eng/package-verification.json --artifacts "$FEED" \
  --version "$VERSION" --commit "$COMMIT" --hashes "$HASHES"
```

`--no-incremental` is load-bearing: changing only the MinVer override and then running `pack --no-build` can otherwise put a new nuspec around an old DLL. The verifier reads the DLL from each nupkg and checks PackageVersion, AssemblyVersion, FileVersion, InformationalVersion, repository commit, exact internal dependencies, the package-specific README, matching snupkg/PDB, and—when supplied—the recorded SHA-256 hashes.

For a central-package-managed consumer, leave its checkout untouched. Create a temporary `Directory.Packages.props` that imports the real one and overrides only this candidate, then add the fresh feed with `RestoreAdditionalProjectSources`:

```bash
CONSUMER_ROOT=/absolute/path/to/consumer
CONSUMER_SOLUTION="$CONSUMER_ROOT/Consumer.slnx"
CONSUMER_PROPS="$GATE_ROOT/Directory.Packages.props"

cat > "$CONSUMER_PROPS" <<EOF
<Project>
  <Import Project="$CONSUMER_ROOT/Directory.Packages.props" />
  <ItemGroup>
    <PackageVersion Update="Boutquin.Domain" Version="[$VERSION]" />
    <PackageVersion Update="Boutquin.Validation" Version="[$VERSION]" />
    <PackageVersion Update="Boutquin.AspNetCore" Version="[$VERSION]" />
  </ItemGroup>
</Project>
EOF

NUGET_PACKAGES="$CONSUMER_CACHE" dotnet restore "$CONSUMER_SOLUTION" \
  "/p:DirectoryPackagesPropsPath=$CONSUMER_PROPS" \
  "/p:RestoreAdditionalProjectSources=$FEED"
NUGET_PACKAGES="$CONSUMER_CACHE" dotnet build "$CONSUMER_SOLUTION" \
  --configuration Release --no-restore \
  "/p:DirectoryPackagesPropsPath=$CONSUMER_PROPS"
```

Never use `dotnet restore --source "$FEED"` for this gate: `--source` replaces the consumer's configured sources, while `RestoreAdditionalProjectSources` augments them. Before accepting the consumer gate, inspect its generated `project.assets.json` and prove that every referenced `Boutquin.Domain`, `Boutquin.Validation`, and `Boutquin.AspNetCore` library resolves to exactly `$VERSION` from the isolated cache; a successful restore alone is insufficient evidence.

```bash
jq -r '.libraries | keys[] | select(startswith("Boutquin."))' \
  "$CONSUMER_ROOT/path/to/obj/project.assets.json"
jq -r '.packageFolders | keys[]' \
  "$CONSUMER_ROOT/path/to/obj/project.assets.json"
```

The first output must show only the candidate version for every referenced Boutquin package; the second must show `$CONSUMER_CACHE`. Finish with the consumer's relevant tests and a real entry-point run when it has one.

Once downstream testing begins, the candidate bytes and hash manifest are immutable. Do not clean/rebuild/repack that version and do not re-record hashes. If anything must change, increment the version and start with a new feed. The only permitted post-consumer check is the read-only `verify ... --hashes "$HASHES"` command against the original artifacts.

## Branch

- Default branch: `main`
- CI triggers: `main` in `pr-verify.yml`; `publish.yml` triggers on `v*` tags.

## Agent-file mirror discipline (`CLAUDE.md` ↔ `AGENTS.md`)

This file is the **Codex-readable mirror of `CLAUDE.md`**. `CLAUDE.md` is
canonical; `AGENTS.md` is derived. Full rule, never-rewrite table, and precedents
live in `~/Developer/AGENTS.md` → *Agent-file mirror discipline*.

- **Exactly one class of difference is permitted:** where the text names the agent
  instruction file the reader should open (`CLAUDE.md` → `AGENTS.md`). Nothing above
  does, so apart from this section's opening sentence the mirror is byte-identical.
  **Every filesystem path and real filename stays byte-identical** — the artifacts
  live under `~/.claude/` no matter which agent reads them, and `CLAUDE.local.md`
  is a real gitignored file, not an agent-file reference.
- **Never regenerate the mirror with a `claude`→`Codex` find/replace.** It broke both
  of this repo's references before the 2026-08-06 repair:
  `~/.claude/conventions/dotnet-conventions.md` (there is no `~/.Codex/conventions/`)
  and `CLAUDE.local.md` (there is no `Codex.local.md`).
- **Drift is the quieter failure.** A change that lands in `CLAUDE.md` alone leaves
  Codex reading stale instructions.
- **Verify before committing** — the diff must be empty but for this section's
  opening:
  ```bash
  diff CLAUDE.md AGENTS.md
  grep -n '~/\.Codex\|Codex\.local' AGENTS.md   # hits outside this section = mangled
  ```
- **Commit both in the same commit** (per-file pathspec). A commit that touches one
  and not the other *is* the drift.
