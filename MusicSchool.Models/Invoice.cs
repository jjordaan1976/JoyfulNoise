using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Valid values for <see cref="Invoice.Status"/>.
    /// </summary>
    public static class InvoiceStatus
    {
        public const string Pending = "Pending";
        public const string Paid    = "Paid";
        public const string Overdue = "Overdue";
        public const string Void    = "Void";
    }

    /// <summary>
    /// One monthly instalment for a <see cref="LessonBundle"/>.
    /// A bundle produces 12 Invoice rows (InstallmentNumber 1–12).
    /// Amount = (TotalLessons * PricePerLesson) / 12,
    /// calculated and written by the application layer.
    /// </summary>
    public class Invoice
    {
        public int       InvoiceID         { get; set; }
        public int       BundleID          { get; set; }
        public int       AccountHolderID   { get; set; }

        /// <summary>
        /// Monthly instalment sequence number: 1–12.
        /// </summary>
        public byte      InstallmentNumber { get; set; }

        public decimal   Amount            { get; set; }
        public DateTime  DueDate           { get; set; }
        public DateTime? PaidDate          { get; set; }

        /// <summary>
        /// See <see cref="InvoiceStatus"/> for valid values.
        /// </summary>
        public string    Status            { get; set; } = InvoiceStatus.Pending;

        public string?   Notes             { get; set; }
        public DateTime  CreatedAt         { get; set; }
    }
}
