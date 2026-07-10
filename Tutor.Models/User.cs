using System;

namespace Tutor.Data.Models
{
    public enum UserRole
    {
        Teacher,
        Student,
        AccountHolder
    }

    /// <summary>
    /// A login identity. One row per role an email address holds —
    /// the same email can be both a Student and an AccountHolder.
    /// </summary>
    public class User
    {
        public int       UserID          { get; set; }
        public string    Email           { get; set; } = string.Empty;
        public string    DisplayName     { get; set; } = string.Empty;
        public UserRole  Role            { get; set; }
        public int?      TeacherID       { get; set; }
        public int?      StudentID       { get; set; }
        public int?      AccountHolderID { get; set; }
        public bool      IsActive        { get; set; } = true;
        public DateTime  CreatedAt       { get; set; }
    }
}
