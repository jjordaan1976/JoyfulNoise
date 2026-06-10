using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class LessonBundleAggregateDataAccessObject : ILessonBundleAggregateDataAccessObject
    {
        private readonly IDbConnection _connection;
        private readonly ILessonBundleDataAccessObject _lessonBundleService;
        private readonly IBundleQuarterDataAccessObject _bundleQuarterService;
        private readonly IInvoiceDataAccessObject _invoiceService;

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

        public LessonBundleAggregateDataAccessObject(
            IDbConnection connection,
            ILessonBundleDataAccessObject lessonBundleService,
            IBundleQuarterDataAccessObject bundleQuarterService,
            IInvoiceDataAccessObject invoiceService)
        {
            _connection = connection;
            _lessonBundleService = lessonBundleService;
            _bundleQuarterService = bundleQuarterService;
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Saves a new LessonBundle, its 4 BundleQuarter rows, and prorated monthly Invoice
        /// instalments — all in a single transaction on the same connection.
        /// </summary>
        /// <param name="bundle">Bundle to save. StartDate is used as the lesson start date for prorating.</param>
        /// <param name="quarters">Quarter structure (dates/numbers). LessonsAllocated is overwritten with prorated values.</param>
        /// <param name="selectedBundleLessons">The full-year bundle size selected (e.g. 32, 48). Prorated lessons and invoices are derived from this.</param>
        public async Task<int> SaveNewBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters, int selectedBundleLessons)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // 1. Prorate lessons and invoice count based on lesson start date.
                var (proratedLessons, monthCount) = CalculateProrata(selectedBundleLessons, bundle.StartDate);
                bundle.TotalLessons = proratedLessons;

                // Distribute prorated lessons evenly across quarters; remainder goes to last quarter.
                var quarterList   = quarters.ToList();
                var baseAlloc     = proratedLessons / quarterList.Count;
                var remainder     = proratedLessons % quarterList.Count;
                for (int q = 0; q < quarterList.Count; q++)
                    quarterList[q].LessonsAllocated = baseAlloc + (q == quarterList.Count - 1 ? remainder : 0);

                // 2. Resolve AccountHolderID inside the transaction.
                var accountHolderId = await _connection.ExecuteScalarAsync<int>(
                    "SELECT AccountHolderID FROM Student WHERE StudentID = @StudentID",
                    new { bundle.StudentID }, transaction);

                if (accountHolderId == 0)
                    throw new InvalidOperationException(
                        $"Student {bundle.StudentID} not found when creating bundle.");

                // 3. Insert bundle
                var bundleId = await _lessonBundleService.InsertAsync(bundle, transaction);

                // 4. Insert quarters — pass _connection explicitly so the INSERT runs on
                //    the same connection that owns the transaction. Without this, Dapper
                //    uses the service's injected connection which is a different instance,
                //    causing the quarters to be inserted outside the transaction or not at
                //    all, leaving LessonsAllocated = 0 and breaking slot/lesson creation.
                foreach (var quarter in quarterList)
                    quarter.BundleID = bundleId;

                await _bundleQuarterService.InsertBatchAsync(quarterList, transaction, _connection);

                // 5. Generate prorated monthly invoice instalments.
                var instalmentAmount = Math.Round(proratedLessons * bundle.PricePerLesson / monthCount, 2);
                var invoices = BuildInstalments(bundleId, accountHolderId, instalmentAmount, bundle.StartDate, monthCount);

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
        /// Calculates prorated lessons and invoice month count for a mid-year start.
        /// Months remaining = from startDate.Month to December inclusive (13 - startDate.Month).
        /// </summary>
        public static (int ProratedLessons, int MonthCount) CalculateProrata(
            int selectedBundleLessons,
            DateTime lessonStartDate)
        {
            var monthCount     = 13 - lessonStartDate.Month;
            var proratedLessons = (int)Math.Round(selectedBundleLessons * monthCount / 12.0);
            return (proratedLessons, monthCount);
        }

        private static IEnumerable<Invoice> BuildInstalments(
            int bundleId,
            int accountHolderId,
            decimal instalmentAmount,
            DateTime bundleStartDate,
            int instalmentCount)
        {
            var firstDue = new DateTime(bundleStartDate.Year, bundleStartDate.Month, 1);

            for (byte i = 1; i <= instalmentCount; i++)
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
