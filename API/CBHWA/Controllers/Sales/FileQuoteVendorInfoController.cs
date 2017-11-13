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
    public class FileQuoteVendorInfoController : ApiController
    {
        static readonly IFileRepository repository = new FileRepository();

        public object GetAll() 
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int vendorkey = Convert.ToInt32(queryValues["vendorkey"]);
            int filekey = Convert.ToInt32(queryValues["filekey"]);
            int ShowOnlyWithQuotes = Convert.ToInt32(queryValues["ShowOnlyWithQuotes"]);

            int totalRecords = 0;

            try
            {
                IList<FileQuoteVendorInfo> lista ;

                if (vendorkey == 0)
                {
                    lista = repository.GetFileQuoteVendorInfo(filekey, ShowOnlyWithQuotes, page, start, limit, ref totalRecords);
                }
                else
                {
                    FileQuoteVendorInfo data = repository.GetFileQuoteVendorInfoById(filekey, vendorkey);
                    totalRecords = (data != null) ? 1 : 0;
                    lista = new List<FileQuoteVendorInfo>() {data};
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

        public object Post(FileQuoteVendorInfo model)
        {
            object json;
            string msgError = "";

            try
            {
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

        public object Put(FileQuoteVendorInfo model)
        {
            object json;

            try
            {
                string msgError = "";
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

        public object Delete(FileQuoteVendorInfo model)
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