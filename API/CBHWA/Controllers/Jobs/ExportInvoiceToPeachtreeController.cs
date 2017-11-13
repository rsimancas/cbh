using CBHWA.Models;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Utilidades;

namespace CBHWA.Controllers
{
    [TokenValidation]
    public class ExportInvoiceToPeachtreeController : Controller
    {
        static readonly IJobsRepository repository = new JobsRepository();
        static readonly IUserRepository userRepository = new UserRepository();
        User currentUser = new User();

        public ActionResult Index()
        {
            var queryValues = Request.QueryString;
            string token = queryValues["t"];

            if (!CheckToken(token))
            {
                return Content("Missing Authorization Token");
            }

            int InvoiceKey = Convert.ToInt32(queryValues["InvoiceKey"]);

            try
            {
                string filePath = repository.ExportInvoiceToPeachtree(InvoiceKey, currentUser.UserName);

                byte[] fileBytes = Utils.GetFile(filePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
        }

        private bool CheckToken(string token) {
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

                currentUser = userLogged;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}