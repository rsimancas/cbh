using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBHBusiness.ClientModels
{
    public class qfrmFileQuoteConfirmationSubVendorInfo
    {
        public int FVFileKey { get; set; }
        public int? FVQHdrKey { get; set; }
        public int FVVendorKey { get; set; }
        public int? FVVendorContactKey { get; set; }
        public decimal FVProfitMargin { get; set; }
        public int? FVPaymentTerms { get; set; }
        public bool FVFreightDirect { get; set; }
        public short FVFreightDestination { get; set; }
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
    }
}
