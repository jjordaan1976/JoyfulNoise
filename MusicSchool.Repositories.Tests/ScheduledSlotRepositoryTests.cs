using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class ScheduledSlotRepositoryTests
{
    private readonly Mock<IScheduledSlotAggregateDataAccessObject> _aggregateMock;
    private readonly Mock<IScheduledSlotDataAccessObject> _slotDaoMock;
    private readonly Mock<ILogger<ScheduledSlotRepository>> _loggerMock;
    private readonly ScheduledSlotRepository _sut;

    public ScheduledSlotRepositoryTests()
    {
        _aggregateMock = new Mock<IScheduledSlotAggregateDataAccessObject>();
        _slotDaoMock   = new Mock<IScheduledSlotDataAccessObject>();
        _loggerMock    = new Mock<ILogger<ScheduledSlotRepository>>();
        _sut = new ScheduledSlotRepository(
            _aggregateMock.Object,
            _slotDaoMock.Object,
            _loggerMock.Object);
    }

    // ── GetSlotAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetSlotAsync_WhenFound_ReturnsSlot()
    {
        var expected = new ScheduledSlot { SlotID = 10, StudentID = 2, DayOfWeek = 1 };
        _slotDaoMock.Setup(d => d.GetSlotAsync(10)).ReturnsAsync(expected);

        var result = await _sut.GetSlotAsync(10);

        Assert.NotNull(result);
        Assert.Equal(10, result.SlotID);
    }

    [Fact]
    public async Task GetSlotAsync_WhenNotFound_ReturnsNull()
    {
        _slotDaoMock.Setup(d => d.GetSlotAsync(99)).ReturnsAsync((ScheduledSlot?)null);

        var result = await _sut.GetSlotAsync(99);

        Assert.Null(result);
    }

    // ── GetActiveByStudentAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetActiveByStudentAsync_ReturnsDaoResult()
    {
        var slots = new List<ScheduledSlot>
        {
            new() { SlotID = 1, StudentID = 3 },
            new() { SlotID = 2, StudentID = 3 }
        };
        _slotDaoMock.Setup(d => d.GetActiveByStudentAsync(3)).ReturnsAsync(slots);

        var result = await _sut.GetActiveByStudentAsync(3);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetActiveByStudentAsync_WhenNoSlots_ReturnsEmptyCollection()
    {
        _slotDaoMock.Setup(d => d.GetActiveByStudentAsync(3))
                    .ReturnsAsync(Enumerable.Empty<ScheduledSlot>());

        var result = await _sut.GetActiveByStudentAsync(3);

        Assert.Empty(result);
    }

    // ── GetActiveByTeacherAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetActiveByTeacherAsync_ReturnsDaoResult()
    {
        var slots = new List<ScheduledSlot>
        {
            new() { SlotID = 1, TeacherID = 5 },
            new() { SlotID = 2, TeacherID = 5 },
            new() { SlotID = 3, TeacherID = 5 }
        };
        _slotDaoMock.Setup(d => d.GetActiveByTeacherAsync(5)).ReturnsAsync(slots);

        var result = await _sut.GetActiveByTeacherAsync(5);

        Assert.Equal(3, result.Count());
    }

    // ── AddSlotAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task AddSlotAsync_WhenSuccessful_ReturnsNewSlotId()
    {
        var slot = new ScheduledSlot { StudentID = 1, TeacherID = 2, DayOfWeek = 3 };
        _aggregateMock.Setup(a => a.SaveNewSlotWithLessonsAsync(slot)).ReturnsAsync(20);

        var result = await _sut.AddSlotAsync(slot);

        Assert.Equal(20, result);
    }

    [Fact]
    public async Task AddSlotAsync_WhenNoBundleExists_LogsWarningAndReturnsNull()
    {
        var slot = new ScheduledSlot { StudentID = 1 };
        _aggregateMock.Setup(a => a.SaveNewSlotWithLessonsAsync(slot))
                      .ThrowsAsync(new InvalidOperationException("StudentID 1 has no active bundle"));

        var result = await _sut.AddSlotAsync(slot);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddSlotAsync_WhenGeneralExceptionThrown_ReturnsNull()
    {
        var slot = new ScheduledSlot { StudentID = 1 };
        _aggregateMock.Setup(a => a.SaveNewSlotWithLessonsAsync(slot))
                      .ThrowsAsync(new Exception("DB connection error"));

        var result = await _sut.AddSlotAsync(slot);

        Assert.Null(result);
    }

    // ── CloseSlotAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task CloseSlotAsync_WhenSuccessful_ReturnsTrue()
    {
        var effectiveTo = DateOnly.FromDateTime(DateTime.Today);
        _slotDaoMock.Setup(d => d.CloseSlotAsync(5, effectiveTo)).ReturnsAsync(true);

        var result = await _sut.CloseSlotAsync(5, effectiveTo);

        Assert.True(result);
    }

    [Fact]
    public async Task CloseSlotAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var effectiveTo = DateOnly.FromDateTime(DateTime.Today);
        _slotDaoMock.Setup(d => d.CloseSlotAsync(5, effectiveTo)).ReturnsAsync(false);

        var result = await _sut.CloseSlotAsync(5, effectiveTo);

        Assert.False(result);
    }

    [Fact]
    public async Task CloseSlotAsync_WhenDaoThrows_ReturnsFalse()
    {
        var effectiveTo = DateOnly.FromDateTime(DateTime.Today);
        _slotDaoMock.Setup(d => d.CloseSlotAsync(5, effectiveTo))
                    .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.CloseSlotAsync(5, effectiveTo);

        Assert.False(result);
    }
}
