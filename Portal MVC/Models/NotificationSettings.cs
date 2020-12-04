using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Portal_MVC.Models
{
    public class NotificationSettings
    {
       

        public int CustomerID { get; set; }
        public int PropertyID { get; set; }

        public bool NewRepairNotification { get; set; }

        public string NewRepairConfirmation
        {
            get
            {
                return "You will receive an email when a new repair is raised.";
            }
        }

        public bool NewAccountCharge { get; set; }

        public string NewAccountChargeConfirmation
        {
            get
            {
                return "You will receive an email when a new charge is added to your service charge account.";
            }
        }

        public bool NewAccountPayment { get; set; }

        public string NewAccountPaymentConfirmation
        {
            get
            {
                return "You will receive an email when a new payment is posted to your service charge account.";
            }
        }

        public bool NewSCBudget { get; set; }

        public string NewSCBudgetConfirmation
        {
            get
            {
                return "You will receive an email when a Service Charge Budget is created.";
            }
        }

        public bool NewInsurance { get; set; }

        public string NewInsuranceConfrimation
        {
            get
            {
                return "You will receive an email when a new Insurance Schedule is logged.";
            }
        }

        private void DoInsert(string colName, bool Param)
        {
            List<string> c = new List<string>();
            List<string> p = new List<string>();
            List<object> o = new List<object>();

            c.Add(colName);
            p.Add("@param");
            o.Add(Param);

            string where = " where customerID = " + CustomerID.ToString() + " and unitid = " + PropertyID.ToString();

            dbConn.dbConnection db = new dbConn.dbConnection();
            db.UpdateCommand(GlobalVariables.CS, "core.PortalNotificationSettings", c, p, o);
        }

        public void UpdateRepair()
        {
            DoInsert("NewRepair", this.NewRepairNotification);
        }

        public void UpdateNewCharge()
        {
            DoInsert("NewCharge", this.NewAccountCharge);
        }
        public void UpdatNewPayment()
        {
            DoInsert("NewPayment", this.NewAccountPayment);
        }
        public void UpdateSCBudget()
        {
            DoInsert("NewScBudget", this.NewSCBudget);
        }
        public void UpdateInsurance()
        {
            DoInsert("NewInsurance", this.NewInsurance);
        }
       

    }
}