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

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Tests that <see cref="DateTimeExtensions.ConvertTimeZone"/> tolerates a <see cref="DateTimeKind"/>
/// that contradicts the source zone — previously this threw an <see cref="ArgumentException"/> from
/// <c>TimeZoneInfo.ConvertTimeToUtc</c>.
/// </summary>
public sealed class DateTimeConvertZoneKindTests
{
    [Fact]
    public void ConvertTimeZone_WithUtcKindButNonUtcSource_DoesNotThrowAndConverts()
    {
        // Kind=Utc contradicts a source zone of America/New_York. Pre-fix this threw; the value is now
        // interpreted as a wall-clock time in the source zone. 2023-03-28 is EDT (UTC-4), so 12:00 → 16:00.
        var input = new DateTime(2023, 3, 28, 12, 0, 0, DateTimeKind.Utc);

        DateTime result = default;
        var act = () => result = input.ConvertTimeZone("America/New_York", "UTC");

        act.Should().NotThrow();
        result.Should().Be(new DateTime(2023, 3, 28, 16, 0, 0));
    }

    [Fact]
    public void ConvertTimeZone_WithLocalKind_DoesNotThrow()
    {
        var input = new DateTime(2023, 6, 1, 9, 0, 0, DateTimeKind.Local);

        var act = () => input.ConvertTimeZone("Asia/Tokyo", "UTC");

        act.Should().NotThrow();
    }

    [Fact]
    public void ConvertTimeZone_DstSpringForwardGap_ThrowsArgumentException()
    {
        // G-04 / AC-2.4: 02:30 on 2023-03-12 does not exist in America/New_York — the clocks jump
        // from 02:00 to 03:00. TimeZoneInfo.ConvertTimeToUtc rejects the non-existent wall-clock time
        // with an ArgumentException; the XML docs now document this DST spring-forward-gap failure.
        var input = new DateTime(2023, 3, 12, 2, 30, 0, DateTimeKind.Unspecified);

        var act = () => input.ConvertTimeZone("America/New_York", "UTC");

        act.Should().Throw<ArgumentException>();
    }
}
