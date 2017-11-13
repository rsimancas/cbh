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

    public class rptJobPurchaseOrderController : Controller
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

            var model = new rptJobPurchaseOrder();
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

            LocalReport lr = new LocalReport();

            lr.ReportPath = "Areas/Reports/ReportDesign/rptJobPurchaseOrder.rdlc";

            ReportParameter param1 = new ReportParameter("EmployeeKey", employeeKey.ToString());
            lr.SetParameters(new ReportParameter[] { param1 });
            lr.DataSources.Add(new ReportDataSource("dsJobPurchaseOrder", dtHeader));

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
            //int.TryParse(e.Parameters["id"].Values[0].ToString(), out id);
            //string lang = e.Parameters["LanguageCode"].Values[0].ToString();

            if (e.ReportPath == "rptJobPurchaseOrderSubShipAddress")
            {
                int CustShipKey = 0;
                int WarehouseKey = 0;
                int LocationKey = 0;
                int ShipDestination = 0;

                string location = (e.Parameters["LocationKey"].Values[0] != null) ? e.Parameters["LocationKey"].Values[0].ToString() : "0";
                int.TryParse(location, out LocationKey);
                string CustShip = e.Parameters["CustShipKey"].Values[0] != null ? e.Parameters["CustShipKey"].Values[0].ToString() : "0";
                int.TryParse(CustShip, out CustShipKey);
                string Warehouse = e.Parameters["WarehouseKey"].Values[0] != null ? e.Parameters["WarehouseKey"].Values[0].ToString() : "0";
                int.TryParse(Warehouse, out WarehouseKey);
                string ShipDes = e.Parameters["ShipDestination"].Values[0] != null ? e.Parameters["ShipDestination"].Values[0].ToString() : "0";
                int.TryParse(ShipDes, out ShipDestination);

                var dt = GetSubShipAddress(CustShipKey, WarehouseKey, LocationKey, ShipDestination);
                e.DataSources.Add(new ReportDataSource("dsShipAddress", dt));
            }
            else if (e.ReportPath == "rptJobPurchaseOrderSubItemDetail")
            {
                int POJobKey = 0;
                int POKey = 0;
                string VendorLanguageCode = string.Empty;

                int.TryParse(e.Parameters["POJobKey"].Values[0].ToString(), out POJobKey);
                int.TryParse(e.Parameters["POKey"].Values[0].ToString(), out POKey);
                VendorLanguageCode = e.Parameters["VendorLanguageCode"].Values[0].ToString();

                var dt = GetSubItemDetail(POJobKey, POKey, VendorLanguageCode);
                e.DataSources.Add(new ReportDataSource("dsJobPurchaseOrderSubItemDetail", dt));
            }
            else if (e.ReportPath == "rptJobPurchaseOrderSubChargeDetail")
            {
                int POChargesJobKey = 0;
                int POChargesPOKey = 0;
                string VendorLanguageCode = string.Empty;

                int.TryParse(e.Parameters["POChargesJobKey"].Values[0].ToString(), out POChargesJobKey);
                int.TryParse(e.Parameters["POChargesPOKey"].Values[0].ToString(), out POChargesPOKey);
                VendorLanguageCode = e.Parameters["VendorLanguageCode"].Values[0].ToString();

                var dt = GetSubChargeDetail(POChargesJobKey, POChargesPOKey, VendorLanguageCode);
                e.DataSources.Add(new ReportDataSource("dsJobPurchaseOrderSubChargeDetail", dt));
            }
            else if (e.ReportPath == "rptJobPurchaseOrderSubInstructionsStandard")
            {
                int POKey = 0;
                string VendorLanguageCode = string.Empty;

                int.TryParse(e.Parameters["POKey"].Values[0].ToString(), out POKey);
                VendorLanguageCode = e.Parameters["VendorLanguageCode"].Values[0].ToString();

                var dt = GetSubInstructionsStandard(POKey, VendorLanguageCode);
                e.DataSources.Add(new ReportDataSource("dsJobPurchaseOrderSubInstructionsStandard", dt));
            }
            else if (e.ReportPath == "rptJobPurchaseOrderSubInstructionsOptional")
            {
                int POKey = 0;
                string VendorLanguageCode = string.Empty;

                int.TryParse(e.Parameters["POKey"].Values[0].ToString(), out POKey);
                VendorLanguageCode = e.Parameters["VendorLanguageCode"].Values[0].ToString();

                var dt = GetSubInstructionsOptional(POKey, VendorLanguageCode);
                e.DataSources.Add(new ReportDataSource("dsJobPurchaseOrderSubInstructionsOptional", dt));
            }
            else if (e.ReportPath == "rptJobPurchaseOrderSubInstructionsCarrier")
            {
                int POKey = 0;
                string VendorLanguageCode = string.Empty;

                int.TryParse(e.Parameters["POKey"].Values[0].ToString(), out POKey);
                VendorLanguageCode = e.Parameters["VendorLanguageCode"].Values[0].ToString();

                var dt = GetSubInstructionsCarrier(POKey, VendorLanguageCode);
                e.DataSources.Add(new ReportDataSource("dsJobPurchaseOrderSubInstructionsCarrier", dt));
            }
        }

        private DataTable GetData(int id, int EmployeeKey)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                var currentUser = employeeRepo.Get(EmployeeKey);
                string sql = @"SELECT a.*
                                    ,dbo.fnGetVendorAddress(a.VendorKey) as VendorAddress
                                    ,dbo.fnGetVendorOriginAddress(a.POVendorOriginAddress) as VendorOriginAddress
                                    ,dbo.fnGetLocationFooter(ISNULL(a.LocationKey,0)) as LocationFooter
                                    ,dbo.fnGetEmployeeTitle(a.EmployeeTitleCode, a.VendorLanguageCode) as EmployeeTitle
                                    ,dbo.fnGetReportText(20, a.VendorLanguageCode) as POVendorReferenceLabel
                                    ,dbo.fnGetReportText(19, a.VendorLanguageCode) as POShipmentTypeLabel
                                    ,dbo.fnGetShipmentType(a.POShipmentType, a.VendorLanguageCode) as ShipmentType
                                    ,ISNULL(charges.POChargesCost,0) as POChargesCost
                                    ,ISNULL(items.POItemsCost,0) as POItemsCost
                                    ,ISNULL(items.POItemsCount,0) as POItemsCount 
                                    ,dbo.fnGetReportText(26, a.VendorLanguageCode) as PODescriptionFooter
                                    ,dbo.fnGetReportText(18, a.VendorLanguageCode) as PODescriptionFooterBlue
                                    ,dbo.fnGetReportText(2, a.VendorLanguageCode) as PaymentTermsLabel
                                    ,dbo.fnGetPaymentTerms(a.POVendorPaymentTerms, a.VendorLanguageCode) as PaymentTerms
                                 FROM qrptJobPurchaseOrder a 
                                 OUTER APPLY (select SUM(b.ChargeLineCost) as POChargesCost from dbo.qrptJobPurchaseOrderSubChargeDetail b WHERE b.POChargesPOKey = a.POKey) as Charges
                                 OUTER APPLY (select SUM(c.ItemLineCost) as POItemsCost,Count(*) as POItemsCount from dbo.qrptJobPurchaseOrderSubItemDetail c WHERE c.POKey = a.POKey) as Items
                                WHERE a.POKey = @id ";

                SqlDataAdapter adp = new SqlDataAdapter(sql, oConn);
                adp.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;

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

        private DataTable GetSubShipAddress(int CustShipKey, int WarehouseKey, int LocationKey, int ShipDestination)
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

        private DataTable GetSubItemDetail(int POJobKey, int POKey, string VendorLanguagecode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT a.*
                                      ,dbo.fnGetReportText(8, @VendorLanguageCode) as TotalLabel
                               FROM dbo.qrptJobPurchaseOrderSubItemDetail a
                               WHERE a.POJobKey = @POJobKey AND a.POKey = @POKey";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@POJobKey", SqlDbType.Int).Value = POJobKey;
                da.SelectCommand.Parameters.Add("@POKey", SqlDbType.Int).Value = POKey;
                da.SelectCommand.Parameters.Add("@VendorLanguageCode", SqlDbType.Char).Value = VendorLanguagecode;

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

        private DataTable GetSubChargeDetail(int POChargesJobKey, int POChargesPOKey, string VendorLanguagecode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"SELECT * 
                               FROM dbo.qrptJobPurchaseOrderSubChargeDetail a
                               WHERE a.POChargesJobKey = @POChargesJobKey AND a.POChargesPOKey = @POChargesPOKey";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@POChargesJobKey", SqlDbType.Int).Value = POChargesJobKey;
                da.SelectCommand.Parameters.Add("@POChargesPOKey", SqlDbType.Int).Value = POChargesPOKey;
                da.SelectCommand.Parameters.Add("@VendorLanguageCode", SqlDbType.Char).Value = VendorLanguagecode;

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

        private DataTable GetSubInstructionsStandard(int POKey, string VendorLanguagecode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"DECLARE @TableName nvarchar(50) = 'qrptJobPurchaseOrder'
	                            ,@script nvarchar(MAX) = null
	                            ,@columns nvarchar(MAX) = null
	
                            DECLARE @Table TableTuple --TABLE(ColName NVARCHAR(MAX), ColValue NVARCHAR(MAX))
                            -- keep the table's column list
                            select @columns = COALESCE(@columns + ',','') + '(''' + c.name + ''',ISNULL(CAST(' + c.name + ' AS VARCHAR(MAX)),''''))' from sys.views  t
                            join sys.columns c on t.object_id = c.object_id
                            where t.name = @TableName

                            set @script = 'SELECT ca.ColName, ca.ColValue FROM dbo.'+@TableName+' CROSS APPLY ( Values '
                            set @script = @script+@columns+' ) as CA (ColName, ColValue) WHERE POKey = ' + STR(@POKey)
                            INSERT @Table EXEC sp_executeSql @script

                            SELECT InstructionKey
                                ,dbo.fnGetPOInstructionText(InstructionKey,@VendorLanguageCode, @POKey, @Table)  as InstructionText
                            FROM tlkpJobPurchaseOrderInstructions WHERE (InstructionCategory = 1)
                            ORDER BY InstructionSort";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@POKey", SqlDbType.Int).Value = POKey;
                da.SelectCommand.Parameters.Add("@VendorLanguageCode", SqlDbType.Char).Value = VendorLanguagecode;

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

        private DataTable GetSubInstructionsCarrier(int POKey, string VendorLanguagecode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"DECLARE @TableName nvarchar(50) = 'qrptJobPurchaseOrder'
	                            ,@script nvarchar(MAX) = null
	                            ,@columns nvarchar(MAX) = null
	
                            DECLARE @Table TableTuple --TABLE(ColName NVARCHAR(MAX), ColValue NVARCHAR(MAX))
                            -- keep the table's column list
                            select @columns = COALESCE(@columns + ',','') + '(''' + c.name + ''',ISNULL(CAST(' + c.name + ' AS VARCHAR(MAX)),''''))' from sys.views  t
                            join sys.columns c on t.object_id = c.object_id
                            where t.name = @TableName

                            set @script = 'SELECT ca.ColName, ca.ColValue FROM dbo.'+@TableName+' CROSS APPLY ( Values '
                            set @script = @script+@columns+' ) as CA (ColName, ColValue) WHERE POKey = ' + STR(@POKey)
                            INSERT @Table EXEC sp_executeSql @script

                            SELECT InstructionKey
                                ,dbo.fnGetPOInstructionText(InstructionKey,@VendorLanguageCode, @POKey, @Table)  as InstructionText
                            FROM tlkpJobPurchaseOrderInstructions WHERE (InstructionCategory = 2)
                            ORDER BY InstructionSort";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@POKey", SqlDbType.Int).Value = POKey;
                da.SelectCommand.Parameters.Add("@VendorLanguageCode", SqlDbType.Char).Value = VendorLanguagecode;

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

        private DataTable GetSubInstructionsOptional(int POKey, string VendorLanguagecode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                string sql = @"DECLARE @TableName nvarchar(50) = 'qrptJobPurchaseOrder'
	                            ,@script nvarchar(MAX) = null
	                            ,@columns nvarchar(MAX) = null
	
                               DECLARE @Table TableTuple --TABLE(ColName NVARCHAR(MAX), ColValue NVARCHAR(MAX))
                               -- keep the table's column list
                               select @columns = COALESCE(@columns + ',','') + '(''' + c.name + ''',ISNULL(CAST(' + c.name + ' AS VARCHAR(MAX)),''''))' from sys.views  t
                               join sys.columns c on t.object_id = c.object_id
                               where t.name = @TableName

                               set @script = 'SELECT ca.ColName, ca.ColValue FROM dbo.'+@TableName+' CROSS APPLY ( Values '
                               set @script = @script+@columns+' ) as CA (ColName, ColValue) WHERE POKey = ' + STR(@POKey)
                               INSERT @Table EXEC sp_executeSql @script

                               SELECT      POI.POInstructionsPOKey, tPOI.InstructionKey, 
                                    POI.POInstructionsMemo, POI.POInstructionsStep, 
                                    tPOI.InstructionSort, POI.POInstructionsMemoFontColor
                                    ,dbo.fnGetPOInstructionText(tPOI.InstructionKey,@VendorLanguageCode, @POKey, @Table)  as InstructionText
                               FROM  tlkpJobPurchaseOrderInstructions tPOI RIGHT OUTER JOIN
                                tblJobPurchaseOrderInstructions POI ON tPOI.InstructionKey = POI.POInstructionsInstructionKey
                               WHERE POI.POInstructionsPOKey = @POKey";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@POKey", SqlDbType.Int).Value = POKey;
                da.SelectCommand.Parameters.Add("@VendorLanguageCode", SqlDbType.Char).Value = VendorLanguagecode;

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
