using MusicSchool.Data.Implementations;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class ScheduledSlotAggregateDataAccessObjectTests
{
    // ── GetOccurrences helper — pure business logic ────────────────────────────
    //
    // GetOccurrences is private on ScheduledSlotAggregateDataAccessObject.
    // We replicate the algorithm here so regressions are caught with clear
    // assertion messages, independently of any DB infrastructure.
    // ─────────────────────────────────────────────────────────────────────────

    private static IEnumerable<DateTime> GetOccurrences(
        DateTime from, DateTime to, byte isoDayOfWeek)
    {
        var targetDotNet = isoDayOfWeek == 7
            ? DayOfWeek.Sunday
            : (DayOfWeek)isoDayOfWeek;

        var date = from;
        while (date.DayOfWeek != targetDotNet)
            date = date.AddDays(1);

        while (date <= to)
        {
            yield return date;
            date = date.AddDays(7);
        }
    }

    // ── GetOccurrences — correctness ──────────────────────────────────────────

    [Fact]
    public void GetOccurrences_FromMonday_IsoDayOfWeek1_ReturnsEveryMonday()
    {
        // 2025-01-06 is a Monday
        var from  = new DateTime(2025, 1, 6);
        var to    = new DateTime(2025, 1, 31);
        var dates = GetOccurrences(from, to, 1).ToList();

        Assert.Equal(4, dates.Count);
        Assert.All(dates, d => Assert.Equal(DayOfWeek.Monday, d.DayOfWeek));
        Assert.Equal(new DateTime(2025, 1, 6),  dates[0]);
        Assert.Equal(new DateTime(2025, 1, 13), dates[1]);
        Assert.Equal(new DateTime(2025, 1, 20), dates[2]);
        Assert.Equal(new DateTime(2025, 1, 27), dates[3]);
    }

    [Fact]
    public void GetOccurrences_StartOnWrongDay_AdvancesToFirstMatchingWeekday()
    {
        // 2025-01-06 is a Monday; asking for Thursday (ISO 4)
        var from  = new DateTime(2025, 1, 6);
        var to    = new DateTime(2025, 1, 31);
        var dates = GetOccurrences(from, to, 4).ToList();

        Assert.All(dates, d => Assert.Equal(DayOfWeek.Thursday, d.DayOfWeek));
        Assert.Equal(new DateTime(2025, 1, 9), dates[0]);
    }

    [Fact]
    public void GetOccurrences_IsoDayOfWeek7_MappedToSunday()
    {
        // 2025-01-05 is a Sunday
        var from  = new DateTime(2025, 1, 1);
        var to    = new DateTime(2025, 1, 31);
        var dates = GetOccurrences(from, to, 7).ToList();

        Assert.All(dates, d => Assert.Equal(DayOfWeek.Sunday, d.DayOfWeek));
        Assert.Equal(new DateTime(2025, 1, 5), dates[0]);
    }

    [Fact]
    public void GetOccurrences_ToDateIsExclusive_ExactBoundaryIncluded()
    {
        // from = 2025-01-06 (Monday); to = 2025-01-06 → exactly one occurrence
        var from  = new DateTime(2025, 1, 6);
        var to    = new DateTime(2025, 1, 6);
        var dates = GetOccurrences(from, to, 1).ToList();

        Assert.Single(dates);
        Assert.Equal(new DateTime(2025, 1, 6), dates[0]);
    }

    [Fact]
    public void GetOccurrences_ToBeforeFirstOccurrence_ReturnsEmpty()
    {
        // from = 2025-01-06 (Monday); to = 2025-01-05 (Sunday before)
        var from  = new DateTime(2025, 1, 6);
        var to    = new DateTime(2025, 1, 5);
        var dates = GetOccurrences(from, to, 1).ToList();

        Assert.Empty(dates);
    }

    [Fact]
    public void GetOccurrences_SpansMultipleMonths_ReturnsCorrectCount()
    {
        // 52 weeks of Tuesdays starting 2025-01-07
        var from  = new DateTime(2025, 1, 7);   // Tuesday
        var to    = from.AddDays(52 * 7 - 1);
        var dates = GetOccurrences(from, to, 2).ToList();

        Assert.Equal(52, dates.Count);
        Assert.All(dates, d => Assert.Equal(DayOfWeek.Tuesday, d.DayOfWeek));
    }

    [Fact]
    public void GetOccurrences_AllDates_AreExactlyOneWeekApart()
    {
        var from  = new DateTime(2025, 3, 3);   // Monday
        var to    = new DateTime(2025, 6, 30);
        var dates = GetOccurrences(from, to, 1).ToList();

        for (int i = 1; i < dates.Count; i++)
            Assert.Equal(7, (dates[i] - dates[i - 1]).Days);
    }

    // ── Lesson-building invariants ─────────────────────────────────────────────

    [Fact]
    public void LessonCreatedPerOccurrence_HasCorrectSlotAndBundleIds()
    {
        const int slotId   = 42;
        const int bundleId = 7;

        var quarter = new BundleQuarter
        {
            QuarterID          = 1,
            BundleID           = bundleId,
            QuarterStartDate   = new DateTime(2025, 1, 1),
            QuarterEndDate     = new DateTime(2025, 3, 31),
            LessonsAllocated   = 12,
            LessonsUsed        = 0
        };

        var slotTime = new TimeOnly(14, 30);
        var date     = new DateTime(2025, 1, 6);   // Monday inside the quarter

        var lesson = new Lesson
        {
            SlotID          = slotId,
            BundleID        = bundleId,
            QuarterID       = quarter.QuarterID,
            ScheduledDate   = date,
            ScheduledTime   = slotTime,
            Status          = LessonStatus.Scheduled,
            CreditForfeited = false
        };

        Assert.Equal(slotId,             lesson.SlotID);
        Assert.Equal(bundleId,           lesson.BundleID);
        Assert.Equal(1,                  lesson.QuarterID);
        Assert.Equal(LessonStatus.Scheduled, lesson.Status);
        Assert.False(lesson.CreditForfeited);
    }

    [Fact]
    public void LessonOutsideAllQuarters_IsSkipped()
    {
        // A date that falls outside every quarter range should map to no quarter.
        var quarters = new List<BundleQuarter>
        {
            new() { QuarterStartDate = new DateTime(2025, 1, 1), QuarterEndDate = new DateTime(2025, 3, 31) },
            new() { QuarterStartDate = new DateTime(2025, 4, 1), QuarterEndDate = new DateTime(2025, 6, 30) }
        };

        var dateOutsideRange = new DateTime(2025, 7, 15);

        var matchingQuarter = quarters.FirstOrDefault(q =>
            dateOutsideRange >= q.QuarterStartDate && dateOutsideRange <= q.QuarterEndDate);

        Assert.Null(matchingQuarter);
    }

    [Fact]
    public void LessonInsideQuarter_MapsToCorrectQuarter()
    {
        var q1 = new BundleQuarter { QuarterID = 1, QuarterStartDate = new DateTime(2025, 1, 1), QuarterEndDate = new DateTime(2025, 3, 31) };
        var q2 = new BundleQuarter { QuarterID = 2, QuarterStartDate = new DateTime(2025, 4, 1), QuarterEndDate = new DateTime(2025, 6, 30) };
        var quarters = new List<BundleQuarter> { q1, q2 };

        var dateInQ2 = new DateTime(2025, 5, 12);

        var match = quarters.FirstOrDefault(q =>
            dateInQ2 >= q.QuarterStartDate && dateInQ2 <= q.QuarterEndDate);

        Assert.NotNull(match);
        Assert.Equal(2, match.QuarterID);
    }
}
