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
    public class CreateNewJobInvoiceController : ApiController
    {
        static readonly IJobsRepository repository = new JobsRepository();

        [HttpPost]
        public object Post(List<int> SelectedItems)
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int JobKey = Convert.ToInt32(queryValues["JobKey"]);
            string currentUser = queryValues["CurrentUser"];

            try
            {
                object json;
                bool success = repository.CreateNewJobInvoice(JobKey, currentUser, SelectedItems);

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