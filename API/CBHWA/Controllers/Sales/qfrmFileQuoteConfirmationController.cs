using Models = CBHWA.Models;
using CBHWA.Mappings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;
using Client = CBHBusiness.ClientModels;
using CBHBusiness;


namespace CBHWA.Controllers
{
    
    [TokenValidation]
    public class qfrmFileQuoteConfirmationController : ApiController
    {
        static readonly Models.IFileRepository repository = new Models.FileRepository();
        static readonly qfrmFileQuoteConfirmationMapping mappings = new qfrmFileQuoteConfirmationMapping();

        public object GetAll()
        {
           var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["QHdrKey"]);

            #region Configuramos query
            string query = !string.IsNullOrWhiteSpace(queryValues["query"]) ? queryValues["query"] : "";
            string strQueryBy = !string.IsNullOrWhiteSpace(queryValues["queryBy"]) ? queryValues["queryBy"] : "";
            string[] queryBy = (!string.IsNullOrWhiteSpace(strQueryBy)) ? strQueryBy.Split(',') : new string[] { };
            #endregion Configuramos query

            #region Configuramos el orden de la consulta si se obtuvo como parametro
            string strOrder = !string.IsNullOrWhiteSpace(queryValues["sort"]) ? queryValues["sort"] : "";
            strOrder = strOrder.Replace('[', ' ');
            strOrder = strOrder.Replace(']', ' ');

            Models.Sort sort;

            if (!string.IsNullOrWhiteSpace(strOrder))
            {
                sort = JsonConvert.DeserializeObject<Models.Sort>(strOrder);
            }
            else
            {
                sort = new Models.Sort();
            }
            #endregion

            #region Configuramos el filtro de la consulta si se obtuvo como parametro
            string strFilter = !string.IsNullOrWhiteSpace(queryValues["filter"]) ? queryValues["filter"] : "";
            strFilter = strFilter.Replace('[', ' ');
            strFilter = strFilter.Replace(']', ' ');

            Models.Filter filter;

            if (!string.IsNullOrWhiteSpace(strFilter))
            {
                filter = JsonConvert.DeserializeObject<Models.Filter>(strFilter);
            }
            else
            {
                filter = new Models.Filter();
            }
            #endregion Configuramos el filtro de la consulta si se obtuvo como parametro

            int totalRecords = 0;

            try
            {
                if (id == 0)
                {
                    object json;
                    IList<Models.qfrmFileQuoteConfirmation> lista;

                    lista = repository.GetqfrmFileQuoteConfirmations(query, sort, filter, page, start, limit, ref totalRecords);

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
                    Models.qfrmFileQuoteConfirmation genericList = repository.GetqfrmFileQuoteConfirmation(id);

                    object json = new
                    {
                        total = 1,
                        data = genericList,
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

        public object Put(Client.qfrmFileQuoteConfirmation model)
        {
            try
            {
                object json = new
                {
                    total = 1,
                    data = mappings.MapModels(new qfrmFileQuoteConfirmationBusiness().Update(model)),
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