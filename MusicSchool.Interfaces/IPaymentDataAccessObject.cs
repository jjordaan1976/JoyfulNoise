using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IPaymentDataAccessObject
    {
        Task<Payment?> GetPaymentAsync(int id);
        Task<IEnumerable<Payment>> GetByAccountHolderAsync(int accountHolderId);

        /// <summary>Inserts a new payment row and returns the new PaymentID.</summary>
        Task<int> InsertAsync(Payment payment);

        /// <summary>Updates the UnallocatedAmount on an existing payment row.</summary>
        Task<bool> UpdateUnallocatedAsync(int paymentId, decimal unallocatedAmount);

        Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentAsync(int paymentId);
        Task<IEnumerable<PaymentAllocation>> GetAllocationsByInvoiceAsync(int invoiceId);

        /// <summary>Inserts a PaymentAllocation row.</summary>
        Task InsertAllocationAsync(PaymentAllocation allocation);

        /// <summary>
        /// Returns the sum of all unallocated amounts across all payments
        /// for the given account holder.
        /// </summary>
        Task<decimal> GetTotalUnallocatedAsync(int accountHolderId);
    }
}
