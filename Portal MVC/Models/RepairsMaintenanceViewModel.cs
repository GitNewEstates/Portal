using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal_MVC.Models;

namespace Portal_MVC.Models
{
    public class RepairsMaintenanceViewModel : ViewModelBase
    {
        public ServiceChargeBudgetViewModel PropListViewModel { get; set; }
        public IEnumerable<Models.Properties> PropertyList { get; set; }
        public Repairs Repair { get; set; }
        public CallReports CallReports { get; set; }
        public Estates ServiceChargeInfo { get; set; }
        public Contractors ContractorInfo { get; set; }

       public string AutomaticUpdateConfirmation { get; set; }
       
    }
}