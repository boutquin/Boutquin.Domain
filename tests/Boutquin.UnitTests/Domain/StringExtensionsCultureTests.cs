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

using System.Globalization;

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Tests for the culture handling of <see cref="StringExtensions"/>: the parameterless casing helpers
/// must use the invariant culture (avoiding the Turkish dotted/dotless-I surprise), while the
/// culture-accepting overloads must honor the supplied culture.
/// </summary>
public sealed class StringExtensionsCultureTests
{
    [Fact]
    public void ToUpperCaseFirst_DefaultsToInvariantCulture()
    {
        // Under the Turkish culture, 'i'.ToUpper() is the dotted capital 'İ'. The default overload must
        // use the invariant culture and produce the plain ASCII 'I' regardless of ambient culture.
        var original = "istanbul".ToUpperCaseFirst();

        original.Should().Be("Istanbul");
    }

    [Fact]
    public void ToUpperCaseFirst_WithTurkishCulture_AppliesTurkishCasing()
    {
        var result = "istanbul".ToUpperCaseFirst(new CultureInfo("tr-TR"));

        result.Should().StartWith("İ"); // 'İ' — Latin capital I with dot above
    }

    [Fact]
    public void ToLowerCaseFirst_DefaultsToInvariantCulture()
        => "INSIDE".ToLowerCaseFirst().Should().Be("iNSIDE");

    [Fact]
    public void ToLowerCaseFirst_WithTurkishCulture_AppliesTurkishCasing()
    {
        // AC-4.3: the explicit-culture overload of ToLowerCaseFirst had no direct test — only its
        // invariant-culture convenience overload (above) was exercised. Under Turkish casing rules,
        // 'I'.ToLower() is the dotless 'ı', distinct from the invariant-culture 'i'.
        var result = "Istanbul".ToLowerCaseFirst(new CultureInfo("tr-TR"));

        result.Should().StartWith("ı"); // 'ı' — Latin small letter dotless i
    }

    [Fact]
    public void ToUpperCaseFirst_WithNullCulture_ThrowsArgumentNullException()
    {
        var act = () => "abc".ToUpperCaseFirst(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("culture");
    }

    [Fact]
    public void Format_WithInvariantProvider_IsLocaleStable()
    {
        var formatted = "{0:N1}".Format(CultureInfo.InvariantCulture, 1234.5m);

        formatted.Should().Be("1,234.5");
    }
}
