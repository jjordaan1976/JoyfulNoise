using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Valid values for <see cref="Payment.Source"/>.
    /// </summary>
    public static class PaymentSource
    {
        public const string Manual   = "Manual";   // Teacher entered an amount
        public const string QuickPay = "QuickPay"; // Teacher clicked "Paid" on an invoice
    }

    /// <summary>
    /// Records a payment received from an account holder.
    /// A payment may be fully allocated (all linked to invoices),
    /// partially allocated (some unallocated remainder), or fully unallocated.
    ///
    /// The allocation engine distributes the amount against the oldest
    /// outstanding invoices first (chronological DueDate order).
    /// Any remainder below the next invoice's amount is stored as unallocated
    /// on this row (UnallocatedAmount) and is reconsidered when further
    /// payments arrive for the same account holder.
    /// </summary>
    public class Payment
    {
        public int       PaymentID       { get; set; }
        public int       AccountHolderID { get; set; }

        /// <summary>Total rand amount received.</summary>
        public decimal   Amount          { get; set; }

        /// <summary>Portion not yet linked to any invoice.</summary>
        public decimal   UnallocatedAmount { get; set; }

        public DateTime  PaymentDate     { get; set; }

        /// <summary>See <see cref="PaymentSource"/> for valid values.</summary>
        public string    Source          { get; set; } = PaymentSource.Manual;

        public string?   Reference       { get; set; }
        public string?   Notes           { get; set; }
        public DateTime  CreatedAt       { get; set; }
    }
}
