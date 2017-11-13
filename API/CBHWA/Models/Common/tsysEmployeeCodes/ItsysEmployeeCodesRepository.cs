using CBHWA.Clases;
using System.Collections.Generic;
using System.Linq;

namespace CBHWA.Models
{
    interface ItsysEmployeeCodesRepository
    {
        IList<tsysEmployeeCode> GetList(FieldFilters fieldFilters, string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        tsysEmployeeCode Get(int id);
        tsysEmployeeCode Add(tsysEmployeeCode employee);
        bool Remove(tsysEmployeeCode employee);
        tsysEmployeeCode Update(tsysEmployeeCode employee);
    }
}
