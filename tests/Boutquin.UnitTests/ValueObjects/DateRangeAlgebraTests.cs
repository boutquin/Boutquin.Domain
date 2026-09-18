// Copyright (c) 2024-2026 Pierre G. Boutquin. All rights reserved.
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
//
//  See the License for the specific language governing permissions and
//  limitations under the License.
//
using Boutquin.Domain.ValueObjects;

namespace Boutquin.UnitTests.ValueObjects;

public sealed class DateRangeAlgebraTests
{
    private static DateOnly D(int y, int m, int d) => new(y, m, d);

    [Fact]
    public void Constructor_ReversedDates_IsAllowedAndRepresentsEmpty()
    {
        var empty = new DateRange(D(2026, 4, 18), D(2020, 1, 1));

        empty.LengthInDays.Should().Be(0);
        empty.Contains(D(2023, 1, 1)).Should().BeFalse();
    }

    [Fact]
    public void LengthInDays_NormalRange_IsInclusiveCount()
    {
        var r = new DateRange(D(2020, 1, 1), D(2020, 1, 3));

        r.LengthInDays.Should().Be(3);
    }

    [Fact]
    public void Contains_Date_InRange_True()
    {
        var r = new DateRange(D(2020, 1, 1), D(2020, 12, 31));

        r.Contains(D(2020, 6, 1)).Should().BeTrue();
        r.Contains(D(2020, 1, 1)).Should().BeTrue();
        r.Contains(D(2020, 12, 31)).Should().BeTrue();
    }

    [Fact]
    public void Contains_Date_OutsideRange_False()
    {
        var r = new DateRange(D(2020, 1, 1), D(2020, 12, 31));

        r.Contains(D(2019, 12, 31)).Should().BeFalse();
        r.Contains(D(2021, 1, 1)).Should().BeFalse();
    }

    [Fact]
    public void Contains_Range_Superset_True()
    {
        var outer = new DateRange(D(2020, 1, 1), D(2026, 1, 1));
        var inner = new DateRange(D(2021, 6, 1), D(2022, 6, 1));

        outer.Contains(inner).Should().BeTrue();
    }

    [Fact]
    public void Contains_Range_PartialOverlap_False()
    {
        var a = new DateRange(D(2020, 1, 1), D(2021, 1, 1));
        var b = new DateRange(D(2020, 6, 1), D(2022, 1, 1));

        a.Contains(b).Should().BeFalse();
    }

    [Fact]
    public void Contains_Range_EmptyIsSubsetOfEverything()
    {
        var normal = new DateRange(D(2020, 1, 1), D(2026, 1, 1));
        var empty = new DateRange(D(2026, 1, 1), D(2020, 1, 1));

        normal.Contains(empty).Should().BeTrue();
        empty.Contains(empty).Should().BeTrue();
    }

    [Fact]
    public void Contains_Range_NonEmptyNotSubsetOfEmpty()
    {
        var empty = new DateRange(D(2026, 1, 1), D(2020, 1, 1));
        var normal = new DateRange(D(2020, 1, 1), D(2026, 1, 1));

        empty.Contains(normal).Should().BeFalse();
    }

