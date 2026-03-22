# Flattened Codebase

Generated: 03/22/2026 14:44:30


## File: AccountHolderDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class AccountHolderDataAccessObject : IAccountHolderDataAccessObject
    {
        private readonly IDbConnection _connection;

        public AccountHolderDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(int id)
        {
            const string sql = @"
                SELECT AccountHolderID,
                       TeacherID,
                       FirstName,
                       LastName,
                       Email,
                       Phone,
                       BillingAddress,
                       IsActive,
                       CreatedAt
                FROM AccountHolder
                WHERE AccountHolderID = @AccountHolderID;";

            return await _connection.QuerySingleOrDefaultAsync<AccountHolder>(sql, new { AccountHolderID = id });
        }

        public async Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId)
        {
            const string sql = @"
                SELECT AccountHolderID,
                       TeacherID,
                       FirstName,
                       LastName,
                       Email,
                       Phone,
                       BillingAddress,
                       IsActive,
                       CreatedAt
                FROM AccountHolder
                WHERE TeacherID = @TeacherID
                  AND IsActive  = 1
                ORDER BY LastName, FirstName;";

            return await _connection.QueryAsync<AccountHolder>(sql, new { TeacherID = teacherId });
        }

        public async Task<int> InsertAsync(AccountHolder accountHolder)
        {
            const string sql = @"
                INSERT INTO AccountHolder
                    (TeacherID, FirstName, LastName, Email, Phone, BillingAddress, IsActive)
                VALUES
                    (@TeacherID, @FirstName, @LastName, @Email, @Phone, @BillingAddress, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, accountHolder);
        }

        public async Task<bool> UpdateAsync(AccountHolder accountHolder)
        {
            const string sql = @"
                UPDATE AccountHolder
                SET TeacherID      = @TeacherID,
                    FirstName      = @FirstName,
                    LastName       = @LastName,
                    Email          = @Email,
                    Phone          = @Phone,
                    BillingAddress = @BillingAddress,
                    IsActive       = @IsActive
                WHERE AccountHolderID = @AccountHolderID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, accountHolder);
            return rowsAffected > 0;
        }
    }
}

```

## File: AccountHolderRepository.cs

```csharp

using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class AccountHolderRepository : IAccountHolderRepository
    {
        private readonly IAccountHolderDataAccessObject _accountHolderService;
        private readonly ILogger<AccountHolderRepository> _logger;

        public AccountHolderRepository(IAccountHolderDataAccessObject accountHolderService, ILogger<AccountHolderRepository> logger)
        {
            _accountHolderService = accountHolderService;
            _logger = logger;
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(int id)
        {
            return await _accountHolderService.GetAccountHolderAsync(id);
        }

        public async Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId)
        {
            return await _accountHolderService.GetByTeacherAsync(teacherId);
        }

        public async Task<int?> AddAccountHolderAsync(AccountHolder accountHolder)
        {
            try
            {
                return await _accountHolderService.InsertAsync(accountHolder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert AccountHolder {FirstName} {LastName}",
                    accountHolder.FirstName, accountHolder.LastName);
                return null;
            }
        }

        public async Task<bool> UpdateAccountHolderAsync(AccountHolder accountHolder)
        {
            try
            {
                return await _accountHolderService.UpdateAsync(accountHolder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update AccountHolderID {AccountHolderID}",
                    accountHolder.AccountHolderID);
                return false;
            }
        }
    }
}

```

## File: BundleQuarterDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class BundleQuarterDataAccessObject : IBundleQuarterDataAccessObject
    {
        private readonly IDbConnection _connection;

        public BundleQuarterDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<BundleQuarter>> GetByBundleAsync(int bundleId)
        {
            const string sql = @"
                SELECT QuarterID,
                       BundleID,
                       QuarterNumber,
                       LessonsAllocated,
                       LessonsUsed,
                       QuarterStartDate,
                       QuarterEndDate
                FROM BundleQuarter
                WHERE BundleID = @BundleID
                ORDER BY QuarterNumber;";

            return await _connection.QueryAsync<BundleQuarter>(sql, new { BundleID = bundleId });
        }

        /// <summary>
        /// Inserts a batch of quarters within an existing transaction.
        /// The connection is passed explicitly so the INSERT runs on the same
        /// connection that owns the transaction — avoiding cross-connection issues
        /// that would cause LessonsAllocated to be 0 or the INSERT to fail silently.
        /// </summary>
        public async Task InsertBatchAsync(IEnumerable<BundleQuarter> quarters, IDbTransaction tx, IDbConnection connection)
        {
            const string sql = @"
                INSERT INTO BundleQuarter
                    (BundleID, QuarterNumber, LessonsAllocated, LessonsUsed,
                     QuarterStartDate, QuarterEndDate)
                VALUES
                    (@BundleID, @QuarterNumber, @LessonsAllocated, @LessonsUsed,
                     @QuarterStartDate, @QuarterEndDate);";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, quarters, tx));
        }

        public async Task<bool> UpdateLessonsUsedAsync(int quarterId, int lessonsUsed)
        {
            const string sql = @"
                UPDATE BundleQuarter
                SET LessonsUsed = @LessonsUsed
                WHERE QuarterID = @QuarterID;";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { QuarterID = quarterId, LessonsUsed = lessonsUsed });
            return rowsAffected > 0;
        }

        /// <summary>
        /// Atomically adjusts LessonsUsed for the quarter that owns the given lesson.
        /// Pass +1 when a lesson is completed or forfeited, -1 when that is reversed.
        /// Clamps to zero so LessonsUsed never goes negative.
        /// </summary>
        public async Task<bool> AdjustLessonsUsedAsync(int lessonId, int delta)
        {
            const string sql = @"
                UPDATE BundleQuarter
                SET LessonsUsed = CASE
                                      WHEN LessonsUsed + @Delta < 0 THEN 0
                                      ELSE LessonsUsed + @Delta
                                  END
                WHERE QuarterID = (
                    SELECT QuarterID FROM Lesson WHERE LessonID = @LessonID
                );";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { LessonID = lessonId, Delta = delta });
            return rowsAffected > 0;
        }
    }
}

```

