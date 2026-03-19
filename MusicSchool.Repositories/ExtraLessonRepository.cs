using Dapper;
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonRepository : IExtraLessonRepository
    {
        private readonly IExtraLessonAggregateService _aggregateService;
        private readonly IExtraLessonService _extraLessonService;
        private readonly IInvoiceService _invoiceService;
        private readonly IDbConnection _connection;
        private readonly ILogger<ExtraLessonRepository> _logger;

        public ExtraLessonRepository(
            IExtraLessonAggregateService aggregateService,
            IExtraLessonService extraLessonService,
            IInvoiceService invoiceService,
            IDbConnection connection,
            ILogger<ExtraLessonRepository> logger)
        {
            _aggregateService = aggregateService;
            _extraLessonService = extraLessonService;
            _invoiceService = invoiceService;
            _connection = connection;
            _logger = logger;
        }

        /// <summary>
        /// Returns a single extra lesson with full context (student, teacher, lesson type).
        /// </summary>
        public async Task<ExtraLessonDetail?> GetExtraLessonAsync(int extraLessonId)
        {
            return await _aggregateService.GetExtraLessonByIdAsync(extraLessonId);
        }

        /// <summary>
        /// Returns all extra lessons for a teacher on a given date, with full context.
        /// </summary>
        public async Task<IEnumerable<ExtraLessonDetail>> GetByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
        {
            return await _aggregateService.GetExtraLessonsByTeacherAndDateAsync(teacherId, scheduledDate);
        }

        public async Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId)
        {
            return await _extraLessonService.GetByStudentAsync(studentId);
        }

        /// <summary>
        /// Inserts the ExtraLesson and a corresponding Invoice atomically.
        /// The Invoice is a single one-off row (InstallmentNumber = 1) for the full
        /// PriceCharged amount, due on the day of the lesson.
        /// Returns the new ExtraLessonID, or null if the operation fails.
        /// </summary>
        public async Task<int?> AddExtraLessonAsync(ExtraLesson extraLesson)
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
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex,
                    "Failed to insert ExtraLesson + Invoice for StudentID {StudentID} on {ScheduledDate}",
                    extraLesson.StudentID, extraLesson.ScheduledDate);
                return null;
            }
        }

        public async Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status)
        {
            try
            {
                return await _extraLessonService.UpdateStatusAsync(extraLessonId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to update status for ExtraLessonID {ExtraLessonID}", extraLessonId);
                return false;
            }
        }
    }
}
