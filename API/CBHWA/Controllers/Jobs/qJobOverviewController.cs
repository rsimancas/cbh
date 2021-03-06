﻿using CBHWA.Models;
using CBHWA.Mappings;
using CBHBusiness;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;

namespace CBHWA.Controllers
{
    [TokenValidation]
    public class qJobOverviewController : ApiController
    {
        static readonly IJobsRepository repository = new JobsRepository();
        static readonly qfrmJobOverviewMapping mapping = new qfrmJobOverviewMapping();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int id = Convert.ToInt32(queryValues["id"]);

            try
            {
                IList<qJobOverview> jobOverview = repository.GetJobOverview(id);

                object json = new
                {
                    total = 1,
                    data = jobOverview,
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

        public object Put(qJobOverview model)
        {
            try
            {
                object json = new
                {
                    total = 1,
                    data = mapping.MapModels(new qfrmJobOverviewBusiness().Update(mapping.MapModels(model))),
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