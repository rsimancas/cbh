using System;

namespace CBHWA.Models
{
    public class Customer
    {
        public int CustKey { get; set; }
        public string CustPeachtreeID { get; set; }
        public int? CustPeachtreeIndex { get; set; }
        public string CustName { get; set; }
        public string CustAddress1 { get; set; }
        public string CustAddress2 { get; set; }
        public string CustCity { get; set; }
        public string CustState { get; set; }
        public string CustZip { get; set; }
        public int? CustCountryKey { get; set; }
        public string CustPhone { get; set; }
        public string CustFax { get; set; }
        public string CustEmail { get; set; }
        public string CustWebsite { get; set; }
        public int? CustSalesRepKey { get; set; }
        public int? CustOrdersRepKey { get; set; }
        public string CustLanguageCode { get; set; }
        public int CustStatus { get; set; }
        public string CustModifiedBy { get; set; }
        public Nullable<DateTime> CustModifiedDate { get; set; }
        public string CustCreatedBy { get; set; }
        public DateTime CustCreatedDate { get; set; }
        public decimal CustCreditLimit { get; set; }
        public string CustCurrencyCode { get; set; }
        public string CustMemo { get; set; }
    }

    public class CustomerContact
    {
        public int ContactKey { get; set; }
        public int ContactCustKey { get; set; }
        public string ContactTitle { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactFax { get; set; }
        public string ContactEmail { get; set; }
        public string ContactMemo { get; set; }
        public bool ContactAllowedWebAccess { get; set; }
        public string ContactPassword { get; set; }
        public bool ContactPasswordReset { get; set; }
        public string ContactModifiedBy { get; set; }
        public Nullable<System.DateTime> ContactModifiedDate { get; set; }
        public string x_ContactFullName { get; set; }
    }

    public class CustomerShipAddress
    {
        public int ShipKey { get; set; }
        public int ShipCustKey { get; set; }
        public string ShipName { get; set; }
        public string ShipComments { get; set; }
        public string ShipAddress1 { get; set; }
        public string ShipAddress2 { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipZip { get; set; }
        public int? ShipCountryKey { get; set; }
        public string ShipPhone { get; set; }
        public bool ShipDefault { get; set; }
        public string ShipModifiedBy { get; set; }
        public Nullable<System.DateTime> ShipModifiedDate { get; set; }
        public string x_CountryName { get; set; }
        public string x_StateName { get; set; }
        public string x_FullShipAddress { get; set; }
    }

    public class CustomerStatus
    {
        public int StatusKey { get; set; }
        public string StatusDescription { get; set; }
    }

    public class CustomerForReport
    {
        public int CustKey { get; set; }
        public string CustName { get; set; }
        public string CustCity { get; set; }
        public string CustState { get; set; }
        public int? CustCountryKey { get; set; }
        public string CountryName { get; set; }
    }

    public class CustomerContactForReport
    {
        public int ContactKey { get; set; }
        public string x_ContactFullName { get; set; }
    }
}