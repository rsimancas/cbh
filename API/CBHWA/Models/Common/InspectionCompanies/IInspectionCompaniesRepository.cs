using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IInspectionCompaniesRepository
    {
        IList<InspectionCompany> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        InspectionCompany Get(int id);
        InspectionCompany Add(InspectionCompany added);
        bool Remove(InspectionCompany deleted);
        InspectionCompany Update(InspectionCompany updated);
    }
}
