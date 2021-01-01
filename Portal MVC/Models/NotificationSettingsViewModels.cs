using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NotificationSettings;

namespace Portal_MVC.Models
{
    public class NotificationSettingsViewModels
    {
        public NotificationSettingsViewModels()
        {
            NotificationSettingObj = new NotificationSettings.NotificationSettings();
        }

        public ServiceChargeBudgetViewModel PropListViewModel { get; set; }
        public IEnumerable<Models.Properties> PropertyList { get; set; }
        public string ViewName { get; set; }
        public string ControllerName { get; set; }
        public NotificationSettings.NotificationSettings NotificationSettingObj { get; set; }

        public bool RepairNotificationStatic { get; set; }
        public bool RepairNotificationReceiveMessage { get; set; }
        public bool RepairNotificationCancelMessage { get; set; }

        public bool ChargeNotificationStatic { get; set; }
        public bool ChargeNotificationReceiveMessage { get; set; }
        public bool ChargeNotificationCancelMessage { get; set; }

        public bool PaymentNotificationStatic { get; set; }
        public bool PaymentNotificationReceiveMessage { get; set; }
        public bool PaymentNotificationCancelMessage { get; set; }

        public bool BudgetNotificationStatic { get; set; }
        public bool BudgetNotificationReceiveMessage { get; set; }
        public bool BudgetNotificationCancelMessage { get; set; }

        public bool InsuranceNotificationStatic { get; set; }
        public bool InsuranceNotificationReceiveMessage { get; set; }
        public bool InsuranceNotificationCancelMessage { get; set; }



        //wrapper so can be done on thread
        public void UpdateSettings()
        {
            NotificationSettingObj.Delete(GlobalVariables.CS);
            NotificationSettingObj.Insert(GlobalVariables.CS);
        }
    }
}