using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBHBusiness.ClientModels
{
    public class qfrmInvoiceMaintenance
    {
        public int InvoiceKey { get; set; }
        public int? InvoiceJobKey { get; set; }
        public short InvoicePrefix { get; set; }
        public int InvoiceNum { get; set; }
        public byte InvoiceRevisionNum { get; set; }
        public DateTime InvoiceDate { get; set; }
        public byte InvoiceRecipient { get; set; }
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
        public int? CustKey { get; set; }
        public string CustPeachtreeID { get; set; }
        public int? CustPeachtreeIndex { get; set; }
        public string CustName { get; set; }
        public string CustAddress1 { get; set; }
        public string CustAddress2 { get; set; }
        public string CustCity { get; set; }
        public string CustState { get; set; }
        public string CustZip { get; set; }
        public int? CustCountryKey { get; set; }
        public string CustPhone { get; set; }
        public string CustFax { get; set; }
        public string CustEmail { get; set; }
        public string CustWebsite { get; set; }
        public int? CustSalesRepKey { get; set; }
        public int? CustOrdersRepKey { get; set; }
        public string CustLanguageCode { get; set; }
        public byte? CustStatus { get; set; }
        public string CustModifiedBy { get; set; }
        public Nullable<DateTime> CustModifiedDate { get; set; }
        public string CustCreatedBy { get; set; }
        public Nullable<DateTime> CustCreatedDate { get; set; }
        public Nullable<decimal> CustCreditLimit { get; set; }
        public string CustCurrencyCode { get; set; }
        public string CustMemo { get; set; }
        public int? VendorKey { get; set; }
        public string VendorName { get; set; }
        public string VendorPeachtreeID { get; set; }
        public string VendorPeachtreeItemID { get; set; }
        public string VendorPeachtreeJobID { get; set; }
        public string VendorDisplayToCust { get; set; }
        public string VendorContact { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorAddress2 { get; set; }
        public string VendorCity { get; set; }
        public string VendorState { get; set; }
        public string VendorZip { get; set; }
        public int? VendorCountryKey { get; set; }
        public string VendorPhone { get; set; }
        public string VendorFax { get; set; }
        public string VendorEmail { get; set; }
        public string VendorWebsite { get; set; }
        public string VendorLanguageCode { get; set; }
        public string VendorAcctNum { get; set; }
        public Nullable<bool> VendorCarrier { get; set; }
        public string VendorModifiedBy { get; set; }
        public Nullable<DateTime> VendorModifiedDate { get; set; }
        public string VendorCreatedBy { get; set; }
        public Nullable<DateTime> VendorCreatedDate { get; set; }
        public Nullable<decimal> VendorDefaultCommissionPercent { get; set; }
        public int? JobShipType { get; set; }
        public Nullable<DateTime> JobShipDate { get; set; }
        public string FullInvoiceNum { get; set; }
    }
}
