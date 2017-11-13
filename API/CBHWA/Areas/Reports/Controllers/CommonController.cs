using CBHWA.Areas.Reports.Models;
using CBHWA.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Utilidades;

namespace CBHWA.Areas.Reports.Controllers
{
    public class CommonController : Controller
    {
        static readonly IEmployeeRepository employeeRepo = new EmployeeRepository();

        [HttpPost]
        public ActionResult Enqueue(Enqueue model)
        {
            var result = employeeRepo.EnqueueReport(model);

            if (result)
            {
                return Content("OK");
            }
            else
            {
                return Content("");
            }
        }

        [HttpPost]
        public ActionResult UpdatePaymentTerm(PaymentPercent model)
        {
            var result = UpdatePaymentTerm(model.QHdrKey);

            if (result)
            {
                return Content("OK");
            }
            else
            {
                return Content("");
            }
        }

        public ActionResult Message(string message, string type)
        {
            var model = new ReportMessage();
            model.message = message ?? "Error at display";
            model.type = type ?? "warning";
            return View(model);
        }

        private bool UpdatePaymentTerm(int QHdrKey)
        {
            using (SqlConnection oConn = ConnManager.OpenConn())
            {

                string sql = @"UPDATE tblFileQuoteHeader SET QHdrCustPaymentTerms = 5 WHERE QHdrKey = @QHdrKey";

                SqlCommand cmd = new SqlCommand(sql, oConn);
                cmd.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = QHdrKey;

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                    return false;
                }

            }

            return true;
        }
    }
}