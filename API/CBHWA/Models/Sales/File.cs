namespace CBHWA.Models
{
    using System;

    public class FileHeader
    {
        public int FileKey { get; set; }
        public int FileYear { get; set; }
        public int FileNum { get; set; }
        public int FileStatusKey { get; set; }
        public int FileQuoteEmployeeKey { get; set; }
        public int FileOrderEmployeeKey { get; set; }
        public int FileCustKey { get; set; }
        public int FileContactKey { get; set; }
        public int FileCustShipKey { get; set; }
        public string FileReference { get; set; }
        public decimal FileProfitMargin { get; set; }
        public int? FileCurrentVendor { get; set; }
        public string FileModifiedBy { get; set; }
        public Nullable<System.DateTime> FileModifiedDate { get; set; }
        public string FileCreatedBy { get; set; }
        public DateTime FileCreatedDate { get; set; }
        public DateTime FileDateCustRequired { get; set; }
        public string FileDateCustRequiredNote { get; set; }
        public string FileDefaultCurrencyCode { get; set; }
        public decimal FileDefaultCurrencyRate { get; set; }
        public Nullable<System.DateTime> FileClosed { get; set; }
        public virtual string FileDefaultCurrencySymbol { get; set; }
    }
    public class FileList
    {
        public int FileKey { get; set; }
        public DateTime Date { get; set; }
        public string FileNum { get; set; }
        public string Customer { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<DateTime> FileClosed { get; set; }
        public int FileCustKey { get; set; }
    }
    public class Quotes
    {
        public int QHdrFileKey { get; set; }
        public int QHdrKey { get; set; }
        public string Date { get; set; }
        public string Quote { get; set; }
        public int Vendors { get; set; }
        public string Status { get; set; }
    }
    public class FileStatus
    {
        public int StatusStatusKey { get; set; }
        public int StatusFileKey { get; set; }
        public DateTime StatusDate { get; set; }
        public string StatusMemo { get; set; }
    }
    public class FileQuoteDetail
    {
        public int QuoteKey { get; set; }
        public int QuoteFileKey { get; set; }
        public int QuoteSort { get; set; }
        public decimal QuoteQty { get; set; }
        public int QuoteVendorKey { get; set; }
        public int QuoteItemKey { get; set; }
        public decimal QuoteItemCost { get; set; }
        public decimal QuoteItemPrice { get; set; }
        public decimal QuoteItemLineCost { get; set; }
        public decimal QuoteItemLinePrice { get; set; }
        public string QuoteItemCurrencyCode { get; set; }
        public decimal QuoteItemCurrencyRate { get; set; }
        public Nullable<decimal> QuoteItemWeight { get; set; }
        public Nullable<decimal> QuoteItemVolume { get; set; }
        public string QuoteItemMemoCustomer { get; set; }
        public bool QuoteItemMemoCustomerMoveBottom { get; set; }
        public string QuoteItemMemoSupplier { get; set; }
        public bool QuoteItemMemoSupplierMoveBottom { get; set; }
        public int? QuotePOItemsKey { get; set; }
        public string x_VendorName { get; set; }
        public string x_ItemName { get; set; }
        public decimal x_ProfitMargin { get; set; }
        public string x_FileNum { get; set; }
        public string x_ItemNum { get; set; }
        public Nullable<decimal> x_LineWeight { get; set; }
        public Nullable<decimal> x_LineVolume { get; set; }
        public virtual string FileDefaultCurrencyCode { get; set; }
        public virtual Nullable<decimal> FileDefaultCurrencyRate { get; set; }
        public virtual string QuoteItemCurrencySymbol { get; set; }
        public virtual string FileDefaultCurrencySymbol { get; set; }
    }
    public class FileQuoteCharge
    {
        public int QChargeKey { get; set; }
        public int QChargeFileKey { get; set; }
        public int QChargeHdrKey { get; set; }
        public int QChargeSort { get; set; }
        public int? QChargeQCDKey { get; set; }
        public int? QChargeChargeKey { get; set; }
        public string QChargeMemo { get; set; }
        public decimal QChargeQty { get; set; }
        public decimal QChargeCost { get; set; }
        public decimal QChargePrice { get; set; }
        public bool QChargeCostEstimated { get; set; }
        public string QChargeCurrencyCode { get; set; }
        public decimal QChargeCurrencyRate { get; set; }
        public int? QChargeFreightCompany { get; set; }
        public bool QChargePrint { get; set; }
        public string x_FreightCompany { get; set; }
        public string x_ChargeDescription { get; set; }
        public string x_ChargeCurrency { get; set; }
    }
    public class qfrmFileQuoteDetailsSub
    {
        public int QuoteKey { get; set; }
        public int QuoteFileKey { get; set; }
        public int QuoteSort { get; set; }
        public int QuoteQty { get; set; }
        public int QuoteVendorKey { get; set; }
        public int QuoteItemKey { get; set; }
        public decimal QuoteItemCost { get; set; }
        public decimal QuoteItemPrice { get; set; }
        public decimal QuoteItemLineCost { get; set; }
        public decimal QuoteItemLinePrice { get; set; }
        public string QuoteItemCurrencyCode { get; set; }
        public decimal QuoteItemCurrencyRate { get; set; }
        public Nullable<decimal> QuoteItemWeight { get; set; }
        public Nullable<decimal> QuoteItemVolume { get; set; }
        public string QuoteItemMemoCustomer { get; set; }
        public bool QuoteItemMemoCustomerMoveBottom { get; set; }
        public string QuoteItemMemoSupplier { get; set; }
        public bool QuoteItemMemoSupplierMoveBottom { get; set; }
        public Nullable<decimal> LineCost { get; set; }
        public Nullable<decimal> LinePrice { get; set; }
        public Nullable<decimal> LineWeight { get; set; }
        public Nullable<decimal> LineVolume { get; set; }
        public virtual string FileDefaultCurrencyCode { get; set; }
        public virtual Nullable<decimal> FileDefaultCurrencyRate { get; set; }
        public virtual string FileDefaultCurrencySymbol { get; set; }
        public virtual string QuoteItemCurrencySymbol { get; set; }
    }
    public class FileOverview
    {
        public int FileKey { get; set; }
        public int FileYear { get; set; }
        public int FileNum { get; set; }
        public int FileStatusKey { get; set; }
        public int FileQuoteEmployeeKey { get; set; }
        public int? FileOrderEmployeeKey { get; set; }
        public int? FileCustKey { get; set; }
        public int? FileContactKey { get; set; }
        public int? FileCustShipKey { get; set; }
        public string FileReference { get; set; }
        public decimal FileProfitMargin { get; set; }
        public int? FileCurrentVendor { get; set; }
        public string FileModifiedBy { get; set; }
        public Nullable<DateTime> FileModifiedDate { get; set; }
        public string FileCreatedBy { get; set; }
        public DateTime FileCreatedDate { get; set; }
        public Nullable<DateTime> FileDateCustRequired { get; set; }
        public string FileDateCustRequiredNote { get; set; }
        public string FileDefaultCurrencyCode { get; set; }
        public decimal FileDefaultCurrencyRate { get; set; }
        public Nullable<DateTime> FileClosed { get; set; }
        public string CustName { get; set; }
        public string CustPeachtreeID { get; set; }
        public string CustPhone { get; set; }
        public string CustFax { get; set; }
        public string CustEmail { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactFax { get; set; }
        public string ContactEmail { get; set; }
        public string x_FileNum { get; set; }
    }
    public class FileQuoteSummary
    {
        public int QHdrFileKey { get; set; }
        public int QHdrKey { get; set; }
        public string Date { get; set; }
        public string Quote { get; set; }
        public int Vendors { get; set; }
        public string Status { get; set; }
    }
    public class FileVendorSummary
    {
        public int QuoteFileKey { get; set; }
        public int? VendorKey { get; set; }
        public string Vendor { get; set; }
        public decimal Qty { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }
    public class FileEmployeeRoles
    {
        public int FileEmployeeKey { get; set; }
        public int FileEmployeeFileKey { get; set; }
        public int FileEmployeeRoleKey { get; set; }
        public int FileEmployeeEmployeeKey { get; set; }
        public string FileEmployeeModifiedBy { get; set; }
        public Nullable<DateTime> FileEmployeeModifiedDate { get; set; }
        public string FileEmployeeCreatedBy { get; set; }
        public DateTime FileEmployeeCreatedDate { get; set; }
        public string x_EmployeeName { get; set; }
        public string x_RoleName { get; set; }

    }
    public class FileStatusHistorySubDetails
    {
        public int StatusFileKey { get; set; }
        public string StatusQuoteNum { get; set; }
        public DateTime StatusDate { get; set; }
        public int StatusStatusKey { get; set; }
        public string StatusMemo { get; set; }
        public string StatusModifiedBy { get; set; }
        public DateTime StatusModifiedDate { get; set; }
        public int StatusKey { get; set; }
        public string x_Status { get; set; }
        public Nullable<DateTime> x_FileClosed { get; set; }
    }
    public class FileStatusHistory
    {
        public int FileStatusKey { get; set; }
        public int FileStatusFileKey { get; set; }
        public DateTime FileStatusDate { get; set; }
        public int FileStatusStatusKey { get; set; }
        public string FileStatusMemo { get; set; }
        public string FileStatusModifiedBy { get; set; }
        public DateTime FileStatusModifiedDate { get; set; }
    }
    public class qfrmFileStatusHistory
    {
        public int FileKey { get; set; }
        public string FileModifiedBy { get; set; }
        public Nullable<DateTime> FileModifiedDate { get; set; }
        public int? EmployeeKey { get; set; }
        public string EmployeeEmail { get; set; }
        public string CustEmail { get; set; }
        public string FileReference { get; set; }
        public string CustName { get; set; }
        public string ContactTitle { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
    }
    public class FileQuoteStatusHistory
    {
        public int QStatusKey { get; set; }
        public int QStatusFileKey { get; set; }
        public int QStatusQHdrKey { get; set; }
        public string QStatusQuoteNumSnapshot { get; set; }
        public DateTime QStatusDate { get; set; }
        public int QStatusStatusKey { get; set; }
        public string QStatusMemo { get; set; }
        public string QStatusModifiedBy { get; set; }
        public DateTime QStatusModifiedDate { get; set; }
        public string x_Status { get; set; }
    }
    public class FileQuoteStatusHistorySubDetails
    {
        public int QStatusKey { get; set; }
        public int QStatusFileKey { get; set; }
        public string QStatusQuoteNum { get; set; }
        public DateTime QStatusDate { get; set; }
        public int QStatusStatusKey { get; set; }
        public string QStatusMemo { get; set; }
        public string QStatusModifiedBy { get; set; }
        public DateTime QStatusModifiedDate { get; set; }
        public string x_Status { get; set; }
        public Nullable<DateTime> x_FileClosed { get; set; }
    }
    public class qfrmFileQuoteStatusHistory
    {
        public int QHdrKey { get; set; }
        public int QHdrFileKey { get; set; }
        public string QuoteNum { get; set; }
        public int? EmployeeKey { get; set; }
        public string EmployeeEmail { get; set; }
        public string CustEmail { get; set; }
        public string ForwarderEmail { get; set; }
        public Nullable<DateTime> QHdrClosed { get; set; }
        public string QHdrModifiedBy { get; set; }
        public Nullable<DateTime> QHdrModifiedDate { get; set; }
        public string CustName { get; set; }
        public string ContactTitle { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string FileReference { get; set; }
    }
    public class FileQuoteVendorInfo
    {
        public int FVFileKey { get; set; }
        public int? FVQHdrKey { get; set; }
        public int FVVendorKey { get; set; }
        public int? FVVendorContactKey { get; set; }
        public decimal FVProfitMargin { get; set; }
        public int? FVPaymentTerms { get; set; }
        public bool FVFreightDirect { get; set; }
        public int FVFreightDestination { get; set; }
        public string FVFreightDestinationZip { get; set; }
        public int FVFreightShipmentType { get; set; }
        public int? FVWarehouseKey { get; set; }
        public decimal FVFreightCost { get; set; }
        public decimal FVFreightPrice { get; set; }
        public string FVLeadTime { get; set; }
        public decimal FVDiscount { get; set; }
        public decimal FVDiscountPercent { get; set; }
        public string FVDiscountCurrencyCode { get; set; }
        public decimal FVDiscountCurrencyRate { get; set; }
        public string FVExcelFilename { get; set; }
        public Nullable<decimal> FVTotalWeight { get; set; }
        public Nullable<decimal> FVTotalVolume { get; set; }
        public string FVRFQFileName { get; set; }
        public Nullable<DateTime> FVRFQDate { get; set; }
        public Nullable<DateTime> FVSentDate { get; set; }
        public bool FVQuoteInfoConfirmed { get; set; }
        public string FVQuotePONotes { get; set; }
        public string FVPOCurrencyCode { get; set; }
        public decimal FVPOCurrencyRate { get; set; }
        public string x_QuoteNum { get; set; }
    }
    public class FileQuoteHeader
    {
        public int QHdrKey { get; set; }
        public int QHdrFileKey { get; set; }
        public string QHdrPrefix { get; set; }
        public int QHdrNum { get; set; }
        public string QHdrRevision { get; set; }
        public DateTime QHdrDate { get; set; }
        public Nullable<DateTime> QHdrGoodThruDate { get; set; }
        public int QHdrCustPaymentTerms { get; set; }
        public string QHdrMemo { get; set; }
        public bool QHdrExFactoryOption { get; set; }
        public int? QHdrExFactoryLocation { get; set; }
        public bool QHdrFOBOption { get; set; }
        public int? QHdrFOBLocation { get; set; }
        public int QHdrCIFOption { get; set; }
        public int? QHdrCIFLocation { get; set; }
        public bool QHdrFreightDirect { get; set; }
        public string QHdrLeadTime { get; set; }
        public int? QHdrCarrierKey { get; set; }
        public int? QHdrWarehouseKey { get; set; }
        public int QHdrShipType { get; set; }
        public Nullable<DateTime> QHdrSentDate { get; set; }
        public string QHdrCurrencyCode { get; set; }
        public decimal QHdrCurrencyRate { get; set; }
        public string QHdrProdDescription { get; set; }
        public string QHdrShippingDescription { get; set; }
        public string QHdrCustRefNum { get; set; }
        public string QHdrModifiedBy { get; set; }
        public Nullable<DateTime> QHdrModifiedDate { get; set; }
        public string QHdrCreatedBy { get; set; }
        public Nullable<DateTime> QHdrCreatedDate { get; set; }
        public Nullable<DateTime> QHdrQuoteConfirmationDate { get; set; }
        public Nullable<bool> QHdrInsurance { get; set; }
        public int? QHdrInspectorKey { get; set; }
        public string QHdrInspectionNum { get; set; }
        public string QHdrDUINum { get; set; }
        public int? QHdrJobKey { get; set; }
        public Nullable<DateTime> QHdrClosed { get; set; }
        public string x_FileCurrencyCode { get; set; }
        public decimal x_FileCurrencyRate { get; set; }
        public string x_FileReference { get; set; }
        public string x_CustName { get; set; }
        public string x_CustLanguageCode { get; set; }
        public string x_CustContactName { get; set; }
        public string x_Status { get; set; }
        public virtual string JobNum { get; set; }
        public virtual string QuoteNum { get; set; }
    }
    public class FileQuoteChargesSubTotals
    {
        public int QSTQHdrKey { get; set; }
        public int QSTSubTotalKey { get; set; }
        public int? QSTLocation { get; set; }
        public int? x_SubTotalSort { get; set; }
        public string x_Location { get; set; }
        public string x_Category { get; set; }
    }
    public class ChargeCategories
    {
        public int ChargeKey { get; set; }
        public string ChargePeachtreeID { get; set; }
        public string ChargePeachtreeJobPhaseID { get; set; }
        public int? ChargePeachtreeJobCostIndex { get; set; }
        public int? ChargeGLAccount { get; set; }
        public int ChargeSubTotalCategory { get; set; }
        public int? ChargeAPAccount { get; set; }
        public string x_DescriptionText { get; set; }
        public string x_DescriptionLanguageCode { get; set; }
        public string x_DescriptionMemo { get; set; }

    }
    public class ChargeCategoryDescriptions
    {
        public int DescriptionKey { get; set; }
        public int DescriptionChargeKey { get; set; }
        public string DescriptionLanguageCode { get; set; }
        public string DescriptionText { get; set; }
        public string DescriptionMemo { get; set; }
    }
    public class FileQuoteItemsSummary
    {
        public int QSummaryKey { get; set; }
        public int QSummaryQHdrKey { get; set; }
        public int QSummarySort { get; set; }
        public decimal QSummaryQty { get; set; }
        public int? QSummaryVendorKey { get; set; }
        public string QSummaryItemNum { get; set; }
        public string QSummaryDescription { get; set; }
        public decimal QSummaryPrice { get; set; }
        public decimal QSummaryLinePrice { get; set; }
        public string QSummaryCurrencyCode { get; set; }
        public decimal QSummaryCurrencyRate { get; set; }
        public int QSummaryDescriptionFontColor { get; set; }
        public string x_VendorName { get; set; }
    }
    public class tlkpGenericLists
    {
        public int ListKey { get; set; }
        public int ListCategory { get; set; }
        public string ListText { get; set; }
    }
    public class tlkpInvoiceSubTotalCategories
    {
        public int SubTotalKey { get; set; }
        public int SubTotalSort { get; set; }
        public virtual string STDescriptionText { get; set; }
    }
    public class LeadTime
    {
        public string FVLeadTime { get; set; }
    }
    public class qfrmFileQuoteConfirmation
    {
        public int FileKey { get; set; }
        public int FileYear { get; set; }
        public int FileNum { get; set; }
        public int FileStatusKey { get; set; }
        public int FileQuoteEmployeeKey { get; set; }
        public int? FileOrderEmployeeKey { get; set; }
        public int? FileCustKey { get; set; }
        public int? FileContactKey { get; set; }
        public int? FileCustShipKey { get; set; }
        public string FileReference { get; set; }
        public decimal FileProfitMargin { get; set; }
        public int? FileCurrentVendor { get; set; }
        public string FileModifiedBy { get; set; }
        public Nullable<DateTime> FileModifiedDate { get; set; }
        public string FileCreatedBy { get; set; }
        public DateTime FileCreatedDate { get; set; }
        public Nullable<DateTime> FileDateCustRequired { get; set; }
        public string FileDateCustRequiredNote { get; set; }
        public string FileDefaultCurrencyCode { get; set; }
        public decimal FileDefaultCurrencyRate { get; set; }
        public Nullable<DateTime> FileClosed { get; set; }
        public string CustName { get; set; }
        public string CustFax { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public int QHdrKey { get; set; }
        public string QHdrPrefix { get; set; }
        public int QHdrFileKey { get; set; }
        public int QHdrNum { get; set; }
        public string QHdrRevision { get; set; }
        public DateTime QHdrDate { get; set; }
        public Nullable<DateTime> QHdrGoodThruDate { get; set; }
        public int QHdrCustPaymentTerms { get; set; }
        public string QHdrMemo { get; set; }
        public bool QHdrExFactoryOption { get; set; }
        public int? QHdrExFactoryLocation { get; set; }
        public bool QHdrFOBOption { get; set; }
        public int? QHdrFOBLocation { get; set; }
        public int QHdrCIFOption { get; set; }
        public int? QHdrCIFLocation { get; set; }
        public bool QHdrFreightDirect { get; set; }
        public string QHdrLeadTime { get; set; }
        public int? QHdrCarrierKey { get; set; }
        public int? QHdrWarehouseKey { get; set; }
        public int QHdrShipType { get; set; }
        public Nullable<DateTime> QHdrSentDate { get; set; }
        public string QHdrCurrencyCode { get; set; }
        public decimal QHdrCurrencyRate { get; set; }
        public string QHdrProdDescription { get; set; }
        public string QHdrShippingDescription { get; set; }
        public string QHdrCustRefNum { get; set; }
        public string QHdrModifiedBy { get; set; }
        public Nullable<DateTime> QHdrModifiedDate { get; set; }
        public string QHdrCreatedBy { get; set; }
        public Nullable<DateTime> QHdrCreatedDate { get; set; }
        public Nullable<DateTime> QHdrQuoteConfirmationDate { get; set; }
        public Nullable<bool> QHdrInsurance { get; set; }
        public int? QHdrInspectorKey { get; set; }
        public string QHdrInspectionNum { get; set; }
        public string QHdrDUINum { get; set; }
        public int? QHdrJobKey { get; set; }
    }
    public class qfrmFileQuoteConfirmationSVInfo
    {
        public int FVFileKey { get; set; }
        public int? FVQHdrKey { get; set; }
        public int FVVendorKey { get; set; }
        public int? FVVendorContactKey { get; set; }
        public decimal FVProfitMargin { get; set; }
        public int? FVPaymentTerms { get; set; }
        public bool FVFreightDirect { get; set; }
        public int FVFreightDestination { get; set; }
        public string FVFreightDestinationZip { get; set; }
        public int FVFreightShipmentType { get; set; }
        public int? FVWarehouseKey { get; set; }
        public decimal FVFreightCost { get; set; }
        public decimal FVFreightPrice { get; set; }
        public string FVLeadTime { get; set; }
        public decimal FVDiscount { get; set; }
        public decimal FVDiscountPercent { get; set; }
        public string FVDiscountCurrencyCode { get; set; }
        public decimal FVDiscountCurrencyRate { get; set; }
        public string FVExcelFilename { get; set; }
        public Nullable<decimal> FVTotalWeight { get; set; }
        public Nullable<decimal> FVTotalVolume { get; set; }
        public string FVRFQFileName { get; set; }
        public Nullable<DateTime> FVRFQDate { get; set; }
        public Nullable<DateTime> FVSentDate { get; set; }
        public bool FVQuoteInfoConfirmed { get; set; }
        public string FVQuotePONotes { get; set; }
        public string FVPOCurrencyCode { get; set; }
        public decimal FVPOCurrencyRate { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string VendorFax { get; set; }

        public virtual Nullable<decimal> FVTotalWeightTag { get; set; }
        public virtual Nullable<decimal> FVTotalVolumeTag { get; set; }
    }
    public class qryFileQuoteVendorSummaryWithDiscount
    {
        public int FileKey { get; set; }
        public int? QHdrKey { get; set; }
        public int VendorKey { get; set; }
        public string Vendor { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> CostAfterDiscount { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Margin { get; set; }
        public string Currency { get; set; }
        public string CurrencyFormat { get; set; }
    }
    public class qfrmItemPriceHistoryPurchaseOrder
    {
        public int POKey { get; set; }
        public int ItemKey { get; set; }
        public int CustKey { get; set; }
        public string PONum { get; set; }
        public decimal CostFromSupplier { get; set; }
        public decimal PricePaidByCustomer { get; set; }
        public string Customer { get; set; }
        public Nullable<DateTime> Date { get; set; }
    }
    public class qryFileQuoteSearch
    {
        public int QHdrKey { get; set; }
        public string QHdrNum { get; set; }
        public DateTime QHdrDate { get; set; }
        public int FileKey { get; set; }
        public string FileNum { get; set; }
        public string JobNum { get; set; }
        public string CustName { get; set; }
        public string CustContact { get; set; }
        public string CustShipName { get; set; }
        public string FileReference { get; set; }
        public string WarehouseName { get; set; }
        public string QHdrShipType { get; set; }
        public string QHdrCurrencyCode { get; set; }
        public string QHdrProdDescription { get; set; }
        public string QHdrCustRefNum { get; set; }
        public Nullable<bool> QHdrInsurance { get; set; }
        public string QHdrInspectionNum { get; set; }
        public string QHdrDUINum { get; set; }
        public string SalesEmployee { get; set; }
        public string OrderEmployee { get; set; }
    }
    public class qryFileSearch
    {
        public int FileKey { get; set; }
        public string FileNum { get; set; }
        public string SalesEmployee { get; set; }
        public string OrderEmployee { get; set; }
        public string CustName { get; set; }
        public string CustContact { get; set; }
        public string CustShipName { get; set; }
        public string FileReference { get; set; }
        public string FileCurrencyCode { get; set; }
        public int? QHdrKey { get; set; }
        public string QHdrNum { get; set; }
        public string VendorName { get; set; }
        public string VendorContact { get; set; }
        public string ShipType { get; set; }
        public string WarehouseName { get; set; }
        public string VendorCurrencyCode { get; set; }
    }
    public class qsumFileQuoteVendorSummary
    {
        public int FileKey { get; set; }
        public int? QHdrKey { get; set; }
        public int VendorKey { get; set; }
        public string Vendor { get; set; }
        public int? Qty { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Currency { get; set; }
    }
    public class FileQuoteJoin
    {
        public int FileKey { get; set; }
        public int FileYear { get; set; }
        public int FileNum { get; set; }
        public int FileStatusKey { get; set; }
        public int FileQuoteEmployeeKey { get; set; }
        public int? FileOrderEmployeeKey { get; set; }
        public int? FileCustKey { get; set; }
        public int? FileContactKey { get; set; }
        public int? FileCustShipKey { get; set; }
        public string FileReference { get; set; }
        public decimal FileProfitMargin { get; set; }
        public int? FileCurrentVendor { get; set; }
        public string FileModifiedBy { get; set; }
        public Nullable<DateTime> FileModifiedDate { get; set; }
        public string FileCreatedBy { get; set; }
        public DateTime FileCreatedDate { get; set; }
        public Nullable<DateTime> FileDateCustRequired { get; set; }
        public string FileDateCustRequiredNote { get; set; }
        public string FileDefaultCurrencyCode { get; set; }
        public decimal FileDefaultCurrencyRate { get; set; }
        public Nullable<DateTime> FileClosed { get; set; }
        public int QHdrKey { get; set; }
        public int QHdrFileKey { get; set; }
        public string QHdrPrefix { get; set; }
        public int QHdrNum { get; set; }
        public string QHdrRevision { get; set; }
        public DateTime QHdrDate { get; set; }
        public Nullable<DateTime> QHdrGoodThruDate { get; set; }
        public int QHdrCustPaymentTerms { get; set; }
        public string QHdrMemo { get; set; }
        public bool QHdrExFactoryOption { get; set; }
        public int? QHdrExFactoryLocation { get; set; }
        public bool QHdrFOBOption { get; set; }
        public int? QHdrFOBLocation { get; set; }
        public int QHdrCIFOption { get; set; }
        public int? QHdrCIFLocation { get; set; }
        public bool QHdrFreightDirect { get; set; }
        public string QHdrLeadTime { get; set; }
        public int? QHdrCarrierKey { get; set; }
        public int? QHdrWarehouseKey { get; set; }
        public int QHdrShipType { get; set; }
        public Nullable<DateTime> QHdrSentDate { get; set; }
        public string QHdrCurrencyCode { get; set; }
        public decimal QHdrCurrencyRate { get; set; }
        public string QHdrProdDescription { get; set; }
        public string QHdrShippingDescription { get; set; }
        public string QHdrCustRefNum { get; set; }
        public string QHdrModifiedBy { get; set; }
        public Nullable<DateTime> QHdrModifiedDate { get; set; }
        public string QHdrCreatedBy { get; set; }
        public Nullable<DateTime> QHdrCreatedDate { get; set; }
        public Nullable<DateTime> QHdrQuoteConfirmationDate { get; set; }
        public Nullable<bool> QHdrInsurance { get; set; }
        public int? QHdrInspectorKey { get; set; }
        public string QHdrInspectionNum { get; set; }
        public string QHdrDUINum { get; set; }
        public int? QHdrJobKey { get; set; }
        public Nullable<DateTime> QHdrClosed { get; set; }
    }
    public class qryFileQuoteToPeachtree
    {
        public int FileKey { get; set; }
        public int QHdrKey { get; set; }
        public int CustKey { get; set; }
        public string CustPeachtreeID { get; set; }
        public string CustLanguageCode { get; set; }
        public string FileReference { get; set; }
        public int QHdrCustPaymentTerms { get; set; }
        public DateTime QHdrDate { get; set; }
        public string QuoteNum { get; set; }
        public Nullable<DateTime> QuoteGoodThru { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress1 { get; set; }
        public string ShipAddress2 { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipZip { get; set; }
        public string ShipCountry { get; set; }
        public string ShipType { get; set; }
        public string EmployeePeachtreeID { get; set; }
        public int QuoteQty { get; set; }
        public int QuoteItemKey { get; set; }
        public int VendorKey { get; set; }
        public string VendorPeachtreeItemID { get; set; }
        public string VendorPeachtreeID { get; set; }
        public string VendorName { get; set; }
        public string ItemNum { get; set; }
        public Nullable<decimal> QuoteItemPrice { get; set; }
        public Nullable<decimal> QuoteItemLinePrice { get; set; }
        public string QuoteItemMemoCustomer { get; set; }
        public bool QuoteItemMemoCustomerMoveBottom { get; set; }
    }
    public class qryFileQuoteChargesToPeachtree
    {
        public int QHdrKey { get; set; }
        public string QCDPeachtreeID { get; set; }
        public string QChargeMemo { get; set; }
        public int? QCDGLAccount { get; set; }
        public Nullable<decimal> QChargePrice { get; set; }
    }
    public class qsumFileQuoteChargesByGLAccount
    {
        public int FileKey { get; set; }
        public int QHdrKey { get; set; }
        public int? QCDGLAccount { get; set; }
        public string FreightCo { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<decimal> Price { get; set; }
        public int? Estimated { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyFormat { get; set; }
    }
    public class QuoteTotal
    {
        public Decimal LinePrice { get; set; }
        public Decimal CurrencyRate { get; set; }
    }
}