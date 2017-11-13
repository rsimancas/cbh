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
    public class FileQuoteStatusHistoryController : ApiController
    {
        static readonly FileRepository repository = new FileRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);
            int filekey = Convert.ToInt32(queryValues["filekey"]);
            int QHdrKey = Convert.ToInt32(queryValues["QHdrKey"]);

            int totalRecords = 0;

            try
            {
                IList<FileQuoteStatusHistory> lista;

                if (id == 0)
                {
                    lista = repository.GetFileQuoteStatusHistory(filekey, QHdrKey, page, start, limit, ref totalRecords);
                }
                else
                {
                    FileQuoteStatusHistory data = repository.GetFileQuoteStatusHistoryById(id, page, start, limit, ref totalRecords);
                    lista = new List<FileQuoteStatusHistory>() { data };
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

        public object Post(FileQuoteStatusHistory model)
        {
            object json;
            string msgError = "";

            try
            {
                model.QStatusModifiedDate = DateTime.Now;
                model = repository.Add(model);

                if (model != null)
                {
                    json = new
                    {
                        total = 1,
                        data = model,
                        success = true
                    };
                }
                else
                {
                    json = new
                    {
                        message = msgError,
                        success = false
                    };
                };
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                object error = new { message = ex.Message };

                json = new
                {
                    message = ex.Message,
                    success = false
                };
            };

            return json;
        }

        public object Put(FileQuoteStatusHistory model)
        {
            object json;

            try
            {
                string msgError = "";
                model.QStatusModifiedDate = DateTime.Now;
                model = repository.Update(model);

                if (model != null)
                {
                    json = new
                    {
                        total = 1,
                        data = model,
                        success = true
                    };
                }
                else
                {
                    json = new
                    {
                        message = msgError,
                        success = false
                    };
                }
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                json = new
                {
                    message = ex.Message,
                    success = false
                };
            };

            return json;
        }

        public object Delete(FileQuoteStatusHistory model)
        {
            string msgError = "";

            bool result = repository.Remove(model);

            object json = new
            {
                message = msgError,
                success = result
            };

            return json;
        }
    }
}