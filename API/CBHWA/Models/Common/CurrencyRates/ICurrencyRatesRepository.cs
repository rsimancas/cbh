using System.Collections.Generic;

namespace CBHWA.Models
{
    interface ICurrencyRatesRepository
    {
        IList<CurrencyRates> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        CurrencyRates Get(string id);
        CurrencyRates Add(CurrencyRates added);
        bool Remove(CurrencyRates deleted);
        CurrencyRates Update(CurrencyRates updated);
    }
}
