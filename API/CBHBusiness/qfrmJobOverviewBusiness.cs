using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client = CBHBusiness.ClientModels;

namespace CBHBusiness
{
    public class qfrmJobOverviewBusiness
    {
        private CBHClassesDataContext db = new CBHClassesDataContext();

        public IQueryable<qfrmJobOverview> GetList()
        {
            var iq = from p in db.qfrmJobOverviews
                     select p;

            return iq;
        }

        public qfrmJobOverview Get(int jobkey)
        {
            var qfrmJobOverview = (from p in db.qfrmJobOverviews
                                    where p.JobKey == jobkey
                                    select p).FirstOrDefault();
            return qfrmJobOverview;
        }

        public qfrmJobOverview Update(qfrmJobOverview model)
        {

            var job = db.tblJobHeaders.Where(w => w.JobKey == model.JobKey).Single();
            job.JobModifiedDate = DateTime.Now;
            job.JobShipmentCarrier = model.JobShipmentCarrier;
            job.JobShipDate = model.JobShipDate;
            job.JobCarrierRefNum = model.JobCarrierRefNum;
            job.JobCarrierVessel = model.JobCarrierVessel;
            job.JobInspectionCertificateNum = model.JobInspectionCertificateNum;
            job.JobArrivalDate = model.JobArrivalDate;

            //db.qfrmJobOverviews.Attach(qfrmJobOverview);
            db.Refresh(System.Data.Linq.RefreshMode.KeepCurrentValues, job);
            db.SubmitChanges();
            return db.qfrmJobOverviews.Where(w => w.JobKey == model.JobKey).Single();
        }
    }
}