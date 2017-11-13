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
    public class CheckConfirmedVendorController : ApiController
    {
        static readonly IFileRepository repository = new FileRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int QHdrKey = Convert.ToInt32(queryValues["QHdrKey"]);
            int VendorKey = Convert.ToInt32(queryValues["VendorKey"]);
            int FileKey = Convert.ToInt32(queryValues["FileKey"]);

            try
            {
                object json;
                bool confirmed = repository.CheckConfirmedVendor(FileKey, QHdrKey, VendorKey);

                if (!confirmed) throw new Exception("Error from confirm");

                json = new
                {
                    success = true
                };

                return json;
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

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