## File: ExtraLessonAggregateDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonAggregateDataAccessObject : IExtraLessonAggregateDataAccessObject
    {
        private readonly IDbConnection _connection;

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

        public ExtraLessonAggregateDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
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
    }
}

```

## File: ExtraLessonDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonDataAccessObject : IExtraLessonDataAccessObject
    {
        private readonly IDbConnection _connection;

        public ExtraLessonDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ExtraLesson?> GetExtraLessonAsync(int id)
        {
            const string sql = @"
                SELECT ExtraLessonID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       ScheduledDate,
                       ScheduledTime,
                       PriceCharged,
                       Status,
                       Notes,
                       CreatedAt
                FROM ExtraLesson
                WHERE ExtraLessonID = @ExtraLessonID;";

            return await _connection.QuerySingleOrDefaultAsync<ExtraLesson>(sql, new { ExtraLessonID = id });
        }

        public async Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId)
        {
            const string sql = @"
                SELECT ExtraLessonID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       ScheduledDate,
                       ScheduledTime,
                       PriceCharged,
                       Status,
                       Notes,
                       CreatedAt
                FROM ExtraLesson
                WHERE StudentID = @StudentID
                ORDER BY ScheduledDate DESC, ScheduledTime DESC;";

            return await _connection.QueryAsync<ExtraLesson>(sql, new { StudentID = studentId });
        }

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        public async Task<int> InsertAsync(ExtraLesson extraLesson)
            => await InsertAsync(extraLesson, null!, _connection);

        /// <summary>Inserts within an existing transaction.</summary>
        public async Task<int> InsertAsync(ExtraLesson extraLesson, IDbTransaction tx, IDbConnection connection)
        {
            const string sql = @"
                INSERT INTO ExtraLesson
                    (StudentID, TeacherID, LessonTypeID, ScheduledDate,
                     ScheduledTime, PriceCharged, Status, Notes)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, @ScheduledDate,
                     @ScheduledTime, @PriceCharged, @Status, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, extraLesson, tx));
        }

        /// <summary>
        /// Updates the status on an extra lesson row.
        /// <paramref name="note"/> is optional; when null the existing Notes value is preserved.
        /// </summary>
        public async Task<bool> UpdateStatusAsync(int extraLessonId, string status, string? note = null)
        {
            const string sql = @"
                UPDATE ExtraLesson
                SET Status = @Status,
                    Notes  = COALESCE(@Notes, Notes)
                WHERE ExtraLessonID = @ExtraLessonID;";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { ExtraLessonID = extraLessonId, Status = status, Notes = note });
            return rowsAffected > 0;
        }
    }
}

```

## File: ExtraLessonRepository.cs

```csharp
using Dapper;
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonRepository : IExtraLessonRepository
    {
        private readonly IExtraLessonAggregateDataAccessObject _aggregateService;
        private readonly IExtraLessonDataAccessObject _extraLessonService;
        private readonly IInvoiceDataAccessObject _invoiceService;
        private readonly IDbConnection _connection;
        private readonly ILogger<ExtraLessonRepository> _logger;

        public ExtraLessonRepository(
            IExtraLessonAggregateDataAccessObject aggregateService,
            IExtraLessonDataAccessObject extraLessonService,
            IInvoiceDataAccessObject invoiceService,
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

        /// <summary>
        /// Updates the status on an extra lesson row.
        /// <paramref name="note"/> is optional; when null the existing Notes value is preserved.
        /// </summary>
        public async Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status, string? note = null)
        {
            try
            {
                return await _extraLessonService.UpdateStatusAsync(extraLessonId, status, note);
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

```

## File: InvoiceDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class InvoiceDataAccessObject : IInvoiceDataAccessObject
    {
        private readonly IDbConnection _connection;

        public InvoiceDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Invoice?> GetInvoiceAsync(int id)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       ExtraLessonID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE InvoiceID = @InvoiceID;";

            return await _connection.QuerySingleOrDefaultAsync<Invoice>(sql, new { InvoiceID = id });
        }

        public async Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       ExtraLessonID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE BundleID = @BundleID
                ORDER BY InstallmentNumber;";

            return await _connection.QueryAsync<Invoice>(sql, new { BundleID = bundleId });
        }

        public async Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       ExtraLessonID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE AccountHolderID = @AccountHolderID
                ORDER BY DueDate;";

            return await _connection.QueryAsync<Invoice>(sql, new { AccountHolderID = accountHolderId });
        }

        public async Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       ExtraLessonID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE AccountHolderID = @AccountHolderID
                  AND Status IN ('Pending', 'Overdue')
                ORDER BY DueDate;";

            return await _connection.QueryAsync<Invoice>(sql, new { AccountHolderID = accountHolderId });
        }

        public async Task InsertBatchAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection)
        {
            const string sql = @"
                INSERT INTO Invoice
                    (BundleID, ExtraLessonID, AccountHolderID, InstallmentNumber,
                     Amount, DueDate, Status, Notes)
                VALUES
                    (@BundleID, @ExtraLessonID, @AccountHolderID, @InstallmentNumber,
                     @Amount, @DueDate, @Status, @Notes);";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, invoices, tx));
        }

        /// <summary>
        /// Inserts a single Invoice row within an existing transaction.
        /// Returns the new InvoiceID.
        /// </summary>
        public async Task<int> InsertAsync(Invoice invoice, IDbTransaction tx, IDbConnection connection)
        {
            const string sql = @"
                INSERT INTO Invoice
                    (BundleID, ExtraLessonID, AccountHolderID, InstallmentNumber,
                     Amount, DueDate, Status, Notes)
                VALUES
                    (@BundleID, @ExtraLessonID, @AccountHolderID, @InstallmentNumber,
                     @Amount, @DueDate, @Status, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, invoice, tx));
        }

        public async Task<bool> UpdateStatusAsync(int invoiceId, string status, DateOnly? paidDate)
        {
            const string sql = @"
                UPDATE Invoice
                SET Status   = @Status,
                    PaidDate = @PaidDate
                WHERE InvoiceID = @InvoiceID;";

            DateTime? paidDateTime = paidDate.HasValue
                ? paidDate.Value.ToDateTime(TimeOnly.MinValue)
                : null;

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { InvoiceID = invoiceId, Status = status, PaidDate = paidDateTime });
            return rowsAffected > 0;
        }
    }
}

