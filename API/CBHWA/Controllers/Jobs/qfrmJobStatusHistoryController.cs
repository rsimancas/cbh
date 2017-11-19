using Client = CBHWA.Models;
using CBHBusiness;
using CBHWA.Mappings;
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
        static readonly Client.IJobsRepository repository = new Client.JobsRepository();
        static readonly qfrmJobStatusHistoryMapping mapping = new qfrmJobStatusHistoryMapping();

        public object GetAll() 
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);

            try
            {
                IList<Client.qfrmJobStatusHistory> data = new List<Client.qfrmJobStatusHistory>();

                if (id != 0)
                {
                    var file = repository.GetqfrmJobStatusHistory(id);

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

        public object Post(Client.qfrmJobStatusHistory model)
        {
            try
            {
                var clientModel = mapping.MapModels(model);

                object json = new
                {
                    total = 1,
                    data = mapping.MapModels(new qfrmJobStatusHistoryBusiness().Update(clientModel)),
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