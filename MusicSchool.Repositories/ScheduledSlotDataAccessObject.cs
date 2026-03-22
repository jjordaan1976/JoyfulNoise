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
        {
            return await InsertAsync(slot, _connection, null);
        }

        public async Task<int> InsertAsync(ScheduledSlot slot, IDbConnection connection, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO ScheduledSlot
                    (StudentID, TeacherID, LessonTypeID, DayOfWeek,
                     SlotTime, EffectiveFrom, EffectiveTo, IsActive)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, @DayOfWeek,
                     @SlotTime, @EffectiveFrom, @EffectiveTo, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await connection.ExecuteScalarAsync<int>(
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
    }
}
