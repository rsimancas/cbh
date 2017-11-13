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

    public class rptQuoteOrderingFormController : Controller
    {
        static readonly IUserRepository userRepository = new UserRepository();
        static readonly IEmployeeRepository employeeRepo = new EmployeeRepository();
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

            var model = new rptQuoteOrderingForm();
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
            //DataTable dtDetail = GetDetail(id.Value);

            LocalReport lr = new LocalReport();

            lr.ReportPath = "Areas/Reports/ReportDesign/rptQuoteOrderingForm.rdlc";

            ReportParameter param1 = new ReportParameter("EmployeeKey", employeeKey.ToString());
            lr.SetParameters(new ReportParameter[] { param1 });
            lr.DataSources.Add(new ReportDataSource("dsQuoteOrderingForm", dtHeader));

            lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_SubreportProcessing);

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

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
            if (e.ReportPath == "rptQuoteOFSubShipTo")
            {
                int CustShipKey = 0;
                int WarehouseKey = 0;
                int LocationKey = 0;
                int ShipDestination = 0;

                string location = (e.Parameters["LocationKey"].Values[0] != null) ? e.Parameters["LocationKey"].Values[0].ToString() : "0";
                int.TryParse(location, out LocationKey);
                int.TryParse(e.Parameters["CustShipKey"].Values[0].ToString(), out CustShipKey);
                int.TryParse(e.Parameters["WarehouseKey"].Values[0].ToString(), out WarehouseKey);
                int.TryParse(e.Parameters["ShipDestination"].Values[0].ToString(), out ShipDestination);

                var dt = GetShipTO(CustShipKey, WarehouseKey, LocationKey, ShipDestination);
                e.DataSources.Add(new ReportDataSource("dsShipAddress", dt));
            }
            else if (e.ReportPath == "rptQuoteOFSubFreight")
            {
                int FileKey = 0;
                int QHdrKey = 0;

                try
                {
                    int.TryParse(e.Parameters["FileKey"].Values[0].ToString(), out FileKey);
                    int.TryParse(e.Parameters["QHdrKey"].Values[0].ToString(), out QHdrKey);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                }
                
                var dt = GetFreight(FileKey, QHdrKey);
                e.DataSources.Add(new ReportDataSource("dsSubFreight", dt));
            }
        }

        private DataTable GetData(int id, int EmployeeKey)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                var currentUser = employeeRepo.Get(EmployeeKey);
                string sql = @"WITH qData
                               AS
                               (
                                    SELECT  dbo.tblFileHeader.*, dbo.tblFileQuoteHeader.*, dbo.tblFileQuoteVendorInfo.*, dbo.tblVendors.VendorName, dbo.tblCustomers.CustKey, 
                                        dbo.tblCustomers.CustName, dbo.tblCustomers.CustPhone, dbo.tblCustomers.CustFax, dbo.tblCustomers.CustEmail, 
                                        dbo.tblCustomerContacts.ContactKey, dbo.tblCustomerContacts.ContactTitle, dbo.tblCustomerContacts.ContactFirstName, 
                                        dbo.tblCustomerContacts.ContactLastName, dbo.tblCustomerContacts.ContactPhone, dbo.tblCustomerContacts.ContactEmail, 
                                        dbo.tblVendorContacts.ContactKey AS VendorContactKey, dbo.tblVendorContacts.ContactTitle AS VendorContactTitle, 
                                        dbo.tblVendorContacts.ContactFirstName AS VendorContactFirstName, dbo.tblVendorContacts.ContactLastName AS VendorContactLastName, 
                                        dbo.tblVendorContacts.ContactPhone AS VendorContactPhone, dbo.tblVendorContacts.ContactEmail AS VendorContactEmail, 
                                        dbo.tblVendors.VendorPhone, dbo.tblVendors.VendorFax, dbo.tblVendors.VendorEmail, dbo.tblEmployees.EmployeeFirstName, 
                                        dbo.tblEmployees.EmployeeMiddleInitial, dbo.tblEmployees.EmployeeLastName, dbo.tblEmployees.EmployeeLocationKey, 
                                        dbo.qryFileQuoteVendorSummaryWithDiscount.Cost, dbo.qryFileQuoteVendorSummaryWithDiscount.DiscountAmount, 
                                        dbo.qryFileQuoteVendorSummaryWithDiscount.CostAfterDiscount, dbo.qryFileQuoteVendorSummaryWithDiscount.Price, 
                                        dbo.qryFileQuoteVendorSummaryWithDiscount.Margin, dbo.qryFileQuoteVendorSummaryWithDiscount.Currency AS VendorCurrencyCode, 
                                        dbo.tblFileQuoteVendorInfo.FVPOCurrencyRate AS VendorCurrencyRate, 
                                        dbo.qryFileQuoteVendorSummaryWithDiscount.CurrencyFormat AS VendorCurrencyFormat, 
                                        dbo.tblCurrencyRates.CurrencyFormat AS QuoteCurrencyFormat, dbo.tblInspectionCompanies.InspectorName, 
                                        ISNULL(dbo.qsumFileQuoteVendorSummaryCharges.Cost, 0) AS FreightCost, ISNULL(dbo.qsumFileQuoteVendorSummaryCharges.Price, 0) 
                                        AS FreightPrice, ISNULL(dbo.qsumQuoteTotals.LinePrice, 0) + ISNULL(dbo.qsumQuoteTotalCharges.Price, 0) AS QuoteTotalPrice, 
                                        dbo.tlkpPaymentTerms.TermWarningFlag
                                    FROM dbo.tblFileQuoteHeader INNER JOIN
                                        dbo.tblFileQuoteVendorInfo ON dbo.tblFileQuoteHeader.QHdrKey = dbo.tblFileQuoteVendorInfo.FVQHdrKey INNER JOIN
                                        dbo.tblFileHeader ON dbo.tblFileQuoteHeader.QHdrFileKey = dbo.tblFileHeader.FileKey INNER JOIN
                                        dbo.tblVendors ON dbo.tblFileQuoteVendorInfo.FVVendorKey = dbo.tblVendors.VendorKey INNER JOIN
                                        dbo.tblCustomers ON dbo.tblFileHeader.FileCustKey = dbo.tblCustomers.CustKey INNER JOIN
                                        dbo.tblEmployees ON dbo.tblFileHeader.FileQuoteEmployeeKey = dbo.tblEmployees.EmployeeKey INNER JOIN
                                        dbo.qryFileQuoteVendorSummaryWithDiscount ON dbo.tblFileQuoteHeader.QHdrKey = dbo.qryFileQuoteVendorSummaryWithDiscount.QHdrKey AND 
                                        dbo.tblFileQuoteVendorInfo.FVVendorKey = dbo.qryFileQuoteVendorSummaryWithDiscount.VendorKey INNER JOIN
                                        dbo.tblCurrencyRates ON dbo.tblFileQuoteHeader.QHdrCurrencyCode = dbo.tblCurrencyRates.CurrencyCode INNER JOIN
                                        dbo.tsysLocations ON dbo.tblEmployees.EmployeeLocationKey = dbo.tsysLocations.LocationKey LEFT OUTER JOIN
                                        dbo.tlkpPaymentTerms ON dbo.tblFileQuoteHeader.QHdrCustPaymentTerms = dbo.tlkpPaymentTerms.TermKey LEFT OUTER JOIN
                                        dbo.qsumFileQuoteVendorSummaryCharges ON 
                                        dbo.tblFileQuoteVendorInfo.FVVendorKey = dbo.qsumFileQuoteVendorSummaryCharges.VendorKey AND 
                                        dbo.tblFileQuoteHeader.QHdrKey = dbo.qsumFileQuoteVendorSummaryCharges.QHdrKey LEFT OUTER JOIN
                                        dbo.qsumQuoteTotalCharges ON dbo.tblFileQuoteHeader.QHdrKey = dbo.qsumQuoteTotalCharges.QHdrKey LEFT OUTER JOIN
                                        dbo.qsumQuoteTotals ON dbo.tblFileQuoteHeader.QHdrKey = dbo.qsumQuoteTotals.QHdrKey LEFT OUTER JOIN
                                        dbo.tblInspectionCompanies ON dbo.tblFileQuoteHeader.QHdrInspectorKey = dbo.tblInspectionCompanies.InspectorKey LEFT OUTER JOIN
                                        dbo.tblVendorContacts ON dbo.tblFileQuoteVendorInfo.FVVendorContactKey = dbo.tblVendorContacts.ContactKey LEFT OUTER JOIN
                                        dbo.tblCustomerContacts ON dbo.tblFileHeader.FileContactKey = dbo.tblCustomerContacts.ContactKey
                                    WHERE dbo.tblFileQuoteHeader.QHdrJobKey = @id
                                )
                                SELECT a.*,
                                    ISNULL((SELECT MAX(b.PONum) FROM tblJobPurchaseOrders b WHERE b.POJobKey = a.QHdrJobKey and b.POVendorKey = a.FVVendorKey),0) as PONumByVendor,
                                    dbo.fnGetLocationFooter(a.EmployeeLocationKey) as LocationFooter,
                                    dbo.fnGetFileNum(a.FileKey) as FileNumFormatted,
                                    dbo.fnGetJobNum(a.QHdrJobKey) as JobNumFormatted,
                                    dbo.fnGetPaymentTerms(a.FVPaymentTerms, 'en') as FVPaymentTermsText
                                FROM qData a";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
                //da.SelectCommand.Parameters.Add("@employee", SqlDbType.Int).Value = EmployeeKey;

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

        private DataTable GetShipTO(int CustShipKey, int WarehouseKey, int LocationKey, int ShipDestination)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SET @WarehouseKey = (CASE WHEN @WarehouseKey = 0 THEN NULL ELSE @WarehouseKey END);
                               SELECT      CAST(ShipDestination AS smallint) AS ShipDestination, ShipKey, ShipName, ShipAddress1, ShipAddress2, Shipcity, ShipState, ShipZip, ShipCountry, 
                                            ShipPhone
                                FROM          dbo.fqryShiptoAddress(@CustShipKey, @WarehouseKey, @LocationKey) fqryShiptoAddress
                                WHERE      (ShipDestination = @ShipDestination)";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@CustShipKey", SqlDbType.Int).Value = CustShipKey;
                da.SelectCommand.Parameters.Add("@WarehouseKey", SqlDbType.Int).Value = WarehouseKey;
                da.SelectCommand.Parameters.Add("@LocationKey", SqlDbType.Int).Value = LocationKey;
                da.SelectCommand.Parameters.Add("@ShipDestination", SqlDbType.Int).Value = ShipDestination;

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

        private DataTable GetFreight(int FileKey, int QHdrKey)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
