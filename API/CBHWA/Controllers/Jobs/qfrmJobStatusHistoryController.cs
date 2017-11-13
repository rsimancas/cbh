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
    public class qfrmJobStatusHistoryController : ApiController
    {
        static readonly IJobsRepository repository = new JobsRepository();

        public object GetAll() 
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);

            try
            {
                IList<qfrmJobStatusHistory> data = new List<qfrmJobStatusHistory>();

                if (id != 0)
                {
                    qfrmJobStatusHistory file = repository.GetqfrmJobStatusHistory(id);

                    if (file != null) data.Add(file);
                }

                object json = new
                {
                    total = 1,
                    data = data,
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