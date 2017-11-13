namespace CBHWA.Areas.Reports.Models
{
    public class Criteria
    {
        public int CriteriaKey { get; set; }
        public int CriteriaEmployeeKey { get; set; }
        public string CriteriaRptName { get; set; }
        public string CriteriaFieldName { get; set; }
        public int CriteriaValue { get; set; }
    }

    public class CriteriaParam
    {
        public string strWhere { get; set; }
        public string labelCriteria { get; set; }
        public int employeeKey { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int NoProfit { get; set; }
    }
}