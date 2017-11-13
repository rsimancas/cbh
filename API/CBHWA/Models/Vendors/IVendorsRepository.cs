using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IVendorsRepository
    {
        //IList<Vendor> GetList(int page, int start, int limit, ref int totalRecords);
        IList<Vendor> GetList(string query, Sort sort, string[] queryBy, int page, int start, int limit, ref int totalRecords, int quoteFileKey, int VendorCarrier, int QHdrKey);
        Vendor Get(int id);
        Vendor Add(Vendor fileheader);
        Vendor Update(Vendor fileheader);
        bool Remove(Vendor vendor, ref string msgError);
        LastQuoteMargin GetLastQuoteMargin(int vendorkey);

        IList<VendorContact> GetContactsById(int filekey);
        IList<VendorContact> GetContactsByVendor(int vendorkey, ref int totalRecords);
        VendorContact Add(VendorContact contact, ref string msgError);
        VendorContact Update(VendorContact contact, ref string msgError);
        bool Remove(VendorContact contact, ref string msgError);

        IList<VendorOriginAddress> GetOriginAddressAll(ref int totalRecords);
        IList<VendorOriginAddress> GetOriginAddressById(int filekey);
        IList<VendorOriginAddress> GetOriginAddressByVendor(int id, ref int totalRecords);
        VendorOriginAddress Add(VendorOriginAddress contact, ref string msgError);
        VendorOriginAddress Update(VendorOriginAddress contact, ref string msgError);
        bool Remove(VendorOriginAddress contact, ref string msgError);

        IList<VendorWarehouse> GetWarehouseById(int filekey);
        IList<VendorWarehouse> GetWarehouseByVendor(int id, ref int totalRecords);
        VendorWarehouse Add(VendorWarehouse contact, ref string msgError);
        VendorWarehouse Update(VendorWarehouse contact, ref string msgError);
        bool Remove(VendorWarehouse contact, ref string msgError);

        IList<WarehouseType> GetWarehouseTypesById(int key);
        IList<WarehouseType> GetWarehouseTypesByVendor(int vendorkey, ref int totalRecords);
        IList<WarehouseType> GetWarehouseTypes();

        IList<VendorForReport> GetList();
    }
}