```

## File: InvoiceRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IInvoiceDataAccessObject _invoiceService;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository(IInvoiceDataAccessObject invoiceService, ILogger<InvoiceRepository> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        public async Task<Invoice?> GetInvoiceAsync(int id)
        {
            return await _invoiceService.GetInvoiceAsync(id);
        }

        public async Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId)
        {
            return await _invoiceService.GetByBundleAsync(bundleId);
        }

        public async Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId)
        {
            return await _invoiceService.GetByAccountHolderAsync(accountHolderId);
        }

        public async Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId)
        {
            return await _invoiceService.GetOutstandingByAccountHolderAsync(accountHolderId);
        }

        /// <summary>
        /// Saves all 12 instalment rows for a bundle atomically.
        /// The application layer is responsible for calculating the Amount
        /// and setting the DueDate for each instalment before calling this method.
        /// </summary>
        public async Task<bool> AddInvoiceInstalmentsAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection)
        {
            try
            {
                await _invoiceService.InsertBatchAsync(invoices, tx, connection);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert invoice instalments for BundleID {BundleID}",
                    invoices.FirstOrDefault()?.BundleID);
                return false;
            }
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, string status, DateOnly? paidDate)
        {
            try
            {
                return await _invoiceService.UpdateStatusAsync(invoiceId, status, paidDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status for InvoiceID {InvoiceID}", invoiceId);
                return false;
            }
        }
    }
}

```

## File: LessonAggregateDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonAggregateDataAccessObject : ILessonAggregateDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string SELECT_LESSON_DETAIL_QRY = @"
            SELECT l.LessonID,
                   l.SlotID,
                   l.BundleID,
                   l.QuarterID,
                   l.ScheduledDate,
                   l.ScheduledTime,
                   l.Status,
                   l.CreditForfeited,
                   l.CancelledBy,
                   l.CancellationReason,
                   l.OriginalLessonID,
                   l.CompletedAt,
                   l.Notes,
                   s.StudentID,
                   s.FirstName      AS StudentFirstName,
                   s.LastName       AS StudentLastName,
                   t.TeacherID,
                   t.Name           AS TeacherName,
                   lt.LessonTypeID,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM Lesson         l
            JOIN ScheduledSlot  ss ON ss.SlotID       = l.SlotID
            JOIN Student         s ON s.StudentID     = ss.StudentID
            JOIN Teacher         t ON t.TeacherID     = ss.TeacherID
            JOIN LessonType     lt ON lt.LessonTypeID = ss.LessonTypeID
            WHERE l.LessonID = @LessonID;";

        public static readonly string SELECT_LESSONS_BY_TEACHER_DATE_QRY = @"
            SELECT l.LessonID,
                   l.SlotID,
                   l.BundleID,
                   l.QuarterID,
                   l.ScheduledDate,
                   l.ScheduledTime,
                   l.Status,
                   l.CreditForfeited,
                   l.CancelledBy,
                   l.CancellationReason,
                   l.OriginalLessonID,
                   l.CompletedAt,
                   l.Notes,
                   s.StudentID,
                   s.FirstName      AS StudentFirstName,
                   s.LastName       AS StudentLastName,
                   t.TeacherID,
                   t.Name           AS TeacherName,
                   lt.LessonTypeID,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM Lesson         l
            JOIN ScheduledSlot  ss ON ss.SlotID       = l.SlotID
            JOIN Student         s ON s.StudentID     = ss.StudentID
            JOIN Teacher         t ON t.TeacherID     = ss.TeacherID
            JOIN LessonType     lt ON lt.LessonTypeID = ss.LessonTypeID
            WHERE t.TeacherID      = @TeacherID
              AND l.ScheduledDate  = @ScheduledDate
            ORDER BY l.ScheduledTime;";

        public LessonAggregateDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<LessonDetail?> GetLessonByIdAsync(int lessonId)
        {
            return await _connection.QuerySingleOrDefaultAsync<LessonDetail>(
                SELECT_LESSON_DETAIL_QRY,
                new { LessonID = lessonId });
        }

        public async Task<IEnumerable<LessonDetail>> GetLessonsByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
        {
            return await _connection.QueryAsync<LessonDetail>(
                SELECT_LESSONS_BY_TEACHER_DATE_QRY,
                new { TeacherID = teacherId, ScheduledDate = scheduledDate });
        }
    }
}

