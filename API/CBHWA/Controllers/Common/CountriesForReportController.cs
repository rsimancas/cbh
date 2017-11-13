namespace CBHWA.Controllers
{
    using CBHWA.Clases;
    using CBHWA.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Reflection;
    using System.Web.Http;
    using Utilidades;

    [TokenValidation]
    public class CountriesForReportController : ApiController
    {
        static readonly ICountryRepository repository = new CountryRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);
            string startDate = queryValues["startDate"];
            string endDate = queryValues["endDate"];
            string reportName = queryValues["reportName"];

            #region Configuramos query
            string query = !string.IsNullOrWhiteSpace(queryValues["query"]) ? queryValues["query"] : "";
            #endregion Configuramos query

            int totalRecords = 0;

            try
            {
                var lista = repository.GetListForReport(startDate, endDate, reportName, query, page, start, limit, ref totalRecords);

                object json = new
                {
                    total = totalRecords,
                    data = lista,
                    success = true
                };

                return json;
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                object error = new { message = ex.Message };

                object json = new
                {
                    message = ex.Message,
                    success = false
                };

                return json;
            }
        }
    }
}