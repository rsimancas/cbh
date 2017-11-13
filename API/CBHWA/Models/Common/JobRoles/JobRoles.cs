using System;

namespace CBHWA.Models
{
    public class JobRole
    {
        public int JobRoleKey { get; set; }
        public int JobRoleSort { get; set; }
        public string JobRoleDescription { get; set; }
        public string JobRoleModifiedBy { get; set; }
        public Nullable<DateTime> JobRoleModifiedDate { get; set; }
        public string JobRoleCreatedBy { get; set; }
        public DateTime JobRoleCreatedDate { get; set; }
    }
}