// Copyright (c) 2024-2026 Pierre G. Boutquin. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Diagnostics;
using System.IO.Compression;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;

namespace Boutquin.PackageVerification;

internal static class Program
{
    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public static int Main(string[] args)
    {
        try
        {
            Run(args);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"PACKAGE VERIFICATION FAILED: {exception.Message}");
            return 1;
        }
    }

    private static void Run(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException(Usage);
        }

        var options = CommandOptions.Parse(args[1..]);
        switch (args[0])
        {
            case "verify":
                options.RequireOnly("spec", "artifacts", "version", "commit", "hashes");
                break;

            case "record-hashes":
                options.RequireOnly("spec", "artifacts", "version", "commit", "output");
                break;

            default:
                throw new ArgumentException($"Unknown command '{args[0]}'.{Environment.NewLine}{Usage}");
        }

        var context = LoadContext(options.Required("spec"));
        var artifactsPath = Path.GetFullPath(options.Required("artifacts"));
        var expectedVersion = options.Optional("version");
        var commit = ValidateCommit(options.Required("commit"));
        var artifacts = DiscoverArtifacts(context, artifactsPath, expectedVersion);

        switch (args[0])
        {
            case "verify":
                Verify(context, artifacts, commit, options.Optional("hashes"));
                Console.WriteLine(
                    $"VERIFIED {artifacts.Packages.Count} packages at {artifacts.Version} from {commit}");
                break;

            case "record-hashes":
                RecordHashes(artifacts, commit, options.Required("output"));
                break;

        }
    }

    private static VerificationContext LoadContext(string specPath)
    {
        var absoluteSpecPath = Path.GetFullPath(specPath);
        using var stream = File.OpenRead(absoluteSpecPath);
        var spec = JsonSerializer.Deserialize<VerificationSpec>(stream, s_jsonOptions)
            ?? throw new InvalidDataException($"Could not deserialize '{absoluteSpecPath}'.");

        if (string.IsNullOrWhiteSpace(spec.RepositoryRoot)
            || string.IsNullOrWhiteSpace(spec.RepositoryUrl)
            || string.IsNullOrWhiteSpace(spec.RepositoryType)
            || string.IsNullOrWhiteSpace(spec.TargetFramework)
            || spec.Packages.Count == 0)
        {
            throw new InvalidDataException("The verification spec is missing required values.");
        }

        var packageIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var package in spec.Packages)
        {
            if (string.IsNullOrWhiteSpace(package.Id)
                || string.IsNullOrWhiteSpace(package.Assembly)
                || string.IsNullOrWhiteSpace(package.ReadmeSource)
                || !packageIds.Add(package.Id))
            {
                throw new InvalidDataException("The verification spec contains an invalid or duplicate package.");
            }
        }

        var specDirectory = Path.GetDirectoryName(absoluteSpecPath)
            ?? throw new InvalidDataException($"Cannot resolve the directory for '{absoluteSpecPath}'.");
        var repositoryRoot = Path.GetFullPath(Path.Combine(specDirectory, spec.RepositoryRoot));
        return new VerificationContext(spec, repositoryRoot);
    }

    private static ArtifactSet DiscoverArtifacts(
        VerificationContext context,
        string artifactsPath,
        string? expectedVersion)
    {
        if (!Directory.Exists(artifactsPath))
        {
            throw new DirectoryNotFoundException($"Artifact directory '{artifactsPath}' does not exist.");
        }

        var nupkgs = Directory.GetFiles(artifactsPath, "*.nupkg", SearchOption.TopDirectoryOnly);
        var snupkgs = Directory.GetFiles(artifactsPath, "*.snupkg", SearchOption.TopDirectoryOnly);
        if (nupkgs.Length != context.Spec.Packages.Count || snupkgs.Length != context.Spec.Packages.Count)
        {
            throw new InvalidDataException(
                $"Expected exactly {context.Spec.Packages.Count} nupkg and snupkg files; "
                + $"found {nupkgs.Length} and {snupkgs.Length}.");
        }

        var identities = nupkgs.Select(ReadIdentity).ToList();
        var packages = new List<PackageArtifact>();
        foreach (var packageSpec in context.Spec.Packages)
        {
            var matches = identities
                .Where(identity => string.Equals(identity.Id, packageSpec.Id, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (matches.Count != 1)
            {
                throw new InvalidDataException(
                    $"Expected exactly one nupkg with id '{packageSpec.Id}'; found {matches.Count}.");
            }

            var identity = matches[0];
            var expectedNupkgName = $"{identity.Id}.{identity.Version}.nupkg";
            if (!string.Equals(Path.GetFileName(identity.Path), expectedNupkgName, StringComparison.Ordinal))
            {
                throw new InvalidDataException(
                    $"Package filename '{Path.GetFileName(identity.Path)}' does not match '{expectedNupkgName}'.");
            }

            var expectedSnupkgName = $"{identity.Id}.{identity.Version}.snupkg";
            var snupkgMatches = snupkgs
                .Where(path => string.Equals(Path.GetFileName(path), expectedSnupkgName, StringComparison.Ordinal))
                .ToList();
            if (snupkgMatches.Count != 1)
            {
                throw new InvalidDataException($"Expected exactly one symbol package '{expectedSnupkgName}'.");
            }

            packages.Add(new PackageArtifact(packageSpec, identity.Path, snupkgMatches[0], identity.Version));
        }

        var versions = packages.Select(package => package.Version).Distinct(StringComparer.Ordinal).ToList();
        if (versions.Count != 1)
        {
            throw new InvalidDataException(
                $"All packages must share one version; found {string.Join(", ", versions)}.");
        }

        var version = versions[0];
        if (expectedVersion is not null
            && !string.Equals(version, expectedVersion, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Expected PackageVersion '{expectedVersion}', found '{version}'.");
        }

        return new ArtifactSet(artifactsPath, version, packages);
    }

    private static PackageIdentity ReadIdentity(string nupkgPath)
    {
        EnsureNonEmpty(nupkgPath);
        using var archive = ZipFile.OpenRead(nupkgPath);
        var nuspec = GetSingleEntry(
            archive,
            entry => entry.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase),
            "nuspec");
        var document = LoadXml(nuspec);
        var metadata = GetMetadata(document);
        var id = RequiredElement(metadata, "id").Value;
        var version = RequiredElement(metadata, "version").Value;
        return new PackageIdentity(id, version, nupkgPath);
    }

    private static void Verify(
        VerificationContext context,
        ArtifactSet artifacts,
        string commit,
        string? hashManifestPath)
    {
        var semanticVersion = SemanticVersion.Parse(artifacts.Version);
        foreach (var package in artifacts.Packages)
        {
            VerifyPackage(context, package, artifacts.Version, semanticVersion, commit);
        }

        if (hashManifestPath is not null)
        {
            VerifyHashes(artifacts, commit, hashManifestPath);
        }
    }

    private static void VerifyPackage(
        VerificationContext context,
        PackageArtifact package,
        string version,
        SemanticVersion semanticVersion,
        string commit)
    {
        EnsureNonEmpty(package.NupkgPath);
        EnsureNonEmpty(package.SnupkgPath);

        using var archive = ZipFile.OpenRead(package.NupkgPath);
        var nuspec = GetSingleEntry(
            archive,
            entry => entry.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase),
            "nuspec");
        var document = LoadXml(nuspec);
        var metadata = GetMetadata(document);

        RequireEqualIgnoreCase(package.Spec.Id, RequiredElement(metadata, "id").Value, "package id");
        RequireEqual(version, RequiredElement(metadata, "version").Value, "PackageVersion");

        var repository = RequiredElement(metadata, "repository");
        RequireEqual(context.Spec.RepositoryType, RequiredAttribute(repository, "type"), "repository type");
        RequireEqual(context.Spec.RepositoryUrl, RequiredAttribute(repository, "url"), "repository URL");
        RequireEqualIgnoreCase(commit, RequiredAttribute(repository, "commit"), "repository commit");

        VerifyDependencies(context, package, metadata, version);

        RequireEqual("README.md", RequiredElement(metadata, "readme").Value, "package readme path");
        var readmeEntry = GetSingleEntry(
            archive,
            entry => string.Equals(entry.FullName, "README.md", StringComparison.Ordinal),
            "root README.md");
        var sourceReadmePath = ResolveRepositoryPath(context.RepositoryRoot, package.Spec.ReadmeSource);
        var committedReadme = ReadCommittedFile(
            context.RepositoryRoot,
            commit,
            Path.GetRelativePath(context.RepositoryRoot, sourceReadmePath));
        using (var packageReadme = readmeEntry.Open())
        using (var sourceReadme = new MemoryStream(committedReadme, writable: false))
        {
            if (!StreamsEqual(packageReadme, sourceReadme))
            {
                throw new InvalidDataException(
                    $"{package.Spec.Id} README.md does not match '{package.Spec.ReadmeSource}' at {commit}.");
            }
        }

        var assemblyPath = $"lib/{context.Spec.TargetFramework}/{package.Spec.Assembly}";
        var assemblyEntry = GetSingleEntry(
            archive,
            entry => string.Equals(entry.FullName, assemblyPath, StringComparison.Ordinal),
            assemblyPath);
        var codeViewGuid = VerifyAssembly(
            package.Spec.Id,
            assemblyEntry,
            semanticVersion,
            version,
            commit);

        using var symbolsArchive = ZipFile.OpenRead(package.SnupkgPath);
        var pdbPath = $"lib/{context.Spec.TargetFramework}/{Path.ChangeExtension(package.Spec.Assembly, ".pdb")}";
        var pdbEntry = GetSingleEntry(
            symbolsArchive,
            entry => string.Equals(entry.FullName, pdbPath, StringComparison.Ordinal),
            pdbPath);
        VerifyPortablePdb(package.Spec.Id, pdbEntry, codeViewGuid);
    }

    private static void VerifyDependencies(
        VerificationContext context,
        PackageArtifact package,
        XElement metadata,
        string version)
    {
        var packageIds = context.Spec.Packages
            .Select(item => item.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var dependencies = metadata
            .Descendants(metadata.Name.Namespace + "dependency")
            .Select(element => new
            {
                Id = RequiredAttribute(element, "id"),
                Version = RequiredAttribute(element, "version"),
            })
            .Where(dependency => packageIds.Contains(dependency.Id))
            .ToList();

        foreach (var expectedDependency in package.Spec.InternalDependencies)
        {
            var matches = dependencies
                .Where(dependency => string.Equals(
                    dependency.Id,
                    expectedDependency,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (matches.Count != 1)
            {
                throw new InvalidDataException(
                    $"{package.Spec.Id} must contain exactly one dependency on {expectedDependency}.");
            }

            RequireEqual($"[{version}]", matches[0].Version, $"{package.Spec.Id} dependency on {expectedDependency}");
        }

        var unexpectedDependencies = dependencies
            .Where(dependency => !package.Spec.InternalDependencies.Contains(
                dependency.Id,
                StringComparer.OrdinalIgnoreCase))
            .Select(dependency => dependency.Id)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (unexpectedDependencies.Count != 0)
        {
            throw new InvalidDataException(
                $"{package.Spec.Id} has unexpected internal dependencies: "
                + string.Join(", ", unexpectedDependencies));
        }
    }

    private static Guid VerifyAssembly(
        string packageId,
        ZipArchiveEntry assemblyEntry,
        SemanticVersion semanticVersion,
        string informationalBaseVersion,
        string commit)
    {
        using var memory = new MemoryStream();
        using (var entryStream = assemblyEntry.Open())
        {
            entryStream.CopyTo(memory);
        }

        memory.Position = 0;
        using var peReader = new PEReader(memory);
        if (!peReader.HasMetadata)
        {
            throw new InvalidDataException($"{packageId} assembly has no managed metadata.");
        }

        var metadataReader = peReader.GetMetadataReader();
        var assemblyDefinition = metadataReader.GetAssemblyDefinition();
        var codeViewEntries = peReader
            .ReadDebugDirectory()
            .Where(entry => entry.Type == DebugDirectoryEntryType.CodeView)
            .ToList();
        if (codeViewEntries.Count != 1)
        {
            throw new InvalidDataException(
                $"{packageId} assembly must contain exactly one CodeView debug entry; found {codeViewEntries.Count}.");
        }

        var codeViewGuid = peReader.ReadCodeViewDebugDirectoryData(codeViewEntries[0]).Guid;
        var assemblyVersion = assemblyDefinition.Version.ToString();
        var expectedAssemblyVersion = semanticVersion.Major == 0
            ? $"0.{semanticVersion.Minor}.0.0"
            : $"{semanticVersion.Major}.0.0.0";
        var expectedFileVersion = $"{semanticVersion.Major}.{semanticVersion.Minor}.{semanticVersion.Patch}.0";
        var fileVersion = ReadAssemblyStringAttribute(metadataReader, "AssemblyFileVersionAttribute");
        var informationalVersion = ReadAssemblyStringAttribute(
            metadataReader,
            "AssemblyInformationalVersionAttribute");
        var expectedInformationalVersion = $"{informationalBaseVersion}+{commit}";

        RequireEqual(expectedAssemblyVersion, assemblyVersion, $"{packageId} AssemblyVersion");
        RequireEqual(expectedFileVersion, fileVersion, $"{packageId} FileVersion");
        if (CountOccurrences(informationalVersion, commit) != 1)
        {
            throw new InvalidDataException(
                $"{packageId} InformationalVersion must contain the full repository SHA exactly once.");
        }

        RequireEqual(
            expectedInformationalVersion,
            informationalVersion,
            $"{packageId} InformationalVersion");
        return codeViewGuid;
    }

    private static void VerifyPortablePdb(string packageId, ZipArchiveEntry pdbEntry, Guid expectedGuid)
    {
        if (pdbEntry.Length == 0)
        {
            throw new InvalidDataException($"{packageId} symbol package contains an empty PDB.");
        }

        using var memory = new MemoryStream();
        using (var entryStream = pdbEntry.Open())
        {
            entryStream.CopyTo(memory);
        }

        memory.Position = 0;
        using var provider = MetadataReaderProvider.FromPortablePdbStream(memory);
        var pdbReader = provider.GetMetadataReader();
        var debugHeader = pdbReader.DebugMetadataHeader
            ?? throw new InvalidDataException($"{packageId} PDB has no debug metadata header.");
        var pdbId = new BlobContentId(debugHeader.Id);
        if (pdbId.Guid != expectedGuid)
        {
            throw new InvalidDataException(
                $"{packageId} PDB id '{pdbId.Guid}' does not match assembly CodeView id '{expectedGuid}'.");
        }
    }

    private static string ReadAssemblyStringAttribute(MetadataReader reader, string attributeName)
    {
        var values = new List<string>();
        foreach (var handle in reader.GetAssemblyDefinition().GetCustomAttributes())
        {
            var attribute = reader.GetCustomAttribute(handle);
            if (!string.Equals(GetAttributeTypeName(reader, attribute.Constructor), attributeName, StringComparison.Ordinal))
            {
                continue;
            }

            var blobReader = reader.GetBlobReader(attribute.Value);
            if (blobReader.ReadUInt16() != 1)
            {
                throw new InvalidDataException($"{attributeName} has an invalid custom-attribute prolog.");
            }

            values.Add(blobReader.ReadSerializedString()
                ?? throw new InvalidDataException($"{attributeName} has a null value."));
        }

        return values.Count == 1
            ? values[0]
            : throw new InvalidDataException($"Expected exactly one {attributeName}; found {values.Count}.");
    }

    private static string? GetAttributeTypeName(MetadataReader reader, EntityHandle constructor)
    {
        EntityHandle declaringType;
        switch (constructor.Kind)
        {
            case HandleKind.MemberReference:
                declaringType = reader.GetMemberReference((MemberReferenceHandle)constructor).Parent;
                break;

            case HandleKind.MethodDefinition:
                declaringType = reader.GetMethodDefinition((MethodDefinitionHandle)constructor).GetDeclaringType();
                break;

            default:
                return null;
        }

        return declaringType.Kind switch
        {
            HandleKind.TypeReference => reader.GetString(
                reader.GetTypeReference((TypeReferenceHandle)declaringType).Name),
            HandleKind.TypeDefinition => reader.GetString(
                reader.GetTypeDefinition((TypeDefinitionHandle)declaringType).Name),
            _ => null,
        };
    }

    private static void RecordHashes(ArtifactSet artifacts, string commit, string outputPath)
    {
        var absoluteOutputPath = Path.GetFullPath(outputPath);
        var entries = artifacts.AllPackagePaths
            .OrderBy(path => Path.GetFileName(path), StringComparer.Ordinal)
            .Select(path => new PackageHash(Path.GetFileName(path), ComputeSha256(path)))
            .ToList();
        var manifest = new HashManifest(artifacts.Version, commit, entries);

        using var stream = new FileStream(
            absoluteOutputPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);
        JsonSerializer.Serialize(stream, manifest, s_jsonOptions);
        stream.WriteByte((byte)'\n');
        Console.WriteLine($"RECORDED {entries.Count} immutable hashes in {absoluteOutputPath}");
    }

    private static void VerifyHashes(ArtifactSet artifacts, string commit, string hashManifestPath)
    {
        var absoluteManifestPath = Path.GetFullPath(hashManifestPath);
        using var stream = File.OpenRead(absoluteManifestPath);
        var manifest = JsonSerializer.Deserialize<HashManifest>(stream, s_jsonOptions)
            ?? throw new InvalidDataException($"Could not deserialize '{absoluteManifestPath}'.");

        RequireEqual(artifacts.Version, manifest.Version, "hash manifest version");
        RequireEqualIgnoreCase(commit, manifest.Commit, "hash manifest commit");

        var expectedPaths = artifacts.AllPackagePaths
            .ToDictionary(
                path => Path.GetFileName(path)
                    ?? throw new InvalidDataException($"Artifact path '{path}' has no filename."),
                StringComparer.Ordinal);
        if (manifest.Artifacts.Count != expectedPaths.Count)
        {
            throw new InvalidDataException(
                $"Hash manifest contains {manifest.Artifacts.Count} files; expected {expectedPaths.Count}.");
        }

        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var artifact in manifest.Artifacts)
        {
            if (!seen.Add(artifact.Name) || !expectedPaths.TryGetValue(artifact.Name, out var path))
            {
                throw new InvalidDataException($"Hash manifest contains unexpected or duplicate file '{artifact.Name}'.");
            }

            RequireEqualIgnoreCase(artifact.Sha256, ComputeSha256(path), $"SHA-256 for {artifact.Name}");
        }
    }

    private static string ComputeSha256(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }

    private static int CountOccurrences(string value, string search)
    {
        var count = 0;
        var startIndex = 0;
        while ((startIndex = value.IndexOf(search, startIndex, StringComparison.OrdinalIgnoreCase)) >= 0)
        {
            count++;
            startIndex += search.Length;
        }

        return count;
    }

    private static string ValidateCommit(string commit)
    {
        if (commit.Length != 40 || commit.Any(character => !Uri.IsHexDigit(character)))
        {
            throw new ArgumentException("The expected repository commit must be a full 40-character SHA.");
        }

        return commit.ToLowerInvariant();
    }

    private static string ResolveRepositoryPath(string repositoryRoot, string relativePath)
    {
        var path = Path.GetFullPath(Path.Combine(repositoryRoot, relativePath));
        var rootWithSeparator = Path.TrimEndingDirectorySeparator(repositoryRoot) + Path.DirectorySeparatorChar;
        if (!path.StartsWith(rootWithSeparator, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Path '{relativePath}' escapes repository root '{repositoryRoot}'.");
        }

        return path;
    }

    private static byte[] ReadCommittedFile(string repositoryRoot, string commit, string relativePath)
    {
        var gitPath = relativePath
            .Replace(Path.DirectorySeparatorChar, '/')
            .Replace(Path.AltDirectorySeparatorChar, '/');
        var startInfo = new ProcessStartInfo("git")
        {
            WorkingDirectory = repositoryRoot,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add("show");
        startInfo.ArgumentList.Add($"{commit}:{gitPath}");

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start git to read the committed README.");
        var errorTask = process.StandardError.ReadToEndAsync();
        using var output = new MemoryStream();
        process.StandardOutput.BaseStream.CopyTo(output);
        process.WaitForExit();
        var error = errorTask.GetAwaiter().GetResult();
        if (process.ExitCode != 0)
        {
            throw new InvalidDataException(
                $"Could not read '{gitPath}' at {commit}: {error.Trim()}");
        }

        return output.ToArray();
    }

    private static bool StreamsEqual(Stream left, Stream right)
    {
        Span<byte> leftBuffer = stackalloc byte[8192];
        Span<byte> rightBuffer = stackalloc byte[8192];
        while (true)
        {
            // Stream.Read may return a short chunk without reaching EOF (notably
            // for compressed zip entries). Compare equal-size blocks or final tails.
            var leftRead = left.ReadAtLeast(leftBuffer, leftBuffer.Length, throwOnEndOfStream: false);
            var rightRead = right.ReadAtLeast(rightBuffer, rightBuffer.Length, throwOnEndOfStream: false);
            if (leftRead != rightRead || !leftBuffer[..leftRead].SequenceEqual(rightBuffer[..rightRead]))
            {
                return false;
            }

            if (leftRead == 0)
            {
                return true;
            }
        }
    }

    private static void EnsureNonEmpty(string path)
    {
        var file = new FileInfo(path);
        if (!file.Exists || file.Length == 0)
        {
            throw new InvalidDataException($"Artifact '{path}' is missing or empty.");
        }
    }

    private static ZipArchiveEntry GetSingleEntry(
        ZipArchive archive,
        Func<ZipArchiveEntry, bool> predicate,
        string description)
    {
        var entries = archive.Entries.Where(predicate).ToList();
        return entries.Count == 1
            ? entries[0]
            : throw new InvalidDataException($"Expected exactly one {description}; found {entries.Count}.");
    }

    private static XDocument LoadXml(ZipArchiveEntry entry)
    {
        using var stream = entry.Open();
        return XDocument.Load(stream, LoadOptions.None);
    }

    private static XElement GetMetadata(XDocument document)
    {
        var root = document.Root ?? throw new InvalidDataException("Nuspec has no root element.");
        return root.Element(root.Name.Namespace + "metadata")
            ?? throw new InvalidDataException("Nuspec has no metadata element.");
    }

    private static XElement RequiredElement(XElement parent, string localName)
    {
        return parent.Element(parent.Name.Namespace + localName)
            ?? throw new InvalidDataException($"Nuspec is missing '{localName}'.");
    }

    private static string RequiredAttribute(XElement element, string localName)
    {
        return element.Attribute(localName)?.Value
            ?? throw new InvalidDataException($"Nuspec element '{element.Name.LocalName}' is missing '{localName}'.");
    }

    private static void RequireEqual(string expected, string actual, string description)
    {
        if (!string.Equals(expected, actual, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Expected {description} '{expected}', found '{actual}'.");
        }
    }

    private static void RequireEqualIgnoreCase(string expected, string actual, string description)
    {
        if (!string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException($"Expected {description} '{expected}', found '{actual}'.");
        }
    }

    private const string Usage = """
        Usage:
          verify --spec <path> --artifacts <directory> [--version <PackageVersion>]
                 --commit <full-SHA> [--hashes <manifest>]
          record-hashes --spec <path> --artifacts <directory> [--version <PackageVersion>]
                        --commit <full-SHA> --output <new-manifest>
        """;
}

internal sealed class CommandOptions
{
    private readonly Dictionary<string, string> _values = new(StringComparer.Ordinal);

    private CommandOptions()
    {
    }

    public static CommandOptions Parse(string[] args)
    {
        var options = new CommandOptions();
        for (var index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length || !args[index].StartsWith("--", StringComparison.Ordinal))
            {
                throw new ArgumentException("Every option must have the form '--name value'.");
            }

            var name = args[index][2..];
            if (!options._values.TryAdd(name, args[index + 1]))
            {
                throw new ArgumentException($"Option '--{name}' was supplied more than once.");
            }
        }

        return options;
    }

    public string Required(string name)
    {
        return _values.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException($"Required option '--{name}' was not supplied.");
    }

    public string? Optional(string name)
    {
        return _values.GetValueOrDefault(name);
    }

    public void RequireOnly(params string[] allowedNames)
    {
        var allowed = allowedNames.ToHashSet(StringComparer.Ordinal);
        var unexpected = _values.Keys.Where(name => !allowed.Contains(name)).Order(StringComparer.Ordinal).ToList();
        if (unexpected.Count != 0)
        {
            throw new ArgumentException(
                $"Unexpected option(s): {string.Join(", ", unexpected.Select(name => $"--{name}"))}.");
        }
    }
}

internal sealed record VerificationContext(VerificationSpec Spec, string RepositoryRoot);

internal sealed record PackageIdentity(string Id, string Version, string Path);

internal sealed record PackageArtifact(
    PackageSpec Spec,
    string NupkgPath,
    string SnupkgPath,
    string Version);

internal sealed record ArtifactSet(
    string ArtifactsPath,
    string Version,
    IReadOnlyList<PackageArtifact> Packages)
{
    public IEnumerable<string> AllPackagePaths => Packages
        .SelectMany(package => new[] { package.NupkgPath, package.SnupkgPath });
}

internal sealed record SemanticVersion(int Major, int Minor, int Patch)
{
    public static SemanticVersion Parse(string value)
    {
        if (value.Contains('+', StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                $"PackageVersion '{value}' contains build metadata; use unique prerelease identifiers instead.");
        }

        var core = value.Split('-', 2)[0];
        var parts = core.Split('.');
        if (parts.Length != 3
            || !int.TryParse(parts[0], out var major)
            || !int.TryParse(parts[1], out var minor)
            || !int.TryParse(parts[2], out var patch)
            || major < 0
            || minor < 0
            || patch < 0)
        {
            throw new InvalidDataException($"PackageVersion '{value}' is not a three-part semantic version.");
        }

        return new SemanticVersion(major, minor, patch);
    }
}

internal sealed record VerificationSpec
{
    public string RepositoryRoot { get; init; } = string.Empty;

    public string RepositoryUrl { get; init; } = string.Empty;

    public string RepositoryType { get; init; } = string.Empty;

    public string TargetFramework { get; init; } = string.Empty;

    public List<PackageSpec> Packages { get; init; } = [];
}

internal sealed record PackageSpec
{
    public string Id { get; init; } = string.Empty;

    public string Assembly { get; init; } = string.Empty;

    public string ReadmeSource { get; init; } = string.Empty;

    public List<string> InternalDependencies { get; init; } = [];
}

internal sealed record HashManifest(string Version, string Commit, List<PackageHash> Artifacts);

internal sealed record PackageHash(string Name, string Sha256);