```

## File: LessonBundleAggregateDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
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
        /// Saves a new LessonBundle, its 4 BundleQuarter rows, and 12 monthly Invoice
        /// instalments — all in a single transaction on the same connection.
        /// </summary>
        public async Task<int> SaveNewBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // 1. Resolve AccountHolderID inside the transaction.
                var accountHolderId = await _connection.ExecuteScalarAsync<int>(
                    "SELECT AccountHolderID FROM Student WHERE StudentID = @StudentID",
                    new { bundle.StudentID }, transaction);

                if (accountHolderId == 0)
                    throw new InvalidOperationException(
                        $"Student {bundle.StudentID} not found when creating bundle.");

                // 2. Insert bundle
                var bundleId = await _lessonBundleService.InsertAsync(bundle, transaction);

                // 3. Insert quarters — pass _connection explicitly so the INSERT runs on
                //    the same connection that owns the transaction. Without this, Dapper
                //    uses the service's injected connection which is a different instance,
                //    causing the quarters to be inserted outside the transaction or not at
                //    all, leaving LessonsAllocated = 0 and breaking slot/lesson creation.
                foreach (var quarter in quarters)
                    quarter.BundleID = bundleId;

                await _bundleQuarterService.InsertBatchAsync(quarters, transaction, _connection);

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

        private static IEnumerable<Invoice> BuildInstalments(
            int bundleId,
            int accountHolderId,
            decimal instalmentAmount,
            DateTime bundleStartDate)
        {
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

```

## File: LessonBundleDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonBundleDataAccessObject : ILessonBundleDataAccessObject
    {
        private readonly IDbConnection _connection;

        public LessonBundleDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<LessonBundle?> GetBundleAsync(int id)
        {
            const string sql = @"
                SELECT BundleID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       TotalLessons,
                       PricePerLesson,
                       StartDate,
                       EndDate,
                       QuarterSize,
                       IsActive,
                       Notes,
                       CreatedAt
                FROM LessonBundle
                WHERE BundleID = @BundleID;";

            return await _connection.QuerySingleOrDefaultAsync<LessonBundle>(sql, new { BundleID = id });
        }

        public async Task<IEnumerable<LessonBundle>> GetByStudentAsync(int studentId)
        {
            const string sql = @"
                SELECT BundleID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       TotalLessons,
                       PricePerLesson,
                       StartDate,
                       EndDate,
                       QuarterSize,
                       IsActive,
                       Notes,
                       CreatedAt
                FROM LessonBundle
                WHERE StudentID = @StudentID
ORDER BY StudentID";

            return await _connection.QueryAsync<LessonBundle>(sql, new { StudentID = studentId });
        }

        public async Task<int> InsertAsync(LessonBundle bundle, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO LessonBundle
                    (StudentID, TeacherID, LessonTypeID, 
                     TotalLessons, PricePerLesson, StartDate, EndDate, IsActive, Notes)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, 
                     @TotalLessons, @PricePerLesson, @StartDate, @EndDate, @IsActive, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, bundle, tx));
        }

        public async Task<bool> UpdateAsync(LessonBundle bundle)
        {
            const string sql = @"
                UPDATE LessonBundle
                SET StudentID      = @StudentID,
                    TeacherID      = @TeacherID,
                    LessonTypeID   = @LessonTypeID,
                    AcademicYear   = @AcademicYear,
                    TotalLessons   = @TotalLessons,
                    PricePerLesson = @PricePerLesson,
                    StartDate      = @StartDate,
                    EndDate        = @EndDate,
                    IsActive       = @IsActive,
                    Notes          = @Notes
                WHERE BundleID = @BundleID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, bundle);
            return rowsAffected > 0;
        }
    }
}

```

## File: LessonBundleRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Data.Implementations
{
    public class LessonBundleRepository : ILessonBundleRepository
    {
        private readonly ILessonBundleAggregateDataAccessObject _aggregateService;
        private readonly ILessonBundleDataAccessObject _lessonBundleService;
        private readonly ILogger<LessonBundleRepository> _logger;

        public LessonBundleRepository(
            ILessonBundleAggregateDataAccessObject aggregateService,
            ILessonBundleDataAccessObject lessonBundleService,
            ILogger<LessonBundleRepository> logger)
        {
            _aggregateService = aggregateService;
            _lessonBundleService = lessonBundleService;
            _logger = logger;
        }

        /// <summary>
        /// Returns the bundle with all four quarters as flat detail rows.
        /// </summary>
        public async Task<IEnumerable<LessonBundleWithQuarterDetail>> GetBundleAsync(int bundleId)
        {
            return await _aggregateService.GetBundleByIdAsync(bundleId);
        }

        public async Task<IEnumerable<LessonBundleDetail>> GetByStudentAsync(int studentId)
        {
            return await _aggregateService.GetBundleByStudentIdAsync(studentId);
        }

        /// <summary>
        /// Saves the bundle and its 4 quarters atomically.
        /// The application layer is responsible for building the quarter list
        /// before calling this method.
        /// </summary>
        public async Task<int?> AddBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters)
        {
            try
            {
                return await _aggregateService.SaveNewBundleAsync(bundle, quarters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to save LessonBundle for StudentID {StudentID}",
                    bundle.StudentID);
                return null;
            }
        }

        public async Task<bool> UpdateBundleAsync(LessonBundle bundle)
        {
            try
            {
                return await _lessonBundleService.UpdateAsync(bundle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update BundleID {BundleID}", bundle.BundleID);
                return false;
            }
        }
    }
}

```

