using System;

namespace CBHWA.Models
{
    public class Item
    {
        public int ItemKey { get; set; }
        public int? ItemVendorKey { get; set; }
        public string ItemNum { get; set; }
        public decimal ItemCost { get; set; }
        public decimal ItemPrice { get; set; }
        public string ItemCurrencyCode { get; set; }
        public decimal ItemWeight { get; set; }
        public decimal ItemVolume { get; set; }
        public int? ItemSchBImportKey { get; set; }
        public int? ItemSchBNum { get; set; }
        public string ItemSchBExportDescription { get; set; }
        public string ItemModifiedBy { get; set; }
        public Nullable<DateTime> ItemModifiedDate { get; set; }
        public string ItemCreatedBy { get; set; }
        public DateTime ItemCreatedDate { get; set; }
        public bool ItemActive { get; set; }
        public string x_ItemName { get; set; }
        public string x_VendorName { get; set; }
        public string x_ItemNumName { get; set; }
        public virtual decimal ItemCurrencyRate { get; set; }
        public virtual string ItemCurrencySymbol { get; set; } 
    }

    public class ItemDescription
    {
        public int DescriptionKey { get; set; }
        public int DescriptionItemKey { get; set; }
        public string DescriptionLanguageCode { get; set; }
        public string DescriptionText { get; set; }
        public string x_Language { get; set; }
    }

    public class qlstScheduleBImport
    {
        public int SBLanguageKey { get; set; }
        public string SchBNumC { get; set; }
        public string SBLanguageSchBSubNum { get; set; }
        public string SBLanguageText { get; set; }
        public string SchBNum { get; set; }
        public string x_Description { get; set; }
        public string x_SchBNumFormatted { get; set; }
    }
}