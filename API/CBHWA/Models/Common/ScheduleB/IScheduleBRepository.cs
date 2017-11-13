using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IScheduleBRepository
    {
        #region ScheduleB
        IList<ScheduleB> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        ScheduleB Get(string id);
        ScheduleB Add(ScheduleB added);
        bool Remove(ScheduleB deleted);
        ScheduleB Update(ScheduleB updated);
        #endregion ScheduleB

        #region SBLanguage
        IList<SBLanguage> GetByParent(string SchBNum, Sort sort, int page, int start, int limit, ref int totalRecords);
        SBLanguage Get(int id);
        SBLanguage Add(SBLanguage added);
        bool Remove(SBLanguage deleted);
        SBLanguage Update(SBLanguage updated);
        #endregion ScheduleB

    }
}
