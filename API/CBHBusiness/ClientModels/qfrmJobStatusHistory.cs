using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBHBusiness.ClientModels
{
    public class qfrmJobStatusHistory
    {
        public int JobKey { get; set; }
        public string JobModifiedBy { get; set; }
        public Nullable<DateTime> JobModifiedDate { get; set; }
        public int? EmployeeKey { get; set; }
        public string EmployeeEmail { get; set; }
        public string CustEmail { get; set; }
        public string ForwarderEmail { get; set; }
        public Nullable<DateTime> JobClosed { get; set; }
        public Nullable<DateTime> JobComplete { get; set; }
        public string QuoteNum { get; set; }
        public string JobProdDescription { get; set; }
        public string JobCustRefNum { get; set; }
        public string CustName { get; set; }
        public string ContactName { get; set; }
        public byte JobStatusKey { get; set; }
        public int StatusStatusKey { get; set; }
        public string StatusMemo { get; set; }
        public bool StatusPublic { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
