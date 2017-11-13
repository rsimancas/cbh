using CBHWA.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;

namespace CBHWA.Controllers
{
    [TokenValidation]
    public class VendorOriginAddressController : ApiController
    {
        static readonly VendorsRepository repository = new VendorsRepository();

        public object GetAll()
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);
            int vendorkey = Convert.ToInt32(queryValues["vendorkey"] ?? "0");

            string query = "";

            query = !string.IsNullOrWhiteSpace(queryValues["query"]) ? queryValues["query"] : "";

            int totalRecords = 0;

            try
            {
                IList<VendorOriginAddress> lista;
                if (id > 0)
                {
                    lista = repository.GetOriginAddressById(id);
                }
                else
                {
                    if (vendorkey > 0)
                    {
                        lista = repository.GetOriginAddressByVendor(vendorkey, ref totalRecords);
                    }
                    else
                    {
                        lista = repository.GetOriginAddressAll(ref totalRecords);
                    }
                }

                if (lista != null)
                {
                    foreach (var address in lista)
                    {
                        if (!string.IsNullOrEmpty(address.OriginAddress1))
                        {
                            var index = address.OriginAddress1.IndexOf(Environment.NewLine);
                            address.OriginAddress1 = (index == -1) ? address.OriginAddress1 : address.OriginAddress1.Substring(0, index + 1);
                        }
                    }
                }

                object json = new
                {
                    total = totalRecords,
                    data = lista,
                    success = true
                };

                return json;
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                object error = new { message = ex.Message };

                object json = new
                {
                    message = ex.Message,
                    success = false
                };

                return json;
            }
        }

        public object Post(VendorOriginAddress model)
        {
            object json;
            string msgError = "";

            try
            {
                model.OriginModifiedDate = DateTime.Now;
                model = repository.Add(model, ref msgError);

                if (model != null)
                {
                    json = new
                    {
                        total = 1,
                        data = model,
                        success = true
                    };
                }
                else
                {
                    json = new
                    {
                        message = msgError,
                        success = false
                    };
                };
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                object error = new { message = ex.Message };

                json = new
                {
                    message = ex.Message,
                    success = false
                };
            };

            return json;
        }

        public object Put(VendorOriginAddress model)
        {
            object json;

            try
            {
                string msgError = "";
                model.OriginModifiedDate = DateTime.Now;
                model = repository.Update(model, ref msgError);

                if (model != null)
                {
                    json = new
                    {
                        total = 1,
                        data = model,
                        success = true
                    };
                }
                else
                {
                    json = new
                    {
                        message = msgError,
                        success = false
                    };
                }
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);

                json = new
                {
                    message = ex.Message,
                    success = false
                };
            };

            return json;
        }

        public object Delete(VendorOriginAddress model)
        {
            string msgError = "";

            bool result = repository.Remove(model, ref msgError);

            object json = new
            {
                message = msgError,
                success = result
            };

            return json;
        }
    }
}