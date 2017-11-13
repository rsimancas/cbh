using System;

namespace CBHWA.Models
{
    public class Country
    {
        public int CountryKey { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public decimal CountryFOBValueForInspection { get; set; }
        public string CountryFOBValueForInspectionCurrencyCode { get; set; }
        public string CountryModifiedBy { get; set; }
        public Nullable<System.DateTime> CountryModifiedDate { get; set; }
    }
}