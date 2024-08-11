// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Boutquin.Domain.Extensions;

using System.Buffers;
using System.Text.Json;

/// <summary>
/// Provides extension methods for the System.Text.Json.JsonElement class.
/// </summary>
public static class JsonElementExtensions
{
    /// <summary>
    /// Deserializes the given JsonElement to an object of type T using the specified JsonSerializerOptions.
    /// </summary>
    /// <typeparam name="T">The target type of the deserialization.</typeparam>
    /// <param name="element">The JsonElement to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions to use during deserialization. If not provided, the default options are used.</param>
    /// <returns>An object of type T created from the given JsonElement.</returns>
    /// <example>
    /// <code>
    /// JsonElement element = ...;
    /// MyClass obj = element.ToObject&lt;MyClass&gt;();
    /// </code>
    /// </example>
    public static T? ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(bufferWriter))
        {
            element.WriteTo(writer);
        }
        return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options);
    }
}
