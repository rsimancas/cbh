using CBHWA.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;

namespace CBHWA.Controllers
{
    
    [TokenValidation]
    public class FileQuoteItemsSummaryController : ApiController
    {
        static readonly IFileRepository repository = new FileRepository();

        public object GetAll()
        {
           var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);

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

            #region Configuramos el filtro de la consulta si se obtuvo como parametro
            string strFilter = !string.IsNullOrWhiteSpace(queryValues["filter"]) ? queryValues["filter"] : "";
            strFilter = strFilter.Replace('[', ' ');
            strFilter = strFilter.Replace(']', ' ');

            Filter filter;

            if (!string.IsNullOrWhiteSpace(strFilter))
            {
                filter = JsonConvert.DeserializeObject<Filter>(strFilter);
            }
            else
            {
                filter = new Filter();
            }
            #endregion Configuramos el filtro de la consulta si se obtuvo como parametro

            int totalRecords = 0;

            try
            {
                if (id == 0)
                {
                    object json;
                    IList<FileQuoteItemsSummary> lista;

                    lista = repository.GetListQuoteItemsSummary(query, sort, filter, page, start, limit, ref totalRecords);

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
                    FileQuoteItemsSummary qitem = repository.GetQuoteItemsSummary(id);

                    object json = new
                    {
                        total = 1,
                        data = qitem,
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

        public object Post(FileQuoteItemsSummary model)
        {
            object json;
            string messageError = "";

            try
            {
                model = repository.Add(model, ref messageError);

                if (model != null)
                {
                    json = new
                    {
                        total = 1,
                        data = model,
                        success = true
                    };
                } else {
                    json = new
                    {
                        message = messageError,
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

        public object Put(FileQuoteItemsSummary model)
        {
            object json;

            try
            {
                string messageError = "";
                model = repository.Update(model, ref messageError);

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
                        message = messageError,
                        success = false
                    };
                }
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

        public object Delete(FileQuoteItemsSummary model)
        {
            bool result = repository.Remove(model);

            object json = new
            {
                success = result
            };

            return json;
        }
    }
}