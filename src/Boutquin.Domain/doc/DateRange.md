# DateRange

**Namespace:** `Boutquin.Domain.ValueObjects`

`DateRange` is an immutable, inclusive range of `DateOnly` values with set-algebra
operations.

```csharp
var month = new DateRange(new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 31));
var week = new DateRange(new DateOnly(2026, 8, 24), new DateOnly(2026, 8, 30));

bool includesToday = month.Contains(new DateOnly(2026, 8, 26));
DateRange? shared = month.Intersect(week);
IReadOnlyList<DateRange> remaining = month.Minus(week);
```

A range with `From > To` is the empty range: it has length zero, overlaps nothing,
and is a subset of every range. All empty ranges compare equal and share a hash code.
`Intersect` returns null when the ranges do not overlap; `Minus` returns zero, one,
or two ranges.

`Union` returns the smallest enclosing range for overlapping or adjacent ranges. It
throws `ArgumentException` for non-empty ranges separated by a gap, because a single
`DateRange` cannot represent that union.
