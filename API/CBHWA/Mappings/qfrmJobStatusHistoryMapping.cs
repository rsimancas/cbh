using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CBHBusiness;
using ClientModels = CBHBusiness.ClientModels;
using Client = CBHWA.Models;

namespace CBHWA.Mappings
{
    public class qfrmJobStatusHistoryMapping { 
        public ClientModels.qfrmJobStatusHistory MapModels(Client.qfrmJobStatusHistory model)
        {
            var data = new ClientModels.qfrmJobStatusHistory
            {
                JobKey = model.JobKey,
                QuoteNum = model.QuoteNum,
                JobClosed = model.JobClosed,
                JobComplete = model.JobComplete,
                JobModifiedBy = model.JobModifiedBy,
                JobModifiedDate = model.JobModifiedDate,
                CustName = model.CustName,
                CustEmail = model.CustEmail,
                EmployeeKey = model.EmployeeKey,
                EmployeeEmail = model.EmployeeEmail,
                ForwarderEmail = model.ForwarderEmail,
                JobProdDescription = model.JobProdDescription,
                JobCustRefNum = model.JobCustRefNum,
                ContactName = model.ContactName,
                JobStatusKey = Convert.ToByte(model.JobStatusKey),
                StatusStatusKey = model.StatusStatusKey,
                StatusMemo = model.StatusMemo,
                StatusPublic = model.StatusPublic,
                StatusDate = model.StatusDate
            };

            return data;
        }

        public Client.qfrmJobStatusHistory MapModels(ClientModels.qfrmJobStatusHistory model)
        {
            var qjo = new Client.JobsRepository().GetqfrmJobStatusHistory(model.JobKey);

            var data = new Client.qfrmJobStatusHistory
            {
                JobKey = model.JobKey,
                QuoteNum = model.QuoteNum,
                JobClosed = model.JobClosed,
                JobComplete = model.JobComplete,
                JobModifiedBy = model.JobModifiedBy,
                JobModifiedDate = model.JobModifiedDate,
                CustName = model.CustName,
                CustEmail = model.CustEmail,
                EmployeeKey = model.EmployeeKey,
                EmployeeEmail = model.EmployeeEmail,
                ForwarderEmail = model.ForwarderEmail,
                JobProdDescription = model.JobProdDescription,
                JobCustRefNum = model.JobCustRefNum,
                ContactName = model.ContactName,
                JobStatusKey = Convert.ToInt32(model.JobStatusKey),
                StatusStatusKey = model.StatusStatusKey,
                StatusMemo = model.StatusMemo,
                StatusPublic = model.StatusPublic,
                StatusDate = model.StatusDate
            };

            return data;
        }

        public Client.qfrmJobStatusHistory MapModels(qfrmJobStatusHistory model)
        {
            var qjo = new Client.JobsRepository().GetqfrmJobStatusHistory(model.JobKey);

            var data = new Client.qfrmJobStatusHistory
            {
                JobKey = model.JobKey,
                QuoteNum = model.QuoteNum,
                JobClosed = model.JobClosed,
                JobComplete = model.JobComplete,
                JobModifiedBy = model.JobModifiedBy,
                JobModifiedDate = model.JobModifiedDate,
                CustName = model.CustName,
                CustEmail = model.CustEmail,
                EmployeeKey = model.EmployeeKey,
                EmployeeEmail = model.EmployeeEmail,
                ForwarderEmail = model.ForwarderEmail,
                JobProdDescription = model.JobProdDescription,
                JobCustRefNum = model.JobCustRefNum,
                ContactName = model.ContactName,
                JobStatusKey = Convert.ToInt32(model.JobStatusKey)
            };

            return data;
        }
    }
}