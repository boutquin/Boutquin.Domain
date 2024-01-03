// Copyright (c) 2023-2024 Pierre G. Boutquin. All rights reserved.
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
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

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
