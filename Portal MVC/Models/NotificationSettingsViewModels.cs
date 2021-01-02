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

        public void CompareNotificationSettings()
        {
            //compares old notification settings with those submitted and displays relevant 
            NotificationSettings.NotificationSettings NewObj = new NotificationSettings.NotificationSettings();
            NewObj.UnitID = NotificationSettingObj.UnitID;
            NewObj.CustomerID = NotificationSettingObj.CustomerID;
            NewObj.GetNotificationSettings(GlobalVariables.CS);

            if (!NewObj.NewRepairNotification || !NotificationSettingObj.NewRepairNotification)
            {
                if (!NewObj.NewRepairNotification && NotificationSettingObj.NewRepairNotification)
                {
                    this.RepairNotificationReceiveMessage = true;
                }
                else if (NewObj.NewRepairNotification && !NotificationSettingObj.NewRepairNotification)
                {
                    this.RepairNotificationCancelMessage = true;
                }
            }

            if (!NewObj.NewAccountCharge || !NotificationSettingObj.NewAccountCharge)
            {
                if (!NewObj.NewAccountCharge && NotificationSettingObj.NewAccountCharge)
                {
                    this.ChargeNotificationReceiveMessage = true;
                }
                else if (NewObj.NewAccountCharge && !NotificationSettingObj.NewAccountCharge)
                {
                    this.ChargeNotificationCancelMessage = true;
                }
            }

            if (!NewObj.NewAccountPayment || !NotificationSettingObj.NewAccountPayment)
            {
                if (!NewObj.NewAccountPayment && NotificationSettingObj.NewAccountPayment)
                {
                    this.PaymentNotificationReceiveMessage = true;
                }
                else if (NewObj.NewAccountPayment && !NotificationSettingObj.NewAccountPayment)
                {
                    this.PaymentNotificationCancelMessage = true;
                }
            }

            if (!NewObj.NewSCBudget || !NotificationSettingObj.NewSCBudget)
            {
                if (!NewObj.NewSCBudget && NotificationSettingObj.NewSCBudget)
                {
                    this.BudgetNotificationReceiveMessage = true;
                }
                else if (NewObj.NewSCBudget && !NotificationSettingObj.NewSCBudget)
                {
                    this.BudgetNotificationCancelMessage = true;
                }
            }

            if (!NewObj.NewInsurance || !NotificationSettingObj.NewInsurance)
            {
                if (!NewObj.NewInsurance && NotificationSettingObj.NewInsurance)
                {
                    this.InsuranceNotificationReceiveMessage = true;
                }
                else if (NewObj.NewInsurance && !NotificationSettingObj.NewInsurance)
                {
                    this.InsuranceNotificationCancelMessage = true;
                }
            }
        }

        public void HideAllConfirmations()
        {
            InsuranceNotificationCancelMessage = false;
            InsuranceNotificationReceiveMessage = false;

            BudgetNotificationCancelMessage = false;
            BudgetNotificationReceiveMessage = false;

            PaymentNotificationCancelMessage = false;
            PaymentNotificationReceiveMessage = false;

            ChargeNotificationCancelMessage = false;
            PaymentNotificationReceiveMessage = false;

            RepairNotificationCancelMessage = false;
            RepairNotificationReceiveMessage = false;
        }

    }
}