using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Utilidades;


namespace CBHWA.Models
{
    public class StateRepository : IStateRepository
    {
        public StateRepository()
        {
            //dbcontext = new DBContext();
        }

        public IList<State> GetAll()
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

            IList<State> data;
            try
            {
                data = GetAll(oConn);
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

        private IList<State> GetAll(SqlConnection oConn)
        {
            string sql = "SELECT * " + 
                         " FROM tblStates " +
                         " ORDER BY NULLIF (StateCountryKey, 232), StateCountry, StateName";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<State> data = EnumExtension.ToList<State>(dt);

            return data;
        }

    }
}