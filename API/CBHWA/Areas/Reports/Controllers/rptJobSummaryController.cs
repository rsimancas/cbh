namespace CBHWA.Areas.Reports.Controllers
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

    public class rptJobSummaryController : Controller
    {
        static readonly IUserRepository userRepository = new UserRepository();
        static readonly IEmployeeRepository employeeRepo = new EmployeeRepository();
        static User currentUser = null;

        ////
        //// POST: /Reports/rptJobSummary/
        public ActionResult Index()
        {
            var nvc = Request.QueryString;

            string strWhere = nvc["strWhere"] ?? "rptDate >= CAST('2000-01-01' AS DATE)";
            string labelCriteria = nvc["labelCriteria"] ?? "";
            string strEmployeeKey = nvc["employeeKey"] ?? "";
            int employeeKey = 0;
            int.TryParse(strEmployeeKey, out employeeKey);
            labelCriteria = Uri.EscapeUriString(labelCriteria);

            var model = new CriteriaParam();
            model.labelCriteria = labelCriteria;
            model.employeeKey = employeeKey;
            model.strWhere = Uri.UnescapeDataString(strWhere);

            var filters = new FieldFilters();
            filters.fields = new List<FieldFilter>();
            filters.fields.Add(new FieldFilter { name = "EmployeeStatusCode", oper = "=", type = "int", value = "8" });
            var sort = new Sort();
            int totalRecords = 0;
            var employeeList = employeeRepo.GetList(filters, null, sort, 0, 0, 0, ref totalRecords);
            ViewBag.Employees = employeeList;

            return View(model);
        }

        public ActionResult PDF(int? employeeKey, string labelCriteria, string strWhere)
        {

            //if (!CheckToken(Request.Headers))
            //{
            //    return RedirectToAction("Error");
            //}

            var nvc = Request.QueryString;
            strWhere = strWhere ?? "rptDate >= CAST('2016-07-01' AS DATE) AND rptDate <= CAST('2016-09-28' AS DATE) AND StatusKey = 100";
            labelCriteria = labelCriteria ?? "";
            employeeKey = employeeKey ?? 0;
            //string strWhere = nvc["strWhere"] ?? "rptDate >= CAST('2016-07-01' AS DATE) AND rptDate <= CAST('2016-09-28' AS DATE) AND StatusKey = 100";
            //string labelCriteria = nvc["labelCriteria"] ?? "";
            //string strEmployeeKey = nvc["employeeKey"] ?? "";
            //int employeeKey = 0;
            //int.TryParse(strEmployeeKey, out employeeKey);

            DataTable dtHeader = GetJobSummary(strWhere, labelCriteria, employeeKey.Value);
            DataTable dtInv = GetSummaryInvoices();
            DataTable dtPO = GetSummarySubPurchaseOrders();

            LocalReport lr = new LocalReport();

            lr.ReportPath = "Areas/Reports/ReportDesign/rptJobSummary.rdlc";

            lr.DataSources.Clear();
            lr.DataSources.Add(new ReportDataSource("dsRptJobSummary", dtHeader));
            lr.DataSources.Add(new ReportDataSource("dsRptJobSummaryInvoices", dtInv));
            lr.DataSources.Add(new ReportDataSource("dsRptJobSummarySubPurchaseOrders", dtPO));

            lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_SubreportProcessing);
            lr.SubreportProcessing += lr_SubreportProcessing;

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //ReportPageSettings rps = lr.GetDefaultPageSettings();

            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>rptJobSummary</OutputFormat>" +
            "  <PageWidth>11in</PageWidth>" +
            "  <PageHeight>8.5in</PageHeight>" +
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

            //string fileName = Utils.GetTempFileNameWithExt("pdf");
            //FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
            //fs.Write(bytes,0,bytes.Length);
            //fs.Close();
            

            //return Content(Path.GetFileNameWithoutExtension(fileName));
            return File(bytes, "application/pdf");
        }

        public ActionResult Error()
        {
            return View();
        }

        void lr_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            int id = 0;
            int.TryParse(e.Parameters["JobKey"].Values[0].ToString(), out id);
            if (e.ReportPath == "rptJobSummaryInvoices")
            {
                var dtInv = GetSummaryInvoices(id);
                e.DataSources.Add(new ReportDataSource("dsRptJobSummaryInvoices", dtInv));
            }
            else
            {
                var dtPO = GetSummarySubPurchaseOrders(id);
                e.DataSources.Add(new ReportDataSource("dsRptJobSummarySubPurchaseOrders", dtPO));
            }
        }

        private DataTable GetJobSummary(string filterWhere, string labelCriteria, int employeeKey)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"WITH qData
                                AS
                                (
                                    SELECT *,
                                    ISNULL(JobDateCustRequired,ISNULL(JobDateCustRequired, '')) as CustRequiredDate,
                                    '{0}' as LabelCriteria
                                   FROM qrptJobSummary
                                   WHERE {1} 
                                )
                                SELECT a.*,
                                    ISNULL(t1.Total,0) as TotalPO,
                                    ISNULL(t2.Total,0) as TotalInv,
                                    ROW_NUMBER() OVER (ORDER BY StatusKey ASC,JobNum ASC) as Row
                                FROM qData a 
                                    LEFT OUTER JOIN (SELECT POJobKey,COUNT(*) as Total FROM dbo.qrptJobSummaryPurchaseOrders WHERE POJobKey IN (SELECT JobKey FROM qData) GROUP BY POJobKey) as t1 ON a.JobKey = t1.POJobKey
                                    LEFT OUTER JOIN (SELECT JobKey,COUNT(*) as Total FROM dbo.qrptJobSummaryInvoices WHERE JobKey IN (SELECT JobKey FROM qData) GROUP BY JobKey) as t2 ON a.JobKey=t2.JobKey  
                                ORDER BY StatusKey,JobNum;

                               DELETE FROM tblReportCriteria WHERE CriteriaRptName = 'rptJobSummary' and CriteriaEmployeeKey = {2}";

                filterWhere = filterWhere ?? "1=1";
                sql = String.Format(sql, labelCriteria, filterWhere, employeeKey);
                SqlCommand cmd = new SqlCommand(sql, oConn);
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

        private DataTable GetSummaryInvoices(int JobKey = 0)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT *,dbo.fnGetPaymentTerms(InvoicePaymentTerms,'en') as PaymentTerms
                             FROM qrptJobSummaryInvoices 
                             WHERE JobKey = @JobKey";

                sql = string.Format(sql);

                SqlDataAdapter adp = new SqlDataAdapter(sql, oConn);
                adp.SelectCommand.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;

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

        private DataTable GetSummarySubPurchaseOrders(int JobKey = 0)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT a.*, b.IsClosed as ParentIsClosed
                             FROM dbo.qrptJobSummaryPurchaseOrders a
                                INNER JOIN dbo.qrptJobSummary b ON a.POJobKey = b.JobKey
                             WHERE a.POJobKey = @JobKey";

                sql = string.Format(sql);

                SqlDataAdapter adp = new SqlDataAdapter(sql, oConn);
                adp.SelectCommand.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;

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

        private bool CheckToken(NameValueCollection headers)
        {
            string token;

            try
            {
                token = headers.GetValues("Authorization-Token").First();
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                string[] split = token.Split(',');

                string usrName = Utils.Decrypt(split[0]);
                string usrPwd = Utils.Decrypt(split[1]);

                var userLogged = userRepository.ValidLogon(usrName, usrPwd);

                if (userLogged == null)
                {
                    return false;
                };

                currentUser = userLogged;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
