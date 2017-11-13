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
    public class JobStatusController : ApiController
    {
        static readonly JobStatusRepository repository = new JobStatusRepository();

        public object GetAll() 
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);

            string query = "";

            int totalRecords = 0;

            try
            {
                IList<JobStatus> lista ;

                if (id == 0)
                {
                    lista = repository.GetWithQuery(query, page, start, limit, ref totalRecords);
                }
                else
                {
                    JobStatus data = repository.Get(id);
                    lista = new List<JobStatus>() {data};
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

        public object Post(JobStatus model)
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

        public object Put(JobStatus model)
        {
            object json;

            try
            {
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
                        message = "",
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

        public object Delete(JobStatus model)
        {
            bool result = repository.Remove(model);

            object json = new
            {
                message = "",
                success = result
            };

            return json;
        }
    }
}