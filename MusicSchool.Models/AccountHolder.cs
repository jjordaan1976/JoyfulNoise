using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Contracted by a teacher. The billing party for one or more students.
    /// </summary>
    public class AccountHolder
    {
        public int      AccountHolderID { get; set; }
        public int      TeacherID       { get; set; }
        public string   FirstName       { get; set; } = string.Empty;
        public string   LastName        { get; set; } = string.Empty;
        public string   Email           { get; set; } = string.Empty;
        public string?  Phone           { get; set; }
        public string?  BillingAddress  { get; set; }
        public bool     IsActive        { get; set; } = true;
        public DateTime CreatedAt       { get; set; }
    }
}
