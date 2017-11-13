namespace Utilidades
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    public static class ConnManager
    {
        public static DateTime DateOnServer;

        public static SqlConnection OpenConn()
        {
            SqlConnection oConn = new SqlConnection(ConnManager.cStringConnect);

            try
            {
                oConn.Open();

                SqlCommand cmd = oConn.CreateCommand();
                cmd.CommandText = "SET IMPLICIT_TRANSACTIONS OFF\n" +
                                  "SET LOCK_TIMEOUT -1\n" +
                                  "SET ANSI_WARNINGS OFF\n";

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (SqlException ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = ConnManager.OpenConn" + "\tMESSAGE = " + ex.Message);
                throw;
            }

            return oConn;
        }

        public static bool CloseConn(SqlConnection oConn)
        {
            if (oConn != null)
            {
                if (oConn.State == ConnectionState.Open)
                {
                    try
                    {
                        oConn.Close();
                        oConn.Dispose();
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string GetPrimaryKey(string tabla, SqlConnection oConn)
        {
            SqlCommand cmd;
            string strcmd = string.Format("SELECT u.COLUMN_NAME, c.CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS c INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS u ON c.CONSTRAINT_NAME = u.CONSTRAINT_NAME where u.TABLE_NAME = '{0}' AND c.TABLE_NAME = '{0}' and c.CONSTRAINT_TYPE = 'PRIMARY KEY'", tabla);
            cmd = new SqlCommand(strcmd, oConn);

            string pkcol = "";
            try
            {
                pkcol = Convert.ToString(cmd.ExecuteScalar());
            }
            catch (SqlException ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = ConnManager.GetPrimeryKey" + "\tMESSAGE = " + ex.Message);
                return ex.Message;
            }
            return pkcol;
        }

        public static string GetRowGUID()
        {
            return Guid.NewGuid().ToString().ToUpper().Substring(2, 20);
        }

        public static DateTime GetServerDate()
        {

            SqlConnection oConn = ConnManager.OpenConn();

            SqlCommand cmd = new SqlCommand("SELECT GETDATE()", oConn);

            var dateServer = DateTime.Now;
            try
            {
                dateServer = (DateTime)cmd.ExecuteScalar();
            }
            catch (Exception)
            {
                dateServer = DateTime.Now;
            }

            ConnManager.CloseConn(oConn);

            DateOnServer = dateServer;

            return dateServer;
        }

        public static string cStringConnect
        {
            get
            {
                string sc = ConfigurationManager.ConnectionStrings["CBHConn"].ConnectionString;
                return sc;
            }
        }

        public static void ExecNonQuery(string sqlCmd, SqlConnection oConn)
        {
            SqlCommand cmd = new SqlCommand(sqlCmd, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
        }

        public static DataSet ExecQuery(string sqlCmd, SqlConnection oConn, List<SqlParameter> parameters = null)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(sqlCmd, oConn);

            if (parameters != null)
            {
                foreach (SqlParameter param in parameters)
                {
                    da.SelectCommand.Parameters.Add(param);
                }
            }
            
            try
            {
                da.Fill(ds);                
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            return ds;
        }

        public static int DLookupInt(string column, string table, string where, SqlConnection oConn)
        {
            string sqlCmd = String.Format("SELECT {0} FROM {1} WHERE {2}", column, table, where);

            SqlCommand cmd = new SqlCommand(sqlCmd, oConn);

            int returnValue = 0;
            try
            {
                returnValue = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            return returnValue;
        }

        public static Decimal DLookupDecimal(string column, string table, string where, SqlConnection oConn)
        {
            string sqlCmd = String.Format("SELECT {0} FROM {1} WHERE {2}", column, table, where);

            SqlCommand cmd = new SqlCommand(sqlCmd, oConn);

            Decimal returnValue = 0;
            try
            {
                returnValue = Convert.ToDecimal(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            return returnValue;
        }
    }
}

