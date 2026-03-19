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
            IBundleQuarterService bundleQuarterService)
        {
            _connection = connection;
            _lessonBundleService = lessonBundleService;
            _bundleQuarterService = bundleQuarterService;
        }

        /// <summary>
        /// Saves a new LessonBundle together with its 4 BundleQuarter rows
        /// in a single transaction. Returns the new BundleID.
        /// </summary>
        public async Task<int> SaveNewBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters)
        {
            _connection.Open();
            using var transaction = _connection.BeginTransaction();

            try
            {
                var bundleId = await _lessonBundleService.InsertAsync(bundle, transaction);

                foreach (var quarter in quarters)
                    quarter.BundleID = bundleId;

                await _bundleQuarterService.InsertBatchAsync(quarters, transaction);

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
        /// Returns a flat LessonBundleDetail row per quarter (4 rows total)
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
                new { StudentId = studentId });
        }
    }
}
