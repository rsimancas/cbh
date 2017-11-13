namespace CBHWA.Areas.Reports.Controllers
{
    using CBHWA.Areas.Reports.Models;
    using CBHWA.Clases;
    using CBHWA.Models;
    using Microsoft.Reporting.WebForms;
    using OfficeOpenXml;
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

    public class rptExcelExportJobProfitReportController : Controller
    {
        private IUserRepository _IUserRepo;
        private IEmployeeRepository _IEmployeeRepo;
        private IFileRepository _IFileRepo;
        static NameValueCollection queryValues = null;

        public rptExcelExportJobProfitReportController()
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
            //labelCriteria = Uri.EscapeUriString(labelCriteria);

            var path = Path.Combine(Request.MapPath("~/App_Data/Excel/"));
            var xlsFile = Path.Combine(path, "ExcelExportJobProfitReport" + DateTime.Now.ToString("yyyyMMddHHss") + ".xls");

            WriteExcel(employeeKey, labelCriteria, strWhere, xlsFile);

            byte[] b = new byte[1024];

            // Open the stream and read it back. 
            try
            {
                if(System.IO.File.Exists(xlsFile)) {
                    b = Utils.ReadFile(xlsFile);
                    if (System.IO.File.Exists(xlsFile))
                    {
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                        System.IO.File.Delete(xlsFile);
                    }
                }
                else
                {
                    throw new Exception(String.Format("File {0} doesn't exists", xlsFile));
                }
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return RedirectToAction("Error");
            }

            Response.ClearContent();
            Response.Buffer = true;

            return File(b, "application/ms-excel", String.Format("ExcelExportJobProfitReport{0}.xls", DateTime.Now.ToString("dd/MM/yyyy")));
        }

        private void WriteExcel(int? employeeKey, string labelCriteria, string strWhere, string xlsFile)
        {
            object misvalue = System.Reflection.Missing.Value;

            try
            {
                string strCurrency = "USD";
                string strReportName = "rptJobProfit";
                FileInfo newFile = new FileInfo(xlsFile);
                ExcelPackage xlPackage = new ExcelPackage(newFile);
                ExcelWorksheet oSheet = xlPackage.Workbook.Worksheets.Add("Job Profit");

                String strExportReportNameOutput = String.Empty;
                String strExportSQL = String.Empty;
                String strExportCentinalField = String.Empty;
                String strExportCentinalStart = String.Empty;
                int intCentinalField = 0;
                int intCentinalStart = 0;
                int intFieldLoop = 0;
                int intFieldLoopStart = 0;
                bool strExportExcel = false;
                int intExportExcelSumField = 0;
                string varExportExcelSumValue = String.Empty;
                int intColumnCounter = 0;
                bool AlreadyDone = false;


                int lngCellRow = 0;
                int lngCellCol = 0;
                int lngJobRowStart = 0;
                int lngJobRowEnd = 0;

                DataTable dt = new DataTable();

                using (SqlConnection oConn = ConnManager.OpenConn())
                {
                    string sql = String.Format("SELECT * FROM tblExportReport WHERE ExportReportName = '{0}'", strReportName);
                    DataSet ds = ConnManager.ExecQuery(sql, oConn);
                    dt = ds.Tables[0];

                }

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    strExportReportNameOutput = (string)row["ExportReportNameOutput"];
                    strExportSQL = (string)row["ExportSQL"];
                    if (row["ExportCentinalField"] != System.DBNull.Value) strExportCentinalField = ((string)(row["ExportCentinalField"])).Trim();
                    if (row["ExportCentinalStart"] != System.DBNull.Value) strExportCentinalStart = ((string)(row["ExportCentinalStart"])).Trim();
                    if (row["exportexcelsum"] != System.DBNull.Value) strExportExcel = (bool)row["exportexcelsum"];
                    if (row["ExportExcelSumField"] != System.DBNull.Value) intExportExcelSumField = Convert.ToInt32(row["ExportExcelSumField"]);
                }
                else
                {
                    return;
                }

                if (string.IsNullOrEmpty(strWhere))
                {
                    strExportSQL = strExportSQL.Replace("REPLACESTRWHEREHERE", "");
                }
                else
                {
                    if (strReportName == "rptPronacaReportClosedShipped" || strReportName == "rptPronacaTransitOrders")
                    {
                        strExportSQL = strExportSQL.Replace("REPLACESTRWHEREHERE", " WHERE " + strWhere);
                    }
                    else
                    {
                        strExportSQL = strExportSQL.Replace("REPLACESTRWHEREHERE", " AND " + strWhere);
                    }

                }

                strExportSQL = strExportSQL.Replace("REPLACECURRENCY", strCurrency);
                //Select Case strReportName
                //    Case Is = "rptPronacaReport NoProfit"
                //    strExportSQL = Replace(strExportSQL, "", "")
                //    Case Else
                //End Select

                lngCellRow = 1;
                lngCellCol = 1;

                oSheet.Select("A1:M5");
                var selected = oSheet.SelectedRange;
                selected.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                selected.Style.WrapText = false;
                selected.Style.ShrinkToFit = true;
                selected.Style.ReadingOrder = OfficeOpenXml.Style.ExcelReadingOrder.ContextDependent;
                selected.Merge = true;
                selected.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                selected.Style.Indent = 0;
                selected.Style.Font.Size = 24;
                selected.Style.Font.Bold = false;
                selected.FormulaR1C1 = String.Format(@"""{0}""", strCurrency);

                oSheet.Select("A6:M6");
                selected = oSheet.SelectedRange;
                selected.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                selected.Style.WrapText = false;
                selected.Style.ShrinkToFit = true;
                selected.Style.ReadingOrder = OfficeOpenXml.Style.ExcelReadingOrder.ContextDependent;
                selected.Merge = true;
                selected.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                selected.Style.Indent = 0;
                selected.Style.Font.Size = 10;
                selected.Style.Font.Bold = false;
                selected.FormulaR1C1 = String.Format(@"""Criteria used:{0}""", labelCriteria);

                using (SqlConnection oConn = ConnManager.OpenConn())
                {
                    DataSet ds = ConnManager.ExecQuery(strExportSQL, oConn);
                    dt = ds.Tables[0];

                }

                intColumnCounter = 0;
                lngCellRow = 8;
                lngCellCol = 1;
                intFieldLoop = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    oSheet.Cells[lngCellRow, lngCellCol].Value = col.ColumnName;
                    if (col.ColumnName.Trim() == strExportCentinalField.Trim()) intCentinalField = intFieldLoop;
                    if (col.ColumnName.Trim() == strExportCentinalStart.Trim()) intCentinalStart = intFieldLoop;
                    lngCellCol = lngCellCol + 1;
                    intFieldLoop++;
                }

                lngCellCol = 1;
                //lngCellRow = 7
                intFieldLoopStart = 0;

                strExportCentinalField = dt.Columns[intCentinalField].ColumnName;

                foreach (DataRow row in dt.Rows)
                {
                    intFieldLoop = intFieldLoopStart;
                    foreach (DataColumn col in dt.Columns)
                    {

                        if (intFieldLoop == intCentinalField)
                        {
                            if (!AlreadyDone)
                            {
                                lngCellRow = lngCellRow + 1;
                                AlreadyDone = true;
                            }
                            intFieldLoopStart = intCentinalField + 1;
                        }

                        oSheet.Cells[lngCellRow, intFieldLoop + 1].Value = row[col.ColumnName];

                        if (intFieldLoop == intExportExcelSumField)
                        {
                            varExportExcelSumValue = varExportExcelSumValue + dt.Columns[intFieldLoop].ColumnName;
                        }

                        intFieldLoop++;
                    }

                    //lngCellRow = lngCellRow + 1;

                    if (strExportCentinalField.Trim() == dt.Columns[intCentinalField].ColumnName.Trim())
                    {
                        intFieldLoopStart = intCentinalField;
                    }
                    else
                    {
                        intFieldLoopStart = 0;
                        strExportCentinalField = dt.Columns[intCentinalField].ColumnName.Trim();
                        //objExcel.Range("A" & lngCellRow & ":" & Chr(64 + intFieldLoop) & lngCellRow).Select
                        //objExcel.Selection.Borders(xlEdgeBottom).LineStyle = xlContinuous
                        //objExcel.Selection.Interior.ColorIndex = 15
                        //AlreadyDone = False
                    }

                    lngCellRow++;

                }


                oSheet.Select("D:D");
                selected = oSheet.SelectedRange;
                selected.Style.Numberformat.Format = "#,##0.00";

                oSheet.Select("F:H");
                selected = oSheet.SelectedRange;
                selected.Style.Numberformat.Format = "#,##0.00";

                //oSheet.Select("G:G");
                //selected = oSheet.SelectedRange;
                //selected.Style.Numberformat.Format = "#,##0.00";

                //oSheet.Select("H:H");
                //selected = oSheet.SelectedRange;
                //selected.Style.Numberformat.Format = "#,##0.00";

                //if (strCurrency == "USD")
                //{
                //    oSheet.Select("D:D");
                //    selected = oSheet.SelectedRange;
                //    selected.Style.Numberformat.Format = "#,##0.00";
                //}
                //else
                //{
                //    //oSheet.Column(4).Style.Numberformat.Format = "[$EUR] #,##0.00_);([$EUR] #,##0.00)";
                //}

                oSheet.Select("B:B");
                selected = oSheet.SelectedRange;
                selected.Style.Numberformat.Format = "mm-dd-yyyy";
                //
                oSheet.Select("A:M");
                selected = oSheet.SelectedRange;
                selected.AutoFitColumns();

                oSheet.Select(String.Format("H{0}",lngCellRow));
                selected = oSheet.SelectedRange;
                selected.Formula = String.Format("SUBTOTAL(9,H9:H{0})", lngCellRow - 1);

                xlPackage.Save();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

        }

        /*private void WriteDocumentExcel(int? employeeKey, string labelCriteria, string strWhere, string xlsFile)
        {
            Microsoft.Office.Interop.Excel.Application oXL;
            Microsoft.Office.Interop.Excel._Workbook oWB;
            Microsoft.Office.Interop.Excel._Worksheet oSheet;
            Microsoft.Office.Interop.Excel.Range oRng;
            object misvalue = System.Reflection.Missing.Value;

            try
            {
                //Start Excel and get Application object.
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = false;

                //Get a new workbook.
                oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

                string strCurrency = "USD";
                string strReportName = "rptJobProfit";
                //FileInfo newFile = new FileInfo(xlsFile);
                //ExcelPackage xlPackage = new ExcelPackage(newFile);
                //ExcelWorksheet oSheet = xlPackage.Workbook.Worksheets.Add("Job Profit");

                String strExportReportNameOutput = String.Empty;
                String strExportSQL = String.Empty;
                String strExportCentinalField = String.Empty;
                String strExportCentinalStart = String.Empty;
                int intCentinalField = 0;
                int intCentinalStart = 0;
                int intFieldLoop = 0;
                int intFieldLoopStart = 0;
                bool strExportExcel = false;
                int intExportExcelSumField = 0;
                string varExportExcelSumValue = String.Empty;
                int intColumnCounter = 0;
                bool AlreadyDone = false;


                int lngCellRow = 0;
                int lngCellCol = 0;
                int lngJobRowStart = 0;
                int lngJobRowEnd = 0;

                DataTable dt = new DataTable();

                using (SqlConnection oConn = ConnManager.OpenConn())
                {
                    string sql = String.Format("SELECT * FROM tblExportReport WHERE ExportReportName = '{0}'", strReportName);
                    DataSet ds = ConnManager.ExecQuery(sql, oConn);
                    dt = ds.Tables[0];

                }

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    strExportReportNameOutput = (string)row["ExportReportNameOutput"];
                    strExportSQL = (string)row["ExportSQL"];
                    if (row["ExportCentinalField"] != System.DBNull.Value) strExportCentinalField = ((string)(row["ExportCentinalField"])).Trim();
                    if (row["ExportCentinalStart"] != System.DBNull.Value) strExportCentinalStart = ((string)(row["ExportCentinalStart"])).Trim();
                    if (row["exportexcelsum"] != System.DBNull.Value) strExportExcel = (bool)row["exportexcelsum"];
                    if (row["ExportExcelSumField"] != System.DBNull.Value) intExportExcelSumField = Convert.ToInt32(row["ExportExcelSumField"]);
                }
                else
                {
                    return;
                }

                if (string.IsNullOrEmpty(strWhere))
                {
                    strExportSQL = strExportSQL.Replace("REPLACESTRWHEREHERE", "");
                }
                else
                {
                    if (strReportName == "rptPronacaReportClosedShipped" || strReportName == "rptPronacaTransitOrders")
                    {
                        strExportSQL = strExportSQL.Replace("REPLACESTRWHEREHERE", " WHERE " + strWhere);
                    }
                    else
                    {
                        strExportSQL = strExportSQL.Replace("REPLACESTRWHEREHERE", " AND " + strWhere);
                    }

                }

                strExportSQL = strExportSQL.Replace("REPLACECURRENCY", strCurrency);
                //Select Case strReportName
                //    Case Is = "rptPronacaReport NoProfit"
                //    strExportSQL = Replace(strExportSQL, "", "")
                //    Case Else
                //End Select

                lngCellRow = 1;
                lngCellCol = 1;

                var selected = oSheet.get_Range("A1:M5");
                selected.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlLeft;
                selected.Style.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                selected.Style.WrapText = false;
                selected.Style.ShrinkToFit = true;
                selected.Style.ReadingOrder = OfficeOpenXml.Style.ExcelReadingOrder.ContextDependent;
                selected.MergeCells = true;
                
                selected.Style.Indent = 0;
                selected.Style.Font.Size = 24;
                selected.Style.Font.Bold = false;
                selected.FormulaR1C1 = String.Format(@"""{0}""", strCurrency);


                selected = oSheet.get_Range("A6:M6");
                selected.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                selected.Style.WrapText = false;
                selected.Style.ShrinkToFit = true;
                selected.Style.ReadingOrder = OfficeOpenXml.Style.ExcelReadingOrder.ContextDependent;
                selected.MergeCells = true;
                selected.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                selected.Style.Indent = 0;
                selected.Style.Font.Size = 10;
                selected.Style.Font.Bold = false;
                selected.FormulaR1C1 = String.Format(@"""Criteria used:{0}""", labelCriteria);

                using (SqlConnection oConn = ConnManager.OpenConn())
                {
                    DataSet ds = ConnManager.ExecQuery(strExportSQL, oConn);
                    dt = ds.Tables[0];

                }

                intColumnCounter = 0;
                lngCellRow = 8;
                lngCellCol = 1;
                intFieldLoop = 0;

                foreach (DataColumn col in dt.Columns)
                {
                    oSheet.Cells[lngCellRow, lngCellCol].Value = col.ColumnName;
                    if (col.ColumnName.Trim() == strExportCentinalField.Trim()) intCentinalField = intFieldLoop;
                    if (col.ColumnName.Trim() == strExportCentinalStart.Trim()) intCentinalStart = intFieldLoop;
                    lngCellCol = lngCellCol + 1;
                    intFieldLoop++;
                }

                lngCellCol = 1;
                //lngCellRow = 7
                intFieldLoopStart = 0;

                strExportCentinalField = dt.Columns[intCentinalField].ColumnName;

                foreach (DataRow row in dt.Rows)
                {
                    intFieldLoop = intFieldLoopStart;
                    foreach (DataColumn col in dt.Columns)
                    {

                        if (intFieldLoop == intCentinalField)
                        {
                            if (!AlreadyDone)
                            {
                                lngCellRow = lngCellRow + 1;
                                AlreadyDone = true;
                            }
                            intFieldLoopStart = intCentinalField + 1;
                        }

                        oSheet.Cells[lngCellRow, intFieldLoop + 1].Value = row[col.ColumnName];

                        if (intFieldLoop == intExportExcelSumField)
                        {
                            varExportExcelSumValue = varExportExcelSumValue + dt.Columns[intFieldLoop].ColumnName;
                        }

                        intFieldLoop++;
                    }

                    //lngCellRow = lngCellRow + 1;

                    if (strExportCentinalField.Trim() == dt.Columns[intCentinalField].ColumnName.Trim())
                    {
                        intFieldLoopStart = intCentinalField;
                    }
                    else
                    {
                        intFieldLoopStart = 0;
                        strExportCentinalField = dt.Columns[intCentinalField].ColumnName.Trim();
                        selected = oSheet.get_Range("A" + lngCellRow + ":" + ((char)(64 + intFieldLoop)) + lngCellRow);
                        selected.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                        selected.Interior.ColorIndex = 15;
                        AlreadyDone = false;
                    }

                    lngCellRow++;

                }


                selected = oSheet.get_Range("D:D");
                selected.Style.Numberformat.Format = "#,##0.00";

                selected = oSheet.get_Range("F:H");
                selected.Style.Numberformat.Format = "#,##0.00";

                //oSheet.Select("G:G");
                //selected = oSheet.SelectedRange;
                //selected.Style.Numberformat.Format = "#,##0.00";

                //oSheet.Select("H:H");
                //selected = oSheet.SelectedRange;
                //selected.Style.Numberformat.Format = "#,##0.00";

                //if (strCurrency == "USD")
                //{
                //    oSheet.Select("D:D");
                //    selected = oSheet.SelectedRange;
                //    selected.Style.Numberformat.Format = "#,##0.00";
                //}
                //else
                //{
                //    //oSheet.Column(4).Style.Numberformat.Format = "[$EUR] #,##0.00_);([$EUR] #,##0.00)";
                //}

                selected = oSheet.get_Range("B:B");
                selected.Style.Numberformat.Format = "mm-dd-yyyy";
                //

                selected = oSheet.get_Range("A:M");
                selected.EntireColumn.AutoFit();

                selected = oSheet.get_Range(String.Format("H{0}", lngCellRow));
                selected.Formula = String.Format("SUBTOTAL(9,H9:H{0})", lngCellRow - 1);

                selected = oSheet.get_Range("A8:" + ((char)(64 + intFieldLoop)).ToString() + lngCellRow.ToString());
                var arrInt = new int[] {intExportExcelSumField};
                var listInt = new List<int>(arrInt);
                selected.Subtotal( 1, Microsoft.Office.Interop.Excel.XlConsolidationFunction.xlSum, listInt, false, false);

                oWB.SaveAs(xlsFile);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

        }*/

        private void WriteExcelOld(int? employeeKey, string labelCriteria, string strWhere, string xlsFile)
        {
            ////Microsoft.Office.Interop.Excel.Application oXL;
            ////Microsoft.Office.Interop.Excel._Workbook oWB;
            ////Microsoft.Office.Interop.Excel._Worksheet oSheet;
            ////Microsoft.Office.Interop.Excel.Range oRng;
            //object misvalue = System.Reflection.Missing.Value;

            //try
            //{
            //    //Start Excel and get Application object.
            //    //oXL = new Microsoft.Office.Interop.Excel.Application();
            //    //oXL.Visible = false;

            //    //Get a new workbook.
            //    //oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
            //    //oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            //    FileInfo newFile = new FileInfo(xlsFile);
            //    ExcelPackage xlPackage = new ExcelPackage(newFile);
            //    ExcelWorksheet oSheet = xlPackage.Workbook.Worksheets.Add("Job Profit");

            //    Decimal dblCurrencyRate;
            //    int lngCellRow;
            //    int lngCellCol;
            //    int lngJobRowStart;
            //    int lngJobRowEnd;
            //    Decimal curTotalRevenue;

            //    //'*** Row 1 is header data
            //    #region Header
            //    lngCellRow = 1;
            //    lngCellCol = 1;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Job Num";
            //    var cell = oSheet.Cells[lngCellRow, lngCellCol];
            //    oSheet.Column(lngCellCol).Width = 16;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "@";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 2;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Quote Num";
            //    oSheet.Column(lngCellCol).Width = 11;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "@";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 3;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Ship Date";
            //    oSheet.Column(lngCellCol).Width = 12;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "dd mmm yyyy";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 4;
            //    //oSheet.Cells[lngCellRow, lngCellCol].Value = "Customer";
            //    oSheet.Column(lngCellCol).Width = 35;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "@";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 5;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Contact";
            //    oSheet.Column(lngCellCol).Width = 25;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "@";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 6;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Country";
            //    oSheet.Column(lngCellCol).Width = 15;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "@";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 7;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "PO Num";
            //    oSheet.Column(lngCellCol).Width = 9;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "@";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 8;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Date";
            //    oSheet.Column(lngCellCol).Width = 12;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "dd mmm yyyy";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 9;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Vendor Name";
            //    oSheet.Column(lngCellCol).Width = 35;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "@";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 10;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Charges";
            //    oSheet.Column(lngCellCol).Width = 15;
            //    //oSheet.Columns[lngCellCol].Style = "Currency";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 11;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Items";
            //    oSheet.Column(lngCellCol).Width = 15;
            //    //oSheet.Columns[lngCellCol].Style = "Currency";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 12;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Total";
            //    oSheet.Column(lngCellCol).Width = 15;
            //    //oSheet.Columns[lngCellCol].Style = "Currency";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 13;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Inv Num";
            //    oSheet.Column(lngCellCol).Width = 9;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "@";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 14;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Date";
            //    oSheet.Column(lngCellCol).Width = 12;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "dd mmm yyyy";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 15;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Name";
            //    oSheet.Column(lngCellCol).Width = 35;
            //    //oSheet.Columns[lngCellCol].NumberFormat = "@";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 16;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Charges";
            //    oSheet.Column(lngCellCol).Width = 15;
            //    //oSheet.Columns[lngCellCol].Style = "Currency";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 17;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Items";
            //    oSheet.Column(lngCellCol).Width = 15;
            //    //oSheet.Columns[lngCellCol].Style = "Currency";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 18;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Total";
            //    oSheet.Column(lngCellCol).Width = 15;
            //    //oSheet.Columns[lngCellCol].Style = "Currency";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 19;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "Profit";
            //    oSheet.Column(lngCellCol).Width = 15;
            //    //oSheet.Columns[lngCellCol].Style = "Currency";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    lngCellCol = 20;
            //    oSheet.Cells[lngCellRow, lngCellCol].Value = "%";
            //    oSheet.Column(lngCellCol).Width = 12;
            //    //oSheet.Columns[lngCellCol].Style = "Percent";
            //    //oSheet.Columns[lngCellCol].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            //    //oSheet.Rows[1].NumberFormat = "@";
            //    //oSheet.Rows[1].HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            //    //oSheet.Rows[1].Font.Bold = true;
            //    #endregion Header

            //    DataTable dtHeader = GetData(strWhere, labelCriteria, employeeKey.Value);

            //    lngCellRow = 2;
            //    foreach (DataRow jobProfit in dtHeader.Rows)
            //    {
            //        dblCurrencyRate = 0;
            //        if (jobProfit["JobCustCurrencyRate"] != System.DBNull.Value)
            //            dblCurrencyRate = Convert.ToDecimal(jobProfit["JobCustCurrencyRate"]);
            //        lngJobRowStart = lngCellRow;
            //        lngJobRowEnd = lngCellRow;
            //        int JobKey = Convert.ToInt32(jobProfit["JobKey"]);

            //        oSheet.Cells[lngCellRow, 1].Value = jobProfit["JobNum"];
            //        oSheet.Cells[lngCellRow, 2].Value = jobProfit["QuoteNum"];
            //        oSheet.Cells[lngCellRow, 3].Value = jobProfit["JobShipDate"];
            //        oSheet.Cells[lngCellRow, 4].Value = jobProfit["CustName"];
            //        oSheet.Cells[lngCellRow, 5].Value = jobProfit["ContactTitle"] + " " + jobProfit["ContactFirstName"] + " " + jobProfit["ContactLastName"];
            //        oSheet.Cells[lngCellRow, 6].Value = jobProfit["ShipCountryName"];

            //        //'*** Purchase Orders
            //        DataTable dtPO = GetJobProfitSubPurchaseOrders(JobKey);

            //        foreach (DataRow jobSPO in dtPO.Rows)
            //        {
            //            oSheet.Cells[lngCellRow, 1].Value = jobProfit["JobNum"];

            //            oSheet.Cells[lngCellRow, 7].Value = jobSPO["DetailNum"];
            //            oSheet.Cells[lngCellRow, 8].Value = jobSPO["DetailDate"];
            //            oSheet.Cells[lngCellRow, 9].Value = jobSPO["Name"];
            //            decimal charges = 0;
            //            if (jobSPO["Charges"] != System.DBNull.Value)
            //                charges = Convert.ToDecimal(jobSPO["Charges"]);
            //            decimal items = 0;
            //            if (jobSPO["Items"] != System.DBNull.Value)
            //                items = Convert.ToDecimal(jobSPO["Items"]);
            //            decimal totals = 0;
            //            if (jobSPO["Total"] != System.DBNull.Value)
            //                totals = Convert.ToDecimal(jobSPO["Total"]);
            //            oSheet.Cells[lngCellRow, 10].Value = charges * dblCurrencyRate;
            //            oSheet.Cells[lngCellRow, 11].Value = items * dblCurrencyRate;
            //            oSheet.Cells[lngCellRow, 12].Value = totals * dblCurrencyRate;

            //            lngJobRowEnd = lngCellRow;
            //            lngCellRow = lngCellRow + 1;
            //        }

            //        //'*** Invoices
            //        LogManager.Write("'*** Invoices");
            //        DataTable dtInv = GetJobProfitSubInvoices(JobKey);

            //        lngCellRow = lngJobRowStart;
            //        curTotalRevenue = 0;

            //        foreach (DataRow jobSIN in dtInv.Rows)
            //        {
            //            oSheet.Cells[lngCellRow, 1].Value = jobProfit["JobNum"];

            //            oSheet.Cells[lngCellRow, 13].Value = jobSIN["DetailNum"];
            //            oSheet.Cells[lngCellRow, 14].Value = jobSIN["DetailDate"];
            //            oSheet.Cells[lngCellRow, 15].Value = jobSIN["Name"];
            //            decimal charges = 0;
            //            if (jobSIN["Charges"] != System.DBNull.Value)
            //                charges = Convert.ToDecimal(jobSIN["Charges"]);
            //            decimal items = 0;
            //            if (jobSIN["Items"] != System.DBNull.Value)
            //                items = Convert.ToDecimal(jobSIN["Items"]);
            //            decimal totals = 0;
            //            if (jobSIN["Total"] != System.DBNull.Value)
            //                totals = Convert.ToDecimal(jobSIN["Total"]);
            //            oSheet.Cells[lngCellRow, 16].Value = charges * dblCurrencyRate;
            //            oSheet.Cells[lngCellRow, 17].Value = items * dblCurrencyRate;
            //            oSheet.Cells[lngCellRow, 18].Value = totals * dblCurrencyRate;
            //            curTotalRevenue = curTotalRevenue + (totals * dblCurrencyRate);

            //            if (lngCellRow > lngJobRowEnd) lngJobRowEnd = lngCellRow;
            //            lngCellRow = lngCellRow + 1;
            //        }


            //        //'*** Totals
            //        lngCellRow = lngJobRowStart;

            //        oSheet.Cells[lngCellRow, 19].Formula = "=Sum(L" + lngJobRowStart + ":L" + lngJobRowEnd + ") + Sum(R" + lngJobRowStart + ":R" + lngJobRowEnd + ") ";

            //        if (Convert.ToInt32(jobProfit["CountPO"]) == 0 && Convert.ToInt32(jobProfit["CountIN"]) > 0)
            //        { //'*** No purchase orders, this means commission only invoice, use Invoice Items as total revenue
            //            oSheet.Cells[lngCellRow, 20].Formula = "=S" + lngJobRowStart + " / Sum(Q" + lngJobRowStart + ":Q" + lngJobRowEnd + ") ";
            //        }
            //        else
            //        {
            //            if (curTotalRevenue > 0)
            //            { //'*** Prevent DIV/0 Error
            //                oSheet.Cells[lngCellRow, 20].Formula = "=S" + lngJobRowStart + " / Sum(R" + lngJobRowStart + ":R" + lngJobRowEnd + ") ";
            //            }
            //        }

            //        //'*** Draw border around job
            //        //oSheet.get_Range("A" + lngJobRowStart, "T" + lngJobRowEnd).BorderAround2(xlMedium, , RGB(0, 0, 0);

            //        lngCellRow = lngJobRowEnd + 1;

            //    }


            //    //'*** Grand Totals
            //    lngCellRow = lngCellRow + 2;

            //    oSheet.Cells[lngCellRow, 17].Value = "GRAND TOTAL - ";
            //    //var oRng = oSheet.Cells["Q" + lngCellRow, "R" + lngCellRow);
            //    //oRng.Merge();
            //    //oRng.HorizontalAlignment ;

            //    //oRng = oSheet.get_Range("Q" + lngCellRow, "T" + lngCellRow);
            //    //oRng.Font.Bold = true;

            //    oSheet.Cells[lngCellRow, 19].Formula = String.Format("=Sum(S2:S{0})", lngCellRow - 3);
            //    oSheet.Cells[lngCellRow, 20].Formula = String.Format("=Average(T2:T{0})", lngCellRow - 3);

            //    //oSheet.Cells[1, 1] = "First Name";
            //    //oSheet.Cells[1, 2] = "Last Name";
            //    //oSheet.Cells[1, 3] = "Full Name";
            //    //oSheet.Cells[1, 4] = "Salary";

            //    //Format A1:D1 as bold, vertical alignment = center.
            //    //oSheet.get_Range("A1", "D1").Font.Bold = true;
            //    //oSheet.get_Range("A1", "D1").VerticalAlignment =
            //    //    Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            //    // Create an array to multiple values at once.
            //    //string[,] saNames = new string[5, 2];

            //    //saNames[0, 0] = "John";
            //    //saNames[0, 1] = "Smith";
            //    //saNames[1, 0] = "Tom";

            //    //saNames[4, 1] = "Johnson";

            //    ////Fill A2:B6 with an array of values (First and Last Names).
            //    //oSheet.get_Range("A2", "B6").Value2 = saNames;

            //    ////Fill C2:C6 with a relative formula (=A2 & " " & B2).
            //    //oRng = oSheet.get_Range("C2", "C6");
            //    //oRng.Formula = "=A2 & \" \" & B2";

            //    ////Fill D2:D6 with a formula(=RAND()*100000) and apply format.
            //    //oRng = oSheet.get_Range("D2", "D6");
            //    //oRng.Formula = "=RAND()*100000";
            //    //oRng.NumberFormat = "$0.00";

            //    ////AutoFit columns A:D.
            //    //oRng = oSheet.get_Range("A1", "D1");
            //    //oRng.EntireColumn.AutoFit();
            //    LogManager.Write("xlsFile 3: " + xlsFile);
            //    //oXL.Visible = false;
            //    //oXL.UserControl = false;
            //    //LogManager.Write(xlsFile + "PASO ACA");
            //    //oWB.SaveAs(xlsFile, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
            //    //    false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
            //    //    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            //    ////oWB.expor

            //    //oWB.Close();

            //    xlPackage.Save();
            //}
            //catch (Exception ex)
            //{
            //    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            //}

        }
    }
}