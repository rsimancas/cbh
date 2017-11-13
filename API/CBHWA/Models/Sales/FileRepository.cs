namespace CBHWA.Models
{
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using Utilidades;

    public class FileRepository : IFileRepository
    {
        static readonly IJobsRepository jobsRepo = new JobsRepository();
        public FileRepository()
        {
            // Construct code
        }

        #region File
        public IList<FileList> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = !string.IsNullOrEmpty(query) ? "" : "1=1";

            if (!string.IsNullOrEmpty(query))
            {
                string whereQuery = "";

                string fieldName = "Customer+' '+FileNum+' '+Reference+' '+CONVERT(VARCHAR(10),Date,101)+Status";
                whereQuery += EnumExtension.generateLikeWhere(query, fieldName);

                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    "(" + whereQuery + ")";
            }

            #region Ordenamiento
            string order = "FileNum";
            string direction = "DESC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

            string sql = @" WITH qList 
                           AS 
                           ( 
                           SELECT a.FileKey,a.FileModifiedBy as ModifiedBy,a.FileModifiedDate as ModifiedDate,   
                            a.FileCreatedDate AS Date,   
                            'F' + RIGHT(CAST(a.FileYear AS nvarchar), 2) + N'-' + RIGHT('0000' + CONVERT(nvarchar, a.FileNum), 4) AS [FileNum], 
                            c.CustName AS Customer, ISNULL(a.FileReference,'') AS Reference, 
                            a.FileCreatedBy as CreatedBy,a.FileCreatedDate as CreatedDate, 
                            ISNULL ((SELECT TOP 1 e.StatusText 
                                     FROM tblFileStatusHistory d  INNER JOIN
                                     tlkpStatus e ON d.FileStatusStatusKey = e.StatusKey 
                                     WHERE d.FileStatusFileKey = a.FileKey 
                                     ORDER BY d.FileStatusDate DESC), '*No Status*') AS Status, 
                            a.FileClosed, a.FileCustKey, d.CurrencySymbol as FileDefaultCurrencySymbol
                            FROM tblFileHeader a 
                                LEFT OUTER JOIN tblEmployees b ON a.FileQuoteEmployeeKey = b.EmployeeKey 
                                LEFT OUTER JOIN tblCustomers c ON a.FileCustKey = c.CustKey
                                LEFT OUTER JOIN tblCurrencyRates d ON a.FileDefaultCurrencyCode = d.CurrencyCode  
                            WHERE (a.FileStatusKey = 0) 
                           ) 
                           select * from  
                           ( 
                             select *,ROW_NUMBER() OVER (ORDER BY {2} {3}) as row, 
                               ISNULL((select count(*) from qList where {0}),0) as TotalRecords  
                              from qList where ({0}) 
                           ) a 
                           where {1} and {0} 
                           ORDER BY row";


            sql = String.Format(sql, where, wherepage, order, direction);


            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileList> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<FileList>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public IList<Quotes> GetQuotes(int filekey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "SELECT * FROM qsumFileQuoteSummary " +
                         " WHERE (QHdrFileKey = @itemkey)  ";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(filekey);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<Quotes> data = EnumExtension.ToList<Quotes>(dt);

            return data;
        }

        public IList<FileStatus> GetStatus(int filekey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "SELECT * FROM qfrmFileOverviewSubFileStatus " +
                         " WHERE (StatusFileKey = {0})  " +
                         " ORDER BY Statusdate DESC";

            sql = String.Format(sql, filekey);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileStatus> data = EnumExtension.ToList<FileStatus>(dt);

            return data;
        }

        // header
        public FileHeader Add(FileHeader fileheader)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tblFileHeader ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            fileheader.FileNum = GetNextFileNum(fileheader.FileYear, oConn);

            EnumExtension.setListValues(fileheader, "FileKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int FileKeyIndentity = 0;

            try
            {
                FileKeyIndentity = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FileHeader data = Get(FileKeyIndentity, oConn);

            insertRoles(data, oConn);

            PrintQueueAdd("rptFileStatusHistory", oConn, "FileKey = " + FileKeyIndentity.ToString());

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileHeader Update(FileHeader fileheader)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tblFileHeader SET {0} WHERE FileKey = " + fileheader.FileKey.ToString();

            EnumExtension.setUpdateValues(fileheader, "FileKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FileHeader data = Get(fileheader.FileKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileHeader Get(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            FileHeader file;
            try
            {
                file = Get(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return file;
        }

        private FileHeader Get(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*,   
                            'F' + RIGHT(CAST(a.FileYear AS nvarchar), 2) + N'-' + RIGHT('0000' + CONVERT(nvarchar, a.FileNum), 4) AS [FileNum], 
                            c.CustName AS Customer, d.CurrencySymbol as FileDefaultCurrencySymbol
                            FROM tblFileHeader a 
                                LEFT OUTER JOIN tblEmployees b ON a.FileQuoteEmployeeKey = b.EmployeeKey 
                                LEFT OUTER JOIN tblCustomers c ON a.FileCustKey = c.CustKey
                                LEFT OUTER JOIN tblCurrencyRates d ON a.FileDefaultCurrencyCode = d.CurrencyCode  
                            WHERE (a.FileKey = @FileKey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@FileKey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileHeader> data = EnumExtension.ToList<FileHeader>(dt);

            return data.FirstOrDefault<FileHeader>();
        }

        public bool Remove(FileHeader file)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(file, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(FileHeader file, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblFileHeader " +
                         " WHERE FileKey = {0}";

            sql = String.Format(sql, file.FileKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        private int GetNextFileNum(int year, SqlConnection oConn)
        {
            string sql = @"DECLARE @FileNum int = 0
                            select @FileNum = MAX(FileNum) from tblFileHeader where FileYear={0}
                          SELECT ISNULL(@FileNum,0)";
            sql = String.Format(sql, year);
            SqlCommand cmd = new SqlCommand(sql, oConn);

            int FileNum = 0;
            try
            {
                FileNum = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SqlException ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            return FileNum + 1;

        }

        private void insertRoles(FileHeader file, SqlConnection oConn)
        {
            string sql = "INSERT INTO tblFileEmployeeRoles (FileEmployeeFileKey, FileEmployeeRoleKey, FileEmployeeEmployeeKey, FileEmployeeCreatedBy) VALUES ({0}, 1, {1}, '{2}')";
            sql = String.Format(sql, file.FileKey, file.FileQuoteEmployeeKey, file.FileCreatedBy);
            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
                sql = "INSERT INTO tblFileEmployeeRoles (FileEmployeeFileKey, FileEmployeeRoleKey, FileEmployeeEmployeeKey, FileEmployeeCreatedBy) VALUES ({0}, 3, {1}, '{2}')";
                sql = String.Format(sql, file.FileKey, file.FileOrderEmployeeKey, file.FileCreatedBy);
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
        }

        public IList<FileQuoteDetail> GetQuoteDetails(int filekey, int vendorkey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            IList<FileQuoteDetail> data;
            try
            {
                data = GetQuoteDetails(filekey, vendorkey, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return data;
        }

        private IList<FileQuoteDetail> GetQuoteDetails(int filekey, int vendorkey, SqlConnection oConn)
        {
            string where = String.Format("a.QuoteFileKey = {0}", filekey);
            where += (vendorkey > 0) ? String.Format(" and a.QuoteVendorKey = {0}", vendorkey) : "";

            string sql = @"SELECT a.*,ISNULL(b.VendorName,'') as x_VendorName, dbo.fnGetItemDescription(a.QuoteItemKey, N'en') AS x_ItemName, 
                              (CASE WHEN a.QuoteItemCost=0 THEN 0 ELSE ROUND(((a.QuoteItemLinePrice - a.QuoteItemLineCost) / a.QuoteItemLinePrice) * 1000,2) / 1000 END) as x_ProfitMargin, 
                              'F' + RIGHT(CAST(c.FileYear AS nvarchar), 2) + N'-' + RIGHT('0000' + CONVERT(nvarchar, c.FileNum), 4) AS x_FileNum, 
                              d.ItemNum as x_ItemNum, (a.QuoteItemCost * a.QuoteQty) as x_LineCost, (a.QuoteItemPrice * a.QuoteQty) as x_LinePrice,
                              c.FileDefaultCurrencyCode, c.FileDefaultCurrencyRate, e.CurrencySymbol as QuoteItemCurrencySymbol, 
                              f.CurrencySymbol as FileDefaultCurrencySymbol
                          FROM qfrmFileQuoteDetailsSub a 
                            LEFT OUTER JOIN tblVendors b on a.QuoteVendorKey=b.VendorKey 
                            LEFT OUTER JOIN tblFileHeader c on a.QuoteFileKey=c.FileKey 
                            LEFT OUTER JOIN tblItems d on a.QuoteItemKey=d.ItemKey
                            LEFT OUTER JOIN tblCurrencyRates e ON a.QuoteItemCurrencyCode = e.CurrencyCode
                            LEFT OUTER JOIN tblCurrencyRates f ON c.FileDefaultCurrencyCode = f.CurrencyCode 
                          WHERE {0}";

            sql = String.Format(sql, where);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<FileQuoteDetail> data = EnumExtension.ToList<FileQuoteDetail>(dt);
                return data;
            }
            else
            {
                return null;
            }
        }

        private IList<FileQuoteDetail> GetQuoteDetails(int FileKey, SqlConnection oConn)
        {
            string where = String.Format("a.QuoteFileKey = {0}", FileKey);

            //(CASE WHEN a.QuoteItemCost=0 THEN 0 ELSE (1 - ROUND(a.QuoteItemCost / a.QuoteItemPrice,2))*100 END) as x_ProfitMargin, 
            string sql = @"SELECT a.*,ISNULL(b.VendorName,'') as x_VendorName,
                              dbo.fnGetItemDescription(a.QuoteItemKey, N'en') AS x_ItemName, 
                              (CASE WHEN a.QuoteItemCost=0 THEN 0 ELSE ROUND(((a.QuoteItemLinePrice - a.QuoteItemLineCost) / a.QuoteItemLinePrice) * 1000,2) / 1000 END) as x_ProfitMargin, 
                              'F' + RIGHT(CAST(c.FileYear AS nvarchar), 2) + N'-' + RIGHT('0000' + CONVERT(nvarchar, c.FileNum), 4) AS x_FileNum, 
                              d.ItemNum as x_ItemNum, 
                              (a.QuoteItemCost * a.QuoteQty) as x_LineCost, 
                              (a.QuoteItemPrice * a.QuoteQty) as x_LinePrice, 
                              (a.QuoteItemWeight * a.QuoteQty) as x_LineWeight, 
                              (a.QuoteItemVolume * a.QuoteQty) as x_LineVolume, 
                              c.FileDefaultCurrencyCode, c.FileDefaultCurrencyRate, 
                              e.CurrencySymbol as QuoteItemCurrencySymbol, f.CurrencySymbol as FileDefaultCurrencySymbol  
                          FROM qfrmFileQuoteDetailsSub a 
                            LEFT OUTER JOIN tblVendors b on a.QuoteVendorKey=b.VendorKey 
                            LEFT OUTER JOIN tblFileHeader c on a.QuoteFileKey=c.FileKey 
                            LEFT OUTER JOIN tblItems d on a.QuoteItemKey=d.ItemKey
                            LEFT OUTER JOIN tblCurrencyRates e ON a.QuoteItemCurrencyCode = e.CurrencyCode
                            LEFT OUTER JOIN tblCurrencyRates f ON c.FileDefaultCurrencyCode = f.CurrencyCode  
                          WHERE {0}";

            sql = String.Format(sql, where);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<FileQuoteDetail> data = EnumExtension.ToList<FileQuoteDetail>(dt);
                return data;
            }
            else
            {
                return null;
            }
        }

        public FileQuoteDetail GetQuoteDetail(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            FileQuoteDetail data;
            try
            {
                data = GetQuoteDetail(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return data;
        }

        private FileQuoteDetail GetQuoteDetail(int id, SqlConnection oConn)
        {
            string where = String.Format("a.QuoteKey = {0}", id);

            string sql = @"SELECT a.*,ISNULL(b.VendorName,'') as x_VendorName,
                              dbo.fnGetItemDescription(a.QuoteItemKey, N'en') AS x_ItemName, 
                              (CASE WHEN a.QuoteItemCost=0 THEN 0 ELSE (1 - ROUND(a.QuoteItemCost / a.QuoteItemPrice,2))*100 END) as x_ProfitMargin, 
                              'F' + RIGHT(CAST(c.FileYear AS nvarchar), 2) + N'-' + RIGHT('0000' + CONVERT(nvarchar, c.FileNum), 4) AS x_FileNum, 
                              d.ItemNum as x_ItemNum, 
                              (a.QuoteItemCost * a.QuoteQty) as x_LineCost, 
                              (a.QuoteItemPrice * a.QuoteQty) as x_LinePrice, 
                              (a.QuoteItemWeight * a.QuoteQty) as x_LineWeight, 
                              (a.QuoteItemVolume * a.QuoteQty) as x_LineVolume,
                              c.FileDefaultCurrencyCode, c.FileDefaultCurrencyRate, 
                              e.CurrencySymbol as QuoteItemCurrencySymbol, f.CurrencySymbol as FileDefaultCurrencySymbol  
                          FROM qfrmFileQuoteDetailsSub a 
                            LEFT OUTER JOIN tblVendors b on a.QuoteVendorKey=b.VendorKey 
                            LEFT OUTER JOIN tblFileHeader c on a.QuoteFileKey=c.FileKey 
                            LEFT OUTER JOIN tblItems d on a.QuoteItemKey=d.ItemKey
                            LEFT OUTER JOIN tblCurrencyRates e ON a.QuoteItemCurrencyCode = e.CurrencyCode
                            LEFT OUTER JOIN tblCurrencyRates f ON c.FileDefaultCurrencyCode = f.CurrencyCode 
                          WHERE {0}";

            sql = String.Format(sql, where);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<FileQuoteDetail> data = EnumExtension.ToList<FileQuoteDetail>(dt);
                return data.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public FileOverview GetOverview(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            FileOverview file;
            try
            {
                file = GetOverview(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return file;
        }

        private FileOverview GetOverview(int id, SqlConnection oConn)
        {
            string sql = @"SELECT *, 'F' + RIGHT(CAST(FileYear AS nvarchar), 2) + N'-' + RIGHT('0000' + CONVERT(nvarchar, FileNum), 4) AS x_FileNum
                            FROM qfrmFileOverview
                            WHERE (FileKey = @itemkey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileOverview> data = EnumExtension.ToList<FileOverview>(dt);

            return data.FirstOrDefault<FileOverview>();
        }

        public IList<FileQuoteSummary> GetQuoteSummary(int id, ref int totalRecords)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            IList<FileQuoteSummary> summary;
            try
            {
                summary = GetQuoteSummary(id, oConn, ref totalRecords);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return summary;
        }

        private IList<FileQuoteSummary> GetQuoteSummary(int filekey, SqlConnection oConn, ref int totalRecords)
        {
            string sql = "SELECT * FROM qsumFileQuoteSummary " +
                         " WHERE (QHdrFileKey = {0})";

            sql = String.Format(sql, filekey);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileQuoteSummary> data = null;

            if (dt.Rows.Count > 0)
            {
                totalRecords = dt.Rows.Count;
                data = EnumExtension.ToList<FileQuoteSummary>(dt);
            }

            return data;
        }

        public IList<FileVendorSummary> GetVendorSummary(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            IList<FileVendorSummary> summary;
            try
            {
                summary = GetVendorSummary(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return summary;
        }

        private IList<FileVendorSummary> GetVendorSummary(int id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM qsumFileVendorSummary " +
                         " WHERE (QuoteFileKey = {0})";

            sql = String.Format(sql, id);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileVendorSummary> data = null;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<FileVendorSummary>(dt);
            }

            return data;
        }

        public IList<FileEmployeeRoles> GetFileEmployeeRoles(int filekey, ref int totalRecords)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            IList<FileEmployeeRoles> summary;
            try
            {
                summary = GetFileEmployeeRoles(filekey, ref totalRecords, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return summary;
        }

        private IList<FileEmployeeRoles> GetFileEmployeeRoles(int filekey, ref int totalRecords, SqlConnection oConn)
        {
            string sql = "select a.*,IsNull(b.JobRoleDescription,'') as x_RoleName," +
                         " IsNull(CONVERT(varchar, c.EmployeeLastName) + ', ' + CONVERT(varchar, c.EmployeeFirstName),'') as x_EmployeeName " +
                         "from tblFileEmployeeRoles a  " +
                         "LEFT OUTER JOIN tlkpJobRoles b on a.FileEmployeeRoleKey=b.JobRoleKey " +
                         "LEFT OUTER JOIN tblEmployees c on a.FileEmployeeEmployeeKey=c.EmployeeKey" +
                         " " +
                         " WHERE (a.FileEmployeeFileKey = {0})";

            sql = String.Format(sql, filekey);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileEmployeeRoles> data = null;
            if (dt.Rows.Count > 0)
            {
                totalRecords = dt.Rows.Count;
                data = EnumExtension.ToList<FileEmployeeRoles>(dt);
            }

            return data;
        }

        public bool Remove(FileEmployeeRoles role)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(role, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(FileEmployeeRoles role, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblFileEmployeeRoles" +
                         " WHERE (FileEmployeeRoleKey = @FileEmployeeRoleKey AND FileEmployeeEmployeeKey = @FileEmployeeEmployeeKey)";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@FileEmployeeRoleKey", SqlDbType.Int).Value = role.FileEmployeeRoleKey;
            cmd.Parameters.Add("@FileEmployeeEmployeeKey", SqlDbType.Int).Value = role.FileEmployeeEmployeeKey;

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public FileEmployeeRoles Update(FileEmployeeRoles filerole)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            FileEmployeeRoles roleupdated;
            try
            {
                roleupdated = UpdateRole(filerole, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return roleupdated;
        }

        private FileEmployeeRoles UpdateRole(FileEmployeeRoles filerole, SqlConnection oConn)
        {
            string sql = "UPDATE tblFileEmployeeRoles SET {0} WHERE FileEmployeeKey = " + filerole.FileEmployeeKey.ToString();

            EnumExtension.setUpdateValues(filerole, "FileEmployeeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FileEmployeeRoles data = GetRole(filerole.FileEmployeeKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileEmployeeRoles GetRole(int id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblFileEmployeeRoles " +
                        " WHERE (FileEmployeeKey = @itemkey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileEmployeeRoles> data = EnumExtension.ToList<FileEmployeeRoles>(dt);

            return data.FirstOrDefault<FileEmployeeRoles>();
        }

        public FileEmployeeRoles Add(FileEmployeeRoles role)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tblFileEmployeeRoles ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(role, "FileEmployeeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int idGenerated = 0;

            try
            {
                idGenerated = Convert.ToInt32(cmd.ExecuteScalar());

            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FileEmployeeRoles data = GetRole(idGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;

        }

        public FileQuoteDetail Add(FileQuoteDetail quotedetail)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tblFileQuoteDetail ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(quotedetail, "QuoteKey", ref sql);

            SqlTransaction oTX = oConn.BeginTransaction();

            SqlCommand cmd = new SqlCommand(sql, oConn, oTX);

            int idGenerated = 0;
            FileQuoteDetail data;

            try
            {
                idGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                data = GetFileQuoteDetail(idGenerated, oConn, oTX);
                setQuoteSort(idGenerated, ref data, oConn, oTX);
                insertFileQuoteVendorInfo(data, oConn, oTX);
            }
            catch (Exception ex)
            {
                oTX.Rollback();
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            oTX.Commit();
            ConnManager.CloseConn(oConn);

            return data;

        }

        private FileQuoteDetail GetFileQuoteDetail(int id, SqlConnection oConn)
        {
            string sql = "SELECT a.*,ISNULL(b.VendorName,'') as x_VendorName," +
                          " dbo.fnGetItemDescription(a.QuoteItemKey, N'en') AS x_ItemName, " +
                          " (CASE WHEN a.QuoteItemCost=0 THEN 0 ELSE (1 - ROUND(a.QuoteItemLineCost / a.QuoteItemLinePrice,2))*100 END) as x_ProfitMargin " +
                          " FROM tblFileQuoteDetail a " +
                          " LEFT OUTER JOIN tblVendors b on a.QuoteVendorKey=b.VendorKey " +
                          " WHERE (a.QuoteKey = @itemkey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<FileQuoteDetail> data = EnumExtension.ToList<FileQuoteDetail>(dt);
                return data.FirstOrDefault<FileQuoteDetail>();
            }
            else
            {
                return null;
            }
        }

        // Select into an transaction
        private FileQuoteDetail GetFileQuoteDetail(int id, SqlConnection oConn, SqlTransaction oTX)
        {
            string sql = "SELECT a.*,ISNULL(b.VendorName,'') as x_VendorName," +
                          " dbo.fnGetItemDescription(a.QuoteItemKey, N'en') AS x_ItemName, " +
                          " (CASE WHEN a.QuoteItemCost=0 THEN 0 ELSE (1 - ROUND(a.QuoteItemLineCost / a.QuoteItemLinePrice,2))*100 END) as x_ProfitMargin " +
                          " FROM tblFileQuoteDetail a " +
                          " LEFT OUTER JOIN tblVendors b on a.QuoteVendorKey=b.VendorKey " +
                          " WHERE (a.QuoteKey = {0})";

            sql = String.Format(sql, id);
            SqlCommand cmd = new SqlCommand(sql, oConn, oTX);

            SqlDataReader dr;
            DataTable dt = new DataTable();
            dr = cmd.ExecuteReader();
            dt.Load(dr);


            //SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            //da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            //DataSet ds = new DataSet();

            //da.Fill(ds);

            //DataTable dt;
            //dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<FileQuoteDetail> data = EnumExtension.ToList<FileQuoteDetail>(dt);
                return data.FirstOrDefault<FileQuoteDetail>();
            }
            else
            {
                return null;
            }
        }

        private void setQuoteSort(int quotekey, ref FileQuoteDetail quote, SqlConnection oConn, SqlTransaction oTX)
        {
            string sql = "select count(*) from tblFileQuoteDetail where QuoteFileKey={0} and Quotekey<={1}";
            sql = String.Format(sql, quote.QuoteFileKey, quote.QuoteKey);
            SqlCommand cmd = new SqlCommand(sql, oConn, oTX);

            int quoteSort = Convert.ToInt32(cmd.ExecuteScalar());

            sql = "update tblFileQuoteDetail set QuoteSort={0} where QuoteKey={1}";
            sql = String.Format(sql, quoteSort * 100, quotekey);

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            quote.QuoteSort = quoteSort * 100;
        }

        private void insertFileQuoteVendorInfo(FileQuoteDetail quote, SqlConnection oConn, SqlTransaction oTX)
        {
            string sql = "select count(*) from tblFileQuoteVendorInfo where FVFileKey={0} and FVVendorKey={1}";
            sql = String.Format(sql, quote.QuoteFileKey, quote.QuoteVendorKey);
            SqlCommand cmd = new SqlCommand(sql, oConn, oTX);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count == 0)
            {

                sql = "insert into tblFileQuoteVendorInfo (FVFileKey,FVVendorKey,FVProfitMargin,FVDiscountCurrencyCode,FVDiscountCurrencyRate,FVPOCurrencyCode,FVPOCurrencyRate) " +
                    " values ({0},{1},{2},'{3}',{4},'{5}',{6})";

                // Reemplazamos la coma por el punto en decima
                //string profitMargin = Decimal.Round(quote.x_ProfitMargin, 2).ToString().Replace(',','.');
                var profitMargin = (quote.QuoteItemPrice > 0) ? (1 - (quote.QuoteItemCost / quote.QuoteItemPrice)) * 100 : 0;
                string profit = profitMargin.ToString().Replace(',', '.');
                string currencyRate = quote.QuoteItemCurrencyRate.ToString().Replace(',', '.');
                sql = String.Format(sql, quote.QuoteFileKey, quote.QuoteVendorKey, profit,
                   quote.QuoteItemCurrencyCode, currencyRate,
                   quote.QuoteItemCurrencyCode, currencyRate);

                cmd.CommandText = sql;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                    throw new Exception(ex.Message);
                }
            }
        }

        public FileQuoteDetail Update(FileQuoteDetail detail)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tblFileQuoteDetail SET {0} WHERE QuoteKey = " + detail.QuoteKey.ToString();

            EnumExtension.setUpdateValues(detail, "QuoteKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FileQuoteDetail data = GetFileQuoteDetail(detail.QuoteKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(FileQuoteDetail quote)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = RemoveFileQuoteDetail(quote, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool RemoveFileQuoteDetail(FileQuoteDetail quote, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblFileQuoteDetail" +
                         " WHERE (QuoteKey = {0})";

            sql = String.Format(sql, quote.QuoteKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public IList<qfrmFileQuoteDetailsSub> GetqfrmFileQuoteDetailsSub(int FileKey)
        {
            SqlConnection oConn;
            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch
            {
                throw;
            }

            string sql = @"SELECT  a.*, b.FileDefaultCurrencyCode, b.FileDefaultCurrencyRate, 
                                c.CurrencySymbol as QuoteItemCurrencySymbol, d.CurrencySymbol as FileDefaultCurrencySymbol
                            FROM qfrmFileQuoteDetailsSub a
                                LEFT OUTER JOIN tblFileHeader b ON a.QuoteFileKey = b.FileKey
                                LEFT OUTER JOIN tblCurrencyRates c ON a.QuoteItemCurrencyCode = c.CurrencyCode
                                LEFT OUTER JOIN tblCurrencyRates d ON b.FileDefaultCurrencyCode = d.CurrencyCode
                            WHERE QuoteFileKey = @FileKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@FileKey", SqlDbType.Int).Value = Convert.ToInt32(FileKey);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            ConnManager.CloseConn(oConn);

            if (dt.Rows.Count > 0)
            {
                IList<qfrmFileQuoteDetailsSub> data = EnumExtension.ToList<qfrmFileQuoteDetailsSub>(dt);
                return data;
            }
            else
            {
                return null;
            }
        }

        public FileQuoteDetail Add(FileQuoteDetail quotedetail, SqlConnection oConn, SqlTransaction oTX)
        {

            string sql = "INSERT INTO tblFileQuoteDetail ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(quotedetail, "QuoteKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn, oTX);

            int idGenerated = 0;
            FileQuoteDetail data;

            try
            {
                idGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                data = GetFileQuoteDetail(idGenerated, oConn, oTX);
                //setQuoteSort(idGenerated, ref data, oConn, oTX);
                //insertFileQuoteVendorInfo(data, oConn, oTX);
            }
            catch (Exception ex)
            {
                //oTX.Rollback();
                //ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            //oTX.Commit();
            //ConnManager.CloseConn(oConn);

            return data;

        }
        #endregion File

        #region File Status History
        public IList<FileStatusHistorySubDetails> GetFSHSubDetails(int filekey, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";

            string sql = @"WITH qStatus 
                              AS 
                             ( 
                               SELECT  a.*, b.FileClosed as x_FileClosed, c.StatusText as x_Status
                               FROM dbo.qfrmFileStatusHistorySubDetails a INNER JOIN tblFileHeader b ON a.StatusFileKey = b.FileKey
                                INNER JOIN tlkpStatus c ON a.StatusStatusKey=c.StatusKey
                               WHERE a.StatusFileKey = @FileKey
                             ) 
                             select * from  
                             ( 
                               select *, 
                               ROW_NUMBER() OVER (ORDER BY StatusDate DESC) as row, 
                               ISNULL((select count(*) from qStatus),0) as TotalRecords
                               from qStatus
                             ) a 
                             where {0} 
                             ORDER BY row";


            sql = String.Format(sql, wherepage);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@FileKey", SqlDbType.Int).Value = filekey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileStatusHistorySubDetails> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<FileStatusHistorySubDetails>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public IList<FileStatusHistory> GetFileStatusHistory(int filekey, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = String.Format("a.StatusFileKey = {0} and a.FileStatusStatusKey=b.StatusKey", filekey);

            string sql = " select * from  " +
                          " ( " +
                          "   select a.*,b.StatusText as x_Status, " +
                          "   ROW_NUMBER() OVER (ORDER BY a.FileStatusDate DESC) as row, " +
                          "   ISNULL((select count(*) from tblFileStatusHistory a, tlkpStatus b where {0}),0) as TotalRecords  " +
                          "   from tblFileStatusHistory a, tlkpStatus b where {0} " +
                          " ) a " +
                          " where {1} " +
                          " ORDER BY row";


            sql = String.Format(sql, where, wherepage);


            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileStatusHistory> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<FileStatusHistory>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public FileStatusHistory GetFileStatusHistoryById(int id, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = String.Format("a.FileStatusKey = {0} and a.FileStatusStatusKey=b.StatusKey", id);

            string sql = " select * from  " +
                          " ( " +
                          "   select a.*,b.StatusText as x_Status, " +
                          "   ROW_NUMBER() OVER (ORDER BY a.FileStatusDate DESC) as row, " +
                          "   ISNULL((select count(*) from tblFileStatusHistory a, tlkpStatus b where {0}),0) as TotalRecords  " +
                          "   from tblFileStatusHistory a, tlkpStatus b where {0} " +
                          " ) a " +
                          " where {1} " +
                          " ORDER BY row";


            sql = String.Format(sql, where, wherepage);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileStatusHistory> lista;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                lista = EnumExtension.ToList<FileStatusHistory>(dt);

            }
            else
            {
                return null;
            }

            return lista.FirstOrDefault<FileStatusHistory>();
        }

        private FileStatusHistory GetFileStatusHistoryById(int id, SqlConnection oConn)
        {
            string where = String.Format("a.FileStatusKey = {0} and a.FileStatusStatusKey=b.StatusKey", id);

            string sql = " select * from  " +
                          " ( " +
                          "   select a.*,b.StatusText as x_Status, " +
                          "   ROW_NUMBER() OVER (ORDER BY a.FileStatusDate DESC) as row, " +
                          "   ISNULL((select count(*) from tblFileStatusHistory a, tlkpStatus b where {0}),0) as TotalRecords  " +
                          "   from tblFileStatusHistory a, tlkpStatus b where {0} " +
                          " ) a " +
                          " ORDER BY row";


            sql = String.Format(sql, where);


            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileStatusHistory> lista;

            if (dt.Rows.Count > 0)
            {
                lista = EnumExtension.ToList<FileStatusHistory>(dt);

            }
            else
            {
                return null;
            }

            return lista.FirstOrDefault<FileStatusHistory>();
        }

        public FileStatusHistory Add(FileStatusHistory added)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tblFileStatusHistory ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            added.FileStatusModifiedDate = DateTime.Now;
            EnumExtension.setListValues(added, "FileStatusKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyValue = added.FileStatusKey;

            try
            {
                keyValue = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FileStatusHistory data = GetFileStatusHistoryById(keyValue, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileStatusHistory Update(FileStatusHistory updated)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tblFileStatusHistory SET {0} WHERE FileStatusKey = " + updated.FileStatusKey.ToString();

            EnumExtension.setUpdateValues(updated, "FileStatusKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FileStatusHistory data = GetFileStatusHistoryById(updated.FileStatusKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(FileStatusHistory deleted)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = RemoveHistory(deleted, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool RemoveHistory(FileStatusHistory deleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblFileStatusHistory " +
                         " WHERE (FileStatusKey = {0})";

            sql = String.Format(sql, deleted.FileStatusKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public qfrmFileStatusHistory GetqfrmFileStatusHistory(int filekey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = @"select *
                           from qfrmFileStatusHistory
                           where FileKey = @key";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = filekey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            qfrmFileStatusHistory data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<qfrmFileStatusHistory>(dt).FirstOrDefault();
            }
            else
            {
                return null;
            }

            return data;
        }
        #endregion File Status History

        #region File Quote Status History
        public IList<FileQuoteStatusHistory> GetFileQuoteStatusHistory(int FileKey, int QHdrKey, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = String.Format("a.QStatusQHdrKey = {0} and a.QStatusStatusKey=b.StatusKey", QHdrKey);

            string sql = @"SELECT * FROM
                          ( 
                             select a.*,b.StatusText as x_Status, 
                             ROW_NUMBER() OVER (ORDER BY a.QStatusDate DESC) as row, 
                             ISNULL((select count(*) from tblFileQuoteStatusHistory a, tlkpStatus b where {0}),0) as TotalRecords  
                             from tblFileQuoteStatusHistory a, tlkpStatus b where {0} 
                           ) a 
                           where {1} 
                           ORDER BY row";


            sql = String.Format(sql, where, wherepage);


            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileQuoteStatusHistory> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<FileQuoteStatusHistory>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public FileQuoteStatusHistory GetFileQuoteStatusHistoryById(int id, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = String.Format("a.QStatusKey = {0} and a.QStatusStatusKey=b.StatusKey", id);

            string sql = " select * from  " +
                          " ( " +
                          "   select a.*,b.StatusText as x_Status, " +
                          "   ROW_NUMBER() OVER (ORDER BY a.QStatusDate DESC) as row, " +
                          "   ISNULL((select count(*) from tblFileQuoteStatusHistory a, tlkpStatus b where {0}),0) as TotalRecords  " +
                          "   from tblFileQuoteStatusHistory a, tlkpStatus b where {0} " +
                          " ) a " +
                          " where {1} " +
                          " ORDER BY row";


            sql = String.Format(sql, where, wherepage);


            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileQuoteStatusHistory> lista;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                lista = EnumExtension.ToList<FileQuoteStatusHistory>(dt);

            }
            else
            {
                return null;
            }

            return lista.FirstOrDefault<FileQuoteStatusHistory>();
        }

        private FileQuoteStatusHistory GetFileQuoteStatusHistoryById(int id, SqlConnection oConn)
        {
            string where = String.Format("a.QStatusKey = {0} and a.QStatusStatusKey=b.StatusKey", id);

            string sql = " select * from  " +
                          " ( " +
                          "   select a.*,b.StatusText as x_Status, " +
                          "   ROW_NUMBER() OVER (ORDER BY a.QStatusDate DESC) as row, " +
                          "   ISNULL((select count(*) from tblFileQuoteStatusHistory a, tlkpStatus b where {0}),0) as TotalRecords  " +
                          "   from tblFileQuoteStatusHistory a, tlkpStatus b where {0} " +
                          " ) a " +
                          " ORDER BY row";


            sql = String.Format(sql, where);


            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileQuoteStatusHistory> lista;

            if (dt.Rows.Count > 0)
            {
                lista = EnumExtension.ToList<FileQuoteStatusHistory>(dt);

            }
            else
            {
                return null;
            }

            return lista.FirstOrDefault<FileQuoteStatusHistory>();
        }

        public FileQuoteStatusHistory Add(FileQuoteStatusHistory added)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tblFileQuoteStatusHistory ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            added.QStatusModifiedDate = DateTime.Now;

            EnumExtension.setListValues(added, "QStatusKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyValue = added.QStatusKey;

            try
            {
                keyValue = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FileQuoteStatusHistory data = GetFileQuoteStatusHistoryById(keyValue, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileQuoteStatusHistory Update(FileQuoteStatusHistory updated)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tblFileQuoteStatusHistory SET {0} WHERE QStatusKey = " + updated.QStatusKey.ToString();

            EnumExtension.setUpdateValues(updated, "QStatusKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FileQuoteStatusHistory data = GetFileQuoteStatusHistoryById(updated.QStatusKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(FileQuoteStatusHistory deleted)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(deleted, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(FileQuoteStatusHistory deleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblFileQuoteStatusHistory " +
                         " WHERE (QStatusKey = {0})";

            sql = String.Format(sql, deleted.QStatusKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public IList<FileQuoteStatusHistorySubDetails> GetFileQuoteStatusHistorySubDetails(int FileKey, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = "a.StatusFileKey = @Key";

            string sql = @"WITH qStatus 
                              AS 
                             ( 
                               SELECT  a.*, b.FileClosed as x_FileClosed, c.StatusText as x_Status
                               FROM dbo.qfrmFileStatusHistorySubDetails a INNER JOIN tblFileHeader b ON a.StatusFileKey = b.FileKey
                                INNER JOIN tlkpStatus c ON a.StatusStatusKey=c.StatusKey
                               WHERE {0}
                             ) 
                             select * from  
                             ( 
                               select StatusFileKey as QStatusFileKey, StatusQuoteNum as QStatusQuoteNum, StatusDate as QStatusDate, StatusStatusKey as QStatusStatusKey, 
                                    StatusMemo as QStatusMemo, StatusModifiedBy as QStatusModifiedBy, StatusModifiedDate as QStatusModifiedDate, x_FileClosed, x_Status,
                               ROW_NUMBER() OVER (ORDER BY StatusDate DESC) as row, 
                               ISNULL((select count(*) from qStatus),0) as TotalRecords
                               from qStatus
                             ) a 
                             where {1} 
                             ORDER BY row";


            sql = String.Format(sql, where, wherepage);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@Key", SqlDbType.Int).Value = FileKey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileQuoteStatusHistorySubDetails> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<FileQuoteStatusHistorySubDetails>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public qfrmFileQuoteStatusHistory GetqfrmFileQuoteStatusHistory(int QHdrKey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = @"select *
                           from qfrmFileQuoteStatusHistory
                           where QHdrKey = @key";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = QHdrKey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            qfrmFileQuoteStatusHistory data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<qfrmFileQuoteStatusHistory>(dt).FirstOrDefault();
            }
            else
            {
                return null;
            }

            return data;
        }
        #endregion File Quote Status History

        #region File Quote Vendor Info

        public IList<FileQuoteVendorInfo> GetFileQuoteVendorInfo(int filekey, int ShowOnlyWithQuotes, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = String.Format("a.FVFileKey = {0}", filekey);

            if (ShowOnlyWithQuotes == 1)
            {
                where += " AND a.FVQHdrKey IS NOT NULL ";
            }

            string sql = @" WITH qDATA
                            AS
                           ( 
                             SELECT a.*, ISNULL(dbo.fnGetQuoteNum(a.FVQHdrKey),'') as x_QuoteNum,
                                ROW_NUMBER() OVER (ORDER BY a.FVQHdrKey) as row
                             FROM tblFileQuoteVendorInfo a
                             WHERE {0} 
                           )
                           SELECT *, ISNULL((select count(*) from qData),0) as TotalRecords  
                            FROM qData
                           where {1} 
                           ORDER BY row";


            sql = String.Format(sql, where, wherepage);


            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileQuoteVendorInfo> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<FileQuoteVendorInfo>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public FileQuoteVendorInfo GetFileQuoteVendorInfoById(int filekey, int vendorkey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string where = String.Format("a.FVFileKey = {0} and a.FVVendorKey = {1}", filekey, vendorkey);

            string sql = @" WITH qDATA
                            AS
                           ( 
                             SELECT a.*, ISNULL(dbo.fnGetQuoteNum(a.FVQHdrKey),'') as x_QuoteNum,
                                ROW_NUMBER() OVER (ORDER BY a.FVQHdrKey) as row
                             FROM tblFileQuoteVendorInfo a
                             WHERE {0} 
                           )
                           SELECT *, ISNULL((select count(*) from qData),0) as TotalRecords  
                            FROM qData
                           ORDER BY row";

            //string sql = " select * from tblFileQuoteVendorInfo a " +
            //              " where {0} " +
            //              " ORDER BY row";

            sql = String.Format(sql, where);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileQuoteVendorInfo> lista;

            if (dt.Rows.Count > 0)
            {
                lista = EnumExtension.ToList<FileQuoteVendorInfo>(dt);

            }
            else
            {
                return null;
            }

            return lista.FirstOrDefault<FileQuoteVendorInfo>();
        }

        private FileQuoteVendorInfo GetFileQuoteVendorInfoById(int filekey, int vendorkey, SqlConnection oConn)
        {
            string where = String.Format("a.FVFileKey = {0} and a.FVVendorKey = {1}", filekey, vendorkey);

            string sql = " select * from  tblFileQuoteVendorInfo a " +
                         " where {0} ";


            sql = String.Format(sql, where);


            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileQuoteVendorInfo> lista;

            if (dt.Rows.Count > 0)
            {
                lista = EnumExtension.ToList<FileQuoteVendorInfo>(dt);

            }
            else
            {
                return null;
            }

            return lista.FirstOrDefault<FileQuoteVendorInfo>();
        }

        public FileQuoteVendorInfo Add(FileQuoteVendorInfo added)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tblFileQuoteVendorInfo ({0}) VALUES ({1}) ";

            EnumExtension.setListValues(added, "", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FileQuoteVendorInfo data = GetFileQuoteVendorInfoById(added.FVFileKey, added.FVVendorKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileQuoteVendorInfo Update(FileQuoteVendorInfo updated)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tblFileQuoteVendorInfo SET {0} "
                + String.Format(" WHERE FVFileKey = {0} and FVVendorKey = {1}", updated.FVFileKey, updated.FVVendorKey);

            EnumExtension.setUpdateValues(updated, "", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                int afectedRows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            FileQuoteVendorInfo data = GetFileQuoteVendorInfoById(updated.FVFileKey, updated.FVVendorKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(FileQuoteVendorInfo deleted)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(deleted, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(FileQuoteVendorInfo deleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblFileQuoteVendorInfo " +
                         " WHERE (FVFileKey = {0} and FVVendorKey = {1})";

            sql = String.Format(sql, deleted.FVFileKey, deleted.FVVendorKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion File Quote Vendor Info

        #region QuoteHeader

        public IList<FileQuoteHeader> GetListQuoteHeader(int FileKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = (FileKey == 0) ? "1=1" : string.Format("QHdrFileKey = {0}", FileKey);

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "QuoteNum + ' ' + CONVERT(VARCHAR, QHdrDate, 101) + ' ' + x_CustName";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "QuoteNum";
            string direction = "DESC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

//            string sql = @"SELECT * FROM ( 
//                            SELECT *, dbo.fnGetJobNum(a.QHdrJobKey) as JobNum, dbo.fnGetQuoteNum(a.QHdrKey) as QuoteNum, 
//                              ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,  
//                              IsNull((select count(*) from tblFileQuoteHeader WHERE {0}),0)  as TotalRecords 
//                            FROM tblFileQuoteHeader a WHERE {0}) a 
//                          WHERE {1} 
//                          ORDER BY row";

            string sql = @"WITH qData
                        AS
                        (
                            SELECT * FROM (
                            SELECT a.*, dbo.fnGetJobNum(a.QHdrJobKey) as JobNum, dbo.fnGetQuoteNum(a.QHdrKey) as QuoteNum,
                                   c.CustName as x_CustName
                            FROM tblFileQuoteHeader a INNER JOIN tblFileHeader b ON a.QHdrFileKey = b.FileKey INNER JOIN
                                tblCustomers c ON b.FileCustKey = c.CustKey
                            ) a
                            WHERE {0}
                        )
                        SELECT *
                        FROM 
                        (
                        SELECT a.*,ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,
                            b.TotalRecords
                        FROM qData a INNER JOIN (SELECT COUNT(*) AS TotalRecords FROM qData) AS b on 1=1) a
                        WHERE {1}";

            sql = String.Format(sql, where, wherepage, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@start", SqlDbType.Int).Value = Convert.ToInt32(start);
            da.SelectCommand.Parameters.Add("@limit", SqlDbType.Int).Value = Convert.ToInt32(limit);

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<FileQuoteHeader> data = EnumExtension.ToList<FileQuoteHeader>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public FileQuoteHeader GetQuoteHeader(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            FileQuoteHeader data = GetQuoteHeader(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileQuoteHeader Add(FileQuoteHeader qheader, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tblFileQuoteHeader ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(qheader, "QHdrKey", ref sql);

            SqlTransaction oTX = oConn.BeginTransaction();

            SqlCommand cmd = new SqlCommand(sql, oConn, oTX);

            int keyGenerated = 0;
            FileQuoteHeader data;

            try
            {
                keyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                data = GetQuoteHeader(keyGenerated, oConn, oTX);
                setQuoteComplement(data, oConn, oTX);
            }
            catch (Exception ex)
            {
                //foreach (ParameterInfo par in MethodBase.GetCurrentMethod().GetParameters())
                //{
                //    LogManager.Write(par.Name + " => " + par.ParameterType.ToString());
                //}

                oTX.Rollback();
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            oTX.Commit();
            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileQuoteHeader Update(FileQuoteHeader qheader, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tblFileQuoteHeader SET {0} WHERE QHdrKey = " + qheader.QHdrKey.ToString();

            EnumExtension.setUpdateValues(qheader, "QHdrKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            FileQuoteHeader data = GetQuoteHeader(qheader.QHdrKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private FileQuoteHeader GetQuoteHeader(int id, SqlConnection oConn, SqlTransaction oTX)
        {
            string sql = @"SELECT a.*, b.FileDefaultCurrencyCode as x_FileCurrencyCode, 
                          b.FileDefaultCurrencyRate as x_FileCurrencyRate, b.FileReference as x_FileReference,  
                          c.CustName as x_CustName, c.CustLanguageCode as x_CustLanguageCode,  
                          LTRIM(ISNULL(d.ContactTitle, '') + N' ' + ISNULL(d.ContactFirstName, '') + N' ') + ISNULL(d.ContactLastName, '') AS x_CustContactName,
                          ISNULL((SELECT TOP (1) dbo.tlkpStatus.StatusText FROM dbo.tblFileQuoteStatusHistory INNER JOIN dbo.tlkpStatus ON dbo.tblFileQuoteStatusHistory.QStatusStatusKey = dbo.tlkpStatus.StatusKey WHERE (dbo.tblFileQuoteStatusHistory.QStatusQHdrKey = a.QHdrKey) ORDER BY dbo.tblFileQuoteStatusHistory.QStatusDate DESC), '*No Status*') as x_Status,
                          dbo.fnGetJobNum(a.QHdrJobKey) as JobNum, dbo.fnGetQuoteNum(a.QHdrKey) as QuoteNum
                          FROM tblFileQuoteHeader a  
                          INNER JOIN tblFileHeader b ON a.QHdrFileKey = b.FileKey  
                          INNER JOIN tblCustomers c ON b.FileCustKey = c.CustKey  
                          LEFT OUTER JOIN tblCustomerContacts d ON b.FileContactKey = d.ContactKey 
                          WHERE a.QHdrKey = {0}";

            sql = String.Format(sql, id);
            SqlCommand cmd = new SqlCommand(sql, oConn, oTX);

            SqlDataReader dr;
            DataTable dt = new DataTable();
            dr = cmd.ExecuteReader();
            dt.Load(dr);

            IList<FileQuoteHeader> data;

            try
            {
                data = EnumExtension.ToList<FileQuoteHeader>(dt);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            return data.FirstOrDefault<FileQuoteHeader>();
        }

        private FileQuoteHeader GetQuoteHeader(int id, SqlConnection oConn)
        {
            //string sql = "SELECT a.*, b.FileDefaultCurrencyCode as x_FileCurrencyCode, " +
            //             " b.FileDefaultCurrencyRate as x_FileCurrencyRate, b.FileReference as x_FileReference,  " +
            //             " c.CustName as x_CustName, c.CustLanguageCode as x_CustLanguageCode,  " +
            //             " LTRIM(ISNULL(d.ContactTitle, '') + N' ' + ISNULL(d.ContactFirstName, '') + N' ')  " +
            //             " + ISNULL(d.ContactLastName, '') AS x_CustContactName " +
            //             " FROM tblFileQuoteHeader a  " +
            //             " INNER JOIN tblFileHeader b ON a.QHdrFileKey = b.FileKey  " +
            //             " INNER JOIN tblCustomers c ON b.FileCustKey = c.CustKey  " +
            //             " LEFT OUTER JOIN tblCustomerContacts d ON b.FileContactKey = d.ContactKey " +
            //             " WHERE a.QHdrKey = {0}";

            string sql = @"SELECT a.*, b.FileDefaultCurrencyCode as x_FileCurrencyCode, 
                          b.FileDefaultCurrencyRate as x_FileCurrencyRate, b.FileReference as x_FileReference,  
                          c.CustName as x_CustName, c.CustLanguageCode as x_CustLanguageCode,  
                          LTRIM(ISNULL(d.ContactTitle, '') + N' ' + ISNULL(d.ContactFirstName, '') + N' ') + ISNULL(d.ContactLastName, '') AS x_CustContactName,
                          ISNULL((SELECT TOP (1) dbo.tlkpStatus.StatusText FROM dbo.tblFileQuoteStatusHistory INNER JOIN dbo.tlkpStatus ON dbo.tblFileQuoteStatusHistory.QStatusStatusKey = dbo.tlkpStatus.StatusKey WHERE (dbo.tblFileQuoteStatusHistory.QStatusQHdrKey = a.QHdrKey) ORDER BY dbo.tblFileQuoteStatusHistory.QStatusDate DESC), '*No Status*') as x_Status,
                          dbo.fnGetJobNum(a.QHdrJobKey) as JobNum, dbo.fnGetQuoteNum(a.QHdrKey) as QuoteNum
                          FROM tblFileQuoteHeader a  
                          INNER JOIN tblFileHeader b ON a.QHdrFileKey = b.FileKey  
                          INNER JOIN tblCustomers c ON b.FileCustKey = c.CustKey  
                          LEFT OUTER JOIN tblCustomerContacts d ON b.FileContactKey = d.ContactKey 
                          WHERE a.QHdrKey = {0}";

            sql = String.Format(sql, id);
            SqlCommand cmd = new SqlCommand(sql, oConn);

            SqlDataReader dr;
            DataTable dt = new DataTable();
            dr = cmd.ExecuteReader();
            dt.Load(dr);

            IList<FileQuoteHeader> data = EnumExtension.ToList<FileQuoteHeader>(dt);

            return data.FirstOrDefault<FileQuoteHeader>();
        }

        public bool Remove(FileQuoteHeader qheader)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(qheader, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(FileQuoteHeader qheader, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblFileQuoteHeader " +
                         " WHERE (QHdrKey = {0})";

            sql = String.Format(sql, qheader.QHdrKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        private void setQuoteComplement(FileQuoteHeader qheader, SqlConnection oConn, SqlTransaction oTX)
        {
            string strYear = qheader.QHdrDate.Year.ToString();
            string strPrefix = "Q" + strYear.Substring(2) + ((char)(60 + Convert.ToInt32(strYear.Substring(0, 2))));

            string sql = "select ISNULL(MAX(QHdrNum),0) from tblFileQuoteHeader where QHdrPrefix='{0}'";
            sql = String.Format(sql, strPrefix);

            SqlCommand cmd = new SqlCommand(sql, oConn, oTX);

            int QHdrNum = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

            sql = "update tblFileQuoteHeader set QHdrNum={0},QHdrPrefix='{1}' where QHdrKey={2}";
            sql = String.Format(sql, QHdrNum, strPrefix, qheader.QHdrKey);
            cmd.CommandText = sql;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            // Assign all open vendors to this quote
            sql = "UPDATE tblFileQuoteVendorInfo SET FVQHdrKey = {0} WHERE FVQHdrKey Is Null AND FVFileKey = {1}";
            sql = String.Format(sql, qheader.QHdrKey, qheader.QHdrFileKey);
            cmd.CommandText = sql;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            //Add 'Ex Factory' option to the quote
            sql = "INSERT INTO tblFileQuoteChargesSubTotals (QSTQHdrKey, QSTSubTotalKey) VALUES ({0}, 7)";

            sql = String.Format(sql, qheader.QHdrKey);

            cmd.CommandText = sql;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
        }

        public bool Remove(Quotes quote)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(quote, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(Quotes quote, SqlConnection oConn)
        {
            string sql = @"DECLARE @mensaje nvarchar(100), @result int = 1
                            SET XACT_ABORT ON;
                            BEGIN TRY
	                            BEGIN TRANSACTION;

	                            UPDATE tblFileQuoteVendorInfo SET FVQHdrKey = Null WHERE FVQHdrKey = @QHdrKey
                                DELETE FROM tblFileQuoteHeader WHERE QHdrKey = @QHdrKey

	                            COMMIT TRANSACTION;
                            END TRY

                            BEGIN CATCH

                                IF (XACT_STATE()) = -1
                                BEGIN
                                    set @mensaje = ERROR_MESSAGE()
                                    set @result = -1
                                    ROLLBACK TRANSACTION;
                                END;

	                            IF (XACT_STATE()) = 1
                                BEGIN
                                    COMMIT TRANSACTION;   
                                END;
                            END CATCH;
                            select @result, ISNULL(@mensaje,'')";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = quote.QHdrKey;

            int number = 0;
            SqlDataReader dr;
            DataTable dt = new DataTable();
            dr = cmd.ExecuteReader();
            dt.Load(dr);

            if (dt.Rows.Count == 0) return false;

            number = Convert.ToInt32(dt.Rows[0][0]);
            var msg = (string)dt.Rows[0][1];
            
            if (number > -1) return true;

            LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + msg);

            return false;
        }
        #endregion QuoteHeader

        #region QuoteCharges
        public IList<FileQuoteCharge> GetListQuoteCharges(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = "1=1";

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and a.{0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "STR(a.QChargeKey)";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "a.QChargeKey";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

            string sql = @"SELECT * FROM ( 
                           SELECT a.*, ISNULL(b.CurrencyDescription,'') as x_ChargeCurrency, 
                                ISNULL(c.DescriptionText,'') as x_ChargeDescription, 
                                ISNULL(d.VendorName,'') as x_FreightCompany, 
                                ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,  
                                IsNull((select count(*) from tblFileQuoteCharges a WHERE {0}),0)  as TotalRecords 
                           FROM tblFileQuoteCharges a 
                               LEFT OUTER JOIN tblCurrencyRates b ON a.QChargeCurrencyCode=b.CurrencyCode 
                               LEFT OUTER JOIN 
                                   (SELECT a.ChargeKey,MIN(b.DescriptionText) as DescriptionText FROM tlkpChargeCategories a 
                                       INNER JOIN tlkpChargeCategoryDescriptions b ON a.ChargeKey = b.DescriptionChargeKey 
                                   WHERE (a.ChargeAPAccount IS NOT NULL) 
                                   GROUP BY a.ChargeKey) as c ON a.QChargeChargeKey = c.ChargeKey
                                LEFT OUTER JOIN tblVendors d ON d.VendorKey = a.QChargeFreightCompany
                           WHERE {0}) a 
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<FileQuoteCharge> data = EnumExtension.ToList<FileQuoteCharge>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public FileQuoteCharge GetQuoteCharges(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            FileQuoteCharge data = GetQuoteCharges(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileQuoteCharge Add(FileQuoteCharge qcharges, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tblFileQuoteCharges ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(qcharges, "QChargeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyGenerated = 0;
            FileQuoteCharge data;

            try
            {
                keyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                data = GetQuoteCharges(keyGenerated, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileQuoteCharge Update(FileQuoteCharge qcharges, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tblFileQuoteCharges SET {0} WHERE " +
                String.Format(" QChargeKey = " + qcharges.QChargeKey);

            EnumExtension.setUpdateValues(qcharges, "QChargeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            FileQuoteCharge data = GetQuoteCharges(qcharges.QChargeKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private FileQuoteCharge GetQuoteCharges(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, ISNULL(b.CurrencyDescription,'') as x_ChargeCurrency, 
                                ISNULL(c.DescriptionText,'') as x_ChargeDescription, 
                                ISNULL(d.VendorName,'') as x_FreightCompany
                           FROM tblFileQuoteCharges a 
                               LEFT OUTER JOIN tblCurrencyRates b ON a.QChargeCurrencyCode=b.CurrencyCode 
                               LEFT OUTER JOIN 
                                   (SELECT a.ChargeKey,MIN(b.DescriptionText) as DescriptionText FROM tlkpChargeCategories a 
                                       INNER JOIN tlkpChargeCategoryDescriptions b ON a.ChargeKey = b.DescriptionChargeKey 
                                   WHERE (a.ChargeAPAccount IS NOT NULL) 
                                   GROUP BY a.ChargeKey) as c ON a.QChargeChargeKey = c.ChargeKey
                                LEFT OUTER JOIN tblVendors d ON d.VendorKey = a.QChargeFreightCompany
                         WHERE a.QChargeKey = {0}";

            sql = String.Format(sql, id);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];
            FileQuoteCharge data = null;

            if (dt.Rows.Count > 0)
            {
                data = dt.ToList<FileQuoteCharge>().FirstOrDefault<FileQuoteCharge>();
            }

            return data;
        }

        public bool Remove(FileQuoteCharge qcharges)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(qcharges, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(FileQuoteCharge qcharges, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblFileQuoteCharges " +
                         " WHERE (QChargeKey = {0})";

            sql = String.Format(sql, qcharges.QChargeKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion QuoteCharges

        #region QuoteCharges SubTotals
        public IList<FileQuoteChargesSubTotals> GetListQuoteChargesSubTotals(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = "1=1";

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and a.{0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "STR(a.QSTSubTotalKey)";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "a.QSTSubTotalKey";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

            string sql = "SELECT * FROM ( " +
                         " SELECT a.*,d.STDescriptionText as x_Category, c.ListText as x_Location, ISNULL(b.SubTotalSort,'') as x_SubTotalSort, " +
                         "      ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,  " +
                         "       IsNull((select count(*) from tblFileQuoteChargesSubTotals a WHERE {0}),0)  as TotalRecords   " +
                         "   FROM tblFileQuoteChargesSubTotals a " +
                         "       INNER JOIN tblFileQuoteHeader h ON a.QSTQHdrKey=h.QHdrKey" +
                         "       INNER JOIN tblFileHeader f ON h.QHdrFileKey = f.FileKey" +
                         "       INNER JOIN tblCustomers cust ON cust.CustKey = f.FileCustKey" +
                         "       LEFT OUTER JOIN tlkpInvoiceSubTotalCategories b ON a.QSTSubTotalKey=b.SubTotalKey " +
                         "       LEFT OUTER JOIN tlkpGenericLists c ON a.QSTLocation=c.ListKey " +
                         "       LEFT OUTER JOIN tlkpInvoiceSubTotalCategoriesDescriptions d on a.QSTSubTotalKey=d.STDescriptionSubTotalKey and d.STDescriptionLanguageCode = cust.CustLanguageCode " +
                         "   WHERE {0}) a  " +
                         "  WHERE {1} " +
                         "  ORDER BY row";

            sql = String.Format(sql, where, wherepage, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<FileQuoteChargesSubTotals> data = EnumExtension.ToList<FileQuoteChargesSubTotals>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public FileQuoteChargesSubTotals GetQuoteChargesSubTotals(int subTotalKey, int qhrKey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            FileQuoteChargesSubTotals data = GetQuoteChargesSubTotals(subTotalKey, qhrKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileQuoteChargesSubTotals Add(FileQuoteChargesSubTotals qchargesst, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tblFileQuoteChargesSubTotals ({0}) VALUES ({1}) ";

            EnumExtension.setListValues(qchargesst, "", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyGenerated = 0;
            FileQuoteChargesSubTotals data;

            try
            {
                keyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                data = GetQuoteChargesSubTotals(qchargesst.QSTSubTotalKey, qchargesst.QSTQHdrKey, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileQuoteChargesSubTotals Update(FileQuoteChargesSubTotals qchargesst, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            FileQuoteChargesSubTotals checkData = GetQuoteChargesSubTotals(qchargesst.QSTSubTotalKey, qchargesst.QSTQHdrKey, oConn);

            if (checkData == null)
            {
                Remove(qchargesst, oConn);
                checkData = Add(qchargesst, ref msgError);
                ConnManager.CloseConn(oConn);
                return checkData;
            }

            string sql = "UPDATE tblFileQuoteChargesSubTotals SET {0} WHERE " +
                String.Format(" QSTSubTotalKey = {0} and QSTQHdrKey = {1} ", qchargesst.QSTSubTotalKey, qchargesst.QSTQHdrKey);

            EnumExtension.setUpdateValues(qchargesst, "", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            FileQuoteChargesSubTotals data = GetQuoteChargesSubTotals(qchargesst.QSTSubTotalKey, qchargesst.QSTQHdrKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private FileQuoteChargesSubTotals GetQuoteChargesSubTotals(int subTotalKey, int qhdrKey, SqlConnection oConn)
        {
            string sql = "SELECT a.* " +
                         " FROM tblFileQuoteChargesSubTotals a  " +
                         " WHERE a.QSTSubTotalKey = {0} and a.QSTQHdrKey = {1}";

            sql = String.Format(sql, subTotalKey, qhdrKey);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];
            FileQuoteChargesSubTotals data = null;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<FileQuoteChargesSubTotals>(dt).FirstOrDefault<FileQuoteChargesSubTotals>();
            }

            return data;
        }

        public bool Remove(FileQuoteChargesSubTotals qcharges)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(qcharges, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(FileQuoteChargesSubTotals qcharges, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblFileQuoteChargesSubTotals " +
                         " WHERE (QSTSubTotalKey = {0}) AND (QSTQHdrKey = {1})";

            sql = String.Format(sql, qcharges.QSTSubTotalKey, qcharges.QSTQHdrKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion QuoteCharges SubTotals

        #region ChargeCategories
        public IList<ChargeCategories> GetListChargeCategories(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = "(a.ChargeAPAccount IS NOT NULL)";

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and {0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "STR(ChargeKey)";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "b.DescriptionText";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

            string sql = "SELECT * FROM ( " +
                         "  SELECT a.*, b.DescriptionText as x_DescriptionText, b.DescriptionLanguageCode as x_DescriptionLanguageCode, " +
                         "      b.DescriptionMemo as x_DescriptionMemo, " +
                         "      ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,  " +
                         "      IsNull((select count(*) from tlkpChargeCategories a WHERE {0}),0)  as TotalRecords   " +
                         "  FROM tlkpChargeCategories a INNER JOIN " +
                         "      tlkpChargeCategoryDescriptions b ON a.ChargeKey = b.DescriptionChargeKey WHERE {0}) a  " +
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<ChargeCategories> data = EnumExtension.ToList<ChargeCategories>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public ChargeCategories GetChargeCategory(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            ChargeCategories data = GetChargeCategory(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public ChargeCategories Add(ChargeCategories qcharges, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tlkpChargeCategories ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(qcharges, "ChargeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyGenerated = 0;
            ChargeCategories data;

            try
            {
                keyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                data = GetChargeCategory(keyGenerated, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            ConnManager.CloseConn(oConn);

            return data;
        }

        public ChargeCategories Update(ChargeCategories qcharges, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tlkpChargeCategories SET {0} WHERE " +
                String.Format(" ChargeKey = " + qcharges.ChargeKey);

            EnumExtension.setUpdateValues(qcharges, "ChargeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            ChargeCategories data = GetChargeCategory(qcharges.ChargeKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private ChargeCategories GetChargeCategory(int id, SqlConnection oConn)
        {
            string sql = "SELECT a.* " +
                         " FROM tlkpChargeCategories a  " +
                         " WHERE a.ChargeKey = {0}";

            sql = String.Format(sql, id);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];
            ChargeCategories data = null;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<ChargeCategories>(dt).FirstOrDefault<ChargeCategories>();
            }

            return data;
        }

        public bool Remove(ChargeCategories qcharges)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(qcharges, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(ChargeCategories qcharges, SqlConnection oConn)
        {
            string sql = "DELETE FROM tlkpChargeCategories " +
                         " WHERE (QChargeKey = {0})";

            sql = String.Format(sql, qcharges.ChargeKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion ChargeCategories

        #region Quote Items Summary
        public IList<FileQuoteItemsSummary> GetListQuoteItemsSummary(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

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
                string fieldName = "STR(QSummaryKey)";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "QSummarySort";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;

                order = (order == "x_VendorName") ? "VendorName" : order;
            }
            #endregion Ordenamiento

            string sql = @"SELECT * FROM ( 
                             SELECT *,b.VendorName as x_VendorName, 
                               ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,  
                               IsNull((select count(*) from tblFileQuoteItemsSummary WHERE {0}),0)  as TotalRecords 
                              FROM tblFileQuoteItemsSummary a LEFT JOIN tblVendors b ON a.QSummaryVendorKey=b.VendorKey WHERE {0}) a 
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<FileQuoteItemsSummary> data = EnumExtension.ToList<FileQuoteItemsSummary>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public FileQuoteItemsSummary GetQuoteItemsSummary(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            FileQuoteItemsSummary data = GetQuoteItemsSummary(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileQuoteItemsSummary Add(FileQuoteItemsSummary qitem, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tblFileQuoteItemsSummary ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(qitem, "QSummaryKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyGenerated = 0;
            FileQuoteItemsSummary data;

            try
            {
                keyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                data = GetQuoteItemsSummary(keyGenerated, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            ConnManager.CloseConn(oConn);

            return data;
        }

        public FileQuoteItemsSummary Update(FileQuoteItemsSummary qitem, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tblFileQuoteItemsSummary SET {0} WHERE QSummaryKey = " + qitem.QSummaryKey.ToString();

            EnumExtension.setUpdateValues(qitem, "QSummaryKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            FileQuoteItemsSummary data = GetQuoteItemsSummary(qitem.QSummaryKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private FileQuoteItemsSummary GetQuoteItemsSummary(int id, SqlConnection oConn)
        {
            string sql = "SELECT a.* " +
                         " FROM tblFileQuoteItemsSummary a  " +
                         " WHERE a.QSummaryKey = {0}";

            sql = String.Format(sql, id);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<FileQuoteItemsSummary> data = EnumExtension.ToList<FileQuoteItemsSummary>(dt);

            return data.FirstOrDefault<FileQuoteItemsSummary>();
        }

        public bool Remove(FileQuoteItemsSummary qitem)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(qitem, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(FileQuoteItemsSummary qitem, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblFileQuoteItemsSummary " +
                         " WHERE (QSummaryKey = {0})";

            sql = String.Format(sql, qitem.QSummaryKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion Quote Items Summary

        #region tlkpGenericLists
        public IList<tlkpGenericLists> GettlkpGenericLists(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = "1=1";

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and a.{0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "STR(a.ListKey)";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "a.ListText";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

            string sql = "SELECT * FROM ( " +
                         "  SELECT a.*, " +
                         "      ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,  " +
                         "      IsNull((select count(*) from tlkpGenericLists a WHERE {0}),0)  as TotalRecords   " +
                         "  FROM tlkpGenericLists a " +
                         "  WHERE {0}) a  " +
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<tlkpGenericLists> data = EnumExtension.ToList<tlkpGenericLists>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public tlkpGenericLists GettlkpGenericLists(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            tlkpGenericLists data = GettlkpGenericLists(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public tlkpGenericLists Add(tlkpGenericLists genericList, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tlkpGenericLists ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(genericList, "ListKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyGenerated = 0;
            tlkpGenericLists data;

            try
            {
                keyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                data = GettlkpGenericLists(keyGenerated, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            ConnManager.CloseConn(oConn);

            return data;
        }

        public tlkpGenericLists Update(tlkpGenericLists genericList, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tlkpGenericLists SET {0} WHERE " +
                String.Format(" ListKey = " + genericList.ListKey);

            EnumExtension.setUpdateValues(genericList, "ListKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            tlkpGenericLists data = GettlkpGenericLists(genericList.ListKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private tlkpGenericLists GettlkpGenericLists(int id, SqlConnection oConn)
        {
            string sql = "SELECT a.* " +
                         " FROM tlkpGenericLists a  " +
                         " WHERE a.ListKey = {0}";

            sql = String.Format(sql, id);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];
            tlkpGenericLists data = null;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<tlkpGenericLists>(dt).FirstOrDefault<tlkpGenericLists>();
            }

            return data;
        }

        public bool Remove(tlkpGenericLists genericList)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(genericList, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(tlkpGenericLists genericList, SqlConnection oConn)
        {
            string sql = "DELETE FROM tlkpGenericLists " +
                         " WHERE (ListKey = {0})";

            sql = String.Format(sql, genericList.ListKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion tlkpGenericLists

        #region tlkpInvoiceSubTotalCategories
        public IList<tlkpInvoiceSubTotalCategories> GettlkpInvoiceSubTotalCategories(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = "1=1";

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                if (filter.property == "STDescriptionLanguageCode")
                {
                    where += String.Format(" and b.{0} = '{1}'", filter.property, filter.value);
                }
                else
                {
                    where += String.Format(" and a.{0} = '{1}'", filter.property, filter.value);
                }
            }
            #endregion Filtros

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "STR(a.SubTotalKey) + ' ' + b.STDescriptionText";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "SubTotalSort";
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
                            SELECT a.*, b.STDescriptionText
                            FROM tlkpInvoiceSubTotalCategories a
                            INNER JOIN tlkpInvoiceSubTotalCategoriesDescriptions b ON a.SubTotalKey = b.STDescriptionSubTotalKey
                            WHERE {0} 
                         ) 
                         SELECT * FROM ( 
                          SELECT *, 
                          	ROW_NUMBER() OVER (ORDER BY {2} {3}) as row, 
                          	IsNull((SELECT count(*) FROM qData),0)  as TotalRecords  
                           FROM qData) a 
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<tlkpInvoiceSubTotalCategories> data = EnumExtension.ToList<tlkpInvoiceSubTotalCategories>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public tlkpInvoiceSubTotalCategories GettlkpInvoiceSubTotalCategories(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            tlkpInvoiceSubTotalCategories data = GettlkpInvoiceSubTotalCategories(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public tlkpInvoiceSubTotalCategories Add(tlkpInvoiceSubTotalCategories model, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "INSERT INTO tlkpInvoiceSubTotalCategories ({0}) VALUES ({1}) ";

            EnumExtension.setListValues(model, "SubTotalKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyGenerated = 0;
            tlkpInvoiceSubTotalCategories data;

            try
            {
                keyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                data = GettlkpInvoiceSubTotalCategories(model.SubTotalKey, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            ConnManager.CloseConn(oConn);

            return data;
        }

        public tlkpInvoiceSubTotalCategories Update(tlkpInvoiceSubTotalCategories model, ref string msgError)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tlkpInvoiceSubTotalCategories SET {0} WHERE SubTotalKey = @id";

            EnumExtension.setUpdateValues(model, "", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.SubTotalKey;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                msgError = ex.Message;
                return null;
            }

            var data = GettlkpInvoiceSubTotalCategories(model.SubTotalKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private tlkpInvoiceSubTotalCategories GettlkpInvoiceSubTotalCategories(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, b.STDescriptionText
                            FROM tlkpInvoiceSubTotalCategories a
                            INNER JOIN tlkpInvoiceSubTotalCategoriesDescriptions b ON a.SubTotalKey = b.STDescriptionSubTotalKey
                            WHERE a.SubTotalKey = @id";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                var data = dt.ToList<tlkpInvoiceSubTotalCategories>().FirstOrDefault();
                return data;
            }
            else
            {
                return null;
            }
        }

        public bool Remove(tlkpInvoiceSubTotalCategories model)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            bool result;
            try
            {
                result = Remove(model, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(tlkpInvoiceSubTotalCategories model, SqlConnection oConn)
        {
            string sql = "DELETE FROM tlkpInvoiceSubTotalCategories WHERE SubTotalKey = @id ";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.SubTotalKey;

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion tlkpInvoiceSubTotalCategories

        public IList<LeadTime> GetListLeadTime(string query, Sort sort, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = "a.FVLeadTime is not null";

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "a.FVLeadTime";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "a.FVLeadTime";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

            string sql = "SELECT * FROM ( " +
                         "  SELECT a.FVLeadTime, " +
                         "      ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,  " +
                         "      IsNull((select count(*) from tblFileQuoteVendorInfo a WHERE {0}),0)  as TotalRecords   " +
                         "  FROM tblFileQuoteVendorInfo a " +
                         "  WHERE {0}) a  " +
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<LeadTime> data = EnumExtension.ToList<LeadTime>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        #region qfrmFileQuoteConfirmation
        public IList<qfrmFileQuoteConfirmation> GetqfrmFileQuoteConfirmations(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = "1=1";

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and a.{0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "STR(QHdrKey)";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "QHdrKey";
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
                            SELECT  *
                            FROM qfrmFileQuoteConfirmation
                            WHERE {0}
                           ) 
                           SELECT * FROM (
                                SELECT *,
                                    ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,
                                    IsNull((select count(*) from qData),0)  as TotalRecords
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qfrmFileQuoteConfirmation> data = EnumExtension.ToList<qfrmFileQuoteConfirmation>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public qfrmFileQuoteConfirmation GetqfrmFileQuoteConfirmation(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            qfrmFileQuoteConfirmation data = GetqfrmFileQuoteConfirmation(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private qfrmFileQuoteConfirmation GetqfrmFileQuoteConfirmation(int id, SqlConnection oConn)
        {
            string sql = @"SELECT  *
                            FROM qfrmFileQuoteConfirmation
                            WHERE QHdrKey = @QHdrkey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@QHdrkey", SqlDbType.Int).Value = id;

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];
            qfrmFileQuoteConfirmation data = null;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<qfrmFileQuoteConfirmation>(dt).FirstOrDefault();
            }

            return data;
        }
        #endregion qfrmFileQuoteConfirmation

        #region qfrmFileQuoteConfirmationSVInfo
        public IList<qfrmFileQuoteConfirmationSVInfo> GetqfrmFileQuoteConfirmationSVInfos(int QHdrKey, string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = String.Format("FVQHrdKey = {0}", QHdrKey);

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and a.{0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "STR(FVQHdrKey)";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "FVQHdrKey";
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
                            SELECT  *
                            FROM qfrmFileQuoteConfirmationSubVendorInfo
                            WHERE {0}
                           ) 
                           SELECT * FROM (
                                SELECT *,
                                    ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,
                                    IsNull((select count(*) from qData),0)  as TotalRecords
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qfrmFileQuoteConfirmationSVInfo> data = EnumExtension.ToList<qfrmFileQuoteConfirmationSVInfo>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public qfrmFileQuoteConfirmationSVInfo GetqfrmFileQuoteConfirmationSVInfo(int QHdrKey, int VendorKey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            qfrmFileQuoteConfirmationSVInfo data = GetqfrmFileQuoteConfirmationSVInfo(QHdrKey, VendorKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private qfrmFileQuoteConfirmationSVInfo GetqfrmFileQuoteConfirmationSVInfo(int QHdrKey, int VendorKey, SqlConnection oConn)
        {
            string sql = @"SELECT a.*,ISNULL(b.TotalVolume,0) as FVTotalWeightTag, ISNULL(b.TotalWeight ,0) as FVTotalVolumeTag
                            FROM qfrmFileQuoteConfirmationSubVendorInfo a
	                        LEFT OUTER JOIN qsumQuoteDetails b on a.FVFileKey=b.FileKey and a.FVVendorKey = b.VendorKey
                            WHERE a.FVQHdrKey = @QHdrkey AND a.FVVendorKey = @VendorKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@QHdrkey", SqlDbType.Int).Value = QHdrKey;
            da.SelectCommand.Parameters.Add("@VendorKey", SqlDbType.Int).Value = VendorKey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];
            qfrmFileQuoteConfirmationSVInfo data = null;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<qfrmFileQuoteConfirmationSVInfo>(dt).FirstOrDefault();
            }

            return data;
        }
        #endregion qfrmFileQuoteConfirmationSVInfo

        #region qryFileQuoteVendorSummaryWithDiscount
        public IList<qryFileQuoteVendorSummaryWithDiscount> GetqryFileQuoteVendorSummaryWithDiscounts(int QHdrKey, string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = String.Format("QHdrKey = {0}", QHdrKey);

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and {0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "STR(QHdrKey)";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "QHdrKey";
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
                            SELECT  *,'Vendor Total' as x_VendorTotal
                            FROM qryFileQuoteVendorSummaryWithDiscount
                            WHERE {0}
                           ) 
                           SELECT * FROM (
                                SELECT *,
                                    ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,
                                    IsNull((select count(*) from qData),0)  as TotalRecords
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qryFileQuoteVendorSummaryWithDiscount> data = EnumExtension.ToList<qryFileQuoteVendorSummaryWithDiscount>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public qryFileQuoteVendorSummaryWithDiscount GetqryFileQuoteVendorSummaryWithDiscount(int QHdrKey, int VendorKey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            qryFileQuoteVendorSummaryWithDiscount data = GetqryFileQuoteVendorSummaryWithDiscount(QHdrKey, VendorKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private qryFileQuoteVendorSummaryWithDiscount GetqryFileQuoteVendorSummaryWithDiscount(int QHdrKey, int VendorKey, SqlConnection oConn)
        {
            string sql = @"SELECT  *
                            FROM qryFileQuoteVendorSummaryWithDiscount
                            WHERE QHdrKey = @QHdrkey AND VendorKey = @VendorKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@QHdrkey", SqlDbType.Int).Value = QHdrKey;
            da.SelectCommand.Parameters.Add("@VendorKey", SqlDbType.Int).Value = VendorKey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];
            qryFileQuoteVendorSummaryWithDiscount data = null;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<qryFileQuoteVendorSummaryWithDiscount>(dt).FirstOrDefault();
            }

            return data;
        }

        public qryFileQuoteVendorSummaryWithDiscount GetqryFileQuoteVendorSummaryWithDiscountByFile(int FileKey, int VendorKey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            qryFileQuoteVendorSummaryWithDiscount data = GetqryFileQuoteVendorSummaryWithDiscountByFile(FileKey, VendorKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private qryFileQuoteVendorSummaryWithDiscount GetqryFileQuoteVendorSummaryWithDiscountByFile(int FileKey, int VendorKey, SqlConnection oConn)
        {
            string sql = @"SELECT  *
                            FROM qryFileQuoteVendorSummaryWithDiscount
                            WHERE FileKey = @Filekey AND VendorKey = @VendorKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@Filekey", SqlDbType.Int).Value = FileKey;
            da.SelectCommand.Parameters.Add("@VendorKey", SqlDbType.Int).Value = VendorKey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];
            qryFileQuoteVendorSummaryWithDiscount data = null;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<qryFileQuoteVendorSummaryWithDiscount>(dt).FirstOrDefault();
            }

            return data;
        }
        #endregion qryFileQuoteVendorSummaryWithDiscount

        #region qfrmItemPriceHistoryPurchaseOrders
        public IList<qfrmItemPriceHistoryPurchaseOrder> GetqfrmItemPriceHistoryPurchaseOrders(int ItemKey, string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = String.Format("ItemKey = {0}", ItemKey);

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and {0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "STR(Date)";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "Date";
            string direction = "DESC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

            string sql = @"WITH qData
                           AS
                           ( 
                            SELECT  POKey,ItemKey,CustKey,[PO Num] as PONum,[Cost From Supplier] as CostFromSupplier,
                                    [Price Paid By Customer] as PricePaidByCustomer, Customer, Date
                            FROM qfrmItemPriceHistoryPurchaseOrders
                            WHERE {0}
                           ) 
                           SELECT * FROM (
                                SELECT *,
                                    ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,
                                    IsNull((select count(*) from qData),0)  as TotalRecords
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qfrmItemPriceHistoryPurchaseOrder> data = EnumExtension.ToList<qfrmItemPriceHistoryPurchaseOrder>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }
        #endregion qfrmItemPriceHistoryPurchaseOrders

        #region qryFileQuoteSearch
        public IList<qryFileQuoteSearch> GetqryFileQuoteSearch(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

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
                string fieldName = "CONVERT(VARCHAR(10),QHdrDate,101)+' '+ISNULL(QHdrNum,'')+' '+ISNULL(FileNum,'') + ' ' + ISNULL(JobNum,'') + ' ' + CustName + ' ' + ISNULL(CustContact,'') + ' ' + ISNULL(FileReference,'') + ' ' + ISNULL(QHdrCustRefNum,'') + ' ' + ISNULL(QHdrCustRefNum,'') + ' ' + ISNULL(QHdrProdDescription,'')";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "QHdrDate";
            string direction = "DESC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

            string sql = @"WITH qData
                           AS
                           ( 
                            SELECT  *
                            FROM qryFileQuoteSearch
                            WHERE {0}
                           ) 
                           SELECT * FROM (
                                SELECT *,
                                    ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,
                                    IsNull((select count(*) from qData),0)  as TotalRecords
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qryFileQuoteSearch> data = EnumExtension.ToList<qryFileQuoteSearch>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }
        #endregion qryFileQuoteSearch

        #region qryFileSearch
        public IList<qryFileSearch> GetqryFileSearch(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

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
                string fieldName = "ISNULL(FileNum,'') + ' ' + ISNULL(SalesEmployee,'') + ' ' + ISNULL(OrderEmployee,'') + ' ' + ISNULL(CustName,'') + ' ' + ISNULL(CustContact,'') + ' ' + ISNULL(CustShipName,'') + ' ' + ISNULL(FileReference,'') + ' ' + ISNULL(FileCurrencyCode,'') + ' ' + ISNULL(VendorName,'') + ' ' + ISNULL(QHdrNum,'') + ' ' + ISNULL(VendorContact,'') + ' ' + ISNULL(ShipType,'') + ' ' + ISNULL(WarehouseName,'') + ' ' + ISNULL(VendorCurrencyCode,'')";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "FileKey";
            string direction = "DESC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }
            #endregion Ordenamiento

            string sql = @"WITH qData
                           AS
                           ( 
                            SELECT  *
                            FROM qryFileSearch
                            WHERE {0}
                           ) 
                           SELECT * FROM (
                                SELECT *,
                                    ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,
                                    IsNull((select count(*) from qData),0)  as TotalRecords
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
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qryFileSearch> data = EnumExtension.ToList<qryFileSearch>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }
        #endregion qryFileSearch

        public IList<qsumFileQuoteVendorSummary> GetqsumFileQuoteVendorSummary(int QHdrKey)
        {

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string where = "QHdrKey = @QHdrKey";

            string sql = @"SELECT  *
                            FROM qsumFileQuoteVendorSummary
                            WHERE {0}";

            sql = String.Format(sql, where);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("QHdrKey", SqlDbType.Int).Value = QHdrKey;

            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qsumFileQuoteVendorSummary> data = EnumExtension.ToList<qsumFileQuoteVendorSummary>(dt);
                return data;
            }
            else
            {
                return null;
            }
        }

        #region Change File Quote Currency
        public bool ChangeFileQuoteCurrency(int QHdrKey, string CurrencyCode, decimal CurrencyRate, string currentUser)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "UPDATE tblFileQuoteHeader SET QHdrCurrencyCode = @code, QHdrCurrencyRate = @rate WHERE QHdrKey = @QHdrKey";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@code", SqlDbType.VarChar).Value = CurrencyCode;
            cmd.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = QHdrKey;
            cmd.Parameters.Add("@rate", SqlDbType.Decimal).Value = CurrencyRate;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return true;
        }
        #endregion Change PO Currency

        #region Convert Quote
        public int CreateJobFromQuote(int QHdrKey, string CurrentUser, string JobNum)
        {
            FileQuoteHeader quote = GetQuoteHeader(QHdrKey);

            if (quote == null) throw new Exception("Quote Not Found");

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            JobNum = String.IsNullOrEmpty(JobNum) ? GetNewNum("tblJobHeader", "Job", DateTime.Now.Year, oConn).ToString() : JobNum;

            var FileKey = quote.QHdrFileKey;

            //'*** Verify Order Employee
            var file = Get(FileKey);
            if (file.FileOrderEmployeeKey == null)
            {
                //... Never it will be null
            }

            //'*** Verify that the carrier is selected if the warehouse is
            if (!quote.QHdrCarrierKey.HasValue && quote.QHdrWarehouseKey.HasValue) {
                quote.QHdrCarrierKey = ConnManager.DLookupInt("WarehouseVendorKey", "tblVendorCarrierWarehouseLocations", "WarehouseKey = " + quote.QHdrWarehouseKey.GetValueOrDefault().ToString(), oConn);
            }

            string errMsg = String.Empty;
            quote = Update(quote, ref errMsg);

            UpdateFileQuoteCurrencyRates(FileKey, oConn);

            string sql = @"SELECT dbo.tblFileHeader.*, dbo.tblFileQuoteHeader.* FROM dbo.tblFileHeader INNER JOIN dbo.tblFileQuoteHeader 
                            ON dbo.tblFileHeader.FileKey = dbo.tblFileQuoteHeader.QHdrFileKey WHERE dbo.tblFileQuoteHeader.QHdrKey = @QHdrKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("QHdrKey", SqlDbType.Int).Value = QHdrKey;

            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            //ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];
            string strStatusMemo = String.Empty;
            int JobKey = 0;


            if (dt.Rows.Count > 0)
            {
                var quoteHeader = EnumExtension.ToList<FileQuoteJoin>(dt).ToList().FirstOrDefault();

                var newJob = new JobHeader();

                newJob.JobQHdrKey = QHdrKey;

                //'*** Get Job Number
                newJob.JobYear = DateTime.Now.Year;
                newJob.JobNum = Convert.ToInt32(JobNum);

                newJob.JobProdDescription = quoteHeader.QHdrProdDescription;
                newJob.JobShippingDescription = quoteHeader.QHdrShippingDescription;
                newJob.JobReference = quoteHeader.FileReference;

                newJob.JobSalesEmployeeKey = quoteHeader.FileQuoteEmployeeKey;
                newJob.JobOrderEmployeeKey = quoteHeader.FileOrderEmployeeKey.Value;
                newJob.JobCustKey = quoteHeader.FileCustKey.Value;
                newJob.JobContactKey = quoteHeader.FileContactKey;
                newJob.JobCustShipKey = quoteHeader.FileCustShipKey;
                newJob.JobCustRefNum = quoteHeader.QHdrCustRefNum;
                newJob.JobDateCustRequired = quoteHeader.FileDateCustRequired;
                newJob.JobDateCustRequiredNote = quoteHeader.FileDateCustRequiredNote;
                newJob.JobCustCurrencyCode = quoteHeader.FileDefaultCurrencyCode;
                newJob.JobCustCurrencyRate = quoteHeader.FileDefaultCurrencyRate;
                newJob.JobCustPaymentTerms = quoteHeader.QHdrCustPaymentTerms;
                newJob.JobCarrierKey = quoteHeader.QHdrCarrierKey;
                newJob.JobWarehouseKey = quoteHeader.QHdrWarehouseKey;
                newJob.JobShipType = quoteHeader.QHdrShipType;
                newJob.JobInspectorKey = quoteHeader.QHdrInspectorKey;
                newJob.JobInspectionNum = quoteHeader.QHdrInspectionNum;
                newJob.JobDUINum = quoteHeader.QHdrDUINum;

                newJob.JobCreatedBy = CurrentUser;
                newJob.JobCreatedDate = DateTime.Now;

                strStatusMemo = "Quote converted to Job " + newJob.JobYear + "-" + newJob.JobNum + "." + Environment.NewLine + "POs include: ";

                newJob = jobsRepo.Add(newJob);

                JobKey = newJob.JobKey;

                //*** Copy Job Roles from file table
                CopyJobRolesFromFile(newJob.JobKey, quoteHeader.FileKey, oConn);

                //*** Update Quote header with new JobKey
                UpdateQuoteHeaderWithJob(JobKey, quoteHeader.FileKey, oConn);

                //*** Add the Ordering form and quote to the print queue TODO

                PrintQueueAdd("rptQuoteOrderingForm", oConn, "QHdrKey = " + QHdrKey.ToString(), quoteHeader.FileOrderEmployeeKey.GetValueOrDefault());

                if (quoteHeader.FileQuoteEmployeeKey != quoteHeader.FileOrderEmployeeKey)
                {
                    PrintQueueAdd("rptQuoteOrderingForm", oConn, "QHdrKey = " + QHdrKey.ToString(), quoteHeader.FileQuoteEmployeeKey);
                }

                if (quoteHeader.QHdrCarrierKey == 1481)
                {
                    PrintQueueAdd("rptQuoteCustomer", oConn, "QHdrKey = " + QHdrKey.ToString(), 15);
                    PrintQueueAdd("rptQuoteOrderingForm", oConn, "QHdrKey = " + QHdrKey.ToString(), 15);
                }

                //*** Copy Vendor info
                CopyVendorInfo(quoteHeader.QHdrKey, newJob.JobKey, quoteHeader, oConn, ref strStatusMemo, CurrentUser);

                //'*** Open charge records that have a vendor listed for carrier and does not already have a PO created
                OpenChargeVendorListedForCarrier(quoteHeader.QHdrKey, JobKey, quoteHeader, oConn, ref strStatusMemo, CurrentUser);

                //'*** Update the Job currency rates
                jobsRepo.UpdateJobCurrencyRates(JobKey, true);

                //'*** Mark Quote Closed
                ConnManager.ExecNonQuery(String.Format("UPDATE tblFileQuoteHeader SET QHdrClosed = getdate() WHERE QHdrKey = {0}", QHdrKey), oConn);

                //'*** Mark File Closed
                strStatusMemo = strStatusMemo.Substring(0, strStatusMemo.Length - 2); //Left(strStatusMemo, Len(strStatusMemo) - 2);

                ConnManager.ExecNonQuery(String.Format("INSERT INTO tblFileQuoteStatusHistory (QStatusFileKey, QStatusQHdrKey, QStatusStatusKey, QStatusMemo) VALUES ({0},{1}, 27, '{2}')", FileKey, QHdrKey, strStatusMemo), oConn);

                CloseFile(FileKey, QHdrKey, oConn, CurrentUser);

                //'*** Open e-mail notification
                //NewJobNotification CreateJobFromQuote

                //PrintQueue

            }
            else
            {
                return 0;
            }


            return JobKey;
        }

        private void PrintQueueAdd(string strReportName, SqlConnection oConn, string strWhere = null, int? lngEmployeeKey = null)
        {
            string strSQL = string.Empty;

            //If lngEmployeeKey = 0 Then lngEmployeeKey = GetCurrentUserKey

            if (!string.IsNullOrEmpty(strWhere))
            {
                strSQL = String.Format("INSERT INTO tblPrintQueue (PrintEmployeeKey, PrintRptName, PrintStrWhere) VALUES ({0}, '{1}', '{2}')", lngEmployeeKey, strReportName, strWhere);
            }
            else
            {
                strSQL = String.Format("INSERT INTO tblPrintQueue (PrintEmployeeKey, PrintRptName) VALUES ({0}, '{2}')", lngEmployeeKey, strReportName);
            }

            ConnManager.ExecNonQuery(strSQL, oConn);
        }

        private void CloseFile(int FileKey, int QHdrKey, SqlConnection oConn, string CurrentUser)
        {
            //'*** Open the file header
            FileHeader file = Get(FileKey, oConn);

            //'*** Check if file is already closed (quit if it is)
            if (file.FileClosed.HasValue) return;

            //'*** Check all quotes and verify that all are closed
            string sql = string.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add( new SqlParameter { ParameterName = "@FileKey", SqlDbType = SqlDbType.Int, Value = FileKey });
            if (QHdrKey != 0)
            {
                sql = "SELECT dbo.fnGetQuoteNum(QHdrKey) AS QuoteNum, QHdrClosed FROM tblFileQuoteHeader WHERE QHdrFileKey = @FileKey AND QHdrKey <> @QHdrKey";
                parameters.Add(new SqlParameter { ParameterName = "@QHdrKey", SqlDbType = SqlDbType.Int, Value = QHdrKey });
            }
            else
            {
                sql = "SELECT dbo.fnGetQuoteNum(QHdrKey) AS QuoteNum, QHdrClosed FROM tblFileQuoteHeader WHERE QHdrFileKey = @FileKey";
            }
            DataSet ds = ConnManager.ExecQuery(sql, oConn, parameters);

            bool blnClosed = true;

            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                if (((DateTime?)row["QHdrClosed"]).HasValue)
                {
                    blnClosed = false;
                    break;
                }
            }

            if (!blnClosed)
            {
                //If QHdrKey = 0 Then MsgBox rstQuotes!QuoteNum & " is open.  You must close all quotes in this file before the file itself can be closed.", vbExclamation
                //GoTo Exit_CloseFile
            }

            //'*** Mark the file closed
            ConnManager.ExecNonQuery(String.Format("UPDATE tblFileHeader SET FileClosed = getdate() WHERE FileKey = {0}", FileKey), oConn);

            ConnManager.ExecNonQuery(String.Format("INSERT INTO tblFileStatusHistory (FileStatusFileKey, FileStatusStatusKey, FileStatusModifiedBy) VALUES ({0}, 62, '{1}')", FileKey, CurrentUser), oConn);
            //CurrentProject.Connection.Execute "INSERT INTO tblFileStatusHistory (FileStatusFileKey, FileStatusStatusKey, FileStatusModifiedBy) VALUES " & _
            //                                                            "(" & FileKey & ", 62, '" & GetCurrentUser & "')"
        }

        private void CopyVendorInfo(int QHdrKey, int JobKey, FileQuoteJoin quoteHeader, SqlConnection oConn, ref string strStatusMemo, string currentUser)
        {
            //'*** Search for vendors in the detail that do not have a FileVendorInfo entry
            string sql = @"SELECT * FROM tblFileQuoteVendorInfo WHERE FVQHdrKey = @QHdrKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("QHdrKey", SqlDbType.Int).Value = QHdrKey;

            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            DataTable dt;
            dt = ds.Tables[0];
            var jobPO = new JobPurchaseOrder();

            if (dt.Rows.Count > 0)
            {
                var lista = EnumExtension.ToList<FileQuoteVendorInfo>(dt).ToList();
                int lngVendorKey = 0;
                int lngPOKey = 0;
                foreach (var item in lista)
                {
                    jobPO.PONum = jobsRepo.GetNewPONum(); //CLng(strInput);
                    jobPO.POJobKey = JobKey;
                    jobPO.PODate = DateTime.Now;
                    jobPO.POGoodThruDate = jobPO.PODate.AddDays(30);
                    jobPO.POCreatedDate = jobPO.PODate;
                    jobPO.POCreatedBy = currentUser;

                    lngVendorKey = item.FVVendorKey;
                    jobPO.POVendorKey = lngVendorKey;
                    jobPO.POVendorContactKey = item.FVVendorContactKey;
                    jobPO.POLeadTime = item.FVLeadTime;
                    jobPO.PODefaultProfitMargin = item.FVProfitMargin;
                    jobPO.POVendorPaymentTerms = item.FVPaymentTerms;
                    jobPO.POCurrencyCode = item.FVPOCurrencyCode;
                    jobPO.POCurrencyRate = item.FVPOCurrencyRate;
                    jobPO.POShipmentType = item.FVFreightShipmentType;
                    jobPO.POFreightDestination = item.FVFreightDestination;
                    jobPO.POFreightDestinationZip = item.FVFreightDestinationZip;
                    jobPO.POCustShipKey = quoteHeader.FileCustShipKey;
                    jobPO.POWarehouseKey = item.FVWarehouseKey;

                    jobPO = jobsRepo.Add(jobPO);

                    lngPOKey = jobPO.POKey;

                    //'*** Add PO to Print Queue
                    PrintQueueAdd("rptJobPurchaseOrder", oConn, "POKey = " + lngPOKey.ToString(), quoteHeader.FileOrderEmployeeKey);

                    if (quoteHeader.QHdrCarrierKey == 1481)
                    {
                        PrintQueueAdd("rptJobPurchaseOrder", oConn, "POKey = " + lngPOKey.ToString(), 15);
                    }

                    //*** Add PO Instructions based on whether there is a forwarder or not
                    InsertJobPOInstructions(item.FVFreightDestination, lngPOKey, oConn);

                    //'*** Copy Items
                    CopyQuoteItemsToPO(item.FVFileKey, item.FVVendorKey, JobKey, lngPOKey, oConn);

                    //*** Copy Charges
                    CopyQuoteChargesToPO(item.FVQHdrKey.GetValueOrDefault(), item.FVFileKey, item.FVVendorKey, JobKey, lngPOKey, oConn);
                }
            }
        }

        private void OpenChargeVendorListedForCarrier(int QHdrKey, int JobKey, FileQuoteJoin quoteHeader, SqlConnection oConn, ref string strStatusMemo, string currentUser)
        {
            string sql = @"SELECT TOP 100 PERCENT * FROM dbo.tblFileQuoteCharges 
                            WHERE QChargeHdrKey = @QHdrKey AND QCHargeFreightCompany IS NOT NULL AND (NOT (QChargeFreightCompany IN 
                            (SELECT POVendorKey FROM dbo.tblJobPurchaseOrders WHERE POJobKey = @JobKey)))
                            ORDER BY dbo.tblFileQuoteCharges.QChargeFreightCompany";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("QHdrKey", SqlDbType.Int).Value = QHdrKey;
            da.SelectCommand.Parameters.Add("JobKey", SqlDbType.Int).Value = JobKey;

            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                var lista = EnumExtension.ToList<FileQuoteCharge>(dt).ToList();
                int POKey = 0;
                int VendorKey = 0;
                foreach (var item in lista)
                {
                    //'*** Create new purchase order
                    if (item.QChargeFreightCompany != VendorKey)
                    {
                        var poTarget = new JobPurchaseOrder();

                        poTarget.PONum = jobsRepo.GetNewPONum();
                        poTarget.POJobKey = JobKey;
                        poTarget.PODate = DateTime.Now;
                        poTarget.POGoodThruDate = poTarget.PODate.AddDays(30);
                        poTarget.POCreatedDate = poTarget.PODate;
                        poTarget.POCreatedBy = currentUser;

                        VendorKey = item.QChargeFreightCompany.GetValueOrDefault();
                        poTarget.POVendorKey = VendorKey;
                        poTarget.PODefaultProfitMargin = 0;
                        poTarget.POVendorPaymentTerms = 67;
                        poTarget.POCurrencyCode = item.QChargeCurrencyCode;
                        poTarget.POCurrencyRate = item.QChargeCurrencyRate;
                        poTarget.POShipmentType = quoteHeader.QHdrShipType;
                        poTarget.POFreightDestination = 100;

                        poTarget.POCustShipKey = quoteHeader.FileCustShipKey;

                        poTarget = jobsRepo.Add(poTarget);

                        POKey = poTarget.POKey;
                        strStatusMemo += String.Format("{0},", poTarget.PONum);

                        //'*** Add PO to Print Queue
                        PrintQueueAdd("rptJobPurchaseOrder", oConn, "POKey = " + poTarget.POKey.ToString(), quoteHeader.FileOrderEmployeeKey);

                        if (quoteHeader.QHdrCarrierKey == 1481)
                        {
                            PrintQueueAdd("rptJobPurchaseOrder", oConn, "POKey = " + poTarget.POKey.ToString(), 15);
                        }

                        //'*** Copy the appropriate instructions
                        //'*** Delete the instruction for penalty
                        ConnManager.ExecNonQuery(String.Format("DELETE FROM tblJobPurchaseOrderInstructions WHERE POInstructionsPOKey = {0}", POKey), oConn);

                        //'*** Copy in the appropriate instructions
                        //'*** Requires inspection:
                        if (!string.IsNullOrEmpty(quoteHeader.QHdrInspectionNum))
                        {
                            ConnManager.ExecNonQuery(String.Format("INSERT INTO tblJobPurchaseOrderInstructions (POInstructionsPOKey, POInstructionsStep, POInstructionsInstructionKey) VALUES ({0}, 1000, 10)", POKey), oConn);
                            ConnManager.ExecNonQuery(String.Format("INSERT INTO tblJobPurchaseOrderInstructions (POInstructionsPOKey, POInstructionsStep, POInstructionsInstructionKey) VALUES ({0}, 3000, 14)", POKey), oConn);
                            ConnManager.ExecNonQuery(String.Format("INSERT INTO tblJobPurchaseOrderInstructions (POInstructionsPOKey, POInstructionsStep, POInstructionsInstructionKey) VALUES ({0}, 4100, 15)", POKey), oConn);
                        }
                        else
                        {
                            ConnManager.ExecNonQuery(String.Format("INSERT INTO tblJobPurchaseOrderInstructions (POInstructionsPOKey, POInstructionsStep, POInstructionsInstructionKey) VALUES ({0}, 4000, 16)", POKey), oConn);
                        }
                    }

                    //'*** Copy Charges
                    var poChargeTarget = new JobPurchaseOrderCharge();
                    poChargeTarget.POChargesJobKey = JobKey;
                    poChargeTarget.POChargesPOKey = POKey;
                    poChargeTarget.POChargesSort = item.QChargeSort;
                    poChargeTarget.POChargesChargeKey = item.QChargeChargeKey.GetValueOrDefault();
                    poChargeTarget.POChargesMemo = item.QChargeMemo;
                    poChargeTarget.POChargesQty = item.QChargeQty;
                    poChargeTarget.POChargesCost = item.QChargeCost;
                    poChargeTarget.POChargesPrice = item.QChargePrice;
                    poChargeTarget.POChargesCurrencyCode = item.QChargeCurrencyCode;
                    poChargeTarget.POChargesCurrencyRate = item.QChargeCurrencyRate;

                    poChargeTarget = jobsRepo.Add(poChargeTarget);
                }
            }
        }

        private void CopyQuoteChargesToPO(int QHdrKey, int FileKey, int VendorKey, int JobKey, int POKey, SqlConnection oConn)
        {
            //'*** Search for vendors in the detail that do not have a FileVendorInfo entry
            string sql = @"SELECT * 
                            FROM tblFileQuoteCharges 
                            WHERE QChargeHdrKey = @QHdrKey AND QChargeFreightCompany = @VendorKey  
                            ORDER BY QChargeSort";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("QHdrKey", SqlDbType.Int).Value = QHdrKey;
            da.SelectCommand.Parameters.Add("VendorKey", SqlDbType.Int).Value = VendorKey;

            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                var lista = EnumExtension.ToList<FileQuoteCharge>(dt).ToList();
                foreach (var item in lista)
                {
                    var jobPOCharge = new JobPurchaseOrderCharge();

                    jobPOCharge.POChargesJobKey = JobKey;
                    jobPOCharge.POChargesPOKey = POKey;

                    jobPOCharge.POChargesSort = item.QChargeSort;

                    if (item.QChargeChargeKey == null && item.QChargeQCDKey != null)
                    {
                        jobPOCharge.POChargesChargeKey = QCDtoChargeKey(item.QChargeQCDKey.GetValueOrDefault(), oConn);
                    }
                    else if (item.QChargeChargeKey != null)
                    {
                        jobPOCharge.POChargesChargeKey = item.QChargeChargeKey.GetValueOrDefault();
                    }

                    jobPOCharge.POChargesMemo = item.QChargeMemo;
                    jobPOCharge.POChargesQty = item.QChargeQty;
                    jobPOCharge.POChargesCost = item.QChargeCost;
                    jobPOCharge.POChargesPrice = item.QChargePrice;
                    jobPOCharge.POChargesCurrencyCode = item.QChargeCurrencyCode;
                    jobPOCharge.POChargesCurrencyRate = item.QChargeCurrencyRate;

                    if (jobPOCharge.POChargesChargeKey != 0)
                    {
                        jobPOCharge = jobsRepo.Add(jobPOCharge);
                    }
                }

                sql = @"UPDATE tblFileQuoteDetail SET QuotePOItemsKey = 
                                (SELECT POItemsKey 
                                    FROM tblJobPurchaseOrderItems 
                                WHERE POItemsQuoteItemKey = QuoteKey AND POItemsPOKey = @POKey) 
                            WHERE QuoteFileKey = @FileKey AND QuoteVendorKey = @VendorKey";

                SqlCommand cmd = new SqlCommand(sql, oConn);
                cmd.Parameters.Add("@POKey", SqlDbType.Int).Value = POKey;
                cmd.Parameters.Add("@VendorKey", SqlDbType.Int).Value = VendorKey;
                cmd.Parameters.Add("@FileKey", SqlDbType.Int).Value = FileKey;

                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
        }

        private int QCDtoChargeKey(int QCDKey, SqlConnection oConn)
        {

            string sql = @"DECLARE @PeachtreeID VARCHAR(MAX), @QCDGLAccount int;
                            SELECT TOP 1 @PeachtreeID = QCDPeachtreeID, @QCDGLAccount = ISNULL(QCDGLAccount,0) FROM tblFileQuoteChargeDescriptions a 
                            WHERE QCDKey = @QCDKey;
                            
                            SELECT ChargeKey FROM tlkpChargeCategories WHERE ChargePeachtreeID = @PeachtreeID AND ChargeGLAccount = @QCDGLAccount";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("QCDKey", SqlDbType.Int).Value = QCDKey;
            int ChargeKey = 0;
            try
            {
                ChargeKey = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            return ChargeKey;
        }

        private void CopyQuoteItemsToPO(int FileKey, int VendorKey, int JobKey, int POKey, SqlConnection oConn)
        {
            //'*** Search for vendors in the detail that do not have a FileVendorInfo entry
            string sql = @"SELECT * FROM tblFileQuoteDetail WHERE QuoteFileKey = @FileKey AND QuoteVendorKey = @VendorKey ORDER BY QuoteSort";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("FileKey", SqlDbType.Int).Value = FileKey;
            da.SelectCommand.Parameters.Add("VendorKey", SqlDbType.Int).Value = VendorKey;

            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                var lista = EnumExtension.ToList<FileQuoteDetail>(dt).ToList();
                foreach (var item in lista)
                {
                    var poItem = new JobPurchaseOrderItem();

                    poItem.POItemsQuoteItemKey = item.QuoteKey;

                    poItem.POItemsJobKey = JobKey;
                    poItem.POItemsPOKey = POKey;

                    poItem.POItemsSort = item.QuoteSort;
                    poItem.POItemsQty = item.QuoteQty;
                    poItem.POItemsItemKey = item.QuoteItemKey;
                    poItem.POItemsLineCost = item.QuoteItemLineCost;
                    poItem.POItemsCost = poItem.POItemsLineCost / poItem.POItemsQty;
                    poItem.POItemsLinePrice = item.QuoteItemLinePrice;
                    poItem.POItemsPrice = poItem.POItemsLinePrice / poItem.POItemsQty;

                    poItem.POItemsCurrencyCode = item.QuoteItemCurrencyCode;
                    poItem.POItemsCurrencyRate = item.QuoteItemCurrencyRate;

                    poItem.POItemsWeight = item.QuoteItemWeight.GetValueOrDefault();
                    poItem.POItemsLineWeight = poItem.POItemsWeight * poItem.POItemsQty;
                    poItem.POItemsVolume = item.QuoteItemVolume.GetValueOrDefault();
                    poItem.POItemsLineVolume = poItem.POItemsVolume * poItem.POItemsQty;

                    poItem.POItemsMemoCustomer = item.QuoteItemMemoCustomer;
                    poItem.POItemsMemoCustomerMoveBottom = item.QuoteItemMemoCustomerMoveBottom;
                    poItem.POItemsMemoVendor = item.QuoteItemMemoSupplier;
                    poItem.POItemsMemoVendorMoveBottom = item.QuoteItemMemoSupplierMoveBottom;

                    poItem = jobsRepo.Add(poItem);
                }

                sql = @"UPDATE tblFileQuoteDetail SET QuotePOItemsKey = 
                                (SELECT POItemsKey 
                                    FROM tblJobPurchaseOrderItems 
                                WHERE POItemsQuoteItemKey = QuoteKey AND POItemsPOKey = @POKey) 
                            WHERE QuoteFileKey = @FileKey AND QuoteVendorKey = @VendorKey";

                SqlCommand cmd = new SqlCommand(sql, oConn);
                cmd.Parameters.Add("@POKey", SqlDbType.Int).Value = POKey;
                cmd.Parameters.Add("@VendorKey", SqlDbType.Int).Value = VendorKey;
                cmd.Parameters.Add("@FileKey", SqlDbType.Int).Value = FileKey;

                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
        }

        private void InsertJobPOInstructions(int FVFreightDestination, int POKey, SqlConnection oConn)
        {
            string sql = string.Empty;

            if (FVFreightDestination == 200)
            {
                sql = "INSERT INTO tblJobPurchaseOrderInstructions (POInstructionsPOKey, POInstructionsStep, POInstructionsInstructionKey) VALUES (@POKey, 350, 17)";
            }
            else
            {
                sql = "INSERT INTO tblJobPurchaseOrderInstructions (POInstructionsPOKey, POInstructionsStep, POInstructionsInstructionKey) VALUES (@POKey, 300, 4)";
            }

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@POKey", SqlDbType.Int).Value = POKey;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
        }

        private void UpdateQuoteHeaderWithJob(int JobKey, int FileKey, SqlConnection oConn)
        {
            string sql = "UPDATE tblFileQuoteHeader SET QHdrJobKey = @JobKey WHERE QHdrFileKey = @FileKey";
            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@FileKey", SqlDbType.Int).Value = FileKey;
            cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
        }

        private void CopyJobRolesFromFile(int JobKey, int FileKey, SqlConnection oConn)
        {
            string sql = "INSERT INTO tblJobEmployeeRoles (JobEmployeeJobKey, JobEmployeeRoleKey, JobEmployeeEmployeeKey) SELECT @JobKey AS JobKey, FileEmployeeRoleKey, FileEmployeeEmployeeKey FROM dbo.tblFileEmployeeRoles WHERE FileEmployeeFileKey = @FileKey";
            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@FileKey", SqlDbType.Int).Value = FileKey;
            cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
        }

        private void UpdateFileQuoteCurrencyRates(int FileKey, SqlConnection oConn)
        {
            string sql = @"SET XACT_ABORT ON;
                            DECLARE @mensaje nvarchar(MAX)

                            BEGIN TRY
                                BEGIN TRANSACTION;

                                --*** Update file header
                                UPDATE dbo.tblFileHeader SET FileDefaultCurrencyRate = 
                                (SELECT dbo.tblCurrencyRates.CurrencyRate FROM dbo.tblCurrencyRates 
                                WHERE dbo.tblCurrencyRates.CurrencyCode = dbo.tblFileHeader.FileDefaultCurrencyCode) 
                                WHERE FileKey = @FileKey
                                
                                --*** Update Vendor Info
                                UPDATE dbo.tblFileQuoteVendorInfo SET FVDiscountCurrencyRate = 
                                (SELECT dbo.tblCurrencyRates.CurrencyRate FROM dbo.tblCurrencyRates 
                                WHERE dbo.tblCurrencyRates.CurrencyCode = dbo.tblFileQuoteVendorInfo.FVDiscountCurrencyCode) 
                                WHERE FVFileKey = @FileKey
                                
                                --*** Then the PO rate
                                UPDATE dbo.tblFileQuoteVendorInfo SET FVPOCurrencyRate = 
                                (SELECT dbo.tblCurrencyRates.CurrencyRate FROM dbo.tblCurrencyRates 
                                WHERE dbo.tblCurrencyRates.CurrencyCode = dbo.tblFileQuoteVendorInfo.FVPOCurrencyCode) 
                                WHERE FVFileKey = @FileKey
                                
                                --*** Update Quote Header (if any)
                                UPDATE dbo.tblFileQuoteHeader SET QHdrCurrencyRate = 
                                (SELECT dbo.tblCurrencyRates.CurrencyRate FROM dbo.tblCurrencyRates 
                                WHERE dbo.tblCurrencyRates.CurrencyCode = dbo.tblFileQuoteHeader.QHdrCurrencyCode) 
                                WHERE QHdrFileKey = @FileKey

                                --*** Update Quote Charges
                                UPDATE dbo.tblFileQuoteCharges SET QChargeCurrencyRate = 
                                (SELECT dbo.tblCurrencyRates.CurrencyRate FROM dbo.tblCurrencyRates 
                                WHERE dbo.tblCurrencyRates.CurrencyCode = dbo.tblFileQuoteCharges.QChargeCurrencyCode) 
                                WHERE QChargeFileKey = @FileKey

                                --*** Update quote detail
                                UPDATE dbo.tblFileQuoteDetail SET QuoteItemCurrencyRate = 
                                (SELECT dbo.tblCurrencyRates.CurrencyRate FROM dbo.tblCurrencyRates 
                                WHERE dbo.tblCurrencyRates.CurrencyCode = dbo.tblFileQuoteDetail.QuoteItemCurrencyCode) 
                                WHERE QuoteFileKey = @FileKey
                            COMMIT TRANSACTION;
                        END TRY

                        BEGIN CATCH

                            IF (XACT_STATE()) = -1
                            BEGIN
                                set @mensaje = ERROR_MESSAGE()
                                ROLLBACK TRANSACTION;
                            END;

	                        IF (XACT_STATE()) = 1
                            BEGIN
                                COMMIT TRANSACTION;   
                            END;
                        END CATCH;

                        select ISNULL(@mensaje,'')";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@FileKey", SqlDbType.Int).Value = FileKey;
            string msg = String.Empty;

            try
            {
                msg = (string)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            if (!string.IsNullOrEmpty(msg))
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + msg);
                throw new Exception(msg);
            }
        }

        private decimal GetFOBValueUSD(int QHdrKey, SqlConnection oConn)
        {
            //decimal curFOBValueUSD = 0;
            //try
            //{
            //    curFOBValueUSD = GetFOBValueUSD(QHdrKey, oConn);
            //}
            //catch (Exception ex)
            //{
            //    ConnManager.CloseConn(oConn);
            //    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            //    throw;
            //}

            //'*** Search for vendors in the detail that do not have a FileVendorInfo entry
            string sql = @"SELECT dbo.qsumQuoteTotals.LinePrice, dbo.tblCurrencyRates.CurrencyRate 
                            FROM dbo.tblCurrencyRates INNER JOIN dbo.qsumQuoteTotals 
                            ON dbo.tblCurrencyRates.CurrencyCode = dbo.qsumQuoteTotals.Currency 
                            WHERE dbo.qsumQuoteTotals.QHdrKey = @QHdrKey

                           SELECT QChargePrice FROM qsumFileQuotechargesByCategoryUSD 
                           WHERE QHdrKey = @QHdrKey AND QCDCategory = 1 ";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("QHdrKey", SqlDbType.Int).Value = QHdrKey;

            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            var dt = ds.Tables[0];
            decimal FOBValueUSD = 0;

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                var LinePrice = (decimal)row[0];
                var CurrencyRate = (decimal)row[1];

                FOBValueUSD = LinePrice * CurrencyRate;
            }

            dt = ds.Tables[1];

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                var ChargePrice = (decimal)row[0];

                FOBValueUSD += ChargePrice;
            }

            return FOBValueUSD;
        }

        public int CheckUnconfirmedVendor(int QHdrKey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = @"SELECT Count(*) 
                            FROM tblFileQuoteVendorInfo
                           WHERE FVQuoteInfoConfirmed = 0 AND FVQHdrKey = @QHdrKey";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = QHdrKey;

            int result = 0;
            try
            {
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        public void CleanupFileQuoteVendorInfo(int FileKey)
        {

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            //'*** Delete FileVendorInfo entries that are no longer needed
            try
            {
                CleanupFileQuoteVendorInfoDelete(FileKey, oConn);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            //'*** Search for vendors in the detail that do not have a FileVendorInfo entry
            string sql = @"SELECT QuoteVendorKey 
                            FROM tblFileQuoteDetail 
                           WHERE QuoteFileKey = @FileKey AND QuoteVendorKey Not IN (SELECT FVVendorKey FROM tblFileQuoteVendorInfo WHERE FVFileKey = @FileKey) 
                            GROUP BY QuoteVendorKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("FileKey", SqlDbType.Int).Value = FileKey;

            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                int vendorKey = 0;
                foreach (DataRow row in dt.Rows)
                {
                    vendorKey = (int)row[0];
                    // Insert FileVendorInfo if don't exists
                    InsertFileQuoteVendorInfo(vendorKey, FileKey, oConn);
                }
            }
            ConnManager.CloseConn(oConn);
        }

        private void CleanupFileQuoteVendorInfoDelete(int FileKey, SqlConnection oConn)
        {
            SqlCommand cmd = new SqlCommand("qdelFileQuoteVendorInfoCleanup", oConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@FileKey", SqlDbType.Int).Value = FileKey;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
        }

        private void InsertFileQuoteVendorInfo(int VendorKey, int FileKey, SqlConnection oConn)
        {
            FileHeader file = Get(FileKey, oConn);

            string sql = "INSERT INTO tblFileQuoteVendorInfo (FVFileKey, FVVendorKey, FVPOCurrencyCode, FVPOCurrencyRate) VALUES (@FileKey, @VendorKey, @CurrencyCode, @CurrencyRate)";
            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@FileKey", SqlDbType.Int).Value = FileKey;
            cmd.Parameters.Add("@VendorKey", SqlDbType.Int).Value = VendorKey;
            cmd.Parameters.Add("@CurrencyCode", SqlDbType.NVarChar).Value = file.FileDefaultCurrencyCode;
            cmd.Parameters.Add("@CurrencyRate", SqlDbType.Decimal).Value = file.FileDefaultCurrencyRate;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
        }

        public bool IsQuoteOverFOBLimit(int QHdrKey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            decimal curFOBValueUSD = 0;
                
            curFOBValueUSD = GetFOBValueUSD(QHdrKey, oConn);
    
            //'*** Open up a recordset with the FOB Limit
            string sql = @"SELECT dbo.tblCountries.CountryFOBValueForInspection * dbo.tblCurrencyRates.CurrencyRate AS FOBLimit 
                            FROM dbo.tblCustomerShipAddress INNER JOIN dbo.tblFileHeader 
                            ON dbo.tblCustomerShipAddress.ShipKey = dbo.tblFileHeader.FileCustShipKey INNER JOIN dbo.tblFileQuoteHeader 
                            ON dbo.tblFileHeader.FileKey = dbo.tblFileQuoteHeader.QHdrFileKey INNER JOIN 
                            dbo.tblCountries ON dbo.tblCustomerShipAddress.ShipCountryKey = dbo.tblCountries.CountryKey INNER JOIN 
                            dbo.tblCurrencyRates ON dbo.tblCountries.CountryFOBValueForInspectionCurrencyCode = dbo.tblCurrencyRates.CurrencyCode 
                            WHERE dbo.tblFileQuoteHeader.QHdrKey = @QHdrKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = QHdrKey;

            DataSet ds = new DataSet();

            try {
                da.Fill(ds);
            } catch(Exception ex) {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            ConnManager.CloseConn(oConn);

            if(ds.Tables[0].Rows.Count == 0) {
                return true;
            } else {
                Decimal FOBLimit = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
                
                if(FOBLimit == 0) return false;

                return (curFOBValueUSD >= FOBLimit);
            }
        }

        public bool CheckConfirmedVendor(int FileKey, int QHdrKey, int VendorKey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = @"UPDATE tblFileQuoteVendorInfo SET FVQuoteInfoConfirmed = 1
                           WHERE FVQHdrKey = @QHdrKey AND FVFileKey = @FileKey AND FVVendorKey = @VendorKey";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = QHdrKey;
            cmd.Parameters.Add("@FileKey", SqlDbType.Int).Value = FileKey;
            cmd.Parameters.Add("@VendorKey", SqlDbType.Int).Value = VendorKey;

            int rowsAffected = 0;
            try
            {
                rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            ConnManager.CloseConn(oConn);

            return true;
        }

        private int GetNewNum(string table, string prefix, int year, SqlConnection oConn)
        {
            string strSQL = @"SELECT TOP 1 {0}Num FROM {1}
                                WHERE {0}Year = {2}
                                ORDER BY {0}Num DESC";

            year = year == 0 ? DateTime.Now.Year : year;

            strSQL = String.Format(strSQL, prefix, table, year);

            SqlCommand cmd = new SqlCommand(strSQL, oConn);

            int jobnum = 0;
            try
            {
                jobnum = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            jobnum = jobnum == 0 ? 1 : jobnum + 1;
            return jobnum;
        }
        #endregion Convert Quote

        #region Export Quote To Peachtree
        public string ExportQuoteToPeachtree(int QHdrKey, string currentUser)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = "SELECT * FROM qryFileQuoteToPeachtree WHERE QHdrKey = @QHdrKey";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = QHdrKey;

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            DataTable dt;
            dt = ds.Tables[0];

            qryFileQuoteToPeachtree fileQuote;

            if (dt.Rows.Count > 0)
            {
                fileQuote = EnumExtension.ToList<qryFileQuoteToPeachtree>(dt).FirstOrDefault();
            }
            else
            {
                ConnManager.CloseConn(oConn);
                return "";
            }

            //'*** Validate data
            //If IsNull(rstQuote!CustPeachtreeID) Then
            //    MsgBox "This Customer record does not have a Peachtree ID.  Please edit this customer in the database and add the Peachtree ID!", vbExclamation
            //    DoCmd.OpenForm "frmCustomers", acNormal, , "CustKey = " & rstQuote!CustKey
            //    Forms!frmCustomers!CustPeachtreeID.SetFocus
            //    GoTo Exit_ExportQuoteToPeachtree
            //End If

            int RecordCount = 0;
            //IList<qryJobPurchaseOrderPeachtreeItem> items = GetPOItemsForExport(QHdrKey, ref RecordCount, oConn);
            IList<qryFileQuoteChargesToPeachtree> charges = GetQuoteChargesForExport(QHdrKey, ref RecordCount, oConn);

            if (RecordCount == 0)
            {
                return "";
            }

            string[] strOutput = new string[51];

            //POShipTo POShipTo = GetPOShipTo(fileQuote, oConn);

            strOutput[0] = fileQuote.CustPeachtreeID ?? "";
            strOutput[1] = "";
            strOutput[2] = "";
            strOutput[3] = "FALSE";
            strOutput[4] = fileQuote.QHdrDate.ToString("m/d/yyyy");
            strOutput[5] = "";
            strOutput[6] = "TRUE";
            strOutput[7] = fileQuote.QuoteNum;

            // shipto
            strOutput[8] = fileQuote.QHdrDate.ToString("m/d/yyyy");
            strOutput[9] = "FALSE"; //(POShipTo != null) ? String.Format("{0}{1}{0}", '"', POShipTo.ShipAddress1) : "";
            strOutput[10] = String.Format("{0}{1}{0}", '"', fileQuote.ShipName);
            strOutput[11] = String.Format("{0}{1}{0}", '"', fileQuote.ShipAddress1);
            strOutput[12] = String.Format("{0}{1}{0}", '"', fileQuote.ShipAddress2);
            strOutput[13] = String.Format("{0}{1}{0}", '"', fileQuote.ShipCity);
            strOutput[14] = String.Format("{0}{1}{0}", '"', fileQuote.ShipState);
            strOutput[15] = String.Format("{0}{1}{0}", '"', fileQuote.ShipZip);
            strOutput[16] = String.Format("{0}{1}{0}", '"', fileQuote.ShipCountry);
            strOutput[17] = String.Format("{0}{1}{0}", '"', fileQuote.FileReference);
            strOutput[18] = String.Format("{0}{1}{0}", '"', fileQuote.ShipType);
            // shipto

            strOutput[19] = "";
            strOutput[20] = strOutput[4];
            strOutput[21] = "0";
            strOutput[22] = strOutput[4];
            strOutput[23] = String.Format("{0}{1}{0}", '"', GetPaymentTerms(fileQuote.QHdrCustPaymentTerms, fileQuote.CustLanguageCode, oConn));
            strOutput[24] = String.Format("{0}{1}{0}", '"', fileQuote.EmployeePeachtreeID);
            strOutput[25] = "12000";
            strOutput[26] = "";
            strOutput[27] = "";
            strOutput[28] = "FALSE";
            strOutput[29] = "";
            strOutput[30] = "FALSE";
            strOutput[31] = "";
            strOutput[32] = "FALSE";
            strOutput[33] = RecordCount.ToString();
            strOutput[35] = "0";
            strOutput[36] = "FALSE";
            strOutput[38] = "";
            strOutput[40] = "0";
            strOutput[42] = "40100";
            strOutput[44] = "1";
            strOutput[46] = "";
            strOutput[47] = "";
            strOutput[48] = "";
            strOutput[49] = "";
            strOutput[50] = "";

            string path = Path.Combine(HttpContext.Current.Request.MapPath("~/App_Data/Peachtree/"));
            string filename = Path.Combine(path, "QUOTE" + DateTime.Now.ToString("yyyyMMdd") + ".csv");

            if (File.Exists(filename))
                File.Delete(filename);

            StreamWriter ws = new StreamWriter(filename);
            //'*** Start exporting the detail lines
            int Counter = 0;
            int lngCurrentVendorKey = 0;
            string strPeachtreeItemID = String.Empty;

            //'*** Export the charges
            foreach (var charge in charges)
            {
                Counter += 1;
                strOutput[34] = Counter.ToString();
                //'*** Check for note on top, add line if necessary
                if (!string.IsNullOrEmpty(fileQuote.QuoteItemMemoCustomer) && !fileQuote.QuoteItemMemoCustomerMoveBottom)
                {
                    strOutput[37] = "";
                    strOutput[39] = "";
                    strOutput[43] = "";
                    strOutput[45] = "";

                    //'*** Process note, each CRLF or ~ has to be a separate line
                    strOutput[41] = "";
                    var strNote = fileQuote.QuoteItemMemoCustomer.Replace(Environment.NewLine, "~") + "~";
                    string[] Notes = strNote.Split(Environment.NewLine.ToCharArray());

                    foreach (var note in Notes)
                    {
                        strOutput[41] = String.Format("{0}{1}{0}", '"', note.Replace(@"""", "''"));
                        ws.WriteLine(string.Join(",", strOutput));
                        strOutput[41] = "";
                        Counter += 1;
                        strOutput[34] = Counter.ToString();
                    }
                }

                //'*** Export the item description
                strOutput[37] = fileQuote.QuoteQty.ToString();
                //'*** Check for null PeachtreeItemID, prompt for one if necessary
                if (fileQuote.VendorKey != lngCurrentVendorKey)
                {
                    if (string.IsNullOrEmpty(fileQuote.VendorPeachtreeItemID))
                    {
                        //strPeachtreeItemID = InputBox("The Peachtree Item ID field is blank for " & !VendorName & ".  Please enter what you want to appear in the Item ID column within Peachtree (this must already be in the item database of Peachtree):", "Enter Peachtree Item ID", !VendorPeachtreeID)
                        //If Len(strPeachtreeItemID & vbNullString) = 0 Then
                        //    GoTo Exit_ExportQuoteToPeachtree
                        //Else
                        //    DoCmd.RunSQL "UPDATE tblVendors SET VendorPeachtreeItemID = '" & Trim(strPeachtreeItemID) & "' WHERE VendorKey = " & !VendorKey
                        //End If
                    }
                    else
                    {
                        strPeachtreeItemID = fileQuote.VendorPeachtreeItemID.Trim();
                    }
                }

                lngCurrentVendorKey = fileQuote.VendorKey;

                strOutput[39] = String.Format("{0}{1}{0}", '"', strPeachtreeItemID);
                strOutput[41] = String.Format("{0}{1}{0}", '"', fileQuote.ItemNum + " - " + GetItemDescription(fileQuote.QuoteItemKey, fileQuote.CustLanguageCode, oConn).Replace(@"""", "''"));
                strOutput[43] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", fileQuote.QuoteItemPrice);
                strOutput[45] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", fileQuote.QuoteItemLinePrice);

                ws.WriteLine(string.Join(",", strOutput));

                //'*** Check for note on bottom, add line if necessary
                if (!string.IsNullOrEmpty(fileQuote.QuoteItemMemoCustomer) && fileQuote.QuoteItemMemoCustomerMoveBottom)
                {
                    strOutput[37] = "";
                    strOutput[39] = "";
                    strOutput[43] = "";
                    strOutput[45] = "";

                    //'*** Process note, each CRLF or ~ has to be a separate line
                    strOutput[41] = "";
                    var strNote = fileQuote.QuoteItemMemoCustomer.Replace(Environment.NewLine, "~") + "~";
                    string[] Notes = strNote.Split(Environment.NewLine.ToCharArray());

                    foreach (var note in Notes)
                    {
                        strOutput[41] = String.Format("{0}{1}{0}", '"', note.Replace(@"""", "''"));
                        ws.WriteLine(string.Join(",", strOutput));
                        strOutput[41] = "";
                        Counter += 1;
                        strOutput[34] = Counter.ToString();
                    }
                }
            }

            //'*** Export the Charges
            Counter = Counter + 1;
            strOutput[34] = Counter.ToString();

            foreach (var charge in charges)
            {
                strOutput[37] = "1";
                strOutput[39] = String.Format("{0}{1}{0}", '"', charge.QCDPeachtreeID);
                strOutput[41] = "";
                strOutput[42] = charge.QCDGLAccount.ToString();
                strOutput[43] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", charge.QChargePrice);
                strOutput[45] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", charge.QChargePrice);

                //'*** Process description
                if (!string.IsNullOrEmpty(charge.QChargeMemo))
                {
                    //strNote = Replace(!QChargeMemo, vbCrLf, "~") & "~"
                    //For intStringPosition = 1 To Len(strNote)
                    //    strCharacter = Mid(strNote, intStringPosition, 1)
                    //    If strCharacter = "~" Then
                    //        strOutput[41] = Chr(34) & Replace(strOutput[41), Chr(34), "''") & Chr(34)
                    //        PrintLine
                    //        strOutput[37] = ""
                    //        strOutput[39] = ""
                    //        strOutput[41] = ""
                    //        strOutput[43] = ""
                    //        strOutput[45] = ""
                    //        lngCounter = lngCounter + 1
                    //        strOutput[34] = CStr(lngCounter)
                    //    Else
                    //        strOutput[41] = strOutput[41) & strCharacter
                    //    End If
                    //Next

                    strOutput[41] = "";
                    var strNote = charge.QChargeMemo.Replace(Environment.NewLine, "~") + "~";
                    string[] Notes = strNote.Split(Environment.NewLine.ToCharArray());

                    foreach (var note in Notes)
                    {
                        strOutput[41] = String.Format("{0}{1}{0}", '"', note.Replace(@"""", "''"));
                        ws.WriteLine(string.Join(",", strOutput));
                        strOutput[37] = "";
                        strOutput[39] = "";
                        strOutput[41] = "";
                        strOutput[43] = "";
                        strOutput[45] = "";
                        Counter += 1;
                        strOutput[34] = Counter.ToString();
                    }
                }
                else
                {
                    ws.WriteLine(string.Join(",", strOutput));
                    Counter = Counter + 1;
                    strOutput[34] = Counter.ToString();
                }
            }

            ConnManager.CloseConn(oConn);
            ws.Close();
            return filename;
        }

        private string GetItemDescription(int ItemKey, string LanguageCode, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblItemDescriptions WHERE DescriptionItemKey = @ItemKey and DescriptionLanguageCode = @LanguageCode";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@ItemKey", SqlDbType.Int).Value = ItemKey;
            da.SelectCommand.Parameters.Add("@LanguageCode", SqlDbType.NVarChar).Value = LanguageCode;

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string DescriptionText = "* * * No Description on File * * *";

            if (ds.Tables[0].Rows.Count > 0)
            {
                ItemDescription itemDesc = EnumExtension.ToList<ItemDescription>(ds.Tables[0]).FirstOrDefault();
                DescriptionText = itemDesc.DescriptionText;
            }
            else if (LanguageCode != "en")
            {
                DescriptionText = GetItemDescription(ItemKey, LanguageCode, oConn);
            }

            return DescriptionText;
        }

        private IList<qryFileQuoteChargesToPeachtree> GetQuoteChargesForExport(int QHdrKey, ref int RecordCount, SqlConnection oConn)
        {
            string sql = "SELECT * FROM qryFileQuoteChargesToPeachtree WHERE QHdrKey = @QHdrKey";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = QHdrKey;

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }


            if (ds.Tables[0].Rows.Count > 0)
            {
                IList<qryFileQuoteChargesToPeachtree> charges = EnumExtension.ToList<qryFileQuoteChargesToPeachtree>(ds.Tables[0]);
                RecordCount = RecordCount + charges.Count;
                return charges;
            }

            return new List<qryFileQuoteChargesToPeachtree>();
        }

        private string GetPaymentTerms(int TermKey, string LanguageCode, SqlConnection oConn)
        {
            LanguageCode = string.IsNullOrEmpty(LanguageCode) ? "en" : LanguageCode;
            string sql = "SELECT * FROM tlkpPaymentTermsDescriptions WHERE PTTermKey = @TermKey AND PTLanguageCode = @LanguageCode";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@TermKey", SqlDbType.Int).Value = TermKey;
            da.SelectCommand.Parameters.Add("@LanguageCode", SqlDbType.NVarChar).Value = LanguageCode;

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string returnValue = "* * * This file has an invalid selection * * *";

            if (ds.Tables[0].Rows.Count > 0)
            {
                PaymentTermsDescriptions term = EnumExtension.ToList<PaymentTermsDescriptions>(ds.Tables[0]).FirstOrDefault();
                returnValue = term.PTDescription;
            }

            return returnValue;
        }
        #endregion Exporte Quote To Peachtree

        public IList<qsumFileQuoteChargesByGLAccount> GetqsumFileQuoteChargesByGLAccount(int QHdrKey, ref int totalRecords)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            string sql = @"SELECT * FROM qsumFileQuoteChargesByGLAccount WHERE QHdrKey = @QHdrKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("QHdrKey", SqlDbType.Int).Value = QHdrKey;

            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qsumFileQuoteChargesByGLAccount> data = EnumExtension.ToList<qsumFileQuoteChargesByGLAccount>(dt);
                return data;
            }
            else
            {
                return new List<qsumFileQuoteChargesByGLAccount>();
            }
        }

        public void UpdateFileQuoteCurrencyRate(int FileKey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            try
            {
                UpdateFileQuoteCurrencyRates(FileKey, oConn);
            }
            catch (Exception ex)
            {
                
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
            finally
            {
                ConnManager.CloseConn(oConn);
            }


        }

        #region Import from File & Quote
        public bool ImportFile(int FileKeySource, int FileKeyTarget, string CurrentUser)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            try
            {
                ImportFile(FileKeySource, FileKeyTarget, CurrentUser, oConn);
            }
            catch (Exception ex)
            {

                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
            finally
            {
                ConnManager.CloseConn(oConn);
            }

            return true;
        }

        private bool ImportFile(int FileKeySource, int FileKeyTarget, string CurrentUser, SqlConnection oConn)
        {
            var newDetails = new List<FileQuoteDetail>();
            var details = GetQuoteDetails(FileKeySource, oConn);

            details = details.OrderBy(o => o.QuoteSort).ToList();

            int nSort = GetNextQuoteSort(FileKeyTarget, oConn);

            foreach (var detail in details)
            {
                var QDetail = new FileQuoteDetail();
                QDetail.QuoteFileKey = FileKeyTarget;
                QDetail.QuoteSort = nSort;
                QDetail.QuoteQty = detail.QuoteQty;
                QDetail.QuoteVendorKey = detail.QuoteVendorKey;
                QDetail.QuoteItemKey = detail.QuoteItemKey;
                QDetail.QuoteItemCost = detail.QuoteItemCost;
                QDetail.QuoteItemPrice = detail.QuoteItemPrice;
                QDetail.QuoteItemLineCost = detail.QuoteItemLineCost;
                QDetail.QuoteItemLinePrice = detail.QuoteItemLinePrice;
                QDetail.QuoteItemCurrencyCode = detail.QuoteItemCurrencyCode;
                QDetail.QuoteItemCurrencyRate = detail.QuoteItemCurrencyRate;
                QDetail.QuoteItemWeight = detail.QuoteItemWeight;
                QDetail.QuoteItemVolume = detail.QuoteItemVolume;
                QDetail.QuoteItemMemoCustomer = detail.QuoteItemMemoCustomer;
                QDetail.QuoteItemMemoCustomerMoveBottom = detail.QuoteItemMemoCustomerMoveBottom;
                QDetail.QuoteItemMemoSupplier = detail.QuoteItemMemoSupplier;
                QDetail.QuoteItemMemoSupplierMoveBottom = detail.QuoteItemMemoSupplierMoveBottom;
                //QDetail.QuotePOItemsKey = Null
                nSort += 100;

                newDetails.Add(QDetail);
            }

            bool saved = Save(newDetails);

            if (!saved)
            {
                return false;
            }

            CleanupFileQuoteVendorInfo(FileKeyTarget);

            return true;
        }

        private bool Save(List<FileQuoteDetail> Details)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            SqlTransaction oTX = oConn.BeginTransaction();

            bool unSafe = false;

            foreach (var detail in Details)
            {
                var saved = Add(detail, oConn, oTX);

                if (saved == null)
                {
                    unSafe = true;
                    break;
                }
            }

            if (unSafe)
            {
                oTX.Rollback();
                ConnManager.CloseConn(oConn);
                return false;
            }

            oTX.Commit();
            ConnManager.CloseConn(oConn);

            return true;
        }

        private int GetNextQuoteSort(int FileKey, SqlConnection oConn)
        {
            string sql = @"DECLARE @next int = 0
                        SELECT @next = MAX(QuoteSort) FROM tblFileQuoteDetail where QuoteFileKey = @FileKey
                        SELECT ISNULL(@next,0)";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@FileKey", SqlDbType.Int).Value = FileKey;

            int nSort = 0;
            try
            {
                nSort = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            return nSort + 100;
        }

        public bool ImportQuote(int QHdrKeySource, int FileKeyTarget, string CurrentUser)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            try
            {
                int FileKeySource = 0;
                int VendorKeySource = 0;
                FindFileFromQuote(QHdrKeySource, ref FileKeySource, ref VendorKeySource, oConn);
                ImportQuote(QHdrKeySource, FileKeySource, VendorKeySource, FileKeyTarget, CurrentUser, oConn);
            }
            catch (Exception ex)
            {

                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
            finally
            {
                ConnManager.CloseConn(oConn);
            }

            return true;
        }

        private void FindFileFromQuote(int QHdrKeySource, ref int FileKeySource, ref int VendorKeySource, SqlConnection oConn)
        {
            string sql = "SELECT FVFileKey, FVVendorKey FROM tblFileQuoteVendorInfo WHERE FVQHdrKey = @QHdrKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@QHdrKey", SqlDbType.Int).Value = QHdrKeySource;

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                FileKeySource = Convert.ToInt32(dt.Rows[0]["FVFileKey"]);
                VendorKeySource = Convert.ToInt32(dt.Rows[0]["FVVendorKey"]);
            }
        }

        private bool ImportQuote(int QHdrKeySource, int FileKeySource, int VendorKeySource, int FileKeyTarget, string CurrentUser, SqlConnection oConn)
        {
            var newDetails = new List<FileQuoteDetail>();
            var details = GetQuoteDetails(FileKeySource, oConn);

            details = details.Where(w => w.QuoteVendorKey == VendorKeySource).OrderBy(o => o.QuoteSort).ToList();

            int nSort = GetNextQuoteSort(FileKeyTarget, oConn);

            foreach (var detail in details)
            {
                var QDetail = new FileQuoteDetail();
                QDetail.QuoteFileKey = FileKeyTarget;
                QDetail.QuoteSort = nSort;
                QDetail.QuoteQty = detail.QuoteQty;
                QDetail.QuoteVendorKey = detail.QuoteVendorKey;
                QDetail.QuoteItemKey = detail.QuoteItemKey;
                QDetail.QuoteItemCost = detail.QuoteItemCost;
                QDetail.QuoteItemPrice = detail.QuoteItemPrice;
                QDetail.QuoteItemLineCost = detail.QuoteItemLineCost;
                QDetail.QuoteItemLinePrice = detail.QuoteItemLinePrice;
                QDetail.QuoteItemCurrencyCode = detail.QuoteItemCurrencyCode;
                QDetail.QuoteItemCurrencyRate = detail.QuoteItemCurrencyRate;
                QDetail.QuoteItemWeight = detail.QuoteItemWeight;
                QDetail.QuoteItemVolume = detail.QuoteItemVolume;
                QDetail.QuoteItemMemoCustomer = detail.QuoteItemMemoCustomer;
                QDetail.QuoteItemMemoCustomerMoveBottom = detail.QuoteItemMemoCustomerMoveBottom;
                QDetail.QuoteItemMemoSupplier = detail.QuoteItemMemoSupplier;
                QDetail.QuoteItemMemoSupplierMoveBottom = detail.QuoteItemMemoSupplierMoveBottom;
                //QDetail.QuotePOItemsKey = Null
                nSort += 100;

                newDetails.Add(QDetail);
            }

            bool saved = Save(newDetails);

            if (!saved)
            {
                return false;
            }

            //CleanupFileQuoteVendorInfo(FileKeyTarget);

            return true;
        }
        #endregion Import from File & Quote

        public bool IsOverCustCreditLimit(int CustKey, decimal AmountUSD)
        {
            decimal CustCreditLimit = 0,
                    CurrencyRate = 0;
            DataTable dt = new DataTable();
            using (SqlConnection oConn = ConnManager.OpenConn())
            {

                string sql = @"SELECT dbo.tblCustomers.CustCreditLimit, dbo.tblCurrencyRates.CurrencyRate 
                                FROM dbo.tblCustomers INNER JOIN dbo.tblCurrencyRates 
                                ON dbo.tblCustomers.CustCurrencyCode = dbo.tblCurrencyRates.CurrencyCode 
                                WHERE dbo.tblCustomers.CustKey = @CustKey";

                SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
                da.SelectCommand.Parameters.Add("@CustKey", SqlDbType.Int).Value = CustKey;

                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                    throw;
                }

                if (dt.Rows.Count == 0)
                {
                    throw new Exception("There was an unknown error trying to find the customer credit limit.");
                }

                CustCreditLimit = Convert.ToDecimal(dt.Rows[0]["CustCreditLimit"]);
                CurrencyRate = Convert.ToDecimal(dt.Rows[0]["CurrencyRate"]);

                CustCreditLimit = CustCreditLimit * CurrencyRate;
            }

            return !(AmountUSD <= CustCreditLimit);
        }
    }
}