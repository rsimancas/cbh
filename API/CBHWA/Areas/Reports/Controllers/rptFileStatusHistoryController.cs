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
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using Utilidades;

    public class rptFileStatusHistoryController : Controller
    {
        static readonly IUserRepository userRepository = new UserRepository();
        static readonly IEmployeeRepository employeeRepo = new EmployeeRepository();
        static readonly IFileRepository fileRepo = new FileRepository();
        static NameValueCollection queryValues = null;

        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Message", "Common", new { message = "Missing Parameters", type = "warning" });
            }

            // Capture parameters
            var nvc = Request.QueryString;
            string strEmployeeKey = nvc["employeeKey"] ?? "";
            int employeeKey = 0;
            int.TryParse(strEmployeeKey, out employeeKey);

            var model = new rptFileStatusHistory();
            model.id = id.Value;
            model.EmployeeKey = employeeKey;

            var filters = new FieldFilters();
            filters.fields = new List<FieldFilter>();
            filters.fields.Add(new FieldFilter { name = "EmployeeStatusCode", oper = "=", type = "int", value = "8" });
            var sort = new Sort();
            int totalRecords = 0;
            var employeeList = employeeRepo.GetList(filters, null, sort, 0, 0, 0, ref totalRecords);
            ViewBag.Employees = employeeList;

            return View(model);
        }

        //[CompressPDF]
        public ActionResult PDF(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction("Message", "Common", new { message = "Missing Parameters", type = "warning" });
            }

            var nvc = Request.QueryString;

            string strEmployeeKey = nvc["employeeKey"] ?? "";
            int employeeKey = 0;
            int.TryParse(strEmployeeKey, out employeeKey);

            DataTable dtHeader = GetData(id.Value, employeeKey);
            DataTable dtDetail = GetDetail(id.Value);

            LocalReport lr = new LocalReport();

            lr.ReportPath = "Areas/Reports/ReportDesign/rptFileStatusHistory.rdlc";

            ReportParameter param1 = new ReportParameter("EmployeeKey", employeeKey.ToString());
            lr.SetParameters(new ReportParameter[] { param1 });
            lr.DataSources.Add(new ReportDataSource("dsFileStatusHistory", dtHeader));
            lr.DataSources.Add(new ReportDataSource("dsFileStatusHistoryDetail", dtDetail));

            //lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_SubreportProcessing);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //ReportPageSettings rps = lr.GetDefaultPageSettings();

            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
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

            return File(bytes, mimeType);
        }

        void lr_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            //int id = 0;
            //int.TryParse(e.Parameters["id"].Values[0].ToString(), out id);
            //string lang = e.Parameters["LanguageCode"].Values[0].ToString();

            //string wSch = e.Parameters["wSch"].Values[0].ToString();
            //var dt = GetDetail(id, wSch);
            //e.DataSources.Add(new ReportDataSource("dsQuoteCustomerItemDetail", dt));
        }

        private DataTable GetData(int id, int EmployeeKey)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                var currentUser = employeeRepo.Get(EmployeeKey);
                string sql = @"SELECT *,
                    	 dbo.fnGetLocationFooter(@employee) as LocationFooter,
                         dbo.fnGetFileNum(a.FileKey) as FileNum   
                     FROM qrptFileStatusHistory a 
                    WHERE FileKey = @id ";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
                da.SelectCommand.Parameters.Add("@employee", SqlDbType.Int).Value = 1;

                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                }

                ConnManager.CloseConn(oConn);

            }
            return dt;
        }

        private DataTable GetDetail(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT  *
                        FROM          (SELECT      FileStatusFileKey AS StatusFileKey, '*' AS StatusQuoteNum, FileStatusDate AS StatusDate, FileStatusStatusKey AS StatusStatusKey, 
                                                StatusText AS FileStatus, FileStatusMemo AS StatusMemo, FileStatusModifiedBy AS StatusModifiedBy, 
                                                FileStatusModifiedDate AS StatusModifiedDate,
                                                REPLACE(FileStatusMemo, CHAR(13)+CHAR(10), ' ') as StatusMemoFormatted
                        FROM           dbo.tblFileStatusHistory INNER JOIN
                                                dbo.tlkpStatus ON dbo.tblFileStatusHistory.FileStatusStatusKey = dbo.tlkpStatus.StatusKey
                        UNION
                        SELECT      QStatusFileKey AS StatusFileKey, dbo.fnGetQuoteNum(QStatusQHdrKey) AS StatusQuoteNum, QStatusDate AS StatusDate, 
                                                QStatusStatusKey AS StatusStatusKey, StatusText AS QuoteStatus, QStatusMemo AS StatusMemo, 
                                                QStatusModifiedBy AS StatusModifiedBy, QStatusModifiedDate AS StatusModifiedDate,
                                                REPLACE(QStatusMemo, CHAR(13)+CHAR(10), ' ') as StatusMemoFormatted
                        FROM          dbo.tblFileQuoteStatusHistory INNER JOIN
                                                dbo.tlkpStatus ON dbo.tblFileQuoteStatusHistory.QStatusStatusKey = dbo.tlkpStatus.StatusKey) StatusList
                             WHERE StatusFileKey = @id
                             ORDER BY StatusDate";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;

                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                }

                ConnManager.CloseConn(oConn);
            }

            return dt;
        }
    }
}
