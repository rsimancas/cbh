using CBHWA.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;

namespace CBHWA.Controllers
{
    [TokenValidation]
    public class LanguagesController : ApiController
    {
        static readonly ILanguageRepository repository = new LanguageRepository();

        public object GetAllLanguages()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            
            string query = "";

            string id = !string.IsNullOrWhiteSpace(queryValues["id"]) ? queryValues["id"] : null;
            query = !string.IsNullOrWhiteSpace(queryValues["query"]) ? queryValues["query"] : "";

            int totalRecords = 0;

            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    object json;

                    IList<Language> lista = repository.GetWithQuery(query, page, start, limit, ref totalRecords);
                    json = new
                    {
                        total = totalRecords,
                        data = lista,
                        success = true
                    };

                    return json;
                }
                else
                {
                    Language lang = repository.Get(id);
                    var lista = new List<Language>
                    {
                       lang
                    };

                    object json = new
                    {
                        total = 1,
                        data = lista,
                        success = true
                    };

                    return json;
                }
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