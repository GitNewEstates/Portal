using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_MVC.Models
{
    public class ServiceChargeBudgetViewModel
    {
        
        public IEnumerable<Models.Properties> PropertyList { get; set; }
        public Models.Properties SelectedProp { get; set; }
        public Models.Estates Estate {get; set;}
        public string ViewName { get; set; }
        public string ControllerName { get; set; }
        public ServiceCharges MyServiceCharges { get; set; }
    }
}