using System.Collections.Generic;
using System.Linq;

namespace CBHWA.Models
{
    interface IJobRoleRepository
    {
        IList<JobRole> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        JobRole Get(int id);
        JobRole Add(JobRole model);
        bool Remove(JobRole model);
        JobRole Update(JobRole model);
    }
}
