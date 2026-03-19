using System;

namespace MusicSchool.Data.Models
{
    public class Teacher
    {
        public int      TeacherID { get; set; }
        public string   Name      { get; set; } = string.Empty;
        public string   Email     { get; set; } = string.Empty;
        public string?  Phone     { get; set; }
        public bool     IsActive  { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
