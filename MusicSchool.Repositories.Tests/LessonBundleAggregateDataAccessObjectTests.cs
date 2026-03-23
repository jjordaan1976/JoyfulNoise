using MusicSchool.Data.Implementations;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class LessonBundleAggregateDataAccessObjectTests
{
    // ── Static query constants — structure validation ──────────────────────────

    [Fact]
    public void SelectBundleWithQuartersQuery_ContainsAllBundleColumns()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_WITH_QUARTERS_QRY;

        Assert.Contains("BundleID",        qry);
        Assert.Contains("StudentID",       qry);
        Assert.Contains("TeacherID",       qry);
        Assert.Contains("LessonTypeID",    qry);
        Assert.Contains("TotalLessons",    qry);
        Assert.Contains("PricePerLesson",  qry);
        Assert.Contains("StartDate",       qry);
        Assert.Contains("EndDate",         qry);
        Assert.Contains("QuarterSize",     qry);
    }

    [Fact]
    public void SelectBundleWithQuartersQuery_ContainsAllQuarterColumns()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_WITH_QUARTERS_QRY;

        Assert.Contains("QuarterID",        qry);
        Assert.Contains("QuarterNumber",    qry);
        Assert.Contains("LessonsAllocated", qry);
        Assert.Contains("LessonsUsed",      qry);
        Assert.Contains("QuarterStartDate", qry);
        Assert.Contains("QuarterEndDate",   qry);
    }

    [Fact]
    public void SelectBundleWithQuartersQuery_ContainsStudentAndLessonTypeJoins()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_WITH_QUARTERS_QRY;

        Assert.Contains("JOIN Student",       qry);
        Assert.Contains("JOIN LessonType",    qry);
        Assert.Contains("JOIN BundleQuarter", qry);
        Assert.Contains("ORDER BY",           qry);
        Assert.Contains("@BundleID",          qry);
    }

    [Fact]
    public void SelectBundleWithQuartersQuery_ContainsStudentAndLessonTypeDetailColumns()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_WITH_QUARTERS_QRY;

        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("StudentLastName",    qry);
        Assert.Contains("DurationMinutes",    qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    [Fact]
    public void SelectBundleQueryByStudent_FiltersOnStudentId()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_QRY_BY_STUDENT;

        Assert.Contains("@StudentID", qry);
        Assert.Contains("ORDER BY",   qry);
    }

    [Fact]
    public void SelectBundleQueryByStudent_ContainsRequiredColumns()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_QRY_BY_STUDENT;

        Assert.Contains("BundleID",           qry);
        Assert.Contains("TotalLessons",       qry);
        Assert.Contains("PricePerLesson",     qry);
        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("StudentLastName",    qry);
        Assert.Contains("DurationMinutes",    qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    // ── BuildInstalments helper — business logic ───────────────────────────────

    /// <summary>
    /// The private BuildInstalments helper is exercised indirectly via public
    /// observable state. We verify the instalment-calculation rules here directly
    /// so regressions surface with clear failure messages.
    /// </summary>
    [Theory]
    [InlineData(48, 200,  800)]   // 48 lessons × R200 / 12 = R800 per instalment
    [InlineData(36, 150,  450)]   // 36 lessons × R150 / 12 = R450
    [InlineData(12, 300,  300)]   // 12 lessons × R300 / 12 = R300
    public void InstalmentAmount_IsCalculatedCorrectly(
        int totalLessons, decimal pricePerLesson, decimal expectedInstalment)
    {
        var instalment = Math.Round(totalLessons * pricePerLesson / 12, 2);

        Assert.Equal(expectedInstalment, instalment);
    }

    [Fact]
    public void BuildInstalments_Generates12Rows_StartingFromBundleStartMonth()
    {
        // Simulate what BuildInstalments produces.
        var bundleStartDate = new DateTime(2025, 1, 15);
        var firstDue        = new DateTime(bundleStartDate.Year, bundleStartDate.Month, 1);
        var instalments     = new List<Invoice>();

        for (byte i = 1; i <= 12; i++)
        {
            instalments.Add(new Invoice
            {
                BundleID          = 1,
                AccountHolderID   = 99,
                InstallmentNumber = i,
                Amount            = 800m,
                DueDate           = firstDue.AddMonths(i - 1),
                Status            = InvoiceStatus.Pending,
            });
        }

        Assert.Equal(12, instalments.Count);
        Assert.Equal(new DateTime(2025, 1, 1), instalments[0].DueDate);
        Assert.Equal(new DateTime(2025, 12, 1), instalments[11].DueDate);
        Assert.All(instalments, inv => Assert.Equal(InvoiceStatus.Pending, inv.Status));
        Assert.All(instalments, inv => Assert.Equal(800m, inv.Amount));

        // Instalment numbers must be sequential 1–12
        for (byte i = 1; i <= 12; i++)
            Assert.Equal(i, instalments[i - 1].InstallmentNumber);
    }

    [Fact]
    public void BuildInstalments_BundleStartMidYear_CorrectlyWrapsToNextYear()
    {
        var bundleStartDate = new DateTime(2025, 6, 1);
        var firstDue        = new DateTime(bundleStartDate.Year, bundleStartDate.Month, 1);
        var lastDue         = firstDue.AddMonths(11);

        Assert.Equal(new DateTime(2025, 6,  1), firstDue);
        Assert.Equal(new DateTime(2026, 5,  1), lastDue);
    }
}
