namespace CBHWA.Models
{
    using CBHWA.Clases;
    using System.Collections.Generic;

    interface ICustomersRepository
    {
        IList<Customer> GetList(FieldFilters fieldFilters, string query, Sort sort, string[] queryBy, int page, int start, int limit, ref int totalRecords);
        IList<CustomerForReport> GetListForReport(string startDate, string endDate, string reportName, string query, int page, int start, int limit, ref int totalRecords);

        Customer Get(int id);
        Customer Add(Customer customer, ref string messageError);
        bool Remove(Customer customer);
        Customer Update(Customer customer, ref string messageError);

        IList<CustomerStatus> GetCustomerStatus();

        IList<CustomerContact> GetListContacts(string query, Sort sort, Filter filter, int page, int start, int limit, ref int totalRecords);
        CustomerContact GetContact(int contactkey);
        IList<CustomerContact> GetContactsByCustomer(string query, int custkey, int page, int start, int limit,  ref int totalRecords);
        CustomerContact AddContact(CustomerContact contact, ref string msgError);
        CustomerContact UpdContact(CustomerContact contact, ref string msgError);
        bool RemoveContact(CustomerContact contact, ref string msgError);

        IList<CustomerShipAddress> GetShipAddressById(int idaddress);
        IList<CustomerShipAddress> GetShipAddressByCustomer(int custkey);
        CustomerShipAddress AddShipAddress(CustomerShipAddress address, ref string msgError);
        CustomerShipAddress UpdShipAddress(CustomerShipAddress address, ref string msgError);
        bool RemoveShipAddress(CustomerShipAddress address, ref string msgError);
        
        IList<CustomerContactForReport> GetContactsByCustomer(int custkey);
    }
}