## File: LessonDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonDataAccessObject : ILessonDataAccessObject
    {
        private readonly IDbConnection _connection;

        public LessonDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Lesson?> GetLessonAsync(int id)
        {
            const string sql = @"
                SELECT LessonID,
                       SlotID,
                       BundleID,
                       QuarterID,
                       ScheduledDate,
                       ScheduledTime,
                       Status,
                       CreditForfeited,
                       CancelledBy,
                       CancellationReason,
                       OriginalLessonID,
                       CompletedAt,
                       Notes,
                       CreatedAt
                FROM Lesson
                WHERE LessonID = @LessonID;";

            return await _connection.QuerySingleOrDefaultAsync<Lesson>(sql, new { LessonID = id });
        }

        public async Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId)
        {
            const string sql = @"
                SELECT LessonID,
                       SlotID,
                       BundleID,
                       QuarterID,
                       ScheduledDate,
                       ScheduledTime,
                       Status,
                       CreditForfeited,
                       CancelledBy,
                       CancellationReason,
                       OriginalLessonID,
                       CompletedAt,
                       Notes,
                       CreatedAt
                FROM Lesson
                WHERE BundleID = @BundleID
                ORDER BY ScheduledDate, ScheduledTime;";

            return await _connection.QueryAsync<Lesson>(sql, new { BundleID = bundleId });
        }

        public async Task<IEnumerable<Lesson>> GetByStatusAsync(string status)
        {
            const string sql = @"
                SELECT LessonID,
                       SlotID,
                       BundleID,
                       QuarterID,
                       ScheduledDate,
                       ScheduledTime,
                       Status,
                       CreditForfeited,
                       CancelledBy,
                       CancellationReason,
                       OriginalLessonID,
                       CompletedAt,
                       Notes,
                       CreatedAt
                FROM Lesson
                WHERE Status = @Status
                ORDER BY ScheduledDate, ScheduledTime;";

            return await _connection.QueryAsync<Lesson>(sql, new { Status = status });
        }

        public async Task<int> InsertAsync(Lesson lesson)
            => await InsertAsync(lesson, null!);

        public async Task<int> InsertAsync(Lesson lesson, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO Lesson
                    (SlotID, BundleID, QuarterID, ScheduledDate, ScheduledTime,
                     Status, CreditForfeited, CancelledBy, CancellationReason,
                     OriginalLessonID, CompletedAt, Notes)
                VALUES
                    (@SlotID, @BundleID, @QuarterID, @ScheduledDate, @ScheduledTime,
                     @Status, @CreditForfeited, @CancelledBy, @CancellationReason,
                     @OriginalLessonID, @CompletedAt, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, lesson, tx));
        }

        public async Task<bool> UpdateStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt,
            string? note = null)
        {
            const string sql = @"
                UPDATE Lesson
                SET Status             = @Status,
                    CreditForfeited    = @CreditForfeited,
                    CancelledBy        = @CancelledBy,
                    CancellationReason = @CancellationReason,
                    CompletedAt        = @CompletedAt,
                    Notes              = COALESCE(@Notes, Notes)
                WHERE LessonID = @LessonID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, new
            {
                LessonID           = lessonId,
                Status             = status,
                CreditForfeited    = creditForfeited,
                CancelledBy        = cancelledBy,
                CancellationReason = cancellationReason,
                CompletedAt        = completedAt,
                Notes              = note
            });
            return rowsAffected > 0;
        }

        public async Task<bool> RescheduleLessonAsync(int lessonId, DateTime newDate, TimeOnly newTime)
        {
            const string sql = @"
                UPDATE Lesson
                SET ScheduledDate      = @ScheduledDate,
                    ScheduledTime      = @ScheduledTime,
                    Status             = @Status,
                    CreditForfeited    = 0,
                    CancelledBy        = NULL,
                    CancellationReason = NULL,
                    CompletedAt        = NULL
                WHERE LessonID = @LessonID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, new
            {
                LessonID      = lessonId,
                ScheduledDate = newDate,
                ScheduledTime = newTime,
                Status        = LessonStatus.Scheduled
            });
            return rowsAffected > 0;
        }
    }
}

```

## File: LessonRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class LessonRepository : ILessonRepository
    {
        private readonly ILessonAggregateDataAccessObject _aggregateService;
        private readonly ILessonDataAccessObject _lessonService;
        private readonly IBundleQuarterDataAccessObject _bundleQuarterService;
        private readonly ILogger<LessonRepository> _logger;

        public LessonRepository(
            ILessonAggregateDataAccessObject aggregateService,
            ILessonDataAccessObject lessonService,
            IBundleQuarterDataAccessObject bundleQuarterService,
            ILogger<LessonRepository> logger)
        {
            _aggregateService = aggregateService;
            _lessonService = lessonService;
            _bundleQuarterService = bundleQuarterService;
            _logger = logger;
        }

        public async Task<LessonDetail?> GetLessonAsync(int lessonId)
            => await _aggregateService.GetLessonByIdAsync(lessonId);

        public async Task<IEnumerable<LessonDetail>> GetByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
            => await _aggregateService.GetLessonsByTeacherAndDateAsync(teacherId, scheduledDate);

        public async Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId)
            => await _lessonService.GetByBundleAsync(bundleId);

        public async Task<int?> AddLessonAsync(Lesson lesson)
        {
            try
            {
                return await _lessonService.InsertAsync(lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert Lesson for BundleID {BundleID} on {ScheduledDate}",
                    lesson.BundleID, lesson.ScheduledDate);
                return null;
            }
        }

        /// <summary>
        /// Updates the lesson status and keeps BundleQuarter.LessonsUsed in sync:
        ///   Completed / Forfeited → +1 (credit consumed)
        ///   CancelledTeacher / CancelledStudent → -1 only if the previous status
        ///   had already consumed a credit (i.e. was Completed or Forfeited).
        /// The delta approach is atomic — no separate read is needed.
        /// </summary>
        public async Task<bool> UpdateLessonStatusAsync(int lessonId, string status,
            bool creditForfeited, string? cancelledBy, string? cancellationReason,
            DateTime? completedAt, string? note = null)
        {
            try
            {
                // 1. Read current status so we know whether to adjust the quarter.
                var lesson = await _lessonService.GetLessonAsync(lessonId);
                if (lesson is null) return false;

                // 2. Update the lesson row.
                var updated = await _lessonService.UpdateStatusAsync(
                    lessonId, status, creditForfeited,
                    cancelledBy, cancellationReason, completedAt, note);

                if (!updated) return false;

                // 3. Adjust BundleQuarter.LessonsUsed.
                //    Credit is consumed when status moves TO Completed or Forfeited.
                //    Credit is released when status moves FROM Completed or Forfeited
                //    to anything that doesn't consume a credit.
                bool previousConsumed = lesson.Status == LessonStatus.Completed
                                     || lesson.Status == LessonStatus.Forfeited;
                bool newConsumed = status == LessonStatus.Completed
                                || status == LessonStatus.Forfeited;

                int delta = (newConsumed ? 1 : 0) - (previousConsumed ? 1 : 0);

                if (delta != 0)
                    await _bundleQuarterService.AdjustLessonsUsedAsync(lessonId, delta);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status for LessonID {LessonID}", lessonId);
                return false;
            }
        }

        /// <summary>
        /// Moves a cancelled lesson to a new date/time and resets it to Scheduled.
        /// No credit adjustment is needed — cancelled lessons never consumed a credit.
        /// </summary>
        public async Task<bool> RescheduleLessonAsync(int lessonId, DateTime newDate, TimeOnly newTime)
        {
            try
            {
                // Guard: only allow rescheduling of cancelled lessons.
                var lesson = await _lessonService.GetLessonAsync(lessonId);
                if (lesson is null) return false;

                if (lesson.Status != LessonStatus.CancelledTeacher
                    && lesson.Status != LessonStatus.CancelledStudent)
                {
                    _logger.LogWarning(
                        "RescheduleLessonAsync rejected: LessonID {LessonID} has status {Status}.",
                        lessonId, lesson.Status);
                    return false;
                }

                return await _lessonService.RescheduleLessonAsync(lessonId, newDate, newTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reschedule LessonID {LessonID}", lessonId);
                return false;
            }
        }
    }
}

```

