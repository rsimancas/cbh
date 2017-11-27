using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CBHBusiness;
using Client = CBHBusiness.ClientModels;

namespace CBHWA.Mappings
{
    public class qfrmFileQuoteConfirmationMapping
    { 
        public Client.qfrmFileQuoteConfirmation MapModels(qfrmFileQuoteConfirmation model)
        {
            var data = new Client.qfrmFileQuoteConfirmation
            {
                FileKey = model.FileKey,
                FileYear = model.FileYear,
                FileNum = model.FileNum,
                FileStatusKey = model.FileStatusKey,
                FileQuoteEmployeeKey = model.FileQuoteEmployeeKey,
                FileOrderEmployeeKey = model.FileOrderEmployeeKey,
                FileCustKey = model.FileCustKey,
                FileContactKey = model.FileContactKey,
                FileCustShipKey = model.FileCustShipKey,
                FileReference = model.FileReference,
                FileProfitMargin = model.FileProfitMargin,
                FileCurrentVendor = model.FileCurrentVendor,
                FileModifiedBy = model.FileModifiedBy,
                FileModifiedDate = model.FileModifiedDate,
                FileCreatedBy = model.FileCreatedBy,
                FileCreatedDate = model.FileCreatedDate,
                FileDateCustRequired = model.FileDateCustRequired,
                FileDateCustRequiredNote = model.FileDateCustRequiredNote,
                FileDefaultCurrencyCode = model.FileDefaultCurrencyCode,
                FileDefaultCurrencyRate = model.FileDefaultCurrencyRate,
                FileClosed = model.FileClosed,
                CustName = model.CustName,
                CustFax = model.CustFax,
                ContactPhone = model.ContactPhone,
                ContactEmail = model.ContactEmail,
                QHdrKey = model.QHdrKey,
                QHdrPrefix = model.QHdrPrefix,
                QHdrFileKey = model.QHdrFileKey,
                QHdrNum = model.QHdrNum,
                QHdrRevision = model.QHdrRevision,
                QHdrDate = model.QHdrDate,
                QHdrGoodThruDate = model.QHdrGoodThruDate,
                QHdrCustPaymentTerms = model.QHdrCustPaymentTerms,
                QHdrMemo = model.QHdrMemo,
                QHdrExFactoryOption = model.QHdrExFactoryOption,
                QHdrExFactoryLocation = model.QHdrExFactoryLocation,
                QHdrFOBOption = model.QHdrFOBOption,
                QHdrFOBLocation = model.QHdrFOBLocation,
                QHdrCIFOption = model.QHdrCIFOption,
                QHdrCIFLocation = model.QHdrCIFLocation,
                QHdrFreightDirect = model.QHdrFreightDirect,
                QHdrLeadTime = model.QHdrLeadTime,
                QHdrCarrierKey = model.QHdrCarrierKey,
                QHdrWarehouseKey = model.QHdrWarehouseKey,
                QHdrShipType = model.QHdrShipType,
                QHdrSentDate = model.QHdrSentDate,
                QHdrCurrencyCode = model.QHdrCurrencyCode,
                QHdrCurrencyRate = model.QHdrCurrencyRate,
                QHdrProdDescription = model.QHdrProdDescription,
                QHdrShippingDescription = model.QHdrShippingDescription,
                QHdrCustRefNum = model.QHdrCustRefNum,
                QHdrModifiedBy = model.QHdrModifiedBy,
                QHdrModifiedDate = model.QHdrModifiedDate,
                QHdrCreatedBy = model.QHdrCreatedBy,
                QHdrCreatedDate = model.QHdrCreatedDate,
                QHdrQuoteConfirmationDate = model.QHdrQuoteConfirmationDate,
                QHdrInsurance = model.QHdrInsurance,
                QHdrInspectorKey = model.QHdrInspectorKey,
                QHdrInspectionNum = model.QHdrInspectionNum,
                QHdrDUINum = model.QHdrDUINum,
                QHdrJobKey = model.QHdrJobKey
            };

            return data;
        }

