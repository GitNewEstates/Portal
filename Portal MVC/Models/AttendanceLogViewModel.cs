using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Portal_MVC.Models
{
    public class AttendanceLogViewModel
    {
        public IEnumerable<Properties> EstateList { get; set; }

        
        public bool CaretakingVisit { get; set; }
        public bool InspectionVisit { get; set; }
        public bool CallOutVisit { get; set; }
    }
}