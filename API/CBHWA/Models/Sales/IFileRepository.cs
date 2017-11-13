using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IFileRepository
    {
        IList<FileList> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        IList<Quotes> GetQuotes(int filekey);
        IList<FileStatus> GetStatus(int filekey);
        FileHeader Add(FileHeader fileheader);
        FileHeader Update(FileHeader fileheader);
        bool Remove(FileHeader fileheader);
        FileHeader Get(int id);
        FileOverview GetOverview(int id);
        IList<FileQuoteSummary> GetQuoteSummary(int id, ref int totalRecords);
        IList<FileVendorSummary> GetVendorSummary(int id);

        #region Employee Roles
        IList<FileEmployeeRoles> GetFileEmployeeRoles(int filekey, ref int totalRecords);
        FileEmployeeRoles Add(FileEmployeeRoles filerole);
        FileEmployeeRoles Update(FileEmployeeRoles filerole);
        bool Remove(FileEmployeeRoles role);
        #endregion Employee Roles

        #region File Quote Details
        IList<FileQuoteDetail> GetQuoteDetails(int filekey, int vendorkey);
        FileQuoteDetail GetQuoteDetail(int id);
        FileQuoteDetail Add(FileQuoteDetail detail);
        FileQuoteDetail Update(FileQuoteDetail detail);
        bool Remove(FileQuoteDetail id);
        IList<qfrmFileQuoteDetailsSub> GetqfrmFileQuoteDetailsSub(int FileKey);
        #endregion File Quote Details

        #region Status History
        IList<FileStatusHistorySubDetails> GetFSHSubDetails(int filekey, int page, int start, int limit, ref int totalRecords);
        IList<FileStatusHistory> GetFileStatusHistory(int filekey, int page, int start, int limit, ref int totalRecords);
        FileStatusHistory GetFileStatusHistoryById(int id, int page, int start, int limit, ref int totalRecords);
        FileStatusHistory Add(FileStatusHistory added);
        FileStatusHistory Update(FileStatusHistory updated);
        bool Remove(FileStatusHistory deleted);
        qfrmFileStatusHistory GetqfrmFileStatusHistory(int id);

        IList<FileQuoteStatusHistory> GetFileQuoteStatusHistory(int filekey, int QHdrKey, int page, int start, int limit, ref int totalRecords);
        FileQuoteStatusHistory GetFileQuoteStatusHistoryById(int id, int page, int start, int limit, ref int totalRecords);
        FileQuoteStatusHistory Add(FileQuoteStatusHistory added);
        FileQuoteStatusHistory Update(FileQuoteStatusHistory updated);
        bool Remove(FileQuoteStatusHistory deleted);
        IList<FileQuoteStatusHistorySubDetails> GetFileQuoteStatusHistorySubDetails(int FileKey, int page, int start, int limit, ref int totalRecords);
        qfrmFileQuoteStatusHistory GetqfrmFileQuoteStatusHistory(int id);
        #endregion Status History

        #region FileQuoteVendorInfo
        IList<FileQuoteVendorInfo> GetFileQuoteVendorInfo(int filekey, int ShowOnlyWithQuotes, int page, int start, int limit, ref int totalRecords);
        FileQuoteVendorInfo GetFileQuoteVendorInfoById(int filekey, int vendorkey);
        FileQuoteVendorInfo Add(FileQuoteVendorInfo added);
        FileQuoteVendorInfo Update(FileQuoteVendorInfo updated);
        bool Remove(FileQuoteVendorInfo deleted);
        #endregion

        #region Quote Header
        IList<FileQuoteHeader> GetListQuoteHeader(int FileKey, string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        FileQuoteHeader GetQuoteHeader(int id);
        FileQuoteHeader Add(FileQuoteHeader qheader, ref string messageError);
        bool Remove(FileQuoteHeader qheader);
        FileQuoteHeader Update(FileQuoteHeader qheader, ref string messageError);

        bool Remove(Quotes quote);
        #endregion Quote Header

        #region Quote Charges
        IList<FileQuoteCharge> GetListQuoteCharges(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        FileQuoteCharge GetQuoteCharges(int id);
        FileQuoteCharge Add(FileQuoteCharge qcharge, ref string messageError);
        bool Remove(FileQuoteCharge qcharge);
        FileQuoteCharge Update(FileQuoteCharge qcharge, ref string messageError);
        #endregion Quote Charges

        #region Quote Charges SubTotals
        IList<FileQuoteChargesSubTotals> GetListQuoteChargesSubTotals(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        FileQuoteChargesSubTotals GetQuoteChargesSubTotals(int subTotalKey, int qhdrKey);
        FileQuoteChargesSubTotals Add(FileQuoteChargesSubTotals qchargest, ref string messageError);
        bool Remove(FileQuoteChargesSubTotals qchargest);
        FileQuoteChargesSubTotals Update(FileQuoteChargesSubTotals qchargest, ref string messageError);
        #endregion Quote Charges SubTotals

        #region Charge Categories
        IList<ChargeCategories> GetListChargeCategories(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        ChargeCategories GetChargeCategory(int id);
        ChargeCategories Add(ChargeCategories qcharge, ref string messageError);
        bool Remove(ChargeCategories qcharge);
        ChargeCategories Update(ChargeCategories qcharge, ref string messageError);
        #endregion Charge Categories

        #region Quote Item Summary
        IList<FileQuoteItemsSummary> GetListQuoteItemsSummary(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        FileQuoteItemsSummary GetQuoteItemsSummary(int id);
        FileQuoteItemsSummary Add(FileQuoteItemsSummary qitem, ref string messageError);
        bool Remove(FileQuoteItemsSummary qitem);
        FileQuoteItemsSummary Update(FileQuoteItemsSummary qitem, ref string messageError);
        #endregion Quote Item Summary

        #region tlkpGenericLists
        IList<tlkpGenericLists> GettlkpGenericLists(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        tlkpGenericLists GettlkpGenericLists(int id);
        tlkpGenericLists Add(tlkpGenericLists genericList, ref string messageError);
        bool Remove(tlkpGenericLists genericList);
        tlkpGenericLists Update(tlkpGenericLists genericList, ref string messageError);
        #endregion tlkpGenericLists

        #region tlkpInvoiceSubTotalCategories
        IList<tlkpInvoiceSubTotalCategories> GettlkpInvoiceSubTotalCategories(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        tlkpInvoiceSubTotalCategories GettlkpInvoiceSubTotalCategories(int id);
        tlkpInvoiceSubTotalCategories Add(tlkpInvoiceSubTotalCategories genericList, ref string messageError);
        bool Remove(tlkpInvoiceSubTotalCategories genericList);
        tlkpInvoiceSubTotalCategories Update(tlkpInvoiceSubTotalCategories genericList, ref string messageError);
        #endregion tlkpInvoiceSubTotalCategories

        IList<LeadTime> GetListLeadTime(string query, Sort sort, int page, int start, int limit, ref int totalRecords);

        #region qfrmFileQuoteConfirmation
        IList<qfrmFileQuoteConfirmation> GetqfrmFileQuoteConfirmations(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        qfrmFileQuoteConfirmation GetqfrmFileQuoteConfirmation(int id);
        #endregion qfrmFileQuoteConfirmation

        #region qfrmFileQuoteConfirmationSubVendorInfo
        IList<qfrmFileQuoteConfirmationSVInfo> GetqfrmFileQuoteConfirmationSVInfos(int QHdrKey, string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        qfrmFileQuoteConfirmationSVInfo GetqfrmFileQuoteConfirmationSVInfo(int QHdrKey, int VendorKey);
        #endregion qfrmFileQuoteConfirmationSubVendorInfo

        #region qryFileQuoteVendorSummaryWithDiscount
        IList<qryFileQuoteVendorSummaryWithDiscount> GetqryFileQuoteVendorSummaryWithDiscounts(int QHdrKey, string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        qryFileQuoteVendorSummaryWithDiscount GetqryFileQuoteVendorSummaryWithDiscount(int QHdrKey, int VendorKey);
        qryFileQuoteVendorSummaryWithDiscount GetqryFileQuoteVendorSummaryWithDiscountByFile(int FileKey, int VendorKey);
        #endregion qryFileQuoteVendorSummaryWithDiscount

        #region qfrmItemPriceHistoryPurchaseOrders
        IList<qfrmItemPriceHistoryPurchaseOrder> GetqfrmItemPriceHistoryPurchaseOrders(int ItemKey, string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        #endregion qfrmItemPriceHistoryPurchaseOrders

        #region qryFileQuoteSearch
        IList<qryFileQuoteSearch> GetqryFileQuoteSearch(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        #endregion qryFileQuoteSearch

        #region qryFileSearch
        IList<qryFileSearch> GetqryFileSearch(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        #endregion qryFileSearch

        IList<qsumFileQuoteVendorSummary> GetqsumFileQuoteVendorSummary(int QHdrKey);

        #region Change File Quote Currency
        bool ChangeFileQuoteCurrency(int QHdrKey, string CurrencyCode, decimal CurrencyRate, string currentUser);
        #endregion Change File Quote Currency

        #region Convert Quote
        int CreateJobFromQuote(int QHdrKey, string CurrentUser, string JobNum);
        void CleanupFileQuoteVendorInfo(int FileKey);
        int CheckUnconfirmedVendor(int QHdrKey);
        bool IsQuoteOverFOBLimit(int QHdrKey);
        #endregion Convert Quote

        #region Export Quote To Peachtree
        string ExportQuoteToPeachtree(int QHdrKey, string currentUser);
        #endregion Export Quote To Peachtree

        IList<qsumFileQuoteChargesByGLAccount> GetqsumFileQuoteChargesByGLAccount(int QHdrKey, ref int totalRecords);

        bool CheckConfirmedVendor(int FileKey, int QHdrKey, int VendorKey);

        void UpdateFileQuoteCurrencyRate(int FileKey);

        bool ImportFile(int FileKeySource, int FileKeyTarget, string CurrentUser);
        bool ImportQuote(int QHdrKeySource, int FileKeyTarget, string CurrentUser);

        bool IsOverCustCreditLimit(int CustKey, decimal AmountUSD);
    }
}
