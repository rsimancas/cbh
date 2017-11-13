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
    public class LanguageRepository : ILanguageRepository
    {
        public LanguageRepository()
        {
            //dbcontext = new DBContext();
        }

        public IList<Language> GetWithQuery(string query, int page, int start, int limit, ref int totalRecords)
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

            //if (!string.IsNullOrEmpty(query))
            //{
            //    string fieldName = "('('+RTRIM(ItemNum) +') '+ISNULL(dbo.fnGetItemDescription(a.ItemKey, N'en'),''))";
            //    where += (!string.IsNullOrEmpty(where) ? " and " : "") +
            //        EnumExtension.generateLikeWhere(query, fieldName);
            //}

            string sql = "SELECT * FROM ( " +
                         "SELECT *, " +
                         "	ROW_NUMBER() OVER (ORDER BY LanguageSort) as row,   " +
                         "	IsNull((SELECT count(*) FROM tblLanguages WHERE {0}),0)  as TotalRecords  " +
                         "FROM tblLanguages WHERE {0}) a " +
                         " WHERE {1}" +
                         " ORDER BY LanguageSort ";

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
                IList<Language> data = EnumExtension.ToList<Language>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public Language Get(string id)
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

            Language lang;
            try
            {
                lang = Get(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return lang;
        }

        private Language Get(string id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblLanguages " +
                         " WHERE (LanguageCode = '{0}')";

            sql = String.Format(sql, id);
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<Language> data = EnumExtension.ToList<Language>(dt);

            return data.FirstOrDefault<Language>();
        }

        public Language Add(Language Language)
        {
            //dbcontext.Configuration.ProxyCreationEnabled = false;
            //if (Language == null)
            //{
            //    throw new ArgumentNullException("item");
            //}
            //dbcontext.Language.Add(Language);
            //dbcontext.SaveChanges();
            //return Language;
            return null;
        }

        public void Remove(string id)
        {
            Language Language = Get(id);
            if (Language != null)
            {
                //dbcontext.Language.Remove(Language);
                //dbcontext.SaveChanges();
            }
        }

        public bool Update(Language Language)
        {
            if (Language == null)
            {
                throw new ArgumentNullException("Language");
            }

            Language LanguageInDB = Get(Language.LanguageCode);

            if (LanguageInDB == null)
            {
                return false;
            }

            //dbcontext.Language.Remove(LanguageInDB);
            //dbcontext.SaveChanges();

            //dbcontext.Language.Add(Language);
            //dbcontext.SaveChanges();

            return true;
        }
    }
}