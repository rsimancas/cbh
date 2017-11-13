using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IItemRepository
    {
        IList<Item> GetList(string query, string ItemNum, int vendorkey, Sort sort, Filter filter, string[] queryBy, int page, int start, int limit, ref int totalRecords);
        Item Get(int id);
        IList<Item> GetByVendor(int vendorkey);
        Item Add(Item fileheader);
        Item Update(Item fileheader);
        bool Remove(Item item);

        ItemDescription GetItemDescription(int id);
        IList<ItemDescription> GetDescriptions(string query, int itemkey, int page, int start, int limit, ref int totalRecords);
        ItemDescription Add(ItemDescription data);
        ItemDescription Update(ItemDescription data);
        bool Remove(ItemDescription data);

        IList<qlstScheduleBImport> GetListScheduleB(string query, Sort sort, Filter filter, string[] queryBy, int page, int start, int limit, ref int totalRecords);
        qlstScheduleBImport GetScheduleB(int id);
    }
}
