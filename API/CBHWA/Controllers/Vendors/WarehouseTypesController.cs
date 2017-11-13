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
    public class WarehouseTypesController : ApiController
    {
        static readonly VendorsRepository repository = new VendorsRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);
            int vendorkey = Convert.ToInt32(queryValues["vendorkey"]);

            string query = "";

            query = !string.IsNullOrWhiteSpace(queryValues["query"]) ? queryValues["query"] : "";

            int totalRecords = 0;

            try
            {
                IList<WarehouseType> lista;
                if (id > 0)
                {
                    lista = repository.GetWarehouseTypesById(id);
                }
                else if (vendorkey > 0)
                {
                    lista = repository.GetWarehouseTypesByVendor(vendorkey, ref totalRecords);
                }
                else
                {
                    lista = repository.GetWarehouseTypes();
                }

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