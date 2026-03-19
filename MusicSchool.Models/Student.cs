using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Enrolled by an account holder.
    /// IsAccountHolder is true when the individual fills both roles.
    /// </summary>
    public class Student
    {
        public int       StudentID       { get; set; }
        public int       AccountHolderID { get; set; }
        public string    FirstName       { get; set; } = string.Empty;
        public string    LastName        { get; set; } = string.Empty;
        public DateOnly? DateOfBirth     { get; set; }
        public bool      IsAccountHolder { get; set; } = false;
        public bool      IsActive        { get; set; } = true;
        public DateTime  CreatedAt       { get; set; }
    }
}
