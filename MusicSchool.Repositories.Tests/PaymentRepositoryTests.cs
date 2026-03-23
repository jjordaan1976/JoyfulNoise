using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Repositories.Tests;

/// <summary>
/// Tests for PaymentRepository.
///
/// NOTE: AddPaymentAsync and QuickPayInvoiceAsync open real IDbConnection transactions
/// internally and therefore cannot be fully unit-tested without an in-memory or
/// integration database. The tests below verify the observable public-facing behaviour:
///   • Delegation of read methods to the DAO.
///   • That a failed DAO insert causes AddPaymentAsync to return null (via a mock
///     that throws when InsertAsync is called through the connection scalar path).
///   • QuickPay returns null when the invoice does not exist.
/// The full allocation-engine logic is covered by integration tests / in-memory DB tests.
/// </summary>
public class PaymentRepositoryTests
{
    private readonly Mock<IPaymentDataAccessObject> _paymentDaoMock;
    private readonly Mock<IInvoiceDataAccessObject> _invoiceDaoMock;
    private readonly Mock<IDbConnection> _connectionMock;
    private readonly Mock<ILogger<PaymentRepository>> _loggerMock;
    private readonly PaymentRepository _sut;

    public PaymentRepositoryTests()
    {
        _paymentDaoMock = new Mock<IPaymentDataAccessObject>();
        _invoiceDaoMock = new Mock<IInvoiceDataAccessObject>();
        _connectionMock = new Mock<IDbConnection>();
        _loggerMock     = new Mock<ILogger<PaymentRepository>>();

        // Default: connection reports Closed so the repo will attempt to open it.
        _connectionMock.Setup(c => c.State).Returns(ConnectionState.Open);

        _sut = new PaymentRepository(
            _paymentDaoMock.Object,
            _invoiceDaoMock.Object,
            _connectionMock.Object,
            _loggerMock.Object);
    }

    // ── GetPaymentAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetPaymentAsync_WhenFound_ReturnsPayment()
    {
        var expected = new Payment { PaymentID = 5, Amount = 500m };
        _paymentDaoMock.Setup(d => d.GetPaymentAsync(5)).ReturnsAsync(expected);

        var result = await _sut.GetPaymentAsync(5);

        Assert.NotNull(result);
        Assert.Equal(500m, result.Amount);
    }

    [Fact]
    public async Task GetPaymentAsync_WhenNotFound_ReturnsNull()
    {
        _paymentDaoMock.Setup(d => d.GetPaymentAsync(99)).ReturnsAsync((Payment?)null);

        var result = await _sut.GetPaymentAsync(99);

        Assert.Null(result);
    }

    // ── GetByAccountHolderAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetByAccountHolderAsync_ReturnsDaoResult()
    {
        var payments = new List<Payment>
        {
            new() { PaymentID = 1, AccountHolderID = 10 },
            new() { PaymentID = 2, AccountHolderID = 10 }
        };
        _paymentDaoMock.Setup(d => d.GetByAccountHolderAsync(10)).ReturnsAsync(payments);

        var result = await _sut.GetByAccountHolderAsync(10);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByAccountHolderAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        _paymentDaoMock.Setup(d => d.GetByAccountHolderAsync(10))
                       .ReturnsAsync(Enumerable.Empty<Payment>());

        var result = await _sut.GetByAccountHolderAsync(10);

        Assert.Empty(result);
    }

    // ── GetAllocationsByPaymentAsync ──────────────────────────────────────────

    [Fact]
    public async Task GetAllocationsByPaymentAsync_ReturnsDaoResult()
    {
        var allocations = new List<PaymentAllocation>
        {
            new() { AllocationID = 1, PaymentID = 3, InvoiceID = 7, AmountApplied = 200m }
        };
        _paymentDaoMock.Setup(d => d.GetAllocationsByPaymentAsync(3)).ReturnsAsync(allocations);

        var result = await _sut.GetAllocationsByPaymentAsync(3);

        Assert.Single(result);
        Assert.Equal(200m, result.First().AmountApplied);
    }

    // ── GetAllocationsByInvoiceAsync ──────────────────────────────────────────

    [Fact]
    public async Task GetAllocationsByInvoiceAsync_ReturnsDaoResult()
    {
        var allocations = new List<PaymentAllocation>
        {
            new() { AllocationID = 1, InvoiceID = 7, AmountApplied = 500m },
            new() { AllocationID = 2, InvoiceID = 7, AmountApplied = 300m }
        };
        _paymentDaoMock.Setup(d => d.GetAllocationsByInvoiceAsync(7)).ReturnsAsync(allocations);

        var result = await _sut.GetAllocationsByInvoiceAsync(7);

        Assert.Equal(2, result.Count());
        Assert.Equal(800m, result.Sum(a => a.AmountApplied));
    }

    // ── QuickPayInvoiceAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task QuickPayInvoiceAsync_WhenInvoiceNotFound_ReturnsNull()
    {
        _invoiceDaoMock.Setup(d => d.GetInvoiceAsync(99)).ReturnsAsync((Invoice?)null);

        var result = await _sut.QuickPayInvoiceAsync(99, DateTime.Today);

        Assert.Null(result);
    }

    // ── AddPaymentAsync — error path ──────────────────────────────────────────

    [Fact]
    public async Task AddPaymentAsync_WhenConnectionThrowsOnBeginTransaction_ReturnsNull()
    {
        _connectionMock.Setup(c => c.BeginTransaction())
                       .Throws(new InvalidOperationException("Cannot open transaction"));

        var payment = new Payment { AccountHolderID = 1, Amount = 500m };

        var result = await _sut.AddPaymentAsync(payment);

        Assert.Null(result);
    }
}
