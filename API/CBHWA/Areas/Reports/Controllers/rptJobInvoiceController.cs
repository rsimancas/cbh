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

    public class rptJobInvoiceController : Controller
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
            string wSch = nvc["wSch"];
            string strEmployeeKey = nvc["employeeKey"] ?? "";
            int employeeKey = 0;
            int.TryParse(strEmployeeKey, out employeeKey);

            var model = new rptJobInvoice();
            model.id = id.Value;
            model.wSch = wSch ?? "0";
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

        public ActionResult PDF(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction("Message", "Common", new { message = "Missing Parameters", type = "warning" });
            }

            var nvc = Request.QueryString;

            string wSch = nvc["wSch"];
            wSch = wSch ?? "0";
            string strEmployeeKey = nvc["employeeKey"] ?? "";
            int employeeKey = 0;
            int.TryParse(strEmployeeKey, out employeeKey);

            DataTable dtHeader = GetData(id.Value, employeeKey);

            LocalReport lr = new LocalReport();

            lr.ReportPath = "Areas/Reports/ReportDesign/rptJobInvoice.rdlc";

            ReportParameter param1 = new ReportParameter("wSch", wSch);
            ReportParameter param2 = new ReportParameter("EmployeeKey", employeeKey.ToString());
            lr.SetParameters(new ReportParameter[] { param1, param2 });
            lr.DataSources.Add(new ReportDataSource("dsJobInvoice", dtHeader));

            lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_SubreportProcessing);

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
            int.TryParse(e.Parameters["id"].Values[0].ToString(), out id);
            string lang = e.Parameters["CustLanguageCode"].Values[0].ToString();

            if (e.ReportPath == "rptJobInvoiceItemSummary")
            {
                var dt = GetItemSummary(id, lang);
                e.DataSources.Add(new ReportDataSource("dsJobInvoiceItemSummary", dt));
            }

            if (e.ReportPath == "rptJobInvoiceItemDetail")
            {
                var dt = GetItemDetail(id, lang);
                e.DataSources.Add(new ReportDataSource("dsJobInvoiceItemDetail", dt));
            }

            if (e.ReportPath == "rptJobInvoiceChargeDetail")
            {
                var dt = GetChargeDetail(id, lang);
                e.DataSources.Add(new ReportDataSource("dsJobInvoiceChargeDetail", dt));
            }
        }

        private DataTable GetData(int id, int EmployeeKey)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                var currentUser = employeeRepo.Get(EmployeeKey);
                string sql = @"SELECT a.*
                           ,dbo.fnGetLocationFooter(a.LocationKey) as LocationFooter
                           ,dbo.fnGetReportText(21, a.CustLanguageCode) as InvoiceNumLabel
                           ,dbo.fnGetPaymentTerms(a.InvoicePaymentTerms, a.CustLanguageCode) as PaymentTerms
                           ,dbo.fnGetReportText(2, a.CustLanguageCode) as PaymentTermsLabel
                           ,(CASE WHEN ISNULL(Summary.SummaryCount,0) = 0 THEN ISNULL(Price.TotalPrice,0) ELSE ISNULL(Summary.TotalSummary,0) END) as TotalPrice
                           ,ISNULL(Summary.TotalSummary,0) as TotalSummary
                           ,ISNULL(Summary.SummaryCount,0) as SummaryCount
                           ,ISNULL(Charges.TotalCharges,0) as TotalCharges
                           ,ISNULL(Payment.TotalPayment,0) as TotalPayment
                           ,dbo.fnGetPaymentTermDetails(a.InvoiceDate,a.InvoicePaymentTerms,ISNULL(Price.TotalPrice,0)+ISNULL(Charges.TotalCharges,0)) as PaymentTermDetails
                           ,dbo.fnGetReportText(25, a.CustLanguageCode) as ReportText25
                     FROM qrptJobInvoice a 
                       CROSS APPLY (SELECT SUM(b.ItemLinePrice) as TotalPrice FROM dbo.qrptJobInvoiceItemDetail b WHERE a.InvoiceKey = b.InvoiceKey ) as Price
                       CROSS APPLY (SELECT SUM(c.LinePrice) as TotalSummary, COUNT(*) as SummaryCount FROM dbo.qrptJobInvoiceItemSummary c WHERE a.InvoiceKey = c.ISummaryInvoiceKey ) as Summary
                       CROSS APPLY (SELECT SUM(d.ChargePrice) as TotalCharges FROM dbo.qrptJobInvoiceChargeDetailSubCharges d WHERE a.InvoiceKey = d.IChargeInvoiceKey  AND d.DescriptionLanguageCode=a.CustLanguageCode) as Charges
                       CROSS APPLY (
		                    SELECT SUM(tblAccountsReceivableJournalDetail.ARdtlAmount * tblAccountsReceivableJournal.ARJournalCurrencyRate / tblInvoiceHeader.InvoiceCurrencyRate) AS TotalPayment
			                    FROM tblAccountsReceivableJournal INNER JOIN
                                    tblAccountsReceivableJournalDetail ON 
                                    tblAccountsReceivableJournal.ARJournalKey = tblAccountsReceivableJournalDetail.ARdtlARJournalKey INNER JOIN
                                    tblInvoiceHeader ON tblAccountsReceivableJournalDetail.ARdtlInvoiceKey = tblInvoiceHeader.InvoiceKey
                                WHERE tblInvoiceHeader.InvoiceKey = @id
                       ) as Payment
                    WHERE a.InvoiceKey = @id ";
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

        private DataTable GetItemSummary(int id, string lang)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT a.*
                                   ,dbo.fnGetReportText(8, @CustLanguageCode) as TotalLabel
                                FROM qrptJobInvoiceItemSummary a
                                WHERE a.ISummaryInvoiceKey = @InvoiceKey";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@InvoiceKey", SqlDbType.Int).Value = id;
                da.SelectCommand.Parameters.Add("@CustLanguageCode", SqlDbType.NVarChar).Value = lang;

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

        private DataTable GetItemDetail(int id, string lang)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"select a.*
                                       ,dbo.fnGetReportText(24, @CustLanguageCode) as ArancelLabel
                                       ,ISNULL(a.SBLanguageSchBNum,'') + ' ' + ISNULL(a.SBLanguageSchBSubNum,'') as Arancel
                                       ,dbo.fnGetReportText(8, @CustLanguageCode) as TotalLabel
                                       ,ISNULL(dbo.fnGetReportText(19, @CustLanguageCode), '') as ShipLabel
                                       ,(CASE When b.JobShipType IS NULL THEN '' ELSE dbo.fnGetShipmentType(b.JobShipType, @CustLanguageCode) END) as ShipText
                                FROM qrptJobInvoiceItemDetail a
                                     INNER JOIN qrptJobInvoice b on a.InvoiceKey = b.InvoiceKey
                                WHERE a.InvoiceKey = @InvoiceKey";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@InvoiceKey", SqlDbType.Int).Value = id;
                da.SelectCommand.Parameters.Add("@CustLanguageCode", SqlDbType.NVarChar).Value = lang;

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

        private DataTable GetChargeDetail(int id, string lang)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"
                SET NOCOUNT ON;
                IF OBJECT_ID('tempdb..#tmpcharges') IS NOT NULL
                    DROP TABLE #tmpcharges

                DECLARE @total DECIMAL(18,2)

                SELECT @total = (CASE WHEN ISNULL(Summary.SummaryCount,0) = 0 THEN ISNULL(Price.TotalPrice,0) ELSE ISNULL(Summary.TotalSummary,0) END)
                FROM qrptJobInvoice a 
                    OUTER APPLY (SELECT SUM(b.ItemLinePrice) as TotalPrice FROM dbo.qrptJobInvoiceItemDetail b WHERE a.InvoiceKey = b.InvoiceKey ) as Price
                    OUTER APPLY (SELECT SUM(c.LinePrice) as TotalSummary, COUNT(*) as SummaryCount FROM dbo.qrptJobInvoiceItemSummary c WHERE a.InvoiceKey = c.ISummaryInvoiceKey ) as Summary
                WHERE a.InvoiceKey = @InvoiceKey

                WITH qData
                AS
                (
                    SELECT      TOP 100 PERCENT dbo.tlkpInvoiceSubTotalCategories.SubTotalKey AS SubTotalKey, dbo.tlkpInvoiceSubTotalCategories.SubTotalSort AS SubTotalSort,
                                         dbo.tlkpInvoiceSubTotalCategoriesDescriptions.STDescriptionLanguageCode AS SubTotalLanguageCode, 
                                        dbo.tlkpInvoiceSubTotalCategoriesDescriptions.STDescriptionText AS SubTotalDescription,
                                            (SELECT      Location
                                              FROM           qrptJobInvoiceChargeDetailLocations
                                              WHERE       InvoiceKey = @InvoiceKey AND SubTotalKey = dbo.tlkpInvoiceSubTotalCategories.SubTotalKey) AS SubTotalLocation, ISNULL
                                            ((SELECT      InvoiceKey
                                                FROM          qrptJobInvoiceChargeDetailLocations
                                                WHERE      InvoiceKey = @InvoiceKey AND SubTotalKey = dbo.tlkpInvoiceSubTotalCategories.SubTotalKey), 0) AS ShowFooter, 
                                        @InvoiceKey AS InvoiceKey
                FROM          dbo.tlkpInvoiceSubTotalCategoriesDescriptions INNER JOIN
                                        dbo.tlkpInvoiceSubTotalCategories ON 
                                        dbo.tlkpInvoiceSubTotalCategoriesDescriptions.STDescriptionSubTotalKey = dbo.tlkpInvoiceSubTotalCategories.SubTotalKey
                WHERE      (dbo.tlkpInvoiceSubTotalCategoriesDescriptions.STDescriptionLanguageCode = @LanguageCode)
                )
                SELECT a.*, b.*, ISNULL(b.ChargePrice*0, 0) AS TotalWCharges, ROW_NUMBER() OVER (ORDER BY a.SubTotalSort, a.SubTotalKey) as row
                INTO #tmpcharges FROM qData a
                LEFT OUTER JOIN 
                (
                     SELECT      TOP 100 PERCENT dbo.tblInvoiceCharges.IChargeSort, dbo.tblInvoiceCharges.IChargeInvoiceKey, dbo.tblInvoiceCharges.IChargeMemo, 
                                        dbo.tblInvoiceCharges.IChargeQty, 
                                        dbo.tblInvoiceCharges.IChargeCost * dbo.tblInvoiceCharges.IChargeCurrencyRate / dbo.tblInvoiceHeader.InvoiceCurrencyRate AS ChargeCost, 
                                        dbo.tblInvoiceCharges.IChargePrice * dbo.tblInvoiceCharges.IChargeCurrencyRate / dbo.tblInvoiceHeader.InvoiceCurrencyRate AS ChargePrice, 
                                        dbo.tlkpChargeCategoryDescriptions.DescriptionLanguageCode, dbo.tlkpChargeCategoryDescriptions.DescriptionText, 
                                        dbo.tlkpChargeCategories.ChargeSubTotalCategory
                FROM          dbo.tblInvoiceCharges INNER JOIN
                                        dbo.tblInvoiceHeader ON dbo.tblInvoiceCharges.IChargeInvoiceKey = dbo.tblInvoiceHeader.InvoiceKey LEFT OUTER JOIN
                                        dbo.tlkpChargeCategories ON dbo.tblInvoiceCharges.IChargeChargeKey = dbo.tlkpChargeCategories.ChargeKey LEFT OUTER JOIN
                                        dbo.tlkpChargeCategoryDescriptions ON 
                                        dbo.tblInvoiceCharges.IChargeChargeKey = dbo.tlkpChargeCategoryDescriptions.DescriptionChargeKey
                ORDER BY dbo.tblInvoiceCharges.IChargeSort
                ) 
                as b ON b.ChargeSubTotalCategory=a.SubTotalKey AND a.InvoiceKey = b.IChargeInvoiceKey AND a.SubTotalLanguageCode = b.DescriptionLanguageCode
                ORDER BY row

                DECLARE @row INT, @price DECIMAL(18,2)

                DECLARE temp_cursor CURSOR FOR
                select row,ISNULL(ChargePrice,0) as ChargePrice
                from #tmpcharges
                OPEN temp_cursor

                FETCH NEXT FROM temp_cursor INTO @row,@price

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @total = @total + @price
                    UPDATE #tmpcharges SET TotalWCharges = @total WHERE row=@row

                FETCH NEXT FROM temp_cursor INTO @row,@price
                END

                CLOSE temp_cursor

                DEALLOCATE temp_cursor

                select * from #tmpcharges ORDER BY row

                DROP TABLE #tmpcharges";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@InvoiceKey", SqlDbType.Int).Value = id;
                da.SelectCommand.Parameters.Add("@LanguageCode", SqlDbType.NVarChar).Value = lang;

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

        private DataTable GetChargesSubDetail(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = "SELECT a.QChargeFileKey, a.QChargeHdrKey, a.QChargeSort, " +
                             "a.QChargeMemo,  " +
                             "a.QChargeCost * a.QChargeCurrencyRate / b.QHdrCurrencyRate AS QChargeCost, " +
                             " a.QChargePrice * a.QChargeCurrencyRate / b.QHdrCurrencyRate AS QChargePrice, " +
                             " d.DescriptionLanguageCode AS QCDLanguageCode,  " +
                             "d.DescriptionText AS QCDDescription,  " +
                             "c.ChargeSubTotalCategory AS SubTotalCategory " +
                             "FROM tblFileQuoteCharges a  " +
                             " INNER JOIN tblFileQuoteHeader b ON a.QChargeHdrKey = b.QHdrKey  " +
                             " INNER JOIN tlkpChargeCategories c ON a.QChargeChargeKey = c.ChargeKey " +
                             " INNER JOIN tlkpChargeCategoryDescriptions d ON a.QChargeChargeKey = d.DescriptionChargeKey " +
                             " INNER JOIN qrptJobInvoice e ON a.QChargeHdrKey=e.QHdrKey and d.DescriptionLanguageCode=e.CustLanguageCode " +
                             " WHERE a.QChargeHdrKey={0} and (a.QChargePrint = 1) " +
                             " ORDER BY a.QChargeFileKey, a.QChargeHdrKey, a.QChargeSort,a.QChargeKey";
                sql = String.Format(sql, id);
                SqlCommand cmd = new SqlCommand(sql, oConn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }

            return dt;
        }
    }
}