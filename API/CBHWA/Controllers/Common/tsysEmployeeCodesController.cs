using CBHWA.Clases;
using CBHWA.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;

namespace CBHWA.Controllers
{
    [TokenValidation]
    public class tsysEmployeeCodesController : ApiController
    {
        static readonly ItsysEmployeeCodesRepository repo = new tsysEmployeeCodeRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);
            string query = !string.IsNullOrWhiteSpace(queryValues["query"]) ? queryValues["query"] : "";
            string strFieldFilters = queryValues["fieldFilters"];

            FieldFilters fieldFilters = new FieldFilters();

            if (!String.IsNullOrEmpty(strFieldFilters))
            {
                fieldFilters = JsonConvert.DeserializeObject<FieldFilters>(strFieldFilters);
            }

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
                if (id == 0)
                {
                    object json;

                    IList<tsysEmployeeCode> lista = repo.GetList(fieldFilters, query, sort, page, start, limit, ref totalRecords);
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
                    tsysEmployeeCode desc = repo.Get(id);
                    var lista = new List<tsysEmployeeCode>();

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

        public object Post(tsysEmployeeCode model)
        {
            try
            {
                model = repo.Add(model);

                object json = new
                {
                    total = 1,
                    data = model,
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

        public object Put(tsysEmployeeCode model)
        {
            try
            {
                model = repo.Update(model);

                object json = new
                {
                    total = 1,
                    data = model,
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

        public object Delete(tsysEmployeeCode model)
        {
            bool result = repo.Remove(model);

            object json = new
            {
                success = result
            };

            return json;
        }
    }
}