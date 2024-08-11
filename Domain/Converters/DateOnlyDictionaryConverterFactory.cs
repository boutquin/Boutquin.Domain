// Copyright (c) 2024 Pierre G. Boutquin. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License").
//   You may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

namespace Boutquin.Domain.Converters;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// A custom JsonConverterFactory to handle SortedDictionaries with DateOnly keys.
/// </summary>
public sealed class DateOnlyDictionaryConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// Determines whether the specified type can be converted to SortedDictionary&lt;DateOnly, TValue&gt;.
    /// </summary>
    /// <param name="typeToConvert">The type of the object to check for compatibility with SortedDictionary&lt;DateOnly, TValue&gt;.</param>
    /// <returns>True if the specified type is compatible with SortedDictionary&lt;DateOnly, TValue&gt;; otherwise, false.</returns>
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType &&
        typeToConvert.GetGenericTypeDefinition() == typeof(SortedDictionary<,>) &&
        typeToConvert.GetGenericArguments()[0] == typeof(DateOnly);

    /// <summary>
    /// Creates a JsonConverter that can read and write SortedDictionary&lt;DateOnly, TValue&gt; instances.
    /// </summary>
    /// <param name="typeToConvert">The type of the object to create a converter for. Must be SortedDictionary&lt;DateOnly, TValue&gt;.</param>
    /// <param name="options">The serializer options to use for the converter.</param>
    /// <returns>A JsonConverter that can read and write SortedDictionary&lt;DateOnly, TValue&gt; instances.</returns>
    /// <exception cref="ArgumentException">Thrown when the typeToConvert is not a SortedDictionary&lt;DateOnly, TValue&gt;.</exception>
    /// <exception cref="MissingMethodException">Thrown when the converter type does not have a constructor that takes a JsonSerializerOptions parameter.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the created converter is null.</exception>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (!CanConvert(typeToConvert))
        {
            throw new ArgumentException($"Invalid type to convert: {typeToConvert}.", nameof(typeToConvert));
        }

        var valueType = typeToConvert.GetGenericArguments()[1];
        var converterType = typeof(DateOnlyDictionaryConverter<>).MakeGenericType(valueType);

        return Activator.CreateInstance(converterType, options) as JsonConverter ?? throw new InvalidOperationException($"The converter for type {converterType} could not be created.");
    }

    /// <summary>
    /// A JsonConverter implementation that converts SortedDictionary&lt;DateOnly, TValue&gt; to and from JSON.
    /// TValue is the type of the values stored in the dictionary.
    /// </summary>
    /// <typeparam name="TValue">The type of the values stored in the dictionary.</typeparam>
    private sealed class DateOnlyDictionaryConverter<TValue> : JsonConverter<SortedDictionary<DateOnly, TValue>>
    {
        private readonly JsonConverter<DateOnly> _dateOnlyConverter;
        private readonly JsonConverter<TValue> _valueConverter;

        /// <summary>
        /// Creates a new instance of DateOnlyDictionaryConverter.
        /// </summary>
        /// <param name="options">The serializer options to use for the converter.</param>
        public DateOnlyDictionaryConverter(JsonSerializerOptions options)
        {
            _dateOnlyConverter = (JsonConverter<DateOnly>)options.GetConverter(typeof(DateOnly));
            _valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));
        }

        /// <summary>
        /// Reads a SortedDictionary&lt;DateOnly, TValue&gt; from JSON.
        /// </summary>
        /// <param name="reader">The JSON reader to read from.</param>
        /// <param name="typeToConvert">The type of object to convert. Must be SortedDictionary&lt;DateOnly, TValue&gt;.</param>
        /// <param name="options">The serializer options to use for the conversion.</param>
        /// <returns>A SortedDictionary&lt;DateOnly, TValue&gt; instance read from JSON.</returns>
        /// <exception cref="JsonException">Thrown when the value is null or the JSON is invalid.</exception>
        public override SortedDictionary<DateOnly, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dictionary = new SortedDictionary<DateOnly, TValue>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dictionary;
                }

                var key = _dateOnlyConverter.Read(ref reader, typeof(DateOnly), options);
                reader.Read();
                var value = _valueConverter.Read(ref reader, typeof(TValue), options);

                if (value == null)
                {
                    throw new JsonException("Null value cannot be added to the dictionary.");
                }

                dictionary.Add(key, value);
            }

            return dictionary;
        }

        /// <summary>
        /// Writes a SortedDictionary&lt;DateOnly, TValue&gt; to JSON.
        /// </summary>
        /// <param name="writer">The JSON writer to write to.</param>
        /// <param name="value">The SortedDictionary&lt;DateOnly, TValue&gt; instance to write.</param>
        /// <param name="options">The serializer options to use for the conversion.</param>
        public override void Write(Utf8JsonWriter writer, SortedDictionary<DateOnly, TValue> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var kvp in value)
            {
                _dateOnlyConverter.Write(writer, kvp.Key, options);
                _valueConverter.Write(writer, kvp.Value, options);
            }

            writer.WriteEndObject();
        }
    }
}
