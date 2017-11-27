using CBHWA.Models;
using CBHWA.Mappings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;
using CBHBusiness;
using Client = CBHBusiness.ClientModels;

namespace CBHWA.Controllers
{

    [TokenValidation]
    public class qfrmFileQuoteConfirmationSVInfoController : ApiController
    {
        static readonly IFileRepository repository = new FileRepository();
        static readonly qfrmFileQuoteConfirmationMapping mappings = new qfrmFileQuoteConfirmationMapping();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int QHdrKey = Convert.ToInt32(queryValues["QHdrKey"]);
            int VendorKey = Convert.ToInt32(queryValues["VendorKey"]);

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
                if (VendorKey == 0)
                {
                    object json;
                    IList<qfrmFileQuoteConfirmationSVInfo> lista;

                    lista = repository.GetqfrmFileQuoteConfirmationSVInfos(QHdrKey, query, sort, filter, page, start, limit, ref totalRecords);

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
                    qfrmFileQuoteConfirmationSVInfo genericList = repository.GetqfrmFileQuoteConfirmationSVInfo(QHdrKey, VendorKey);

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

        public object Put(Client.qfrmFileQuoteConfirmationSubVendorInfo model)
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