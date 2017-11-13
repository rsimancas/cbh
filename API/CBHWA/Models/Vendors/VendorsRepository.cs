namespace CBHWA.Models
{
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using Utilidades;

    public class VendorsRepository : IVendorsRepository
    {
        #region "Vendor"
        public IList<Vendor> GetList(string query, Sort sort, string[] queryBy, int page, int start, int limit, ref int totalRecords, int quoteFileKey, int VendorCarrier, int QHdrKey)
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
            string where = (VendorCarrier > 0) ? "VendorCarrier = 1" : "1=1";

            where += (quoteFileKey > 0) ? String.Format(" AND VendorKey in (Select FVVendorKey from tblFileQuoteVendorInfo where FVFileKey = {0})", quoteFileKey) : "";

            where += (QHdrKey > 0) ? String.Format(" AND VendorKey in (Select FVVendorKey from tblFileQuoteVendorInfo where FVQHdrKey = {0})", QHdrKey) : "";

            #region Query By Settings
            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "";

                if (queryBy.Length == 0)
                {
                    fieldName = "VendorName+ISNULL(VendorAddress1,'')+ISNULL(VendorAddress2,'')+ISNULL(VendorCity,'')+STR(VendorKey)";
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

            // Ordenamiento
            string order = "VendorKey";
            string direction = "Desc";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            } else {
                if (queryBy.Length > 0)
                {
                    order = queryBy[0];
                    direction = "ASC";
                }
            }

            string sql = @"SELECT * FROM ( 
                         SELECT *, 
                            dbo.fnGetVendorAddress(VendorKey) as x_VendorAddress,
                           ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,  
                           IsNull((select count(*) from tblVendors WHERE {0}),0)  as TotalRecords 
                          FROM tblVendors WHERE {0}) a 
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
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                IList<Vendor> data = EnumExtension.ToList<Vendor>(dt);
                return data;
            }
            else
            {
                return null;
            }
        }

        public Vendor Get(int id)
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

            Vendor item;
            try
            {
                item = Get(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return item;
        }

        private Vendor Get(int id, SqlConnection oConn)
        {
            string sql = @"SELECT *, 
                            dbo.fnGetVendorAddress(VendorKey) as x_VendorAddress FROM tblVendors 
                         WHERE (VendorKey = {0})";

            sql = String.Format(sql, id);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<Vendor> data = EnumExtension.ToList<Vendor>(dt);
                var vendor = data.FirstOrDefault<Vendor>();
                var last = GetLastQuoteMargin(vendor.VendorKey, oConn);
                if(last != null)
                    vendor.x_LastQuoteMargin = last.FVProfitMargin;
                return vendor;
            }
            else
            {
                return null;
            }
        }

        public Vendor Add(Vendor vendor)
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

            string sql = "INSERT INTO tblVendors ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            vendor.VendorCreatedDate = DateTime.UtcNow;
            EnumExtension.setListValues(vendor, "VendorKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int identityGenerated = 0;

            try
            {
                identityGenerated = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            Vendor data = Get(identityGenerated, oConn);

            setOriginAddress(data);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public Vendor Update(Vendor item)
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

            string sql = "UPDATE tblVendors SET {0} WHERE VendorKey = " + item.VendorKey.ToString();

            EnumExtension.setUpdateValues(item, "VendorKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            Vendor data = Get(item.VendorKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(Vendor vendor, ref string msgError)
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
                result = Remove(vendor, oConn);
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

        private bool Remove(Vendor vendor, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblVendors " +
                         " WHERE (VendorKey = {0})";

            sql = String.Format(sql, vendor.VendorKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public LastQuoteMargin GetLastQuoteMargin(int vendorkey)
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

            LastQuoteMargin item;
            try
            {
                item = GetLastQuoteMargin(vendorkey, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return item;
        }

        private LastQuoteMargin GetLastQuoteMargin(int vendorkey, SqlConnection oConn)
        {
            string sql = "SELECT TOP 1 b.FVVendorKey, b.FVProfitMargin " +
                         " FROM tblFileHeader a INNER JOIN tblFileQuoteVendorInfo b ON a.FileKey = b.FVFileKey " +
                         " WHERE (b.FVVendorKey =  {0}) " +
                         " ORDER BY a.FileCreatedDate DESC";

            sql = String.Format(sql, vendorkey);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<LastQuoteMargin> data = EnumExtension.ToList<LastQuoteMargin>(dt);
                return data.FirstOrDefault<LastQuoteMargin>();
            }
            else
            {
                return null;
            }

        }

        private void setOriginAddress(VendorOriginAddress address, SqlConnection oConn)
        {
            string sql = "UPDATE tblVendorOriginAddress set OriginDefault = 0 " +
                         " WHERE OriginVendorKey = {0} and OriginKey <> {1} ";

            sql = String.Format(sql, address.OriginVendorKey, address.OriginKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());
        }

        private void setOriginAddress(Vendor vendor)
        {
            VendorOriginAddress address = new VendorOriginAddress();
            address.OriginVendorKey = vendor.VendorKey;
            address.OriginCity = vendor.VendorCity;
            address.OriginState = vendor.VendorState;
            address.OriginName = vendor.VendorName;
            address.OriginAddress1 = vendor.VendorAddress1;
            address.OriginAddress2 = vendor.VendorAddress2;
            address.OriginModifiedBy = (!string.IsNullOrWhiteSpace(vendor.VendorModifiedBy)) ? vendor.VendorModifiedBy : vendor.VendorCreatedBy;
            address.OriginModifiedDate = DateTime.UtcNow;
            address.OriginPhone = vendor.VendorPhone;
            address.OriginZip = vendor.VendorZip;
            address.OriginDefault = true;

            string msgError = "";
            Add(address, ref msgError);
        }
        #endregion "Vendor"

        #region "Contacts"

        public IList<VendorContact> GetContactsById(int id)
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

            string sql = "SELECT * FROM tblVendorContacts " +
                         " WHERE (ContactKey = @itemkey)  ";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<VendorContact> data = EnumExtension.ToList<VendorContact>(dt);

            return data;
        }

        public IList<VendorContact> GetContactsByVendor(int vendorkey, ref int totalRecords)
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

            IList<VendorContact> data;
            try
            {
                data = GetContactsByVendor(vendorkey, oConn);
                if (data != null)
                {
                    totalRecords = data.Count;
                }
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return data;
        }

        private IList<VendorContact> GetContactsByVendor(int vendorkey, SqlConnection oConn)
        {
            string sql = "SELECT a.*" +
                         " FROM tblVendorContacts a " +
                         " WHERE (a.ContactVendorKey = {0}) and (a.ContactLastName is not null or a.ContactFirstName is not null)";
            
            sql = String.Format(sql, vendorkey);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<VendorContact> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<VendorContact>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public VendorContact GetContact(int id)
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

            VendorContact data = GetContact(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private VendorContact GetContact(int id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblVendorContacts " +
                         " WHERE (ContactKey = @itemkey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<VendorContact> data = EnumExtension.ToList<VendorContact>(dt);

            return data.FirstOrDefault<VendorContact>();
        }

        public VendorContact Add(VendorContact contact, ref string msgError)
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
            string sql = "INSERT INTO tblVendorContacts ({0}) VALUES ({1}) " +
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

            VendorContact data = GetContact(keyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public VendorContact Update(VendorContact contact, ref string msgError)
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
            string sql = "UPDATE tblVendorContacts SET {0} WHERE ContactKey = " + contact.ContactKey.ToString();

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

            VendorContact data = GetContact(contact.ContactKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(VendorContact contact, ref string msgError)
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
                result = Remove(contact, oConn);
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

        private bool Remove(VendorContact contact, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblVendorContacts " +
                         " WHERE (ContactKey = {0})";

            sql = String.Format(sql, contact.ContactKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion "Contacts"

        #region "OriginAddress"
        public IList<VendorOriginAddress> GetOriginAddressAll(ref int totalRecords)
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

            string sql = @"SELECT a.*
                                ,ISNULL(b.CountryName,'') as x_CountryName
                                ,ISNULL((SELECT TOP 1 StateName FROM tblStates c WHERE a.OriginState=c.StateCode ORDER BY NULLIF (StateCountryKey, 232), StateCountry, StateName),'') as x_StateName 
                            FROM tblVendorOriginAddress  a
                            LEFT OUTER JOIN tblCountries b on a.OriginCountryKey=b.CountryKey 
                            ORDER BY a.OriginDefault DESC, a.OriginName";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];
            totalRecords = dt.Rows.Count;

            IList<VendorOriginAddress> data = dt.ToList<VendorOriginAddress>();

            return data;
        }

        public IList<VendorOriginAddress> GetOriginAddressById(int idaddress)
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

            string sql = String.Format("SELECT * FROM tblVendorOriginAddress WHERE (OriginKey = {0})", idaddress);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<VendorOriginAddress> data = EnumExtension.ToList<VendorOriginAddress>(dt);

            return data;
        }

        public IList<VendorOriginAddress> GetOriginAddressByVendor(int vendorkey, ref int totalRecords)
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

            string sql = "SELECT a.*,ISNULL(b.CountryName,'') as x_CountryName, " +
                         "ISNULL((SELECT TOP 1 StateName FROM tblStates c WHERE a.OriginState=c.StateCode ORDER BY NULLIF (StateCountryKey, 232), StateCountry, StateName),'') as x_StateName " +
                         "FROM tblVendorOriginAddress a LEFT OUTER JOIN tblCountries b on a.OriginCountryKey=b.CountryKey " +
                         "WHERE a.OriginVendorKey={0}";

            sql = String.Format(sql, vendorkey);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<VendorOriginAddress> data = EnumExtension.ToList<VendorOriginAddress>(dt);

            return data;
        }

        public VendorOriginAddress GetOriginAddress(int id)
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

            VendorOriginAddress data = GetOriginAddress(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private VendorOriginAddress GetOriginAddress(int id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblVendorOriginAddress " +
                         " WHERE (OriginKey = @itemkey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<VendorOriginAddress> data = EnumExtension.ToList<VendorOriginAddress>(dt);

            return data.FirstOrDefault<VendorOriginAddress>();
        }

        public VendorOriginAddress Add(VendorOriginAddress address, ref string msgError)
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

            string sql = "INSERT INTO tblVendorOriginAddress ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            address.OriginModifiedDate = DateTime.UtcNow;
            EnumExtension.setListValues(address, "OriginKey", ref sql);

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

            VendorOriginAddress data = GetOriginAddress(keyGenerated, oConn);

            setOriginAddress(data, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public VendorOriginAddress Update(VendorOriginAddress address, ref string msgError)
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

            string sql = "UPDATE tblVendorOriginAddress SET {0} WHERE OriginKey = " + address.OriginKey.ToString();

            EnumExtension.setUpdateValues(address, "OriginKey", ref sql);

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

            VendorOriginAddress data = GetOriginAddress(address.OriginKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(VendorOriginAddress address, ref string msgError)
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
                result = Remove(address, oConn);
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

        private bool Remove(VendorOriginAddress address, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblVendorOriginAddress " +
                         " WHERE (OriginKey = {0})";

            sql = String.Format(sql, address.OriginKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        #endregion "OriginAddress"

        #region "Warehouse"

        public IList<VendorWarehouse> GetWarehouseById(int warehouseid)
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

            string sql = String.Format("SELECT * FROM tblVendorCarrierWarehouseLocations WHERE (WarehouseKey = {0})", warehouseid);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<VendorWarehouse> data = EnumExtension.ToList<VendorWarehouse>(dt);

            return data;
        }

        public IList<VendorWarehouse> GetWarehouseByVendor(int vendorkey, ref int totalRecords)
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

            string sql = "SELECT a.*,ISNULL(b.CountryName,'') as x_CountryName, " +
                         "ISNULL((SELECT TOP 1 StateName FROM tblStates c WHERE a.WarehouseState=c.StateCode ORDER BY NULLIF (StateCountryKey, 232), StateCountry, StateName),'') as x_StateName " +
                         "FROM tblVendorCarrierWarehouseLocations a LEFT OUTER JOIN tblCountries b on a.WarehouseCountryKey=b.CountryKey " +
                         "WHERE a.WarehouseVendorKey={0}";

            sql = String.Format(sql, vendorkey);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<VendorWarehouse> data = EnumExtension.ToList<VendorWarehouse>(dt);

            return data;
        }

        public VendorWarehouse GetWarehouse(int id)
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

            VendorWarehouse data = GetWarehouse(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private VendorWarehouse GetWarehouse(int id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblVendorCarrierWarehouseLocations " +
                         " WHERE (WarehouseKey = @itemkey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<VendorWarehouse> data = EnumExtension.ToList<VendorWarehouse>(dt);

            return data.FirstOrDefault<VendorWarehouse>();
        }

        public VendorWarehouse Add(VendorWarehouse warehouse, ref string msgError)
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

            string sql = "INSERT INTO tblVendorCarrierWarehouseLocations ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(warehouse, "WarehouseKey", ref sql);

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

            VendorWarehouse data = GetWarehouse(keyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public VendorWarehouse Update(VendorWarehouse warehouse, ref string msgError)
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

            string sql = "UPDATE tblVendorCarrierWarehouseLocations SET {0} WHERE WarehouseKey = " + warehouse.WarehouseKey.ToString();

            EnumExtension.setUpdateValues(warehouse, "WarehouseKey", ref sql);

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

            VendorWarehouse data = GetWarehouse(warehouse.WarehouseKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(VendorWarehouse warehouse, ref string msgError)
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
                result = Remove(warehouse, oConn);
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

        private bool Remove(VendorWarehouse warehouse, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblVendorCarrierWarehouseLocations " +
                         " WHERE (WarehouseKey = {0})";

            sql = String.Format(sql, warehouse.WarehouseKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        #endregion "Warehouse"

        #region Warehouse Types

        public IList<WarehouseType> GetWarehouseTypesById(int key)
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

            string sql = "SELECT  a.WarehouseKey, a.WarehouseVendorKey AS CarrierKey, " +
                      " b.VendorName + N' / ' + a.WarehouseName AS CarrierWarehouse, " +
                      " a.WarehouseCity + N', ' + a.WarehouseState AS CityState, " +
                      " a.WarehouseZip AS ZipCode " +
                      " FROM    tblVendorCarrierWarehouseLocations a INNER JOIN " +
                      "   tblVendors b ON a.WarehouseVendorKey = b.VendorKey " +
                      " WHERE a.WarehouseKey = {0} ";

            sql = String.Format(sql, key);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<WarehouseType> data = EnumExtension.ToList<WarehouseType>(dt);

            return data;
        }

        public IList<WarehouseType> GetWarehouseTypesByVendor(int vendorkey, ref int totalRecords)
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

            string where = (vendorkey > 0) ? String.Format("a.WarehouseVendorKey = {0}",vendorkey) : "1=1";

            string sql = "SELECT  a.WarehouseKey, a.WarehouseVendorKey AS CarrierKey, " +
                      " b.VendorName + N' / ' + a.WarehouseName AS CarrierWarehouse, " +
                      " a.WarehouseCity + N', ' + a.WarehouseState AS CityState, " + 
                      " a.WarehouseZip AS ZipCode " +
                      " FROM    tblVendorCarrierWarehouseLocations a INNER JOIN " +
                      "   tblVendors b ON a.WarehouseVendorKey = b.VendorKey " +
                      " WHERE {0} " +
                      " ORDER BY a.WarehouseName";

            sql = String.Format(sql, where);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<WarehouseType> data = EnumExtension.ToList<WarehouseType>(dt);

            return data;
        }

        public IList<WarehouseType> GetWarehouseTypes()
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

            string sql = "SELECT  a.WarehouseKey, a.WarehouseVendorKey AS CarrierKey, " +
                      " b.VendorName + N' / ' + a.WarehouseName AS CarrierWarehouse, " +
                      " a.WarehouseCity + N', ' + a.WarehouseState AS CityState, " +
                      " a.WarehouseZip AS ZipCode " +
                      " FROM    tblVendorCarrierWarehouseLocations a INNER JOIN " +
                      "   tblVendors b ON a.WarehouseVendorKey = b.VendorKey " +
                      " ORDER BY a.WarehouseName";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                ConnManager.CloseConn(oConn);
                throw;
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<WarehouseType> data = EnumExtension.ToList<WarehouseType>(dt);

            return data;
        }

        #endregion Warehouse Types

        #region Vendors For Report
        public IList<VendorForReport> GetList()
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

            string sql = @"SELECT VendorKey,VendorName
                            FROM tblVendors
                          ORDER BY VendorName";

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
                IList<VendorForReport> data = dt.ToList<VendorForReport>();
                return data;
            }
            else
            {
                return null;
            }
        }
        #endregion Vendors For Report
    }
}