//                string sql = @"WITH qData
//                               AS
//                               (
//                                    SELECT DISTINCT a.*
//                                     FROM qsumFileQuoteChargesByGLAccount a
//                                     WHERE (a.FileKey = @FileKey AND a.QHdrKey = @QHdrKey)
//                               )
//                               select a.*,
//                                   ISNULL(c.DescriptionText, 'Error finding GL Account Description') as GLAAccountDescription
//                                   FROM qData a 
//                                       LEFT OUTER JOIN tlkpChargeCategories b ON a.QCDGLAccount = b.ChargeGLAccount
//                                       LEFT OUTER JOIN tlkpChargeCategoryDescriptions c ON b.ChargeKey = c.DescriptionChargeKey AND c.DescriptionLanguageCode = 'en'
//                               ";
                string sql = @"SELECT distinct a.*,
                                       (SELECT top 1 C.DescriptionText 
                                        from tlkpChargeCategories b
                                            LEFT OUTER JOIN tlkpChargeCategoryDescriptions c ON b.ChargeKey = c.DescriptionChargeKey AND c.DescriptionLanguageCode = 'en'
                                        where a.QCDGLAccount = b.ChargeGLAccount) as GLAAccountDescription
                                FROM qsumFileQuoteChargesByGLAccount a       
                                WHERE (a.FileKey = @FileKey AND a.QHdrKey = @QHdrKey)";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@FileKey", SqlDbType.Int).Value = FileKey;
                da.SelectCommand.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = QHdrKey;

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
