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
    public class JobEmployeeRolesController : ApiController
    {
        static readonly IJobsRepository repository = new JobsRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int jobkey = Convert.ToInt32(queryValues["jobkey"]);

            int totalRecords = 0;

            try
            {
                IList<JobEmployeeRoles> lista = repository.GetJobEmployeeRoles(jobkey, ref totalRecords);

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

        public object Post(JobEmployeeRoles model)
        {
            object json;
            model.JobEmployeeCreatedDate = DateTime.Now;
            model = repository.Add(model);

            json = new
            {
                data = model,
                success = true
            };

            return json;
        }

        public object Put(JobEmployeeRoles model)
        {
            object json;
            model.JobEmployeeModifiedDate = DateTime.Now;
            model = repository.Update(model);

            json = new
            {
                data = model,
                success = true
            };

            return json;
        }

        public object Delete(JobEmployeeRoles model)
        {
            bool result = repository.Remove(model);

            object json = new {
                success = result
            };

            return json;
        }
    }
}