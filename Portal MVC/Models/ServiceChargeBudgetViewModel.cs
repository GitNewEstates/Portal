using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_MVC.Models
{
    public class ServiceChargeBudgetViewModel : ViewModelBase
    {
        public ServiceChargeBudgetViewModel(ViewModelLevel level):base(level)
        {

        }
        public IEnumerable<Models.Properties> PropertyList { get; set; }
        public Models.Properties SelectedProp { get; set; }
        public Models.Estates Estate {get; set;}
      
        public ServiceCharges MyServiceCharges { get; set; }
    }
}