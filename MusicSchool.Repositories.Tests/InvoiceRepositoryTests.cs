using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Repositories.Tests;

public class InvoiceRepositoryTests
{
    private readonly Mock<IInvoiceDataAccessObject> _daoMock;
    private readonly Mock<ILogger<InvoiceRepository>> _loggerMock;
    private readonly InvoiceRepository _sut;

    public InvoiceRepositoryTests()
    {
        _daoMock    = new Mock<IInvoiceDataAccessObject>();
        _loggerMock = new Mock<ILogger<InvoiceRepository>>();
        _sut        = new InvoiceRepository(_daoMock.Object, _loggerMock.Object);
    }

    // ── GetInvoiceAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetInvoiceAsync_WhenFound_ReturnsInvoice()
    {
        var expected = new Invoice { InvoiceID = 5, Amount = 1000m, Status = InvoiceStatus.Pending };
        _daoMock.Setup(d => d.GetInvoiceAsync(5)).ReturnsAsync(expected);

        var result = await _sut.GetInvoiceAsync(5);

        Assert.NotNull(result);
        Assert.Equal(1000m, result.Amount);
    }

    [Fact]
    public async Task GetInvoiceAsync_WhenNotFound_ReturnsNull()
    {
        _daoMock.Setup(d => d.GetInvoiceAsync(99)).ReturnsAsync((Invoice?)null);

        var result = await _sut.GetInvoiceAsync(99);

        Assert.Null(result);
    }

    // ── GetByBundleAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetByBundleAsync_ReturnsDaoResult()
    {
        var invoices = Enumerable.Range(1, 12).Select(i => new Invoice
        {
            InvoiceID         = i,
            BundleID          = 3,
            InstallmentNumber = (byte)i
        }).ToList();

        _daoMock.Setup(d => d.GetByBundleAsync(3)).ReturnsAsync(invoices);

        var result = await _sut.GetByBundleAsync(3);

        Assert.Equal(12, result.Count());
    }

    // ── GetByAccountHolderAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetByAccountHolderAsync_ReturnsDaoResult()
    {
        var invoices = new List<Invoice>
        {
            new() { InvoiceID = 1, AccountHolderID = 10 },
            new() { InvoiceID = 2, AccountHolderID = 10 }
        };
        _daoMock.Setup(d => d.GetByAccountHolderAsync(10)).ReturnsAsync(invoices);

        var result = await _sut.GetByAccountHolderAsync(10);

        Assert.Equal(2, result.Count());
    }

    // ── GetOutstandingByAccountHolderAsync ────────────────────────────────────

    [Fact]
    public async Task GetOutstandingByAccountHolderAsync_ReturnsDaoResult()
    {
        var invoices = new List<Invoice>
        {
            new() { InvoiceID = 1, Status = InvoiceStatus.Pending },
            new() { InvoiceID = 2, Status = InvoiceStatus.Overdue }
        };
        _daoMock.Setup(d => d.GetOutstandingByAccountHolderAsync(10)).ReturnsAsync(invoices);

        var result = await _sut.GetOutstandingByAccountHolderAsync(10);

        Assert.Equal(2, result.Count());
        Assert.All(result, inv => Assert.NotEqual(InvoiceStatus.Paid, inv.Status));
    }

    // ── AddInvoiceInstalmentsAsync ────────────────────────────────────────────

    [Fact]
    public async Task AddInvoiceInstalmentsAsync_WhenSuccessful_ReturnsTrue()
    {
        var invoices = new List<Invoice> { new() { BundleID = 1 } };
        var connMock = new Mock<IDbConnection>();
        var txMock   = new Mock<IDbTransaction>();

        _daoMock.Setup(d => d.InsertBatchAsync(invoices, txMock.Object, connMock.Object))
                .Returns(Task.CompletedTask);

        var result = await _sut.AddInvoiceInstalmentsAsync(invoices, txMock.Object, connMock.Object);

        Assert.True(result);
    }

    [Fact]
    public async Task AddInvoiceInstalmentsAsync_WhenDaoThrows_ReturnsFalse()
    {
        var invoices = new List<Invoice> { new() { BundleID = 1 } };
        var connMock = new Mock<IDbConnection>();
        var txMock   = new Mock<IDbTransaction>();

        _daoMock.Setup(d => d.InsertBatchAsync(invoices, txMock.Object, connMock.Object))
                .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddInvoiceInstalmentsAsync(invoices, txMock.Object, connMock.Object);

        Assert.False(result);
    }

    // ── UpdateInvoiceStatusAsync ──────────────────────────────────────────────

    [Fact]
    public async Task UpdateInvoiceStatusAsync_WhenSuccessful_ReturnsTrue()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, InvoiceStatus.Paid, It.IsAny<DateOnly?>()))
                .ReturnsAsync(true);

        var result = await _sut.UpdateInvoiceStatusAsync(1, InvoiceStatus.Paid, DateOnly.FromDateTime(DateTime.Today));

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateInvoiceStatusAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, InvoiceStatus.Void, It.IsAny<DateOnly?>()))
                .ReturnsAsync(false);

        var result = await _sut.UpdateInvoiceStatusAsync(1, InvoiceStatus.Void, null);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateInvoiceStatusAsync_WhenDaoThrows_ReturnsFalse()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, InvoiceStatus.Paid, It.IsAny<DateOnly?>()))
                .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateInvoiceStatusAsync(1, InvoiceStatus.Paid, null);

        Assert.False(result);
    }
}
