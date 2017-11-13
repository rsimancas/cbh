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

    public class rptJobProfitController : Controller
    {
        private IUserRepository _IUserRepo;
        private IEmployeeRepository _IEmployeeRepo;
        private IFileRepository _IFileRepo;
        static NameValueCollection queryValues = null;

        public rptJobProfitController()
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

            lr.ReportPath = "Areas/Reports/ReportDesign/rptJobProfit.rdlc";

            ReportParameter param1 = new ReportParameter("EmployeeKey", employeeKey.ToString());
            lr.SetParameters(new ReportParameter[] { param1 });
            lr.DataSources.Add(new ReportDataSource("dsJobProfit", dtHeader));

            lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_SubreportProcessing);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //ReportPageSettings rps = lr.GetDefaultPageSettings();

            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>rptJobProfit</OutputFormat>" +
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

        void lr_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            int id = 0;
            int.TryParse(e.Parameters["JobKey"].Values[0].ToString(), out id);
            //string lang = e.Parameters["CustLanguageCode"].Values[0].ToString();

            if (e.ReportPath == "rptJobProfitSubInvoices")
            {
                var dt = GetJobProfitSubInvoices(id);
                e.DataSources.Add(new ReportDataSource("dsJobProfitSubInvoices", dt));
            }


            if (e.ReportPath == "rptJobProfitSubPurchaseOrders")
            {
                var dt = GetJobProfitSubPurchaseOrders(id);
                e.DataSources.Add(new ReportDataSource("dsJobProfitSubPurchaseOrders", dt));
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
								   SELECT JP.*
										, ISNULL(TR.Total,0)  as TotalRevenue
										, ISNULL(TR.Items,0) as TotalItems
										, ISNULL(TR.Charges,0) as TotalCharges
										, ISNULL(TE.Total,0) as TotalExpenses
										, '{0}' as LabelCriteria
										, TR.CountIN
										, TE.CountPO
									FROM dbo.qrptJobProfit JP
											outer apply (SELECT SUM(JSI.Total) as Total, SUM(JSI.Items) as Items, SUM(JSI.Charges) as Charges, COUNT(*) as CountIN 
                                                         FROM dbo.qrptJobProfitSubInvoices JSI 
                                                         WHERE JP.JobKey = JSI.JobKey) AS TR
											outer apply (SELECT SUM(JSPO.Total) as Total, COUNT(*) as CountPO FROM dbo.qrptJobProfitSubPurchaseOrders JSPO  WHERE JP.JobKey = JSPO.JobKey) AS TE
									WHERE {1}
                                )
								SELECT Replace(Replace(q.CurrencyFormat, '[RED]""CR €""', '-€'), '[RED]""CR $""', '-$') as CurrencyFormat
									,q.JobKey, q.JobNum, q.rptDate, q.JobShipDate, q.CountryKey, q.ShipCountryName, q.CustKey, q.CustName, q.ContactKey, q.ContactTitle, q.ContactFirstName
									, q.ContactLastName, q.QuoteNum  
									, q.CurrencyCode, q.EmployeeKey, q.IsClosed, q.JobExemptFromProfitReport, q.StatusKey, q.StatusDescription, q.JobCustCurrencyRate
									, q.TotalRevenue, q.TotalItems, q.TotalCharges, q.TotalExpenses, q.LabelCriteria, q.CountIN, q.CountPO
                                    , t.GrandTotalExpenses, t.GrandTotalRevenue
								    , ROUND(((GrandTotalRevenue + GrandTotalExpenses)*10000)/GrandTotalRevenue, 0)/10000 as GrandCurrencyAverage
									, (GrandTotalRevenue + GrandTotalExpenses2) as GrandTotal
                                    , (CASE WHEN q.TotalRevenue = 0 Or q.TotalItems = 0 THEN -1 
                                       ELSE 
                                        (CASE WHEN q.TotalExpenses = 0 THEN ROUND(q.TotalItems+q.TotalCharges/q.TotalItems*10000, 0)/10000
                                         ELSE
                                            ROUND((q.TotalRevenue+q.TotalExpenses)/q.TotalRevenue*10000,0)/10000
                                         END) 
                                       END) as Percentage
								FROM qData q
									inner join
									(
										SELECT CurrencyCode
										, SUM((CASE WHEN ((CountPO = 0 Or TotalExpenses = 0) AND (CountIN > 0 AND TotalCharges < 0)) THEN TotalItems ELSE TotalRevenue END)) as GrandTotalRevenue
										, SUM((CASE WHEN ((CountPO = 0 Or TotalExpenses = 0) AND (CountIN > 0 AND TotalCharges < 0)) THEN TotalCharges ELSE (CASE WHEN CountIN > 0 THEN TotalExpenses ELSE 0 END)  END)) as GrandTotalExpenses
                                        , SUM((CASE WHEN ((CountPO = 0 Or TotalExpenses = 0) AND (CountIN > 0 AND TotalCharges < 0)) THEN TotalCharges ELSE TotalExpenses END)) as GrandTotalExpenses2
                                        --, SUM((CASE WHEN (CountPO = 0 AND (CountIN > 0 AND TotalCharges < 0)) THEN TotalCharges ELSE TotalExpenses END)) as GrandTotalExpenses
										FROM qData GROUP BY CurrencyCode
									) as t ON q.CurrencyCode = t.CurrencyCode
                                ORDER BY q.CurrencyCode, q.JobShipDate, q.JobKey
                                --ORDER BY q.CurrencyCode,rptDate,JobNum

                               DELETE FROM tblReportCriteria WHERE CriteriaRptName = 'rptJobProfit' and CriteriaEmployeeKey = {2}";

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

                //ConnManager.CloseConn(oConn);

            }

            return dt;
        }

        private DataTable GetJobProfitSubInvoices(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT a.*
                                FROM qrptJobProfitSubInvoices a
                                WHERE a.JobKey = @JobKey";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@JobKey", SqlDbType.Int).Value = id;

                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                }
                //ConnManager.CloseConn(oConn);
            }

            return dt;
        }

        private DataTable GetJobProfitSubPurchaseOrders(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT a.*
                                FROM qrptJobProfitSubPurchaseOrders a
                                WHERE a.JobKey = @JobKey";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@JobKey", SqlDbType.Int).Value = id;

                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                }
                //ConnManager.CloseConn(oConn);
            }

            return dt;
        }
    }
}