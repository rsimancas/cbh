using System.Collections.Generic;

namespace CBHWA.Models
{
    interface ILanguageRepository
    {
        IList<Language> GetWithQuery(string query, int page, int start, int limit, ref int totalRecords);
        Language Get(string id);
        Language Add(Language language);
        void Remove(string id);
        bool Update(Language language);
    }
}
