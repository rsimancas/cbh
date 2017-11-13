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
    public class ScheduleBRepository : IScheduleBRepository
    {
        #region ScheduleB
        public IList<ScheduleB> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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
                string strqry = "SchBNum + ' ' + ISNULL(SchBShortDescription,'')";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, strqry);
            }

            #region Ordenamiento
            string order = "SchBNum";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;

                if (order == "x_SchBNumFormatted") order = "SchBNum";
            }
            #endregion Ordenamiento

            string sql = @"WITH qData
                          AS 
                          (
                             SELECT *
                             FROM tblScheduleB 
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
                IList<ScheduleB> data = EnumExtension.ToList<ScheduleB>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }
            
            return null;
        }

        public ScheduleB Get(string id)
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

            ScheduleB desc;
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

        private ScheduleB Get(string id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblScheduleB " +
                         " WHERE (SchBNum = @id)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<ScheduleB> data = EnumExtension.ToList<ScheduleB>(dt);

            if (dt.Rows.Count > 0)
            {
                return data.FirstOrDefault<ScheduleB>();
            }

            return null;
        }

        public ScheduleB Add(ScheduleB dataadded)
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

            string sql = "INSERT INTO tblScheduleB ({0}) VALUES ({1}) ";

            EnumExtension.setListValues(dataadded, "", ref sql);
            
            SqlCommand cmd = new SqlCommand(sql, oConn);

            string SchBNum = dataadded.SchBNum;

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

            ScheduleB data = Get(SchBNum, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public ScheduleB Update(ScheduleB dataupdated)
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

            string sql = "UPDATE tblScheduleB SET {0} WHERE SchBNum = @id";

            dataupdated.SchBModifiedDate = DateTime.Now;

            EnumExtension.setUpdateValues(dataupdated, "SchBNum", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = dataupdated.SchBNum;

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

            ScheduleB data = Get(dataupdated.SchBNum, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(ScheduleB datadeleted)
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

        private bool Remove(ScheduleB datadeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblScheduleB WHERE (SchBNum = @id)";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = datadeleted.SchBNum;

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion ScheduleB

        #region ScheduleBLanguages
        public IList<SBLanguage> GetByParent(string SchBNum, Sort sort, int page, int start, int limit, ref int totalRecords)
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

            if (string.IsNullOrEmpty(SchBNum)) SchBNum = "00000000000000000";
            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = "a.SBLanguageSchBNum = @SchBNum";

            #region Ordenamiento
            string order = "SBLanguageKey";
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
                             SELECT a.*, b.LanguageName as x_Language
                                FROM tblScheduleBLanguages  a 
	                                INNER JOIN tblLanguages b on a.SBLanguageCode = b.LanguageCode
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
            da.SelectCommand.Parameters.Add("@SchBNum", SqlDbType.VarChar).Value = SchBNum;

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
                IList<SBLanguage> data = EnumExtension.ToList<SBLanguage>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }

            return null;
        }

        public SBLanguage Get(int id)
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

            SBLanguage desc;
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

        private SBLanguage Get(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, b.LanguageName as x_Language
                            FROM tblScheduleBLanguages  a 
	                            INNER JOIN tblLanguages b on a.SBLanguageCode = b.LanguageCode
                           WHERE (a.SBLanguageKey = @id)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<SBLanguage> data = EnumExtension.ToList<SBLanguage>(dt);

            if (dt.Rows.Count > 0)
            {
                return data.FirstOrDefault<SBLanguage>();
            }

            return null;
        }

        public SBLanguage Add(SBLanguage dataadded)
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

            string sql = @"INSERT INTO tblScheduleBLanguages ({0}) VALUES ({1}) 
                            SELECT SCOPE_IDENTITY();";

            EnumExtension.setListValues(dataadded, "SBLanguageKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int SBLanguageKey = dataadded.SBLanguageKey;

            try
            {
                SBLanguageKey = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            SBLanguage data = Get(SBLanguageKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public SBLanguage Update(SBLanguage dataupdated)
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

            string sql = "UPDATE tblScheduleBLanguages SET {0} WHERE SBLanguageKey = @id";

            EnumExtension.setUpdateValues(dataupdated, "SBLanguageKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = dataupdated.SBLanguageKey;

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

            SBLanguage data = Get(dataupdated.SBLanguageKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(SBLanguage datadeleted)
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

        private bool Remove(SBLanguage datadeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblScheduleBLanguages WHERE (SBLanguageKey = @id)";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = datadeleted.SBLanguageKey;

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion ScheduleBLanguages
    }
}