## File: LessonTypeDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonTypeDataAccessObject : ILessonTypeDataAccessObject
    {
        private readonly IDbConnection _connection;

        public LessonTypeDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<LessonType?> GetLessonTypeAsync(int id)
        {
            const string sql = @"
                SELECT LessonTypeID,
                       DurationMinutes,
                       BasePricePerLesson,
                       IsActive
                FROM LessonType
                WHERE LessonTypeID = @LessonTypeID;";

            return await _connection.QuerySingleOrDefaultAsync<LessonType>(sql, new { LessonTypeID = id });
        }

        public async Task<IEnumerable<LessonType>> GetAllActiveAsync()
        {
            const string sql = @"
                SELECT LessonTypeID,
                       DurationMinutes,
                       BasePricePerLesson,
                       IsActive
                FROM LessonType
                WHERE IsActive = 1
                ORDER BY DurationMinutes;";

            return await _connection.QueryAsync<LessonType>(sql);
        }

        public async Task<int> InsertAsync(LessonType lessonType)
        {
            const string sql = @"
                INSERT INTO LessonType (DurationMinutes, BasePricePerLesson, IsActive)
                VALUES (@DurationMinutes, @BasePricePerLesson, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, lessonType);
        }

        public async Task<bool> UpdateAsync(LessonType lessonType)
        {
            const string sql = @"
                UPDATE LessonType
                SET DurationMinutes    = @DurationMinutes,
                    BasePricePerLesson = @BasePricePerLesson,
                    IsActive           = @IsActive
                WHERE LessonTypeID = @LessonTypeID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, lessonType);
            return rowsAffected > 0;
        }
    }
}

```

## File: LessonTypeRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class LessonTypeRepository : ILessonTypeRepository
    {
        private readonly ILessonTypeDataAccessObject _lessonTypeService;
        private readonly ILogger<LessonTypeRepository> _logger;

        public LessonTypeRepository(ILessonTypeDataAccessObject lessonTypeService, ILogger<LessonTypeRepository> logger)
        {
            _lessonTypeService = lessonTypeService;
            _logger = logger;
        }

        public async Task<LessonType?> GetLessonTypeAsync(int id)
        {
            return await _lessonTypeService.GetLessonTypeAsync(id);
        }

        public async Task<IEnumerable<LessonType>> GetAllActiveAsync()
        {
            return await _lessonTypeService.GetAllActiveAsync();
        }

        public async Task<int?> AddLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                return await _lessonTypeService.InsertAsync(lessonType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert LessonType {DurationMinutes}min",
                    lessonType.DurationMinutes);
                return null;
            }
        }

        public async Task<bool> UpdateLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                return await _lessonTypeService.UpdateAsync(lessonType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update LessonTypeID {LessonTypeID}",
                    lessonType.LessonTypeID);
                return false;
            }
        }
    }
}

```

## File: MusicSchool.Repositories.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Dapper" Version="2.1.72" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.5" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MusicSchool.Interfaces\MusicSchool.Interfaces.csproj" />
    <ProjectReference Include="..\MusicSchool.Models\MusicSchool.Models.csproj" />
  </ItemGroup>

</Project>

