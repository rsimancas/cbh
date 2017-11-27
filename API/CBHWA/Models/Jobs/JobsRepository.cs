namespace CBHWA.Models
{
    using CBHWA.Clases;
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

    public class JobsRepository : IJobsRepository
    {
        static readonly ICurrencyRatesRepository curRatesRepo = new CurrencyRatesRepository();
        static readonly IPaymentTermsRepository payTermsRepo = new PaymentTermsRepository();
        static readonly IEmployeeRepository employeeRepo = new EmployeeRepository();
        static readonly IVendorsRepository vendorsRepo = new VendorsRepository();
        public JobsRepository()
        {
            //dbcontext = new DBContext();
        }

        public IList<JobList> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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
            string where = !string.IsNullOrEmpty(query) ? "" : "1=1";

            if (!string.IsNullOrEmpty(query))
            {
                string whereQuery = "";

                string fieldName = "ISNULL(a.[Job Num],'')+ISNULL(CONVERT(NVARCHAR,a.JobCreatedDate,101),'')+ISNULL(a.Customer,'')+ISNULL(a.Reference,'')+ISNULL(a.Status,'')";
                whereQuery += EnumExtension.generateLikeWhere(query, fieldName);

                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    "(" + whereQuery + ")";
            }

            #region Ordenamiento
            string order = "JobNum";
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
                        select a.JobKey,a.JobCreatedDate as Date,a.[Job Num] as JobNum,a.Customer,a.Reference,a.Status,a.JobCreatedBy,  
                        a.JobClosed,dbo.fnGetJobStatus(JobKey) AS JobStatusDesc,a.JobModifiedBy as StatusModifiedBy, CONVERT(VARCHAR(8),a.JobModifiedDate) as StatusModifiedDate,
                        a.JobCustKey, a.JobCustShipKey, a.JobShipType, a.JobWarehouseKey, a.CustCurrencyCode, a.CustCurrencyRate, a.JobQHdrKey, a.Quote, a.QHdrFileKey, a.FileNum
                        from dbo.qlstjobs a 
                        where {0} 
                        ) 
                        select * 
                        from ( 
                           select *, 
                           ROW_NUMBER() OVER (ORDER BY {1} {2}) as row,  
                           IsNull((select count(*) from qData),0)  as TotalRecords 
                           from qData ) a 
                        WHERE row between @start + 1 and @start + @limit 
                        ORDER BY row";

            sql = String.Format(sql, where, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.CommandTimeout = 15000;
            da.SelectCommand.Parameters.Add("@start", SqlDbType.Int).Value = Convert.ToInt32(start);
            da.SelectCommand.Parameters.Add("@limit", SqlDbType.Int).Value = Convert.ToInt32(limit);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobList> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<JobList>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public JobList GetList(int id)
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
            string where = "a.JobKey=@JobKey";

            string sql = @"WITH qData 
                        AS 
                        ( 
                        select a.JobKey,a.JobCreatedDate as Date,a.[Job Num] as JobNum,a.Customer,a.Reference,a.Status,a.JobCreatedBy,  
                        a.JobClosed,dbo.fnGetJobStatus(JobKey) AS JobStatusDesc,a.JobModifiedBy as StatusModifiedBy, CONVERT(VARCHAR(8),a.JobModifiedDate) as StatusModifiedDate,
                        a.JobCustKey, a.JobCustShipKey, a.JobShipType, a.JobWarehouseKey, a.CustCurrencyCode, a.CustCurrencyRate, a.JobQHdrKey, a.Quote, a.QHdrFileKey, a.FileNum
                        from dbo.qlstjobs a 
                        where {0} 
                        ) 
                        select * 
                        from ( 
                           select *, 
                           IsNull((select count(*) from qData),0)  as TotalRecords 
                           from qData ) a 
                        ORDER BY JobKey";

            sql = String.Format(sql, where);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@JobKey", SqlDbType.Int).Value = id;

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            JobList data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<JobList>(dt).FirstOrDefault();
            }
            else
            {
                return null;
            }

            return data;
        }

        public IList<qJobOverview> GetJobOverview(int jobKey)
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

            string sql = @"WITH qData
                            AS
                            (
                                SELECT a.JobKey, a.JobQHdrKey, dbo.fnGetQuoteNum(a.JobQHdrKey) AS QuoteNum, dbo.fnGetJobNum(a.JobKey) AS JobNum, 
                                    a.JobShipDate, a.JobArrivalDate, a.JobShipmentCarrier, a.JobCarrierRefNum, a.JobCarrierVessel, 
                                    a.JobInspectionCertificateNum, a.JobCustPaymentTerms, a.JobClosed, a.JobComplete, a.JobModifiedBy, 
                                    a.JobModifiedDate, a.JobCreatedBy, a.JobCreatedDate, a.JobExemptFromProfitReport, b.CustName, 
                                    c.ContactLastName, c.ContactFirstName, b.CustPhone, b.CustFax, b.CustEmail, 
                                    c.ContactPhone, c.ContactFax, c.ContactEmail, b.CustCurrencyCode,
	                                d.CurrencyRate as CustCurrencyRate
                                FROM tblJobHeader a INNER JOIN
                                    tblCustomers b ON a.JobCustKey = b.CustKey INNER JOIN
                                    tblCustomerContacts c ON a.JobContactKey = c.ContactKey INNER JOIN
	                                tblCurrencyRates d ON b.CustCurrencyCode = d.CurrencyCode
                                WHERE JobKey = @jobKey
                            )
                            select *,ISNULL(ContactPhone, CustPhone) as x_Phone, ISNULL(ContactEmail, CustEmail) as x_Email, 
                                ISNULL(ContactFax, CustFax) as x_Fax, RTRIM(ContactLastName) + ',' + RTRIM(ContactFirstName) as x_ContactName, 
                                ISNULL(ListText,'') as x_JobShipmentCarrierText,
                                dbo.fnGetPaymentTerms(a.JobCustPaymentTerms, 'en') as x_JobCustPaymentTerms,
	                            ISNULL('Closed ' + CONVERT(VARCHAR(11),JobClosed,109) ,'') + CHAR(13) + CHAR(10) +
	                            ISNULL('Complete ' + CONVERT(VARCHAR(11),JobComplete,109) ,'') as x_Info
                            from qData a LEFT OUTER JOIN tlkpGenericLists ON ListKey = JobShipmentCarrier AND ListCategory = 2";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@jobKey", SqlDbType.Int).Value = Convert.ToInt32(jobKey);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<qJobOverview> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<qJobOverview>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        #region Used in Convert Quote To Job
        public void UpdateJobCurrencyRates(int JobKey, bool UseCurrentRates)
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

            //'*** Make sure the Job Currency Master table is up to date
            UpdateJobCurrencyMaster(JobKey, oConn);

            //'*** If using current rates, update the job master currency table
            if (UseCurrentRates) UpdateJobCurrencyMasterWithCurrentRates(JobKey, oConn);

            //'*** Update job header
            string sql = @"UPDATE dbo.tblJobHeader SET JobCustCurrencyRate = 
                            (SELECT dbo.tblJobCurrencyMaster.JobCurrencyRate FROM dbo.tblJobCurrencyMaster 
                            WHERE dbo.tblJobCurrencyMaster.JobCurrencyJobKey = @JobKey AND dbo.tblJobCurrencyMaster.JobCurrencyCode = dbo.tblJobHeader.JobCustCurrencyCode) 
                            WHERE JobKey = @JobKey";

            //'*** Update job header
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, oConn))
                {
                    cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            //'*** Update Job Purchase Order Items
            sql = @"UPDATE dbo.tblJobPurchaseOrderItems SET POItemsCurrencyRate = 
                            (SELECT dbo.tblJobCurrencyMaster.JobCurrencyRate FROM dbo.tblJobCurrencyMaster 
                            WHERE dbo.tblJobCurrencyMaster.JobCurrencyJobKey = @JobKey AND dbo.tblJobCurrencyMaster.JobCurrencyCode = dbo.tblJobPurchaseOrderItems.POItemsCurrencyCode) 
                        WHERE POItemsJobKey = @JobKey";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, oConn))
                {
                    cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            //'*** Update Job Purchase Orders
            sql = @"UPDATE dbo.tblJobPurchaseOrders SET POCurrencyRate = 
                            (SELECT dbo.tblJobCurrencyMaster.JobCurrencyRate FROM dbo.tblJobCurrencyMaster 
                            WHERE dbo.tblJobCurrencyMaster.JobCurrencyJobKey = @JobKey AND dbo.tblJobCurrencyMaster.JobCurrencyCode = dbo.tblJobPurchaseOrders.POCurrencyCode) 
                        WHERE POJobKey = @JobKey";
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, oConn))
                {
                    cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            //'*** Update Invoice
            sql = @"UPDATE dbo.tblInvoiceHeader SET InvoiceCurrencyRate = 
                            (SELECT dbo.tblJobCurrencyMaster.JobCurrencyRate FROM dbo.tblJobCurrencyMaster 
                            WHERE dbo.tblJobCurrencyMaster.JobCurrencyJobKey = @JobKey AND dbo.tblJobCurrencyMaster.JobCurrencyCode = dbo.tblInvoiceHeader.InvoiceCurrencyCode) 
                        WHERE InvoiceJobKey = @JobKey";

            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, oConn))
                {
                    cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            //'*** Update Invoice
            sql = @"UPDATE dbo.tblInvoiceHeader SET InvoiceCurrencyRate = 
                            (SELECT dbo.tblJobCurrencyMaster.JobCurrencyRate FROM dbo.tblJobCurrencyMaster 
                            WHERE dbo.tblJobCurrencyMaster.JobCurrencyJobKey = @JobKey AND dbo.tblJobCurrencyMaster.JobCurrencyCode = dbo.tblInvoiceHeader.InvoiceCurrencyCode) 
                        WHERE InvoiceJobKey = @JobKey";

            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, oConn))
                {
                    cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            //'*** Update Invoice Charges
            sql = @"UPDATE dbo.tblInvoiceCharges SET IChargeCurrencyRate = 
                            (SELECT dbo.tblJobCurrencyMaster.JobCurrencyRate FROM dbo.tblJobCurrencyMaster 
                            WHERE dbo.tblJobCurrencyMaster.JobCurrencyJobKey = @JobKey AND dbo.tblJobCurrencyMaster.JobCurrencyCode = dbo.tblInvoiceCharges.IChargeCurrencyCode) 
                        WHERE IChargeInvoiceKey IN (SELECT InvoiceKey FROM dbo.tblInvoiceHeader WHERE InvoiceJobKey = @JobKey)";

            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, oConn))
                {
                    cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            //'*** Update Invoice Summary Items
            sql = @"UPDATE dbo.tblInvoiceItemsSummary SET ISummaryCurrencyRate = 
                            (SELECT dbo.tblJobCurrencyMaster.JobCurrencyRate FROM dbo.tblJobCurrencyMaster 
                            WHERE dbo.tblJobCurrencyMaster.JobCurrencyJobKey = @JobKey AND dbo.tblJobCurrencyMaster.JobCurrencyCode = dbo.tblInvoiceItemsSummary.ISummaryCurrencyCode) 
                        WHERE ISummaryInvoiceKey IN (SELECT InvoiceKey FROM dbo.tblInvoiceHeader WHERE InvoiceJobKey = @JobKey)";

            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, oConn))
                {
                    cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }


        }

        private void UpdateJobCurrencyMaster(int JobKey, SqlConnection oConn)
        {
            //'*** This routine goes through the various charges in the job and finds what currency rates and present.
            //'*** If currency matches JobHeader, use JobRate
            //'*** All others look up current rate from table
            var jobHeader = Get(JobKey);
            var jobCurrencyList = GetJobCurrencyMasterList(JobKey, oConn);

            if (jobCurrencyList == null)
            {
                //'*** Record does not exist in the Currency Master, add it
                var jobCurrency= new JobCurrencyMaster();
                jobCurrency.JobCurrencyCode = jobHeader.JobCustCurrencyCode;
                jobCurrency.JobCurrencyRate = jobHeader.JobCustCurrencyRate;
                jobCurrency.JobCurrencyJobKey = JobKey;

                jobCurrency = Add(jobCurrency);
            }
            else
            {
                //'*** Look for record
                var curCust = (from p in jobCurrencyList 
                               where p.JobCurrencyCode == jobHeader.JobCustCurrencyCode
                               select p).ToList().First();

                if(curCust == null) {
                    var jobCurrency= new JobCurrencyMaster();
                    jobCurrency.JobCurrencyCode = jobHeader.JobCustCurrencyCode;
                    jobCurrency.JobCurrencyRate = jobHeader.JobCustCurrencyRate;
                    jobCurrency.JobCurrencyJobKey = JobKey;

                    jobCurrency = Add(jobCurrency);
                } else {
                    //'*** Match found, verify rates match
                    if(curCust.JobCurrencyRate != jobHeader.JobCustCurrencyRate) {
                            jobHeader.JobCustCurrencyRate = curCust.JobCurrencyRate;
                            jobHeader = Update(jobHeader);
                    }
                }
            }

            jobCurrencyList = GetJobCurrencyMasterList(JobKey, oConn);

            //'*** Review the rest of the currency rates
            //'*** Go through all PO Headers
            var jobDetails = GetJobPurchaseOrderByJob(JobKey);

            foreach(var jobDetail in jobDetails) {
                //'*** If currency matches the job header, no changes needed
                if (jobDetail.POCurrencyCode == jobHeader.JobCustCurrencyCode) {
                    //'*** Verify that rate matches
                    if (jobDetail.POCurrencyRate != jobHeader.JobCustCurrencyRate) {
                        jobDetail.POCurrencyRate = jobHeader.JobCustCurrencyRate;
                        Update(jobDetail);
                    }
                } else {
                    //'*** PO Currency does not match Job currency, make sure it is in the master table
                    var jobCurrency = jobCurrencyList.Where(w => w.JobCurrencyCode == jobDetail.POCurrencyCode).First();
                    if(jobCurrency == null) {
                        jobCurrency= new JobCurrencyMaster();
                        jobCurrency.JobCurrencyCode = jobDetail.POCurrencyCode;
                        jobCurrency.JobCurrencyRate = jobDetail.POCurrencyRate.GetValueOrDefault();
                        jobCurrency.JobCurrencyJobKey = JobKey;
                    } else {
                        //'*** Make sure the PO Rate matches the master rate
                        if(jobDetail.POCurrencyRate != jobCurrency.JobCurrencyRate) {
                            jobDetail.POCurrencyRate = jobCurrency.JobCurrencyRate;
                            Update(jobDetail);
                        }
                    }
                }

                //'*** Use the POKey to scan the PO Payments table
                List<string> otherDetails = GetOtherDetails(JobKey, jobDetail.POKey, oConn);
                //'*** Add this currency to the Currency Master
                foreach(var currencyCode in otherDetails) {
                    //'*** Get the current rate for this currency
                    var curRate = curRatesRepo.Get(currencyCode);
                    var jobCurrency = new JobCurrencyMaster();
                    jobCurrency.JobCurrencyJobKey = JobKey;
                    jobCurrency.JobCurrencyCode = curRate.CurrencyCode;
                    jobCurrency.JobCurrencyRate = curRate.CurrencyRate;
                    jobCurrency = Add(jobCurrency);
                }
            }

            //'*** Go through all Invoice Headers
            var invoices = GetListInvoicesByJob(JobKey, oConn);

            foreach(var invoice in invoices) {
                //'*** If currency matches the job header, no changes needed
                if(invoice.InvoiceCurrencyCode == jobHeader.JobCustCurrencyCode) {
                    //'*** Verify that rate matches
                    
                }
            }
        }

        private void UpdateJobCurrencyMasterWithCurrentRates(int JobKey, SqlConnection oConn)
        {
            string sql = "SELECT JobCurrencyLockedDate FROM tblJobHeader WHERE JobKey = @JobKey";

            try
            {
                Nullable<DateTime> date = null;
                using (SqlCommand cmd = new SqlCommand(sql, oConn))
                {
                    cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;
                    var result = cmd.ExecuteScalar();
                    date = (result is DBNull) ? null : (Nullable<DateTime>)result;
                }

                if (date == null) return;

                sql = @"UPDATE tblJobCurrencyMaster SET JobCurrencyRate = (SELECT CurrencyRate FROM tblCurrencyRates WHERE CurrencyCode = JobCurrencyCode) 
                            WHERE JobCurrencyJobKey = @JobKey";

                using (SqlCommand cmd = new SqlCommand(sql, oConn))
                {
                    cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

        }

        private IList<JobCurrencyMaster> GetJobCurrencyMasterList(int JobKey, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblJobCurrencyMaster WHERE JobCurrencyJobKey = @JobKey";

            using (SqlDataAdapter da = new SqlDataAdapter(sql, oConn))
            {
                da.SelectCommand.Parameters.Add("JobKey", SqlDbType.Int).Value = JobKey;

                DataSet ds = new DataSet();

                da.Fill(ds);

                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    var lista = EnumExtension.ToList<JobCurrencyMaster>(dt);
                    return lista;
                }
            }

            return null;
        }

        private List<string> GetOtherDetails(int JobKey, int POKey, SqlConnection oConn)
        {
            string sql = @"SELECT POPaymentCurrencyCode 
                            FROM tblJobPurchaseOrderPayments 
                            WHERE POPaymentPOKey = @POKey AND POPaymentCurrencyCode Not In (SELECT JobCurrencyCode FROM tblJobCurrencyMaster WHERE JobCurrencyJobKey = @JobKey) 
                          GROUP BY POPaymentCurrencyCode";

            using (SqlDataAdapter da = new SqlDataAdapter(sql, oConn))
            {
                da.SelectCommand.Parameters.Add("JobKey", SqlDbType.Int).Value = JobKey;
                da.SelectCommand.Parameters.Add("POKey", SqlDbType.Int).Value = JobKey;

                DataSet ds = new DataSet();

                da.Fill(ds);

                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    var lista = new List<string>();
                    foreach (DataRow row in dt.Rows)
                    {
                        lista.Add((string)row[0]);
                    }
                    return lista;
                }
            }

            return new List<string>();
        }

        public JobCurrencyMaster Add(JobCurrencyMaster jobCurMaster)
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

            string sql = "INSERT INTO tblJobCurrencyMaster ({0}) VALUES ({1})";

            EnumExtension.setListValues(jobCurMaster, "", ref sql);

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

            JobCurrencyMaster data = jobCurMaster;

            ConnManager.CloseConn(oConn);

            return data;
        }
        #endregion Used in Convert Quote To Job

        #region Header
        public JobHeader Add(JobHeader jobHeader)
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

            string sql = "INSERT INTO tblJobHeader ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            jobHeader.JobYear = jobHeader.JobCreatedDate.Year;

            EnumExtension.setListValues(jobHeader, "JobKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int jobKeyGenerated = 0;

            try
            {
                jobKeyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                setJobNum(jobKeyGenerated, jobHeader.JobYear, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

            JobHeader data = Get(jobKeyGenerated, oConn);

            insertRoles(data, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private void insertRoles(JobHeader jobData, SqlConnection oConn)
        {
            //string sql = "INSERT INTO tblJobEmployeeRoles (JobEmployeeJobKey, JobEmployeeRoleKey, JobEmployeeEmployeeKey, JobEmployeeCreatedBy) VALUES ({0}, 1, {1}, '{2}')";
            //sql = String.Format(sql, jobData.JobKey, jobData.JobSalesEmployeeKey, jobData.JobCreatedBy);
            string sql = "INSERT INTO tblJobEmployeeRoles (JobEmployeeJobKey, JobEmployeeRoleKey, JobEmployeeEmployeeKey, JobEmployeeCreatedBy) VALUES ({0}, 3, {1}, '{2}')";
            sql = String.Format(sql, jobData.JobKey, jobData.JobOrderEmployeeKey, jobData.JobCreatedBy);
            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
                //sql = "INSERT INTO tblJobEmployeeRoles (JobEmployeeJobKey, JobEmployeeRoleKey, JobEmployeeEmployeeKey, JobEmployeeCreatedBy) VALUES ({0}, 3, {1}, '{2}')";
                //sql = String.Format(sql, jobData.JobKey, jobData.JobOrderEmployeeKey, jobData.JobCreatedBy);
                //cmd.CommandText = sql;
                //cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }
        }

        public JobHeader Update(JobHeader jobHeader)
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

            string sql = "UPDATE tblJobHeader SET {0} WHERE JobKey = " + jobHeader.JobKey.ToString();

            EnumExtension.setUpdateValues(jobHeader, "JobKey", ref sql);

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

            JobHeader data = Get(jobHeader.JobKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public JobHeader Get(int id)
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

            JobHeader job;
            try
            {
                job = Get(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return job;
        }

        private JobHeader Get(int id, SqlConnection oConn)
        {
            string sql = "SELECT a.*,dbo.fnGetJobNum(a.JobKey) as x_JobNumFormatted " +
                " FROM tblJobHeader a " +
                " WHERE (JobKey = @key)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobHeader> data = EnumExtension.ToList<JobHeader>(dt);

            return data.FirstOrDefault<JobHeader>();
        }

        public bool Remove(JobHeader job)
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
                result = Remove(job, oConn);
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

        private bool Remove(JobHeader job, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblJobHeader " +
                         " WHERE JobKey = {0}";

            sql = String.Format(sql, job.JobKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        private void setJobNum(int id, int year, SqlConnection oConn)
        {
            string sql = "select count(*) from tblJobHeader where JobYear={0} and JobKey<={1}";
            sql = String.Format(sql, year.ToString(), id.ToString());
            SqlCommand cmd = new SqlCommand(sql, oConn);
            try
            {
                int jobNum = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

                sql = "update tblJobHeader set JobNum={0} where JobKey={1}";
                sql = String.Format(sql, jobNum.ToString(), id.ToString());

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

        }
        #endregion Header

        #region Job Employee Roles
        public IList<JobEmployeeRoles> GetJobEmployeeRoles(int jobkey, ref int totalRecords)
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

            IList<JobEmployeeRoles> summary;
            try
            {
                summary = GetJobEmployeeRoles(jobkey, ref totalRecords, oConn);
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

        private IList<JobEmployeeRoles> GetJobEmployeeRoles(int jobkey, ref int totalRecords, SqlConnection oConn)
        {
            string sql = @"select a.*,IsNull(b.JobRoleDescription,'') as x_RoleName,
                            IsNull(CONVERT(varchar, c.EmployeeLastName) + ', ' + CONVERT(varchar, c.EmployeeFirstName),'') as x_EmployeeName 
                         from tblJobEmployeeRoles a 
                            LEFT OUTER JOIN tlkpJobRoles b on a.JobEmployeeRoleKey=b.JobRoleKey 
                            LEFT OUTER JOIN tblEmployees c on a.JobEmployeeEmployeeKey=c.EmployeeKey
                          WHERE (a.JobEmployeeJobKey = {0})";

            sql = String.Format(sql, jobkey);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobEmployeeRoles> data = null;
            if (dt.Rows.Count > 0)
            {
                totalRecords = dt.Rows.Count;
                data = EnumExtension.ToList<JobEmployeeRoles>(dt);
            }

            return data;
        }

        public bool Remove(JobEmployeeRoles role)
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

        private bool Remove(JobEmployeeRoles role, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblJobEmployeeRoles" +
                         " WHERE (JobEmployeeRoleKey = {0})";

            sql = String.Format(sql, role.JobEmployeeRoleKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public JobEmployeeRoles Update(JobEmployeeRoles jobrole)
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

            JobEmployeeRoles roleupdated;
            try
            {
                roleupdated = Update(jobrole, oConn);
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

        private JobEmployeeRoles Update(JobEmployeeRoles jobrole, SqlConnection oConn)
        {
            string sql = "UPDATE tblJobEmployeeRoles SET {0} WHERE JobEmployeeKey = " + jobrole.JobEmployeeKey.ToString();

            EnumExtension.setUpdateValues(jobrole, "JobEmployeeKey", ref sql);

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

            JobEmployeeRoles data = GetRole(jobrole.JobEmployeeKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public JobEmployeeRoles GetRole(int id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblJobEmployeeRoles " +
                        " WHERE (JobEmployeeKey = @key)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobEmployeeRoles> data = EnumExtension.ToList<JobEmployeeRoles>(dt);

            return data.FirstOrDefault<JobEmployeeRoles>();
        }

        public JobEmployeeRoles Add(JobEmployeeRoles role)
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

            string sql = "INSERT INTO tblJobEmployeeRoles ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(role, "JobEmployeeKey", ref sql);

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

            JobEmployeeRoles data = GetRole(idGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;

        }
        #endregion Job Employee Roles

        #region qLstJobPurchaseOrder
        public IList<qJobPurchaseOrder> GetListJobPurchaseOrders(int jobKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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
            string where = (jobKey > 0) ? String.Format("a.POJobKey = {0}", jobKey) : "1=1";

            if (!string.IsNullOrEmpty(query))
            {
                string whereQuery = "";

                string fieldName = "ISNULL(STR(a.Date),'')+ISNULL(a.[Job Num],'')+ISNULL(a.Vendor,'')+ISNULL(a.Status,'')";
                whereQuery += EnumExtension.generateLikeWhere(query, fieldName);

                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    "(" + whereQuery + ")";
            }

            #region Ordenamiento
            string order = "PONum";
            string direction = "DESC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;

                //if (order == "PONum") order = "a.[PO Num]";
            }
            #endregion Ordenamiento

            //            string sql = @"WITH qData 
            //                         AS ( 
            //                          SELECT a.POKey,a.JobKey,b.PODate as Date,a.[PO Num] as PONum,a.Vendor,a.Status,a.Cost, 
            //                         ROW_NUMBER() OVER (ORDER BY {1} {2}) as row 
            //                          FROM dbo.qlstJobPurchaseOrders a 
            //                            INNER JOIN tblJobPurchaseOrders b ON a.POKey = b.POKey
            //                          WHERE {0} 
            //                         ) 
            //                         SELECT a.*,b.TotalRecords 
            //                         FROM qData a
            //                            INNER JOIN (SELECT COUNT(*) as TotalRecords FROM qData) as b ON 1=1
            //                         WHERE row between @start and @limit";

            string sql = @"WITH qData
                         AS (
                            SELECT a.POKey, a.POJobKey AS JobKey, a.PODate AS Date, 
	                            dbo.fnGetPONumStyle(a.POKey, 0) AS PONum, b.VendorName AS Vendor, 
	                            ISNULL
		                            ((SELECT  TOP (1) y.StatusText
			                            FROM  tblJobPurchaseOrderstatusHistory x INNER JOIN
			                              tlkpStatus y ON x.POStatusStatusKey = y.StatusKey
			                            WHERE (x.POStatusPOKey = a.POKey)
			                            ORDER BY x.POStatusDate DESC), '*No Status*') AS Status, 
	                            a.POCurrencyCode as CurrencyCode, (ISNULL(c.POItemsCost, 0) + ISNULL(d.POChargesCost, 0)) AS Cost,
	                            a.POCurrencyRate as CurrencyRate, ROW_NUMBER() OVER (ORDER BY {1} {2}) as row 
                            FROM tblJobPurchaseOrders a INNER JOIN
	                             tblVendors b ON a.POVendorKey = b.VendorKey LEFT OUTER JOIN
	                             qsumJobPurchaseOrderItems c ON a.POKey = c.POKey LEFT OUTER JOIN
	                             qsumJobPurchaseOrderCharges d ON a.POKey = d.POKey
                            WHERE {0}
                         ) 
                         SELECT a.*,b.TotalRecords 
                         FROM qData a
                            INNER JOIN (SELECT COUNT(*) as TotalRecords FROM qData) as b ON 1=1
                         WHERE row between @start and @limit 
                         ORDER BY row";


            sql = String.Format(sql, where, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@start", SqlDbType.Int).Value = Convert.ToInt32(start);
            da.SelectCommand.Parameters.Add("@limit", SqlDbType.Int).Value = Convert.ToInt32(limit);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<qJobPurchaseOrder> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<qJobPurchaseOrder>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }
        #endregion qLstJobPurchaseOrder

        #region qlstJobInvoices
        public IList<qJobInvoice> GetListJobInvoices(int jobKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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
            string where = (jobKey > 0) ? String.Format("a.InvoiceJobKey = {0}", jobKey) : "1=1";

            if (!string.IsNullOrEmpty(query))
            {
                string whereQuery = "";

                string fieldName = "ISNULL(STR(a.InvoiceDate),'')";
                whereQuery += EnumExtension.generateLikeWhere(query, fieldName);

                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    "(" + whereQuery + ")";
            }

            #region Ordenamiento
            string order = "a.InvoiceDate";
            string direction = "DESC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;

                if (order == "BillTo") order = "a.InvoiceBillingName";
            }
            #endregion Ordenamiento

            string sql = @"WITH qData
                            AS
                            (
                            SELECT  a.InvoiceKey, a.InvoiceJobKey AS JobKey, a.InvoiceDate AS Date, 
                                dbo.fnGetInvoiceNum(a.InvoiceKey) AS Invoice, a.InvoiceBillingName AS BillTo, 
	                            a.InvoiceCurrencyCode as CurrencyCode,
                                ISNULL(b.InvoiceSummaryPrice, ISNULL(d.InvoiceItemsPrice, 0)) + ISNULL(c.IChargePrice, 0) AS Price,
	                            ROW_NUMBER() OVER (ORDER BY {1} {2}) as row
                            FROM tblInvoiceHeader a LEFT OUTER JOIN
                                qsumInvoiceSummaryItems b ON a.InvoiceKey = b.InvoiceKey LEFT OUTER JOIN
                                qsumInvoiceCharges c ON a.InvoiceKey = c.InvoiceKey LEFT OUTER JOIN
                                qsumInvoiceJobItems d ON a.InvoiceKey = d.InvoiceKey
                            WHERE  ({0})
                            )
                            select *
                            from qData a INNER JOIN (select count(*) as TotalRecords FROM qData) as b ON 1=1
                            ORDER BY row";

            sql = String.Format(sql, where, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            //da.SelectCommand.Parameters.Add("@start", SqlDbType.Int).Value = Convert.ToInt32(start);
            //da.SelectCommand.Parameters.Add("@limit", SqlDbType.Int).Value = Convert.ToInt32(limit);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<qJobInvoice> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<qJobInvoice>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }
        #endregion

        #region Job Purchase Orders
        public IList<JobPurchaseOrder> GetListJobPurchaseOrder(int poJobKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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
            string where = (poJobKey > 0) ? "POJobKey = @jobkey" : "1=1";

            if (!string.IsNullOrEmpty(query))
            {
                string whereQuery = "";

                string fieldName = "PONum";
                whereQuery += EnumExtension.generateLikeWhere(query, fieldName);

                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    "(" + whereQuery + ")";
            }

            #region Ordenamiento
            string order = "PONum";
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
                               select *,dbo.fnGetJobNum(POJobKey) as x_JobNumFormatted, dbo.fnGetPONumStyle(POKey, 0) as x_PONumFormatted
                                    from tblJobPurchaseOrders
                               where {0}
                            )
                            SELECT *
                            FROM (
                               select *, ROW_NUMBER() OVER (ORDER BY {1} {2}) as row,
                                    IsNull((select count(*) from qData),0)  as TotalRecords
                               from qData
                            ) a
                            WHERE row > @start and row <= @limit";

            sql = String.Format(sql, where, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            if (poJobKey > 0) da.SelectCommand.Parameters.Add("@jobkey", SqlDbType.Int).Value = Convert.ToInt32(poJobKey);

            da.SelectCommand.Parameters.Add("@start", SqlDbType.Int).Value = Convert.ToInt32(start);
            da.SelectCommand.Parameters.Add("@limit", SqlDbType.Int).Value = Convert.ToInt32(limit);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobPurchaseOrder> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<JobPurchaseOrder>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public JobPurchaseOrder Add(JobPurchaseOrder dataAdded)
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

            string sql = "INSERT INTO tblJobPurchaseOrders ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataAdded, "POKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int poKeyGenerated = 0;

            try
            {
                poKeyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                setPONum(poKeyGenerated, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            JobPurchaseOrder data = GetJobPurchaseOrder(poKeyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        private void setPONum(int id, SqlConnection oConn)
        {
            string sql = "select TOP 1 PONum from tblJobPurchaseOrders WHERE PONum <> 0 ORDER BY PONum DESC";
            SqlCommand cmd = new SqlCommand(sql, oConn);
            try
            {
                int jobNum = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

                jobNum = jobNum == 1 ? 12000 : jobNum;

                sql = "update tblJobPurchaseOrders set PONum=@po where POKey=@id";
                cmd.CommandText = sql;
                cmd.Parameters.Add("@po", SqlDbType.Int).Value = jobNum;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

        }

        public JobPurchaseOrder Update(JobPurchaseOrder dataUpdated)
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

            string sql = "UPDATE tblJobPurchaseOrders SET {0} WHERE POKey = @POKey";

            EnumExtension.setUpdateValues(dataUpdated, "POKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@POKey", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.POKey);

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

            JobPurchaseOrder data = GetJobPurchaseOrder(dataUpdated.POKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public JobPurchaseOrder GetJobPurchaseOrder(int id)
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

            JobPurchaseOrder job;
            try
            {
                job = GetJobPurchaseOrder(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return job;
        }

        private JobPurchaseOrder GetJobPurchaseOrder(int id, SqlConnection oConn)
        {
            string sql = "SELECT a.*,dbo.fnGetJobNum(POJobKey) as x_JobNumFormatted, dbo.fnGetPONumStyle(POKey, 0) as x_PONumFormatted " +
                " FROM tblJobPurchaseOrders a " +
                " WHERE (POKey = @key)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobPurchaseOrder> data = EnumExtension.ToList<JobPurchaseOrder>(dt);

            return data.FirstOrDefault<JobPurchaseOrder>();
        }

        public bool Remove(JobPurchaseOrder dataDeleted)
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
                result = RemoveJobPurchaseOrder(dataDeleted, oConn);
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

        private bool RemoveJobPurchaseOrder(JobPurchaseOrder dataDeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblJobPurchaseOrders " +
                         " WHERE POKey = @key";

            sql = String.Format(sql, dataDeleted.POKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(dataDeleted.POKey);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public int GetNewPONum()
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

            string sql = "SELECT TOP 1 PONum FROM tblJobPurchaseOrders ORDER BY PONum DESC";
            SqlCommand cmd = new SqlCommand(sql, oConn);

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

            jobnum = jobnum == 0 ? 12000 : jobnum + 1;

            return jobnum;
        }

        public IList<JobPurchaseOrder> GetJobPurchaseOrderByJob(int JobKey)
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
            string where = (JobKey > 0) ? "POJobKey = @jobkey" : "1=1";

            #region Ordenamiento
            string order = "POCreatedDate";
            string direction = "DESC";
            #endregion Ordenamiento

            string sql = @"WITH qData
                           AS 
                           (
                               select *,dbo.fnGetJobNum(POJobKey) as x_JobNumFormatted, dbo.fnGetPONumStyle(POKey, 0) as x_PONumFormatted
                                    from tblJobPurchaseOrders
                               where {0}
                            )
                            SELECT *
                            FROM (
                               select *, ROW_NUMBER() OVER (ORDER BY {1} {2}) as row
                               from qData
                            ) a";

            sql = String.Format(sql, where, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@jobkey", SqlDbType.Int).Value = JobKey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobPurchaseOrder> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<JobPurchaseOrder>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }
        #endregion

        #region Job Purchase Order Items
        public IList<JobPurchaseOrderItem> GetJobPurchaseOrderItems(int POItemsPOKey)
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

            string sql = @"SELECT a.*, b.ItemNum as x_ItemNum,
                                ISNULL(dbo.fnGetItemDescription(a.POItemsItemKey, N'en'),'') as x_ItemName,
                                CAST(POItemsLineCost * POItemsCurrencyRate AS money) AS x_LineCost, 
                                CAST(POItemsLinePrice * POItemsCurrencyRate AS money) AS x_LinePrice, 
                                CAST(POItemsWeight * POItemsQty AS decimal) AS x_LineWeight, 
                                CAST(POItemsVolume * POItemsQty AS decimal) AS x_LineVolume
                            FROM tblJobPurchaseOrderItems a INNER JOIN tblItems b ON a.POItemsItemKey = b.ItemKey
                            WHERE (POItemsPOKey = @pokey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@pokey", SqlDbType.Int).Value = Convert.ToInt32(POItemsPOKey);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobPurchaseOrderItem> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<JobPurchaseOrderItem>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public JobPurchaseOrderItem Add(JobPurchaseOrderItem dataAdded)
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

            string sql = "INSERT INTO tblJobPurchaseOrderItems ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataAdded, "POItemsKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int poKeyGenerated = 0;

            try
            {
                poKeyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                //setJobNum(poKeyGenerated, dataAdded.JobYear, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            JobPurchaseOrderItem data = GetJobPurchaseOrderItem(poKeyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public JobPurchaseOrderItem Updated(JobPurchaseOrderItem dataUpdated)
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

            string sql = "UPDATE tblJobPurchaseOrderItems SET {0} WHERE POItemsKey = @POItemsKey";

            EnumExtension.setUpdateValues(dataUpdated, "POItemsKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@POItemsKey", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.POItemsKey);

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

            JobPurchaseOrderItem data = GetJobPurchaseOrderItem(dataUpdated.POItemsKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public JobPurchaseOrderItem GetJobPurchaseOrderItem(int id)
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

            JobPurchaseOrderItem job;
            try
            {
                job = GetJobPurchaseOrderItem(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return job;
        }

        private JobPurchaseOrderItem GetJobPurchaseOrderItem(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, b.ItemNum as x_ItemNum,
                                CAST(POItemsLineCost * POItemsCurrencyRate AS money) AS x_LineCost, 
                                CAST(POItemsLinePrice * POItemsCurrencyRate AS money) AS x_LinePrice, 
                                CAST(POItemsWeight * POItemsQty AS decimal) AS x_LineWeight, 
                                CAST(POItemsVolume * POItemsQty AS decimal) AS x_LineVolume
                            FROM tblJobPurchaseOrderItems a INNER JOIN tblItems b ON a.POItemsItemKey = b.ItemKey
                            WHERE (POItemsKey = @key)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobPurchaseOrderItem> data = EnumExtension.ToList<JobPurchaseOrderItem>(dt);

            return data.FirstOrDefault<JobPurchaseOrderItem>();
        }

        public bool RemovePurchaseOrderItem(JobPurchaseOrderItem dataDeleted)
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
                result = RemovePurchaseOrderItem(dataDeleted, oConn);
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

        private bool RemovePurchaseOrderItem(JobPurchaseOrderItem dataDeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblJobPurchaseOrderItems WHERE POItemsKey = @key";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(dataDeleted.POItemsKey);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion Job Purchase Order Items

        #region Job Purchase Order Charges
        public IList<JobPurchaseOrderCharge> GetJobPurchaseOrderCharges(int POChargesPOKey)
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

            string sql = @"SELECT a.*, 
                                ROUND(a.POChargesCost / a.POChargesQty,2) AS x_UnitCost, 
                                ROUND(a.POChargesPrice / a.POChargesQty,2) AS x_UnitPrice,
                                c.DescriptionText as x_DescriptionText
                            FROM tblJobPurchaseOrderCharges a 
                                INNER JOIN tlkpChargeCategories b ON a.POChargesChargeKey = b.ChargeKey
                                INNER JOIN tlkpChargeCategoryDescriptions c ON b.ChargeKey = c.DescriptionChargeKey and c.DescriptionLanguageCode='en'
                            WHERE (a.POChargesPOKey = @pokey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            if (POChargesPOKey > 0) da.SelectCommand.Parameters.Add("@pokey", SqlDbType.Int).Value = Convert.ToInt32(POChargesPOKey);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobPurchaseOrderCharge> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<JobPurchaseOrderCharge>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public JobPurchaseOrderCharge Add(JobPurchaseOrderCharge dataAdded)
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

            string sql = "INSERT INTO tblJobPurchaseOrderCharges ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataAdded, "POChargesKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int poKeyGenerated = 0;

            try
            {
                poKeyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                //setJobNum(poKeyGenerated, dataAdded.JobYear, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            JobPurchaseOrderCharge data = GetJobPurchaseOrderCharge(poKeyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public JobPurchaseOrderCharge Updated(JobPurchaseOrderCharge dataUpdated)
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

            string sql = "UPDATE tblJobPurchaseOrderCharges SET {0} WHERE POChargesKey = @POChargesKey";

            EnumExtension.setUpdateValues(dataUpdated, "POChargesKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@POChargesKey", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.POChargesKey);

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

            JobPurchaseOrderCharge data = GetJobPurchaseOrderCharge(dataUpdated.POChargesKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public JobPurchaseOrderCharge GetJobPurchaseOrderCharge(int id)
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

            JobPurchaseOrderCharge job;
            try
            {
                job = GetJobPurchaseOrderCharge(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return job;
        }

        private JobPurchaseOrderCharge GetJobPurchaseOrderCharge(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, 
                                ROUND(a.POChargesCost / a.POChargesQty,2) AS x_UnitCost, 
                                ROUND(a.POChargesPrice / a.POChargesQty,2) AS x_UnitPrice,
                                c.DescriptionText as x_DescriptionText
                            FROM tblJobPurchaseOrderCharges a 
                                INNER JOIN tlkpChargeCategories b ON a.POChargesChargeKey = b.ChargeKey
                                INNER JOIN tlkpChargeCategoryDescriptions c ON b.ChargeKey = c.DescriptionChargeKey and c.DescriptionLanguageCode='en'
                            WHERE (a.POChargesKey = @key)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobPurchaseOrderCharge> data = EnumExtension.ToList<JobPurchaseOrderCharge>(dt);

            return data.FirstOrDefault<JobPurchaseOrderCharge>();
        }

        public bool RemovePurchaseOrderCharge(JobPurchaseOrderCharge dataDeleted)
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
                result = RemovePurchaseOrderCharge(dataDeleted, oConn);
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

        private bool RemovePurchaseOrderCharge(JobPurchaseOrderCharge dataDeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblJobPurchaseOrderCharges WHERE POChargesKey = @key";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(dataDeleted.POChargesKey);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion Job Purchase Order Charges

        #region Job Purchase Order Instructions
        public IList<JobPurchaseOrderInstruction> GetJobPurchaseOrderInstructions(int POInstructionsPOKey)
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

            string sql = @"SELECT a.*, ISNULL(c.ITextMemo,'') as x_ITextMemo,
                            (
                                CASE WHEN POInstructionsMemoFontColor=1 THEN 'Green'
                                    ELSE CASE WHEN POInstructionsMemoFontColor=2 THEN 'Blue'
                                        ELSE CASE WHEN POInstructionsMemoFontColor=3 THEN 'Red'
                                            ELSE 'Black' END
                                    END
                                END
                            ) as x_NotesFontColor
                            FROM tblJobPurchaseOrderInstructions a 
                                INNER JOIN tblJobPurchaseOrders d on a.POInstructionsPOKey = d.POKey
                                INNER JOIN tblJobHeader e on d.POJobKey = e.JobKey 
                                INNER JOIN tblCustomers f on e.JobCustKey = f.CustKey
                                LEFT OUTER JOIN tlkpJobPurchaseOrderInstructions b ON a.POInstructionsInstructionKey = b.InstructionKey
                                LEFT OUTER JOIN tlkpJobPurchaseOrderInstructionsText c ON b.InstructionKey = c.ITextInstructionKey and c.ITextLanguageCode = 'en' --c.ITextLanguageCode = f.CustLanguageCode
                            WHERE (a.POInstructionsPOKey = @pokey AND b.InstructionCategory = 0)
                          ORDER BY POInstructionsStep";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            if (POInstructionsPOKey > 0) da.SelectCommand.Parameters.Add("@pokey", SqlDbType.Int).Value = Convert.ToInt32(POInstructionsPOKey);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobPurchaseOrderInstruction> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<JobPurchaseOrderInstruction>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public JobPurchaseOrderInstruction Add(JobPurchaseOrderInstruction dataAdded)
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

            string sql = "INSERT INTO tblJobPurchaseOrderInstructions ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataAdded, "POInstructionsKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int poKeyGenerated = 0;

            try
            {
                poKeyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                //setJobNum(poKeyGenerated, dataAdded.JobYear, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            JobPurchaseOrderInstruction data = GetJobPurchaseOrderInstruction(poKeyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public JobPurchaseOrderInstruction Updated(JobPurchaseOrderInstruction dataUpdated)
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

            string sql = "UPDATE tblJobPurchaseOrderInstructions SET {0} WHERE POInstructionsKey = @POInstructionsKey";

            EnumExtension.setUpdateValues(dataUpdated, "POInstructionsKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@POInstructionsKey", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.POInstructionsKey);

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

            JobPurchaseOrderInstruction data = GetJobPurchaseOrderInstruction(dataUpdated.POInstructionsKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public JobPurchaseOrderInstruction GetJobPurchaseOrderInstruction(int id)
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

            JobPurchaseOrderInstruction job;
            try
            {
                job = GetJobPurchaseOrderInstruction(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return job;
        }

        private JobPurchaseOrderInstruction GetJobPurchaseOrderInstruction(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, ISNULL(c.ITextMemo,'') as x_ITextMemo,
                            (
                                CASE WHEN POInstructionsMemoFontColor=1 THEN 'Green'
                                    ELSE CASE WHEN POInstructionsMemoFontColor=2 THEN 'Blue'
                                        ELSE CASE WHEN POInstructionsMemoFontColor=3 THEN 'Red'
                                            ELSE 'Black' END
                                    END
                                END
                            ) as x_NotesFontColor
                            FROM tblJobPurchaseOrderInstructions a 
                                LEFT OUTER JOIN tlkpJobPurchaseOrderInstructions b ON a.POInstructionsInstructionKey = b.InstructionKey
                                LEFT OUTER JOIN tlkpJobPurchaseOrderInstructionsText c ON b.InstructionKey = c.ITextInstructionKey
                            WHERE (a.POInstructionsKey = @key)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobPurchaseOrderInstruction> data = EnumExtension.ToList<JobPurchaseOrderInstruction>(dt);

            return data.FirstOrDefault<JobPurchaseOrderInstruction>();
        }

        public bool RemovePurchaseOrderInstruction(JobPurchaseOrderInstruction dataDeleted)
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
                result = RemovePurchaseOrderInstruction(dataDeleted, oConn);
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

        private bool RemovePurchaseOrderInstruction(JobPurchaseOrderInstruction dataDeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblJobPurchaseOrderInstructions WHERE POInstructionsKey = @key";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(dataDeleted.POInstructionsKey);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion Job Purchase Order Instructions

        #region Charges Categories
        public IList<tlkpChargeCategory> GetChargesCategories()
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

            string sql = @"SELECT a.*, b.DescriptionText as x_DescriptionText, 
                                  b.DescriptionLanguageCode as x_DescriptionLanguageCode
                            FROM tlkpChargeCategories a INNER JOIN
                                tlkpChargeCategoryDescriptions b ON a.ChargeKey = b.DescriptionChargeKey
                            WHERE (a.ChargeAPAccount IS NOT NULL)
                            ORDER BY x_DescriptionText";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<tlkpChargeCategory> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<tlkpChargeCategory>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public tlkpChargeCategory Add(tlkpChargeCategory dataAdded)
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

            EnumExtension.setListValues(dataAdded, "ChargeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int poKeyGenerated = 0;

            try
            {
                poKeyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                //setJobNum(poKeyGenerated, dataAdded.JobYear, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            tlkpChargeCategory data = GetChargeCategory(poKeyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public tlkpChargeCategory Updated(tlkpChargeCategory dataUpdated)
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

            string sql = "UPDATE tlkpChargeCategories SET {0} WHERE ChargeKey = @ChargeKey";

            EnumExtension.setUpdateValues(dataUpdated, "ChargeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@ChargeKey", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.ChargeKey);

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

            tlkpChargeCategory data = GetChargeCategory(dataUpdated.ChargeKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public tlkpChargeCategory GetChargeCategory(int id)
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

            tlkpChargeCategory job;
            try
            {
                job = GetChargeCategory(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return job;
        }

        private tlkpChargeCategory GetChargeCategory(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, b.DescriptionText as x_DescriptionText, 
                                  b.DescriptionLanguageCode as x_DescriptionLanguageCode
                            FROM tlkpChargeCategories a INNER JOIN
                                tlkpChargeCategoryDescriptions b ON a.ChargeKey = b.DescriptionChargeKey
                            WHERE a.ChargeKey = @key";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<tlkpChargeCategory> data = EnumExtension.ToList<tlkpChargeCategory>(dt);

            return data.FirstOrDefault<tlkpChargeCategory>();
        }

        public bool RemoveChargeCategory(tlkpChargeCategory dataDeleted)
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
                result = RemoveChargeCategory(dataDeleted, oConn);
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

        private bool RemoveChargeCategory(tlkpChargeCategory dataDeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tlkpChargeCategories WHERE ChargeKey = @key";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(dataDeleted.ChargeKey);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion Charges Categories

        #region Instructions
        public IList<tlkpJobPurchaseOrderInstruction> GettlkpJobPOInstructions(string language)
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

            string where = string.IsNullOrEmpty(language) ? "1=1" : string.Format("b.ITextLanguageCode='{0}'", language);

            string sql = @"SELECT a.*, b.ITextMemo as x_ITextMemo 
                            FROM tlkpJobPurchaseOrderInstructions a 
                                INNER JOIN tlkpJobPurchaseOrderInstructionsText b ON a.InstructionKey = b.ITextInstructionKey
                            WHERE (a.InstructionCategory = 0 AND {0})";

            sql = string.Format(sql, where);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<tlkpJobPurchaseOrderInstruction> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<tlkpJobPurchaseOrderInstruction>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public tlkpJobPurchaseOrderInstruction Add(tlkpJobPurchaseOrderInstruction dataAdded)
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

            string sql = "INSERT INTO tlkpJobPurchaseOrderInstructions ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataAdded, "InstructionKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int poKeyGenerated = 0;

            try
            {
                poKeyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            tlkpJobPurchaseOrderInstruction data = GettlkpJobPOInstruction(poKeyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public tlkpJobPurchaseOrderInstruction Updated(tlkpJobPurchaseOrderInstruction dataUpdated)
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

            string sql = "UPDATE tlkpJobPurchaseOrderInstructions SET {0} WHERE InstructionKey = @id";

            EnumExtension.setUpdateValues(dataUpdated, "InstructionKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.InstructionKey);

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

            tlkpJobPurchaseOrderInstruction data = GettlkpJobPOInstruction(dataUpdated.InstructionKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public tlkpJobPurchaseOrderInstruction GettlkpJobPOInstruction(int id)
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

            tlkpJobPurchaseOrderInstruction job;
            try
            {
                job = GettlkpJobPOInstruction(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return job;
        }

        private tlkpJobPurchaseOrderInstruction GettlkpJobPOInstruction(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, b.ITextMemo as x_ITextMemo 
                            FROM tlkpJobPurchaseOrderInstructions a 
                                INNER JOIN tlkpJobPurchaseOrderInstructionsText b ON a.InstructionKey = b.ITextInstructionKey
                            WHERE a.InstructionKey = @key";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<tlkpJobPurchaseOrderInstruction> data = EnumExtension.ToList<tlkpJobPurchaseOrderInstruction>(dt);

            return data.FirstOrDefault<tlkpJobPurchaseOrderInstruction>();
        }

        public bool Remove(tlkpJobPurchaseOrderInstruction dataDeleted)
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
                result = RemovetlkpJobPOInstruction(dataDeleted, oConn);
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

        private bool RemovetlkpJobPOInstruction(tlkpJobPurchaseOrderInstruction dataDeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tlkpJobPurchaseOrderInstructions WHERE InstructionKey = @key";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(dataDeleted.InstructionKey);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion Instructions

        #region Job Purchase Order Status History
        public IList<JobPurchaseOrderStatusHistory> GetListJobPurchaseOrderStatusHistory(int POKey)
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

            string sql = @"WITH qData
                            AS  
                           ( 
                             select a.*,b.StatusText as x_Status, 
                             ROW_NUMBER() OVER (ORDER BY a.POStatusDate DESC) as row
                             from tblJobPurchaseOrderStatusHistory a INNER JOIN tlkpStatus b on a.POStatusStatusKey = b.StatusKey  
                             where a.POStatusPOKey = @key
                           )
                           SELECT *  
                           FROM qData
                           ORDER BY row";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = POKey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobPurchaseOrderStatusHistory> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<JobPurchaseOrderStatusHistory>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public JobPurchaseOrderStatusHistory GetJobPurchaseOrderStatusHistory(int id)
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

            JobPurchaseOrderStatusHistory job;
            try
            {
                job = GetJobPurchaseOrderStatusHistory(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return job;
        }

        private JobPurchaseOrderStatusHistory GetJobPurchaseOrderStatusHistory(int id, SqlConnection oConn)
        {
            string sql = @"WITH qData
                            AS  
                           ( 
                             select a.*,b.StatusText as x_Status, 
                             ROW_NUMBER() OVER (ORDER BY a.POStatusDate DESC) as row
                             from tblJobPurchaseOrderStatusHistory a INNER JOIN tlkpStatus b on a.POStatusStatusKey = b.StatusKey  
                             where a.POStatusKey = @key
                           )
                           SELECT *  
                           FROM qData
                           ORDER BY row";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobPurchaseOrderStatusHistory> data = EnumExtension.ToList<JobPurchaseOrderStatusHistory>(dt);

            return data.FirstOrDefault<JobPurchaseOrderStatusHistory>();
        }

        public JobPurchaseOrderStatusHistory Add(JobPurchaseOrderStatusHistory dataAdded)
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

            string sql = "INSERT INTO tblJobPurchaseOrderStatusHistory ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            dataAdded.POStatusModifiedDate = DateTime.Now;

            EnumExtension.setListValues(dataAdded, "POStatusKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int poKeyGenerated = 0;

            try
            {
                poKeyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            JobPurchaseOrderStatusHistory data = GetJobPurchaseOrderStatusHistory(poKeyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public JobPurchaseOrderStatusHistory Update(JobPurchaseOrderStatusHistory dataUpdated)
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

            string sql = "UPDATE tblJobPurchaseOrderStatusHistory SET {0} WHERE POStatusKey = @id";

            dataUpdated.POStatusModifiedDate = DateTime.Now;
            EnumExtension.setUpdateValues(dataUpdated, "POStatusKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.POStatusKey);

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

            JobPurchaseOrderStatusHistory data = GetJobPurchaseOrderStatusHistory(dataUpdated.POStatusKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(JobPurchaseOrderStatusHistory dataDeleted)
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
                result = Remove(dataDeleted, oConn);
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

        private bool Remove(JobPurchaseOrderStatusHistory dataDeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblJobPurchaseOrderStatusHistory WHERE POStatusKey = @key";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = dataDeleted.POStatusKey;

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public qfrmJobPurchaseOrderStatusHistory GetqfrmJobPurchaseOrderStatusHistory(int POKey)
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

            string sql = @"SELECT *
                           FROM qfrmJobPurchaseOrderStatusHistory
                           WHERE POKey = @key";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = POKey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            qfrmJobPurchaseOrderStatusHistory data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<qfrmJobPurchaseOrderStatusHistory>(dt).FirstOrDefault();
            }
            else
            {
                return null;
            }

            return data;
        }
        #endregion Purchase Order Status History

        #region Invoice Header
        public IList<InvoiceHeader> GetListInvoiceHeader(int JobKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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
            string where = (JobKey > 0) ? "InvoiceJobKey = @jobkey" : "1=1";

            if (!string.IsNullOrEmpty(query))
            {
                string whereQuery = "";

                string fieldName = "InvoiceNum";
                whereQuery += EnumExtension.generateLikeWhere(query, fieldName);

                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    "(" + whereQuery + ")";
            }

            #region Ordenamiento
            string order = "InvoiceNum";
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
                        SELECT a.*, d.CustKey, d.CustPeachtreeID, 
                            d.CustPeachtreeIndex, d.CustName, d.CustAddress1, d.CustAddress2, d.CustCity, d.CustState, 
                            d.CustZip, d.CustCountryKey, d.CustPhone, d.CustFax, d.CustEmail, d.CustWebsite, 
                            d.CustSalesRepKey, d.CustOrdersRepKey, d.CustLanguageCode, d.CustStatus, d.CustModifiedBy, 
                            d.CustModifiedDate, d.CustCreatedBy, d.CustCreatedDate, d.CustCreditLimit, d.CustCurrencyCode, d.CustMemo, 
                            b.VendorKey, b.VendorName, b.VendorPeachtreeID, b.VendorPeachtreeItemID, b.VendorPeachtreeJobID, b.VendorDisplayToCust, 
                            b.VendorContact, b.VendorAddress1, b.VendorAddress2, b.VendorCity, b.VendorState, b.VendorZip, 
                            b.VendorCountryKey, b.VendorPhone, b.VendorFax, b.VendorEmail, b.VendorWebsite, b.VendorLanguageCode, 
                            b.VendorAcctNum, b.VendorCarrier, b.VendorModifiedBy, b.VendorModifiedDate, b.VendorCreatedBy, b.VendorCreatedDate, 
                            b.VendorDefaultCommissionPercent, c.JobShipType, c.JobShipDate, dbo.fnGetInvoiceNum(a.InvoiceKey) AS FullInvoiceNum
                        FROM tblInvoiceHeader a LEFT OUTER JOIN
	                        tblVendors b ON a.InvoiceVendorKey = b.VendorKey LEFT OUTER JOIN
	                        tblJobHeader c ON a.InvoiceJobKey = c.JobKey LEFT OUTER JOIN
	                        tblCustomers d ON a.InvoiceCustKey = d.CustKey
                        WHERE {0}
                        )
                        SELECT *
                        FROM 
                        (
                        SELECT a.*,ROW_NUMBER() OVER (ORDER BY {1} {2}) as row,
                            b.TotalRecords
                        FROM qData a INNER JOIN (SELECT COUNT(*) AS TotalRecords FROM qData) AS b on 1=1) a
                        WHERE row > @start and row <= @limit";

            sql = String.Format(sql, where, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            if (JobKey > 0) da.SelectCommand.Parameters.Add("@jobkey", SqlDbType.Int).Value = Convert.ToInt32(JobKey);

            da.SelectCommand.Parameters.Add("@start", SqlDbType.Int).Value = Convert.ToInt32(start);
            da.SelectCommand.Parameters.Add("@limit", SqlDbType.Int).Value = Convert.ToInt32(limit);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<InvoiceHeader> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<InvoiceHeader>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public IList<InvoiceDDL> GetListInvoiceDDL(int JobKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords)
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
            string where = (JobKey > 0) ? "InvoiceJobKey = @jobkey" : "1=1";

            if (!string.IsNullOrEmpty(query))
            {
                string whereQuery = "";

                string fieldName = "dbo.fnGetInvoiceNum(InvoiceKey)";
                whereQuery += EnumExtension.generateLikeWhere(query, fieldName);

                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    "(" + whereQuery + ")";
            }

            #region Ordenamiento
            string order = "x_InvoiceNum";
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
                            SELECT InvoiceKey, dbo.fnGetInvoiceNum(InvoiceKey) AS x_InvoiceNum 
                            FROM tblInvoiceHeader
                            WHERE {0}
                        )
                        SELECT *
                        FROM 
                        (
                        SELECT a.*,ROW_NUMBER() OVER (ORDER BY {1} {2}) as row,
                            b.TotalRecords
                        FROM qData a INNER JOIN (SELECT COUNT(*) AS TotalRecords FROM qData) AS b on 1=1) a
                        WHERE row > @start and row <= @limit";

            sql = String.Format(sql, where, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            if (JobKey > 0) da.SelectCommand.Parameters.Add("@jobkey", SqlDbType.Int).Value = Convert.ToInt32(JobKey);

            da.SelectCommand.Parameters.Add("@start", SqlDbType.Int).Value = Convert.ToInt32(start);
            da.SelectCommand.Parameters.Add("@limit", SqlDbType.Int).Value = Convert.ToInt32(limit);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<InvoiceDDL> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<InvoiceDDL>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public InvoiceHeader Add(InvoiceHeader dataAdded)
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

            string sql = "INSERT INTO tblInvoiceHeader ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataAdded, "InvoiceKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int poKeyGenerated = 0;

            try
            {
                poKeyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
                //setJobNum(poKeyGenerated, dataAdded.JobYear, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            InvoiceHeader data = GetInvoiceHeader(poKeyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public InvoiceHeader Update(InvoiceHeader dataUpdated)
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

            string sql = "UPDATE tblInvoiceHeader SET {0} WHERE InvoiceKey = @key";

            EnumExtension.setUpdateValues(dataUpdated, "InvoiceKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.InvoiceKey);

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

            InvoiceHeader data = GetInvoiceHeader(dataUpdated.InvoiceKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public InvoiceHeader GetInvoiceHeader(int id)
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

            InvoiceHeader job;
            try
            {
                job = GetInvoiceHeader(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return job;
        }

        private InvoiceHeader GetInvoiceHeader(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, d.CustKey, d.CustPeachtreeID, 
                            d.CustPeachtreeIndex, d.CustName, d.CustAddress1, d.CustAddress2, d.CustCity, d.CustState, 
                            d.CustZip, d.CustCountryKey, d.CustPhone, d.CustFax, d.CustEmail, d.CustWebsite, 
                            d.CustSalesRepKey, d.CustOrdersRepKey, d.CustLanguageCode, d.CustStatus, d.CustModifiedBy, 
                            d.CustModifiedDate, d.CustCreatedBy, d.CustCreatedDate, d.CustCreditLimit, d.CustCurrencyCode, d.CustMemo, 
                            b.VendorKey, b.VendorName, b.VendorPeachtreeID, b.VendorPeachtreeItemID, b.VendorPeachtreeJobID, b.VendorDisplayToCust, 
                            b.VendorContact, b.VendorAddress1, b.VendorAddress2, b.VendorCity, b.VendorState, b.VendorZip, 
                            b.VendorCountryKey, b.VendorPhone, b.VendorFax, b.VendorEmail, b.VendorWebsite, b.VendorLanguageCode, 
                            b.VendorAcctNum, b.VendorCarrier, b.VendorModifiedBy, b.VendorModifiedDate, b.VendorCreatedBy, b.VendorCreatedDate, 
                            b.VendorDefaultCommissionPercent, c.JobShipType, c.JobShipDate, dbo.fnGetInvoiceNum(a.InvoiceKey) AS FullInvoiceNum
                        FROM tblInvoiceHeader a LEFT OUTER JOIN
	                        tblVendors b ON a.InvoiceVendorKey = b.VendorKey LEFT OUTER JOIN
	                        tblJobHeader c ON a.InvoiceJobKey = c.JobKey LEFT OUTER JOIN
	                        tblCustomers d ON a.InvoiceCustKey = d.CustKey
                            WHERE (InvoiceKey = @key)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<InvoiceHeader> data = EnumExtension.ToList<InvoiceHeader>(dt);

            return data.FirstOrDefault<InvoiceHeader>();
        }

        public bool Remove(InvoiceHeader dataDeleted)
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
                result = Remove(dataDeleted, oConn);
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

        private bool Remove(InvoiceHeader dataDeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblInvoiceHeader " +
                         " WHERE InvoiceKey = @key";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(dataDeleted.InvoiceKey);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        private IList<InvoiceHeader> GetListInvoicesByJob(int JobKey, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, d.CustKey, d.CustPeachtreeID, 
                            d.CustPeachtreeIndex, d.CustName, d.CustAddress1, d.CustAddress2, d.CustCity, d.CustState, 
                            d.CustZip, d.CustCountryKey, d.CustPhone, d.CustFax, d.CustEmail, d.CustWebsite, 
                            d.CustSalesRepKey, d.CustOrdersRepKey, d.CustLanguageCode, d.CustStatus, d.CustModifiedBy, 
                            d.CustModifiedDate, d.CustCreatedBy, d.CustCreatedDate, d.CustCreditLimit, d.CustCurrencyCode, d.CustMemo, 
                            b.VendorKey, b.VendorName, b.VendorPeachtreeID, b.VendorPeachtreeItemID, b.VendorPeachtreeJobID, b.VendorDisplayToCust, 
                            b.VendorContact, b.VendorAddress1, b.VendorAddress2, b.VendorCity, b.VendorState, b.VendorZip, 
                            b.VendorCountryKey, b.VendorPhone, b.VendorFax, b.VendorEmail, b.VendorWebsite, b.VendorLanguageCode, 
                            b.VendorAcctNum, b.VendorCarrier, b.VendorModifiedBy, b.VendorModifiedDate, b.VendorCreatedBy, b.VendorCreatedDate, 
                            b.VendorDefaultCommissionPercent, c.JobShipType, c.JobShipDate, dbo.fnGetInvoiceNum(a.InvoiceKey) AS FullInvoiceNum
                        FROM tblInvoiceHeader a LEFT OUTER JOIN
	                        tblVendors b ON a.InvoiceVendorKey = b.VendorKey LEFT OUTER JOIN
	                        tblJobHeader c ON a.InvoiceJobKey = c.JobKey LEFT OUTER JOIN
	                        tblCustomers d ON a.InvoiceCustKey = d.CustKey
                            WHERE (InvoiceJobKey = @JobKey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@JobKey", SqlDbType.Int).Value = Convert.ToInt32(JobKey);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            var data = EnumExtension.ToList<InvoiceHeader>(dt);

            return data;
        }
        #endregion Invoice Header

        #region Invoice Items Summary
        public IList<InvoiceItemsSummary> GetListInvoiceItemsSummary(int InvoiceKey)
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
            string where = "(ISummaryInvoiceKey = @invoicekey)";

            string sql = @"SELECT a.*, b.VendorName as x_ItemVendorName
                            FROM tblInvoiceItemsSummary a LEFT OUTER JOIN tblVendors b ON a.ISummaryVendorKey = b.VendorKey
                            WHERE {0}";

            sql = String.Format(sql, where);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@invoicekey", SqlDbType.Int).Value = Convert.ToInt32(InvoiceKey);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<InvoiceItemsSummary> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<InvoiceItemsSummary>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public InvoiceItemsSummary Add(InvoiceItemsSummary dataAdded)
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

            string sql = "INSERT INTO tblInvoiceItemsSummary ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataAdded, "ISummaryKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int poKeyGenerated = 0;

            try
            {
                poKeyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            InvoiceItemsSummary data = GetInvoiceItemsSummary(poKeyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public InvoiceItemsSummary Update(InvoiceItemsSummary dataUpdated)
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

            string sql = "UPDATE tblInvoiceItemsSummary SET {0} WHERE ISummaryKey = @ISummaryKey";

            EnumExtension.setUpdateValues(dataUpdated, "ISummaryKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@ISummaryKey", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.ISummaryKey);

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

            InvoiceItemsSummary data = GetInvoiceItemsSummary(dataUpdated.ISummaryKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public InvoiceItemsSummary GetInvoiceItemsSummary(int id)
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

            InvoiceItemsSummary itemSummary;
            try
            {
                itemSummary = GetInvoiceItemsSummary(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return itemSummary;
        }

        private InvoiceItemsSummary GetInvoiceItemsSummary(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, b.VendorName as x_ItemVendorName
                            FROM tblInvoiceItemsSummary a LEFT OUTER JOIN tblVendors b on a.ISummaryVendorKey = b.VendorKey
                            WHERE (ISummaryKey = @key)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<InvoiceItemsSummary> data = EnumExtension.ToList<InvoiceItemsSummary>(dt);

            return data.FirstOrDefault<InvoiceItemsSummary>();
        }

        public bool Remove(InvoiceItemsSummary dataDeleted)
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
                result = Remove(dataDeleted, oConn);
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

        private bool Remove(InvoiceItemsSummary dataDeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblInvoiceItemsSummary WHERE ISummaryKey = @key";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(dataDeleted.ISummaryKey);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion Invoice Items Summary

        #region Invoice Charges
        public IList<InvoiceCharge> GetListInvoiceCharge(int InvoiceKey)
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

            string sql = @"WITH qData AS
                           ( 
                           SELECT a.*, 
                                ROUND(a.IChargeCost / a.IChargeQty,2) AS x_UnitCost, 
                                ROUND(a.IChargePrice / a.IChargeQty,2) AS x_UnitPrice,
                               ISNULL(b.CurrencyDescription,'') as x_ChargeCurrency, 
                               ISNULL(c.DescriptionText,'') as x_ChargeDescription
                           FROM tblInvoiceCharges a 
                               LEFT OUTER JOIN tblCurrencyRates b ON a.IChargeCurrencyCode=b.CurrencyCode 
                               LEFT OUTER JOIN 
                               (SELECT a.ChargeKey,MIN(b.DescriptionText) as DescriptionText FROM tlkpChargeCategories a 
                                   INNER JOIN tlkpChargeCategoryDescriptions b ON a.ChargeKey = b.DescriptionChargeKey 
                               WHERE (a.ChargeAPAccount IS NOT NULL) 
                               GROUP BY a.ChargeKey) as c ON a.IChargeChargeKey = c.ChargeKey 
                           WHERE (IChargeInvoiceKey = @invoicekey)
                            )  
                            SELECT *
                            FROM qData";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@invoicekey", SqlDbType.Int).Value = Convert.ToInt32(InvoiceKey);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<InvoiceCharge> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<InvoiceCharge>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public InvoiceCharge Add(InvoiceCharge dataAdded)
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

            string sql = "INSERT INTO tblInvoiceCharges ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(dataAdded, "IChargeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int poKeyGenerated = 0;

            try
            {
                poKeyGenerated = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            InvoiceCharge data = GetInvoiceCharge(poKeyGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public InvoiceCharge Update(InvoiceCharge dataUpdated)
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

            string sql = "UPDATE tblInvoiceCharges SET {0} WHERE IChargeKey = @IChargeKey";

            EnumExtension.setUpdateValues(dataUpdated, "IChargeKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@IChargeKey", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.IChargeKey);

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

            InvoiceCharge data = GetInvoiceCharge(dataUpdated.IChargeKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public InvoiceCharge GetInvoiceCharge(int id)
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

            InvoiceCharge itemSummary;
            try
            {
                itemSummary = GetInvoiceCharge(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return itemSummary;
        }

        private InvoiceCharge GetInvoiceCharge(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, 
                                ROUND(a.IChargeCost / a.IChargeQty,2) AS x_UnitCost, 
                                ROUND(a.IChargePrice / a.IChargeQty,2) AS x_UnitPrice, 
                               ISNULL(c.DescriptionText,'') as x_ChargeDescription
                           FROM tblInvoiceCharges a 
                               LEFT OUTER JOIN tblCurrencyRates b ON a.IChargeCurrencyCode=b.CurrencyCode 
                               LEFT OUTER JOIN 
                               (SELECT a.ChargeKey,MIN(b.DescriptionText) as DescriptionText FROM tlkpChargeCategories a 
                                   INNER JOIN tlkpChargeCategoryDescriptions b ON a.ChargeKey = b.DescriptionChargeKey 
                               WHERE (a.ChargeAPAccount IS NOT NULL) 
                               GROUP BY a.ChargeKey) as c ON a.IChargeChargeKey = c.ChargeKey 
                           WHERE (IChargeInvoiceKey = @key)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<InvoiceCharge> data = EnumExtension.ToList<InvoiceCharge>(dt);

            return data.FirstOrDefault<InvoiceCharge>();
        }

        public bool Remove(InvoiceCharge dataDeleted)
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
                result = Remove(dataDeleted, oConn);
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

        private bool Remove(InvoiceCharge dataDeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblInvoiceCharges WHERE IChargeKey = @key";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(dataDeleted.IChargeKey);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion Invoice Charges

        #region Invoice Charges SubTotal
        public IList<InvoiceChargesSubTotal> GetListInvoiceChargesSubTotal(int InvoiceKey)
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

            string sql = @"SELECT a.*,d.STDescriptionText as x_Category, c.ListText as x_Location, ISNULL(b.SubTotalSort,'') as x_SubTotalSort
                           FROM tblInvoiceChargesSubTotals a 
                               INNER JOIN tblInvoiceHeader h ON a.ISTInvoiceKey=h.InvoiceKey
	                           INNER JOIN tblCustomers cust ON h.InvoiceCustKey = cust.CustKey
                               LEFT OUTER JOIN tlkpInvoiceSubTotalCategories b ON a.ISTSubTotalKey=b.SubTotalKey 
                               LEFT OUTER JOIN tlkpGenericLists c ON a.ISTLocation=c.ListKey 
                               LEFT OUTER JOIN tlkpInvoiceSubTotalCategoriesDescriptions d 
                                on a.ISTSubTotalKey=d.STDescriptionSubTotalKey and d.STDescriptionLanguageCode = cust.CustLanguageCode
                            WHERE (ISTInvoiceKey = @invoicekey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@invoicekey", SqlDbType.Int).Value = Convert.ToInt32(InvoiceKey);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<InvoiceChargesSubTotal> data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<InvoiceChargesSubTotal>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public InvoiceChargesSubTotal Add(InvoiceChargesSubTotal dataAdded)
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

            string sql = "INSERT INTO tblInvoiceChargesSubTotals ({0}) VALUES ({1})";

            EnumExtension.setListValues(dataAdded, "", ref sql);

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

            InvoiceChargesSubTotal data = GetInvoiceChargesSubTotal(dataAdded.ISTSubTotalKey, dataAdded.ISTInvoiceKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public InvoiceChargesSubTotal Update(InvoiceChargesSubTotal dataUpdated)
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

            string sql = "UPDATE tblInvoiceChargesSubTotals SET {0} WHERE ISTSubTotalKey = @ISTSubTotalKey AND ISTInvoiceKey = @ISTInvoiceKey";

            EnumExtension.setUpdateValues(dataUpdated, "", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@ISTSubTotalKey", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.ISTSubTotalKey);
            cmd.Parameters.Add("@ISTInvoiceKey", SqlDbType.Int).Value = Convert.ToInt32(dataUpdated.ISTInvoiceKey);

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

            InvoiceChargesSubTotal data = GetInvoiceChargesSubTotal(dataUpdated.ISTSubTotalKey, dataUpdated.ISTInvoiceKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public InvoiceChargesSubTotal GetInvoiceChargesSubTotal(int ISTSubTotalKey, int ISTInvoiceKey)
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

            InvoiceChargesSubTotal itemSummary;
            try
            {
                itemSummary = GetInvoiceChargesSubTotal(ISTSubTotalKey, ISTInvoiceKey, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return itemSummary;
        }

        private InvoiceChargesSubTotal GetInvoiceChargesSubTotal(int ISTSubTotalKey, int ISTInvoiceKey, SqlConnection oConn)
        {
            string sql = @"SELECT *
                            FROM tblInvoiceChargesSubTotals  
                            WHERE (ISTSubTotalKey = @ISTSubTotalKey AND ISTInvoiceKey = @ISTInvoiceKey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@ISTSubTotalKey", SqlDbType.Int).Value = Convert.ToInt32(ISTSubTotalKey);
            da.SelectCommand.Parameters.Add("@ISTInvoiceKey", SqlDbType.Int).Value = Convert.ToInt32(ISTInvoiceKey);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<InvoiceChargesSubTotal> data = EnumExtension.ToList<InvoiceChargesSubTotal>(dt);

            return data.FirstOrDefault<InvoiceChargesSubTotal>();
        }

        public bool Remove(InvoiceChargesSubTotal dataDeleted)
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
                result = Remove(dataDeleted, oConn);
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

        private bool Remove(InvoiceChargesSubTotal dataDeleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblInvoiceChargesSubTotals WHERE ISTSubTotalKey = @key";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(dataDeleted.ISTSubTotalKey);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion Invoice Charges SubTotal

        #region Split BackOrder
        public bool SplitBackOrder(int POKey, string currentUser)
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

            var POItemsTarget = GetJobPurchaseOrderItems(POKey);
            POItemsTarget = (from p in POItemsTarget
                             where p.POItemsBackorderQty > 0
                             select p).ToList();


            JobPurchaseOrder POSource = GetJobPurchaseOrder(POKey, oConn);
            JobPurchaseOrder POTarget = new JobPurchaseOrder();

            POTarget.PONum = POSource.PONum;
            POTarget.PONumShipment = MaxShipmentNum(POSource, oConn) + 1;
            POTarget.POJobKey = POSource.POJobKey;
            POTarget.POVendorKey = POSource.POVendorKey;
            POTarget.POVendorContactKey = POSource.POVendorContactKey;
            POTarget.POVendorReference = POSource.POVendorReference;
            POTarget.POVendorOriginAddress = POSource.POVendorOriginAddress;
            POTarget.PODate = DateTime.Now;
            POTarget.POGoodThruDate = POTarget.PODate.AddDays(30);
            POTarget.POLeadTime = "On Backorder";
            POTarget.PODefaultProfitMargin = POSource.PODefaultProfitMargin;
            POTarget.POVendorPaymentTerms = POSource.POVendorPaymentTerms;
            POTarget.POCurrencyCode = POSource.POCurrencyCode;
            POTarget.POCurrencyRate = POSource.POCurrencyRate;
            POTarget.POShipmentType = POSource.POShipmentType;
            POTarget.POFreightDestination = POSource.POFreightDestination;
            POTarget.POFreightDestinationZip = POSource.POFreightDestinationZip;
            POTarget.POCustShipKey = POSource.POCustShipKey;
            POTarget.POWarehouseKey = POSource.POWarehouseKey;
            POTarget.POCreatedBy = currentUser;
            POTarget.POCreatedDate = DateTime.Now;

            POTarget = Add(POTarget);

            foreach (JobPurchaseOrderItem item in POItemsTarget)
            {
                if (item.POItemsBackorderQty == item.POItemsQty)
                {
                    //'*** Moving all items, just change the POKey
                    item.POItemsPOKey = POTarget.POKey;
                    item.POItemsBackorderQty = null;
                    Updated(item);
                }
                else
                {
                    JobPurchaseOrderItem POItemTarget = new JobPurchaseOrderItem();
                    POItemTarget.POItemsJobKey = item.POItemsJobKey;
                    POItemTarget.POItemsPOKey = POTarget.POKey;
                    POItemTarget.POItemsSort = item.POItemsSort;
                    POItemTarget.POItemsQty = item.POItemsBackorderQty.GetValueOrDefault();
                    POItemTarget.POItemsItemKey = item.POItemsItemKey;
                    POItemTarget.POItemsCost = item.POItemsCost;
                    POItemTarget.POItemsPrice = item.POItemsPrice;
                    POItemTarget.POItemsLineCost = item.POItemsCost * item.POItemsBackorderQty.GetValueOrDefault();
                    POItemTarget.POItemsLinePrice = item.POItemsPrice * item.POItemsBackorderQty.GetValueOrDefault();
                    POItemTarget.POItemsCurrencyCode = item.POItemsCurrencyCode;
                    POItemTarget.POItemsCurrencyRate = item.POItemsCurrencyRate;
                    POItemTarget.POItemsWeight = item.POItemsWeight;
                    POItemTarget.POItemsVolume = item.POItemsVolume;
                    POItemTarget.POItemsLineWeight = item.POItemsWeight * item.POItemsBackorderQty.GetValueOrDefault();
                    POItemTarget.POItemsLineVolume = item.POItemsVolume * item.POItemsBackorderQty.GetValueOrDefault();
                    POItemTarget.POItemsMemoCustomer = item.POItemsMemoCustomer;
                    POItemTarget.POItemsMemoCustomerMoveBottom = item.POItemsMemoCustomerMoveBottom;
                    POItemTarget.POItemsMemoVendor = item.POItemsMemoVendor;
                    POItemTarget.POItemsMemoVendorMoveBottom = item.POItemsMemoVendorMoveBottom;
                    POItemTarget = Add(POItemTarget);

                    item.POItemsQty = item.POItemsQty - item.POItemsBackorderQty.GetValueOrDefault();
                    item.POItemsLineCost = item.POItemsCost * item.POItemsQty;
                    item.POItemsLinePrice = item.POItemsPrice * item.POItemsQty;
                    item.POItemsBackorderQty = null;
                    Updated(item);
                }
            }

            JobPurchaseOrderStatusHistory POStatusHistory = new JobPurchaseOrderStatusHistory();

            POStatusHistory.POStatusJobKey = POSource.POJobKey;
            POStatusHistory.POStatusPOKey = POKey;
            POStatusHistory.POStatusStatusKey = 69;
            POStatusHistory.POStatusDate = DateTime.Now;
            POStatusHistory.POStatusMemo = "New Purchase Order: " + POTarget.x_JobNumFormatted;

            Add(POStatusHistory);

            ConnManager.CloseConn(oConn);

            return true;
        }

        public int MaxShipmentNum(JobPurchaseOrder POSource, SqlConnection oConn)
        {
            //SELECT Max(PONumShipment) As MaxShipmentNum FROM tblJobPurchaseOrders WHERE PONum

            string sql = "SELECT Max(PONumShipment) As MaxShipmentNum FROM tblJobPurchaseOrders WHERE PONum = @PONum";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@PONum", SqlDbType.Int).Value = POSource.PONum;

            int number = Convert.ToInt32(cmd.ExecuteScalar());

            return number;
        }
        #endregion Split BackOrder

        #region Create New Job Invoice
        public bool CreateNewJobInvoice(int JobKey, string currentUser, List<int> SelectedItems)
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

            IUserRepository usrRepository = new UserRepository();
            User User = usrRepository.Get(currentUser);

            ICustomersRepository cusRepository = new CustomersRepository();
            JobHeader JobHeader = Get(JobKey);
            Customer Customer = cusRepository.Get(JobHeader.JobCustKey);

            InvoiceHeader InvoiceHeader = new InvoiceHeader();
            InvoiceHeader.InvoiceJobKey = JobKey;
            InvoiceHeader.InvoiceDate = DateTime.Now;
            InvoiceHeader.InvoicePrefix = GetInvoicePrefix(InvoiceHeader.InvoiceDate);
            InvoiceHeader.InvoiceNum = GetNewInvoiceNum(InvoiceHeader.InvoicePrefix, oConn);
            InvoiceHeader.InvoiceCustKey = JobHeader.JobCustKey;
            InvoiceHeader.InvoiceCustContactKey = JobHeader.JobContactKey;
            InvoiceHeader.InvoiceBillingName = Customer.CustName;
            InvoiceHeader.InvoiceBillingAddress1 = Customer.CustAddress1;
            InvoiceHeader.InvoiceBillingAddress2 = Customer.CustAddress2;
            InvoiceHeader.InvoiceBillingCity = Customer.CustCity;
            InvoiceHeader.InvoiceBillingState = Customer.CustState;
            InvoiceHeader.InvoiceBillingZip = Customer.CustZip;
            InvoiceHeader.InvoiceBillingCountryKey = Customer.CustCountryKey;
            InvoiceHeader.InvoiceCustShipKey = JobHeader.JobCustShipKey;
            InvoiceHeader.InvoiceCustReference = JobHeader.JobCustRefNum;
            InvoiceHeader.InvoiceEmployeeKey = User.EmployeeKey;
            InvoiceHeader.InvoiceCurrencyCode = JobHeader.JobCustCurrencyCode;
            InvoiceHeader.InvoiceCurrencyRate = JobHeader.JobCustCurrencyRate;
            InvoiceHeader.InvoicePaymentTerms = JobHeader.JobCustPaymentTerms;

            // Insert New Invoice Header
            InvoiceHeader = Add(InvoiceHeader);

            // Purchase Orders Items Selected
            if (SelectedItems.Count > 0)
                SetInvoiceKeyOnJobPurchaseOrders(InvoiceHeader.InvoiceKey, SelectedItems, oConn);

            // Copy Charges from Quote
            CopyChargesFromQuote(InvoiceHeader.InvoiceKey, JobKey, oConn);

            ConnManager.CloseConn(oConn);

            return true;
        }

        public bool CreateNewCommissionInvoice(int JobKey, string currentUser, CommissionInvoice CommissionInvoice)
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

            IUserRepository usrRepository = new UserRepository();
            User User = usrRepository.Get(currentUser);

            ICustomersRepository cusRepository = new CustomersRepository();
            JobHeader JobHeader = Get(JobKey);
            Customer Customer = cusRepository.Get(JobHeader.JobCustKey);

            InvoiceHeader InvoiceHeader = new InvoiceHeader();
            InvoiceHeader.InvoiceJobKey = JobKey;
            InvoiceHeader.InvoiceDate = DateTime.Now;
            InvoiceHeader.InvoicePrefix = GetInvoicePrefix(InvoiceHeader.InvoiceDate);
            InvoiceHeader.InvoiceNum = GetNewInvoiceNum(InvoiceHeader.InvoicePrefix, oConn);
            InvoiceHeader.InvoiceRecipient = 1;
            InvoiceHeader.InvoiceVendorKey = CommissionInvoice.VendorKey;
            InvoiceHeader.InvoiceVendorContactKey = CommissionInvoice.VendorContactKey;
            InvoiceHeader.InvoiceEmployeeKey = User.EmployeeKey;
            InvoiceHeader.InvoiceCurrencyCode = CommissionInvoice.CommissionTotalCurrencyCode;
            InvoiceHeader.InvoiceCurrencyRate = CommissionInvoice.CommissionTotalCurrencyRate.GetValueOrDefault();
            InvoiceHeader.InvoicePaymentTerms = JobHeader.JobCustPaymentTerms; //(DLookup("JobCustPaymentTerms", "tblJobHeader", "JobKey = " & Me.JobKey), 55)

            // Insert New Invoice Header
            InvoiceHeader = Add(InvoiceHeader);

            // Copy General commisssion data
            CopyCommissionData(InvoiceHeader, Customer, CommissionInvoice, oConn);

            ConnManager.CloseConn(oConn);

            return true;
        }

        private void SetInvoiceKeyOnJobPurchaseOrders(int InvoiceKey, List<int> SelectedItems, SqlConnection oConn)
        {
            string sql = "UPDATE tblJobPurchaseOrders SET POInvoiceKey = {0} WHERE POKey = {1}";

            SqlCommand cmd = new SqlCommand(sql, oConn);

            foreach (int POKey in SelectedItems)
            {
                cmd.CommandText = string.Format(sql, InvoiceKey, POKey);
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();
        }

        private void CopyChargesFromQuote(int InvoiceKey, int JobKey, SqlConnection oConn)
        {
            string sql = @"SELECT QHdrKey
                            FROM tblFileQuoteHeader
                            WHERE QHdrJobKey = {0}";

            sql = string.Format(sql, JobKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int QHdrKey = Convert.ToInt32(cmd.ExecuteScalar());

            //If no quote, then copy charges from PO
            if (QHdrKey == 0)
            {
                sql = @"INSERT INTO tblInvoiceCharges (IChargeInvoiceKey, IChargeSort, IChargeChargeKey, IChargeMemo, IChargeQty, IChargeCost, IChargePrice, IChargeCurrencyCode, IChargeCurrencyRate) 
                      SELECT {0} AS InvoiceKey, POChargesSort, POChargesChargeKey, POChargesMemo, POChargesQty, POChargesCost, POChargesPrice, POChargesCurrencyCode, POChargesCurrencyRate FROM tblJobPurchaseOrderCharges WHERE POChargesPOKey IN (SELECT POKey FROM tblJobPurchaseOrders WHERE POInvoiceKey = {0})";

                sql = string.Format(sql, InvoiceKey);

            }
            else
            {
                // Copy the charges from the quote
                sql = @"INSERT INTO tblInvoiceCharges (IChargeInvoiceKey, IChargeSort, IChargeChargeKey, IChargeMemo, IChargeQty, IChargeCost, IChargePrice, IChargeCurrencyCode, IChargeCurrencyRate) 
                        SELECT {0} AS InvoiceKey, QChargeSort, QChargeChargeKey, QChargeMemo, QChargeQty, QChargeCost, QChargePrice, QChargeCurrencyCode, QChargeCurrencyRate FROM tblFileQuoteCharges WHERE QChargeHdrKey = {1} AND QChargePrint = 1 ORDER BY QChargeSort
                        GO
                        ";

                // Copy summary items from quote
                sql += @"INSERT INTO tblInvoiceItemsSummary (ISummaryInvoiceKey, ISummarySort, ISummaryQty, ISummaryVendorKey, ISummaryItemNum, ISummaryDescription, ISummaryPrice, ISummaryLinePrice, ISummaryCurrencyCode, ISummaryCurrencyRate) 
                        SELECT {0} AS InvoiceKey, QSummarySort, QSummaryQty, QSummaryVendorKey, QSummaryItemNum, QSummaryDescription, QSummaryPrice, QSummaryLinePrice, QSummaryCurrencyCode, QSummaryCurrencyRate FROM tblFileQuoteItemsSummary WHERE QSummaryQHdrKey = {1}
                        GO
                        ";

                // Copy subtotal from quote
                sql += @"INSERT INTO tblInvoiceChargesSubTotals (ISTInvoiceKey, ISTSubTotalKey, ISTLocation)
                        SELECT {0} AS InvoiceKey, QSTSubTotalKey, QSTLocation FROM tblFileQuoteChargesSubTotals WHERE QSTQHdrKey = {1}
                        GO
                        ";

                sql = string.Format(sql, InvoiceKey, QHdrKey);
            }

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            cmd.Dispose();
        }

        private void CopyCommissionData(InvoiceHeader Invoice, Customer Customer, CommissionInvoice Commission, SqlConnection oConn)
        {
            string sql = "";


            SqlCommand cmd = new SqlCommand(sql, oConn);


            //'*** Add the volume of this sale to the ItemsSummary table
            sql = @"INSERT INTO tblInvoiceItemsSummary (ISummaryInvoiceKey, ISummarySort, ISummaryQty, ISummaryVendorKey, 
                        ISummaryDescription, ISummaryPrice, ISummaryLinePrice, ISummaryCurrencyCode, ISummaryCurrencyRate) 
                    VALUES ({0}, 100, 1, {1}, 'Sale to {2}', {3}, {3}, '{4}', {5})
                    GO
                    ";

            sql = string.Format(sql, Invoice.InvoiceKey, Commission.VendorKey, Customer.CustName, Commission.TotalSale, Commission.TotalSaleCurrencyCode, Commission.TotalSaleCurrencyRate);

            //'*** Add the commission amount to the ItemsSummary table
            sql += @"INSERT INTO tblInvoiceItemsSummary (ISummaryInvoiceKey, ISummarySort, ISummaryQty, ISummaryVendorKey, 
                        ISummaryDescription, ISummaryPrice, ISummaryLinePrice, ISummaryCurrencyCode, ISummaryCurrencyRate) 
                    VALUES ( {0} , 200, 1, {1}, '{2}% Commission for sale', {3}, {3}, '{4}', {5})
                    GO
                    ";

            sql = string.Format(sql, Invoice.InvoiceKey, Commission.VendorKey, Commission.CommissionPCT, Commission.CommissionTotal, Commission.CommissionTotalCurrencyCode, Commission.CommissionTotalCurrencyRate);

            //'*** Add the credit to cancel volume amount to the InvoiceCharges table
            sql += @"INSERT INTO tblInvoiceCharges (IChargeInvoiceKey, IChargeSort, IChargeChargeKey, IChargeQty, 
                        IChargeCost, IChargePrice, IChargeCurrencyCode, IChargeCurrencyRate) 
                        VALUES ( {0}, 100, 15, 1, {1} * -1 , {1} * -1, '{2}', {3})
                    GO
                    ";
            sql = string.Format(sql, Invoice.InvoiceKey, Commission.TotalSale, Commission.TotalSaleCurrencyCode, Commission.TotalSaleCurrencyRate);

            sql += @"UPDATE tblVendors SET VendorDefaultCommissionPercent = {0}  WHERE VendorKey = {1}
                    GO
                    ";
            sql = string.Format(sql, Commission.CommissionPCT, Commission.VendorKey);

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            cmd.Dispose();
        }

        private int GetInvoicePrefix(DateTime date)
        {
            string prefix = "";
            string strCentury = date.Year.ToString().Substring(0, 2);
            string strYear = date.Year.ToString().Substring(2);
            if (strCentury.Substring(1) == "0")
            {
                strCentury = strCentury.Substring(0, 1);
                strYear = int.Parse(strYear).ToString();
            }

            prefix = strCentury + strYear;

            return int.Parse(prefix);
        }

        private int GetNewInvoiceNum(int Prefix, SqlConnection oConn)
        {
            string sql = @"SELECT TOP 1 InvoiceNum
                            FROM tblInvoiceHeader
                            WHERE InvoicePrefix = {0} 
                            ORDER BY InvoiceNum DESC";

            sql = string.Format(sql, Prefix);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int last = Convert.ToInt32(cmd.ExecuteScalar());

            return last + 1;

            //If Prefix = 0 Then      '*** Prefix not specified, find prefix for today's date
            //    Prefix = GetInvoicePrefix()
            //End If
        }
        #endregion Create New Job Invoice

        #region Job Status History
        public JobStatusHistory GetJobStatusHistoryById(int id)
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

            string sql = @"select a.*,b.StatusText as x_Status, 
                             ROW_NUMBER() OVER (ORDER BY a.JobStatusDate DESC) as row
                             from tblJobStatusHistory a INNER JOIN tlkpStatus b on a.JobStatusStatusKey = b.StatusKey  
                             where a.JobStatusKey = @key ";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = id;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobStatusHistory> lista;

            if (dt.Rows.Count > 0)
            {
                lista = EnumExtension.ToList<JobStatusHistory>(dt);

            }
            else
            {
                return null;
            }

            return lista.FirstOrDefault<JobStatusHistory>();
        }

        public JobStatusHistory GetJobStatusHistoryById(int id, SqlConnection oConn)
        {
            string sql = @"WITH qData
                            AS  
                           ( 
                             select a.*,b.StatusText as x_Status, 
                             ROW_NUMBER() OVER (ORDER BY a.JobStatusDate DESC) as row
                             from tblJobStatusHistory a INNER JOIN tlkpStatus b on a.JobStatusStatusKey = b.StatusKey  
                             where a.JobStatusKey = @key
                           )
                           SELECT *  
                           FROM qData
                           ORDER BY row";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = id;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobStatusHistory> lista;

            if (dt.Rows.Count > 0)
            {
                lista = EnumExtension.ToList<JobStatusHistory>(dt);

            }
            else
            {
                return null;
            }

            return lista.FirstOrDefault<JobStatusHistory>();
        }

        public JobStatusHistory AddJobHistory(JobStatusHistory added)
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

            string sql = "INSERT INTO tblJobStatusHistory ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            added.JobStatusModifiedDate = DateTime.Now;

            EnumExtension.setListValues(added, "JobStatusKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int keyValue = added.JobStatusKey;

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

            JobStatusHistory data = GetJobStatusHistoryById(keyValue, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public JobStatusHistory UpdateJobHistory(JobStatusHistory updated)
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

            string sql = "UPDATE tblJobStatusHistory SET {0} WHERE JobStatusKey = @key";

            EnumExtension.setUpdateValues(updated, "QStatusKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = updated.JobStatusKey;

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

            JobStatusHistory data = GetJobStatusHistoryById(updated.JobStatusKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool RemoveJobHistory(JobStatusHistory deleted)
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
                result = RemoveJobHistory(deleted, oConn);
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

        private bool RemoveJobHistory(JobStatusHistory deleted, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblJobStatusHistory " +
                         " WHERE (JobStatusKey = @key)";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@key", SqlDbType.Int).Value = deleted.JobStatusKey;

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public IList<JobStatusHistorySubDetails> GetJobStatusHistorySubDetails(int JobKey, int page, int start, int limit, ref int totalRecords)
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
                               SELECT  a.*, c.StatusText as x_Status
                               FROM dbo.qfrmJobStatusHistorySubDetails a 
                                INNER JOIN tlkpStatus c ON a.StatusStatusKey=c.StatusKey
                               WHERE a.StatusJobKey = @JobKey
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

            da.SelectCommand.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobStatusHistorySubDetails> data;

            if (dt.Rows.Count > 0)
            {
                totalRecords = (int)dt.Rows[0]["TotalRecords"];
                data = EnumExtension.ToList<JobStatusHistorySubDetails>(dt);
            }
            else
            {
                return null;
            }

            return data;
        }

        public qfrmJobStatusHistory GetqfrmJobStatusHistory(int JobKey)
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

            string sql = @"SELECT *
                           FROM qfrmJobStatusHistory
                           WHERE JobKey = @key";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = JobKey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            qfrmJobStatusHistory data;

            if (dt.Rows.Count > 0)
            {
                data = EnumExtension.ToList<qfrmJobStatusHistory>(dt).FirstOrDefault();
            }
            else
            {
                return null;
            }

            return data;
        }
        #endregion Job Status History

        #region qryJobSearch
        public IList<qryJobSearch> GetqryJobSearch(int showClosed, string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
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
            string where = showClosed == 1 ? "1=1" : "JobClosed IS NULL";

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and {0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "ISNULL(JobNum,'') + ' ' + ISNULL(QHdrNum,'') + ' ' + ISNULL(JobOrderEmployee,'') + ' ' + CustName + ' ' + ISNULL(CustContact,'') + ' ' + ISNULL(CustShipName,'') + ' ' + ISNULL(JobReference,'') + ' ' + ISNULL(JobCustRefNum,'') + ' ' + ISNULL(JobProdDescription,'') + ' ' + ISNULL(JobCustCurrencyCode,'') + ' ' + ISNULL(ShipType,'') + ' ' + ISNULL(InspectionNum,'') + ' ' + ISNULL(JobDUINum,'')";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "JobNum";
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
                            FROM qryJobSearch
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
                throw;
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qryJobSearch> data = EnumExtension.ToList<qryJobSearch>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }
        #endregion qryJobSearch

        #region Change PO Currency
        public bool ChangePOCurrency(int POKey, string CurrencyCode, decimal CurrencyRate, string currentUser)
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

            string sql = "UPDATE tblJobPurchaseOrders SET POCurrencyCode = @code, POCurrencyRate = @rate WHERE POKey = @pokey";

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@code", SqlDbType.VarChar).Value = CurrencyCode;
            cmd.Parameters.Add("@pokey", SqlDbType.Int).Value = POKey;
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

        #region Export Invoice To Peachtree
        public string ExportInvoiceToPeachtree(int InvoiceKey, string currentUser)
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

            string sql = "SELECT * FROM qrptJobInvoice WHERE InvoiceKey = @InvoiceKey";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@InvoiceKey", SqlDbType.Int).Value = InvoiceKey;

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

            qrptJobInvoice invoice;

            if (dt.Rows.Count > 0)
            {
                invoice = EnumExtension.ToList<qrptJobInvoice>(dt).FirstOrDefault();
            }
            else
            {
                ConnManager.CloseConn(oConn);
                return "";
            }

            int RecordCount = 0;
            IList<qryJobInvoiceToPeachtreeItem> items = GetInvoiceItemsForExport(InvoiceKey, ref RecordCount, oConn);
            IList<qryJobInvoiceToPeachtreeCharge> charges = GetInvoiceChargesForExport(InvoiceKey, ref RecordCount, oConn);

            if (RecordCount == 0)
            {
                return "";
            }

            string[] strOutput = new string[51];

            strOutput[0] = invoice.CustPeachtreeID ?? "";
            strOutput[1] = invoice.InvoiceNum;
            strOutput[2] = "";
            strOutput[3] = "FALSE";
            strOutput[4] = invoice.JobShipDate.GetValueOrDefault().ToString("MM/d/yyyy");
            strOutput[5] = "";
            strOutput[6] = "FALSE";
            strOutput[7] = invoice.QuoteNum ?? "";
            strOutput[8] = "";
            strOutput[9] = "FALSE";
            strOutput[10] = String.Format("{0}{1}{0}", '"', invoice.InvoiceBillingName);
            strOutput[11] = String.Format("{0}{1}{0}", '"', invoice.InvoiceBillingAddress1 ?? "");
            strOutput[12] = String.Format("{0}{1}{0}", '"', invoice.InvoiceBillingAddress2 ?? "");
            strOutput[13] = String.Format("{0}{1}{0}", '"', invoice.InvoiceBillingCity ?? "");
            strOutput[14] = String.Format("{0}{1}{0}", '"', invoice.InvoiceBillingState ?? "");
            strOutput[15] = String.Format("{0}{1}{0}", '"', invoice.InvoiceBillingZip ?? "");
            strOutput[16] = String.Format("{0}{1}{0}", '"', invoice.InvoiceBillingCountryName ?? "");
            strOutput[17] = String.Format("{0}{1}{0}", '"', invoice.JobCustRefNum ?? "");
            strOutput[18] = String.Format("{0}{1}{0}", '"', GetShipmentType(invoice.JobShipType, invoice.CustLanguageCode, oConn));
            strOutput[19] = strOutput[4];
            strOutput[20] = GetPaymentTermDueDate(invoice.InvoiceDate, invoice.InvoicePaymentTerms, oConn).ToString("MM/d/yyyy");
            strOutput[21] = "0";
            strOutput[22] = strOutput[4];
            strOutput[23] = String.Format("{0}{1}{0}", '"', GetPaymentTerms(invoice.InvoicePaymentTerms, invoice.CustLanguageCode, oConn));
            strOutput[24] = String.Format("{0}{1}{0}", '"', invoice.EmployeePeachtreeID);
            strOutput[25] = "12000";
            strOutput[26] = "";
            strOutput[27] = "";
            strOutput[28] = "FALSE";
            strOutput[29] = "";
            strOutput[30] = "FALSE";
            strOutput[31] = "";
            strOutput[32] = "FALSE";
            strOutput[33] = RecordCount.ToString();
            strOutput[34] = "";
            strOutput[35] = "0";
            strOutput[36] = "FALSE";
            strOutput[37] = "";
            strOutput[38] = "";
            strOutput[39] = "";
            strOutput[40] = "0";
            strOutput[41] = "";
            strOutput[42] = "40100";
            strOutput[43] = "";
            strOutput[44] = "1";
            strOutput[45] = "";
            strOutput[46] = String.Format("{0}{1}{0}", '"', invoice.JobNum + ",CLIENT,SALES");
            strOutput[47] = "";
            strOutput[48] = "";
            strOutput[49] = "";
            strOutput[50] = "";

            ConnManager.CloseConn(oConn);

            string path = Path.Combine(HttpContext.Current.Request.MapPath("~/App_Data/Peachtree/"));
            string filename = Path.Combine(path, "SALES" + DateTime.Now.ToString("yyyyMMdd") + ".csv");

            if(File.Exists(filename))
                File.Delete(filename);

            using (StreamWriter ws = new StreamWriter(filename))
            {
                //'*** Start exporting the detail lines
                int Counter = 0;

                foreach (var item in items)
                {
                    Counter += 1;
                    strOutput[34] = Counter.ToString();

                    //'*** Check for note on top, add line if necessary
                    if (!string.IsNullOrEmpty(item.ItemMemo) && item.ItemMemoMoveBottom == 0)
                    {
                        strOutput[37] = "";
                        strOutput[39] = "";
                        strOutput[43] = "";
                        strOutput[45] = "";
                        strOutput[46] = "";

                        //'*** Process note, each CRLF or ~ has to be a separate line
                        strOutput[41] = "";
                        string[] notes = item.ItemMemo.Split(Environment.NewLine.ToCharArray());
                        foreach (var note in notes)
                        {
                            strOutput[41] = String.Format("{0}{1}{0}", '"', note.Replace(@"""", "''"));
                            ws.WriteLine(string.Join(",", strOutput));
                            strOutput[41] = "";
                            Counter += 1;
                            strOutput[34] = Counter.ToString();
                        }
                    }

                    //'*** Export the item description
                    strOutput[37] = item.ItemQty.ToString();

                    var strPeachTreeItemID = item.ItemID;
                    //TODO '*** Check for null PeachtreeItemID, prompt for one if necessary
                    //If !VendorKey <> lngCurrentVendorKey Then
                    //    If Len(!ItemID & vbNullString) = 0 Then
                    //        strPeachtreeItemID = InputBox("The Peachtree Item ID field is blank for " & !VendorName & ".  Please enter what you want to appear in the Item ID column within Peachtree (this must already be in the item database of Peachtree):", "Enter Peachtree Item ID", !VendorName)
                    //        If Len(strPeachtreeItemID & vbNullString) = 0 Then
                    //            GoTo Exit_ExportInvoiceToPeachtree
                    //        Else
                    //            DoCmd.RunSQL "UPDATE tblVendors SET VendorPeachtreeItemID = '" & Trim(strPeachtreeItemID) & "' WHERE VendorKey = " & !VendorKey
                    //        End If
                    //    Else
                    //        strPeachtreeItemID = Trim(!ItemID)
                    //    End If
                    //End If

                    //lngCurrentVendorKey = !VendorKey

                    strOutput[39] = String.Format(@"{0}{1}{0}", '"', strPeachTreeItemID);
                    strOutput[41] = String.Format(@"{0}{1}{0}", '"', item.ItemNum + " - " + item.ItemDescription.Replace(@"""", "''"));
                    strOutput[43] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.ItemPrice);
                    strOutput[45] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.ItemLinePrice * -1);
                    strOutput[46] = String.Format(@"{0}{1},CLIENTE,SALES{0}", '"', invoice.JobNum);

                    ws.WriteLine(string.Join(",", strOutput));

                    //'*** Check for note on bottom, add line if necessary
                    if (!string.IsNullOrEmpty(item.ItemMemo) && item.ItemMemoMoveBottom == 1)
                    {
                        strOutput[37] = "";
                        strOutput[39] = "";
                        strOutput[43] = "";
                        strOutput[45] = "";
                        strOutput[46] = "";

                        //'*** Process note, each CRLF or ~ has to be a separate line
                        strOutput[41] = "";
                        string[] notes = item.ItemMemo.Split(Environment.NewLine.ToCharArray());
                        foreach (var note in notes)
                        {
                            strOutput[41] = String.Format("{0}{1}{0}", '"', note.Replace(@"""", "''"));
                            ws.WriteLine(string.Join(",", strOutput));
                            strOutput[41] = "";
                            Counter += 1;
                            strOutput[34] = Counter.ToString();
                        }
                    }
                }

                //'*** Export the charges
                Counter += 1;
                strOutput[34] = Counter.ToString();

                foreach (var charge in charges)
                {
                    strOutput[37] = charge.ChargeQty.ToString();
                    strOutput[39] = String.Format("{0}{1}{0}", '"', charge.ChargePeachtreeID);
                    strOutput[41] = "";
                    strOutput[42] = charge.ChargeGLAccount.GetValueOrDefault().ToString();
                    strOutput[43] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", charge.ChargeLinePrice / charge.ChargeQty);
                    strOutput[45] = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", charge.ChargeLinePrice * -1);
                    strOutput[46] = String.Format("{0}{1},CLIENT,{2}{0}", '"', invoice.JobNum, charge.ChargePeachtreeJobPhaseID);

                    //'*** Process description
                    if (!String.IsNullOrWhiteSpace(charge.ChargeMemo))
                    {
                        string[] notes = charge.ChargeMemo.Split(Environment.NewLine.ToCharArray());
                        foreach (var note in notes)
                        {
                            strOutput[41] = String.Format("{0}{1}{0}", '"', note.Replace(@"""", "''"));
                            ws.WriteLine(string.Join(",", strOutput));
                            strOutput[41] = "";
                            Counter += 1;
                            strOutput[34] = Counter.ToString();
                        }
                    }

                    ws.WriteLine(string.Join(",", strOutput));
                    Counter += 1;
                }

                ws.Close();
                return filename;
            }
        }

        private IList<qryJobInvoiceToPeachtreeItem> GetInvoiceItemsForExport(int InvoiceKey, ref int RecordCount, SqlConnection oConn)
        {
            string sql = "SELECT * FROM qryJobInvoiceToPeachtreeItemsSummary WHERE InvoiceKey = @InvoiceKey";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@InvoiceKey", SqlDbType.Int).Value = InvoiceKey;

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
                IList<qryJobInvoiceToPeachtreeItem> items = EnumExtension.ToList<qryJobInvoiceToPeachtreeItem>(ds.Tables[0]);
                RecordCount = RecordCount + items.Count;
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.ItemMemo))
                    {
                        string[] Note = item.ItemMemo.Split(Environment.NewLine.ToCharArray());
                        RecordCount += Note.Length;
                    }
                }
                return items;
            }

            sql = "SELECT * FROM qryJobInvoiceToPeachtreeItems WHERE InvoiceKey = @InvoiceKey";
            da.SelectCommand.CommandText = sql;
            //da.SelectCommand.Parameters.Add("@InvoiceKey", SqlDbType.Int).Value = InvoiceKey;

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
                IList<qryJobInvoiceToPeachtreeItem> items = EnumExtension.ToList<qryJobInvoiceToPeachtreeItem>(ds.Tables[0]);
                RecordCount = RecordCount + items.Count;
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.ItemMemo))
                    {
                        string[] Note = item.ItemMemo.Split(Environment.NewLine.ToCharArray());
                        RecordCount += Note.Length;
                    }
                }
                return items;
            }

            return new List<qryJobInvoiceToPeachtreeItem>();
        }

        private IList<qryJobInvoiceToPeachtreeCharge> GetInvoiceChargesForExport(int InvoiceKey, ref int RecordCount, SqlConnection oConn)
        {
            string sql = "SELECT * FROM qryJobInvoiceToPeachtreeCharges WHERE InvoiceKey = @InvoiceKey";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@InvoiceKey", SqlDbType.Int).Value = InvoiceKey;

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
                IList<qryJobInvoiceToPeachtreeCharge> charges = EnumExtension.ToList<qryJobInvoiceToPeachtreeCharge>(ds.Tables[0]);
                //RecordCount = RecordCount + charges.Count;
                foreach (var charge in charges)
                {
                    RecordCount += 1;
                    if (!string.IsNullOrEmpty(charge.ChargeMemo))
                    {
                        string[] Note = charge.ChargeMemo.Split(Environment.NewLine.ToCharArray());
                        RecordCount += Note.Length;
                    }
                }
                return charges;
            }

            return new List<qryJobInvoiceToPeachtreeCharge>();
        }

        private DateTime GetPaymentTermDueDate(DateTime InvoiceDate, int TermKey, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tlkpPaymentTerms WHERE TermKey = @TermKey";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@TermKey", SqlDbType.Int).Value = TermKey;

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

            DateTime termDate = InvoiceDate;

            if (ds.Tables[0].Rows.Count > 0)
            {
                PaymentTerms term = EnumExtension.ToList<PaymentTerms>(ds.Tables[0]).FirstOrDefault();
                termDate = InvoiceDate.AddDays(term.TermPercentDays);
            }

            return termDate;
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

        private string GetShipmentType(int ShipType, string LanguageCode, SqlConnection oConn)
        {
            string sql = "SELECT ShipTypeLanguageCode, ShipTypeText FROM tsysShipmentTypes WHERE ShipTypeExpression = @ShipType and ShipTypeLanguageCode = @LanguageCode";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@ShipType", SqlDbType.Int).Value = ShipType;
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

            string ShipText = "* * * Invalid Ship Type * * *";

            if (ds.Tables[0].Rows.Count > 0)
            {
                ShipmentType ship = EnumExtension.ToList<ShipmentType>(ds.Tables[0]).FirstOrDefault();
                ShipText = ship.ShipTypeText;
            }

            return ShipText;
        }
        #endregion  Export Invoice To Peachtree

        #region Export PO To Peachtree
        public string ExportPurchaseOrderToPeachtree(int POKey, string currentUser)
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

            string sql = "SELECT * FROM qrptJobPurchaseOrder WHERE POKey = @POKey";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@POKey", SqlDbType.Int).Value = POKey;

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

            qrptJobPurchaseOrder purchaseOrder;

            if (dt.Rows.Count > 0)
            {
                purchaseOrder = EnumExtension.ToList<qrptJobPurchaseOrder>(dt).FirstOrDefault();
            }
            else
            {
                ConnManager.CloseConn(oConn);
                return "";
            }

            int RecordCount = 0;
            IList<qryJobPurchaseOrderPeachtreeItem> items = GetPOItemsForExport(POKey, ref RecordCount, oConn);
            IList<qryJobPurchaseOrderPeachtreeCharge> charges = GetPOChargesForExport(POKey, ref RecordCount, oConn);

            if (RecordCount == 0)
            {
                return "";
            }

            string[] strOutput = new string[31];

            POShipTo POShipTo = GetPOShipTo(purchaseOrder, oConn);

            strOutput[0] = purchaseOrder.VendorPeachtreeID ?? "";
            strOutput[1] = String.Format("{0}{1}{0}", '"', GetPONum(purchaseOrder.POKey, oConn));
            strOutput[2] = purchaseOrder.PODate.ToString("m/d/yyyy");
            strOutput[3] = "FALSE";
            strOutput[4] = purchaseOrder.POGoodThruDate.ToString("m/d/yyyy");
            strOutput[5] = "FALSE";
            strOutput[6] = "";
            strOutput[7] = "";

            // shipto
            strOutput[8] = (POShipTo != null) ? String.Format("{0}{1}{0}", '"', POShipTo.ShipName): "";
            strOutput[9] = (POShipTo != null) ? String.Format("{0}{1}{0}", '"', POShipTo.ShipAddress1): "";
            strOutput[10] = (POShipTo != null) ? String.Format("{0}{1}{0}", '"', POShipTo.ShipAddress2) : "";
            strOutput[11] = (POShipTo != null) ? String.Format("{0}{1}{0}", '"', POShipTo.ShipCity): "";
            strOutput[12] = (POShipTo != null) ? String.Format("{0}{1}{0}", '"', POShipTo.ShipState): "";
            strOutput[13] = (POShipTo != null) ? String.Format("{0}{1}{0}", '"', POShipTo.ShipZip): "";
            strOutput[14] = (POShipTo != null) ? String.Format("{0}{1}{0}", '"', POShipTo.ShipCountry) : "";
            // shipto

            strOutput[15] = "0";
            strOutput[16] = String.Format("{0}{1}{0}", '"', GetPaymentTerms(purchaseOrder.POVendorPaymentTerms.GetValueOrDefault(), purchaseOrder.VendorLanguageCode, oConn));
            strOutput[17] = "20100";
            strOutput[18] = String.Format("{0}{1}{0}", '"', GetShipmentType(purchaseOrder.POShipmentType, purchaseOrder.VendorLanguageCode, oConn));
            strOutput[19] = "";
            strOutput[20] = "";
            strOutput[21] = "FALSE";
            strOutput[22] = RecordCount.ToString();
            strOutput[23] = "";
            strOutput[24] = "";
            strOutput[25] = "";
            strOutput[26] = "";
            strOutput[27] = "";
            strOutput[28] = "";
            strOutput[29] = "";
            strOutput[30] = "";
            
            ConnManager.CloseConn(oConn);

            string path = Path.Combine(HttpContext.Current.Request.MapPath("~/App_Data/Peachtree/"));
            string filename = Path.Combine(path, "PO" + DateTime.Now.ToString("yyyyMMdd") + ".csv");

            if (File.Exists(filename))
                File.Delete(filename);

            using (StreamWriter ws = new StreamWriter(filename))
            {
                //'*** Start exporting the detail lines
                int Counter = 0;

                //'*** Loop through items
                foreach (var item in items)
                {
                    Counter += 1;
                    
                    strOutput[23] = Counter.ToString();
                    strOutput[24] = item.POItemsQty.ToString();
                    strOutput[25] = String.Format("{0}{1}{0}", '"', purchaseOrder.VendorPeachtreeItemID); 
                    strOutput[26] = String.Format("{0}{1}{0}", '"', item.DescriptionText);
                    strOutput[27] = "50100";
                    strOutput[28] = String.Format("{0:0.00}", item.ItemCost);
                    strOutput[29] = strOutput[28] = String.Format("{0:0.00}", item.ItemLineCost);
                    strOutput[30] = String.Format("{0}{1},{2},Cost of Goods{0}", '"', purchaseOrder.JobNum, purchaseOrder.VendorPeachtreeJobID);

                    ws.WriteLine(string.Join(",", strOutput));
                }

                //'*** Export the charges
                foreach (var charge in charges)
                {
                    Counter += 1;
                    strOutput[23] = Counter.ToString();
                    strOutput[24] = "1";
                    strOutput[25] = String.Format("{0}{1}{0}", '"', charge.ChargePeachtreeID);
                    strOutput[26] = String.Format("{0}{1}{0}", '"', charge.POChargesMemo);
                    strOutput[27] = charge.ChargeAPAccount.ToString();
                    strOutput[28] = String.Format("{0:0.00}", charge.ChargeCost);
                    strOutput[29] = strOutput[28];
                    strOutput[30] = String.Format("{0}{1},{2},{3}{0}", '"', purchaseOrder.JobNum, purchaseOrder.VendorPeachtreeItemID, charge.ChargePeachtreeJobPhaseID);

                    ws.WriteLine(string.Join(",", strOutput));
                }

                ws.Close();
                return filename;
            }
        }

        private IList<qryJobPurchaseOrderPeachtreeItem> GetPOItemsForExport(int POKey, ref int RecordCount, SqlConnection oConn)
        {
            string sql = "SELECT * FROM qryJobPurchaseOrderPeachtreeItems WHERE POKey = @POKey";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@POKey", SqlDbType.Int).Value = POKey;
            da.SelectCommand.CommandText = sql;
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
                IList<qryJobPurchaseOrderPeachtreeItem> items = EnumExtension.ToList<qryJobPurchaseOrderPeachtreeItem>(ds.Tables[0]);
                RecordCount = RecordCount + items.Count;
                return items;
            }

            return new List<qryJobPurchaseOrderPeachtreeItem>();
        }

        private IList<qryJobPurchaseOrderPeachtreeCharge> GetPOChargesForExport(int POKey, ref int RecordCount, SqlConnection oConn)
        {
            string sql = "SELECT * FROM qryJobPurchaseOrderPeachtreeCharges WHERE POChargesPOKey = @POKey";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@POKey", SqlDbType.Int).Value = POKey;

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
                IList<qryJobPurchaseOrderPeachtreeCharge> charges = EnumExtension.ToList<qryJobPurchaseOrderPeachtreeCharge>(ds.Tables[0]);
                RecordCount = RecordCount + charges.Count;
                return charges;
            }

            return new List<qryJobPurchaseOrderPeachtreeCharge>();
        }

        private POShipTo GetPOShipTo(qrptJobPurchaseOrder purchaseOrder, SqlConnection oConn)
        {
            //
            string PONum = String.Empty;
            string sql = "SELECT * FROM dbo.fqryShipToAddress(@POCustShipKey,@POWarehouseKey,@LocationKey) WHERE ShipDestination = @POFreightDestination";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.CommandText = sql;
            da.SelectCommand.Parameters.Add("@POCustShipKey", SqlDbType.Int).Value = purchaseOrder.POCustShipKey;
            da.SelectCommand.Parameters.Add("@POWarehouseKey", SqlDbType.Int).Value = purchaseOrder.POWarehouseKey;
            da.SelectCommand.Parameters.Add("@LocationKey", SqlDbType.Int).Value = purchaseOrder.LocationKey;
            da.SelectCommand.Parameters.Add("@POFreightDestination", SqlDbType.Int).Value = purchaseOrder.POFreightDestination;
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
                var data = EnumExtension.ToList<POShipTo>(ds.Tables[0]).FirstOrDefault();
            }

            return null;
        }

        private string GetPONum(int POKey, SqlConnection oConn)
        {
            string PONum = String.Empty;
            string sql = "SELECT PONum, PORevisionNum, PONumShipment FROM tblJobPurchaseOrders WHERE POKey =  @POKey";
            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@POKey", SqlDbType.Int).Value = POKey;
            da.SelectCommand.CommandText = sql;
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
                var row = ds.Tables[0].Rows[0];
                PONum = Convert.ToInt32(row[0]).ToString();
                int PORevisionNum = Convert.ToInt32(row[1]);
                if (PORevisionNum > 0)
                    PONum = PONum + ((char)(PORevisionNum + 64)).ToString(); // if 1 then A if 2 then B, etc.....

                int PONumShipment = Convert.ToInt32(row[2]);
                PONum += "-" + PONumShipment.ToString().PadLeft(2, '0');
            }

            return PONum;
        }
        #endregion  Export PO To Peachtree

        #region Job Public Functions
        //public string GetJobNum(int JobKey, int Style = 0)
        //{
        //    SqlConnection oConn = null;

        //    try
        //    {
        //        oConn = ConnManager.OpenConn();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
        //        throw;
        //    };

        //    string sql = "SELECT JobYear, JobNum FROM tblJobHeader WHERE JobKey = @JobKey";

        //    SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
        //    da.SelectCommand.CommandTimeout = 15000;
        //    da.SelectCommand.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;

        //    DataSet ds = new DataSet();

        //    da.Fill(ds);

        //    ConnManager.CloseConn(oConn);

        //    DataTable dt;
        //    dt = ds.Tables[0];

        //    string jobnum = String.Empty;

        //    if (dt.Rows.Count > 0)
        //    {
        //        jobnum = EnumExtension.ToList<JobList>(dt).FirstOrDefault().JobNum;
        //    }

        //    return jobnum;
        //}
        #endregion

        #region qryPOSearch
        public IList<qryPOSearch> GetqryPOSearch(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
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
                string fieldName = "PONumFormatted + ' ' + ISNULL(VendorName,'') + ' ' + CONVERT(VARCHAR(10), PODate, 101) + ' ' + ISNULL(POVendorReference,'') + JobNum";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "JobNum";
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
                               select *
                                from qryPOSearch
                               where {0}
                            )
                            SELECT *
                            FROM (
                               select *, ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,
                                    IsNull((select count(*) from qData),0)  as TotalRecords
                               from qData
                            ) a
                            WHERE {1}";

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
                throw;
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qryPOSearch> data = EnumExtension.ToList<qryPOSearch>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }
        #endregion qryPOSearch

        #region qryJobInvoiceSearch
        public IList<qryJobInvoiceSearch> GetqryJobInvoiceSearch(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords)
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
                string fieldName = "JobNum + ' ' + InvoiceNum + ' ' + ISNULL(BillTo,'') + ' ' + CONVERT(VARCHAR(10), Date, 101)";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

            #region Ordenamiento
            string order = "JobNum DESC, InvoiceNum";
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
                               select *
                                from qryJobInvoiceSearch
                               where {0}
                            )
                            SELECT *
                            FROM (
                               select *, ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,
                                    IsNull((select count(*) from qData),0)  as TotalRecords
                               from qData
                            ) a
                            WHERE {1}";

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
                throw;
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qryJobInvoiceSearch> data = EnumExtension.ToList<qryJobInvoiceSearch>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }
        #endregion qryJobInvoiceSearch

        public NewJobMessage NewJobNotification(int JobKey)
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

            var job = Get(JobKey);

            if (job == null) return null;

            var terms = payTermsRepo.Get(job.JobCustPaymentTerms);

            var employee = employeeRepo.Get(job.JobOrderEmployeeKey);

            int totalContacts = 0;
            var vendorContacts = vendorsRepo.GetContactsByVendor(job.JobCarrierKey.GetValueOrDefault(), ref totalContacts);

            NewJobMessage msg = new NewJobMessage();
            msg.RecipientsCC = new List<string>();
            msg.RecipientsTO = new List<string>();

            if (employee != null)
            {
                if (!String.IsNullOrEmpty(employee.EmployeeEmail)) msg.RecipientsTO.Add(employee.EmployeeEmail);
                if (!String.IsNullOrEmpty(employee.EmployeeEmailCC)) msg.RecipientsTO.Add(employee.EmployeeEmailCC);
            }

            foreach(var contact in vendorContacts) {
                if (contact.ContactEmail != null) msg.RecipientsCC.Add(contact.ContactEmail);
            }

            msg.Subject = "[CBH]  New Job Created: " + job.x_JobNumFormatted;

            msg.Body = "All printouts related to this job have been added to the print queue.  (The queue will auto-print when the database is started.  Otherwise, select 'Print Queue' from the Jobs Menu.";
            //'**** If the payment terms require warning...
            if (terms.TermWarningFlag) {
                string strPaymentTerms = GetPaymentTerms(job.JobCustPaymentTerms, "en", oConn);
                if (strPaymentTerms.Length < 75) {
                    strPaymentTerms = ' '.Repeat(Convert.ToInt32((75 - strPaymentTerms.Length) / 2)) + "(" + strPaymentTerms;
                    strPaymentTerms = strPaymentTerms + ")" + ' '.Repeat(75 - strPaymentTerms.Length);
                }
                
                msg.Body = String.Format(@"
                         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
                         *           PLEASE MAKE NOTE OF THIS JOB'S PAYMENT TERMS                      *
                           {0}
                         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *

                        ", strPaymentTerms) + msg.Body;
            }

            ConnManager.CloseConn(oConn);

            return msg;
        }

        #region Job Currency Master
        public IList<JobCurrencyMaster> GetListJobCurrencyMaster(int JobKey)
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

            IList<JobCurrencyMaster> summary;
            try
            {
                summary = GetListJobCurrencyMaster(JobKey, oConn);
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

        private IList<JobCurrencyMaster> GetListJobCurrencyMaster(int JobKey, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, b.CurrencyDescription, b.CurrencySymbol 
                            FROM tblJobCurrencyMaster a
	                            INNER JOIN tblCurrencyRates b ON a.JobCurrencyCode = b.CurrencyCode 
                            WHERE (a.JobCurrencyJobKey = @JobKey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobCurrencyMaster> data = new List<JobCurrencyMaster>();
            if (dt.Rows.Count > 0)
            {
                data = dt.ToList<JobCurrencyMaster>();
            }

            return data;
        }

        public JobCurrencyMaster Update(JobCurrencyMaster data)
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

            JobCurrencyMaster result;
            try
            {
                result = Update(data, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private JobCurrencyMaster Update(JobCurrencyMaster data, SqlConnection oConn)
        {
            string sql = "UPDATE tblJobCurrencyMaster SET {0} WHERE JobCurrencyJobKey = @JobKey AND JobCurrencyCode = @JobCurrencyCode";

            EnumExtension.setUpdateValues(data, "JobCurrencyJobKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = data.JobCurrencyJobKey;
            cmd.Parameters.Add("@JobCurrencyCode", SqlDbType.Char).Value = data.JobCurrencyCode;

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

            JobCurrencyMaster result = GetJobCurrencyMaster(data.JobCurrencyJobKey, data.JobCurrencyCode, oConn);

            ConnManager.CloseConn(oConn);

            return result;
        }

        private JobCurrencyMaster GetJobCurrencyMaster(int id, string code, SqlConnection oConn)
        {
            string sql = @"SELECT a.*, b.CurrencyDescription, b.CurrencySymbol 
                            FROM tblJobCurrencyMaster a
	                            INNER JOIN tblCurrencyRates b ON a.JobCurrencyCode = b.CurrencyCode
                            WHERE a.JobCurrencyJobKey = @JobKey AND a.JobCurrencyCode = @JobCurrencyCode";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@JobKey", SqlDbType.Int).Value = Convert.ToInt32(id);
            da.SelectCommand.Parameters.Add("@JobCurrencyCode", SqlDbType.Char).Value = code;

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<JobCurrencyMaster> result = dt.ToList<JobCurrencyMaster>();

            return result.FirstOrDefault<JobCurrencyMaster>();
        }

        public void UpdateJobCurrencyMaster(int JobKey)
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
                UpdateJobCurrencyMaster(JobKey, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            }

        }
        #endregion Job Currency Master

        public IList<qfrmJobOverviewPopupUpdateCurrency> GetqfrmJobOverviewPopupUpdateCurrency(int JobKey)
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

            string sql = @"SELECT *
                            FROM qfrmJobOverviewPopupUpdateCurrency 
                            WHERE JobKey = @JobKey";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);
            da.SelectCommand.Parameters.Add("@JobKey", SqlDbType.Int).Value = JobKey;

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

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                var data = dt.ToList<qfrmJobOverviewPopupUpdateCurrency>();
                return data;
            }
            else
            {
                return null;
            }
        }

        public IList<qfrmJobExemptFromProfitReport> GetJobExemptFromProfitReport(FieldFilters fieldFilters, string query, Sort sort, string[] queryBy, int page, int start, int limit, ref int totalRecords)
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

            #region Query By Settings
            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "";

                if (queryBy.Length == 0)
                {
                    fieldName = "JobNum+' '+ISNULL(JobProdDescription,'')+' '+ISNULL(JobReference,'')+' '+ISNULL(CustName,'')+' '+ISNULL(CustContact,'')";
                }
                else
                {
                    foreach (string field in queryBy)
                    {
                        fieldName += (!String.IsNullOrWhiteSpace(fieldName)) ? " + " : "";
                        fieldName += field;
                    }
                }

                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                        EnumExtension.generateLikeWhere(query, fieldName);

            }
            #endregion Query By Settings

            #region Ordenamiento
            string order = "JobNum";
            string direction = "DESC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;

                //if (order == "x_Estatus") order = "EstatusTipo";
            }
            #endregion Ordenamiento

            string sql = @"WITH qData
                            AS
                            (
                            SELECT *, 
                                ROW_NUMBER() OVER (ORDER BY {2} {3}) as row
                            FROM qfrmJobExemptFromProfitReport WHERE {0}
                            )
                            SELECT *,IsNull((select count(*) from qData),0)  as TotalRecords 
                            FROM qData
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
                var data = dt.ToList<qfrmJobExemptFromProfitReport>();
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public qfrmJobExemptFromProfitReport Update(qfrmJobExemptFromProfitReport model)
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

            string sql = "UPDATE tblJobHeader SET JobExemptFromProfitReport = @value WHERE JobKey = @JobKey";

            //EnumExtension.setUpdateValues(model, "InvoiceKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = Convert.ToInt32(model.JobKey);
            cmd.Parameters.Add("@value", SqlDbType.Bit).Value = model.JobExemptFromProfitReport ? 1 : 0;

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

            ConnManager.CloseConn(oConn);

            return model;
        }

        public IList<qfrmJobExemptFromPronacaReport> GetJobExemptFromPronacaReport(FieldFilters fieldFilters, string query, Sort sort, string[] queryBy, int page, int start, int limit, ref int totalRecords)
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

            #region Query By Settings
            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "";

                if (queryBy.Length == 0)
                {
                    fieldName = "JobNum+' '+ISNULL(JobProdDescription,'')+' '+ISNULL(JobReference,'')+' '+ISNULL(CustName,'')+' '+ISNULL(CustContact,'')";
                }
                else
                {
                    foreach (string field in queryBy)
                    {
                        fieldName += (!String.IsNullOrWhiteSpace(fieldName)) ? " + " : "";
                        fieldName += field;
                    }
                }

                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                        EnumExtension.generateLikeWhere(query, fieldName);

            }
            #endregion Query By Settings

            #region Ordenamiento
            string order = "JobNum";
            string direction = "DESC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;

                //if (order == "x_Estatus") order = "EstatusTipo";
            }
            #endregion Ordenamiento

            string sql = @"WITH qData
                            AS
                            (
                            SELECT *, 
                                ROW_NUMBER() OVER (ORDER BY {2} {3}) as row
                            FROM qfrmJobExemptFromPronacaReport WHERE {0}
                            )
                            SELECT *,IsNull((select count(*) from qData),0)  as TotalRecords 
                            FROM qData
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
                var data = dt.ToList<qfrmJobExemptFromPronacaReport>();
                totalRecords = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public qfrmJobExemptFromPronacaReport Update(qfrmJobExemptFromPronacaReport model)
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

            string sql = "UPDATE tblJobHeader SET JobExemptFromPronacaReport = @value WHERE JobKey = @JobKey";

            //EnumExtension.setUpdateValues(model, "InvoiceKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@JobKey", SqlDbType.Int).Value = Convert.ToInt32(model.JobKey);
            cmd.Parameters.Add("@value", SqlDbType.Bit).Value = model.JobExemptFromPronacaReport ? 1 : 0;

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

            ConnManager.CloseConn(oConn);

            return model;
        }
    }
}