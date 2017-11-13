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
    public class FileEmployeeRolesController : ApiController
    {
        static readonly IFileRepository repository = new FileRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int filekey = Convert.ToInt32(queryValues["filekey"]);

            int totalRecords = 0;

            try
            {
                IList<FileEmployeeRoles> lista = repository.GetFileEmployeeRoles(filekey, ref totalRecords);

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

        public object Post(FileEmployeeRoles model)
        {
            object json;
            model.FileEmployeeCreatedDate = DateTime.Now;
            model = repository.Add(model);

            json = new
            {
                data = model,
                success = true
            };

            return json;
        }

        public object Put(FileEmployeeRoles model)
        {
            object json;
            model.FileEmployeeModifiedDate = DateTime.Now;
            model = repository.Update(model);

            json = new
            {
                data = model,
                success = true
            };

            return json;
        }

        public object Delete(FileEmployeeRoles model)
        {
            bool result = repository.Remove(model);

            object json = new {
                success = result
            };

            return json;
        }
    }
}