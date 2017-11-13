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
    public class ConvertQuoteToOrderController : ApiController
    {
        static readonly IFileRepository repository = new FileRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int QHdrKey = Convert.ToInt32(queryValues["QHdrKey"]);
            string currentUser = queryValues["CurrentUser"];
            string JobNum = queryValues["JobNum"];

            try
            {
                object json;
                int JobKey = repository.CreateJobFromQuote(QHdrKey, currentUser, JobNum);

                json = new
                {
                    success = true,
                    JobKey = JobKey
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