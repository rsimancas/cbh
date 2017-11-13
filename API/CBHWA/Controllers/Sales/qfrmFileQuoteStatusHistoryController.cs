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
    public class qfrmFileQuoteStatusHistoryController : ApiController
    {
        static readonly FileRepository repository = new FileRepository();

        public object GetAll() 
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);

            try
            {
                IList<qfrmFileQuoteStatusHistory> data = new List<qfrmFileQuoteStatusHistory>();

                if (id != 0)
                {
                    qfrmFileQuoteStatusHistory file = repository.GetqfrmFileQuoteStatusHistory(id);

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