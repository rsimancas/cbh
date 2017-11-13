using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IShipmentTypesRepository
    {
        IList<ShipmentType> GetList(string query, int page, int start, int limit, ref int totalRecords);
        ShipmentType Get(int id);
        ShipmentType Add(ShipmentType added);
        bool Remove(ShipmentType deleted);
        ShipmentType Update(ShipmentType updated);
    }
}
