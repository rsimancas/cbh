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
    public class FreightDestinationsRepository : IFreightDestinationsRepository
    {
        #region FreightDestinations
        public IList<FreightDestinations> GetList(string query, int page, int start, int limit, ref int totalRecords)
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
                string strqry = "STR(a.DestinationKey)+a.Destination";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, strqry);
            }

            string sql = "SELECT * FROM ( " +
                         " SELECT *, " +
                         " 	ROW_NUMBER() OVER (ORDER BY DestinationKey) as row, " +
                         " 	IsNull((SELECT count(*) FROM tlkpFreightDestinations a WHERE {0}),0)  as TotalRecords  " +
                         "  FROM tlkpFreightDestinations a WHERE {0}) a " +
                         " WHERE {1} " +
                         " ORDER BY row";


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
                IList<FreightDestinations> data = EnumExtension.ToList<FreightDestinations>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }
            
            return null;
        }

        public FreightDestinations Get(int id)
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

            FreightDestinations desc;
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

        private FreightDestinations Get(int id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tlkpFreightDestinations " +
                         " WHERE (DestinationKey = {0})";

            sql = String.Format(sql, id);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FreightDestinations> data = EnumExtension.ToList<FreightDestinations>(dt);

            if (dt.Rows.Count > 0)
            {
                return data.FirstOrDefault<FreightDestinations>();
            }

            return null;
        }

        public FreightDestinations Add(FreightDestinations dataadded)
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

            string sql = "INSERT INTO tlkpFreightDestinations ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataadded, "DestinationKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int identity = dataadded.DestinationKey;

            try
            {
                identity = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FreightDestinations data = Get(identity, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FreightDestinations Update(FreightDestinations dataupdated)
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

            string sql = "UPDATE tlkpFreightDestinations SET {0} WHERE DestinationKey = " + dataupdated.DestinationKey.ToString();

            EnumExtension.setUpdateValues(dataupdated, "DestinationKey", ref sql);

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

            FreightDestinations data = Get(dataupdated.DestinationKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(FreightDestinations datadeleted)
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

        private bool Remove(FreightDestinations datadeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tlkpFreightDestinations " +
                         " WHERE (DestinationKey = {0})";

            sql = String.Format(sql, datadeleted.DestinationKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion FreightDestinations
    }
}