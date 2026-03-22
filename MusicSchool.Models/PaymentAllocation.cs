using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Links a <see cref="Payment"/> to an <see cref="Invoice"/>.
    /// One payment can cover many invoices; one invoice can be covered by many payments
    /// (when unallocated amounts from earlier payments accumulate to reach the invoice total).
    /// </summary>
    public class PaymentAllocation
    {
        public int     AllocationID { get; set; }
        public int     PaymentID    { get; set; }
        public int     InvoiceID    { get; set; }

        /// <summary>Portion of this payment applied to this invoice.</summary>
        public decimal AmountApplied { get; set; }

        public DateTime CreatedAt   { get; set; }
    }
}
