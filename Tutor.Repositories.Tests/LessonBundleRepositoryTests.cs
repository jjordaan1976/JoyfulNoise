using Microsoft.Extensions.Logging;
using Moq;
using Tutor.Data.Implementations;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models;

namespace Tutor.Repositories.Tests;

public class LessonBundleRepositoryTests
{
    private readonly Mock<ILessonBundleAggregateDataAccessObject> _aggregateMock;
    private readonly Mock<ILessonBundleDataAccessObject> _bundleDaoMock;
    private readonly Mock<ILogger<LessonBundleRepository>> _loggerMock;
    private readonly LessonBundleRepository _sut;

    public LessonBundleRepositoryTests()
    {
        _aggregateMock = new Mock<ILessonBundleAggregateDataAccessObject>();
        _bundleDaoMock = new Mock<ILessonBundleDataAccessObject>();
        _loggerMock    = new Mock<ILogger<LessonBundleRepository>>();
        _sut = new LessonBundleRepository(
            _aggregateMock.Object,
            _bundleDaoMock.Object,
            _loggerMock.Object);
    }

    // ── GetBundleAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetBundleAsync_WhenFound_ReturnsQuarterDetails()
    {
        var rows = new List<LessonBundleWithQuarterDetail>
        {
            new() { BundleID = 1, QuarterNumber = 1 },
            new() { BundleID = 1, QuarterNumber = 2 },
            new() { BundleID = 1, QuarterNumber = 3 },
            new() { BundleID = 1, QuarterNumber = 4 }
        };
        _aggregateMock.Setup(a => a.GetBundleByIdAsync(1)).ReturnsAsync(rows);

        var result = await _sut.GetBundleAsync(1);

        Assert.Equal(4, result.Count());
    }

    [Fact]
    public async Task GetBundleAsync_WhenNotFound_ReturnsEmptyCollection()
    {
        _aggregateMock.Setup(a => a.GetBundleByIdAsync(99))
                      .ReturnsAsync(Enumerable.Empty<LessonBundleWithQuarterDetail>());

        var result = await _sut.GetBundleAsync(99);

        Assert.Empty(result);
    }

    // ── GetByStudentAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetByStudentAsync_ReturnsDaoResult()
    {
        var details = new List<LessonBundleDetail>
        {
            new() { BundleID = 1, StudentID = 5 },
            new() { BundleID = 2, StudentID = 5 }
        };
        _aggregateMock.Setup(a => a.GetBundleByStudentIdAsync(5)).ReturnsAsync(details);

        var result = await _sut.GetByStudentAsync(5);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByStudentAsync_WhenNoBundles_ReturnsEmptyCollection()
    {
        _aggregateMock.Setup(a => a.GetBundleByStudentIdAsync(5))
                      .ReturnsAsync(Enumerable.Empty<LessonBundleDetail>());

        var result = await _sut.GetByStudentAsync(5);

        Assert.Empty(result);
    }

    // ── AddBundleAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task AddBundleAsync_FullMode_JanuaryStart_CreatesFourFullQuarters()
    {
        var bundle = new LessonBundle { StudentID = 3, TotalLessons = 32, StartDate = new DateTime(2026, 1, 15) };
        List<BundleQuarter>? captured = null;
        _aggregateMock
            .Setup(a => a.SaveNewBundleAsync(bundle, It.IsAny<IEnumerable<BundleQuarter>>()))
            .Callback<LessonBundle, IEnumerable<BundleQuarter>>((_, q) => captured = q.ToList())
            .ReturnsAsync(7);

        var result = await _sut.AddBundleAsync(bundle, BundleCreationMode.Full);

        Assert.Equal(7, result);
        Assert.NotNull(captured);
        Assert.Equal(4, captured.Count);
        Assert.All(captured, q => Assert.Equal(8, q.LessonsAllocated));
        Assert.Equal(new DateTime(2026, 1, 15), captured[0].QuarterStartDate);
        Assert.Equal(new DateTime(2026, 3, 31), captured[0].QuarterEndDate);
        Assert.Equal(new DateTime(2026, 12, 31), captured[3].QuarterEndDate);
        Assert.Equal(32, bundle.TotalLessons);
        Assert.Equal(new DateTime(2026, 12, 31), bundle.EndDate);
    }

    [Fact]
    public async Task AddBundleAsync_FullMode_AfterFebruary_Throws()
    {
        var bundle = new LessonBundle { StudentID = 3, TotalLessons = 32, StartDate = new DateTime(2026, 3, 1) };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.AddBundleAsync(bundle, BundleCreationMode.Full));

        _aggregateMock.Verify(a => a.SaveNewBundleAsync(
            It.IsAny<LessonBundle>(), It.IsAny<IEnumerable<BundleQuarter>>()), Times.Never);
    }

