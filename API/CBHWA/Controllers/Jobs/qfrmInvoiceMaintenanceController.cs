using CBHWA.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;
using CBHWA.Mappings;
using CBHBusiness;
using Client = CBHBusiness.ClientModels;


namespace CBHWA.Controllers
{
    [TokenValidation]
    public class qfrmInvoiceMaintenanceController : ApiController
    {
        static readonly IJobsRepository repository = new JobsRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);

            string query = !string.IsNullOrWhiteSpace(queryValues["query"]) ? queryValues["query"] : "";

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
                if (id != 0)
                {
                    Client.qfrmInvoiceMaintenance invoice = new qfrmInvoiceMaintenanceMapping().MapModels(new InvoiceBusiness().GetInvoiceMaintenance(id));

                    object json = new
                    {
                        total = 1,
                        data = invoice,
                        success = true
                    };

                    return json;
                }
                else
                {
                    object json = new
                    {
                        total = totalRecords,
                        data = String.Empty,
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

        public object Post(InvoiceHeader model)
        {
            try
            {
                model.InvoiceCreatedDate = DateTime.Now;
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

        public object Put(Client.qfrmInvoiceMaintenance model)
        {
            try
            {
                model.InvoiceModifiedDate = DateTime.Now;

                object json = new
                {
                    total = 1,
                    data = new InvoiceBusiness().UpdateInvoiceMaintenance(model),
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

        public object Delete(InvoiceHeader model)
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