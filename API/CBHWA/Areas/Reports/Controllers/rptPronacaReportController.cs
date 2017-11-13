﻿namespace CBHWA.Areas.Reports.Controllers
{
    using CBHWA.Areas.Reports.Models;
    using CBHWA.Clases;
    using CBHWA.Models;
    using Microsoft.Reporting.WebForms;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using Utilidades;

    public class rptPronacaReportController : Controller
    {
        static readonly IUserRepository userRepository = new UserRepository();
        static readonly IEmployeeRepository employeeRepo = new EmployeeRepository();
        static User currentUser = null;

        ////
        //// POST: /Reports/rptPronacaReportClosed/
        public ActionResult Index()
        {
            var nvc = Request.QueryString;

            string strWhere = nvc["strWhere"] ?? "rptDate > CAST('2012-12-31' AS DATE) AND rptDate < CAST('2013-04-09' AS DATE)";
            string labelCriteria = nvc["labelCriteria"] ?? "";
            string strEmployeeKey = nvc["employeeKey"] ?? "";
            int employeeKey = 0;
            int.TryParse(strEmployeeKey, out employeeKey);
            labelCriteria = Uri.EscapeUriString(labelCriteria);

            var model = new CriteriaParam();
            model.labelCriteria = labelCriteria;
            model.employeeKey = employeeKey;
            model.strWhere = Uri.UnescapeDataString(strWhere);
            model.startDate = nvc["startDate"];
            model.endDate = nvc["endDate"];


            string strNoProfit = nvc["NoProfit"] ?? "0";
            int NoProfit = 0;
            int.TryParse(strNoProfit, out NoProfit);
            model.NoProfit = NoProfit;

            var filters = new FieldFilters();
            filters.fields = new List<FieldFilter>();
            filters.fields.Add(new FieldFilter { name = "EmployeeStatusCode", oper = "=", type = "int", value = "8" });
            var sort = new Sort();
            int totalRecords = 0;
            var employeeList = employeeRepo.GetList(filters, null, sort, 0, 0, 0, ref totalRecords);
            ViewBag.Employees = employeeList;

            return View(model);
        }

        public ActionResult PDF(int? employeeKey, string labelCriteria, string strWhere, string startDate, string endDate, int? NoProfit)
        {
            var nvc = Request.QueryString;
            strWhere = strWhere ?? "rptDate > CAST('2012-12-31' AS DATE) AND rptDate < CAST('2013-04-09' AS DATE)";
            labelCriteria = labelCriteria ?? "";
            employeeKey = employeeKey ?? 0;
            startDate = startDate ?? "12-31-2012";
            endDate = endDate ?? "04-09-2013";
            NoProfit = NoProfit.GetValueOrDefault();

            DataTable dtHeader = GetPronacaReport(strWhere, labelCriteria, employeeKey.Value, startDate, endDate, NoProfit.Value);

            LocalReport lr = new LocalReport();

            lr.ReportPath = "Areas/Reports/ReportDesign/rptPronacaReport.rdlc";

            //lr.DataSources.Clear();
            ReportParameter param1 = new ReportParameter("NoProfit", NoProfit.ToString());
            lr.SetParameters(new ReportParameter[] { param1 });
            lr.DataSources.Add(new ReportDataSource("dsRptPronacaReport", dtHeader));

            //lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_SubreportProcessing);
            //lr.SubreportProcessing += lr_SubreportProcessing;

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>rptPronacaReportClosed</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.2in</MarginTop>" +
            "  <MarginLeft>0.2in</MarginLeft>" +
            "  <MarginRight>0.2in</MarginRight>" +
            "  <MarginBottom>0.2in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] bytes;

            bytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(bytes, "application/pdf", "rptPronacaReport");
        }

        public ActionResult Error()
        {
            return View();
        }

        void lr_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            int id = 0;
            int.TryParse(e.Parameters["FileKey"].Values[0].ToString(), out id);
            if (e.ReportPath == "rptPronacaReportClosedSubQuotes")
            {
                var dt = GetPronacaReportClosedSubQuotes(id);
                e.DataSources.Add(new ReportDataSource("dsRptPronacaReportClosedSubQuotes", dt));
            }
        }

        private DataTable GetPronacaReport(string filterWhere, string labelCriteria, int employeeKey, string startDate, string endDate, int NoProfit)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"DECLARE @LocationFooter NVARCHAR(MAX) = dbo.fnGetLocationFooter(1);
                                WITH qData
                                AS
                                (
                                    SELECT a.*
                                   FROM qrptPronacaReport a
                                   WHERE {0} 
                                )
                                SELECT a.*
                                    ,ROW_NUMBER() OVER (ORDER BY a.CustKey, a.FileNum) as Row
                                    ,'{2}' as LabelCriteria
                                    ,'{3}' as LabelCreatedDate
                                    ,@LocationFooter as LocationFooter
                                    ,dbo.fnPronacaProfitMargin(
                                        JobNum
	                                    ,QuotePrice
                                        ,QuoteCost
                                        ,QuoteProfit
                                        ,JobPriceUSD
                                        ,JobCostUSD
                                        ,JobProfit
                                    ) as ProfitPercent 
                                FROM qData a
                                ORDER BY Row

                                DELETE FROM tblReportCriteria WHERE CriteriaRptName = '{4}' and CriteriaEmployeeKey = {1};";

                string reportName = "rptPronacaReport";

                if(NoProfit == 1) 
                    reportName += " NoProfit";

                filterWhere = filterWhere ?? "1=1";
                string labelCreatedDate = string.Format("Between {0} and {1}", startDate, endDate);
                sql = String.Format(sql, filterWhere, employeeKey, labelCriteria, labelCreatedDate, reportName);

                SqlCommand cmd = new SqlCommand(sql, oConn);
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                }
                
            }
            return dt;
        }

        private DataTable GetPronacaReportClosedSubQuotes(int FileKey = 0)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT *
                             FROM qrptPronacaReportClosedSubQuotes
                             WHERE QHdrFileKey = @FileKey";

                sql = string.Format(sql);

                SqlDataAdapter adp = new SqlDataAdapter(sql, oConn);
                adp.SelectCommand.Parameters.Add("@FileKey", SqlDbType.Int).Value = FileKey;

                try
                {
                    adp.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                }
            }

            return dt;
        }
    }
}