    [Fact]
    public void Overlaps_TouchingAtPoint_True()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 6, 15));
        var b = new DateRange(D(2020, 6, 15), D(2020, 12, 31));

        a.Overlaps(b).Should().BeTrue();
    }

    [Fact]
    public void Overlaps_Disjoint_False()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 6, 14));
        var b = new DateRange(D(2020, 6, 16), D(2020, 12, 31));

        a.Overlaps(b).Should().BeFalse();
    }

    [Fact]
    public void Overlaps_WithEmpty_False()
    {
        var normal = new DateRange(D(2020, 1, 1), D(2020, 12, 31));
        var empty = new DateRange(D(2020, 12, 31), D(2020, 1, 1));

        normal.Overlaps(empty).Should().BeFalse();
        empty.Overlaps(normal).Should().BeFalse();
    }

    [Fact]
    public void Intersect_Overlapping_ReturnsIntersection()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 12, 31));
        var b = new DateRange(D(2020, 6, 1), D(2022, 1, 1));

        a.Intersect(b).Should().Be(new DateRange(D(2020, 6, 1), D(2020, 12, 31)));
    }

    [Fact]
    public void Intersect_Disjoint_ReturnsNull()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 1, 31));
        var b = new DateRange(D(2020, 3, 1), D(2020, 3, 31));

        a.Intersect(b).Should().BeNull();
    }

    [Fact]
    public void Intersect_WithEmpty_ReturnsNull()
    {
        var normal = new DateRange(D(2020, 1, 1), D(2020, 12, 31));
        var empty = new DateRange(D(2020, 12, 31), D(2020, 1, 1));

        normal.Intersect(empty).Should().BeNull();
    }

    [Fact]
    public void Minus_FullSubset_ReturnsEmptyList()
    {
        var outer = new DateRange(D(2020, 1, 1), D(2021, 12, 31));
        var covered = new DateRange(D(2019, 1, 1), D(2022, 12, 31));

        outer.Minus(covered).Should().BeEmpty();
    }

    [Fact]
    public void Minus_PrefixGap_ReturnsPrefixOnly()
    {
        var requested = new DateRange(D(2005, 1, 1), D(2026, 4, 18));
        var covered = new DateRange(D(2010, 1, 1), D(2026, 4, 18));

        var gaps = requested.Minus(covered);

        gaps.Should().ContainSingle();
        gaps[0].Should().Be(new DateRange(D(2005, 1, 1), D(2009, 12, 31)));
    }

    [Fact]
    public void Minus_SuffixGap_ReturnsSuffixOnly()
    {
        var requested = new DateRange(D(2010, 1, 1), D(2026, 4, 18));
        var covered = new DateRange(D(2010, 1, 1), D(2020, 12, 31));

        var gaps = requested.Minus(covered);

        gaps.Should().ContainSingle();
        gaps[0].Should().Be(new DateRange(D(2021, 1, 1), D(2026, 4, 18)));
    }

    [Fact]
    public void Minus_BothGaps_ReturnsTwo()
    {
        var requested = new DateRange(D(2005, 1, 1), D(2026, 12, 31));
        var covered = new DateRange(D(2010, 1, 1), D(2020, 12, 31));

        var gaps = requested.Minus(covered);

        gaps.Should().HaveCount(2);
        gaps[0].Should().Be(new DateRange(D(2005, 1, 1), D(2009, 12, 31)));
        gaps[1].Should().Be(new DateRange(D(2021, 1, 1), D(2026, 12, 31)));
    }

    [Fact]
    public void Minus_Disjoint_ReturnsOriginal()
    {
        var a = new DateRange(D(2005, 1, 1), D(2009, 12, 31));
        var b = new DateRange(D(2015, 1, 1), D(2020, 12, 31));

        var gaps = a.Minus(b);

        gaps.Should().ContainSingle();
        gaps[0].Should().Be(a);
    }

    [Fact]
    public void Union_Overlapping_ReturnsEnclosingRange()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 6, 30));
        var b = new DateRange(D(2020, 6, 1), D(2020, 12, 31));

        a.Union(b).Should().Be(new DateRange(D(2020, 1, 1), D(2020, 12, 31)));
    }

    [Fact]
    public void Union_Adjacent_ReturnsEnclosingRange()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 6, 30));
        var b = new DateRange(D(2020, 7, 1), D(2020, 12, 31));

        a.Union(b).Should().Be(new DateRange(D(2020, 1, 1), D(2020, 12, 31)));
    }

    [Fact]
    public void Union_Disjoint_Throws()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 6, 30));
        var b = new DateRange(D(2020, 8, 1), D(2020, 12, 31));

        Action act = () => a.Union(b);

        act.Should().Throw<ArgumentException>();
    }

    // Minus + Intersect reproduce the original range (set-wise).
    [Fact]
    public void MinusAndIntersect_Composition_ReproducesOriginal()
    {
        var a = new DateRange(D(2020, 1, 1), D(2026, 4, 18));
        var b = new DateRange(D(2018, 6, 1), D(2022, 6, 30));

        var pieces = new List<DateRange>();
        pieces.AddRange(a.Minus(b));
        var intersection = a.Intersect(b);
        if (intersection is not null)
        {
            pieces.Add(intersection.Value);
        }

        // Sum of inclusive day counts equals the original.
        pieces.Sum(p => p.LengthInDays).Should().Be(a.LengthInDays);
    }

    // ── Empty-side branches in the algebra ───────────────────────────────────

    [Fact]
    public void Overlaps_LeftEmpty_ReturnsFalse()
    {
        var empty = new DateRange(D(2020, 12, 31), D(2020, 1, 1));
        var nonEmpty = new DateRange(D(2020, 1, 1), D(2020, 12, 31));
        empty.Overlaps(nonEmpty).Should().BeFalse();
    }

    [Fact]
    public void Overlaps_RightEmpty_ReturnsFalse()
    {
        var nonEmpty = new DateRange(D(2020, 1, 1), D(2020, 12, 31));
        var empty = new DateRange(D(2020, 12, 31), D(2020, 1, 1));
        nonEmpty.Overlaps(empty).Should().BeFalse();
    }

    [Fact]
    public void Minus_EmptyMinusAnything_ReturnsEmpty()
    {
        var empty = new DateRange(D(2020, 12, 31), D(2020, 1, 1));
        var other = new DateRange(D(2020, 1, 1), D(2020, 12, 31));
        empty.Minus(other).Should().BeEmpty();
    }

    [Fact]
    public void Minus_PrefixOnly_ReturnsSinglePrefixRange()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 12, 31));
        var b = new DateRange(D(2020, 7, 1), D(2020, 12, 31));
        var result = a.Minus(b);
        result.Should().HaveCount(1);
        result[0].From.Should().Be(D(2020, 1, 1));
        result[0].To.Should().Be(D(2020, 6, 30));
    }

    [Fact]
    public void Minus_SuffixOnly_ReturnsSingleSuffixRange()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 12, 31));
        var b = new DateRange(D(2020, 1, 1), D(2020, 6, 30));
        var result = a.Minus(b);
        result.Should().HaveCount(1);
        result[0].From.Should().Be(D(2020, 7, 1));
        result[0].To.Should().Be(D(2020, 12, 31));
    }

    [Fact]
    public void Union_LeftEmpty_ReturnsRight()
    {
        var empty = new DateRange(D(2020, 12, 31), D(2020, 1, 1));
        var nonEmpty = new DateRange(D(2020, 1, 1), D(2020, 12, 31));
        empty.Union(nonEmpty).Should().Be(nonEmpty);
    }

    [Fact]
    public void Union_RightEmpty_ReturnsLeft()
    {
        var nonEmpty = new DateRange(D(2020, 1, 1), D(2020, 12, 31));
        var empty = new DateRange(D(2020, 12, 31), D(2020, 1, 1));
        nonEmpty.Union(empty).Should().Be(nonEmpty);
    }

    [Fact]
    public void Union_AdjacentRanges_LeftBeforeRight_Merges()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 6, 30));
        var b = new DateRange(D(2020, 7, 1), D(2020, 12, 31));
        var result = a.Union(b);
        result.From.Should().Be(D(2020, 1, 1));
        result.To.Should().Be(D(2020, 12, 31));
    }

    [Fact]
    public void Union_AdjacentRanges_RightBeforeLeft_Merges()
    {
        var a = new DateRange(D(2020, 7, 1), D(2020, 12, 31));
        var b = new DateRange(D(2020, 1, 1), D(2020, 6, 30));
        var result = a.Union(b);
        result.From.Should().Be(D(2020, 1, 1));
        result.To.Should().Be(D(2020, 12, 31));
    }

    [Fact]
    public void Equals_TwoDistinctEmptyRanges_AreEqual()
    {
        // All empty ranges denote the same (empty) set, regardless of their From/To endpoints.
        var emptyA = new DateRange(D(2030, 1, 1), D(2020, 1, 1));
        var emptyB = new DateRange(D(2025, 6, 1), D(2019, 1, 1));

        emptyA.IsEmpty.Should().BeTrue();
        emptyB.IsEmpty.Should().BeTrue();
        emptyA.Should().Be(emptyB);
        (emptyA == emptyB).Should().BeTrue();
        emptyA.GetHashCode().Should().Be(emptyB.GetHashCode());
    }

    [Fact]
    public void Equals_EmptyAndNonEmpty_AreNotEqual()
    {
        var empty = new DateRange(D(2030, 1, 1), D(2020, 1, 1));
        var nonEmpty = new DateRange(D(2020, 1, 1), D(2020, 1, 31));

        empty.Should().NotBe(nonEmpty);
        (empty == nonEmpty).Should().BeFalse();
    }

    [Fact]
    public void Equals_TwoEqualNonEmptyRanges_RemainEqual()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 6, 30));
        var b = new DateRange(D(2020, 1, 1), D(2020, 6, 30));

        a.Should().Be(b);
        (a == b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Minus_NonEmptyMinusEmpty_ReturnsOriginal()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 6, 30));
        var empty = new DateRange(D(2030, 1, 1), D(2020, 1, 1));

        a.Minus(empty).Should().ContainSingle().Which.Should().Be(a);
    }

    [Fact]
    public void Intersect_SameRange_ReturnsSelf()
    {
        var a = new DateRange(D(2020, 1, 1), D(2020, 6, 30));

        a.Intersect(a).Should().Be(a);
    }

    [Fact]
    public void Union_BothEmpty_ReturnsEmpty()
    {
        var emptyA = new DateRange(D(2030, 1, 1), D(2020, 1, 1));
        var emptyB = new DateRange(D(2025, 1, 1), D(2019, 1, 1));

        emptyA.Union(emptyB).IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void SingleDayRange_HasExpectedAlgebra()
    {
        var day = new DateRange(D(2020, 5, 10), D(2020, 5, 10));

        day.IsEmpty.Should().BeFalse();
        day.LengthInDays.Should().Be(1);
        day.Contains(D(2020, 5, 10)).Should().BeTrue();
        day.Contains(D(2020, 5, 11)).Should().BeFalse();
        day.Overlaps(day).Should().BeTrue();
        day.Intersect(day).Should().Be(day);
    }

    [Fact]
    public void FullRange_MinToMax_DoesNotOverflowAndContains()
    {
        var full = new DateRange(DateOnly.MinValue, DateOnly.MaxValue);

        full.IsEmpty.Should().BeFalse();
        full.LengthInDays.Should().Be(DateOnly.MaxValue.DayNumber - DateOnly.MinValue.DayNumber + 1);
        full.Contains(D(2020, 1, 1)).Should().BeTrue();
    }
}
