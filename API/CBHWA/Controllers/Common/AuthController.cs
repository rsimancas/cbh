using CBHWA.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CBHWA.Controllers
{
    [AllowAnonymous]
    //[TokenValidation]
    public class AuthController : ApiController
    {
        static readonly IUserRepository userRepository = new UserRepository();

        public HttpResponseMessage PostAuth(JObject jsonRequest)
        {

            var userName = (string)jsonRequest["data"]["UserName"];
            var userPassword = (string)jsonRequest["data"]["UserPassword"];

            var userLogged = userRepository.ValidLogon(userName, userPassword);

            //valor.UserPassword = "";

            if (userLogged == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Existe el usuario");
                //return Request.CreateErrorResponse(HttpStatusCode.NotFound, String.Format("E: {0}, D:{1}",encrypted, decrypted));
            }

            string token = userRepository.GenToken(userLogged);

            // Limpiamos el password para no devolverlo como objeto
            userLogged.UserPassword = "";

            object json = new
            {
                data = userLogged,
                security = token
            };

            return Request.CreateResponse(HttpStatusCode.OK, json);
        }
    }
}