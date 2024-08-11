// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
