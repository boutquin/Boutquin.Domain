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
namespace Boutquin.Domain.Converters;

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// A custom JSON converter for serializing and deserializing DateOnly objects.
/// </summary>
public sealed class DateOnlyConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Deserializes a JSON string to a DateOnly object.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type of the object being deserialized.</param>
    /// <param name="options">The JSON serializer options.</param>
    /// <returns>A DateOnly object deserialized from the JSON string.</returns>
    /// <exception cref="JsonException">Thrown when the date string is null or cannot be parsed to a DateOnly object.</exception>
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString() ??
            throw new JsonException("Date string is null.");

        if (!DateOnly.TryParse(dateString, out var date))
        {
            throw new JsonException($"Invalid date format: {dateString}");
        }

        return date;
    }

    /// <summary>
    /// Serializes a DateOnly object to a JSON string.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The DateOnly object to serialize.</param>
    /// <param name="options">The JSON serializer options.</param>
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
}