```

## File: ScheduledSlotDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ScheduledSlotDataAccessObject : IScheduledSlotDataAccessObject
    {
        private readonly IDbConnection _connection;

        public ScheduledSlotDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ScheduledSlot?> GetSlotAsync(int id)
        {
            const string sql = @"
                SELECT SlotID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       DayOfWeek,
                       SlotTime,
                       EffectiveFrom,
                       EffectiveTo,
                       IsActive
                FROM ScheduledSlot
                WHERE SlotID = @SlotID;";

            return await _connection.QuerySingleOrDefaultAsync<ScheduledSlot>(sql, new { SlotID = id });
        }

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId)
        {
            const string sql = @"
                SELECT SlotID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       DayOfWeek,
                       SlotTime,
                       EffectiveFrom,
                       EffectiveTo,
                       IsActive
                FROM ScheduledSlot
                WHERE StudentID  = @StudentID
                  AND IsActive   = 1
                  AND EffectiveTo IS NULL
                ORDER BY DayOfWeek, SlotTime;";

            return await _connection.QueryAsync<ScheduledSlot>(sql, new { StudentID = studentId });
        }

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId)
        {
            const string sql = @"
                SELECT SlotID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       DayOfWeek,
                       SlotTime,
                       EffectiveFrom,
                       EffectiveTo,
                       IsActive
                FROM ScheduledSlot
                WHERE TeacherID  = @TeacherID
                  AND IsActive   = 1
                  AND EffectiveTo IS NULL
                ORDER BY DayOfWeek, SlotTime;";

            return await _connection.QueryAsync<ScheduledSlot>(sql, new { TeacherID = teacherId });
        }

        public async Task<int> InsertAsync(ScheduledSlot slot)
            => await InsertAsync(slot);

        public async Task<int> InsertAsync(ScheduledSlot slot, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO ScheduledSlot
                    (StudentID, TeacherID, LessonTypeID, DayOfWeek,
                     SlotTime, EffectiveFrom, EffectiveTo, IsActive)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, @DayOfWeek,
                     @SlotTime, @EffectiveFrom, @EffectiveTo, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, slot, tx));
        }

        public async Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo)
        {
            const string sql = @"
                UPDATE ScheduledSlot
                SET EffectiveTo = @EffectiveTo,
                    IsActive    = 0
                WHERE SlotID = @SlotID;";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { SlotID = slotId, EffectiveTo = effectiveTo });
            return rowsAffected > 0;
        }

        /// <summary>
        /// Opens the connection if needed, begins a transaction, runs <paramref name="work"/>,
        /// and commits. Rolls back on any exception.
        /// </summary>
        public async Task ExecuteInTransactionAsync(Func<IDbTransaction, IDbConnection, Task> work)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var tx = _connection.BeginTransaction();
            try
            {
                await work(tx, _connection);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}

```

## File: ScheduledSlotRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class ScheduledSlotRepository : IScheduledSlotRepository
    {
        private readonly IScheduledSlotDataAccessObject _slotService;
        private readonly ILessonBundleDataAccessObject _bundleService;
        private readonly IBundleQuarterDataAccessObject _quarterService;
        private readonly ILessonDataAccessObject _lessonService;
        private readonly ILogger<ScheduledSlotRepository> _logger;

        public ScheduledSlotRepository(
            IScheduledSlotDataAccessObject slotService,
            ILessonBundleDataAccessObject bundleService,
            IBundleQuarterDataAccessObject quarterService,
            ILessonDataAccessObject lessonService,
            ILogger<ScheduledSlotRepository> logger)
        {
            _slotService = slotService;
            _bundleService = bundleService;
            _quarterService = quarterService;
            _lessonService = lessonService;
            _logger = logger;
        }

        public async Task<ScheduledSlot?> GetSlotAsync(int id)
            => await _slotService.GetSlotAsync(id);

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId)
            => await _slotService.GetActiveByStudentAsync(studentId);

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId)
            => await _slotService.GetActiveByTeacherAsync(teacherId);

        /// <summary>
        /// Validates that the student has an active bundle with remaining credits,
        /// inserts the slot, then generates all future Lesson rows up to the bundle's
        /// EndDate — one per weekly occurrence matching the slot's DayOfWeek.
        /// Everything runs in a single transaction; nothing is committed if any step fails.
        /// Returns null if the student has no usable bundle, or on any error.
        /// </summary>
        public async Task<int?> AddSlotAsync(ScheduledSlot slot)
        {
            try
            {
                // 1. Find the student's active bundle that still has remaining credits.
                //    "Active" means IsActive = true, not yet expired, and at least one
                //    quarter still has lessons remaining.
                var bundles = await _bundleService.GetByStudentAsync(slot.StudentID);

                LessonBundle? bundle = null;

                foreach (var b in bundles.Where(b => b.IsActive && b.EndDate >= DateTime.Today))
                {
                    var quartersl = (await _quarterService.GetByBundleAsync(b.BundleID)).ToList();
                    if (quartersl.Any(q => q.LessonsUsed < q.LessonsAllocated))
                    {
                        bundle = b;
                        break;
                    }
                }

                if (bundle is null)
                {
                    _logger.LogWarning(
                        "AddSlotAsync rejected: StudentID {StudentID} has no active bundle with remaining credits.",
                        slot.StudentID);
                    return null;
                }

                var quarters = (await _quarterService.GetByBundleAsync(bundle.BundleID)).ToList();

                // 2. Insert the slot and generate lessons in one transaction.
                await _slotService.ExecuteInTransactionAsync(async (tx, conn) =>
                {
                    var slotId = await _slotService.InsertAsync(slot, tx);
                    slot.SlotID = slotId;

                    // Generate one Lesson per weekly occurrence from the slot's EffectiveFrom
                    // through the bundle's EndDate. EffectiveFrom is authoritative — do not
                    // clamp to today, as slots may be created retroactively or in advance.
                    var lessonDates = GetOccurrences(slot.EffectiveFrom.Date, bundle.EndDate, slot.DayOfWeek);

                    foreach (var date in lessonDates)
                    {
                        var quarter = quarters.FirstOrDefault(q =>
                            date >= q.QuarterStartDate && date <= q.QuarterEndDate);

                        if (quarter is null) continue;

                        await _lessonService.InsertAsync(new Lesson
                        {
                            SlotID          = slotId,
                            BundleID        = bundle.BundleID,
                            QuarterID       = quarter.QuarterID,
                            ScheduledDate   = date,
                            ScheduledTime   = slot.SlotTime,
                            Status          = LessonStatus.Scheduled,
                            CreditForfeited = false,
                        }, tx);
                    }
                });

                return slot.SlotID;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert ScheduledSlot for StudentID {StudentID}", slot.StudentID);
                return null;
            }
        }

        /// <summary>
        /// Closes a slot by setting EffectiveTo and IsActive = false.
        /// Call AddSlotAsync afterwards to open the replacement slot.
        /// </summary>
        public async Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo)
        {
            try
            {
                return await _slotService.CloseSlotAsync(slotId, effectiveTo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to close SlotID {SlotID}", slotId);
                return false;
            }
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        /// <summary>
        /// Enumerates every calendar date between <paramref name="from"/> and
        /// <paramref name="to"/> (inclusive) that falls on <paramref name="isoDayOfWeek"/>
        /// (1 = Monday … 7 = Sunday, matching the ScheduledSlot.DayOfWeek convention).
        /// </summary>
        private static IEnumerable<DateTime> GetOccurrences(
            DateTime from, DateTime to, byte isoDayOfWeek)
        {
            // .NET DayOfWeek: Sunday=0, Monday=1 … Saturday=6
            // ISO DayOfWeek:  Monday=1, Tuesday=2 … Sunday=7
            var targetDotNet = isoDayOfWeek == 7
                ? DayOfWeek.Sunday
                : (DayOfWeek)isoDayOfWeek;

            var date = from;
            // Advance to the first matching weekday
            while (date.DayOfWeek != targetDotNet)
                date = date.AddDays(1);

            while (date <= to)
            {
                yield return date;
                date = date.AddDays(7);
            }
        }
    }
}

