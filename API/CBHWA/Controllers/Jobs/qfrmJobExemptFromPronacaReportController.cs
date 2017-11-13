namespace CBHWA.Controllers
{
    using CBHWA.Clases;
    using CBHWA.Models;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Reflection;
    using System.Web.Http;
    using Utilidades;

    [TokenValidation]
    public class qfrmJobExemptFromPronacaReportController : ApiController
    {
        static readonly IJobsRepository repository = new JobsRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);
            string strFieldFilters = queryValues["fieldFilters"];

            FieldFilters fieldFilters = new FieldFilters();

            if (!String.IsNullOrEmpty(strFieldFilters))
            {
                fieldFilters = JsonConvert.DeserializeObject<FieldFilters>(strFieldFilters);
            }

            #region Configuramos query
            string query = !string.IsNullOrWhiteSpace(queryValues["query"]) ? queryValues["query"] : "";
            string strQueryBy = !string.IsNullOrWhiteSpace(queryValues["queryBy"]) ? queryValues["queryBy"] : "";
            string[] queryBy = (!string.IsNullOrWhiteSpace(strQueryBy)) ? strQueryBy.Split(',') : new string[] { };
            #endregion Configuramos query

            #region Configuramos el orden de la consulta si se obtuvo como parametro
            string strOrder = !string.IsNullOrWhiteSpace(queryValues["sort"]) ? queryValues["sort"] : "";
            strOrder = strOrder.Replace('[', ' ');
            strOrder = strOrder.Replace(']', ' ');

            Sort sort;

            if (!string.IsNullOrWhiteSpace(strOrder))
            {
                sort = JsonConvert.DeserializeObject<Sort>(strOrder);
            }
            else
            {
                sort = new Sort();
            }
            #endregion

            int totalRecords = 0;

            try
            {
                object json;

                if (id == 0)
                {

                    var lista = repository.GetJobExemptFromPronacaReport(fieldFilters, query, sort, queryBy, page, start, limit, ref totalRecords);

                    json = new
                    {
                        total = totalRecords,
                        data = lista,
                        success = true
                    };
                }
                else
                {
                    json = new
                    {
                        total = 0,
                        success = true
                    };
                }

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

        public object Post(qfrmJobExemptFromPronacaReport model)
        {
            object json;
            string msgError = "";

            try
            {
                if (model != null)
                {
                    json = new
                    {
                        total = 1,
                        data = model,
                        success = true
                    };
                }
                else
                {
                    json = new
                    {
                        message = msgError,
                        success = false
                    };
                };
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                object error = new { message = ex.Message };

                json = new
                {
                    message = ex.Message,
                    success = false
                };
            };

            return json;
        }

        public object Put(qfrmJobExemptFromPronacaReport model)
        {
            object json;

            try
            {
                model = repository.Update(model);

                json = new
                {
                    total = 1,
                    data = model,
                    success = true
                };
             }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                json = new
                {
                    message = ex.Message,
                    success = false
                };
            };

            return json;
        }

        public object Delete(qfrmJobExemptFromPronacaReport model)
        {
            string msgError = "";

            bool result = false;

            object json = new
            {
                message = msgError,
                success = result
            };

            return json;
        }
    }
}