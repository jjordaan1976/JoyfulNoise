using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class StudentRepositoryTests
{
    private readonly Mock<IStudentDataAccessObject> _daoMock;
    private readonly Mock<ILogger<StudentRepository>> _loggerMock;
    private readonly StudentRepository _sut;

    public StudentRepositoryTests()
    {
        _daoMock    = new Mock<IStudentDataAccessObject>();
        _loggerMock = new Mock<ILogger<StudentRepository>>();
        _sut        = new StudentRepository(_daoMock.Object, _loggerMock.Object);
    }

    // ── GetStudentAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetStudentAsync_WhenFound_ReturnsStudent()
    {
        var expected = new Student { StudentID = 3, FirstName = "Tom", LastName = "Jones" };
        _daoMock.Setup(d => d.GetStudentAsync(3)).ReturnsAsync(expected);

        var result = await _sut.GetStudentAsync(3);

        Assert.NotNull(result);
        Assert.Equal(3, result.StudentID);
    }

    [Fact]
    public async Task GetStudentAsync_WhenNotFound_ReturnsNull()
    {
        _daoMock.Setup(d => d.GetStudentAsync(99)).ReturnsAsync((Student?)null);

        var result = await _sut.GetStudentAsync(99);

        Assert.Null(result);
    }

    // ── GetByAccountHolderAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetByAccountHolderAsync_ReturnsDaoResult()
    {
        var students = new List<Student>
        {
            new() { StudentID = 1, AccountHolderID = 10 },
            new() { StudentID = 2, AccountHolderID = 10 }
        };
        _daoMock.Setup(d => d.GetByAccountHolderAsync(10)).ReturnsAsync(students);

        var result = await _sut.GetByAccountHolderAsync(10);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByAccountHolderAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        _daoMock.Setup(d => d.GetByAccountHolderAsync(10)).ReturnsAsync(Enumerable.Empty<Student>());

        var result = await _sut.GetByAccountHolderAsync(10);

        Assert.Empty(result);
    }

    // ── AddStudentAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task AddStudentAsync_WhenSuccessful_ReturnsNewId()
    {
        var student = new Student { FirstName = "Anna", LastName = "Bell" };
        _daoMock.Setup(d => d.InsertAsync(student)).ReturnsAsync(55);

        var result = await _sut.AddStudentAsync(student);

        Assert.Equal(55, result);
    }

    [Fact]
    public async Task AddStudentAsync_WhenDaoThrows_ReturnsNull()
    {
        var student = new Student { FirstName = "Anna", LastName = "Bell" };
        _daoMock.Setup(d => d.InsertAsync(student)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddStudentAsync(student);

        Assert.Null(result);
    }

    // ── UpdateStudentAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStudentAsync_WhenSuccessful_ReturnsTrue()
    {
        var student = new Student { StudentID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(student)).ReturnsAsync(true);

        var result = await _sut.UpdateStudentAsync(student);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateStudentAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var student = new Student { StudentID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(student)).ReturnsAsync(false);

        var result = await _sut.UpdateStudentAsync(student);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateStudentAsync_WhenDaoThrows_ReturnsFalse()
    {
        var student = new Student { StudentID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(student)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateStudentAsync(student);

        Assert.False(result);
    }
}
