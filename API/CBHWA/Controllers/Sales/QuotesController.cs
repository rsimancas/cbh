using CBHWA.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;

namespace CBHWA.Controllers
{
    [TokenValidation]
    public class QuotesController : ApiController
    {
        static readonly IFileRepository repository = new FileRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);

            var jsonfilter = (queryValues["filter"]);
            jsonfilter = jsonfilter.Substring(1,jsonfilter.Length-2);

            int id = 0;
 
            try
            {
                JObject o = JObject.Parse(jsonfilter);
                id = Convert.ToInt32(o["value"]);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            

            int totalRecords = 0;

            try
            {
                IList<Quotes> lista = repository.GetQuotes(id);

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

        public object Delete(Quotes model)
        {
            string msgError = "";

            bool result = repository.Remove(model);

            object json = new
            {
                message = msgError,
                success = result
            };

            return json;
        }
    }
}