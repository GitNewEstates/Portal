using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Portal_MVC.Models;

namespace Portal_MVC.Models
{
    public class RepairsMaintenanceViewModel : ViewModelBase
    {
        public RepairsMaintenanceViewModel(ViewModelLevel level = ViewModelLevel.Estate) : base(level)
        {
            Repair = new Repairs();
        }
        public ServiceChargeBudgetViewModel PropListViewModel { get; set; }
        public IEnumerable<Models.Properties> PropertyList { get; set; }
        public Repairs Repair { get; set; }
        public CallReports CallReports { get; set; }
        public Estates ServiceChargeInfo { get; set; }
        public Contractors ContractorInfo { get; set; }

       public string AutomaticUpdateConfirmation { get; set; }

       
       
    }
    public class RepairDetailViewModel : ViewModelBase
    {
        public RepairDetailViewModel(ViewModelLevel level = ViewModelLevel.none) : base(level)
        {
            Repair = new APIRepairs();
        }
        public APIRepairs Repair { get; set; }

        public async Task SetReapir()
        {
            Repair = await RepairExtensions.GetRepair(Repair.ID, true);
        }
    }

}

