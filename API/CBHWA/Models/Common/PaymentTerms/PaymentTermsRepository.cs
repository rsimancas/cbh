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

    public class PaymentTermsRepository : IPaymentTermsRepository
    {
        #region Payment
        public IList<PaymentTerms> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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

            if (!string.IsNullOrEmpty(query))
            {
                string strqry = "STR(a.TermKey)+IsNull(c.PTDescription,'')";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, strqry);
            }

            #region Ordenamiento
            string order = "x_Description";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;

                //if (order == "x_Estatus") order = "EstatusTipo";
            }
            #endregion Ordenamiento

            string sql = @"WITH qTerms
                         AS
                         ( 
                            SELECT a.*,c.PTDescription as x_Description 
                            FROM tlkpPaymentTerms a left outer join (select PTTermKey,min(PTKey) as PTKey from tlkpPaymentTermsDescriptions group by PTTermKey) as b on a.TermKey=b.PTTermKey 
                           left outer join tlkpPaymentTermsDescriptions c on b.PTKey=c.PTKey 
                            WHERE {0} 
                         ) 
                         SELECT * FROM ( 
                          SELECT *, 
                          	ROW_NUMBER() OVER (ORDER BY {2} {3}) as row, 
                          	IsNull((SELECT count(*) FROM qTerms),0)  as TotalRecords 
                           FROM qTerms) a 
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
                IList<PaymentTerms> data = EnumExtension.ToList<PaymentTerms>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }
            
            return null;
        }

        public IList<PaymentTerms> GetListForDDL(string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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
            string where = "c.PTDescription IS NOT NULL AND c.PTDescription <> ''";

            if (!string.IsNullOrEmpty(query))
            {
                string strqry = "STR(a.TermKey)+IsNull(c.PTDescription,'')";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, strqry);
            }

            #region Ordenamiento
            string order = "x_Description";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;

                //if (order == "x_Estatus") order = "EstatusTipo";
            }
            #endregion Ordenamiento

            string sql = @"WITH qTerms
                          AS
                          ( 
                                SELECT a.*,c.PTDescription as x_Description
                                FROM tlkpPaymentTerms a 
                                inner join tlkpPaymentTermsDescriptions c on c.PTTermKey=a.TermKey
                                WHERE  {0} 
                          ) 
                          SELECT * FROM 
                          ( 
                                SELECT * 
                                    ,ROW_NUMBER() OVER (ORDER BY {2} {3}) as row
                                    FROM qTerms a 
                                        CROSS APPLY (SELECT count(*) as TotalRecords FROM qTerms) as t 
                          ) a 
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
                IList<PaymentTerms> data = EnumExtension.ToList<PaymentTerms>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }

            return null;
        }

        public PaymentTerms Get(int id)
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

            PaymentTerms desc;
            try
            {
                desc = Get(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return desc;
        }

        private PaymentTerms Get(int id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tlkpPaymentTerms " +
                         " WHERE (TermKey = {0})";

            sql = String.Format(sql, id);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<PaymentTerms> data = EnumExtension.ToList<PaymentTerms>(dt);

            if (dt.Rows.Count > 0)
            {
                return data.FirstOrDefault<PaymentTerms>();
            }

            return null;
        }

        public PaymentTerms Add(PaymentTerms dataadded)
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

            string sql = "INSERT INTO tlkpPaymentTerms ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataadded, "TermKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int termkey = dataadded.TermKey;

            try
            {
                termkey = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            PaymentTerms data = Get(termkey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public PaymentTerms Update(PaymentTerms dataupdated)
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

            string sql = "UPDATE tlkpPaymentTerms SET {0} WHERE TermKey = " + dataupdated.TermKey.ToString();

            EnumExtension.setUpdateValues(dataupdated, "TermKey", ref sql);

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

            PaymentTerms data = Get(dataupdated.TermKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(PaymentTerms datadeleted)
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
                result = Remove(datadeleted, oConn);
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

        private bool Remove(PaymentTerms datadeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tlkpPaymentTerms " +
                         " WHERE (TermKey = {0})";

            sql = String.Format(sql, datadeleted.TermKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion Payment

        #region Descriptions
        public IList<PaymentTermsDescriptions> GetDescriptionByTermKey(int termkey, ref int totalRecords)
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

            string where = String.Format("a.PTTermKey={0}", termkey);

            string sql = "SELECT * FROM ( " +
                         "SELECT a.*,b.LanguageName as x_Language, " +
                         "	ROW_NUMBER() OVER (ORDER BY a.PTKey) as row,   " +
                         "	IsNull((SELECT count(*) FROM tlkpPaymentTermsDescriptions a WHERE {0}),0)  as TotalRecords  " +
                         "FROM tlkpPaymentTermsDescriptions a " +
                         " left outer join tblLanguages b on a.PTLanguageCode=b.LanguageCode " +
                         " WHERE {0}) a " +
                         " ORDER BY PTKey";


            sql = String.Format(sql, where);

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
                IList<PaymentTermsDescriptions> data = EnumExtension.ToList<PaymentTermsDescriptions>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }

            return null;
        }

        public IList<PaymentTermsDescriptions> GetWithQueryDescription(string query, int page, int start, int limit, ref int totalRecords)
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

            if (!string.IsNullOrEmpty(query))
            {
                string strqry = "PTDescription";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, strqry);
            }

            string sql = "SELECT * FROM ( " +
                         "SELECT *, " +
                         "	ROW_NUMBER() OVER (ORDER BY PTKey) as row,   " +
                         "	IsNull((SELECT count(*) FROM tlkpPaymentTermsDescriptions a WHERE {0}),0)  as TotalRecords  " +
                         "FROM tlkpPaymentTermsDescriptions a WHERE {0}) a " +
                         " WHERE {1} " +
                         " ORDER BY PTKey";


            sql = String.Format(sql, where, wherepage);


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
                IList<PaymentTermsDescriptions> data = EnumExtension.ToList<PaymentTermsDescriptions>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }

            return null;
        }

        public PaymentTermsDescriptions GetDescription(int id)
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

            PaymentTermsDescriptions desc;
            try
            {
                desc = GetDescription(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return desc;
        }

        private PaymentTermsDescriptions GetDescription(int id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tlkpPaymentTermsDescriptions " +
                         " WHERE (PTKey = {0})";

            sql = String.Format(sql, id);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<PaymentTermsDescriptions> data = EnumExtension.ToList<PaymentTermsDescriptions>(dt);

            if (dt.Rows.Count > 0)
            {
                return data.FirstOrDefault<PaymentTermsDescriptions>();
            }

            return null;
        }

        public PaymentTermsDescriptions AddDescription(PaymentTermsDescriptions dataadded)
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

            string sql = "INSERT INTO tlkpPaymentTermsDescriptions ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataadded, "PTKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int ptkey = dataadded.PTKey;

            try
            {
                ptkey = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            PaymentTermsDescriptions data = GetDescription(ptkey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public PaymentTermsDescriptions UpdateDescription(PaymentTermsDescriptions dataupdated)
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

            string sql = "UPDATE tlkpPaymentTermsDescriptions SET {0} WHERE PTKey = " + dataupdated.PTKey.ToString();

            EnumExtension.setUpdateValues(dataupdated, "PTKey", ref sql);

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

            PaymentTermsDescriptions data = GetDescription(dataupdated.PTKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool RemoveDescription(PaymentTermsDescriptions datadeleted)
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
                result = RemoveDescription(datadeleted, oConn);
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

        private bool RemoveDescription(PaymentTermsDescriptions datadeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tlkpPaymentTermsDescriptions " +
                         " WHERE (PTKey = {0})";

            sql = String.Format(sql, datadeleted.PTKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion Descriptions
    }
}