namespace MusicSchool.Data.Models
{
    public class MagicLink
    {
        public int MagicLinkID { get; set; }
        public Guid Token { get; set; }
        public string LinkType { get; set; } = string.Empty;   // "Student" | "AccountHolder"
        public int EntityID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }

    public static class MagicLinkType
    {
        public const string Student = "Student";
        public const string AccountHolder = "AccountHolder";
    }
}
