namespace MusicSchool.Models
{
    public class LessonBundleDetail
    {
        // LessonBundle fields
        public int BundleID { get; set; }
        public int StudentID { get; set; }
        public int TeacherID { get; set; }
        public int LessonTypeID { get; set; }
        public short AcademicYear { get; set; }
        public int TotalLessons { get; set; }
        public decimal PricePerLesson { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int QuarterSize { get; set; }
        public string? BundleNotes { get; set; }

        // Student fields
        public string StudentFirstName { get; set; } = string.Empty;
        public string StudentLastName { get; set; } = string.Empty;

        // LessonType fields
        public int DurationMinutes { get; set; }
        public decimal BasePricePerLesson { get; set; }

        // BundleQuarter fields
        public int QuarterID { get; set; }
        public byte QuarterNumber { get; set; }
        public int LessonsAllocated { get; set; }
        public int LessonsUsed { get; set; }
        public DateOnly QuarterStartDate { get; set; }
        public DateOnly QuarterEndDate { get; set; }
    }
}
