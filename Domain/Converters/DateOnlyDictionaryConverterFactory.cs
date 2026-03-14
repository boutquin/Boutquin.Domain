// Copyright (c) 2024-2026 Pierre G. Boutquin. All rights reserved.
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

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// A custom <see cref="JsonConverterFactory"/> that enables <see cref="System.Text.Json"/> to
/// serialize and deserialize dictionaries with <see cref="DateOnly"/> keys.
/// </summary>
/// <remarks>
/// <para>
/// <b>The problem this solves:</b> <see cref="System.Text.Json"/> can only serialize dictionaries
/// whose keys are strings or types with a built-in conversion to string (e.g., <c>int</c>, <c>Guid</c>).
/// <see cref="DateOnly"/> — introduced in .NET 6 — is <em>not</em> one of those types. Attempting to
/// serialize a <c>Dictionary&lt;DateOnly, T&gt;</c> without this converter throws a
/// <see cref="NotSupportedException"/> at runtime:
/// <code>
/// "The type 'System.DateOnly' is not a supported dictionary key using converter of type 'DateOnlyConverter'."
/// </code>
/// This is a known limitation of <see cref="System.Text.Json"/>: the serializer requires dictionary
/// keys to implement a string representation that it can use as JSON property names, and <see cref="DateOnly"/>
/// doesn't satisfy that requirement out of the box.
/// </para>
/// <para>
/// <b>Why a JsonConverterFactory (not a plain JsonConverter):</b> The factory pattern is required
/// because the value type <c>TValue</c> of the dictionary is not known at compile time. A single
/// <c>JsonConverter&lt;Dictionary&lt;DateOnly, decimal&gt;&gt;</c> would only handle <c>decimal</c>
/// values. The factory inspects the actual generic arguments at runtime, constructs the appropriately
/// typed converter via <see cref="Type.MakeGenericType"/>, and returns it — supporting any value type
/// without needing a separate converter registration per <c>TValue</c>.
/// </para>
/// <para>
/// <b>Why two separate inner converter classes (Sorted vs. Unsorted):</b>
/// <see cref="SortedDictionary{TKey,TValue}"/> and <see cref="Dictionary{TKey,TValue}"/> are distinct
/// types with different generic type definitions. <see cref="System.Text.Json"/> matches converters by
/// exact type — a <c>JsonConverter&lt;Dictionary&lt;DateOnly, T&gt;&gt;</c> will not be used for a
/// <c>SortedDictionary&lt;DateOnly, T&gt;</c>. Each dictionary type therefore needs its own strongly-typed
/// converter so that the serializer can find the right one via the type system. The alternative —
/// a single converter using <c>IDictionary</c> — would lose the concrete type information during
/// deserialization (the caller would get back a <c>Dictionary</c> when they serialized a
/// <c>SortedDictionary</c>).
/// </para>
/// <para>
/// <b>Why ISO 8601 (<c>yyyy-MM-dd</c>) for key serialization:</b> JSON property names must be strings.
/// The ISO 8601 date format is the universally accepted, unambiguous, sortable representation of dates.
/// Using <see cref="CultureInfo.InvariantCulture"/> ensures the format is consistent regardless of the
/// server's locale — a <c>DateOnly</c> of 2024-03-15 always serializes as <c>"2024-03-15"</c>, never
/// <c>"15/03/2024"</c> or <c>"3/15/2024"</c>. This makes the JSON output portable across systems and
/// parseable by any JSON consumer without locale negotiation.
/// </para>
/// <para>
/// <b>Why null values are rejected during deserialization:</b> The inner converters throw
/// <see cref="JsonException"/> when a deserialized value is <c>null</c>. This is because
/// <c>Dictionary&lt;DateOnly, TValue&gt;.Add</c> would accept <c>null</c> for reference-type values
/// but produce a dictionary entry that violates the non-null expectations of most domain code.
/// Failing fast at deserialization time — with a clear message — is easier to diagnose than a
/// <see cref="NullReferenceException"/> later when the value is accessed.
/// </para>
/// </remarks>
/// <example>
/// Register the converter globally in <c>JsonSerializerOptions</c>:
/// <code>
/// var options = new JsonSerializerOptions();
/// options.Converters.Add(new DateOnlyDictionaryConverterFactory());
///
/// var json = JsonSerializer.Serialize(myDateDictionary, options);
/// var dict = JsonSerializer.Deserialize&lt;Dictionary&lt;DateOnly, decimal&gt;&gt;(json, options);
/// </code>
/// </example>
public sealed class DateOnlyDictionaryConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// Determines whether the specified type can be converted.
    /// </summary>
    /// <remarks>
    /// The check inspects the generic type definition (not just assignability) because we need
    /// to know whether to create a Sorted or Unsorted converter. Checking <c>IsGenericType</c>
    /// first avoids the <see cref="InvalidOperationException"/> that <c>GetGenericTypeDefinition</c>
    /// would throw on non-generic types.
    /// </remarks>
    /// <param name="typeToConvert">The type to check for compatibility.</param>
    /// <returns>True if the type is a Dictionary or SortedDictionary with DateOnly keys.</returns>
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType &&
        typeToConvert.GetGenericArguments()[0] == typeof(DateOnly) &&
        (typeToConvert.GetGenericTypeDefinition() == typeof(SortedDictionary<,>) ||
         typeToConvert.GetGenericTypeDefinition() == typeof(Dictionary<,>));

    /// <summary>
    /// Creates a <see cref="JsonConverter"/> for the specified dictionary type with <see cref="DateOnly"/> keys.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why <see cref="Type.MakeGenericType"/> and <see cref="Activator.CreateInstance"/>:</b>
    /// The value type <c>TValue</c> is only known at runtime (from the dictionary's generic arguments).
    /// We cannot write <c>new SortedDateOnlyDictionaryConverter&lt;TValue&gt;()</c> because <c>TValue</c>
    /// is a <see cref="Type"/> variable, not a compile-time generic parameter. <c>MakeGenericType</c>
    /// constructs the closed generic type, and <c>Activator.CreateInstance</c> instantiates it via
    /// reflection. This is the standard pattern used by <see cref="JsonConverterFactory"/> implementations
    /// throughout the .NET ecosystem.
    /// </para>
    /// <para>
    /// <b>Why the defensive <c>CanConvert</c> check at the top:</b> The <see cref="JsonConverterFactory"/>
    /// contract says <c>CreateConverter</c> will only be called for types where <c>CanConvert</c> returned
    /// <c>true</c>. However, if someone calls this method directly (outside the serializer pipeline), the
    /// guard prevents an <see cref="IndexOutOfRangeException"/> from <c>GetGenericArguments()[1]</c> on a
    /// non-generic type.
    /// </para>
    /// </remarks>
    /// <param name="typeToConvert">The type to create a converter for.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>A JsonConverter instance for the specified type.</returns>
    /// <exception cref="ArgumentException">Thrown when the type is not supported.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the converter cannot be created.</exception>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (!CanConvert(typeToConvert))
        {
            throw new ArgumentException($"Invalid type to convert: {typeToConvert}.", nameof(typeToConvert));
        }

        var valueType = typeToConvert.GetGenericArguments()[1];
        var genericDef = typeToConvert.GetGenericTypeDefinition();

        // Select the converter class that matches the concrete dictionary type. This branching
        // is necessary because System.Text.Json resolves converters by exact type match — a
        // converter for Dictionary<,> will not be applied to a SortedDictionary<,>.
        Type converterType;
        if (genericDef == typeof(SortedDictionary<,>))
        {
            converterType = typeof(SortedDateOnlyDictionaryConverter<>).MakeGenericType(valueType);
        }
        else
        {
            converterType = typeof(UnsortedDateOnlyDictionaryConverter<>).MakeGenericType(valueType);
        }

        return Activator.CreateInstance(converterType, options) as JsonConverter
            ?? throw new InvalidOperationException($"The converter for type {converterType} could not be created.");
    }

    /// <summary>
    /// Handles serialization/deserialization of <see cref="SortedDictionary{TKey,TValue}"/>
    /// with <see cref="DateOnly"/> keys.
    /// </summary>
    /// <remarks>
    /// <b>Why a separate class from <see cref="UnsortedDateOnlyDictionaryConverter{TValue}"/>:</b>
    /// System.Text.Json matches converters by exact generic type. <c>SortedDictionary&lt;,&gt;</c>
    /// and <c>Dictionary&lt;,&gt;</c> have different <c>GetGenericTypeDefinition()</c> results,
    /// so they require separate converter registrations. Merging them into one converter over
    /// <c>IDictionary</c> would lose the concrete collection type during deserialization — the
    /// caller would get a <c>Dictionary</c> back when they expected a <c>SortedDictionary</c>.
    /// </remarks>
    /// <typeparam name="TValue">The type of the dictionary values.</typeparam>
    private sealed class SortedDateOnlyDictionaryConverter<TValue> : JsonConverter<SortedDictionary<DateOnly, TValue>>
    {
        private readonly JsonConverter<DateOnly> _dateOnlyConverter;
        private readonly JsonConverter<TValue> _valueConverter;

        /// <remarks>
        /// Converters are resolved from the provided <paramref name="options"/> rather than
        /// created directly. This ensures we respect any custom converters the caller has
        /// registered (e.g., a custom DateOnly format) and avoids duplicating serializer logic.
        /// </remarks>
        public SortedDateOnlyDictionaryConverter(JsonSerializerOptions options)
        {
            _dateOnlyConverter = (JsonConverter<DateOnly>)options.GetConverter(typeof(DateOnly));
            _valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));
        }

        /// <remarks>
        /// <para>
        /// <b>Why manual token reading instead of <c>JsonSerializer.Deserialize</c>:</b>
        /// The <see cref="Utf8JsonReader"/> is a forward-only reader. Using
        /// <c>JsonSerializer.Deserialize</c> for the key would consume the property name token,
        /// but we need to use the <c>_dateOnlyConverter</c> to parse it as a <see cref="DateOnly"/>.
        /// Manual token-by-token reading gives us full control over the parse sequence:
        /// read property name → parse as DateOnly key → advance → parse value.
        /// </para>
        /// </remarks>
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

                // Reject null values explicitly — allowing them would create dictionary entries
                // that silently violate non-null expectations downstream, producing hard-to-diagnose
                // NullReferenceExceptions far from the deserialization site.
                if (value == null)
                {
                    throw new JsonException("Null value cannot be added to the dictionary.");
                }

                dictionary.Add(key, value);
            }

            return dictionary;
        }

        /// <remarks>
        /// Keys are serialized as ISO 8601 property names (<c>yyyy-MM-dd</c>) using
        /// <see cref="CultureInfo.InvariantCulture"/> to guarantee locale-independent output.
        /// This format is sortable, unambiguous, and the de facto standard for date interchange.
        /// </remarks>
        public override void Write(Utf8JsonWriter writer, SortedDictionary<DateOnly, TValue> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var kvp in value)
            {
                writer.WritePropertyName(kvp.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                _valueConverter.Write(writer, kvp.Value, options);
            }

            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// Handles serialization/deserialization of <see cref="Dictionary{TKey,TValue}"/>
    /// with <see cref="DateOnly"/> keys.
    /// </summary>
    /// <remarks>
    /// This is the unsorted counterpart to <see cref="SortedDateOnlyDictionaryConverter{TValue}"/>.
    /// See that class's remarks for why both must exist as separate types. The serialization logic
    /// is identical — only the concrete dictionary type differs.
    /// </remarks>
    /// <typeparam name="TValue">The type of the dictionary values.</typeparam>
    private sealed class UnsortedDateOnlyDictionaryConverter<TValue> : JsonConverter<Dictionary<DateOnly, TValue>>
    {
        private readonly JsonConverter<DateOnly> _dateOnlyConverter;
        private readonly JsonConverter<TValue> _valueConverter;

        public UnsortedDateOnlyDictionaryConverter(JsonSerializerOptions options)
        {
            _dateOnlyConverter = (JsonConverter<DateOnly>)options.GetConverter(typeof(DateOnly));
            _valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));
        }

        public override Dictionary<DateOnly, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dictionary = new Dictionary<DateOnly, TValue>();

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

        public override void Write(Utf8JsonWriter writer, Dictionary<DateOnly, TValue> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var kvp in value)
            {
                writer.WritePropertyName(kvp.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                _valueConverter.Write(writer, kvp.Value, options);
            }

            writer.WriteEndObject();
        }
    }
}
