using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_MVC.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            PropListViewModel = new ServiceChargeBudgetViewModel();
        }
        public ServiceChargeBudgetViewModel PropListViewModel { get; set; }
        public IEnumerable<Models.Properties> PropertyList { get; set; }
        public string viewName { get; set; }
        public object anonObj { get; set; }
        public string Name { get; set; }
    }
}