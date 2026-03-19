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
    /// One monthly instalment for a <see cref="LessonBundle"/>, or a one-off invoice
    /// for an <see cref="ExtraLesson"/>.
    ///
    /// For bundle instalments: BundleID is set, ExtraLessonID is null,
    ///   InstallmentNumber runs 1–12, Amount = (TotalLessons * PricePerLesson) / 12.
    ///
    /// For extra-lesson invoices: ExtraLessonID is set, BundleID is null,
    ///   InstallmentNumber = 1, Amount = ExtraLesson.PriceCharged.
    /// </summary>
    public class Invoice
    {
        public int       InvoiceID         { get; set; }

        /// <summary>Populated for bundle instalments; null for extra-lesson invoices.</summary>
        public int?      BundleID          { get; set; }

        /// <summary>Populated for extra-lesson invoices; null for bundle instalments.</summary>
        public int?      ExtraLessonID     { get; set; }

        public int       AccountHolderID   { get; set; }

        /// <summary>
        /// Monthly instalment sequence number: 1–12 for bundle invoices; always 1 for extra lessons.
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
