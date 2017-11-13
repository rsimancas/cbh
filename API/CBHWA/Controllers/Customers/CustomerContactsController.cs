using CBHWA.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Utilidades;

namespace CBHWA.Controllers
{
    [TokenValidation]
    public class CustomerContactsController : ApiController
    {
        static readonly CustomersRepository repository = new CustomersRepository();

        public object GetAll() 
        {
            var queryValues = Request.RequestUri.ParseQueryString();

            int page = Convert.ToInt32(queryValues["page"]);
            int start = Convert.ToInt32(queryValues["start"]);
            int limit = Convert.ToInt32(queryValues["limit"]);
            int id = Convert.ToInt32(queryValues["id"]);
            int custkey = Convert.ToInt32(queryValues["custkey"]);

            #region Configuramos query
            string query = !string.IsNullOrWhiteSpace(queryValues["query"]) ? queryValues["query"] : "";
            string strQueryBy = !string.IsNullOrWhiteSpace(queryValues["queryBy"]) ? queryValues["queryBy"] : "";
            string[] queryBy = (!string.IsNullOrWhiteSpace(strQueryBy)) ? strQueryBy.Split(',') : new string[] { };
            #endregion Configuramos query

            #region Configuramos el orden de la consulta si se obtuvo como parametro
            string strOrder = !string.IsNullOrWhiteSpace(queryValues["sort"]) ? queryValues["sort"] : "";
            strOrder = strOrder.Replace('[', ' ');
            strOrder = strOrder.Replace(']', ' ');

            Sort sort;

            if (!string.IsNullOrWhiteSpace(strOrder))
            {
                sort = JsonConvert.DeserializeObject<Sort>(strOrder);
            }
            else
            {
                sort = new Sort();
            }
            #endregion

            #region Configuramos el filtro de la consulta si se obtuvo como parametro
            string strFilter = !string.IsNullOrWhiteSpace(queryValues["filter"]) ? queryValues["filter"] : "";
            strFilter = strFilter.Replace('[', ' ');
            strFilter = strFilter.Replace(']', ' ');

            Filter filter;

            if (!string.IsNullOrWhiteSpace(strFilter))
            {
                filter = JsonConvert.DeserializeObject<Filter>(strFilter);
            }
            else
            {
                filter = new Filter();
            }
            #endregion Configuramos el filtro de la consulta si se obtuvo como parametro

            int totalRecords = 0;

            try
            {
                IList<CustomerContact> lista = null;

                if (custkey == 0 && id == 0)
                {
                    lista = repository.GetListContacts(query, sort, filter, page, start, limit, ref totalRecords);
                }
                else
                {
                    if (id == 0)
                    {
                        lista = repository.GetContactsByCustomer(query, custkey, page, start, limit, ref totalRecords);
                    }
                    else
                    {
                        CustomerContact contact = repository.GetContact(id);
                        lista = new List<CustomerContact>
                        {
                            contact
                        };
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

        public object Post(CustomerContact model)
        {
            object json;
            string msgError = "";

            try
            {
                model.ContactModifiedDate = DateTime.Now;
                model = repository.AddContact(model, ref msgError);

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

        public object Put(CustomerContact model)
        {
            object json;

            try
            {
                string msgError = "";
                model.ContactModifiedDate = DateTime.Now;
                CustomerContact putting = repository.UpdContact(model, ref msgError);

                if (putting != null)
                {
                    json = new
                    {
                        total = 1,
                        data = putting,
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

        public object Delete(CustomerContact model)
        {
            string msgError = "";

            bool result = repository.RemoveContact(model, ref msgError);

            object json = new
            {
                message = msgError,
                success = result
            };

            return json;
        }
    }
}