using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IJobStatusRepository
    {
        IList<JobStatus> GetWithQuery(string query, int page, int start, int limit, ref int totalRecords);
        JobStatus Get(int id);
        JobStatus Add(JobStatus modle);
        bool Remove(JobStatus model);
        JobStatus Update(JobStatus model);

    }
}
