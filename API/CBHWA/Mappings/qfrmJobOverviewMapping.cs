using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CBHBusiness;
using ClientModel = CBHWA.Models;

namespace CBHWA.Mappings
{
    public class qfrmJobOverviewMapping { 
        public qfrmJobOverview MapModels(ClientModel.qJobOverview model)
        {
            var data = new qfrmJobOverview
            {
                JobKey = model.JobKey,
                JobQHdrKey = model.JobQHdrKey,
                QuoteNum = model.QuoteNum,
                JobNum = model.JobNum,
                JobShipDate = model.JobShipDate,
                JobArrivalDate = model.JobArrivalDate,
                JobShipmentCarrier = model.JobShipmentCarrier,
                JobCarrierRefNum = model.JobCarrierRefNum,
                JobCarrierVessel = model.JobCarrierVessel,
                JobInspectionCertificateNum = model.JobInspectionCertificateNum,
                JobCustPaymentTerms = model.JobCustPaymentTerms,
                JobClosed = model.JobClosed,
                JobComplete = model.JobComplete,
                JobModifiedBy = model.JobModifiedBy,
                JobModifiedDate = model.JobModifiedDate,
                JobCreatedBy = model.JobCreatedBy,
                JobCreatedDate = model.JobCreatedDate,
                JobExemptFromProfitReport = model.JobExemptFromProfitReport,
                CustName = model.CustName,
                ContactLastName = model.ContactLastName,
                ContactFirstName = model.ContactFirstName,
                CustPhone = model.CustPhone,
                CustFax = model.CustFax,
                CustEmail = model.CustEmail,
                ContactPhone = model.ContactPhone,
                ContactFax = model.ContactFax,
                ContactEmail = model.ContactEmail
            };

            return data;
        }

        public ClientModel.qJobOverview MapModels(qfrmJobOverview model)
        {
            var qjo = new ClientModel.JobsRepository().GetJobOverview(model.JobKey).First();

            var data = new ClientModel.qJobOverview
            {
                JobKey = model.JobKey,
                JobQHdrKey = model.JobQHdrKey,
                QuoteNum = model.QuoteNum,
                JobNum = model.JobNum,
                JobShipDate = model.JobShipDate,
                JobArrivalDate = model.JobArrivalDate,
                JobShipmentCarrier = model.JobShipmentCarrier,
                JobCarrierRefNum = model.JobCarrierRefNum,
                JobCarrierVessel = model.JobCarrierVessel,
                JobInspectionCertificateNum = model.JobInspectionCertificateNum,
                JobCustPaymentTerms = model.JobCustPaymentTerms,
                JobClosed = model.JobClosed,
                JobComplete = model.JobComplete,
                JobModifiedBy = model.JobModifiedBy,
                JobModifiedDate = model.JobModifiedDate,
                JobCreatedBy = model.JobCreatedBy,
                JobCreatedDate = model.JobCreatedDate,
                JobExemptFromProfitReport = model.JobExemptFromProfitReport,
                CustName = model.CustName,
                ContactLastName = model.ContactLastName,
                ContactFirstName = model.ContactFirstName,
                CustPhone = model.CustPhone,
                CustFax = model.CustFax,
                CustEmail = model.CustEmail,
                ContactPhone = model.ContactPhone,
                ContactFax = model.ContactFax,
                ContactEmail = model.ContactEmail,
                x_ContactName = qjo.x_ContactName,
                x_Email = qjo.x_Email,
                x_Fax = qjo.x_Fax,
                x_Info = qjo.x_Info,
                x_JobCustPaymentTerms = qjo.x_JobCustPaymentTerms,
                x_JobShipmentCarrierText = qjo.x_JobShipmentCarrierText,
                x_Phone = qjo.x_Phone,
                CustCurrencyCode = qjo.CustCurrencyCode,
                CustCurrencyRate = qjo.CustCurrencyRate
            };

            return data;
        }
    }
}