```

## File: StudentDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class StudentDataAccessObject : IStudentDataAccessObject
    {
        private readonly IDbConnection _connection;

        public StudentDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            const string sql = @"
                SELECT StudentID,
                       AccountHolderID,
                       FirstName,
                       LastName,
                       DateOfBirth,
                       IsAccountHolder,
                       IsActive,
                       CreatedAt
                FROM Student
                WHERE StudentID = @StudentID;";

            return await _connection.QuerySingleOrDefaultAsync<Student>(sql, new { StudentID = id });
        }

        public async Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT StudentID,
                       AccountHolderID,
                       FirstName,
                       LastName,
                       DateOfBirth,
                       IsAccountHolder,
                       IsActive,
                       CreatedAt
                FROM Student
                WHERE AccountHolderID = @AccountHolderID
                  AND IsActive        = 1
                ORDER BY LastName, FirstName;";

            return await _connection.QueryAsync<Student>(sql, new { AccountHolderID = accountHolderId });
        }

        public async Task<int> InsertAsync(Student student)
        {
            const string sql = @"
                INSERT INTO Student
                    (AccountHolderID, FirstName, LastName, DateOfBirth, IsAccountHolder, IsActive)
                VALUES
                    (@AccountHolderID, @FirstName, @LastName, @DateOfBirth, @IsAccountHolder, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, student);
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            const string sql = @"
                UPDATE Student
                SET AccountHolderID = @AccountHolderID,
                    FirstName       = @FirstName,
                    LastName        = @LastName,
                    DateOfBirth     = @DateOfBirth,
                    IsAccountHolder = @IsAccountHolder,
                    IsActive        = @IsActive
                WHERE StudentID = @StudentID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, student);
            return rowsAffected > 0;
        }
    }
}

```

## File: StudentRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IStudentDataAccessObject _studentService;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(IStudentDataAccessObject studentService, ILogger<StudentRepository> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            return await _studentService.GetStudentAsync(id);
        }

        public async Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId)
        {
            return await _studentService.GetByAccountHolderAsync(accountHolderId);
        }

        public async Task<int?> AddStudentAsync(Student student)
        {
            try
            {
                return await _studentService.InsertAsync(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert Student {FirstName} {LastName}",
                    student.FirstName, student.LastName);
                return null;
            }
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            try
            {
                return await _studentService.UpdateAsync(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update StudentID {StudentID}", student.StudentID);
                return false;
            }
        }
    }
}

```

## File: TeacherDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class TeacherDataAccessObject : ITeacherDataAccessObject
    {
        private readonly IDbConnection _connection;

        public TeacherDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            const string sql = @"
                SELECT TeacherID,
                       Name,
                       Email,
                       Phone,
                       IsActive,
                       CreatedAt
                FROM Teacher
                WHERE TeacherID = @TeacherID;";

            return await _connection.QuerySingleOrDefaultAsync<Teacher>(sql, new { TeacherID = id });
        }

        public async Task<IEnumerable<Teacher>> GetAllActiveAsync()
        {
            const string sql = @"
                SELECT TeacherID,
                       Name,
                       Email,
                       Phone,
                       IsActive,
                       CreatedAt
                FROM Teacher
                WHERE IsActive = 1
                ORDER BY Name;";

            return await _connection.QueryAsync<Teacher>(sql);
        }

        public async Task<int> InsertAsync(Teacher teacher)
        {
            const string sql = @"
                INSERT INTO Teacher (Name, Email, Phone, IsActive)
                VALUES (@Name, @Email, @Phone, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, teacher);
        }

        public async Task<bool> UpdateAsync(Teacher teacher)
        {
            const string sql = @"
                UPDATE Teacher
                SET Name     = @Name,
                    Email    = @Email,
                    Phone    = @Phone,
                    IsActive = @IsActive
                WHERE TeacherID = @TeacherID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, teacher);
            return rowsAffected > 0;
        }
    }
}

```

## File: TeacherRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ITeacherDataAccessObject _teacherService;
        private readonly ILogger<TeacherRepository> _logger;

        public TeacherRepository(ITeacherDataAccessObject teacherService, ILogger<TeacherRepository> logger)
        {
            _teacherService = teacherService;
            _logger = logger;
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            return await _teacherService.GetTeacherAsync(id);
        }

        public async Task<IEnumerable<Teacher>> GetAllActiveAsync()
        {
            return await _teacherService.GetAllActiveAsync();
        }

        public async Task<int?> AddTeacherAsync(Teacher teacher)
        {
            try
            {
                return await _teacherService.InsertAsync(teacher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert Teacher {Name}", teacher.Name);
                return null;
            }
        }

        public async Task<bool> UpdateTeacherAsync(Teacher teacher)
        {
            try
            {
                return await _teacherService.UpdateAsync(teacher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update TeacherID {TeacherID}", teacher.TeacherID);
                return false;
            }
        }
    }
}

```
