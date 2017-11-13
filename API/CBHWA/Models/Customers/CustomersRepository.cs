namespace CBHWA.Models
{
    using CBHWA.Clases;
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using Utilidades;

    public class CustomersRepository : ICustomersRepository
    {
        #region "Customer"

        public IList<Customer> GetList(FieldFilters fieldFilters, string query, Sort sort, string[] queryBy, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = "1=1";

            if (fieldFilters.fields != null && fieldFilters.fields.Count > 0)
            {
                foreach (var item in fieldFilters.fields)
                {
                    string value = item.value;
                    string name = item.name;

                    if (item.type == "string" || item.type == "date") 
                        value = "'" + value + "'";

                    if (item.type == "date")
                        name = String.Format("CAST({0} as DATE)", name);

                    where += String.Format(" AND {0} = {1}", name, value) ;
                }
            }

            #region Query By Settings
            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "";

                if (queryBy.Length == 0)
                {
                    fieldName = "CustName+ISNULL(CustAddress1,'')+ISNULL(CustAddress2,'')+ISNULL(CustCity,'')+STR(CustKey)";
                }
                else
                {
                    foreach (string field in queryBy)
                    {
                        fieldName += (!String.IsNullOrWhiteSpace(fieldName)) ? " + " : "";
                        fieldName += field;
                    }
                }

                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                        EnumExtension.generateLikeWhere(query, fieldName);

            }
            #endregion Query By Settings

            #region Ordenamiento
            string order = "CustName";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;

                //if (order == "x_Estatus") order = "EstatusTipo";
            }
            #endregion Ordenamiento

            string sql = @"WITH qData
                            AS
                            (
                            SELECT *, 
                                ROW_NUMBER() OVER (ORDER BY {2} {3}) as row
                            FROM tblCustomers WHERE {0}
                            )
                            SELECT *,IsNull((select count(*) from qData),0)  as TotalRecords 
                            FROM qData
                            WHERE {1}
                            ORDER BY row";

            sql = String.Format(sql, where, wherepage, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<Customer> data = EnumExtension.ToList<Customer>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public Customer Get(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            Customer data = Get(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public Customer Add(Customer customer, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string sql = "INSERT INTO tblCustomers ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(customer, "CustKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyGenerated = 0;

            try
            {
                keyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            Customer data = Get(keyGenerated, oConn);

            setDefaultShipAddress(data);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public Customer Update(Customer customer, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string sql = "UPDATE tblCustomers SET {0} WHERE CustKey = " + customer.CustKey.ToString();

            EnumExtension.setUpdateValues(customer, "CustKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            Customer data = Get(customer.CustKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private Customer Get(int id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblCustomers " +
                         " WHERE (CustKey = @itemkey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<Customer> data = EnumExtension.ToList<Customer>(dt);

            return data.FirstOrDefault<Customer>();
        }

        public bool Remove(Customer customer)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            bool result;
            try
            {
                result = Remove(customer, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(Customer customer, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblCustomers " +
                         " WHERE (CustKey = {0})";

            sql = String.Format(sql, customer.CustKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public IList<CustomerStatus> GetCustomerStatus()
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string sql = "SELECT * FROM tlkpCustomerStatus " +
                         "ORDER BY StatusKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<CustomerStatus> data = EnumExtension.ToList<CustomerStatus>(dt);

            return data;
        }

        #endregion "Customer"

        #region "Contacts"
        public IList<CustomerContact> GetListContacts(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = "1=1";

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and {0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "ISNULL(ContactLastName,'')+ISNULL(ContactFirstName,'')";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "ContactKey";
            string direction = "DESC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

            string sql = "SELECT * FROM ( " +
                         "SELECT *, ISNULL(ContactFirstName,'')+' '+ISNULL(ContactLastName,'') as x_ContactFullName, " +
                         "	ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,   " +
                         "	IsNull((SELECT count(*) FROM tblCustomerContacts WHERE {0}),0)  as TotalRecords  " +
                         "FROM tblCustomerContacts WHERE {0}) a " +
                         " WHERE {1} " +
                         " ORDER BY row";


            sql = String.Format(sql, where, wherepage, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<CustomerContact> data = EnumExtension.ToList<CustomerContact>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public IList<CustomerContact> GetContactsByCustomer(string query, int custkey, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = String.Format("ContactCustKey = {0}", custkey);

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "ContactLastName+ContactFirstName";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            string sql = "SELECT * FROM (" +
                         " SELECT *, ISNULL(ContactFirstName,'')+' '+ISNULL(ContactLastName,'') as x_ContactFullName, " +
                         "  ROW_NUMBER() OVER (ORDER BY ContactFirstName) as row,  " +
                         "  IsNull((select count(*) from tblCustomerContacts where {0}),0)  as TotalRecords " +
                         " FROM tblCustomerContacts " +
                         " WHERE {0} " +
                         ") a " +
                         " WHERE {1} " +
                         " ORDER BY row";

            sql = String.Format(sql, where, wherepage);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<CustomerContact> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                data = EnumExtension.ToList<CustomerContact>(dt);
            }
            else
            {
                ConnManager.CloseConn(oConn);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return data;
        }

        public CustomerContact GetContact(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            CustomerContact data = GetContact(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private CustomerContact GetContact(int id, SqlConnection oConn)
        {
            string sql = @"SELECT *,ISNULL(ContactFirstName,'')+' '+ISNULL(ContactLastName,'') as x_ContactFullName 
                         FROM tblCustomerContacts 
                         WHERE (ContactKey = @itemkey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<CustomerContact> data = EnumExtension.ToList<CustomerContact>(dt);

            return data.FirstOrDefault<CustomerContact>();
        }

        public CustomerContact AddContact(CustomerContact contact, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            // asigna date add por defecto
            contact.ContactModifiedDate = System.DateTime.Now;

            string sql = "INSERT INTO tblCustomerContacts ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(contact, "ContactKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyGenerated = 0;

            try
            {
                keyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            CustomerContact data = GetContact(keyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public CustomerContact UpdContact(CustomerContact contact, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            // asigna date add por defecto
            contact.ContactModifiedDate = System.DateTime.Now;
            string sql = "UPDATE tblCustomerContacts SET {0} WHERE ContactKey = " + contact.ContactKey.ToString();

            EnumExtension.setUpdateValues(contact, "ContactKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            CustomerContact data = GetContact(contact.ContactKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool RemoveContact(CustomerContact contact, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            bool result;
            try
            {
                result = RemoveContact(contact, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool RemoveContact(CustomerContact contact, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblCustomerContacts " +
                         " WHERE (ContactKey = {0})";

            sql = String.Format(sql, contact.ContactKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion "Contacts"

        #region "ShipAddress"

        public IList<CustomerShipAddress> GetShipAddressById(int idaddress)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string sql = String.Format("SELECT *,dbo.fnGetCustShipAddress(ShipKey) as x_FullShipAddress FROM tblCustomerShipAddress WHERE (ShipKey = {0})", idaddress);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<CustomerShipAddress> data = EnumExtension.ToList<CustomerShipAddress>(dt);

            return data;
        }

        public IList<CustomerShipAddress> GetShipAddressByCustomer(int custkey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string sql = @"SELECT a.*,ISNULL(b.CountryName,'') as x_CountryName, 
                            ISNULL((SELECT TOP 1 StateName FROM tblStates c WHERE a.ShipState=c.StateCode ORDER BY NULLIF (StateCountryKey, 232), StateCountry, StateName),'') as x_StateName, 
                            dbo.fnGetCustShipAddress(a.ShipKey) as x_FullShipAddress
                         FROM tblCustomerShipAddress a LEFT OUTER JOIN tblCountries b on a.ShipCountryKey=b.CountryKey
                         WHERE a.ShipCustKey={0}";

            sql = String.Format(sql, custkey);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<CustomerShipAddress> data = EnumExtension.ToList<CustomerShipAddress>(dt);

            return data;
        }

        public CustomerShipAddress GetShipAddress(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            CustomerShipAddress data = GetShipAddress(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private CustomerShipAddress GetShipAddress(int id, SqlConnection oConn)
        {
            string sql = @"SELECT *,
                            dbo.fnGetCustShipAddress(ShipKey) as x_FullShipAddress FROM tblCustomerShipAddress 
                            WHERE (ShipKey = @itemkey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<CustomerShipAddress> data = EnumExtension.ToList<CustomerShipAddress>(dt);

            return data.FirstOrDefault<CustomerShipAddress>();
        }

        public CustomerShipAddress AddShipAddress(CustomerShipAddress address, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string sql = "INSERT INTO tblCustomerShipAddress ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(address, "ShipKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyGenerated = 0;

            try
            {
                keyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            CustomerShipAddress data = GetShipAddress(keyGenerated, oConn);

            if (data.ShipDefault) setShipAddressDefault(data, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public CustomerShipAddress UpdShipAddress(CustomerShipAddress address, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string sql = "UPDATE tblCustomerShipAddress SET {0} WHERE ShipKey = " + address.ShipKey.ToString();

            EnumExtension.setUpdateValues(address, "ShipKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            CustomerShipAddress data = GetShipAddress(address.ShipKey, oConn);

            if (data.ShipDefault) setShipAddressDefault(data, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool RemoveShipAddress(CustomerShipAddress address, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            bool result;
            try
            {
                result = RemoveShipAddress(address, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool RemoveShipAddress(CustomerShipAddress address, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblCustomerShipAddress " +
                         " WHERE (ShipKey = {0})";

            sql = String.Format(sql, address.ShipKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        private void setShipAddressDefault(CustomerShipAddress address, SqlConnection oConn)
        {
            string sql = "UPDATE tblCustomerShipAddress set ShipDefault = 0 " +
                         " WHERE ShipCustKey = {0} and ShipKey <> {1} ";

            sql = String.Format(sql, address.ShipCustKey, address.ShipKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());
        }

        private void setDefaultShipAddress(Customer customer)
        {
            CustomerShipAddress address = new CustomerShipAddress();
            address.ShipCustKey = customer.CustKey;
            address.ShipCity = customer.CustCity;
            address.ShipCountryKey = customer.CustCountryKey;
            address.ShipDefault = true;
            address.ShipPhone = customer.CustPhone;
            address.ShipState = customer.CustState;
            address.ShipName = customer.CustName;
            address.ShipZip = customer.CustZip;
            address.ShipModifiedBy = customer.CustCreatedBy;
            address.ShipModifiedDate = customer.CustCreatedDate;
            address.ShipAddress1 = customer.CustAddress1;
            address.ShipAddress2 = customer.CustAddress2;

            string msgError = "";
            AddShipAddress(address, ref msgError);
        }

        #endregion "ShipAddress"

        #region "For Report"
        public IList<CustomerForReport> GetListForReport(string startDate, string endDate, string reportName, string query, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string sql = @"SELECT a.CustKey, a.CustName, a.CustCity, a.CustState, b.CountryName 
                            FROM tblCustomers a LEFT OUTER JOIN tblCountries b ON a.CustCountryKey = b.CountryKey 
                            WHERE a.CustKey = 0";

            string where = "";

            if ((new string[] {"rptJobProfit", "rptJobProfitWithExemptions", "ExcelJobProfit", "rptJobSummary"}).Contains(reportName)) {
                sql = @"SELECT a.CustKey, a.CustName, a.CustCity, a.CustState, b.CountryName 
                        FROM tblCustomers a LEFT OUTER JOIN tblCountries b ON a.CustCountryKey = b.CountryKey 
                        WHERE a.CustKey IN (SELECT DISTINCT JobCustKey FROM tblJobHeader WHERE {0})
                        ORDER BY a.CustName";

                where = "1=1";
                if (!string.IsNullOrEmpty(startDate))
                    where += string.Format(" AND CAST(JobShipDate as Date) >= '{0}'", startDate);
                if (!string.IsNullOrEmpty(endDate))
                    where += string.Format(" AND CAST(JobShipDate as Date) <= '{0}'", endDate);

                where = where.Replace("1=1 AND", "");

                sql = string.Format(sql, where).Replace("WHERE 1=1", "");
            }

            if ((new string[] { "rptJobPurchaseOrderStatusReport" }).Contains(reportName))
            {
                sql = @"SELECT a.CustKey, a.CustName, a.CustCity, a.CustState, b.CountryName 
                        FROM tblCustomers a LEFT OUTER JOIN tblCountries b ON a.CustCountryKey = b.CountryKey 
                        WHERE a.CustKey IN (SELECT DISTINCT JobCustKey FROM tblJobHeader WHERE {0}) 
                        ORDER BY a.CustName";

                where = "1=1";
                if (!string.IsNullOrEmpty(startDate))
                    where += string.Format(" AND CAST(ISNULL(JobModifiedDate, JobCreatedDate) as Date) >= '{0}'", startDate);
                if (!string.IsNullOrEmpty(endDate))
                    where += string.Format(" AND CAST(ISNULL(JobModifiedDate, JobCreatedDate) as Date) <= '{0}'", endDate);

                where = where.Replace("1=1 AND", "");

                sql = string.Format(sql, where).Replace("WHERE 1=1", "");
            }

            if ((new string[] { "rptFileQuoteStatusReport", "rptFileSummary", "rptFileSummaryByContacts" }).Contains(reportName))
            {
                sql = @"SELECT a.CustKey, a.CustName, a.CustCity, a.CustState, b.CountryName 
                        FROM tblCustomers a LEFT OUTER JOIN tblCountries b ON a.CustCountryKey = b.CountryKey 
                        WHERE a.CustKey IN (SELECT DISTINCT FileCustKey FROM tblFileHeader WHERE {0})
                        ORDER BY a.CustName";

                where = "1=1";
                if (!string.IsNullOrEmpty(startDate))
                    where += string.Format(" AND CAST(ISNULL(FileModifiedDate, FileCreatedDate) as Date) >= '{0}'", startDate);
                if (!string.IsNullOrEmpty(endDate))
                    where += string.Format(" AND CAST(ISNULL(FileModifiedDate, FileCreatedDate) as Date) <= '{0}'", endDate);

                where = where.Replace("1=1 AND", "");

                sql = string.Format(sql, where).Replace("WHERE 1=1", "");
            }

            if ((new string[] { "rptCustomerWebLogins" }).Contains(reportName))
            {
                sql = @"SELECT a.CustKey, a.CustName, a.CustCity, a.CustState, b.CountryName 
                        FROM tblCustomers a LEFT OUTER JOIN tblCountries b ON a.CustCountryKey = b.CountryKey 
                        WHERE CustKey IN (SELECT DISTINCT CustKey FROM qrptCustomerWebLogins WHERE {0}) 
                        ORDER BY a.CustName";

                where = "1=1";
                if (!string.IsNullOrEmpty(startDate))
                    where += string.Format(" AND CAST(rptDate as Date) >= '{0}'", startDate);
                if (!string.IsNullOrEmpty(endDate))
                    where += string.Format(" AND CAST(rptDate as Date) <= '{0}'", endDate);

                where = where.Replace("1=1 AND", "");

                sql = string.Format(sql, where).Replace("WHERE 1=1", "");
            }

            if ((new string[] { "rptPronacaReport", "rptPronacaReport NoProfit", "rptPronacaReportClosedShipped", "rptPronacaTransitOrders", "rptPronacaReportQuotes", "rptPronacaReportQuotes NoProfit", "rptPronacaReportCommissionOnly" }).Contains(reportName))
            {
                sql = @"SELECT a.CustKey, a.CustName, a.CustCity, a.CustState, b.CountryName 
                        FROM tblCustomers a LEFT OUTER JOIN tblCountries b ON a.CustCountryKey = b.CountryKey 
                        WHERE a.CustKey IN (SELECT DISTINCT CustKey FROM qrptPronacaReport WHERE {0}) 
                        ORDER BY a.CustName";

                where = "1=1";
                if (!string.IsNullOrEmpty(startDate))
                    where += string.Format(" AND CAST(rptDate as Date) >= '{0}'", startDate);
                if (!string.IsNullOrEmpty(endDate))
                    where += string.Format(" AND CAST(rptDate as Date) <= '{0}'", endDate);

                where = where.Replace("1=1 AND", "");

                sql = string.Format(sql, where).Replace("WHERE 1=1", "");
            }

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<CustomerForReport> data = dt.ToList<CustomerForReport>();
                totalRecords = Convert.ToInt32(dt.Rows.Count);
                return data;
            }
            else
            {
                return null;
            }
        }

        public IList<CustomerContactForReport> GetContactsByCustomer(int custkey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string sql = @"SELECT ContactKey, ISNULL(ContactFirstName,'')+' '+ISNULL(ContactLastName,'') as x_ContactFullName 
                            FROM tblCustomerContacts 
                            WHERE ContactCustKey = @CustKey 
                            ORDER BY x_ContactFullName";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@CustKey", SqlDbType.Int).Value = custkey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<CustomerContactForReport> data;

            if (dt.Rows.Count > 0)
            {
                data = dt.ToList<CustomerContactForReport>();
                return data;
            }
            
            return null;
        }
        #endregion "For Report"
    }
}