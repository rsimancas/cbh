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

    public class rptQuoteCustomerController : Controller
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

            var model = new rptQuoteCustomer();
            model.id = id.Value;
            model.wSch = wSch ?? "0";
            model.EmployeeKey = employeeKey;

            int CustKey = 0;

            // *** Find the quote total to compare to the customer credit limit
            decimal amountUSD = 0;
            string msg = "";
            if (!CheckTotals(model, ref CustKey, ref amountUSD, ref msg)) return RedirectToAction("Message", "Common", new { message = msg, type = "warning" });

            //*** Compare the quote total to the cust limit and warn user if over
            if (fileRepo.IsOverCustCreditLimit(CustKey, amountUSD))
            {
                decimal? TermPercentPrepaid = FindPaymentTerms(id.Value);

                if (!TermPercentPrepaid.HasValue || TermPercentPrepaid.Value == 0)
                {
                    //msg = "Can't find the payment terms!";
                    //return RedirectToAction("Message", "Common", new { message = msg, type = "warning" });
                    TermPercentPrepaid = 0;
                }

                if (TermPercentPrepaid.Value < 1)
                {
                    model.TermPercentPrepaid = TermPercentPrepaid.Value;
                    model.askForPaymentTerms = true;
                }
            }

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

            //dynamic objPB = Activator.CreateInstance(Type.GetTypeFromProgID("XStandard.MD5"));
            //string key = "que clave tan hija de puta ?";
            //LogManager.Write(Utils.GetMd5Hash(key));
            //Utils.ReadDBFUsingOdbc();
            //return RedirectToAction("GetPDF");
            if (!id.HasValue)
            {
                return RedirectToAction("Message", "Common", new { message = "Missing Parameters", type = "warning" });
            }

            var nvc = Request.QueryString;

            string wSch = nvc["wSch"];
            string strEmployeeKey = nvc["employeeKey"] ?? "";
            int employeeKey = 0;
            int.TryParse(strEmployeeKey, out employeeKey);

            DataTable dtHeader = GetData(id.Value, employeeKey);
            //DataTable dtDetails = GetDetail(id.Value, wSch);

            LocalReport lr = new LocalReport();

            //lr.DataSources.Add(new ReportDataSource("dsQuoteCustomerItemDetail", dtDetails));

            lr.ReportPath = "Areas/Reports/ReportDesign/QuoteCustomer.rdlc";

            ReportParameter param1 = new ReportParameter("wSch", wSch);
            ReportParameter param2 = new ReportParameter("EmployeeKey", employeeKey.ToString());
            lr.SetParameters(new ReportParameter[] { param1, param2 });
            lr.DataSources.Add(new ReportDataSource("dsQuoteCustomer", dtHeader));

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
            int id = 0;
            int.TryParse(e.Parameters["id"].Values[0].ToString(), out id);
            string lang = e.Parameters["LanguageCode"].Values[0].ToString();

            if (e.ReportPath == "QuoteCustomerChargeDetails")
            {
                var dt = GetChargesDetail(id, lang);
                e.DataSources.Add(new ReportDataSource("dsQuoteCustomerChargeDetail", dt));
            }
            else if (e.ReportPath == "QuoteCustomerItemDetail")
            {
                string wSch = e.Parameters["wSch"].Values[0].ToString();
                var dt = GetDetail(id, wSch);
                e.DataSources.Add(new ReportDataSource("dsQuoteCustomerItemDetail", dt));
            }
            else if (e.ReportPath == "QuoteCustomerItemSummary")
            {
                string wSch = e.Parameters["wSch"].Values[0].ToString();
                var dt = GetDetailSummary(id, wSch);
                e.DataSources.Add(new ReportDataSource("dsQuoteCustomerItemSummary", dt));
            }
        }

        private DataTable GetData(int id, int EmployeeKey)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                var currentUser = employeeRepo.Get(EmployeeKey);
                string sql = @"SELECT *,
                                     dbo.fnGetQuoteNum(QHdrKey) AS QHdrPrefixNum,  --RTRIM(QHdrPrefix)+RIGHT('000' + CONVERT(nvarchar, QHdrNum), 4) AS QHdrPrefixNum, 
                                     'F' + RIGHT(CAST(FileYear AS nvarchar), 2) + N'-' + RIGHT('0000' + CONVERT(nvarchar, FileNum), 4) AS FormattedFileNum, 
                                     dbo.fnGetReportText(2, CustLanguageCode) AS PaymentText,dbo.fnGetPaymentTerms(QHdrCustPaymentTerms, CustLanguageCode) AS PaymentTerms, 
                                     dbo.fnGetReportText(12, CustLanguageCode) + ' ' + CONVERT(VARCHAR(10), QHdrGoodThruDate, 103) AS ValidThru, 
                                     dbo.fnGetReportText(25, CustLanguageCode) AS ReportText25,  
                                     (CASE WHEN QHdrLeadTime IS NULL  
		                                THEN '' 
		                                ELSE dbo.fnGetReportText(3, CustLanguageCode) + ' - ' + QHdrLeadTime
		                                END) AS LeadTime, 
	                                 dbo.fnGetReportText(23, CustLanguageCode) AS ReportText23, 
	                                 dbo.fnGetLocationFooter(@EmployeeKey) as LocationFooter, 
                                    (CASE WHEN s.Items = 0 OR s.Items IS NULL
		                                THEN ROUND((ISNULL(TotalCharges,0)+ISNULL(l.TotalLinePrice,0)),2)
		                                ELSE ROUND((ISNULL(TotalCharges,0)+ISNULL(s.TotalLinePrice,0)),2)
                                     END) as TotalQuote,
                                    dbo.fnGetEmployeeTitle(EmployeeTitleCode, CustLanguageCode) as EmployeeTitle,
                                    dbo.fnGetEmployeeTitle(1, CustLanguageCode) as EmployeeMainTitle,
                                    ISNULL(d.CurrencySymbol,'$') as CurrencySymbol
                                    ,CAST((CASE WHEN s.Items = 0 OR s.Items IS NULL THEN 0 ELSE 1 END) AS BIT) as HasSummary
                                 FROM qrptQuoteCustomer a 
                                    CROSS APPLY (select SUM(QSummaryLinePrice) as TotalLinePrice, COUNT(*) as Items from qrptFileQuoteCustomerItemSummary where QSummaryQHdrKey = QHdrKey) as s
	                                CROSS APPLY (select SUM(QuoteItemLinePrice) as TotalLinePrice from qrptFileQuoteCustomerItemDetail where FVQHdrKey = QHdrKey) as l
                                    CROSS APPLY (select SUM(QChargePrice) as TotalCharges from tblFileQuoteCharges where QChargeHdrKey =QHdrKey) as c
                                    LEFT OUTER JOIN tblCurrencyRates d ON a.QHdrCurrencyCode = d.CurrencyCode 
                                WHERE QHdrKey = @id";

                SqlCommand cmd = new SqlCommand(sql, oConn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@EmployeeKey", SqlDbType.Int).Value = EmployeeKey;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                try
                {
                    adp.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                }

                ConnManager.CloseConn(oConn);

            }
            return dt;
        }

        private DataTable GetDetail(int id, string wSch)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT *, 
                             (CASE WHEN SBLanguageKey IS NULL THEN '' ELSE dbo.fnGetReportText(24, CustLanguageCode) + ' ' + SBLanguageSchBNum END) AS LineReportText,  
                             (CASE WHEN SBLanguageKey IS NULL THEN '' ELSE 
                                dbo.fnGetReportText(24, CustLanguageCode) + ' ' + SBLanguageSchBNum + ISNULL(SBLanguageSchBSubNum,'') END) AS ScheduleItem,
                             {1} as WithSch 
                             FROM qrptFileQuoteCustomerItemDetail 
                              INNER JOIN qrptQuoteCustomer ON FVQHdrKey=QHdrKey 
                             WHERE FVQHdrKey = {0}";
                sql = String.Format(sql, id, wSch);
                SqlCommand cmd = new SqlCommand(sql, oConn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                try
                {
                    adp.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                }

                ConnManager.CloseConn(oConn);
            }

            return dt;
        }

        private DataTable GetDetailSummary(int id, string wSch)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT *
                             ,dbo.fnGetReportText(24, CustLanguageCode) as SubtotalText  
                             FROM qrptFileQuoteCustomerItemSummary
                              INNER JOIN qrptQuoteCustomer ON QSummaryQHdrKey=QHdrKey 
                             WHERE QSummaryQHdrKey = @id";
                SqlCommand cmd = new SqlCommand(sql, oConn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = @id;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                try
                {
                    adp.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                }

                ConnManager.CloseConn(oConn);
            }

            return dt;
        }

        private DataTable GetChargesDetail(int id, string lang)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string randomName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                string sql = @"SET NOCOUNT ON;
                                IF OBJECT_ID('tempdb..{0}') IS NOT NULL
                                    DROP TABLE {0}

                                DECLARE @total DECIMAL(18,2)

                                SELECT @total = SUM(a.QuoteItemLinePrice) FROM qrptFileQuoteCustomerItemDetail a where a.FVQHdrKey=@QHdrKey;

                                WITH qData
                                AS
                                (
                                    SELECT b.SubTotalKey AS SubTotalKey, b.SubTotalSort AS SubTotalSort,
                                        a.STDescriptionLanguageCode AS SubTotalLanguageCode, 
                                    a.STDescriptionText AS SubTotalDescription,
                                        (SELECT      Location
                                            FROM           qrptFileQuoteCustomerChargeDetailLocations
                                            WHERE       QHdrKey = @QHdrKey AND SubTotalKey = b.SubTotalKey) AS SubTotalLocation, ISNULL
                                        ((SELECT      QHdrKey
                                            FROM          qrptFileQuoteCustomerChargeDetailLocations
                                            WHERE      QHdrKey = @QHdrKey AND SubTotalKey = b.SubTotalKey), 0) AS ShowFooter, 
                                    @QHdrKey AS QHdrKey
                                    FROM          dbo.tlkpInvoiceSubTotalCategoriesDescriptions a INNER JOIN
                                                            dbo.tlkpInvoiceSubTotalCategories b ON 
                                                            a.STDescriptionSubTotalKey = b.SubTotalKey
                                    WHERE      (a.STDescriptionLanguageCode = @LanguageCode)
                                )
                                SELECT a.*, b.*, ISNULL(b.QChargePrice*0, 0) AS TotalWCharges, ROW_NUMBER() OVER (ORDER BY a.SubTotalSort, a.SubTotalKey) as row
                                INTO {0} FROM qData a
                                LEFT OUTER JOIN 
                                (
                                     SELECT      TOP 100 PERCENT a.QChargeFileKey, a.QChargeHdrKey, a.QChargeSort, 
                                                        a.QChargeMemo, 
                                                        a.QChargeCost * a.QChargeCurrencyRate / b.QHdrCurrencyRate AS QChargeCost,
                                                         a.QChargePrice * a.QChargeCurrencyRate / b.QHdrCurrencyRate AS QChargePrice,
                                                         d.DescriptionLanguageCode AS QCDLanguageCode, 
                                                        d.DescriptionText AS QCDDescription, 
                                                        ISNULL(c.ChargeSubTotalCategory,0) AS SubTotalCategory
                                    FROM          dbo.tblFileQuoteCharges a INNER JOIN
                                                            dbo.tblFileQuoteHeader b ON a.QChargeHdrKey = b.QHdrKey INNER JOIN
                                                            dbo.tlkpChargeCategories c ON a.QChargeChargeKey = c.ChargeKey INNER JOIN
                                                            dbo.tlkpChargeCategoryDescriptions d ON 
                                                            a.QChargeChargeKey = d.DescriptionChargeKey
                                    WHERE      (a.QChargePrint = 1) AND a.QChargeHdrKey = @QHdrKey
                                    ORDER BY a.QChargeFileKey, a.QChargeHdrKey, a.QChargeSort, a.QChargeKey
                                ) 
                                as b ON b.SubTotalCategory=a.SubTotalKey AND a.QHdrKey = b.QChargeHdrKey AND a.SubTotalLanguageCode = b.QCDLanguageCode
                                ORDER BY row

                                DECLARE @row INT, @price DECIMAL(18,2)

                                DECLARE temp_cursor CURSOR FOR
                                select row,ISNULL(QChargePrice,0) as QChargePrice
                                from {0}
                                OPEN temp_cursor

                                FETCH NEXT FROM temp_cursor INTO @row,@price

                                WHILE @@FETCH_STATUS = 0
                                BEGIN
                                    SET @total = @total + @price
                                    UPDATE {0} SET TotalWCharges = @total WHERE row=@row

                                FETCH NEXT FROM temp_cursor INTO @row,@price
                                END

                                CLOSE temp_cursor

                                DEALLOCATE temp_cursor

                                select * from {0} ORDER BY row

                                DROP TABLE {0}";

                sql = String.Format(sql, randomName);

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = id;
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
                             " INNER JOIN qrptQuoteCustomer e ON a.QChargeHdrKey=e.QHdrKey and d.DescriptionLanguageCode=e.CustLanguageCode " +
                             " WHERE a.QChargeHdrKey={0} and (a.QChargePrint = 1) " +
                             " ORDER BY a.QChargeFileKey, a.QChargeHdrKey, a.QChargeSort,a.QChargeKey";
                sql = String.Format(sql, id);
                SqlCommand cmd = new SqlCommand(sql, oConn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }

            return dt;
        }

        private bool CheckTotals(rptQuoteCustomer model, ref int CustKey, ref decimal AmountUSD, ref string msg)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {

                string sql = @"SELECT dbo.qsumQuoteTotals.FileKey, dbo.qsumQuoteTotals.LinePrice, dbo.tblCurrencyRates.CurrencyRate 
                            FROM dbo.qsumQuoteTotals INNER JOIN dbo.tblCurrencyRates 
                            ON dbo.qsumQuoteTotals.Currency = dbo.tblCurrencyRates.CurrencyCode 
                            WHERE dbo.qsumQuoteTotals.QHdrKey = @QHdrKey";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = model.id;

                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                    throw;
                }

                int FileKey = 0;
                if (dt.Rows.Count == 0)
                {
                    msg = "Can't find the Quote Total!";
                    return false;
                }
                else
                {
                    if (dt.Rows[0]["LinePrice"] == System.DBNull.Value)
                    {
                        msg = "Can't find the Quote Total!";
                        return false;
                    }

                    FileKey = Convert.ToInt32(dt.Rows[0]["FileKey"]);
                    var currencyRate = (dt.Rows[0]["CurrencyRate"] != System.DBNull.Value) ? Convert.ToDecimal(dt.Rows[0]["CurrencyRate"]) : 0;
                    var linePrice = (dt.Rows[0]["LinePrice"] != System.DBNull.Value) ? Convert.ToDecimal(dt.Rows[0]["LinePrice"]) : 0;
                    AmountUSD = linePrice * currencyRate;
                    
                }

                CustKey = ConnManager.DLookupInt("FileCustKey", "tblFileHeader", string.Format("FileKey = {0}", FileKey), oConn);
                if (CustKey == 0)
                {
                    msg = "Can't find the customer record!";
                    return false;
                }
            }

            return true;
        }

        private decimal? FindPaymentTerms(int id)
        {
            decimal? percent = 0;
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {

                string sql = @"SELECT dbo.tlkpPaymentTerms.TermPercentPrepaid 
                                FROM dbo.tblFileQuoteHeader INNER JOIN dbo.tlkpPaymentTerms
                                ON dbo.tblFileQuoteHeader.QHdrCustPaymentTerms = dbo.tlkpPaymentTerms.TermKey
                                WHERE dbo.tblFileQuoteHeader.QHdrKey =  @QHdrKey";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = id;

                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                    throw;
                }

                if (dt.Rows.Count > 0)
                {
                    percent = Convert.ToDecimal(dt.Rows[0]["TermPercentPrepaid"]);
                }
                else
                {
                    percent = null;
                }
            }

            return percent;
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

                //currentUser = userLogged;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
