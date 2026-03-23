using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class TeacherRepositoryTests
{
    private readonly Mock<ITeacherDataAccessObject> _daoMock;
    private readonly Mock<ILogger<TeacherRepository>> _loggerMock;
    private readonly TeacherRepository _sut;

    public TeacherRepositoryTests()
    {
        _daoMock    = new Mock<ITeacherDataAccessObject>();
        _loggerMock = new Mock<ILogger<TeacherRepository>>();
        _sut        = new TeacherRepository(_daoMock.Object, _loggerMock.Object);
    }

    // ── GetTeacherAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetTeacherAsync_WhenFound_ReturnsTeacher()
    {
        var expected = new Teacher { TeacherID = 1, Name = "Alice" };
        _daoMock.Setup(d => d.GetTeacherAsync(1)).ReturnsAsync(expected);

        var result = await _sut.GetTeacherAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
    }

    [Fact]
    public async Task GetTeacherAsync_WhenNotFound_ReturnsNull()
    {
        _daoMock.Setup(d => d.GetTeacherAsync(99)).ReturnsAsync((Teacher?)null);

        var result = await _sut.GetTeacherAsync(99);

        Assert.Null(result);
    }

    // ── GetAllActiveAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllActiveAsync_ReturnsDaoResult()
    {
        var teachers = new List<Teacher> { new() { TeacherID = 1 }, new() { TeacherID = 2 } };
        _daoMock.Setup(d => d.GetAllActiveAsync()).ReturnsAsync(teachers);

        var result = await _sut.GetAllActiveAsync();

        Assert.Equal(2, result.Count());
    }

    // ── AddTeacherAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task AddTeacherAsync_WhenSuccessful_ReturnsNewId()
    {
        var teacher = new Teacher { Name = "Bob" };
        _daoMock.Setup(d => d.InsertAsync(teacher)).ReturnsAsync(7);

        var result = await _sut.AddTeacherAsync(teacher);

        Assert.Equal(7, result);
    }

    [Fact]
    public async Task AddTeacherAsync_WhenDaoThrows_ReturnsNull()
    {
        var teacher = new Teacher { Name = "Bob" };
        _daoMock.Setup(d => d.InsertAsync(teacher)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddTeacherAsync(teacher);

        Assert.Null(result);
    }

    // ── UpdateTeacherAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateTeacherAsync_WhenSuccessful_ReturnsTrue()
    {
        var teacher = new Teacher { TeacherID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(teacher)).ReturnsAsync(true);

        var result = await _sut.UpdateTeacherAsync(teacher);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateTeacherAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var teacher = new Teacher { TeacherID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(teacher)).ReturnsAsync(false);

        var result = await _sut.UpdateTeacherAsync(teacher);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateTeacherAsync_WhenDaoThrows_ReturnsFalse()
    {
        var teacher = new Teacher { TeacherID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(teacher)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateTeacherAsync(teacher);

        Assert.False(result);
    }
}
