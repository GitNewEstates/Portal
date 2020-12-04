using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Portal_MVC.Models
{
    public class Insurance
    {
        public int PolicyID { get; set; }
        public int EstateID { get; set; }
        public int BrokerSupplierID { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Documents> Documents { get; set; }
        public string ClaimInstructions { get; set; }

        public long DocInstanceID { get; set; }

        public string PremiumAmountString { get { return Controls.CurrencyString(PremiumAmount); } }

        public double PremiumAmount { get; set; }

        public string BrokerName { get; set; }

        public string StartDateStr
        {
            get
            {
                return StartDate.ToShortDateString();
            }
        }

        public string EndDateStr
        {
            get
            {
                return EndDate.ToShortDateString();
            }
        }

        public List<InsuranceExcessObj> ExcessList { get; set; }
    }

    public static class InsuranceMethods
    {
        public static List<Insurance> GetInsuranceList(int unitID)
        {
            string q = "Select core.Insurances.id, core.Insurances.policyname, core.Insurances.startdate, enddate from core.Insurances " +
                "inner join core.Units on core.units.EstateID = core.Insurances.EstateID where core.Units.id = " + unitID.ToString();


            dbConn.dbConnection db = new dbConn.dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

            List<Insurance> r = new List<Insurance>();

            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach (DataRow dr in dt.Rows)
                {
                   // int.TryParse(dr[0].ToString(), out int id);
                    int id = Convert.ToInt32(dr[0]);
                    //start date
                    DateTime st = new DateTime();
                    if (dr[2] != DBNull.Value)
                    {
                        st = Convert.ToDateTime(dr[2]);
                    }

                    //enddate 
                    
                    DateTime en = new DateTime();
                    if (dr[3] != DBNull.Value)
                    {
                        en = Convert.ToDateTime(dr[3]);
                    }

                    r.Add(new Insurance
                    {
                        PolicyID = id,
                        PolicyName = dr[1].ToString(),
                        StartDate = st,
                        EndDate = en
                    });
                }
            }

            return r;


        }

        public static Insurance GetInsuranceByPolicyID(int PolicyID)
        {
            string q = "select core.insurances.policyNumber, core.insurances.PolicyName, core.insurances.StartDate, " +
                        "core.insurances.EndDate, core.insurances.ClaimInstructions, core.insurances.DocInstanceID, " +
                        "core.insurances.premium, core.contractors.Name from core.insurances " +
                        "inner join core.Contractors on core.Insurances.BrokerSUpplierID = core.Contractors.id " +
                        "where core.Insurances.id = " + PolicyID.ToString();

            Insurance r = new Insurance();

            dbConn.dbConnection db = new dbConn.dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {


                foreach(DataRow dr in dt.Rows)
                {
                    //start date
                    DateTime st = new DateTime();
                    if (dr[2] != DBNull.Value)
                    {
                        st = Convert.ToDateTime(dr[2]);
                    }

                    //enddate 

                    DateTime en = new DateTime();
                    if (dr[3] != DBNull.Value)
                    {
                        en = Convert.ToDateTime(dr[3]);
                    }
                    
                    //long.TryParse(dr[5].ToString(), out long doc);
                    long doc = long.Parse(dr[5].ToString());
                    double p = Convert.ToDouble(dr[6]);

                    r.PolicyID = PolicyID;
                    r.PolicyNumber = dr[0].ToString();
                    r.PolicyName = dr[1].ToString();
                    r.StartDate = st;
                    r.EndDate = en;
                    r.ClaimInstructions = dr[4].ToString();
                    r.DocInstanceID = doc;
                    r.PremiumAmount = p; 
                    r.BrokerName = dr[7].ToString();


                }
            }

            return r;
        }
    }

    public class InsuranceExcessObj
    {
        public int ExcessID { get; set; }
        public int PolicyID { get; set; }
        public string ExcessName { get; set; }
        public double ExcessAmount { get; set; }
        public string ExcessAmountString { get; set; }
    }
}