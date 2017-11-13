namespace CBHWA.Models
{
    using CBHWA.Clases;
    using System.Collections.Generic;
    interface IJobsRepository
    {
        JobHeader Add(JobHeader jobHeader);
        JobHeader Update(JobHeader jobHeader);
        bool Remove(JobHeader jobHeader);
        JobHeader Get(int id);
        IList<JobList> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        JobList GetList(int id);

        #region Employee Roles
        IList<JobEmployeeRoles> GetJobEmployeeRoles(int jobkey, ref int totalRecords);
        JobEmployeeRoles Add(JobEmployeeRoles jobrole);
        JobEmployeeRoles Update(JobEmployeeRoles jobrole);
        bool Remove(JobEmployeeRoles role);
        #endregion Employee Roles

        IList<qJobPurchaseOrder> GetListJobPurchaseOrders(int jobKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords);

        IList<qJobInvoice> GetListJobInvoices(int jobKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords);

        IList<qJobOverview> GetJobOverview(int jobKey);

        void UpdateJobCurrencyRates(int JobKey, bool UseCurrentRates);

        JobCurrencyMaster Add(JobCurrencyMaster jobCurMaster);

        #region Job Purchase Orders
        IList<JobPurchaseOrder> GetListJobPurchaseOrder(int poJobKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        JobPurchaseOrder Add(JobPurchaseOrder dataAdded);
        JobPurchaseOrder Update(JobPurchaseOrder dataUpdated);
        JobPurchaseOrder GetJobPurchaseOrder(int id);
        bool Remove(JobPurchaseOrder dataDeleted);
        int GetNewPONum();
        IList<JobPurchaseOrder> GetJobPurchaseOrderByJob(int poJobKey);
        #endregion Job Purchase Orders

        #region Job Purchase Order Items
        IList<JobPurchaseOrderItem> GetJobPurchaseOrderItems(int POJobKey);
        JobPurchaseOrderItem GetJobPurchaseOrderItem(int POItemsKey);
        JobPurchaseOrderItem Add(JobPurchaseOrderItem POItems);
        JobPurchaseOrderItem Updated(JobPurchaseOrderItem POItems);
        bool RemovePurchaseOrderItem(JobPurchaseOrderItem POItem);
        #endregion Job Purchase Order Items

        #region Job Purchase Order Charges
        IList<JobPurchaseOrderCharge> GetJobPurchaseOrderCharges(int POJobKey);
        JobPurchaseOrderCharge GetJobPurchaseOrderCharge(int POChargesKey);
        JobPurchaseOrderCharge Add(JobPurchaseOrderCharge POCharge);
        JobPurchaseOrderCharge Updated(JobPurchaseOrderCharge POCharge);
        bool RemovePurchaseOrderCharge(JobPurchaseOrderCharge POCharge);
        #endregion Job Purchase Order Charges

        #region Job Purchase Order Instructions
        IList<JobPurchaseOrderInstruction> GetJobPurchaseOrderInstructions(int POJobKey);
        JobPurchaseOrderInstruction GetJobPurchaseOrderInstruction(int POInstructionsKey);
        JobPurchaseOrderInstruction Add(JobPurchaseOrderInstruction POInstruction);
        JobPurchaseOrderInstruction Updated(JobPurchaseOrderInstruction POInstruction);
        bool RemovePurchaseOrderInstruction(JobPurchaseOrderInstruction POInstruction);
        #endregion Job Purchase Order Instructions

        #region Job Purchase Order Status History
        IList<JobPurchaseOrderStatusHistory> GetListJobPurchaseOrderStatusHistory(int POKey);
        JobPurchaseOrderStatusHistory GetJobPurchaseOrderStatusHistory(int POStatusKey);
        JobPurchaseOrderStatusHistory Add(JobPurchaseOrderStatusHistory POStatus);
        JobPurchaseOrderStatusHistory Update(JobPurchaseOrderStatusHistory POStatus);
        bool Remove(JobPurchaseOrderStatusHistory POInstruction);
        qfrmJobPurchaseOrderStatusHistory GetqfrmJobPurchaseOrderStatusHistory(int POKey);
        #endregion Job Purchase Order Status History

        #region Charge Categories
        IList<tlkpChargeCategory> GetChargesCategories();
        tlkpChargeCategory GetChargeCategory(int ChargeKey);
        tlkpChargeCategory Add(tlkpChargeCategory Charge);
        tlkpChargeCategory Updated(tlkpChargeCategory Charge);
        bool RemoveChargeCategory(tlkpChargeCategory dataDeleted);
        #endregion Charge Categories

        #region Instructions
        IList<tlkpJobPurchaseOrderInstruction> GettlkpJobPOInstructions(string language);
        tlkpJobPurchaseOrderInstruction GettlkpJobPOInstruction(int ChargeKey);
        tlkpJobPurchaseOrderInstruction Add(tlkpJobPurchaseOrderInstruction Charge);
        tlkpJobPurchaseOrderInstruction Updated(tlkpJobPurchaseOrderInstruction Charge);
        bool Remove(tlkpJobPurchaseOrderInstruction dataDeleted);
        #endregion Instructions

        #region Invoice Header
        IList<InvoiceHeader> GetListInvoiceHeader(int JobKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        InvoiceHeader Add(InvoiceHeader dataAdded);
        InvoiceHeader Update(InvoiceHeader dataUpdated);
        InvoiceHeader GetInvoiceHeader(int id);
        bool Remove(InvoiceHeader dataDeleted);
        IList<InvoiceDDL> GetListInvoiceDDL(int JobKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        #endregion Invoice Header

        #region Invoice Items Summary
        IList<InvoiceItemsSummary> GetListInvoiceItemsSummary(int InvoiceKey);
        InvoiceItemsSummary GetInvoiceItemsSummary(int id);
        InvoiceItemsSummary Add(InvoiceItemsSummary InvoiceItem);
        InvoiceItemsSummary Update(InvoiceItemsSummary InvoiceItem);
        bool Remove(InvoiceItemsSummary InvoiceItem);
        #endregion Invoice Items Summary

        #region Invoice Charges
        IList<InvoiceCharge> GetListInvoiceCharge(int InvoiceKey);
        InvoiceCharge GetInvoiceCharge(int id);
        InvoiceCharge Add(InvoiceCharge InvoiceItem);
        InvoiceCharge Update(InvoiceCharge InvoiceItem);
        bool Remove(InvoiceCharge InvoiceItem);
        #endregion Invoice Charges

        #region Invoice Charges SubTotals
        IList<InvoiceChargesSubTotal> GetListInvoiceChargesSubTotal(int InvoiceKey);
        InvoiceChargesSubTotal GetInvoiceChargesSubTotal(int ISTSubTotalKey, int ISTInvoiceKey);
        InvoiceChargesSubTotal Add(InvoiceChargesSubTotal IChargeST);
        InvoiceChargesSubTotal Update(InvoiceChargesSubTotal IChargeST);
        bool Remove(InvoiceChargesSubTotal IChargeST);
        #endregion Invoice Charges SubTotals

        #region Split BackOrder
        bool SplitBackOrder(int POKey, string currentUser);
        #endregion Split BackOrder

        #region New Job Invoice
        bool CreateNewJobInvoice(int JobKey, string currentUser, List<int> SelectedItems);
        bool CreateNewCommissionInvoice(int JobKey, string currentUser, CommissionInvoice CommissionInvoice);
        #endregion New Job Invoice

        #region Job Status History
        JobStatusHistory GetJobStatusHistoryById(int id);
        JobStatusHistory AddJobHistory(JobStatusHistory added);
        JobStatusHistory UpdateJobHistory(JobStatusHistory updated);
        bool RemoveJobHistory(JobStatusHistory deleted);
        IList<JobStatusHistorySubDetails> GetJobStatusHistorySubDetails(int JobKey, int page, int start, int limit, ref int totalRecords);
        qfrmJobStatusHistory GetqfrmJobStatusHistory(int JobKey);
        #endregion Job Status History

        #region qryJobSearch
        IList<qryJobSearch> GetqryJobSearch(int showClosed, string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        #endregion qryJobSearch

        #region Change PO Currency
        bool ChangePOCurrency(int POKey, string CurrencyCode, decimal CurrencyRate, string currentUser);
        #endregion Change PO Currency

        #region Export To Peachtree
        string ExportInvoiceToPeachtree(int InvoiceKey, string currentUser);
        string ExportPurchaseOrderToPeachtree(int POKey, string currentUser);
        #endregion Export To Peachtree

        #region qryPOSearch
        IList<qryPOSearch> GetqryPOSearch(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        #endregion qryPOSearch

        #region qryJobInvoiceSearch
        IList<qryJobInvoiceSearch> GetqryJobInvoiceSearch(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        #endregion qryJobInvoiceSearch

        NewJobMessage NewJobNotification(int JobKey);
       
        #region Job Currency Master
        IList<JobCurrencyMaster> GetListJobCurrencyMaster(int JobKey);
        JobCurrencyMaster Update(JobCurrencyMaster data);
        IList<qfrmJobOverviewPopupUpdateCurrency> GetqfrmJobOverviewPopupUpdateCurrency(int JobKey);

        void UpdateJobCurrencyMaster(int JobKey);
        #endregion Job Currency Master

        IList<qfrmJobExemptFromProfitReport> GetJobExemptFromProfitReport(FieldFilters fieldFilters, string query, Sort sort, string[] queryBy, int page, int start, int limit, ref int totalRecords);

        qfrmJobExemptFromProfitReport Update(qfrmJobExemptFromProfitReport model);


        IList<qfrmJobExemptFromPronacaReport> GetJobExemptFromPronacaReport(FieldFilters fieldFilters, string query, Sort sort, string[] queryBy, int page, int start, int limit, ref int totalRecords);

        qfrmJobExemptFromPronacaReport Update(qfrmJobExemptFromPronacaReport model);

    }
}