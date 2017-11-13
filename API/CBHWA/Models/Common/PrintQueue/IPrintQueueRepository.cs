using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IPrintQueueRepository
    {
        IList<PrintQueue> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        PrintQueue Get(int id);
        PrintQueue Add(PrintQueue added);
        bool Remove(PrintQueue deleted);
        PrintQueue Update(PrintQueue updated);
    }
}
