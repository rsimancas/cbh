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

    public class rptJobInvoicePackingListController : Controller
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

            var model = new rptJobInvoicePackingList();
            model.id = id.Value;
            model.EmployeeKey = employeeKey;

            var filters = new FieldFilters();
            filters.fields = new List<FieldFilter>();
            filters.fields.Add(new FieldFilter { name = "EmployeeStatusCode", oper = "=", type = "int", value = "8" });
            var sort = new Sort();
            int totalRecords = 0;
            var employeeList = employeeRepo.GetList(filters, null, sort, 0, 0, 0, ref totalRecords).Where(w => w.EmployeeKey != employeeKey).ToList();
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

            lr.ReportPath = "Areas/Reports/ReportDesign/rptJobInvoicePackingList.rdlc";

            lr.DataSources.Add(new ReportDataSource("dsJobInvoicePackingList", dtHeader));

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

        private DataTable GetData(int id, int EmployeeKey)
        {
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {
                var currentUser = employeeRepo.Get(EmployeeKey);
                string sql = @"SELECT      TOP 100 PERCENT dbo.tblInvoiceHeader.InvoiceKey, dbo.fnGetInvoiceNum(dbo.tblInvoiceHeader.InvoiceKey) AS InvoiceNum, 
                                    dbo.tblInvoiceHeader.InvoiceJobKey AS JobKey, dbo.fnGetJobNum(dbo.tblInvoiceHeader.InvoiceJobKey) AS JobNum, 
                                    dbo.tblJobPurchaseOrderItems.POItemsQty, ISNULL(dbo.tblVendors.VendorDisplayToCust, dbo.tblVendors.VendorName) AS VendorName, 
                                    dbo.tblJobPurchaseOrderItems.POItemsItemKey, dbo.tblJobPurchaseOrderItems.POItemsMemoCustomer, 
                                    dbo.tblJobPurchaseOrderItems.POItemsMemoCustomerMoveBottom, dbo.fnGetItemDescription(dbo.tblItems.ItemKey, 
                                    dbo.tblCustomers.CustLanguageCode) AS DescriptionText, dbo.tblItems.ItemNum, dbo.tblItems.ItemWeight, dbo.tblItems.ItemVolume, 
                                    dbo.tblJobPurchaseOrderItems.POItemsQty * dbo.tblItems.ItemWeight AS LineWeight, 
                                    dbo.tblJobPurchaseOrderItems.POItemsQty * dbo.tblItems.ItemVolume AS LineVolume, dbo.tblItems.ItemSchBNum,
                                    dbo.fnGetLocationFooter(1) as LocationFooter
                                FROM          dbo.tblJobPurchaseOrderItems INNER JOIN
                                    dbo.tblJobPurchaseOrders ON dbo.tblJobPurchaseOrderItems.POItemsPOKey = dbo.tblJobPurchaseOrders.POKey INNER JOIN
                                    dbo.tblVendors ON dbo.tblJobPurchaseOrders.POVendorKey = dbo.tblVendors.VendorKey INNER JOIN
                                    dbo.tblInvoiceHeader ON dbo.tblJobPurchaseOrderItems.POItemsJobKey = dbo.tblInvoiceHeader.InvoiceJobKey AND 
                                    dbo.tblJobPurchaseOrders.POInvoiceKey = dbo.tblInvoiceHeader.InvoiceKey INNER JOIN
                                    dbo.tblCustomers ON dbo.tblInvoiceHeader.InvoiceCustKey = dbo.tblCustomers.CustKey INNER JOIN
                                    dbo.tblItems ON dbo.tblJobPurchaseOrderItems.POItemsItemKey = dbo.tblItems.ItemKey
                                WHERE InvoiceKey = @InvoiceKey
                                ORDER BY dbo.tblInvoiceHeader.InvoiceJobKey DESC, dbo.tblJobPurchaseOrderItems.POItemsSort";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@InvoiceKey", SqlDbType.Int).Value = id;


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
