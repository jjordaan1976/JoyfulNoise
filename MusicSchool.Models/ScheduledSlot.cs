using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// The recurring weekly pattern for a student/teacher pair.
    /// When a slot changes, close it by setting EffectiveTo and open a new one
    /// to preserve the history of past lessons.
    /// </summary>
    public class ScheduledSlot
    {
        public int       SlotID        { get; set; }
        public int       StudentID     { get; set; }
        public int       TeacherID     { get; set; }
        public int       LessonTypeID  { get; set; }

        /// <summary>
        /// ISO 8601 day of week: 1 = Monday … 7 = Sunday.
        /// </summary>
        public byte      DayOfWeek     { get; set; }

        public TimeOnly  SlotTime      { get; set; }
        public DateOnly  EffectiveFrom { get; set; }

        /// <summary>
        /// Null indicates the slot is still active.
        /// </summary>
        public DateOnly? EffectiveTo   { get; set; }

        public bool      IsActive      { get; set; } = true;
    }
}
