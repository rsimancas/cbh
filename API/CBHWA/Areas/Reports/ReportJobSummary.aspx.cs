using Microsoft.Reporting.WebForms;
using Neodynamic.SDK.Web;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilidades;

namespace CBHWA.Areas.Reports.Views.rptJobSummary
{
    public partial class ReportJobSummary : System.Web.UI.Page
    {
        protected void Page_Init(object sender, System.EventArgs e)
        {
            if (WebClientPrint.ProcessPrintJob(HttpContext.Current.Request))
            {
                var nvc = HttpContext.Current.Request.QueryString;

                string strWhere = nvc["strWhere"] ?? "rptDate >= CAST('2016-07-01' AS DATE) AND rptDate <= CAST('2016-09-28' AS DATE) AND StatusKey = 100";
                string labelCriteria = nvc["labelCriteria"] ?? "";
                string strEmployeeKey = nvc["employeeKey"] ?? "";
                int employeeKey = 0;
                int.TryParse(strEmployeeKey, out employeeKey);

                reportViewer.Reset();

                DataTable dtHeader = GetJobSummary(strWhere, labelCriteria, employeeKey);
                DataTable dtInv = GetSummaryInvoices();
                DataTable dtPO = GetSummarySubPurchaseOrders();

                LocalReport lr = reportViewer.LocalReport;

                lr.ReportPath = "Areas/Reports/ReportDesign/rptJobSummary.rdlc";
                lr.EnableExternalImages = true;

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
                byte[] pdfContent;

                //Export to PDF. Get binary content.
                pdfContent = lr.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                //Now send this file to the client side for printing
                //IMPORTANT: Adobe Reader needs to be installed at the client side

                //Get printer name
                string printerName = Server.UrlDecode(HttpContext.Current.Request.QueryString["printerName"]);

                //'create a temp file name for our PDF report...
                string fileName = Guid.NewGuid().ToString("N") + ".pdf";

                //Create a PrintFile object with the pdf report
                var file = new PrintFile(pdfContent, fileName);
                //'Create a ClientPrintJob and send it back to the client!
                var cpj = new ClientPrintJob();
                //'set file to print...
                cpj.PrintFile = file;
                //set client printer...
                if (printerName == "Default Printer")
                {
                    cpj.ClientPrinter = new DefaultPrinter();
                }
                else
                {
                    cpj.ClientPrinter = new InstalledPrinter(printerName);
                }
                //send it...
                cpj.SendToClient(Response);
            }

        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
                RenderReport();
        }

        private void RenderReport()
        {
            var nvc = HttpContext.Current.Request.QueryString;

            string strWhere = nvc["strWhere"] ?? "rptDate >= CAST('2016-07-01' AS DATE) AND rptDate <= CAST('2016-09-28' AS DATE) AND StatusKey = 100";
            string labelCriteria = nvc["labelCriteria"] ?? "";
            string strEmployeeKey = nvc["employeeKey"] ?? "";
            int employeeKey = 0;
            int.TryParse(strEmployeeKey, out employeeKey);

            reportViewer.Reset();

            DataTable dtHeader = GetJobSummary(strWhere, labelCriteria, employeeKey);
            DataTable dtInv = GetSummaryInvoices();
            DataTable dtPO = GetSummarySubPurchaseOrders();

            LocalReport lr = reportViewer.LocalReport;
            lr.EnableExternalImages = true;

            lr.ReportPath = "Areas/Reports/ReportDesign/rptJobSummary.rdlc";

            lr.DataSources.Clear();
            lr.DataSources.Add(new ReportDataSource("dsRptJobSummary", dtHeader));
            lr.DataSources.Add(new ReportDataSource("dsRptJobSummaryInvoices", dtInv));
            lr.DataSources.Add(new ReportDataSource("dsRptJobSummarySubPurchaseOrders", dtPO));

            lr.SubreportProcessing += new SubreportProcessingEventHandler(lr_SubreportProcessing);
            lr.SubreportProcessing += lr_SubreportProcessing;

            //AddPrintBtn(reportViewer);

            lr.Refresh();
        }

        private void AddPrintBtn(ReportViewer rv)
        {
            foreach (Control c in rv.Controls)
            {
                foreach (Control c1 in c.Controls)
                {
                    foreach (Control c2 in c1.Controls)
                    {
                        foreach (Control c3 in c2.Controls)
                        {
                            if (c3.ToString() == "Microsoft.Reporting.WebForms.ToolbarControl")
                            {
                                foreach (Control c4 in c3.Controls)
                                {
                                    if (c4.ToString() == "Microsoft.Reporting.WebForms.PageNavigationGroup")
                                    {
                                        var btn = new Button();
                                        btn.Text = "Enqueue";
                                        btn.ID = "btnEnqueue";
                                        btn.OnClientClick = string.Format("alert('{0} {1}');", reportViewer.ClientID, btn.ClientID);
                                        c4.Controls.Add(btn);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
    }
}