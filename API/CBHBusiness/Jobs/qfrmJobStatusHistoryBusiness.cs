using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client = CBHBusiness.ClientModels;

namespace CBHBusiness
{
    public class qfrmJobStatusHistoryBusiness
    {
        private CBHClassesDataContext db = new CBHClassesDataContext();

        public IQueryable<qfrmJobStatusHistory> GetList()
        {
            var iq = from p in db.qfrmJobStatusHistories
                     select p;

            return iq;
        }

        public qfrmJobStatusHistory Get(int jobkey)
        {
            var qfrmJobStatusHistory = (from p in db.qfrmJobStatusHistories
                                    where p.JobKey == jobkey
                                    select p).FirstOrDefault();
            return qfrmJobStatusHistory;
        }

        public qfrmJobStatusHistory Update(Client.qfrmJobStatusHistory model)
        {

            var job = db.tblJobHeaders.Where(w => w.JobKey == model.JobKey).Single();
            job.JobModifiedDate = DateTime.Now;
            job.JobModifiedBy = model.JobModifiedBy;
            job.JobClosed = model.JobClosed;
            job.JobComplete = model.JobComplete != null && model.JobComplete.Value.Year > 1 ? model.JobComplete : null;
            job.JobStatusKey = Convert.ToByte(model.StatusStatusKey);

            var his = new tblJobStatusHistory();
            his.JobStatusJobKey = model.JobKey;
            his.JobStatusDate = model.StatusDate;
            his.JobStatusMemo = model.StatusMemo;
            his.JobStatusPublic = model.StatusPublic;
            his.JobStatusStatusKey = model.JobStatusKey;
            his.JobStatusModifiedDate = DateTime.Now;
            his.JobStatusModifiedBy = model.JobModifiedBy;
            db.tblJobStatusHistories.InsertOnSubmit(his);

            db.Refresh(System.Data.Linq.RefreshMode.KeepCurrentValues, job);
            db.SubmitChanges();

            return db.qfrmJobStatusHistories.Where(w => w.JobKey == model.JobKey).Single();
        }
    }
}