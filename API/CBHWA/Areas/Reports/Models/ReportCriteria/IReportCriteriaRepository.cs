namespace CBHWA.Areas.Reports.Models
{
    using CBHWA.Models;
    using System.Collections.Generic;
    interface IReportCriteriaRepository
    {
        IList<Criteria> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        Criteria Get(int id);
        Criteria Add(Criteria model);
        bool Remove(Criteria model);
        Criteria Update(Criteria model);
    }
}
