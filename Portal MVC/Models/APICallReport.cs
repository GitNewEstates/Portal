using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_MVC.Models
{
    public class APICallReport : IDBase
    {
        public APICallReport()
        {
            ReporterType = new ReporterType();
            ReportType = new ReportTyes();
            ReportDate = new DateTime();
        }
        public DateTime ReportDate { get; set; }
        public string ReportDateStr { get { return ControlsDLL.ControlActions.DateFormatLong(ReportDate); } }
        public int ReportingCustomerID { get; set; }
        public ReportTyes ReportType { get; set; }
        public string ReporterName { get; set; }
        public string ReportTitle { get; set; }
        public string ReportDetails { get; set; }
        public string ReporterEmail { get; set; }
        public string ReporterPhone { get; set; }
        public ReporterType ReporterType { get; set; }

        public int EstateID { get; set; }
    }

    public class ReportTyes : IDBase
    {
        public string ReportType { get; set; }
    }

    public class ReporterType : NameBase
    {
        public string reporterType { get; set; }
    }
}