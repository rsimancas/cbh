using System;

namespace CBHWA.Models
{
    public class CurrencyRates
    {
        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }
        public string CurrencyDescription { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyFormat { get; set; }
        public Nullable<System.DateTime> CurrencyModifiedDate { get; set; }
        public virtual string x_CurrencyCodeDesc { get; set; }
    }

 
}