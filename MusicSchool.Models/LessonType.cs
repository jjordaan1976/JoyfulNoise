namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Defines a lesson duration (30 / 45 / 60 minutes) and its base price.
    /// The application layer applies any teacher discount on top of BasePricePerLesson.
    /// </summary>
    public class LessonType
    {
        public int     LessonTypeID       { get; set; }
        public int     DurationMinutes    { get; set; }   // 30, 45, or 60
        public decimal BasePricePerLesson { get; set; }
        public bool    IsActive           { get; set; } = true;

        public string DisplayName { get { return $"{DurationMinutes} min"; } }
    }
}
