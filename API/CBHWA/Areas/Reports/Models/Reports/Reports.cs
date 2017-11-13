namespace CBHWA.Areas.Reports.Models
{
    public class ReportMessage
    {
        public string message { get; set; }
        public string type { get; set; }
    }

    public class Enqueue
    {
        public int EmployeeKey { get; set; }
        public string strWhere { get; set; }

        public string ReportName { get; set; }
    }

    public class PaymentPercent {
        public int QHdrKey { get; set; }
    }

    public class rptQuoteCustomer
    {
        public int id { get; set; }
        public string wSch { get; set; }
        public int EmployeeKey { get; set; }
        public bool askForPaymentTerms { get; set; }
        public decimal TermPercentPrepaid { get; set; }
        public bool hasNotPaymentTerms { get; set; }
    }

    public class rptFileStatusHistory
    {
        public int id { get; set; }
        public int EmployeeKey { get; set; }
    }

    public class rptQuoteOrderingForm
    {
        public int id { get; set; }
        public int EmployeeKey { get; set; }
    }

    public class rptJobInvoicePackingList {
        public int id { get; set; }
        public int EmployeeKey { get; set; }
    }

    public class rptJobPurchaseOrder
    {
        public int id { get; set; }
        public int EmployeeKey { get; set; }
    }

    public class rptJobInvoice
    {
        public int id { get; set; }
        public string wSch { get; set; }
        public int EmployeeKey { get; set; }
    }

    public class rptJobProfit
    {
        public int id { get; set; }
        public int EmployeeKey { get; set; }
    }

    public class rptCustomerWebLogins
    {
        public int id { get; set; }
        public int EmployeeKey { get; set; }
    }
}