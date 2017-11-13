using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Utilidades;

namespace CBHWA.Models
{
    public class CurrencyRatesRepository : ICurrencyRatesRepository
    {
        public IList<CurrencyRates> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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
                string strqry = "CurrencyCode + ' ' + CurrencyDescription";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, strqry);
            }

            #region Ordenamiento
            string order = "CurrencyCode";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

            string sql = @"WITH qData
                          AS 
                          (
                             SELECT *, CurrencyCode + ' - ' + CurrencyDescription as x_CurrencyCodeDesc
                             FROM tblCurrencyRates 
                             WHERE {0}
                          )
                          SELECT * 
                          FROM (
                            SELECT *, ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,   
                         	  IsNull((SELECT count(*) FROM qData),0)  as TotalRecords
                            FROM qData  
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
                IList<CurrencyRates> data = EnumExtension.ToList<CurrencyRates>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }
            
            return null;
        }

        public CurrencyRates Get(string id)
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

            CurrencyRates desc;
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

        private CurrencyRates Get(string id, SqlConnection oConn)
        {
            string sql = "SELECT *, CurrencyCode + ' - ' + CurrencyDescription as x_CurrencyCodeDesc FROM tblCurrencyRates " +
                         " WHERE (CurrencyCode = @id)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<CurrencyRates> data = EnumExtension.ToList<CurrencyRates>(dt);

            if (dt.Rows.Count > 0)
            {
                return data.FirstOrDefault<CurrencyRates>();
            }

            return null;
        }

        public CurrencyRates Add(CurrencyRates dataadded)
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

            string sql = "INSERT INTO tblCurrencyRates ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataadded, "", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            string currencyCode = dataadded.CurrencyCode;

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

            CurrencyRates data = Get(currencyCode, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public CurrencyRates Update(CurrencyRates dataupdated)
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

            string sql = "UPDATE tblCurrencyRates SET {0} WHERE CurrencyCode = '" + dataupdated.CurrencyCode+"'";

            EnumExtension.setUpdateValues(dataupdated, "CurrencyCode", ref sql);

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

            CurrencyRates data = Get(dataupdated.CurrencyCode, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(CurrencyRates datadeleted)
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

        private bool Remove(CurrencyRates datadeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblCurrencyRates " +
                         " WHERE (CurrencyCode = '{0}')";

            sql = String.Format(sql, datadeleted.CurrencyCode);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
    }
}