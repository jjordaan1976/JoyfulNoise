public class LessonDetail
{
    // Lesson fields
    public int LessonID { get; set; }
    public int SlotID { get; set; }
    public int BundleID { get; set; }
    public int QuarterID { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public TimeOnly ScheduledTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool CreditForfeited { get; set; }
    public string? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }
    public int? OriginalLessonID { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Student fields
    public int StudentID { get; set; }
    public string StudentFirstName { get; set; } = string.Empty;
    public string StudentLastName { get; set; } = string.Empty;

    // Teacher fields
    public int TeacherID { get; set; }
    public string TeacherName { get; set; } = string.Empty;

    // LessonType fields
    public int LessonTypeID { get; set; }
    public int DurationMinutes { get; set; }
    public decimal BasePricePerLesson { get; set; }
}