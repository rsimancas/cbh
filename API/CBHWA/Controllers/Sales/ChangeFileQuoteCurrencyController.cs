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
    public class ChangeFileQuoteCurrencyController : ApiController
    {
        static readonly IFileRepository repository = new FileRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int QHdrKey = Convert.ToInt32(queryValues["QHdrKey"]);
            string currentUser = queryValues["CurrentUser"];
            string currency = queryValues["CurrencyCode"];
            decimal rate = Convert.ToDecimal(queryValues["CurrencyRate"]);

            try
            {
                object json;
                bool success = repository.ChangeFileQuoteCurrency(QHdrKey, currency, rate, currentUser);

                json = new
                {
                    data = repository.Get(QHdrKey),
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