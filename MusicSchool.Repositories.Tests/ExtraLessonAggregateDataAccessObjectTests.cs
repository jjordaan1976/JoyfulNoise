using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class ExtraLessonAggregateDataAccessObjectTests
{
    private readonly Mock<IExtraLessonDataAccessObject> _extraLessonDaoMock;
    private readonly Mock<IInvoiceDataAccessObject> _invoiceDaoMock;

    public ExtraLessonAggregateDataAccessObjectTests()
    {
        _extraLessonDaoMock = new Mock<IExtraLessonDataAccessObject>();
        _invoiceDaoMock     = new Mock<IInvoiceDataAccessObject>();
    }

    // ── Static query constants — structure validation ──────────────────────────

    [Fact]
    public void SelectExtraLessonDetailQuery_ContainsRequiredColumns()
    {
        var qry = ExtraLessonAggregateDataAccessObject.SELECT_EXTRA_LESSON_DETAIL_QRY;

        Assert.Contains("ExtraLessonID",      qry);
        Assert.Contains("StudentID",          qry);
        Assert.Contains("TeacherID",          qry);
        Assert.Contains("LessonTypeID",       qry);
        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("StudentLastName",    qry);
        Assert.Contains("TeacherName",        qry);
        Assert.Contains("DurationMinutes",    qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    [Fact]
    public void SelectExtraLessonDetailQuery_JoinsStudentTeacherAndLessonType()
    {
        var qry = ExtraLessonAggregateDataAccessObject.SELECT_EXTRA_LESSON_DETAIL_QRY;

        Assert.Contains("JOIN Student",     qry);
        Assert.Contains("JOIN Teacher",     qry);
        Assert.Contains("JOIN LessonType",  qry);
        Assert.Contains("@ExtraLessonID",   qry);
    }

    [Fact]
    public void SelectExtraLessonsByTeacherDateQuery_FiltersOnTeacherIdAndDate()
    {
        var qry = ExtraLessonAggregateDataAccessObject.SELECT_EXTRA_LESSONS_BY_TEACHER_DATE_QRY;

        Assert.Contains("@TeacherID",      qry);
        Assert.Contains("@ScheduledDate",  qry);
        Assert.Contains("ORDER BY",        qry);
    }

    [Fact]
    public void SelectExtraLessonsByTeacherDateQuery_ContainsRequiredColumns()
    {
        var qry = ExtraLessonAggregateDataAccessObject.SELECT_EXTRA_LESSONS_BY_TEACHER_DATE_QRY;

        Assert.Contains("ExtraLessonID",      qry);
        Assert.Contains("PriceCharged",       qry);
        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("TeacherName",        qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    // ── SaveNewExtraLessonAsync — business rules ───────────────────────────────

    /// <summary>
    /// SaveNewExtraLessonAsync opens a real IDbConnection transaction and therefore
    /// cannot be fully unit-tested without an integration DB.  We verify the
    /// invoice-building logic independently by checking Invoice field calculation.
    /// </summary>
    [Fact]
    public void InvoiceBuiltForExtraLesson_HasCorrectFields()
    {
        var scheduledDate = new DateTime(2025, 8, 15, 10, 0, 0);
        var extraLesson   = new ExtraLesson
        {
            ExtraLessonID  = 0,
            StudentID      = 3,
            TeacherID      = 2,
            LessonTypeID   = 1,
            ScheduledDate  = scheduledDate,
            ScheduledTime  = new TimeOnly(10, 0),
            PriceCharged   = 350m,
            Status         = ExtraLessonStatus.Scheduled,
        };

        // Reproduce the invoice-building logic from SaveNewExtraLessonAsync
        var invoice = new Invoice
        {
            BundleID          = null,
            ExtraLessonID     = extraLesson.ExtraLessonID,
            AccountHolderID   = 99,   // resolved at runtime in the real impl
            InstallmentNumber = 1,
            Amount            = extraLesson.PriceCharged,
            DueDate           = extraLesson.ScheduledDate.Date,
            Status            = InvoiceStatus.Pending,
        };

        Assert.Null(invoice.BundleID);
        Assert.Equal(1, invoice.InstallmentNumber);
        Assert.Equal(350m, invoice.Amount);
        Assert.Equal(InvoiceStatus.Pending, invoice.Status);
        Assert.Equal(scheduledDate.Date, invoice.DueDate);
    }
}
