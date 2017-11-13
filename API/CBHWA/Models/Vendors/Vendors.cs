namespace CBHWA.Models
{
    using System;

    public class Vendor
    {
        public int VendorKey { get; set; }
        public string VendorName { get; set; }
        public string VendorPeachtreeID { get; set; }
        public string VendorPeachtreeItemID { get; set; }
        public string VendorPeachtreeJobID { get; set; }
        public string VendorDisplayToCust { get; set; }
        public string VendorContact { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorAddress2 { get; set; }
        public string VendorCity { get; set; }
        public string VendorState { get; set; }
        public string VendorZip { get; set; }
        public int? VendorCountryKey { get; set; }
        public string VendorPhone { get; set; }
        public string VendorFax { get; set; }
        public string VendorEmail { get; set; }
        public string VendorWebsite { get; set; }
        public string VendorLanguageCode { get; set; }
        public string VendorAcctNum { get; set; }
        public bool VendorCarrier { get; set; }
        public string VendorModifiedBy { get; set; }
        public Nullable<DateTime> VendorModifiedDate { get; set; }
        public string VendorCreatedBy { get; set; }
        public DateTime VendorCreatedDate { get; set; }
        public decimal VendorDefaultCommissionPercent { get; set; }
        public decimal x_LastQuoteMargin { get; set; }
        public string x_VendorAddress { get; set; }
    }

    public class LastQuoteMargin
    {
        public int FVVendorKey { get; set; }
        public decimal FVProfitMargin { get; set; }
    }

     public class VendorOriginAddress
    {
        public int OriginKey { get; set; }
        public int OriginVendorKey { get; set; }
        public string OriginName { get; set; }
        public string OriginComments { get; set; }
        public string OriginAddress1 { get; set; }
        public string OriginAddress2 { get; set; }
        public string OriginCity { get; set; }
        public string OriginState { get; set; }
        public string OriginZip { get; set; }
        public int OriginCountryKey { get; set; }
        public bool OriginDefault { get; set; }
        public string OriginPhone { get; set; }
        public string OriginModifiedBy { get; set; }
        public DateTime OriginModifiedDate { get; set; }
        public string x_CountryName { get; set; }
        public string x_StateName { get; set; }
    }

    public class VendorContact
    {
        public int ContactKey { get; set; }
        public int ContactVendorKey { get; set; }
        public string ContactTitle { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactFax { get; set; }
        public string ContactEmail { get; set; }
    }

    public class VendorWarehouse
    {
        public int WarehouseKey { get; set; }
        public int WarehouseVendorKey { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseAddress1 { get; set; }
        public string WarehouseAddress2 { get; set; }
        public string WarehouseCity { get; set; }
        public string WarehouseState { get; set; }
        public string WarehouseZip { get; set; }
        public int? WarehouseCountryKey { get; set; }
        public string WarehousePhone { get; set; }
        public string WarehouseModifiedBy { get; set; }
        public DateTime WarehouseModifiedDate { get; set; }
        public string x_CountryName { get; set; }
        public string x_StateName { get; set; }
    }

    public class WarehouseType
    {
        public int WarehouseKey { get; set; }
        public int CarrierKey { get; set; }
        public string CarrierWarehouse { get; set; }
        public string CityState { get; set; }
        public string ZipCode { get; set; }
    }

    public class VendorForReport
    {
        public int VendorKey { get; set; }
        public string VendorName { get; set; }
    }
}