namespace CBHWA.Models
{
    using CBHWA.Areas.Reports.Models;
    using CBHWA.Clases;
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using Utilidades;

    public class EmployeeRepository : IEmployeeRepository
    {
        public IList<Employee> GetList(FieldFilters fieldFilters, string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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
                string strqry = "ISNULL(EmployeeFirstName,'') + ' ' + ISNULL(EmployeeLastName,'') + ' ' + ISNULL(EmployeeEmail,'')";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, strqry);
            }

            #region field filters
            if (fieldFilters.fields != null && fieldFilters.fields.Count > 0)
            {
                foreach (var item in fieldFilters.fields)
                {
                    string value = item.value;
                    string name = item.name;

                    if (item.type == "string" || item.type == "date")
                        value = "'" + value + "'";

                    if (item.type == "date")
                        name = String.Format("CAST({0} as DATE)", name);

                    where += String.Format(" AND {0} = {1}", name, value);
                }
            }
            #endregion field filters

            #region Ordenamiento
            string order = "x_EmployeeFullName";
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
                             SELECT *, RTRIM(EmployeeLastName)+' '+RTRIM(EmployeeFirstName) as x_EmployeeFullName
                             FROM tblEmployees 
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
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<Employee> data = EnumExtension.ToList<Employee>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }

            return null;
        }

        public IList<Employee> GetAll(FieldFilters fieldFilters, string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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

            //string where = (page != 0) ? " EmployeeStatusCode = 8 and row>@start and row<=@limit " : " EmployeeStatusCode = 8 ";
            string where = (page != 0) ? " row>@start and row<=@limit " : " 1 = 1 ";

            if (!string.IsNullOrEmpty(query))
            {
                string strqry = "ISNULL(EmployeeFirstName,'') + ' ' + ISNULL(EmployeeLastName,'') + ' ' + ISNULL(EmployeeEmail,'')";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, strqry);
            }

            if (fieldFilters.fields != null && fieldFilters.fields.Count > 0)
            {
                foreach (var item in fieldFilters.fields)
                {
                    string value = item.value;
                    string name = item.name;

                    if (item.type == "string" || item.type == "date")
                        value = "'" + value + "'";

                    if (item.type == "date")
                        name = String.Format("CAST({0} as DATE)", name);

                    where += String.Format(" AND {0} = {1}", name, value);
                }
            }

            string sql = "SELECT * FROM ( " +
                         "SELECT *, RTRIM(EmployeeLastName)+' '+RTRIM(EmployeeFirstName) as x_EmployeeFullName," +
                         "  ROW_NUMBER() OVER (ORDER BY EmployeeLastName) as row,  " +
                         "  IsNull((select count(*) from tblEmployees),0)  as TotalRecords   " +
                         " FROM tblEmployees) a  " +
                         "WHERE " + where +
                         " ORDER BY EmployeeLastName";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            if (page != 0)
            {
                da.SelectCommand.Parameters.Add("@start", SqlDbType.Int).Value = Convert.ToInt32(start);
                da.SelectCommand.Parameters.Add("@limit", SqlDbType.Int).Value = Convert.ToInt32(limit);
            };

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            totalRecords = (int)dt.Rows[0]["TotalRecords"];

            IList<Employee> data = EnumExtension.ToList<Employee>(dt);

            return data;
        }

        public Employee Get(int id)
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

            Employee desc;
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

        private Employee Get(int id, SqlConnection oConn)
        {
            string sql = @"SELECT *, RTRIM(EmployeeLastName)+' '+RTRIM(EmployeeFirstName) as x_EmployeeFullName 
                            FROM tblEmployees
                           WHERE (EmployeeKey = @id)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<Employee> data = EnumExtension.ToList<Employee>(dt);

            if (dt.Rows.Count > 0)
            {
                return data.FirstOrDefault<Employee>();
            }

            return null;
        }

        public Employee Add(Employee dataadded)
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

            string sql = "INSERT INTO tblEmployees ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            dataadded.EmployeeCreatedDate = DateTime.Now;
            EnumExtension.setListValues(dataadded, "EmployeeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int id = 0;

            try
            {
                id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            Employee data = Get(id, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public Employee Update(Employee dataupdated)
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

            string sql = "UPDATE tblEmployees SET {0} WHERE EmployeeKey = @id";

            EnumExtension.setUpdateValues(dataupdated, "EmployeeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(dataupdated.EmployeeKey);

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

            Employee data = Get(dataupdated.EmployeeKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(Employee datadeleted)
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

        private bool Remove(Employee datadeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblEmployees " +
                         " WHERE (EmployeeKey = @id)";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = datadeleted.EmployeeKey;

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public IList<Employee> GetListForReport(int JobRoleKey, string startDate, string endDate, string reportName, string query, int page, int start, int limit, ref int totalRecords)
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
                sql = @"SELECT EmployeeKey, CONVERT (varchar, EmployeeLastName) + ', ' + CONVERT (varchar, EmployeeFirstName) AS x_EmployeeFullName 
                            FROM tblEmployees 
                            WHERE EmployeeStatusCode = 8 AND EmployeeKey IN (SELECT EmployeeKey FROM qryReportCriteriaJobRoles WHERE {0})
                            ORDER BY EmployeeLastName";

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
                sql = @"SELECT EmployeeKey, CONVERT (varchar, EmployeeLastName) + ', ' + CONVERT (varchar, EmployeeFirstName) AS x_EmployeeFullName 
                            FROM tblEmployees 
                            WHERE EmployeeStatusCode = 8 AND EmployeeKey IN (SELECT EmployeeKey FROM qryReportCriteriaJobRoles WHERE {0})
                            ORDER BY EmployeeLastName";

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
                sql = @"SELECT EmployeeKey, CONVERT (varchar, EmployeeLastName) + ', ' + CONVERT (varchar, EmployeeFirstName) AS x_EmployeeFullName 
                            From tblEmployees 
                            WHERE EmployeeStatusCode = 8 
                            AND EmployeeKey in (SELECT FileEmployeeEmployeeKey 
                                                    From tblFileEmployeeRoles 
                                                 WHERE FileEmployeeFileKey in (SELECT FileKey 
                                                                                  From tblFileHeader 
                                                                                  WHERE {0})) 
                            ORDER BY EmployeeLastName";

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
                sql = @"SELECT EmployeeKey, CONVERT (varchar, EmployeeLastName) + ', ' + CONVERT (varchar, EmployeeFirstName) AS x_EmployeeFullName 
                            FROM tblEmployees WHERE EmployeeKey IN (SELECT EmployeeKey FROM dbo.qrptCustomerWebLogins WHERE {0})
                            ORDER BY EmployeeLastName";

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
                sql = @"SELECT EmployeeKey, CONVERT (varchar, EmployeeLastName) + ', ' + CONVERT (varchar, EmployeeFirstName) AS x_EmployeeFullName 
                            FROM tblEmployees 
                        WHERE EmployeeStatusCode = 8";
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
                return null;
            }
            finally
            {
                ConnManager.CloseConn(oConn);
            }

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<Employee> data = dt.ToList<Employee>();
                totalRecords = Convert.ToInt32(dt.Rows.Count);
                return data;
            }
            else
            {
                return null;
            }
        }

        public bool EnqueueReport(Enqueue model)
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

            string sql = "INSERT INTO tblPrintQueue (PrintEmployeeKey, PrintRptName, PrintStrWhere) VALUES (@id, @report, @where)";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.EmployeeKey;
            cmd.Parameters.Add("@report", SqlDbType.NVarChar).Value = model.ReportName;
            cmd.Parameters.Add("@where", SqlDbType.NVarChar).Value = model.strWhere ?? "";

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return true;
        }
    }
}