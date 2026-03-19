namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Valid values for <see cref="ExtraLesson.Status"/>.
    /// </summary>
    public static class ExtraLessonStatus
    {
        public const string Scheduled = "Scheduled";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public const string Forfeited = "Forfeited";
    }

    /// <summary>
    /// Ad-hoc lesson purchased after a bundle is exhausted.
    /// PriceCharged holds the teacher-adjusted rate (base price or override).
    /// </summary>
    public class ExtraLesson
    {
        public int      ExtraLessonID { get; set; }
        public int      StudentID     { get; set; }
        public int      TeacherID     { get; set; }
        public int      LessonTypeID  { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime ScheduledTime { get; set; }
        public decimal  PriceCharged  { get; set; }

        /// <summary>
        /// See <see cref="ExtraLessonStatus"/> for valid values.
        /// </summary>
        public string   Status        { get; set; } = ExtraLessonStatus.Scheduled;

        public string?  Notes         { get; set; }
        public DateTime CreatedAt     { get; set; }
    }
}
