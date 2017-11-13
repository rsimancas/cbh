using System;

namespace CBHWA.Models
{
    public class Employee
    {
        public int EmployeeKey { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeMiddleInitial { get; set; }
        public string EmployeeLastName { get; set; }
        public int? EmployeeTitleCode { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeEmailCC { get; set; }
        public string EmployeeAddress1 { get; set; }
        public string EmployeeCity { get; set; }
        public string EmployeeState { get; set; }
        public string EmployeeZip { get; set; }
        public string EmployeePhone { get; set; }
        public string EmployeeLogin { get; set; }
        public int EmployeeStatusCode { get; set; }
        public string EmployeePeachtreeID { get; set; }
        public int EmployeeLocationKey { get; set; }
        public string EmployeePassword { get; set; }
        public string EmployeeModifiedBy { get; set; }
        public Nullable<DateTime> EmployeeModifiedDate { get; set; }
        public string EmployeeCreatedBy { get; set; }
        public DateTime EmployeeCreatedDate { get; set; }
        public bool EmployeeSecurityLevel { get; set; }
        public string x_EmployeeFullName { get; set; }
        public int? EmployeeAccessLevel { get; set; }
    }
}