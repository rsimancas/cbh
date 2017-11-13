using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IStatusRepository
    {
        IList<Status> GetWithQuery(string query, int category, int page, int start, int limit, ref int totalRecords);
        Status Get(int id);
        Status Add(Status added);
        bool Remove(Status deleted);
        Status Update(Status updated);

    }
}