        public qfrmFileQuoteConfirmation MapModels(Client.qfrmFileQuoteConfirmation model)
        {
            var data = new qfrmFileQuoteConfirmation
            {
                FileKey = model.FileKey,
                FileYear = model.FileYear,
                FileNum = model.FileNum,
                FileStatusKey = model.FileStatusKey,
                FileQuoteEmployeeKey = model.FileQuoteEmployeeKey,
                FileOrderEmployeeKey = model.FileOrderEmployeeKey,
                FileCustKey = model.FileCustKey,
                FileContactKey = model.FileContactKey,
                FileCustShipKey = model.FileCustShipKey,
                FileReference = model.FileReference,
                FileProfitMargin = model.FileProfitMargin,
                FileCurrentVendor = model.FileCurrentVendor,
                FileModifiedBy = model.FileModifiedBy,
                FileModifiedDate = model.FileModifiedDate,
                FileCreatedBy = model.FileCreatedBy,
                FileCreatedDate = model.FileCreatedDate,
                FileDateCustRequired = model.FileDateCustRequired,
                FileDateCustRequiredNote = model.FileDateCustRequiredNote,
                FileDefaultCurrencyCode = model.FileDefaultCurrencyCode,
                FileDefaultCurrencyRate = model.FileDefaultCurrencyRate,
                FileClosed = model.FileClosed,
                CustName = model.CustName,
                CustFax = model.CustFax,
                ContactPhone = model.ContactPhone,
                ContactEmail = model.ContactEmail,
                QHdrKey = model.QHdrKey,
                QHdrPrefix = model.QHdrPrefix,
                QHdrFileKey = model.QHdrFileKey,
                QHdrNum = model.QHdrNum,
                QHdrRevision = model.QHdrRevision,
                QHdrDate = model.QHdrDate,
                QHdrGoodThruDate = model.QHdrGoodThruDate,
                QHdrCustPaymentTerms = model.QHdrCustPaymentTerms,
                QHdrMemo = model.QHdrMemo,
                QHdrExFactoryOption = model.QHdrExFactoryOption,
                QHdrExFactoryLocation = model.QHdrExFactoryLocation,
                QHdrFOBOption = model.QHdrFOBOption,
                QHdrFOBLocation = model.QHdrFOBLocation,
                QHdrCIFOption = model.QHdrCIFOption,
                QHdrCIFLocation = model.QHdrCIFLocation,
                QHdrFreightDirect = model.QHdrFreightDirect,
                QHdrLeadTime = model.QHdrLeadTime,
                QHdrCarrierKey = model.QHdrCarrierKey,
                QHdrWarehouseKey = model.QHdrWarehouseKey,
                QHdrShipType = model.QHdrShipType,
                QHdrSentDate = model.QHdrSentDate,
                QHdrCurrencyCode = model.QHdrCurrencyCode,
                QHdrCurrencyRate = model.QHdrCurrencyRate,
                QHdrProdDescription = model.QHdrProdDescription,
                QHdrShippingDescription = model.QHdrShippingDescription,
                QHdrCustRefNum = model.QHdrCustRefNum,
                QHdrModifiedBy = model.QHdrModifiedBy,
                QHdrModifiedDate = model.QHdrModifiedDate,
                QHdrCreatedBy = model.QHdrCreatedBy,
                QHdrCreatedDate = model.QHdrCreatedDate,
                QHdrQuoteConfirmationDate = model.QHdrQuoteConfirmationDate,
                QHdrInsurance = model.QHdrInsurance,
                QHdrInspectorKey = model.QHdrInspectorKey,
                QHdrInspectionNum = model.QHdrInspectionNum,
                QHdrDUINum = model.QHdrDUINum,
                QHdrJobKey = model.QHdrJobKey
            };

            return data;
        }

        public Client.qfrmFileQuoteConfirmationSubVendorInfo MapModels(qfrmFileQuoteConfirmationSubVendorInfo model)
        {
            var data = new Client.qfrmFileQuoteConfirmationSubVendorInfo
            {
                FVFileKey = model.FVFileKey,
                FVQHdrKey = model.FVQHdrKey,
                FVVendorKey = model.FVVendorKey,
                FVVendorContactKey = model.FVVendorContactKey,
                FVProfitMargin = model.FVProfitMargin,
                FVPaymentTerms = model.FVPaymentTerms,
                FVFreightDirect = model.FVFreightDirect,
                FVFreightDestination = model.FVFreightDestination,
                FVFreightDestinationZip = model.FVFreightDestinationZip,
                FVFreightShipmentType = model.FVFreightShipmentType,
                FVWarehouseKey = model.FVWarehouseKey,
                FVFreightCost = model.FVFreightCost,
                FVFreightPrice = model.FVFreightPrice,
                FVLeadTime = model.FVLeadTime,
                FVDiscount = model.FVDiscount,
                FVDiscountPercent = model.FVDiscountPercent,
                FVDiscountCurrencyCode = model.FVDiscountCurrencyCode,
                FVDiscountCurrencyRate = model.FVDiscountCurrencyRate,
                FVExcelFilename = model.FVExcelFilename,
                FVTotalWeight = model.FVTotalWeight,
                FVTotalVolume = model.FVTotalVolume,
                FVRFQFileName = model.FVRFQFileName,
                FVRFQDate = model.FVRFQDate,
                FVSentDate = model.FVSentDate,
                FVQuoteInfoConfirmed = model.FVQuoteInfoConfirmed,
                FVQuotePONotes = model.FVQuotePONotes,
                FVPOCurrencyCode = model.FVPOCurrencyCode,
                FVPOCurrencyRate = model.FVPOCurrencyRate,
                ContactPhone = model.ContactPhone,
                ContactEmail = model.ContactEmail,
                VendorFax = model.VendorFax
            };

            return data;
        }
    }
}