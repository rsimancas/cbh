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
    public class UpdateJobCurrencyRatesController : ApiController
    {
        static readonly IJobsRepository repository = new JobsRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int JobKey = Convert.ToInt32(queryValues["JobKey"]);
            int UserCurrentRates = Convert.ToInt32((queryValues["UseCurrentRates"]));

            try
            {

                repository.UpdateJobCurrencyRates(JobKey, UserCurrentRates == 1);

                var json = new
                {
                    success = true
                };

                return json;
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                var json = new
                {
                    message = ex.Message,
                    success = false
                };

                return json;
            }
        }
    }
}