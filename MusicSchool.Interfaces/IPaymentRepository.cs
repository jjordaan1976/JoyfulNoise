using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetPaymentAsync(int id);
        Task<IEnumerable<Payment>> GetByAccountHolderAsync(int accountHolderId);
        Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentAsync(int paymentId);
        Task<IEnumerable<PaymentAllocation>> GetAllocationsByInvoiceAsync(int invoiceId);

        /// <summary>
        /// Records a new payment and immediately runs the allocation engine:
        /// links the payment to as many outstanding invoices (oldest-first) as
        /// the amount covers, marks those invoices as Paid, and stores any
        /// remainder as UnallocatedAmount on the Payment row.
        ///
        /// Also sweeps existing unallocated amounts from prior payments so
        /// they contribute toward the next invoice when accumulated funds are
        /// sufficient.
        ///
        /// Returns the new PaymentID, or null on failure.
        /// </summary>
        Task<int?> AddPaymentAsync(Payment payment);

        /// <summary>
        /// Creates a QuickPay payment exactly equal to the invoice amount,
        /// links it to that invoice, and marks the invoice Paid.
        /// Returns the new PaymentID, or null on failure.
        /// </summary>
        Task<int?> QuickPayInvoiceAsync(int invoiceId, DateTime paymentDate);
    }
}
