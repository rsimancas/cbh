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

    public class rptFileSummaryController : Controller
    {
        static readonly IUserRepository userRepository = new UserRepository();
        static readonly IEmployeeRepository employeeRepo = new EmployeeRepository();
        static User currentUser = null;

        ////
        //// POST: /Reports/rptFileSummary/
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

            DataTable dtHeader = GetFileSummary(strWhere, labelCriteria, employeeKey.Value);

            LocalReport lr = new LocalReport();

            lr.ReportPath = "Areas/Reports/ReportDesign/rptFileSummary.rdlc";

            lr.DataSources.Clear();
            lr.DataSources.Add(new ReportDataSource("dsRptFileSummary", dtHeader));

            lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_SubreportProcessing);
            lr.SubreportProcessing += lr_SubreportProcessing;

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //ReportPageSettings rps = lr.GetDefaultPageSettings();

            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>rptFileSummary</OutputFormat>" +
            "  <PageWidth>11in</PageWidth>" +
            "  <PageHeight>8.5in</PageHeight>" +
            "  <MarginTop>0.1667in</MarginTop>" +
            "  <MarginLeft>0.2in</MarginLeft>" +
            "  <MarginRight>0.2in</MarginRight>" +
            "  <MarginBottom>0.1667in</MarginBottom>" +
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
            int.TryParse(e.Parameters["FileKey"].Values[0].ToString(), out id);
            if (e.ReportPath == "rptFileSummarySubQuotes")
            {
                var dt = GetFileSummarySubQuotes(id);
                e.DataSources.Add(new ReportDataSource("dsRptFileSummarySubQuotes", dt));
            }
        }

        private DataTable GetFileSummary(string filterWhere, string labelCriteria, int employeeKey)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"WITH qData
                                AS
                                (
                                    SELECT a.*
                                    ,'{0}' as LabelCriteria
                                    ,fh.FileYear 
                                   FROM qrptFileSummary a
                                        left join tblFileHeader fh on a.FileKey = fh.FileKey
                                   WHERE {1} 
                                )
                                SELECT a.*
                                    ,ISNULL(SQ.Total,0) as TotalSQ
                                    ,ROW_NUMBER() OVER (ORDER BY a.IsClosed DESC, a.FileYear DESC, a.FileNum Desc) as Row
                                FROM qData a 
                                    OUTER APPLY (SELECT COUNT(*) as Total FROM dbo.qrptFileSummarySubQuotes b WHERE b.QHdrFileKey = a.FileKey) as SQ
                                ORDER BY Row;

                               DELETE FROM tblReportCriteria WHERE CriteriaRptName = 'rptFileSummary' and CriteriaEmployeeKey = {2}";

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

        private DataTable GetFileSummarySubQuotes(int FileKey = 0)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT *
                             FROM qrptFileSummarySubQuotes
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
