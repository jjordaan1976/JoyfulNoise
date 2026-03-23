using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class AccountHolderRepositoryTests
{
    private readonly Mock<IAccountHolderDataAccessObject> _daoMock;
    private readonly Mock<ILogger<AccountHolderRepository>> _loggerMock;
    private readonly AccountHolderRepository _sut;

    public AccountHolderRepositoryTests()
    {
        _daoMock    = new Mock<IAccountHolderDataAccessObject>();
        _loggerMock = new Mock<ILogger<AccountHolderRepository>>();
        _sut        = new AccountHolderRepository(_daoMock.Object, _loggerMock.Object);
    }

    // ── GetAccountHolderAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetAccountHolderAsync_WhenFound_ReturnsAccountHolder()
    {
        var expected = new AccountHolder { AccountHolderID = 1, FirstName = "Jane", LastName = "Doe" };
        _daoMock.Setup(d => d.GetAccountHolderAsync(1)).ReturnsAsync(expected);

        var result = await _sut.GetAccountHolderAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.AccountHolderID);
    }

    [Fact]
    public async Task GetAccountHolderAsync_WhenNotFound_ReturnsNull()
    {
        _daoMock.Setup(d => d.GetAccountHolderAsync(99)).ReturnsAsync((AccountHolder?)null);

        var result = await _sut.GetAccountHolderAsync(99);

        Assert.Null(result);
    }

    // ── GetByTeacherAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetByTeacherAsync_ReturnsDaoResult()
    {
        var list = new List<AccountHolder>
        {
            new() { AccountHolderID = 1, TeacherID = 5 },
            new() { AccountHolderID = 2, TeacherID = 5 }
        };
        _daoMock.Setup(d => d.GetByTeacherAsync(5)).ReturnsAsync(list);

        var result = await _sut.GetByTeacherAsync(5);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByTeacherAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        _daoMock.Setup(d => d.GetByTeacherAsync(5)).ReturnsAsync(Enumerable.Empty<AccountHolder>());

        var result = await _sut.GetByTeacherAsync(5);

        Assert.Empty(result);
    }

    // ── AddAccountHolderAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task AddAccountHolderAsync_WhenSuccessful_ReturnsNewId()
    {
        var ah = new AccountHolder { FirstName = "John", LastName = "Smith" };
        _daoMock.Setup(d => d.InsertAsync(ah)).ReturnsAsync(42);

        var result = await _sut.AddAccountHolderAsync(ah);

        Assert.Equal(42, result);
    }

    [Fact]
    public async Task AddAccountHolderAsync_WhenDaoThrows_ReturnsNull()
    {
        var ah = new AccountHolder { FirstName = "John", LastName = "Smith" };
        _daoMock.Setup(d => d.InsertAsync(ah)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddAccountHolderAsync(ah);

        Assert.Null(result);
    }

    // ── UpdateAccountHolderAsync ──────────────────────────────────────────────

    [Fact]
    public async Task UpdateAccountHolderAsync_WhenSuccessful_ReturnsTrue()
    {
        var ah = new AccountHolder { AccountHolderID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(ah)).ReturnsAsync(true);

        var result = await _sut.UpdateAccountHolderAsync(ah);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAccountHolderAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var ah = new AccountHolder { AccountHolderID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(ah)).ReturnsAsync(false);

        var result = await _sut.UpdateAccountHolderAsync(ah);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAccountHolderAsync_WhenDaoThrows_ReturnsFalse()
    {
        var ah = new AccountHolder { AccountHolderID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(ah)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateAccountHolderAsync(ah);

        Assert.False(result);
    }
}
