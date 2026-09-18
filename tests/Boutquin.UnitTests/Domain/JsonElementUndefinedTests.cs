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

using System.Text.Json;

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Tests that <see cref="JsonElementExtensions.ToObject{T}"/> rejects an <see cref="JsonValueKind.Undefined"/>
/// element with a clear <see cref="ArgumentException"/> rather than the opaque
/// <see cref="InvalidOperationException"/> that <c>WriteTo</c> would otherwise raise.
/// </summary>
public sealed class JsonElementUndefinedTests
{
    [Fact]
    public void ToObject_OnDefaultElement_ThrowsArgumentException()
    {
        var undefined = default(JsonElement);

        var act = () => undefined.ToObject<string>();

        act.Should().Throw<ArgumentException>().WithParameterName("element");
    }

    [Fact]
    public void ToObject_OnAbsentProperty_ThrowsArgumentException()
    {
        using var doc = JsonDocument.Parse("{}");
        doc.RootElement.TryGetProperty("missing", out var absent);

        var act = () => absent.ToObject<string>();

        act.Should().Throw<ArgumentException>().WithParameterName("element");
    }

    [Fact]
    public void ToObject_OnValidElement_Deserializes()
    {
        using var doc = JsonDocument.Parse("\"hello\"");

        doc.RootElement.ToObject<string>().Should().Be("hello");
    }
}
