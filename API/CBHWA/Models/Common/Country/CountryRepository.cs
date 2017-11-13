namespace CBHWA.Models
{
    using CBHWA.Clases;
    using Helpers;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Reflection;
    using Utilidades;

    public class CountryRepository : ICountryRepository
    {
        public IList<Country> GetAll()
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

            IList<Country> data;
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

        private IList<Country> GetAll(SqlConnection oConn)
        {
            string sql = "SELECT * " +
                         " FROM tblCountries " +
                         " ORDER BY CountryName";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<Country> data = EnumExtension.ToList<Country>(dt);

            return data;
        }

        public IList<Country> GetListForReport(string startDate, string endDate, string reportName, string query, int page, int start, int limit, ref int totalRecords)
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
            string sql = "";
            string where = "";

            if ((new string[] { "rptJobProfit", "rptJobProfitWithExemptions", "ExcelJobProfit", "rptJobSummary" }).Contains(reportName))
            {
                sql = @"SELECT CountryKey, CountryName 
                            FROM tblCountries
                            WHERE CountryKey IN (SELECT ShipCountryKey FROM dbo.tblJobHeader INNER JOIN dbo.tblCustomerShipAddress ON dbo.tblJobHeader.JobCustShipKey = dbo.tblCustomerShipAddress.ShipKey WHERE {0}) 
                            ORDER BY CountryName";

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
                sql = @"SELECT CountryKey, CountryName 
                        FROM tblCountries 
                        WHERE CountryKey IN (SELECT ShipCountryKey FROM dbo.tblJobHeader INNER JOIN dbo.tblCustomerShipAddress ON dbo.tblJobHeader.JobCustShipKey = dbo.tblCustomerShipAddress.ShipKey WHERE {0}) ORDER BY CountryName";

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
                sql = @"SELECT CountryKey, CountryName 
                        FROM tblCountries 
                        WHERE CountryKey IN (SELECT ShipCountryKey FROM dbo.tblFileHeader INNER JOIN dbo.tblCustomerShipAddress ON dbo.tblFileHeader.FileCustShipKey = dbo.tblCustomerShipAddress.ShipKey WHERE {0}) 
                        ORDER BY CountryName";

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
                sql = @"SELECT CountryKey, CountryName 
                        FROM tblCountries 
                        WHERE CountryKey IN (SELECT CountryKey FROM dbo.qrptCustomerWebLogins WHERE {0}) 
                        ORDER BY CountryName";

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
                sql = @"SELECT CountryKey, CountryName 
                        FROM tblCountries 
                        WHERE CountryKey IN (SELECT CountryKey FROM dbo.qrptPronacaReport WHERE {0}) 
                        ORDER BY CountryName";

                where = "1=1";
                if (!string.IsNullOrEmpty(startDate))
                    where += string.Format(" AND CAST(JobShipDate as Date) >= '{0}'", startDate);
                if (!string.IsNullOrEmpty(endDate))
                    where += string.Format(" AND CAST(JobShipDate as Date) <= '{0}'", endDate);

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
                IList<Country> data = dt.ToList<Country>();
                totalRecords = Convert.ToInt32(dt.Rows.Count);
                return data;
            }
            else
            {
                return null;
            }
        }
     }
}