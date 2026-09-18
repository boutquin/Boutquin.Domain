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
/// A strongly typed identifier backed by a decimal, used to pin <see cref="StronglyTypedId{TValue}.ToString"/>'s
/// culture-invariant formatting (P-11) — a <see cref="Guid"/>- or <see cref="int"/>-backed id renders
/// identically under every culture, so it cannot expose a culture-sensitivity regression the way a
/// decimal-backed id can (comma vs. period decimal separator).
/// </summary>
public sealed record TestDecimalId(decimal Value) : StronglyTypedId<decimal>(Value);

/// <summary>
/// Contains unit tests pinning <see cref="StronglyTypedId{TValue}.ToString"/>'s culture-invariant
/// formatting contract (P-11 / AC-3.11), split out from <c>StronglyTypedIdTests</c> so this
/// culture-sensitive fixture stays isolated from the class's Guid/int-backed identity tests.
/// </summary>
public sealed class StronglyTypedIdCultureTests
{
    [Fact]
    public void ToString_UnderNonInvariantCulture_FormatsWithInvariantCulture()
    {
        // Format on a dedicated thread whose culture uses a comma decimal separator, proving invariance
        // WITHOUT mutating the shared process/ambient culture (which would race parallel-running tests) —
        // same isolation pattern as MoneyTests.ToString_IsInvariantAmountThenCode.
        string? formatted = null;
        var thread = new Thread(() =>
        {
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            formatted = new TestDecimalId(1234.5m).ToString();
        });
        thread.Start();
        thread.Join();

        // Under fr-FR, decimal.ToString() would render "1234,5" (comma separator) absent the fix;
        // CultureInfo.InvariantCulture always renders "1234.5" (period separator).
        formatted.Should().Be("1234.5");
    }
}
