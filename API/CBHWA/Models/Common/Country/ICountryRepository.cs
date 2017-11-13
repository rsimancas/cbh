namespace CBHWA.Models
{
    using CBHWA.Clases;
    using System.Collections.Generic;

    interface ICountryRepository
    {
        IList<Country> GetAll();
        IList<Country> GetListForReport(string startDate, string endDate, string reportName, string query, int page, int start, int limit, ref int totalRecords);
    }
}