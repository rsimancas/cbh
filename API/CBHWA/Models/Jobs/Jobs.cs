namespace CBHWA.Models
{
    using System;
    using System.Collections.Generic;

    public class Job
    {
        public int JobKey { get; set; }
        public string JobFirstName { get; set; }
        public string JobMiddleInitial { get; set; }
        public string JobLastName { get; set; }
        public int? JobTitleCode { get; set; }
        public string JobEmail { get; set; }
        public string JobEmailCC { get; set; }
        public string JobAddress1 { get; set; }
        public string JobCity { get; set; }
        public string JobState { get; set; }
        public string JobZip { get; set; }
        public string JobPhone { get; set; }
        public string JobLogin { get; set; }
        public int JobStatusCode { get; set; }
        public string JobPeachtreeID { get; set; }
        public int JobLocationKey { get; set; }
        public string JobPassword { get; set; }
        public string JobModifiedBy { get; set; }
        public Nullable<System.DateTime> JobModifiedDate { get; set; }
        public string JobCreatedBy { get; set; }
        public System.DateTime JobCreatedDate { get; set; }
        public bool JobSecurityLevel { get; set; }
    }

    public class JobList
    {
        public int JobKey { get; set; }
        public DateTime Date { get; set; }
        public string JobNum { get; set; }
        public string Customer { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public string JobCreatedBy { get; set; }
        public DateTime JobClosed { get; set; }
        public string JobStatusDesc { get; set; }
        public string StatusModifiedBy { get; set; }
        public string StatusModifiedDate { get; set; }
        public string CustCurrencyCode { get; set; }
        public decimal CustCurrencyRate { get; set; }
        public int row { get; set; }
        public int JobCustShipKey { get; set; }
        public int JobWarehouseKey { get; set; }
        public int JobShipType { get; set; }
        public int JobCustKey { get; set; }

        public virtual int? JobQHdrKey { get; set; }
        public virtual string Quote { get; set; }
        public virtual int? QHdrFileKey { get; set; }
        public virtual string FileNum { get; set; }
    }

    public class JobHeader
    {
        public int JobKey { get; set; }
        public int? JobQHdrKey { get; set; }
        public int JobYear { get; set; }
        public int JobNum { get; set; }
        public string JobProdDescription { get; set; }
        public string JobShippingDescription { get; set; }
        public string JobReference { get; set; }
        public int? JobSalesEmployeeKey { get; set; }
        public int JobOrderEmployeeKey { get; set; }
        public int JobCustKey { get; set; }
        public int? JobContactKey { get; set; }
        public int? JobCustShipKey { get; set; }
        public string JobCustRefNum { get; set; }
        public Nullable<DateTime> JobDateCustRequired { get; set; }
        public string JobDateCustRequiredNote { get; set; }
        public string JobCustCurrencyCode { get; set; }
        public decimal JobCustCurrencyRate { get; set; }
        public int JobCustPaymentTerms { get; set; }
        public int? JobCarrierKey { get; set; }
        public int? JobWarehouseKey { get; set; }
        public int JobShipType { get; set; }
        public Nullable<DateTime> JobShipDate { get; set; }
        public Nullable<DateTime> JobArrivalDate { get; set; }
        public int? JobShipmentCarrier { get; set; }
        public string JobCarrierRefNum { get; set; }
        public string JobCarrierVessel { get; set; }
        public int? JobInspectorKey { get; set; }
        public string JobInspectionNum { get; set; }
        public string JobInspectionCertificateNum { get; set; }
        public string JobDUINum { get; set; }
        public Nullable<DateTime> JobClosed { get; set; }
        public Nullable<DateTime> JobComplete { get; set; }
        public int? JobPTIndex { get; set; }
        public bool JobExemptFromProfitReport { get; set; }
        public bool JobExemptFromPronacaReport { get; set; }
        public Nullable<decimal> JobCurrencyLockedRate { get; set; }
        public Nullable<DateTime> JobCurrencyLockedDate { get; set; }
        public int JobStatusKey { get; set; }
        public string JobModifiedBy { get; set; }
        public Nullable<DateTime> JobModifiedDate { get; set; }
        public string JobCreatedBy { get; set; }
        public DateTime JobCreatedDate { get; set; }
        public string x_JobNumFormatted { get; set; }
    }

    public class JobEmployeeRoles
    {
        public int JobEmployeeKey { get; set; }
        public int JobEmployeeJobKey { get; set; }
        public int JobEmployeeRoleKey { get; set; }
        public int JobEmployeeEmployeeKey { get; set; }
        public string JobEmployeeModifiedBy { get; set; }
        public Nullable<DateTime> JobEmployeeModifiedDate { get; set; }
        public string JobEmployeeCreatedBy { get; set; }
        public DateTime JobEmployeeCreatedDate { get; set; }
        public string x_EmployeeName { get; set; }
        public string x_RoleName { get; set; }
    }

    public class qJobPurchaseOrder
    {
        public int POKey { get; set; }
        public int JobKey { get; set; }
        public DateTime Date { get; set; }
        public string PONum { get; set; }
        public string Vendor { get; set; }
        public string Status { get; set; }
        public decimal Cost { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
    }

    public class qJobInvoice
    {
        public int InvoiceKey { get; set; }
        public int? JobKey { get; set; }
        public DateTime Date { get; set; }
        public string Invoice { get; set; }
        public string BillTo { get; set; }
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class qJobOverview
    {
        public int JobKey { get; set; }
        public int? JobQHdrKey { get; set; }
        public string QuoteNum { get; set; }
        public string JobNum { get; set; }
        public Nullable<DateTime> JobShipDate { get; set; }
        public Nullable<DateTime> JobArrivalDate { get; set; }
        public int? JobShipmentCarrier { get; set; }
        public string JobCarrierRefNum { get; set; }
        public string JobCarrierVessel { get; set; }
        public string JobInspectionCertificateNum { get; set; }
        public int JobCustPaymentTerms { get; set; }
        public Nullable<DateTime> JobClosed { get; set; }
        public Nullable<DateTime> JobComplete { get; set; }
        public string JobModifiedBy { get; set; }
        public Nullable<DateTime> JobModifiedDate { get; set; }
        public string JobCreatedBy { get; set; }
        public DateTime JobCreatedDate { get; set; }
        public bool JobExemptFromProfitReport { get; set; }
        public string CustName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactFirstName { get; set; }
        public string CustPhone { get; set; }
        public string CustFax { get; set; }
        public string CustEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactFax { get; set; }
        public string ContactEmail { get; set; }
        public string x_Phone { get; set; }
        public string x_Email { get; set; }
        public string x_Fax { get; set; }
        public string x_ContactName { get; set; }
        public string x_JobShipmentCarrierText { get; set; }
        public string CustCurrencyCode { get; set; }
        public decimal CustCurrencyRate { get; set; }
        public string x_JobCustPaymentTerms { get; set; }
        public string x_Info { get; set; }
    }

    public class JobPurchaseOrder
    {
        public int POKey { get; set; }
        public int PONum { get; set; }
        public int PORevisionNum { get; set; }
        public int PONumShipment { get; set; }
        public int POJobKey { get; set; }
        public int? POInvoiceKey { get; set; }
        public int POVendorKey { get; set; }
        public int? POVendorContactKey { get; set; }
        public string POVendorReference { get; set; }
        public int? POVendorOriginAddress { get; set; }
        public DateTime PODate { get; set; }
        public DateTime POGoodThruDate { get; set; }
        public string POLeadTime { get; set; }
        public Nullable<decimal> PODefaultProfitMargin { get; set; }
        public int? POVendorPaymentTerms { get; set; }
        public string POCurrencyCode { get; set; }
        public Nullable<decimal> POCurrencyRate { get; set; }
        public int POShipmentType { get; set; }
        public int POFreightDestination { get; set; }
        public string POFreightDestinationZip { get; set; }
        public int? POCustShipKey { get; set; }
        public int? POWarehouseKey { get; set; }
        public Nullable<DateTime> POShipETA { get; set; }
        public Nullable<DateTime> POSubmittedDate { get; set; }
        public string POStatusReport { get; set; }
        public bool POClosed { get; set; }
        public string POModifiedBy { get; set; }
        public Nullable<DateTime> POModifiedDate { get; set; }
        public string POCreatedBy { get; set; }
        public DateTime POCreatedDate { get; set; }
        public int? POPeachtreeNRecord { get; set; }
        public bool POUseOriginAddress { get; set; }
        public string x_JobNumFormatted { get; set; }
        public string x_PONumFormatted { get; set; }
    }

    public class JobPurchaseOrderItem
    {
        public int POItemsKey { get; set; }
        public int POItemsJobKey { get; set; }
        public int POItemsPOKey { get; set; }
        public int? POItemsSort { get; set; }
        public decimal POItemsQty { get; set; }
        public int POItemsItemKey { get; set; }
        public decimal POItemsCost { get; set; }
        public decimal POItemsPrice { get; set; }
        public decimal POItemsLineCost { get; set; }
        public decimal POItemsLinePrice { get; set; }
        public string POItemsCurrencyCode { get; set; }
        public decimal POItemsCurrencyRate { get; set; }
        public decimal POItemsWeight { get; set; }
        public decimal POItemsVolume { get; set; }
        public decimal POItemsLineWeight { get; set; }
        public decimal POItemsLineVolume { get; set; }
        public int? POItemsBackorderQty { get; set; }
        public string POItemsMemoCustomer { get; set; }
        public bool POItemsMemoCustomerMoveBottom { get; set; }
        public string POItemsMemoVendor { get; set; }
        public bool POItemsMemoVendorMoveBottom { get; set; }
        public int? POItemsQuoteItemKey { get; set; }
        public string x_ItemName { get; set; }
        public decimal x_LineCost { get; set; }
        public decimal x_LinePrice { get; set; }
        public decimal x_LineWeight { get; set; }
        public decimal x_LineVolume { get; set; }
        public string x_ItemNum { get; set; }
    }

    public class JobPurchaseOrderCharge
    {
        public int POChargesKey { get; set; }
        public int POChargesJobKey { get; set; }
        public int POChargesPOKey { get; set; }
        public int POChargesSort { get; set; }
        public int POChargesChargeKey { get; set; }
        public string POChargesMemo { get; set; }
        public decimal POChargesQty { get; set; }
        public decimal POChargesCost { get; set; }
        public decimal POChargesPrice { get; set; }
        public string POChargesCurrencyCode { get; set; }
        public decimal POChargesCurrencyRate { get; set; }
        public int? POChargesFreightCompany { get; set; }
        public decimal x_UnitCost { get; set; }
        public decimal x_UnitPrice { get; set; }
        public string x_DescriptionText { get; set; }
    }

    public class JobPurchaseOrderInstruction
    {
        public int POInstructionsKey { get; set; }
        public int POInstructionsPOKey { get; set; }
        public int POInstructionsStep { get; set; }
        public int? POInstructionsInstructionKey { get; set; }
        public string POInstructionsMemo { get; set; }
        public int POInstructionsMemoFontColor { get; set; }
        public string x_ITextMemo { get; set; }
        public string x_NotesFontColor { get; set; }
    }

    public class tlkpChargeCategory
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
    }

    public class tlkpJobPurchaseOrderInstruction
    {
        public int InstructionKey { get; set; }
        public int InstructionSort { get; set; }
        public int InstructionCategory { get; set; }
        public string x_ITextMemo { get; set; }
    }

    public class JobPurchaseOrderStatusHistory
    {
        public int POStatusKey { get; set; }
        public int POStatusJobKey { get; set; }
        public int POStatusPOKey { get; set; }
        public DateTime POStatusDate { get; set; }
        public int POStatusStatusKey { get; set; }
        public string POStatusMemo { get; set; }
        public bool POStatusPublic { get; set; }
        public string POStatusModifiedBy { get; set; }
        public DateTime POStatusModifiedDate { get; set; }
        public string x_Status { get; set; }
        public string x_JobClosed { get; set; }
    }

    public class qfrmJobPurchaseOrderStatusHistory
    {
        public int POKey { get; set; }
        public int POJobKey { get; set; }
        public string VendorName { get; set; }
        public Nullable<DateTime> POShipETA { get; set; }
        public int? EmployeeKey { get; set; }
        public string EmployeeEmail { get; set; }
        public string CustEmail { get; set; }
        public string ForwarderEmail { get; set; }
        public string JobProdDescription { get; set; }
        public string JobCustRefNum { get; set; }
        public string VendorDisplayToCust { get; set; }
        public string QuoteNum { get; set; }
    }

    public class InvoiceHeader
    {
        public int InvoiceKey { get; set; }
        public int? InvoiceJobKey { get; set; }
        public int InvoicePrefix { get; set; }
        public int InvoiceNum { get; set; }
        public int InvoiceRevisionNum { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int InvoiceRecipient { get; set; }
        public int? InvoiceVendorKey { get; set; }
        public int? InvoiceVendorContactKey { get; set; }
        public int? InvoiceCustKey { get; set; }
        public int? InvoiceCustContactKey { get; set; }
        public string InvoiceBillingName { get; set; }
        public string InvoiceBillingAddress1 { get; set; }
        public string InvoiceBillingAddress2 { get; set; }
        public string InvoiceBillingCity { get; set; }
        public string InvoiceBillingState { get; set; }
        public string InvoiceBillingZip { get; set; }
        public int? InvoiceBillingCountryKey { get; set; }
        public int? InvoiceCustShipKey { get; set; }
        public string InvoiceCustReference { get; set; }
        public int InvoiceEmployeeKey { get; set; }
        public string InvoiceCurrencyCode { get; set; }
        public decimal InvoiceCurrencyRate { get; set; }
        public int InvoicePaymentTerms { get; set; }
        public string InvoiceMemo { get; set; }
        public string InvoiceModifiedBy { get; set; }
        public Nullable<DateTime> InvoiceModifiedDate { get; set; }
        public string InvoiceCreatedBy { get; set; }
        public Nullable<DateTime> InvoiceCreatedDate { get; set; }
        public int? InvoiceMemoFont { get; set; }
        public virtual int? CustKey { get; set; }
        public virtual string CustPeachtreeID { get; set; }
        public virtual int? CustPeachtreeIndex { get; set; }
        public virtual string CustName { get; set; }
        public virtual string CustAddress1 { get; set; }
        public virtual string CustAddress2 { get; set; }
        public virtual string CustCity { get; set; }
        public virtual string CustState { get; set; }
        public virtual string CustZip { get; set; }
        public virtual int? CustCountryKey { get; set; }
        public virtual string CustPhone { get; set; }
        public virtual string CustFax { get; set; }
        public virtual string CustEmail { get; set; }
        public virtual string CustWebsite { get; set; }
        public virtual int? CustSalesRepKey { get; set; }
        public virtual int? CustOrdersRepKey { get; set; }
        public virtual string CustLanguageCode { get; set; }
        public virtual Nullable<int> CustStatus { get; set; }
        public virtual string CustModifiedBy { get; set; }
        public virtual Nullable<DateTime> CustModifiedDate { get; set; }
        public virtual string CustCreatedBy { get; set; }
        public virtual Nullable<DateTime> CustCreatedDate { get; set; }
        public virtual Nullable<decimal> CustCreditLimit { get; set; }
        public virtual string CustCurrencyCode { get; set; }
        public virtual string CustMemo { get; set; }
        public virtual int? VendorKey { get; set; }
        public virtual string VendorName { get; set; }
        public virtual string VendorPeachtreeID { get; set; }
        public virtual string VendorPeachtreeItemID { get; set; }
        public virtual string VendorPeachtreeJobID { get; set; }
        public virtual string VendorDisplayToCust { get; set; }
        public virtual string VendorContact { get; set; }
        public virtual string VendorAddress1 { get; set; }
        public virtual string VendorAddress2 { get; set; }
        public virtual string VendorCity { get; set; }
        public virtual string VendorState { get; set; }
        public virtual string VendorZip { get; set; }
        public virtual int? VendorCountryKey { get; set; }
        public virtual string VendorPhone { get; set; }
        public virtual string VendorFax { get; set; }
        public virtual string VendorEmail { get; set; }
        public virtual string VendorWebsite { get; set; }
        public virtual string VendorLanguageCode { get; set; }
        public virtual string VendorAcctNum { get; set; }
        public virtual Nullable<bool> VendorCarrier { get; set; }
        public virtual string VendorModifiedBy { get; set; }
        public virtual Nullable<DateTime> VendorModifiedDate { get; set; }
        public virtual string VendorCreatedBy { get; set; }
        public virtual Nullable<DateTime> VendorCreatedDate { get; set; }
        public virtual Nullable<decimal> VendorDefaultCommissionPercent { get; set; }
        public virtual int? JobShipType { get; set; }
        public virtual Nullable<DateTime> JobShipDate { get; set; }
        public virtual string FullInvoiceNum { get; set; }
    }

    public class InvoiceDDL
    {
        public int InvoiceKey { get; set; }
        public string x_InvoiceNum { get; set; }
    }

    public class InvoiceItemsSummary
    {
        public int ISummaryKey { get; set; }
        public int ISummaryInvoiceKey { get; set; }
        public int ISummarySort { get; set; }
        public decimal ISummaryQty { get; set; }
        public int? ISummaryVendorKey { get; set; }
        public string ISummaryItemNum { get; set; }
        public string ISummaryDescription { get; set; }
        public decimal ISummaryPrice { get; set; }
        public decimal ISummaryLinePrice { get; set; }
        public string ISummaryCurrencyCode { get; set; }
        public decimal ISummaryCurrencyRate { get; set; }
        public string x_ItemName { get; set; }
        public string x_ItemVendorName { get; set; }
    }

    public class InvoiceCharge
    {
        public int IChargeKey { get; set; }
        public int IChargeInvoiceKey { get; set; }
        public int IChargeSort { get; set; }
        public int? IChargeChargeKey { get; set; }
        public string IChargeMemo { get; set; }
        public decimal IChargeQty { get; set; }
        public decimal IChargeCost { get; set; }
        public decimal IChargePrice { get; set; }
        public string IChargeCurrencyCode { get; set; }
        public decimal IChargeCurrencyRate { get; set; }
        public virtual decimal x_UnitCost { get; set; }
        public virtual decimal x_UnitPrice { get; set; }
        public virtual string x_ChargeDescription { get; set; }
        public virtual string x_ChargeCurrency { get; set; }
    }

    public class InvoiceChargesSubTotal
    {
        public int ISTInvoiceKey { get; set; }
        public int ISTSubTotalKey { get; set; }
        public int? ISTLocation { get; set; }
        public virtual string x_Category { get; set; }
        public virtual string x_SubTotalSort { get; set; }
        public virtual string x_Location { get; set; }
    }

    public class CommissionInvoice
    {
        public Nullable<decimal> CommissionPCT { get; set; }
        public Nullable<decimal> CommissionTotal { get; set; }
        public string CommissionTotalCurrencyCode { get; set; }
        public Nullable<decimal> CommissionTotalCurrencyRate { get; set; }
        public Nullable<decimal> TotalSale { get; set; }
        public string TotalSaleCurrencyCode { get; set; }
        public Nullable<decimal> TotalSaleCurrencyRate { get; set; }
        public int? VendorContactKey { get; set; }
        public int VendorKey { get; set; }
    }

    public class JobStatusHistory
    {
        public int JobStatusKey { get; set; }
        public int JobStatusJobKey { get; set; }
        public DateTime JobStatusDate { get; set; }
        public int JobStatusStatusKey { get; set; }
        public string JobStatusMemo { get; set; }
        public bool JobStatusPublic { get; set; }
        public string JobStatusModifiedBy { get; set; }
        public Nullable<DateTime> JobStatusModifiedDate { get; set; }
    }

    public class JobStatusHistorySubDetails
    {
        public int StatusKey { get; set; }
        public int StatusJobKey { get; set; }
        public string StatusPONum { get; set; }
        public DateTime StatusDate { get; set; }
        public int StatusStatusKey { get; set; }
        public string StatusMemo { get; set; }
        public int StatusPublic { get; set; }
        public string StatusModifiedBy { get; set; }
        public DateTime StatusModifiedDate { get; set; }
        public string x_Status { get; set; }
    }

    public class qfrmJobStatusHistory
    {
        public int JobKey { get; set; }
        public string JobModifiedBy { get; set; }
        public Nullable<DateTime> JobModifiedDate { get; set; }
        public int? EmployeeKey { get; set; }
        public string EmployeeEmail { get; set; }
        public string CustEmail { get; set; }
        public string ForwarderEmail { get; set; }
        public Nullable<DateTime> JobClosed { get; set; }
        public Nullable<DateTime> JobComplete { get; set; }
        public string QuoteNum { get; set; }
        public string JobProdDescription { get; set; }
        public string JobCustRefNum { get; set; }
        public string CustName { get; set; }
        public string ContactName { get; set; }
        public int JobStatusKey { get; set; }
        public int StatusStatusKey { get; set; }
        public string StatusMemo { get; set; }
        public bool StatusPublic { get; set; }
        public DateTime StatusDate { get; set; }
    }
    public class qryJobSearch
    {
        public int JobKey { get; set; }
        public string JobNum { get; set; }
        public int? QHdrKey { get; set; }
        public string QHdrNum { get; set; }
        public string JobProdDescription { get; set; }
        public string JobReference { get; set; }
        public string JobOrderEmployee { get; set; }
        public string CustName { get; set; }
        public string CustContact { get; set; }
        public string CustShipName { get; set; }
        public string JobCustRefNum { get; set; }
        public string JobCustCurrencyCode { get; set; }
        public string ShipType { get; set; }
        public string InspectionNum { get; set; }
        public string JobDUINum { get; set; }
        public DateTime JobShipDate { get; set; }
        public Nullable<DateTime> JobClosed { get; set; }
    }

    #region Export Invoice To Peachtree
    public class qrptJobInvoice
    {
        public int InvoiceKey { get; set; }
        public string InvoiceNum { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int JobShipType { get; set; }
        public Nullable<DateTime> JobShipDate { get; set; }
        public int InvoicePaymentTerms { get; set; }
        public string InvoiceCustReference { get; set; }
        public string InvoiceCurrencyCode { get; set; }
        public decimal InvoiceCurrencyRate { get; set; }
        public string InvoiceMemo { get; set; }
        public int? JobKey { get; set; }
        public string JobNum { get; set; }
        public string JobCustRefNum { get; set; }
        public string CustLanguageCode { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeMiddleInitial { get; set; }
        public string EmployeeLastName { get; set; }
        public int? EmployeeTitleCode { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeePeachtreeID { get; set; }
        public string InvoiceBillingName { get; set; }
        public string InvoiceBillingAddress1 { get; set; }
        public string InvoiceBillingAddress2 { get; set; }
        public string InvoiceBillingCity { get; set; }
        public string InvoiceBillingState { get; set; }
        public string InvoiceBillingZip { get; set; }
        public string InvoiceBillingCountryName { get; set; }
        public string ContactTitle { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress1 { get; set; }
        public string ShipAddress2 { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipZip { get; set; }
        public string ShipCountry { get; set; }
        public string CurrencyFormat { get; set; }
        public int LocationKey { get; set; }
        public string CustPeachtreeID { get; set; }
        public string QuoteNum { get; set; }
        public string JobDUINum { get; set; }
        public int InvoiceRecipient { get; set; }
        public string Expr1 { get; set; }
        public string VendorPeachtreeID { get; set; }
        public int? JobPTIndex { get; set; }
        public int? InvoiceCustKey { get; set; }
        public int? InvoiceVendorKey { get; set; }
        public int? InvoiceMemoFont { get; set; }
    }

    public class qryJobInvoiceToPeachtreeItem
    {
        public decimal ItemQty { get; set; }
        public string ItemID { get; set; }
        public string ItemNum { get; set; }
        public string ItemDescription { get; set; }
        public Nullable<decimal> ItemPrice { get; set; }
        public Nullable<decimal> ItemLinePrice { get; set; }
        public int InvoiceKey { get; set; }
        public string ItemMemo { get; set; }
        public int ItemMemoMoveBottom { get; set; }
        public int? VendorKey { get; set; }
        public string VendorName { get; set; }
    }

    public class qryJobInvoiceToPeachtreeCharge
    {
        public int InvoiceKey { get; set; }
        public string ChargeDescription { get; set; }
        public string ChargePeachtreeJobPhaseID { get; set; }
        public int? ChargeGLAccount { get; set; }
        public decimal ChargeQty { get; set; }
        public Nullable<decimal> ChargeLinePrice { get; set; }
        public string ChargeMemo { get; set; }
        public string ChargePeachtreeID { get; set; }
    }
    #endregion Export Invoice to Peachtree

    #region Export PO To Peachtree
    public class qrptJobPurchaseOrder
    {
        public int JobKey { get; set; }
        public string JobNum { get; set; }
        public int POKey { get; set; }
        public string PONum { get; set; }
        public DateTime PODate { get; set; }
        public DateTime POGoodThruDate { get; set; }
        public int? POVendorPaymentTerms { get; set; }
        public string POCurrencyCode { get; set; }
        public Nullable<decimal> POCurrencyRate { get; set; }
        public string CurrencyFormat { get; set; }
        public int POFreightDestination { get; set; }
        public int POCustShipKey { get; set; }
        public int POWarehouseKey { get; set; }
        public int POShipmentType { get; set; }
        public string POVendorReference { get; set; }
        public int VendorKey { get; set; }
        public string VendorName { get; set; }
        public string ContactTitle { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorAddress2 { get; set; }
        public string VendorCity { get; set; }
        public string VendorState { get; set; }
        public string VendorZip { get; set; }
        public string VendorCountry { get; set; }
        public string VendorLanguageCode { get; set; }
        public string VendorPeachtreeID { get; set; }
        public string VendorPeachtreeItemID { get; set; }
        public string VendorPeachtreeJobID { get; set; }
        public string EmployeeName { get; set; }
        public int? EmployeeTitleCode { get; set; }
        public string EmployeeEmail { get; set; }
        public int LocationKey { get; set; }
        public string LocationName { get; set; }
        public string LocationDescription { get; set; }
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationCity { get; set; }
        public string LocationState { get; set; }
        public string LocationZip { get; set; }
        public int LocationCountry { get; set; }
        public string LocationPhone { get; set; }
        public string LocationFax { get; set; }
        public int? LocationReportHeaderImage { get; set; }
        public string LocationWebsite { get; set; }
        public string InspectorName { get; set; }
        public string JobInspectionNum { get; set; }
        public string JobDUINum { get; set; }
        public int? POPeachtreeNRecord { get; set; }
        public int? JobPTIndex { get; set; }
        public bool POUseOriginAddress { get; set; }
        public int? POVendorOriginAddress { get; set; }
    }
    public class qryJobPurchaseOrderPeachtreeItem
    {
        public int POJobKey { get; set; }
        public int POKey { get; set; }
        public int POItemsQty { get; set; }
        public int POItemsItemKey { get; set; }
        public Nullable<decimal> ItemCost { get; set; }
        public Nullable<decimal> ItemLineCost { get; set; }
        public string POItemsMemoVendor { get; set; }
        public bool POItemsMemoVendorMoveBottom { get; set; }
        public string ItemNum { get; set; }
        public string DescriptionText { get; set; }
        public Nullable<decimal> POCurrencyRate { get; set; }
    }
    public class qryJobPurchaseOrderPeachtreeCharge
    {
        public int POChargesJobKey { get; set; }
        public int POChargesPOKey { get; set; }
        public string POChargesMemo { get; set; }
        public decimal POChargesQty { get; set; }
        public Nullable<decimal> ChargeLineCost { get; set; }
        public Nullable<decimal> ChargeCost { get; set; }
        public string DescriptionText { get; set; }
        public int? ChargeAPAccount { get; set; }
        public string ChargePeachtreeID { get; set; }
        public Nullable<decimal> POCurrencyRate { get; set; }
        public string ChargePeachtreeJobPhaseID { get; set; }
    }
    public class POShipTo
    {
        public string ShipName { get; set; }
            public string ShipAddress1 { get; set; }
            public string ShipAddress2 { get; set; }
            public string ShipCity { get; set; }
            public string ShipState { get; set; }
            public string ShipZip { get; set; }
            public string ShipCountry { get; set; }
    } 
    #endregion Export PO To Peachtree

    public class JobCurrencyMaster
    {
        public int JobCurrencyJobKey { get; set; }
        public string JobCurrencyCode { get; set; }
        public decimal JobCurrencyRate { get; set; }
        public virtual string CurrencyDescription { get; set; }
        public virtual string CurrencySymbol { get; set; }
    }

    public class qryPOSearch
    {
        public int POKey { get; set; }
        public int PONum { get; set; }
        public int PORevisionNum { get; set; }
        public int PONumShipment { get; set; }
        public int POJobKey { get; set; }
        public int? POInvoiceKey { get; set; }
        public int POVendorKey { get; set; }
        public int? POVendorContactKey { get; set; }
        public string POVendorReference { get; set; }
        public int? POVendorOriginAddress { get; set; }
        public DateTime PODate { get; set; }
        public DateTime POGoodThruDate { get; set; }
        public string POLeadTime { get; set; }
        public Nullable<decimal> PODefaultProfitMargin { get; set; }
        public int? POVendorPaymentTerms { get; set; }
        public string POCurrencyCode { get; set; }
        public Nullable<decimal> POCurrencyRate { get; set; }
        public int POShipmentType { get; set; }
        public int POFreightDestination { get; set; }
        public string POFreightDestinationZip { get; set; }
        public int? POCustShipKey { get; set; }
        public int? POWarehouseKey { get; set; }
        public Nullable<DateTime> POShipETA { get; set; }
        public Nullable<DateTime> POSubmittedDate { get; set; }
        public string POStatusReport { get; set; }
        public bool POClosed { get; set; }
        public string POModifiedBy { get; set; }
        public Nullable<DateTime> POModifiedDate { get; set; }
        public string POCreatedBy { get; set; }
        public DateTime POCreatedDate { get; set; }
        public int? POPeachtreeNRecord { get; set; }
        public bool POUseOriginAddress { get; set; }
        public string JobNum { get; set; }
        public string PONumFormatted { get; set; }
        public string VendorName { get; set; }
    }

    public class qryJobInvoiceSearch
    {
        public int InvoiceKey { get; set; }
        public int? JobKey { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNum { get; set; }
        public string JobNum { get; set; }
        public string BillTo { get; set; }
        public string InvoiceCurrencyCode { get; set; }
        public Nullable<decimal> Price { get; set; }
    }

    public class NewJobMessage
    {
        public List<string> RecipientsTO { get; set; }
        public List<string> RecipientsCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class qfrmJobOverviewPopupUpdateCurrency
    {
        public int JobKey { get; set; }
        public Nullable<DateTime> JobCurrencyLockedDate { get; set; }
    }

    public class qfrmJobExemptFromProfitReport
    {
        public virtual int JobKey { get; set; }

        public virtual string JobNum { get; set; }

        public virtual string JobProdDescription { get; set; }

        public virtual string JobReference { get; set; }

        public virtual string CustName { get; set; }

        public virtual string CustContact { get; set; }

        public virtual bool JobExemptFromProfitReport { get; set; }
    }

    public class qfrmJobExemptFromPronacaReport
    {
        public int JobKey { get; set; }
        public string JobNum { get; set; }
        public string JobProdDescription { get; set; }
        public string JobReference { get; set; }
        public string CustName { get; set; }
        public string CustContact { get; set; }
        public bool JobExemptFromPronacaReport { get; set; }
    }

}