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

    public class rptCustomerWebLoginsController : Controller
    {
        private IUserRepository _IUserRepo;
        private IEmployeeRepository _IEmployeeRepo;
        private IFileRepository _IFileRepo;
        static NameValueCollection queryValues = null;

        public rptCustomerWebLoginsController()
        {
            this._IUserRepo = new UserRepository();
            this._IEmployeeRepo = new EmployeeRepository();
            this._IFileRepo = new FileRepository();
        }

        public ActionResult Index(int? id)
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
            var employeeList = _IEmployeeRepo.GetList(filters, null, sort, 0, 0, 0, ref totalRecords);
            ViewBag.Employees = employeeList;

            return View(model);
        }

        public ActionResult PDF(int? employeeKey, string labelCriteria, string strWhere)
        {

            var nvc = Request.QueryString;
            strWhere = strWhere ?? "rptDate >= CAST('2016-07-01' AS DATE) AND rptDate <= CAST('2016-09-28' AS DATE) AND StatusKey = 100";
            labelCriteria = labelCriteria ?? " ";
            employeeKey = employeeKey ?? 0;

            DataTable dtHeader = GetData(strWhere, labelCriteria, employeeKey.Value);

            LocalReport lr = new LocalReport();

            lr.ReportPath = "Areas/Reports/ReportDesign/rptCustomerWebLogins.rdlc";

            ReportParameter param1 = new ReportParameter("EmployeeKey", employeeKey.ToString());
            lr.SetParameters(new ReportParameter[] { param1 });
            lr.DataSources.Add(new ReportDataSource("dsCustomerWebLogins", dtHeader));

            
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //ReportPageSettings rps = lr.GetDefaultPageSettings();

            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>rptCustomerWebLogins</OutputFormat>" +
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

            try
            {
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
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }
        }

        private DataTable GetData(string filterWhere, string labelCriteria, int employeeKey)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"WITH qData
                                AS
                                (
								   SELECT a.*
                                        ,'{0}' as LabelCriteria
                                        , dbo.fnGetLocationFooter({2}) as LocationFooter
									FROM dbo.qrptCustomerWebLogins a
									WHERE {1}
                                )
								SELECT q.*
								FROM qData q
                                ORDER BY WebLoginSuccess,CustName,rptDate

                               DELETE FROM tblReportCriteria WHERE CriteriaRptName = 'rptCustomerWebLogins' and CriteriaEmployeeKey = {2}";

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
    }
}