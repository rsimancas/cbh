using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client = CBHBusiness.ClientModels;

namespace CBHBusiness
{
    public class InvoiceBusiness
    {
        private CBHClassesDataContext db = new CBHClassesDataContext();

        public IQueryable<tblInvoiceHeader> GetInvoices()
        {
            var iq = from p in db.tblInvoiceHeaders
                     select p;

            return iq;
        }

        public tblInvoiceHeader GetInvoice(int invoiceKey)
        {
            var tblInvoiceHeader = (from p in db.tblInvoiceHeaders
                                    where p.InvoiceKey == invoiceKey
                                    select p).FirstOrDefault();
            return tblInvoiceHeader;
        }

        public tblInvoiceHeader AddInvoiceHeader(tblInvoiceHeader tblInvoiceHeader)
        {
            db.tblInvoiceHeaders.InsertOnSubmit(tblInvoiceHeader);
            db.SubmitChanges();
            return tblInvoiceHeader;
        }

        public tblInvoiceHeader UpdateInvoiceHeader(tblInvoiceHeader tblInvoiceHeader)
        {
            db.tblInvoiceHeaders.Attach(tblInvoiceHeader);
            db.Refresh(System.Data.Linq.RefreshMode.KeepCurrentValues, tblInvoiceHeader);
            db.SubmitChanges();
            return tblInvoiceHeader;
        }

        public tblInvoiceHeader DeleteInvoiceHeader(int invoiceKey)
        {
            var tblInvoiceHeader = db.tblInvoiceHeaders.Where(item => item.InvoiceKey == invoiceKey).Single();
            db.tblInvoiceHeaders.DeleteOnSubmit(tblInvoiceHeader);
            db.SubmitChanges();
            return tblInvoiceHeader;
        }

        public qfrmInvoiceMaintenance GetInvoiceMaintenance(int invoiceKey)
        {
            var data = (from p in db.qfrmInvoiceMaintenances
                        where p.InvoiceKey == invoiceKey
                        select p).FirstOrDefault();

            return data;
        }

        public qfrmInvoiceMaintenance UpdateInvoiceMaintenance(Client.qfrmInvoiceMaintenance model)
        {
            var invoice = db.tblInvoiceHeaders.Where(w => w.InvoiceKey == model.InvoiceKey).Single();
            invoice.InvoiceModifiedDate = DateTime.Now;
            invoice.InvoiceJobKey = model.InvoiceJobKey;
            invoice.InvoicePrefix = model.InvoicePrefix;
            invoice.InvoiceNum = model.InvoiceNum;
            invoice.InvoiceRevisionNum = model.InvoiceRevisionNum;
            invoice.InvoiceDate = model.InvoiceDate;
            invoice.InvoiceRecipient = model.InvoiceRecipient;
            invoice.InvoiceVendorKey = model.InvoiceVendorKey;
            invoice.InvoiceVendorContactKey = model.InvoiceVendorContactKey;
            invoice.InvoiceCustKey = model.InvoiceCustKey;
            invoice.InvoiceCustContactKey = model.InvoiceCustContactKey;
            invoice.InvoiceBillingName = model.InvoiceBillingName;
            invoice.InvoiceBillingAddress1 = model.InvoiceBillingAddress1;
            invoice.InvoiceBillingAddress2 = model.InvoiceBillingAddress2;
            invoice.InvoiceBillingCity = model.InvoiceBillingCity;
            invoice.InvoiceBillingState = model.InvoiceBillingState;
            invoice.InvoiceBillingZip = model.InvoiceBillingZip;
            invoice.InvoiceBillingCountryKey = model.InvoiceBillingCountryKey;
            invoice.InvoiceCustShipKey = model.InvoiceCustShipKey;
            invoice.InvoiceCustReference = model.InvoiceCustReference;
            invoice.InvoiceEmployeeKey = model.InvoiceEmployeeKey;
            invoice.InvoiceCurrencyCode = model.InvoiceCurrencyCode;
            invoice.InvoiceCurrencyRate = model.InvoiceCurrencyRate;
            invoice.InvoicePaymentTerms = model.InvoicePaymentTerms;
            invoice.InvoiceMemo = model.InvoiceMemo;
            invoice.InvoiceModifiedBy = model.InvoiceModifiedBy;
            invoice.InvoiceModifiedDate = model.InvoiceModifiedDate;
            invoice.InvoiceCreatedBy = model.InvoiceCreatedBy;
            invoice.InvoiceCreatedDate = model.InvoiceCreatedDate;
            invoice.InvoiceMemoFont = model.InvoiceMemoFont;
            invoice.tblJobHeader.JobShipDate = model.JobShipDate;
            invoice.tblJobHeader.JobShipType = model.JobShipType.Value;
            invoice.tblCustomer.CustZip = model.CustZip;

            //db.tblInvoiceHeaders.Attach(invoice);
            db.Refresh(System.Data.Linq.RefreshMode.KeepCurrentValues, invoice);
            db.SubmitChanges();
            return db.qfrmInvoiceMaintenances.Where(w => w.InvoiceKey == invoice.InvoiceKey).Single();
        }
    }
}