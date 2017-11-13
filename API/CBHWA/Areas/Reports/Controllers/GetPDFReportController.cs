using CBHWA.Models;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Utilidades;

namespace CBHWA.Areas.Reports.Controllers
{
    public class GetPDFReportController : Controller
    {
        static readonly IUserRepository userRepository = new UserRepository();
        static NameValueCollection queryValues = null;
        static int currentUser = 0;

        //
        // GET: /Reports/QuoteCustomer/
        public ActionResult Index()
        {

            //dynamic objPB = Activator.CreateInstance(Type.GetTypeFromProgID("XStandard.MD5"));
            //string key = "que clave tan hija de puta ?";
            //LogManager.Write(Utils.GetMd5Hash(key));
            //Utils.ReadDBFUsingOdbc();
            //if (!CheckToken(Request.Headers))
            //{
            //    return RedirectToAction("Error");
            //}
            //return RedirectToAction("GetPDF");

            queryValues = Request.QueryString;
            string pdfFile = Path.Combine(Path.GetTempPath(),queryValues["_file"]+".pdf");

            byte[] b = new byte[1024];

            // Open the stream and read it back. 
            b = Utils.ReadFile(pdfFile);

            System.IO.File.Delete(pdfFile);

            return File(b, "application/pdf");

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

                currentUser = userLogged.EmployeeKey;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

    }
}
