using Dapper;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using System.Data;

namespace Tutor.Data.Implementations
{
    public class ScheduledSlotDataAccessObject : IScheduledSlotDataAccessObject
    {
        private readonly IDbConnection _connection;

        public static readonly string GetByIdSql = @"
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

        public static readonly string GetActiveByStudentSql = @"
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

        public static readonly string GetActiveByTeacherSql = @"
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

        public static readonly string InsertSql = @"
                INSERT INTO ScheduledSlot
                    (StudentID, TeacherID, LessonTypeID, DayOfWeek,
                     SlotTime, EffectiveFrom, EffectiveTo, IsActive)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, @DayOfWeek,
                     @SlotTime, @EffectiveFrom, @EffectiveTo, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

        public static readonly string CloseSlotSql = @"
                UPDATE ScheduledSlot
                SET EffectiveTo = @EffectiveTo,
                    IsActive    = 0
                WHERE SlotID = @SlotID;";

        public ScheduledSlotDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ScheduledSlot?> GetSlotAsync(int id)
        {
            return await _connection.QuerySingleOrDefaultAsync<ScheduledSlot>(GetByIdSql, new { SlotID = id });
        }

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId)
        {
            return await _connection.QueryAsync<ScheduledSlot>(GetActiveByStudentSql, new { StudentID = studentId });
        }

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId)
        {
            return await _connection.QueryAsync<ScheduledSlot>(GetActiveByTeacherSql, new { TeacherID = teacherId });
        }

        public async Task<int> InsertAsync(ScheduledSlot slot)
        {
            return await InsertAsync(slot, _connection, null);
        }

        /// <summary>
        /// Inserts within an existing transaction. Executes against the passed-in
        /// connection (the one that owns the transaction), never the injected one.
        /// </summary>
        public async Task<int> InsertAsync(ScheduledSlot slot, IDbConnection connection, IDbTransaction tx)
        {
            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(InsertSql, slot, tx));
        }

        public async Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo)
        {
            var rowsAffected = await _connection.ExecuteAsync(CloseSlotSql,
                new { SlotID = slotId, EffectiveTo = effectiveTo });
            return rowsAffected > 0;
        }
    }
}
