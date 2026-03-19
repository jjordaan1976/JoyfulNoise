public class ExtraLessonDetail
{
    // ExtraLesson fields
    public int ExtraLessonID { get; set; }
    public int StudentID { get; set; }
    public int TeacherID { get; set; }
    public int LessonTypeID { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeOnly ScheduledTime { get; set; }
    public decimal PriceCharged { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Student fields
    public string StudentFirstName { get; set; } = string.Empty;
    public string StudentLastName { get; set; } = string.Empty;
    public string StudentFullName { get { return $"{StudentFirstName} {StudentLastName}"; } }

    // Teacher fields
    public string TeacherName { get; set; } = string.Empty;

    // LessonType fields
    public int DurationMinutes { get; set; }
    public decimal BasePricePerLesson { get; set; }
}