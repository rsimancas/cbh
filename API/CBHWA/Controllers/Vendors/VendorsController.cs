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
    public class VendorsController : ApiController
    {
        static readonly IVendorsRepository repository = new VendorsRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);
            int quoteFileKey = Convert.ToInt32(queryValues["QuoteFileKey"]);
            int VendorCarrier = Convert.ToInt32(queryValues["VendorCarrier"]);
            int QHdrKey = Convert.ToInt32(queryValues["QHdrKey"]);


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
                if (id == 0)
                {
                    object json;
                    IList<Vendor> lista;

                    lista = repository.GetList(query, sort, queryBy, page, start, limit, ref totalRecords, quoteFileKey, VendorCarrier, QHdrKey);

                    if (lista != null)
                    {
                        foreach (var address in lista)
                        {
                            if (!string.IsNullOrEmpty(address.VendorAddress1))
                            {
                                var index = address.VendorAddress1.IndexOf(Environment.NewLine);
                                address.VendorAddress1 = (index == -1) ? address.VendorAddress1 : address.VendorAddress1.Substring(0, index + 1);
                            }
                        }
                    }


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
                    var vendor = repository.Get(id);

                    if (vendor != null)
                    {
                        if (!string.IsNullOrEmpty(vendor.VendorAddress1))
                        {
                            var index = vendor.VendorAddress1.IndexOf(Environment.NewLine);
                            vendor.VendorAddress1 = (index == -1) ? vendor.VendorAddress1 : vendor.VendorAddress1.Substring(0, index + 1);
                        }
                    }

                    object json = new
                    {
                        total = 1,
                        data = vendor,
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

        public object Post(Vendor model)
        {
            try
            {
                model.VendorCreatedDate = DateTime.Now;
                model = repository.Add(model);

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

        public object Put(Vendor model)
        {
            try
            {
                model.VendorModifiedDate = DateTime.Now;
                model = repository.Update(model);

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

        public object Delete(Vendor model)
        {
            string msgError = "";

            bool result = repository.Remove(model, ref msgError);

            object json = new
            {
                message = msgError,
                success = result
            };

            return json;
        }
    }
}