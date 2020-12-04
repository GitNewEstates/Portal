using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_MVC.Models
{
    public class NotificationSettingsViewModels
    {
        public NotificationSettingsViewModels()
        {
            NotificationSettingObj = new NotificationSettings();
        }

        public ServiceChargeBudgetViewModel PropListViewModel { get; set; }
        public IEnumerable<Models.Properties> PropertyList { get; set; }
        public string ViewName { get; set; }
        public string ControllerName { get; set; }
        public NotificationSettings NotificationSettingObj { get; set; }
    }
}