using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// One of the four equal portions of a LessonBundle.
    /// LessonsUsed is incremented by the application each time a lesson
    /// is marked Completed or Forfeited.
    /// </summary>
    public class BundleQuarter
    {
        public int      QuarterID        { get; set; }
        public int      BundleID         { get; set; }

        /// <summary>
        /// Quarter sequence within the bundle: 1–4.
        /// </summary>
        public byte     QuarterNumber    { get; set; }

        public int      LessonsAllocated { get; set; }
        public int      LessonsUsed      { get; set; } = 0;
        public DateOnly QuarterStartDate { get; set; }
        public DateOnly QuarterEndDate   { get; set; }
    }
}
