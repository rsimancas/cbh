using CBHWA.Models;
using Newtonsoft.Json;
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
    public class JobRolesController : ApiController
    {
        static readonly IJobRoleRepository repository = new JobRoleRepository();

        //public object GetAll()
        //{
        //    var queryValues = Request.RequestUri.ParseQueryString();

        //    int page = Convert.ToInt32(queryValues["page"]);
        //    int start = Convert.ToInt32(queryValues["start"]);
        //    int limit = Convert.ToInt32(queryValues["limit"]);

        //    int totalRecords = 0;

        //    try
        //    {
        //        IList<JobRole> jobroles = repository.GetAll(page, start, limit, ref totalRecords);

        //        object json = new
        //        {
        //            total = totalRecords,
        //            data = jobroles
        //        };

        //        return json;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
        //        return null;
        //    }
        //}

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);

            string query = "";

            #region Configuramos el orden de la consulta si se obtuvo como parametro
            string strOrder = !string.IsNullOrWhiteSpace(queryValues["sort"]) ? queryValues["sort"] : "";
            strOrder = strOrder.Replace('[', ' ');
            strOrder = strOrder.Replace(']', ' ');

            Sort sort;

            if (!string.IsNullOrWhiteSpace(strOrder))
            {
                sort = JsonConvert.DeserializeObject<Sort>(strOrder);
            }
            else
            {
                sort = new Sort();
            }
            #endregion

            query = !string.IsNullOrWhiteSpace(queryValues["query"]) ? queryValues["query"] : "";

            int totalRecords = 0;

            try
            {
                if (id == 0)
                {
                    object json;

                    var lista = repository.GetList(query, sort, page, start, limit, ref totalRecords);
                    json = new
                    {
                        total = totalRecords,
                        data = lista,
                        success = true
                    };

                    return json;
                }
                else
                {
                    var model = repository.Get(id);
                    var lista = new List<JobRole>
                    {
                       model
                    };

                    object json = new
                    {
                        total = 1,
                        data = lista,
                        success = true
                    };

                    return json;
                }
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

        public object Post(JobRole model)
        {
            try
            {
                model.JobRoleCreatedDate = DateTime.Now;
                model = repository.Add(model);

                object json = new
                {
                    total = 1,
                    data = model,
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

        public object Put(JobRole model)
        {
            try
            {
                model.JobRoleModifiedDate = DateTime.Now;
                model = repository.Update(model);

                object json = new
                {
                    total = 1,
                    data = model,
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

        public object Delete(JobRole model)
        {
            bool result = repository.Remove(model);

            object json = new
            {
                success = result
            };

            return json;
        }

        //public HttpResponseMessage GetJobRole(int id)
        //{
        //    JobRole JobRole = repository.Get(id);
        //    if (JobRole == null)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "JobRole Not found for the Given ID");
        //    }

        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, JobRole);
        //    }
        //}

        //public HttpResponseMessage PostJobRole(JobRole model)
        //{
        //    model.JobRoleCreatedDate = DateTime.Now;
        //    model = repository.Add(model);
        //    var response = Request.CreateResponse<JobRole>(HttpStatusCode.Created, model);
        //    string uri = Url.Link("DefaultApi", new { id = model.JobRoleKey });
        //    response.Headers.Location = new Uri(uri);
        //    return response;
        //}

        //public HttpResponseMessage PutJobRole(int id, JobRole model)
        //{
        //    model.JobRoleModifiedDate = DateTime.Now;
        //    model.JobRoleKey = id;
        //    if (!repository.Update(model))
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to Update the JobRole for the Given ID");
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }
        //}

        //public HttpResponseMessage DeleteProduct(int id)
        //{
        //    repository.Remove(id);
        //    return new HttpResponseMessage(HttpStatusCode.NoContent);
        //}
    }
}