    [Fact]
    public async Task AddBundleAsync_ProrataMode_JulyStart_CreatesTwoQuartersAtNormalRate()
    {
        var bundle = new LessonBundle { StudentID = 3, TotalLessons = 32, StartDate = new DateTime(2026, 7, 10) };
        List<BundleQuarter>? captured = null;
        _aggregateMock
            .Setup(a => a.SaveNewBundleAsync(bundle, It.IsAny<IEnumerable<BundleQuarter>>()))
            .Callback<LessonBundle, IEnumerable<BundleQuarter>>((_, q) => captured = q.ToList())
            .ReturnsAsync(8);

        var result = await _sut.AddBundleAsync(bundle, BundleCreationMode.Prorata);

        Assert.Equal(8, result);
        Assert.NotNull(captured);
        Assert.Equal(2, captured.Count);
        Assert.Equal(3, captured[0].QuarterNumber);
        Assert.Equal(4, captured[1].QuarterNumber);
        Assert.All(captured, q => Assert.Equal(8, q.LessonsAllocated));
        Assert.Equal(new DateTime(2026, 7, 10), captured[0].QuarterStartDate);
        Assert.Equal(new DateTime(2026, 9, 30), captured[0].QuarterEndDate);
        Assert.Equal(new DateTime(2026, 10, 1), captured[1].QuarterStartDate);
        Assert.Equal(new DateTime(2026, 12, 31), captured[1].QuarterEndDate);
        Assert.Equal(16, bundle.TotalLessons);
    }

    [Fact]
    public async Task AddBundleAsync_ProrataMode_FebruaryStart_KeepsAllFourQuarters()
    {
        var bundle = new LessonBundle { StudentID = 3, TotalLessons = 36, StartDate = new DateTime(2026, 2, 10) };
        List<BundleQuarter>? captured = null;
        _aggregateMock
            .Setup(a => a.SaveNewBundleAsync(bundle, It.IsAny<IEnumerable<BundleQuarter>>()))
            .Callback<LessonBundle, IEnumerable<BundleQuarter>>((_, q) => captured = q.ToList())
            .ReturnsAsync(9);

        await _sut.AddBundleAsync(bundle, BundleCreationMode.Prorata);

        Assert.NotNull(captured);
        Assert.Equal(4, captured.Count);
        Assert.All(captured, q => Assert.Equal(9, q.LessonsAllocated));
        Assert.Equal(new DateTime(2026, 2, 10), captured[0].QuarterStartDate);
        Assert.Equal(36, bundle.TotalLessons);
    }

    [Fact]
    public async Task AddBundleAsync_WhenTotalLessonsNotDivisibleByFour_Throws()
    {
        var bundle = new LessonBundle { StudentID = 3, TotalLessons = 30, StartDate = new DateTime(2026, 1, 1) };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.AddBundleAsync(bundle, BundleCreationMode.Full));
    }

    [Fact]
    public async Task AddBundleAsync_WhenAggregateDaoThrows_Throws()
    {
        var bundle = new LessonBundle { StudentID = 3, TotalLessons = 32, StartDate = new DateTime(2026, 1, 1) };
        _aggregateMock.Setup(a => a.SaveNewBundleAsync(bundle, It.IsAny<IEnumerable<BundleQuarter>>()))
                      .ThrowsAsync(new Exception("DB connection error"));

        await Assert.ThrowsAsync<Exception>(
            () => _sut.AddBundleAsync(bundle, BundleCreationMode.Full));
    }

    // ── UpdateBundleAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateBundleAsync_WhenSuccessful_ReturnsTrue()
    {
        var bundle = new LessonBundle { BundleID = 1 };
        _bundleDaoMock.Setup(d => d.UpdateAsync(bundle)).ReturnsAsync(true);

        var result = await _sut.UpdateBundleAsync(bundle);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateBundleAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var bundle = new LessonBundle { BundleID = 1 };
        _bundleDaoMock.Setup(d => d.UpdateAsync(bundle)).ReturnsAsync(false);

        var result = await _sut.UpdateBundleAsync(bundle);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateBundleAsync_WhenDaoThrows_Throws()
    {
        var bundle = new LessonBundle { BundleID = 1 };
        _bundleDaoMock.Setup(d => d.UpdateAsync(bundle)).ThrowsAsync(new Exception("DB error"));

        await Assert.ThrowsAsync<Exception>(() => _sut.UpdateBundleAsync(bundle));
    }
}
