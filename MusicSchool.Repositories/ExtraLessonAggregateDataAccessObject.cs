using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonAggregateDataAccessObject : IExtraLessonAggregateDataAccessObject
    {
        private readonly IDbConnection _connection;
        private readonly IExtraLessonDataAccessObject _extraLessonService;
        private readonly IInvoiceDataAccessObject _invoiceService;

        public static readonly string SELECT_EXTRA_LESSON_DETAIL_QRY = @"
            SELECT el.ExtraLessonID,
                   el.StudentID,
                   el.TeacherID,
                   el.LessonTypeID,
                   el.ScheduledDate,
                   el.ScheduledTime,
                   el.PriceCharged,
                   el.Status,
                   el.Notes,
                   s.FirstName       AS StudentFirstName,
                   s.LastName        AS StudentLastName,
                   t.Name            AS TeacherName,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM ExtraLesson  el
            JOIN Student       s ON s.StudentID     = el.StudentID
            JOIN Teacher       t ON t.TeacherID     = el.TeacherID
            JOIN LessonType   lt ON lt.LessonTypeID = el.LessonTypeID
            WHERE el.ExtraLessonID = @ExtraLessonID;";

        public static readonly string SELECT_EXTRA_LESSONS_BY_TEACHER_DATE_QRY = @"
            SELECT el.ExtraLessonID,
                   el.StudentID,
                   el.TeacherID,
                   el.LessonTypeID,
                   el.ScheduledDate,
                   el.ScheduledTime,
                   el.PriceCharged,
                   el.Status,
                   el.Notes,
                   s.FirstName       AS StudentFirstName,
                   s.LastName        AS StudentLastName,
                   t.Name            AS TeacherName,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM ExtraLesson  el
            JOIN Student       s ON s.StudentID     = el.StudentID
            JOIN Teacher       t ON t.TeacherID     = el.TeacherID
            JOIN LessonType   lt ON lt.LessonTypeID = el.LessonTypeID
            WHERE el.TeacherID    = @TeacherID
              AND el.ScheduledDate = @ScheduledDate
            ORDER BY el.ScheduledTime;";

        public ExtraLessonAggregateDataAccessObject(
            IDbConnection connection,
            IExtraLessonDataAccessObject extraLessonService,
            IInvoiceDataAccessObject invoiceService)
        {
            _connection = connection;
            _extraLessonService = extraLessonService;
            _invoiceService = invoiceService;
        }

        public async Task<ExtraLessonDetail?> GetExtraLessonByIdAsync(int extraLessonId)
        {
            return await _connection.QuerySingleOrDefaultAsync<ExtraLessonDetail>(
                SELECT_EXTRA_LESSON_DETAIL_QRY,
                new { ExtraLessonID = extraLessonId });
        }

        public async Task<IEnumerable<ExtraLessonDetail>> GetExtraLessonsByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
        {
            return await _connection.QueryAsync<ExtraLessonDetail>(
                SELECT_EXTRA_LESSONS_BY_TEACHER_DATE_QRY,
                new { TeacherID = teacherId, ScheduledDate = scheduledDate });
        }

        /// <summary>
        /// Inserts the ExtraLesson and a corresponding Invoice atomically.
        /// The Invoice is a single one-off row (InstallmentNumber = 1) for the full
        /// PriceCharged amount, due on the day of the lesson.
        /// Returns the new ExtraLessonID.
        /// </summary>
        public async Task<int> SaveNewExtraLessonAsync(ExtraLesson extraLesson)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // 1. Resolve the AccountHolderID for the student so we know who to bill.
                var accountHolderId = await _connection.ExecuteScalarAsync<int>(
                    "SELECT AccountHolderID FROM Student WHERE StudentID = @StudentID",
                    new { extraLesson.StudentID },
                    transaction);

                if (accountHolderId == 0)
                    throw new InvalidOperationException(
                        $"Student {extraLesson.StudentID} not found when creating extra lesson.");

                // 2. Insert the ExtraLesson row.
                var extraLessonId = await _extraLessonService.InsertAsync(extraLesson, transaction, _connection);

                // 3. Build and insert a single invoice for the full price, due on lesson day.
                var invoice = new Invoice
                {
                    BundleID          = null,
                    ExtraLessonID     = extraLessonId,
                    AccountHolderID   = accountHolderId,
                    InstallmentNumber = 1,
                    Amount            = extraLesson.PriceCharged,
                    DueDate           = extraLesson.ScheduledDate.Date,
                    Status            = InvoiceStatus.Pending,
                    Notes             = $"Extra lesson on {extraLesson.ScheduledDate:dd MMM yyyy}"
                };

                await _invoiceService.InsertAsync(invoice, transaction, _connection);

                transaction.Commit();
                return extraLessonId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
