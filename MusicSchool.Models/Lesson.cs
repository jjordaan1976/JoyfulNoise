namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Valid values for <see cref="Lesson.Status"/>.
    /// </summary>
    public static class LessonStatus
    {
        /// <summary>Upcoming; not yet attended.</summary>
        public const string Scheduled        = "Scheduled";

        /// <summary>Attended; credit consumed.</summary>
        public const string Completed        = "Completed";

        /// <summary>
        /// Teacher cancelled. Credit is NOT forfeited;
        /// teacher is responsible for rescheduling.
        /// </summary>
        public const string CancelledTeacher = "CancelledTeacher";

        /// <summary>
        /// Lesson was moved. OriginalLessonID references the cancelled lesson.
        /// </summary>
        public const string Rescheduled      = "Rescheduled";

        /// <summary>Student cancelled; teacher decides the outcome.</summary>
        public const string CancelledStudent = "CancelledStudent";

        /// <summary>
        /// Student cancelled and teacher chose not to reschedule.
        /// Credit is forfeited.
        /// </summary>
        public const string Forfeited        = "Forfeited";
    }

    /// <summary>
    /// Valid values for <see cref="Lesson.CancelledBy"/>.
    /// </summary>
    public static class CancelledBy
    {
        public const string Teacher = "Teacher";
        public const string Student = "Student";
    }

    /// <summary>
    /// One instance of a lesson, generated from a <see cref="ScheduledSlot"/>.
    /// Draws a credit from a <see cref="BundleQuarter"/>.
    /// </summary>
    public class Lesson
    {
        public int       LessonID           { get; set; }
        public int       SlotID             { get; set; }
        public int       BundleID           { get; set; }
        public int       QuarterID          { get; set; }
        public DateTime  ScheduledDate      { get; set; }
        public TimeOnly  ScheduledTime      { get; set; }

        /// <summary>
        /// See <see cref="LessonStatus"/> for valid values.
        /// </summary>
        public string    Status             { get; set; } = LessonStatus.Scheduled;

        public bool      CreditForfeited    { get; set; } = false;

        /// <summary>
        /// See <see cref="CancelledBy"/> for valid values. Null when not cancelled.
        /// </summary>
        public string?   CancelledBy        { get; set; }

        public string?   CancellationReason { get; set; }

        /// <summary>
        /// Populated on rescheduled lessons. References the original lesson that was cancelled.
        /// </summary>
        public int?      OriginalLessonID   { get; set; }

        public DateTime? CompletedAt        { get; set; }

        /// <summary>
        /// Optional free-text note that can be attached when updating the lesson status.
        /// </summary>
        public string?   Notes              { get; set; }

        public DateTime  CreatedAt          { get; set; }
    }
}
