﻿using CBHWA.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;

namespace CBHWA.Controllers
{
    [TokenValidation]
    public class JobPurchaseOrderStatusHistoryController : ApiController
    {
        static readonly IJobsRepository repository = new JobsRepository();

        public object GetAll() 
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);
            int POKey = Convert.ToInt32(queryValues["POKey"]);

            try
            {
                IList<JobPurchaseOrderStatusHistory> lista ;

                if (id == 0)
                {
                    lista = repository.GetListJobPurchaseOrderStatusHistory(POKey);
                }
                else
                {
                    JobPurchaseOrderStatusHistory data = repository.GetJobPurchaseOrderStatusHistory(id);
                    lista = new List<JobPurchaseOrderStatusHistory>() { data };
                }

                

                object json = new
                {
                    total = lista.Count,
                    data = lista,
                    success = true
                };

                return json;
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                object json = new
                {
                    message = ex.Message,
                    success = false
                };

                return json;
            }
        }

        public object Post(JobPurchaseOrderStatusHistory model)
        {
            object json;
            string msgError = "";

            try
            {
                model.POStatusModifiedDate = DateTime.Now;
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

        public object Put(JobPurchaseOrderStatusHistory model)
        {
            object json;

            try
            {
                string msgError = "";
                model.POStatusModifiedDate = DateTime.Now;
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

        public object Delete(JobPurchaseOrderStatusHistory model)
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