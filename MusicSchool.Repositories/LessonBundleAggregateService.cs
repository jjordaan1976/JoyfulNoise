using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models;
using System.Data;
namespace MusicSchool.Data.Implementations
{
    public class LessonBundleAggregateService : ILessonBundleAggregateService
    {
        private readonly IDbConnection _connection;
        private readonly ILessonBundleService _lessonBundleService;
        private readonly IBundleQuarterService _bundleQuarterService;
        private readonly IInvoiceService _invoiceService;

        public static readonly string SELECT_BUNDLE_WITH_QUARTERS_QRY = @"
            SELECT lb.BundleID,
                   lb.StudentID,
                   lb.TeacherID,
                   lb.LessonTypeID,
                   lb.TotalLessons,
                   lb.PricePerLesson,
                   lb.StartDate,
                   lb.EndDate,
                   lb.QuarterSize,
                   lb.Notes        AS BundleNotes,
                   s.FirstName     AS StudentFirstName,
                   s.LastName      AS StudentLastName,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson,
                   bq.QuarterID,
                   bq.QuarterNumber,
                   bq.LessonsAllocated,
                   bq.LessonsUsed,
                   bq.QuarterStartDate,
                   bq.QuarterEndDate
            FROM LessonBundle lb
            JOIN Student       s  ON s.StudentID     = lb.StudentID
            JOIN LessonType    lt ON lt.LessonTypeID = lb.LessonTypeID
            JOIN BundleQuarter bq ON bq.BundleID     = lb.BundleID
            WHERE lb.BundleID = @BundleID
            ORDER BY bq.QuarterNumber;";

        public static readonly string SELECT_BUNDLE_QRY_BY_STUDENT = @"
            SELECT lb.BundleID,
                   lb.StudentID,
                   lb.TeacherID,
                   lb.LessonTypeID,
                   lb.TotalLessons,
                   lb.PricePerLesson,
                   lb.StartDate,
                   lb.EndDate,
                   lb.QuarterSize,
                   lb.Notes        AS BundleNotes,
                   s.FirstName     AS StudentFirstName,
                   s.LastName      AS StudentLastName,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM LessonBundle lb
            JOIN Student       s  ON s.StudentID     = lb.StudentID
            JOIN LessonType    lt ON lt.LessonTypeID = lb.LessonTypeID
            WHERE s.StudentID = @StudentID
            ORDER BY lb.BundleID;";

        public LessonBundleAggregateService(
            IDbConnection connection,
            ILessonBundleService lessonBundleService,
            IBundleQuarterService bundleQuarterService,
            IInvoiceService invoiceService)
        {
            _connection = connection;
            _lessonBundleService = lessonBundleService;
            _bundleQuarterService = bundleQuarterService;
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Saves a new LessonBundle, its 4 BundleQuarter rows, and 12 monthly Invoice
        /// instalments — all in a single transaction. Returns the new BundleID.
        ///
        /// Invoice due dates are the 1st of each month starting from the month the
        /// bundle starts, running for 12 consecutive months.
        /// Amount per instalment = (TotalLessons × PricePerLesson) / 12, rounded to 2dp.
        /// </summary>
        public async Task<int> SaveNewBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // 1. Resolve AccountHolderID inside the transaction so we don't need
                //    a separate round-trip before opening the connection.
                var accountHolderId = await _connection.ExecuteScalarAsync<int>(
                    "SELECT AccountHolderID FROM Student WHERE StudentID = @StudentID",
                    new { bundle.StudentID }, transaction);

                if (accountHolderId == 0)
                    throw new InvalidOperationException(
                        $"Student {bundle.StudentID} not found when creating bundle.");

                // 2. Insert bundle
                var bundleId = await _lessonBundleService.InsertAsync(bundle, transaction);

                // 3. Insert quarters
                foreach (var quarter in quarters)
                    quarter.BundleID = bundleId;

                await _bundleQuarterService.InsertBatchAsync(quarters, transaction);

                // 4. Generate 12 monthly invoice instalments
                var instalmentAmount = Math.Round(bundle.TotalLessons * bundle.PricePerLesson / 12, 2);
                var invoices = BuildInstalments(bundleId, accountHolderId, instalmentAmount, bundle.StartDate);

                await _invoiceService.InsertBatchAsync(invoices, transaction, _connection);

                transaction.Commit();
                return bundleId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Returns a flat LessonBundleWithQuarterDetail row per quarter (4 rows total)
        /// for the given bundle, joining Student and LessonType for context.
        /// </summary>
        public async Task<IEnumerable<LessonBundleWithQuarterDetail>> GetBundleByIdAsync(int bundleId)
        {
            return await _connection.QueryAsync<LessonBundleWithQuarterDetail>(
                SELECT_BUNDLE_WITH_QUARTERS_QRY,
                new { BundleID = bundleId });
        }

        public async Task<IEnumerable<LessonBundleDetail>> GetBundleByStudentIdAsync(int studentId)
        {
            return await _connection.QueryAsync<LessonBundleDetail>(
                SELECT_BUNDLE_QRY_BY_STUDENT,
                new { StudentID = studentId });
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        /// <summary>
        /// Builds 12 Invoice rows for a bundle. DueDate is the 1st of each month,
        /// starting from the 1st of the month in which <paramref name="bundleStartDate"/> falls.
        /// </summary>
        private static IEnumerable<Invoice> BuildInstalments(
            int bundleId,
            int accountHolderId,
            decimal instalmentAmount,
            DateTime bundleStartDate)
        {
            // Anchor to the 1st of the start month regardless of the day the bundle starts.
            var firstDue = new DateTime(bundleStartDate.Year, bundleStartDate.Month, 1);

            for (byte i = 1; i <= 12; i++)
            {
                yield return new Invoice
                {
                    BundleID          = bundleId,
                    AccountHolderID   = accountHolderId,
                    InstallmentNumber = i,
                    Amount            = instalmentAmount,
                    DueDate           = firstDue.AddMonths(i - 1),
                    Status            = InvoiceStatus.Pending,
                };
            }
        }
    }
}
