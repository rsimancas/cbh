using CBHWA.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;

namespace CBHWA.Controllers
{
    [TokenValidation]
    public class FileQuoteDetailsController : ApiController
    {
        static readonly IFileRepository repository = new FileRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int filekey = Convert.ToInt32(queryValues["filekey"]);
            int vendorkey = Convert.ToInt32(queryValues["vendorkey"]);
            int id = Convert.ToInt32(queryValues["id"]);

            int totalRecords = 0;

            try
            {
                IList<FileQuoteDetail> lista;

                if (id == 0)
                {
                    lista = repository.GetQuoteDetails(filekey, vendorkey);
                } else {
                    lista = new List<FileQuoteDetail>();
                    lista.Add(repository.GetQuoteDetail(id));
                }

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

        public object Post(FileQuoteDetail model)
        {
            object json;
            try 
            {
                model = repository.Add(model);

                json = new
                {
                    total = 1,
                    data = model,
                    success = (model == null) ? false : true
                };

            } catch (Exception ex) {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                json = new
                {
                    total = 0,
                    data = "",
                    success = false
                };
            };

            return json;
        }

        public object Put(FileQuoteDetail model)
        {
            try
            {
                model = repository.Update(model);

                object json = new
                {
                    data = model,
                    success = true
                };

                return json;
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                object json = new
                {
                    data = "",
                    success = false
                };

                return json;
            };
        }

        public HttpResponseMessage Delete(FileQuoteDetail model)
        {
            repository.Remove(model);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
    }
}