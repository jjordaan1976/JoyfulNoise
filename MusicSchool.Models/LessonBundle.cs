namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Annual lesson bundle purchased for a student.
    /// PricePerLesson holds the teacher-adjusted (possibly discounted) rate
    /// agreed at the time of purchase.
    /// </summary>
    public class LessonBundle
    {
        public int      BundleID       { get; set; }
        public int      StudentID      { get; set; }
        public int      TeacherID      { get; set; }
        public int      LessonTypeID   { get; set; }
        public int      TotalLessons   { get; set; }
        public decimal  PricePerLesson { get; set; }
        public DateTime StartDate      { get; set; }
        public DateTime EndDate        { get; set; }

        /// <summary>
        /// Computed by the database as TotalLessons / 4. Read-only.
        /// </summary>
        public int      QuarterSize    { get; set; }

        public bool     IsActive       { get; set; } = true;
        public string?  Notes          { get; set; }
        public DateTime CreatedAt      { get; set; }
    }
}
