using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class ExtraLessonRepositoryTests
{
    private readonly Mock<IExtraLessonAggregateDataAccessObject> _aggregateMock;
    private readonly Mock<IExtraLessonDataAccessObject> _daoMock;
    private readonly Mock<ILogger<ExtraLessonRepository>> _loggerMock;
    private readonly ExtraLessonRepository _sut;

    public ExtraLessonRepositoryTests()
    {
        _aggregateMock = new Mock<IExtraLessonAggregateDataAccessObject>();
        _daoMock       = new Mock<IExtraLessonDataAccessObject>();
        _loggerMock    = new Mock<ILogger<ExtraLessonRepository>>();
        _sut           = new ExtraLessonRepository(_aggregateMock.Object, _daoMock.Object, _loggerMock.Object);
    }

    // ── GetExtraLessonAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task GetExtraLessonAsync_WhenFound_ReturnsDetail()
    {
        var expected = new ExtraLessonDetail { ExtraLessonID = 7, TeacherName = "Alice" };
        _aggregateMock.Setup(a => a.GetExtraLessonByIdAsync(7)).ReturnsAsync(expected);

        var result = await _sut.GetExtraLessonAsync(7);

        Assert.NotNull(result);
        Assert.Equal("Alice", result.TeacherName);
    }

    [Fact]
    public async Task GetExtraLessonAsync_WhenNotFound_ReturnsNull()
    {
        _aggregateMock.Setup(a => a.GetExtraLessonByIdAsync(99)).ReturnsAsync((ExtraLessonDetail?)null);

        var result = await _sut.GetExtraLessonAsync(99);

        Assert.Null(result);
    }

    // ── GetByTeacherAndDateAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetByTeacherAndDateAsync_ReturnsDaoResult()
    {
        var date    = new DateTime(2025, 6, 2);
        var details = new List<ExtraLessonDetail>
        {
            new() { ExtraLessonID = 1 },
            new() { ExtraLessonID = 2 }
        };
        _aggregateMock.Setup(a => a.GetExtraLessonsByTeacherAndDateAsync(5, date)).ReturnsAsync(details);

        var result = await _sut.GetByTeacherAndDateAsync(5, date);

        Assert.Equal(2, result.Count());
    }

    // ── GetByStudentAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetByStudentAsync_ReturnsDaoResult()
    {
        var lessons = new List<ExtraLesson>
        {
            new() { ExtraLessonID = 1, StudentID = 3 },
            new() { ExtraLessonID = 2, StudentID = 3 }
        };
        _daoMock.Setup(d => d.GetByStudentAsync(3)).ReturnsAsync(lessons);

        var result = await _sut.GetByStudentAsync(3);

        Assert.Equal(2, result.Count());
    }

    // ── AddExtraLessonAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task AddExtraLessonAsync_WhenSuccessful_ReturnsNewId()
    {
        var el = new ExtraLesson { StudentID = 1, TeacherID = 2 };
        _aggregateMock.Setup(a => a.SaveNewExtraLessonAsync(el)).ReturnsAsync(88);

        var result = await _sut.AddExtraLessonAsync(el);

        Assert.Equal(88, result);
    }

    [Fact]
    public async Task AddExtraLessonAsync_WhenAggregateDaoThrows_ReturnsNull()
    {
        var el = new ExtraLesson { StudentID = 1 };
        _aggregateMock.Setup(a => a.SaveNewExtraLessonAsync(el))
                      .ThrowsAsync(new InvalidOperationException("No student found"));

        var result = await _sut.AddExtraLessonAsync(el);

        Assert.Null(result);
    }

    // ── UpdateExtraLessonStatusAsync ──────────────────────────────────────────

    [Fact]
    public async Task UpdateExtraLessonStatusAsync_WhenSuccessful_ReturnsTrue()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, ExtraLessonStatus.Completed, null))
                .ReturnsAsync(true);

        var result = await _sut.UpdateExtraLessonStatusAsync(1, ExtraLessonStatus.Completed);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateExtraLessonStatusAsync_WithNote_PassesNoteThrough()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, ExtraLessonStatus.Cancelled, "Weather"))
                .ReturnsAsync(true);

        var result = await _sut.UpdateExtraLessonStatusAsync(1, ExtraLessonStatus.Cancelled, "Weather");

        Assert.True(result);
        _daoMock.Verify(d => d.UpdateStatusAsync(1, ExtraLessonStatus.Cancelled, "Weather"), Times.Once);
    }

    [Fact]
    public async Task UpdateExtraLessonStatusAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, ExtraLessonStatus.Cancelled, null))
                .ReturnsAsync(false);

        var result = await _sut.UpdateExtraLessonStatusAsync(1, ExtraLessonStatus.Cancelled);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateExtraLessonStatusAsync_WhenDaoThrows_ReturnsFalse()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, ExtraLessonStatus.Completed, null))
                .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateExtraLessonStatusAsync(1, ExtraLessonStatus.Completed);

        Assert.False(result);
    }
}
