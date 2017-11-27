using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBHBusiness.ClientModels
{
    public class qfrmFileQuoteConfirmation
    {
        public int FileKey { get; set; }
        public short FileYear { get; set; }
        public short FileNum { get; set; }
        public byte FileStatusKey { get; set; }
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
        public short QHdrCIFOption { get; set; }
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
}
