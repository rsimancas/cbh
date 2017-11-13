
namespace CBHWA.Models
{
    public class PaymentTerms
    {
        public int TermKey { get; set; }
        public decimal TermPercentPrepaid { get; set; }
        public decimal TermPercentWithOrder { get; set; }
        public decimal TermPercentPriorToShip { get; set; }
        public decimal TermPercentAgainstShipDocs { get; set; }
        public decimal TermPercentNet { get; set; }
        public int TermPercentDays { get; set; }
        public bool TermWarningFlag { get; set; }
        public string x_Description { get; set; }
    }

    public class PaymentTermsDescriptions
    {
        public int PTKey { get; set; }
        public int PTTermKey { get; set; }
        public string PTLanguageCode { get; set; }
        public string PTDescription { get; set; }
        public string x_Language { get; set; }
    }

 
}