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
namespace Boutquin.Domain.ValueObjects;

// DateRange is a foundation value object: a general-purpose inclusive span of calendar dates
// with total set-algebra. It lives in Boutquin.Domain so every Boutquin.* library shares one
// canonical range type (storage coverage windows, market-data request ranges, retention/audit
// filters) without any of them coupling to another library's value-object catalogue.

/// <summary>
/// Inclusive date range with set-algebra operations (contains, overlaps, intersect,
/// minus, union). A range with <see cref="From"/> &gt; <see cref="To"/> is treated as
/// the empty range — no exception is thrown at construction; consumers that require
/// well-formed ranges should validate at their API boundary.
/// </summary>
/// <param name="From">Inclusive lower bound. If greater than <paramref name="To"/> the range is empty.</param>
/// <param name="To">Inclusive upper bound.</param>
/// <remarks>
/// <para><b>Why a record struct:</b> <see cref="DateRange"/> is a value-semantics pair of
/// <see cref="DateOnly"/> with no identity of its own; it should be cheap to copy and
/// compare by value.</para>
/// <para><b>Why inclusive:</b> matches the calling convention of range scans
/// (<c>GetRange(from, to)</c> returns items whose dates are in <c>[from, to]</c>),
/// so consumers do not have to translate between half-open and closed intervals.</para>
/// <para><b>Empty range semantics:</b> the empty range is a subset of every range
/// (including itself), overlaps nothing, intersects to <c>null</c>, and has
/// <see cref="LengthInDays"/> of 0. This keeps algebra total without introducing an
/// option type at the API boundary.</para>
/// <para><b>Validation asymmetry:</b> constructors and most range algebra
/// (<see cref="Intersect(DateRange)"/>, <see cref="Contains(DateRange)"/>,
/// <see cref="Minus(DateRange)"/>, <see cref="Overlaps(DateRange)"/>) treat invalid
/// input (from &gt; to) as the empty range — the endpoints are retained as-is (no
/// normalisation occurs) and <see cref="IsEmpty"/> reports <see langword="true"/>.
/// <see cref="Union(DateRange)"/> is the exception: it rejects disjoint non-empty ranges
/// with <see cref="ArgumentException"/> because no single <see cref="DateRange"/> can
/// represent their union.</para>
/// </remarks>
public readonly record struct DateRange(DateOnly From, DateOnly To)
{
    /// <summary>
    /// Number of days in the inclusive range, or 0 if <see cref="From"/> &gt; <see cref="To"/>.
    /// </summary>
    public int LengthInDays => IsEmpty ? 0 : To.DayNumber - From.DayNumber + 1;

    /// <summary>
    /// True if <see cref="From"/> &gt; <see cref="To"/>, which represents the empty range.
    /// </summary>
    public bool IsEmpty => From > To;

    /// <summary>
    /// Determines set equality with another range. All empty ranges are equal to one another
    /// (every <c>From &gt; To</c> pair denotes the same empty set, regardless of endpoints); two
    /// non-empty ranges are equal when their bounds match.
    /// </summary>
    /// <remarks>
    /// The compiler-synthesized structural equality of the underlying <c>record struct</c> would treat
    /// two differently-bounded empty ranges (e.g. <c>[2030, 2020]</c> and <c>[2025, 2019]</c>) as
    /// unequal — a trap when ranges are used as dictionary/set keys. This override canonicalizes empties.
    /// </remarks>
    /// <param name="other">The range to compare with.</param>
    /// <returns><c>true</c> if the two ranges denote the same set of dates; otherwise <c>false</c>.</returns>
    public bool Equals(DateRange other)
    {
        if (IsEmpty || other.IsEmpty)
        {
            return IsEmpty && other.IsEmpty;
        }

        return From == other.From && To == other.To;
    }

    /// <summary>
    /// Returns a hash code consistent with <see cref="Equals(DateRange)"/>: all empty ranges share a
    /// single hash code, and non-empty ranges hash by their bounds.
    /// </summary>
    /// <returns>A hash code for the range.</returns>
    public override int GetHashCode()
        => IsEmpty ? 0 : HashCode.Combine(From, To);

    /// <summary>
    /// Returns true if <paramref name="date"/> falls within the inclusive range.
    /// </summary>
    /// <param name="date">The date to test for membership in this range.</param>
    /// <returns><see langword="true"/> if this range is non-empty and <paramref name="date"/> falls
    /// within <see cref="From"/> and <see cref="To"/> inclusive; otherwise <see langword="false"/>.</returns>
    public bool Contains(DateOnly date)
        => !IsEmpty && date >= From && date <= To;

    /// <summary>
    /// Returns true if this range fully contains <paramref name="other"/>.
    /// The empty range is a subset of every range; nothing non-empty is a subset of the empty range.
    /// </summary>
    /// <param name="other">The range to test for containment within this range.</param>
    /// <returns><see langword="true"/> if <paramref name="other"/> is empty, or if this range is
    /// non-empty and its bounds enclose <paramref name="other"/>'s; otherwise <see langword="false"/>.</returns>
    public bool Contains(DateRange other)
    {
        if (other.IsEmpty)
        {
            return true;
        }

        if (IsEmpty)
        {
            return false;
        }

        return other.From >= From && other.To <= To;
    }

    /// <summary>
    /// Returns true if the two ranges share at least one date. Empty ranges overlap nothing.
    /// </summary>
    /// <param name="other">The range to test for a shared date with this range.</param>
    /// <returns><see langword="true"/> if both ranges are non-empty and share at least one date;
    /// otherwise <see langword="false"/>.</returns>
    public bool Overlaps(DateRange other)
    {
        if (IsEmpty || other.IsEmpty)
        {
            return false;
        }

        return From <= other.To && other.From <= To;
    }

    /// <summary>
    /// Returns the intersection of the two ranges, or <c>null</c> if they are disjoint
    /// or either side is empty.
    /// </summary>
    /// <param name="other">The range to intersect with this range.</param>
    /// <returns>A <see cref="DateRange"/> covering the dates common to both ranges, or
    /// <see langword="null"/> if either range is empty or they share no date.</returns>
    public DateRange? Intersect(DateRange other)
    {
        if (!Overlaps(other))
        {
            return null;
        }

        var lo = From > other.From ? From : other.From;
        var hi = To < other.To ? To : other.To;
        return new DateRange(lo, hi);
    }

    /// <summary>
    /// Returns <c>this \ other</c> as 0, 1, or 2 ranges. If <paramref name="other"/> fully
    /// covers this range, the result is empty. If the two are disjoint, this range is returned
    /// unchanged.
    /// </summary>
    /// <remarks>
    /// Used by range gap-computation: <c>requested.Minus(coverage)</c> yields the prefix
    /// and/or suffix gaps that still need to be fetched.
    /// </remarks>
    /// <param name="other">The range to subtract from this range.</param>
    /// <returns>An empty list if this range is empty or fully covered by <paramref name="other"/>; a
    /// single-element list containing this range unchanged if the two ranges do not overlap; otherwise
    /// a list of 1 or 2 ranges covering the prefix and/or suffix of this range left after removing
    /// <paramref name="other"/>.</returns>
    public IReadOnlyList<DateRange> Minus(DateRange other)
    {
        if (IsEmpty)
        {
            return [];
        }

        if (!Overlaps(other))
        {
            return [this];
        }

        if (other.Contains(this))
        {
            return [];
        }

        var result = new List<DateRange>(2);

        // Prefix gap: this starts before other.
        if (From < other.From)
        {
            result.Add(new DateRange(From, DateOnly.FromDayNumber(other.From.DayNumber - 1)));
        }

        // Suffix gap: this ends after other.
        if (To > other.To)
        {
            result.Add(new DateRange(DateOnly.FromDayNumber(other.To.DayNumber + 1), To));
        }

        return result;
    }

    /// <summary>
    /// Returns the smallest enclosing range when the two ranges overlap or are adjacent
    /// (i.e., <c>other.From</c> = <c>this.To + 1</c> or vice versa). Throws
    /// <see cref="ArgumentException"/> when the ranges are disjoint with a gap between them —
    /// the union of disjoint ranges is not representable as a single range.
    /// </summary>
    /// <param name="other">The range to union with this range.</param>
    /// <returns><paramref name="other"/> if this range is empty; this range if <paramref name="other"/>
    /// is empty; otherwise the smallest single <see cref="DateRange"/> enclosing both.</returns>
    /// <exception cref="ArgumentException">Thrown when both ranges are non-empty, do not overlap, and
    /// are not adjacent — the gap between them is not representable as a single range.</exception>
    public DateRange Union(DateRange other)
    {
        if (IsEmpty)
        {
            return other;
        }

        if (other.IsEmpty)
        {
            return this;
        }

        var adjacent =
            To.DayNumber + 1 == other.From.DayNumber
            || other.To.DayNumber + 1 == From.DayNumber;

        if (!adjacent && !Overlaps(other))
        {
            throw new ArgumentException(
                "Cannot Union disjoint DateRanges — the gap between them is not representable as a single range.",
                nameof(other));
        }

        var lo = From < other.From ? From : other.From;
        var hi = To > other.To ? To : other.To;
        return new DateRange(lo, hi);
    }
}
