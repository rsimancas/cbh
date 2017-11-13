using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IFreightDestinationsRepository
    {
        IList<FreightDestinations> GetList(string query, int page, int start, int limit, ref int totalRecords);
        FreightDestinations Get(int id);
        FreightDestinations Add(FreightDestinations added);
        bool Remove(FreightDestinations deleted);
        FreightDestinations Update(FreightDestinations updated);
    }
}
