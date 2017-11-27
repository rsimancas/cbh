using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client = CBHBusiness.ClientModels;

namespace CBHBusiness
{
    public class qfrmFileQuoteConfirmationBusiness
    {
        private CBHClassesDataContext db = new CBHClassesDataContext();

        public IQueryable<qfrmFileQuoteConfirmation> GetList()
        {
            var iq = from p in db.qfrmFileQuoteConfirmations
                     select p;

            return iq;
        }

        public qfrmFileQuoteConfirmation Get(int QHdrKey)
        {
            var qfrmFileQuoteConfirmation = (from p in db.qfrmFileQuoteConfirmations
                                    where p.QHdrKey == QHdrKey
                                    select p).FirstOrDefault();
            return qfrmFileQuoteConfirmation;
        }

        public qfrmFileQuoteConfirmation Update(Client.qfrmFileQuoteConfirmation model)
        {

            var quote = db.tblFileQuoteHeaders.Where(w => w.QHdrKey == model.QHdrKey).Single();
            var file = db.tblFileHeaders.Where(w => w.FileKey == model.FileKey).Single();

            // update file
            file.FileQuoteEmployeeKey = model.FileQuoteEmployeeKey;
            file.FileOrderEmployeeKey = model.FileOrderEmployeeKey;
            file.FileContactKey = model.FileContactKey;
            file.FileCustShipKey = model.FileCustShipKey;
            file.FileReference = model.FileReference;
            file.FileProfitMargin = model.FileProfitMargin;
            file.FileCurrentVendor = model.FileCurrentVendor;
            file.FileModifiedBy = model.FileModifiedBy;
            file.FileModifiedDate = model.FileModifiedDate;
            file.FileCreatedBy = model.FileCreatedBy;
            file.FileCreatedDate = model.FileCreatedDate;
            file.FileDateCustRequired = model.FileDateCustRequired;
            file.FileDateCustRequiredNote = model.FileDateCustRequiredNote;
            file.FileDefaultCurrencyCode = model.FileDefaultCurrencyCode;
            file.FileDefaultCurrencyRate = model.FileDefaultCurrencyRate;
            file.FileClosed = model.FileClosed;
            file.FileCurrentVendor = model.FileCurrentVendor.Value;
            // update quote
            quote.QHdrRevision = model.QHdrRevision;
            quote.QHdrDate = model.QHdrDate;
            quote.QHdrGoodThruDate = model.QHdrGoodThruDate;
            quote.QHdrCustPaymentTerms = model.QHdrCustPaymentTerms;
            quote.QHdrMemo = model.QHdrMemo;
            quote.QHdrExFactoryOption = model.QHdrExFactoryOption;
            quote.QHdrExFactoryLocation = model.QHdrExFactoryLocation;
            quote.QHdrFOBOption = model.QHdrFOBOption;
            quote.QHdrFOBLocation = model.QHdrFOBLocation;
            quote.QHdrCIFOption = model.QHdrCIFOption;
            quote.QHdrCIFLocation = model.QHdrCIFLocation;
            quote.QHdrFreightDirect = model.QHdrFreightDirect;
            quote.QHdrLeadTime = model.QHdrLeadTime;
            quote.QHdrCarrierKey = model.QHdrCarrierKey;
            quote.QHdrWarehouseKey = model.QHdrWarehouseKey;
            quote.QHdrShipType = model.QHdrShipType;
            quote.QHdrSentDate = model.QHdrSentDate;
            quote.QHdrCurrencyCode = model.QHdrCurrencyCode;
            quote.QHdrCurrencyRate = model.QHdrCurrencyRate;
            quote.QHdrProdDescription = model.QHdrProdDescription;
            quote.QHdrShippingDescription = model.QHdrShippingDescription;
            quote.QHdrCustRefNum = model.QHdrCustRefNum;
            quote.QHdrModifiedBy = model.QHdrModifiedBy;
            quote.QHdrModifiedDate = DateTime.Now;
            quote.QHdrQuoteConfirmationDate = model.QHdrQuoteConfirmationDate;
            quote.QHdrInsurance = model.QHdrInsurance;
            quote.QHdrInspectorKey = model.QHdrInspectorKey;
            quote.QHdrInspectionNum = model.QHdrInspectionNum;
            quote.QHdrDUINum = model.QHdrDUINum;
            quote.QHdrJobKey = model.QHdrJobKey;


            // update file
            db.Refresh(System.Data.Linq.RefreshMode.KeepCurrentValues, quote);
            db.SubmitChanges();
            return db.qfrmFileQuoteConfirmations.Where(w => w.QHdrKey == model.QHdrKey).Single();
        }

        public qfrmFileQuoteConfirmationSubVendorInfo Update(Client.qfrmFileQuoteConfirmationSubVendorInfo model)
        {
            var fv = db.tblFileQuoteVendorInfos.Where(w => w.FVVendorKey == model.FVVendorKey && w.FVQHdrKey == model.FVQHdrKey).Single();

            fv.FVFileKey = model.FVFileKey;
            fv.FVQHdrKey = model.FVQHdrKey;
            fv.FVVendorKey = model.FVVendorKey;
            fv.FVVendorContactKey = model.FVVendorContactKey;
            fv.FVProfitMargin = model.FVProfitMargin;
            fv.FVPaymentTerms = model.FVPaymentTerms;
            fv.FVFreightDirect = model.FVFreightDirect;
            fv.FVFreightDestination = model.FVFreightDestination;
            fv.FVFreightDestinationZip = model.FVFreightDestinationZip;
            fv.FVFreightShipmentType = model.FVFreightShipmentType;
            fv.FVWarehouseKey = model.FVWarehouseKey;
            fv.FVFreightCost = model.FVFreightCost;
            fv.FVFreightPrice = model.FVFreightPrice;
            fv.FVLeadTime = model.FVLeadTime;
            fv.FVDiscount = model.FVDiscount;
            fv.FVDiscountPercent = model.FVDiscountPercent;
            fv.FVDiscountCurrencyCode = model.FVDiscountCurrencyCode;
            fv.FVDiscountCurrencyRate = model.FVDiscountCurrencyRate;
            fv.FVExcelFilename = model.FVExcelFilename;
            fv.FVTotalWeight = model.FVTotalWeight;
            fv.FVTotalVolume = model.FVTotalVolume;
            fv.FVRFQFileName = model.FVRFQFileName;
            fv.FVRFQDate = model.FVRFQDate;
            fv.FVSentDate = model.FVSentDate;
            fv.FVQuoteInfoConfirmed = model.FVQuoteInfoConfirmed;
            fv.FVQuotePONotes = model.FVQuotePONotes;
            fv.FVPOCurrencyCode = model.FVPOCurrencyCode;
            fv.FVPOCurrencyRate = model.FVPOCurrencyRate;

            fv.tblVendor.VendorFax = model.VendorFax;
            fv.tblVendorContact.ContactPhone = model.ContactPhone;
            fv.tblVendorContact.ContactEmail = model.ContactPhone;

            db.Refresh(System.Data.Linq.RefreshMode.KeepCurrentValues, fv);
            db.SubmitChanges();
            return db.qfrmFileQuoteConfirmationSubVendorInfos.Where(w => w.FVVendorKey == model.FVVendorKey && w.FVQHdrKey == model.FVQHdrKey).Single();
        }
    }
}