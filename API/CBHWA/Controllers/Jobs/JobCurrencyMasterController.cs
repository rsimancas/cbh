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
    public class JobCurrencyMasterController : ApiController
    {
        static readonly IJobsRepository repository = new JobsRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int JobKey = Convert.ToInt32(queryValues["JobKey"]);

            try
            {
                IList<JobCurrencyMaster> lista = repository.GetListJobCurrencyMaster(JobKey);

                object json = new
                {
                    data = lista,
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

        public object Put(JobCurrencyMaster model)
        {
            object json;
            model = repository.Update(model);

            json = new
            {
                data = model,
                success = true
            };

            return json;
        }
    }
}