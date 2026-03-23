using MusicSchool.Data.Implementations;

namespace MusicSchool.Repositories.Tests;

public class LessonAggregateDataAccessObjectTests
{
    // ── Static query constants — structure validation ──────────────────────────

    [Fact]
    public void SelectLessonDetailQuery_ContainsAllLessonColumns()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSON_DETAIL_QRY;

        Assert.Contains("LessonID",           qry);
        Assert.Contains("SlotID",             qry);
        Assert.Contains("BundleID",           qry);
        Assert.Contains("QuarterID",          qry);
        Assert.Contains("ScheduledDate",      qry);
        Assert.Contains("ScheduledTime",      qry);
        Assert.Contains("Status",             qry);
        Assert.Contains("CreditForfeited",    qry);
        Assert.Contains("CancelledBy",        qry);
        Assert.Contains("CancellationReason", qry);
        Assert.Contains("OriginalLessonID",   qry);
        Assert.Contains("CompletedAt",        qry);
    }

    [Fact]
    public void SelectLessonDetailQuery_ContainsStudentAndTeacherAndLessonTypeColumns()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSON_DETAIL_QRY;

        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("StudentLastName",    qry);
        Assert.Contains("TeacherName",        qry);
        Assert.Contains("DurationMinutes",    qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    [Fact]
    public void SelectLessonDetailQuery_JoinsScheduledSlotStudentTeacherLessonType()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSON_DETAIL_QRY;

        Assert.Contains("JOIN ScheduledSlot", qry);
        Assert.Contains("JOIN Student",       qry);
        Assert.Contains("JOIN Teacher",       qry);
        Assert.Contains("JOIN LessonType",    qry);
        Assert.Contains("@LessonID",          qry);
    }

    [Fact]
    public void SelectLessonsByTeacherDateQuery_FiltersOnTeacherIdAndDate()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSONS_BY_TEACHER_DATE_QRY;

        Assert.Contains("@TeacherID",     qry);
        Assert.Contains("@ScheduledDate", qry);
        Assert.Contains("ORDER BY",       qry);
    }

    [Fact]
    public void SelectLessonsByTeacherDateQuery_ContainsRequiredColumns()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSONS_BY_TEACHER_DATE_QRY;

        Assert.Contains("LessonID",           qry);
        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("StudentLastName",    qry);
        Assert.Contains("TeacherName",        qry);
        Assert.Contains("DurationMinutes",    qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    [Fact]
    public void SelectLessonsByTeacherDateQuery_JoinsScheduledSlotStudentTeacherLessonType()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSONS_BY_TEACHER_DATE_QRY;

        Assert.Contains("JOIN ScheduledSlot", qry);
        Assert.Contains("JOIN Student",       qry);
        Assert.Contains("JOIN Teacher",       qry);
        Assert.Contains("JOIN LessonType",    qry);
    }
}
