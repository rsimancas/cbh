
namespace CBHWA.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserFullName { get; set; }
        public int EmployeeKey { get; set; }
        public int EmployeeLocationKey { get; set; }
        public int EmployeeAccessLevel { get; set; }
    }
}