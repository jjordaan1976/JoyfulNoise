using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class LessonRepositoryTests
{
    private readonly Mock<ILessonAggregateDataAccessObject> _aggregateMock;
    private readonly Mock<ILessonDataAccessObject> _lessonDaoMock;
    private readonly Mock<IBundleQuarterDataAccessObject> _quarterDaoMock;
    private readonly Mock<ILogger<LessonRepository>> _loggerMock;
    private readonly LessonRepository _sut;

    public LessonRepositoryTests()
    {
        _aggregateMock  = new Mock<ILessonAggregateDataAccessObject>();
        _lessonDaoMock  = new Mock<ILessonDataAccessObject>();
        _quarterDaoMock = new Mock<IBundleQuarterDataAccessObject>();
        _loggerMock     = new Mock<ILogger<LessonRepository>>();
        _sut = new LessonRepository(
            _aggregateMock.Object,
            _lessonDaoMock.Object,
            _quarterDaoMock.Object,
            _loggerMock.Object);
    }

    // ── GetLessonAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetLessonAsync_WhenFound_ReturnsDetail()
    {
        var expected = new LessonDetail { LessonID = 10, TeacherName = "Alice" };
        _aggregateMock.Setup(a => a.GetLessonByIdAsync(10)).ReturnsAsync(expected);

        var result = await _sut.GetLessonAsync(10);

        Assert.NotNull(result);
        Assert.Equal(10, result.LessonID);
    }

    [Fact]
    public async Task GetLessonAsync_WhenNotFound_ReturnsNull()
    {
        _aggregateMock.Setup(a => a.GetLessonByIdAsync(99)).ReturnsAsync((LessonDetail?)null);

        var result = await _sut.GetLessonAsync(99);

        Assert.Null(result);
    }

    // ── GetByTeacherAndDateAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetByTeacherAndDateAsync_ReturnsDaoResult()
    {
        var date    = new DateTime(2025, 5, 12);
        var details = new List<LessonDetail> { new() { LessonID = 1 }, new() { LessonID = 2 } };
        _aggregateMock.Setup(a => a.GetLessonsByTeacherAndDateAsync(3, date)).ReturnsAsync(details);

        var result = await _sut.GetByTeacherAndDateAsync(3, date);

        Assert.Equal(2, result.Count());
    }

    // ── GetByBundleAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetByBundleAsync_ReturnsDaoResult()
    {
        var lessons = new List<Lesson> { new() { LessonID = 1 }, new() { LessonID = 2 } };
        _lessonDaoMock.Setup(d => d.GetByBundleAsync(5)).ReturnsAsync(lessons);

        var result = await _sut.GetByBundleAsync(5);

        Assert.Equal(2, result.Count());
    }

    // ── AddLessonAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task AddLessonAsync_WhenSuccessful_ReturnsNewId()
    {
        var lesson = new Lesson { BundleID = 1, QuarterID = 2 };
        _lessonDaoMock.Setup(d => d.InsertAsync(lesson)).ReturnsAsync(33);

        var result = await _sut.AddLessonAsync(lesson);

        Assert.Equal(33, result);
    }

    [Fact]
    public async Task AddLessonAsync_WhenDaoThrows_ReturnsNull()
    {
        var lesson = new Lesson { BundleID = 1 };
        _lessonDaoMock.Setup(d => d.InsertAsync(lesson)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddLessonAsync(lesson);

        Assert.Null(result);
    }

    // ── UpdateLessonStatusAsync — status transitions ───────────────────────────

    [Fact]
    public async Task UpdateLessonStatusAsync_WhenLessonNotFound_ReturnsFalse()
    {
        _lessonDaoMock.Setup(d => d.GetLessonAsync(1)).ReturnsAsync((Lesson?)null);

        var result = await _sut.UpdateLessonStatusAsync(
            1, LessonStatus.Completed, false, null, null, null);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateLessonStatusAsync_WhenUpdateFails_ReturnsFalse()
    {
        var lesson = new Lesson { LessonID = 1, Status = LessonStatus.Scheduled };
        _lessonDaoMock.Setup(d => d.GetLessonAsync(1)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.UpdateStatusAsync(
            1, LessonStatus.Completed, false, null, null, null, null))
            .ReturnsAsync(false);

        var result = await _sut.UpdateLessonStatusAsync(
            1, LessonStatus.Completed, false, null, null, null);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateLessonStatusAsync_ScheduledToCompleted_IncrementsQuarter()
    {
        // Scheduled → Completed: delta = +1
        var lesson = new Lesson { LessonID = 1, QuarterID = 10, Status = LessonStatus.Scheduled };
        _lessonDaoMock.Setup(d => d.GetLessonAsync(1)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.UpdateStatusAsync(
            1, LessonStatus.Completed, false, null, null, It.IsAny<DateTime?>(), null))
            .ReturnsAsync(true);
        _quarterDaoMock.Setup(d => d.AdjustLessonsUsedAsync(1, 1)).ReturnsAsync(true);

        var result = await _sut.UpdateLessonStatusAsync(
            1, LessonStatus.Completed, false, null, null, DateTime.UtcNow);

        Assert.True(result);
        _quarterDaoMock.Verify(d => d.AdjustLessonsUsedAsync(1, 1), Times.Once);
    }

    [Fact]
    public async Task UpdateLessonStatusAsync_CompletedToCancelledTeacher_DecrementsQuarter()
    {
        // Completed → CancelledTeacher: delta = -1
        var lesson = new Lesson { LessonID = 2, QuarterID = 10, Status = LessonStatus.Completed };
        _lessonDaoMock.Setup(d => d.GetLessonAsync(2)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.UpdateStatusAsync(
            2, LessonStatus.CancelledTeacher, false, CancelledBy.Teacher, null, null, null))
            .ReturnsAsync(true);
        _quarterDaoMock.Setup(d => d.AdjustLessonsUsedAsync(2, -1)).ReturnsAsync(true);

        var result = await _sut.UpdateLessonStatusAsync(
            2, LessonStatus.CancelledTeacher, false, CancelledBy.Teacher, null, null);

        Assert.True(result);
        _quarterDaoMock.Verify(d => d.AdjustLessonsUsedAsync(2, -1), Times.Once);
    }

    [Fact]
    public async Task UpdateLessonStatusAsync_ScheduledToCancelledTeacher_NoQuarterChange()
    {
        // Scheduled → CancelledTeacher: neither previously consumed, neither now — delta = 0
        var lesson = new Lesson { LessonID = 3, Status = LessonStatus.Scheduled };
        _lessonDaoMock.Setup(d => d.GetLessonAsync(3)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.UpdateStatusAsync(
            3, LessonStatus.CancelledTeacher, false, CancelledBy.Teacher, null, null, null))
            .ReturnsAsync(true);

        var result = await _sut.UpdateLessonStatusAsync(
            3, LessonStatus.CancelledTeacher, false, CancelledBy.Teacher, null, null);

        Assert.True(result);
        _quarterDaoMock.Verify(d => d.AdjustLessonsUsedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateLessonStatusAsync_WhenDaoThrows_ReturnsFalse()
    {
        _lessonDaoMock.Setup(d => d.GetLessonAsync(1)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateLessonStatusAsync(
            1, LessonStatus.Completed, false, null, null, null);

        Assert.False(result);
    }

    // ── RescheduleLessonAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task RescheduleLessonAsync_WhenCancelledTeacher_Succeeds()
    {
        var newDate = new DateTime(2025, 7, 1);
        var newTime = new TimeOnly(10, 0);
        var lesson  = new Lesson { LessonID = 4, Status = LessonStatus.CancelledTeacher };

        _lessonDaoMock.Setup(d => d.GetLessonAsync(4)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.RescheduleLessonAsync(4, newDate, newTime)).ReturnsAsync(true);

        var result = await _sut.RescheduleLessonAsync(4, newDate, newTime);

        Assert.True(result);
    }

    [Fact]
    public async Task RescheduleLessonAsync_WhenCancelledStudent_Succeeds()
    {
        var newDate = new DateTime(2025, 7, 2);
        var newTime = new TimeOnly(14, 0);
        var lesson  = new Lesson { LessonID = 5, Status = LessonStatus.CancelledStudent };

        _lessonDaoMock.Setup(d => d.GetLessonAsync(5)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.RescheduleLessonAsync(5, newDate, newTime)).ReturnsAsync(true);

        var result = await _sut.RescheduleLessonAsync(5, newDate, newTime);

        Assert.True(result);
    }

    [Fact]
    public async Task RescheduleLessonAsync_WhenStatusIsScheduled_ReturnsFalse()
    {
        var lesson = new Lesson { LessonID = 6, Status = LessonStatus.Scheduled };
        _lessonDaoMock.Setup(d => d.GetLessonAsync(6)).ReturnsAsync(lesson);

        var result = await _sut.RescheduleLessonAsync(6, DateTime.Today, new TimeOnly(9, 0));

        Assert.False(result);
        _lessonDaoMock.Verify(d => d.RescheduleLessonAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<TimeOnly>()), Times.Never);
    }

    [Fact]
    public async Task RescheduleLessonAsync_WhenLessonNotFound_ReturnsFalse()
    {
        _lessonDaoMock.Setup(d => d.GetLessonAsync(99)).ReturnsAsync((Lesson?)null);

        var result = await _sut.RescheduleLessonAsync(99, DateTime.Today, new TimeOnly(9, 0));

        Assert.False(result);
    }

    [Fact]
    public async Task RescheduleLessonAsync_WhenDaoThrows_ReturnsFalse()
    {
        _lessonDaoMock.Setup(d => d.GetLessonAsync(4)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.RescheduleLessonAsync(4, DateTime.Today, new TimeOnly(10, 0));

        Assert.False(result);
    }
}
