namespace CBHWA.Models
{
    using CBHWA.Areas.Reports.Models;
    using CBHWA.Clases;
    using System.Collections.Generic;

    interface IEmployeeRepository
    {
        IList<Employee> GetList(FieldFilters fieldFilters, string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        Employee Get(int id);
        Employee Add(Employee employee);
        bool Remove(Employee employee);
        Employee Update(Employee employee);
        IList<Employee> GetListForReport(int JobRoleKey, string startDate, string endDate, string reportName, string query, int page, int start, int limit, ref int totalRecords);
        bool EnqueueReport(Enqueue model);
    }
}
