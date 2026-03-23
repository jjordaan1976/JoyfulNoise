using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Repositories.Tests;

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
    public async Task AddBundleAsync_WhenSuccessful_ReturnsNewBundleId()
    {
        var bundle   = new LessonBundle { StudentID = 3 };
        var quarters = new List<BundleQuarter> { new() { QuarterNumber = 1 } };
        _aggregateMock.Setup(a => a.SaveNewBundleAsync(bundle, quarters)).ReturnsAsync(7);

        var result = await _sut.AddBundleAsync(bundle, quarters);

        Assert.Equal(7, result);
    }

    [Fact]
    public async Task AddBundleAsync_WhenAggregateDaoThrows_ReturnsNull()
    {
        var bundle   = new LessonBundle { StudentID = 3 };
        var quarters = new List<BundleQuarter>();
        _aggregateMock.Setup(a => a.SaveNewBundleAsync(bundle, quarters))
                      .ThrowsAsync(new InvalidOperationException("Student not found"));

        var result = await _sut.AddBundleAsync(bundle, quarters);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddBundleAsync_WhenGeneralExceptionThrown_ReturnsNull()
    {
        var bundle   = new LessonBundle { StudentID = 3 };
        var quarters = new List<BundleQuarter>();
        _aggregateMock.Setup(a => a.SaveNewBundleAsync(bundle, quarters))
                      .ThrowsAsync(new Exception("DB connection error"));

        var result = await _sut.AddBundleAsync(bundle, quarters);

        Assert.Null(result);
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
    public async Task UpdateBundleAsync_WhenDaoThrows_ReturnsFalse()
    {
        var bundle = new LessonBundle { BundleID = 1 };
        _bundleDaoMock.Setup(d => d.UpdateAsync(bundle)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateBundleAsync(bundle);

        Assert.False(result);
    }
}
