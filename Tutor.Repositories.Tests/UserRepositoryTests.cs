using Microsoft.Extensions.Logging;
using Moq;
using Tutor.Data.Implementations;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;

namespace Tutor.Repositories.Tests;

public class UserRepositoryTests
{
    private readonly Mock<IUserDataAccessObject> _daoMock;
    private readonly Mock<ILogger<UserRepository>> _loggerMock;
    private readonly UserRepository _sut;

    public UserRepositoryTests()
    {
        _daoMock    = new Mock<IUserDataAccessObject>();
        _loggerMock = new Mock<ILogger<UserRepository>>();
        _sut        = new UserRepository(_daoMock.Object, _loggerMock.Object);
    }

    // ── GetByEmailAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetByEmailAsync_WhenFound_ReturnsUsers()
    {
        var users = new List<User>
        {
            new() { UserID = 1, Email = "a@b.com", Role = UserRole.Student, StudentID = 5 },
            new() { UserID = 2, Email = "a@b.com", Role = UserRole.AccountHolder, AccountHolderID = 9 }
        };
        _daoMock.Setup(d => d.GetByEmailAsync("a@b.com")).ReturnsAsync(users);

        var result = await _sut.GetByEmailAsync("a@b.com");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByEmailAsync_WhenNoneFound_ReturnsEmpty()
    {
        _daoMock.Setup(d => d.GetByEmailAsync("x@y.com")).ReturnsAsync(Enumerable.Empty<User>());

        var result = await _sut.GetByEmailAsync("x@y.com");

        Assert.Empty(result);
    }

    // ── CreateUserAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task CreateUserAsync_WhenNew_InsertsAndReturnsId()
    {
        var user = new User { Email = "new@b.com", DisplayName = "New User", Role = UserRole.Teacher, TeacherID = 1 };
        _daoMock.Setup(d => d.ExistsAsync("new@b.com", UserRole.Teacher)).ReturnsAsync(false);
        _daoMock.Setup(d => d.InsertAsync(user)).ReturnsAsync(42);

        var result = await _sut.CreateUserAsync(user);

        Assert.Equal(42, result);
    }

    [Fact]
    public async Task CreateUserAsync_WhenAlreadyExists_DoesNotInsertAndReturnsNull()
    {
        var user = new User { Email = "dup@b.com", DisplayName = "Dup", Role = UserRole.Student, StudentID = 3 };
        _daoMock.Setup(d => d.ExistsAsync("dup@b.com", UserRole.Student)).ReturnsAsync(true);

        var result = await _sut.CreateUserAsync(user);

        Assert.Null(result);
        _daoMock.Verify(d => d.InsertAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_WhenEmailEmpty_DoesNotInsertAndReturnsNull()
    {
        var user = new User { Email = "", DisplayName = "No Email", Role = UserRole.Student, StudentID = 3 };

        var result = await _sut.CreateUserAsync(user);

        Assert.Null(result);
        _daoMock.Verify(d => d.InsertAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_WhenDaoThrows_Throws()
    {
        var user = new User { Email = "err@b.com", DisplayName = "Err", Role = UserRole.Teacher, TeacherID = 1 };
        _daoMock.Setup(d => d.ExistsAsync("err@b.com", UserRole.Teacher)).ReturnsAsync(false);
        _daoMock.Setup(d => d.InsertAsync(user)).ThrowsAsync(new Exception("DB error"));

        await Assert.ThrowsAsync<Exception>(() => _sut.CreateUserAsync(user));
    }
}
