using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IPaymentTermsRepository
    {
        IList<PaymentTerms> GetList(string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        IList<PaymentTerms> GetListForDDL(string query, Sort sort, int page, int start, int limit, ref int totalRecords);
        PaymentTerms Get(int id);
        PaymentTerms Add(PaymentTerms added);
        bool Remove(PaymentTerms deleted);
        PaymentTerms Update(PaymentTerms updated);
        IList<PaymentTermsDescriptions> GetDescriptionByTermKey(int termkey, ref int totalRecords);
        IList<PaymentTermsDescriptions> GetWithQueryDescription(string query, int page, int start, int limit, ref int totalRecords);
        PaymentTermsDescriptions GetDescription(int id);
        PaymentTermsDescriptions AddDescription(PaymentTermsDescriptions added);
        bool RemoveDescription(PaymentTermsDescriptions deleted);
        PaymentTermsDescriptions UpdateDescription(PaymentTermsDescriptions updated);
    }
}
