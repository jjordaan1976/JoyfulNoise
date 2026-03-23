using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class LessonTypeRepositoryTests
{
    private readonly Mock<ILessonTypeDataAccessObject> _daoMock;
    private readonly Mock<ILogger<LessonTypeRepository>> _loggerMock;
    private readonly LessonTypeRepository _sut;

    public LessonTypeRepositoryTests()
    {
        _daoMock    = new Mock<ILessonTypeDataAccessObject>();
        _loggerMock = new Mock<ILogger<LessonTypeRepository>>();
        _sut        = new LessonTypeRepository(_daoMock.Object, _loggerMock.Object);
    }

    // ── GetLessonTypeAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task GetLessonTypeAsync_WhenFound_ReturnsLessonType()
    {
        var expected = new LessonType { LessonTypeID = 2, DurationMinutes = 45 };
        _daoMock.Setup(d => d.GetLessonTypeAsync(2)).ReturnsAsync(expected);

        var result = await _sut.GetLessonTypeAsync(2);

        Assert.NotNull(result);
        Assert.Equal(45, result.DurationMinutes);
    }

    [Fact]
    public async Task GetLessonTypeAsync_WhenNotFound_ReturnsNull()
    {
        _daoMock.Setup(d => d.GetLessonTypeAsync(99)).ReturnsAsync((LessonType?)null);

        var result = await _sut.GetLessonTypeAsync(99);

        Assert.Null(result);
    }

    // ── GetAllActiveAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllActiveAsync_ReturnsDaoResult()
    {
        var list = new List<LessonType>
        {
            new() { LessonTypeID = 1, DurationMinutes = 30 },
            new() { LessonTypeID = 2, DurationMinutes = 45 },
            new() { LessonTypeID = 3, DurationMinutes = 60 }
        };
        _daoMock.Setup(d => d.GetAllActiveAsync()).ReturnsAsync(list);

        var result = await _sut.GetAllActiveAsync();

        Assert.Equal(3, result.Count());
    }

    // ── AddLessonTypeAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task AddLessonTypeAsync_WhenSuccessful_ReturnsNewId()
    {
        var lt = new LessonType { DurationMinutes = 60, BasePricePerLesson = 200 };
        _daoMock.Setup(d => d.InsertAsync(lt)).ReturnsAsync(10);

        var result = await _sut.AddLessonTypeAsync(lt);

        Assert.Equal(10, result);
    }

    [Fact]
    public async Task AddLessonTypeAsync_WhenDaoThrows_ReturnsNull()
    {
        var lt = new LessonType { DurationMinutes = 60 };
        _daoMock.Setup(d => d.InsertAsync(lt)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddLessonTypeAsync(lt);

        Assert.Null(result);
    }

    // ── UpdateLessonTypeAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task UpdateLessonTypeAsync_WhenSuccessful_ReturnsTrue()
    {
        var lt = new LessonType { LessonTypeID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(lt)).ReturnsAsync(true);

        var result = await _sut.UpdateLessonTypeAsync(lt);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateLessonTypeAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var lt = new LessonType { LessonTypeID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(lt)).ReturnsAsync(false);

        var result = await _sut.UpdateLessonTypeAsync(lt);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateLessonTypeAsync_WhenDaoThrows_ReturnsFalse()
    {
        var lt = new LessonType { LessonTypeID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(lt)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateLessonTypeAsync(lt);

        Assert.False(result);
    }
}
