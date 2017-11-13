using CBHWA.Models;
using CBHWA.ViewModels;
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
    public class ExportQuoteToPeachtreeController : Controller
    {
        static readonly IFileRepository repository = new FileRepository();
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

            int QHdrKey = Convert.ToInt32(queryValues["QHdrKey"]);

            try
            {
                string filePath = repository.ExportQuoteToPeachtree(QHdrKey, currentUser.UserName);

                if (string.IsNullOrEmpty(filePath))
                {
                    var model = new ErrorExportModel { Text = "There are no items in this quote!" };
                    return View("ErrorExport", model);
                }

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