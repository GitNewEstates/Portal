using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dbConn;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Portal_MVC.Models
{
    public class Properties
    {
        public Properties(PropertyTypes _type)
        {
            PropertyType = _type;
        }

        [MaxLength(30, ErrorMessage = "Cannot be greater than 30 characters")]
        public string Address1 { get; set; }

        [Required]
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address5 { get; set; }
        public string FullAddress { get; set; }
        public int ID { get; set; }
        public PropertyTypes PropertyType { get; set; }
    }

    public enum PropertyTypes
    {
        Estate,
        Unit,
        Owner, 
        None
    }

    public static class PropertyMethods
    {
        public static string SetAddress1(string FlatOrApt, string number, string blockname)
        {
            string returnString = "";
            if (FlatOrApt != null && FlatOrApt != "")
            {
                returnString = FlatOrApt;
            }
            if (number != null && number != "")
            {
                returnString = returnString + " " + number;
            }
            if (blockname != null && blockname != "")
            {
                returnString = returnString + " " + blockname;
            }

            return returnString;
        }

        public static string SetAddressString(string add1, string add2, string add3, string add4, string add5)
        {

           
            string r = "";
            if (add1 != null && add1 != "")
            {
                r = add1;
            }

            if (add2 != null && add2 != "")
            {
                r = r + ", " + add2;
            }
            if (add3 != null && add3 != "")
            {
                r = r + ", " + add3;
            }
            if (add4 != null && add4 != "")
            {
                r = r + ", " + add4;
            }
            if (add5 != null && add5 != "")
            {
                r = r + ", " + add5;
            }

            return r;
        }

        public static List<Properties> GetAllOwnedProperties(int CustomerID)
        {
            string q = "select Core.Units.FlatOrApt, Core.Units.UnitNumber, Core.Units.Address1, Core.Units.Address2, Core.Units.Address3, " +
                "Core.Units.Address4, Core.Units.Address5, Core.Units.ID FROM CORE.Units inner join core.PropertyOwnership on core.PropertyOwnership.UnitID = " +
                "core.Units.ID where core.PropertyOwnership.OwnerID = " + CustomerID.ToString();

            List<Properties> rList = new List<Properties>();
            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);
            
            foreach (DataRow dr in dt.Rows)
            {
                int id = 0;
                if (dr[7] != DBNull.Value)
                {
                    id = Convert.ToInt32(dr[7]);
                }
                string Add1 = SetAddress1(dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
                rList.Add(new Properties(PropertyTypes.Unit) { ID = id, Address1 = Add1, Address2 = dr[3].ToString(), Address3 = dr[4].ToString(), Address4 = dr[5].ToString(), Address5 = dr[6].ToString(), FullAddress = SetAddressString(Add1, dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString()) });
            }
            return rList;

        }

        public static List<Properties> GetAllUnitsProperties(int estateid)
        {
            string q = "select Core.Units.FlatOrApt, Core.Units.UnitNumber, Core.Units.Address1, Core.Units.Address2, Core.Units.Address3, " +
                "Core.Units.Address4, Core.Units.Address5, Core.Units.ID FROM CORE.Units where estateid = " + estateid.ToString();

            List<Properties> rList = new List<Properties>();
            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

            foreach (DataRow dr in dt.Rows)
            {
                int id = 0;
                if (dr[7] != DBNull.Value)
                {
                    id = Convert.ToInt32(dr[7]);
                }
                string Add1 = SetAddress1(dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
                rList.Add(new Properties(PropertyTypes.Unit) { ID = id, Address1 = Add1, Address2 = dr[3].ToString(), Address3 = dr[4].ToString(), Address4 = dr[5].ToString(), Address5 = dr[6].ToString(), FullAddress = SetAddressString(Add1, dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString()) });
            }
            return rList;

        }

        public static List<Properties> GetAllUnitsowners(int UnitID)
        {
            string q = "select Core.leaseholders.title, Core.leaseholders.firstname," +
                "core.leaseholders.surname, Core.leaseholders.Address_1, Core.leaseholders.Address_2, " +
                "Core.leaseholders.Address_3, " +
                "Core.leaseholders.Address_4, Core.leaseholders.Address_5, Core.leaseholders.ID " +
                "FROM CORE.leaseholders " +
                "inner join core.PropertyOwnership on core.Leaseholders.ID = core.PropertyOwnership.OwnerID " +
                "where core.PropertyOwnership.UnitID = " + UnitID.ToString() + " and core.propertyownership.currentowner = 1";

            List <Properties> rList = new List<Properties>();
            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int id = 0;
                    if (dr[8] != DBNull.Value)
                    {
                        id = Convert.ToInt32(dr[8]);
                    }
                    string Add1 = SetAddress1(dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
                    rList.Add(new Properties(PropertyTypes.Owner) { ID = id, Address1 = Add1, FullAddress = Add1 });
                }
            }
            return rList;

        }



        public static List<Properties> GetAllEstates()
        {
            string q = "select * FROM CORE.Estates";

            List<Properties> rList = new List<Properties>();
            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

            foreach (DataRow dr in dt.Rows)
            {
                int id = 0;
                if (dr[0] != DBNull.Value)
                {
                    id = Convert.ToInt32(dr[0]);
                }
                string Add1 = SetAddress1(dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
                rList.Add(new Properties(PropertyTypes.Estate) { ID = id,
                    Address1 = dr[1].ToString(),
                    Address2 = dr[2].ToString(),
                    Address3 = dr[3].ToString(),
                    Address4 = dr[4].ToString(),
                    Address5 = dr[5].ToString(),
                    FullAddress = SetAddressString(dr[1].ToString(), dr[2].ToString(),
                    dr[3].ToString(), dr[4].ToString(), dr[5].ToString()) });
            }
            return rList;

        }

        public static string AddressTable(string add1, string add2, string add3, string add4, string add5, int AddressNumber)
        {
            string r = "";
            switch (AddressNumber)
            {
                case 1:
                    if (add1 == "" || add1 == null)
                    {
                        if (add2 == "" || add2 == null)
                        {
                            if (add3 == "" || add3 == null)
                            {
                                if (add4 == "" || add4 == null)
                                {
                                    if (add5 == "" || add5 == null)
                                    {

                                    }
                                    else
                                    {
                                        r = add5;
                                    }
                                }
                                else
                                {
                                    r = add4;
                                }
                            }
                            else
                            {
                                r = add3;
                            }
                        }
                        else
                        {
                            r = add2;
                        }

                    } else
                    {
                        r = add1;
                    }



                    break;
                case 2:
                    if (add2 == "" || add2 == null)
                    {
                        if (add3 == "" || add3 == null)
                        {
                            if (add4 == "" || add4 == null)
                            {
                                if (add5 == "" || add5 == null)
                                {

                                }
                                else
                                {
                                    r = add5;
                                }
                            }
                            else
                            {
                                r = add4;
                            }
                        }
                        else
                        {
                            r = add3;
                        }
                    }
                    else
                    {
                        r = add2;
                    }


                    break;
                case 3:

                    if (add3 == "" || add3 == null)
                    {
                        if (add4 == "" || add4 == null)
                        {
                            if (add5 == "" || add5 == null)
                            {

                            }
                            else
                            {
                                r = add5;
                            }
                        }
                        else
                        {
                            r = add4;
                        }
                    }
                    else
                    {
                        r = add3;
                    }

                    break;
                case 4:

                    if (add4 == "" || add4 == null)
                    {
                        if (add5 == "" || add5 == null)
                        {

                        }
                        else
                        {
                            r = add5;
                        }
                    }
                    else
                    {
                        r = add4;
                    }


                    break;
                case 5:
                    if (add5 == "" || add5 == null)
                    {

                    }
                    else
                    {
                        r = add5;
                    }
                    break;
            }
            return r;
        }

        public static string GetPropertyName(int unitid)
        {
            string q = "select concat(flatorapt, ' ', UnitNumber, ' ', Address1) from core.Units where id = " +
                unitid.ToString();

            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);
            string r = "";
            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                r = dt.Rows[0][0].ToString();

            }
            return r;
        }

        public static string PropertyAddress(int UnitID)
        {
            string q = "select Core.Units.FlatOrApt, Core.Units.UnitNumber, Core.Units.Address1 from " +
                       " core.units where ID = " + UnitID.ToString();

            string r = "";
            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);
            foreach (DataRow dr in dt.Rows)
            {
                r = SetAddress1(dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
            }
            return r;

        }


    }

    public class Estates
    {
        public string EstateName { get; set; }
        public int EstatedID { get; set; }
        public string SCStartDate { get; set; }
        public string SCEndDate { get; set; }
        public string SCScheduleName { get; set; }
        public string SCHeading { get; set; }
        public double SCCost { get; set; }
        public string SCCoststr { get; set; }
        public List<Models.Estates> BudgetList { get; set; }
        public int BudgetId { get; set; }
        public double Apportionment { get; set; }
        private string _strApp;
        public string strApportionment
        {
            get
            {
                return _strApp + "%";
            }
            set
            {
                _strApp = value;
            }
        }
        public double contribution { get; set; }
        public string strContribution { get; set; }
        public List<string> Schedules { get; set; }
        public DataTable PieChartData { get; set; }
        public string TransDate { get; set; }
        public string TransDescription { get; set; }
        public string InError { get; set; }
        public string TransType { get; set; }
        public List<Models.Estates> TransactionList { get; set; }
        public int TransID { get; set; }
        public int PONumber { get; set; }
        public double TrCost { get; set; }
        public string TrCoststr { get; set; }
        public List<Models.Estates> BVAList { get; set; }
        public double BVADiff { get; set; }
        public string BVADiffstr { get; set; }
        public double TotalBudget { get; set; }
        public string TotalBudgetstr { get; set; }
        public double TotalExp { get; set; }
        public string TotalExpstr { get; set; }
        public double TotalDiff { get; set; }
        public string TotalDiffstr { get; set; }
        public int HeadingID { get; set; }

        public string BudgetNotes { get; set; }
        public string ServiceChargePeriod { get; set; }

        public string _status { get; set; }

        public string FundName { get; set; }


    }

    public static class EstateMethods
    {
        public static List<Estates> GetServiceChargePeriodList(int EstateID)
        {
            List<Estates> rlist = new List<Estates>();

            string q = "Select core.ServiceChargeBudgets.ID, core.ServiceChargeBudgets.startDate, " +
                "core.ServiceChargeBudgets.endDate, core.ServiceChargeBudgets._status, " +
                "core.ServiceChargeBudgets.chargePeriod, core.FundTypes.FundType, core.ServiceChargeBudgets.BudgetName " +
                "from core.ServiceChargeBudgets " +
                "inner join core.FundTypes on core.FundTypes.Id = core.ServiceChargeBudgets.FundTypeID " +
                "where core.ServiceChargeBudgets.EstateID = " + EstateID.ToString() + " and core.servicechargebudgets.fundtypeID = 2" +
                " and core.ServiceChargeBudgets._status <> 'Progress'";

            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);
            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DateTime sdate = new DateTime();
                    if (dr[1] != DBNull.Value)
                    {
                        sdate = Convert.ToDateTime(dr[1]);
                    }
                    DateTime edate = new DateTime();
                    if (dr[2] != DBNull.Value)
                    {
                        edate = Convert.ToDateTime(dr[2]);
                    }
                    rlist.Add(new Estates()
                    {
                        BudgetId = Convert.ToInt32(dr[0]),
                        SCStartDate = sdate.ToShortDateString(),
                        SCEndDate = edate.ToShortDateString(),
                        ServiceChargePeriod = SetServiceChargePeriod(sdate.ToShortDateString(),
                        edate.ToShortDateString()),
                        _status = dr[3].ToString(),
                        FundName = dr[6].ToString(),
                        EstatedID = EstateID
                    });
                }
            }


            return rlist;
        }

        public static string SetServiceChargePeriod(string StartDateString, string EndDateString)
        {
            string rstr = "";
            if (!string.IsNullOrWhiteSpace(StartDateString) && !string.IsNullOrWhiteSpace(EndDateString))
            {
                rstr = StartDateString + " - " + EndDateString;

            }

            return rstr;
        }

        public static Models.Estates GetServiceChargeBudget(Estates Estate, int PropertyID)
        {
            //get current year service charge budget
          

            string q = "select core.ServiceChargeBudgets.startDate, core.ServiceChargeBudgets.endDate, " +
                "core.ServiceChargeBudgetDetail.ScheduleName, core.ServiceChargeBudgetDetail.Heading, " +
                "core.ServiceChargeBudgetDetail.Cost, core.ServiceChargeBudgets.ID, core.ServiceChargeBudgets._status, " +
                " core.servicechargebudgetdetail.notes from core.ServiceChargeBudgets inner join " +
                "core.ServiceChargeBudgetDetail on core.ServiceChargeBudgetDetail.BudgetID = " +
                "core.ServiceChargeBudgets.ID where core.ServiceChargeBudgets.ID = " + Estate.BudgetId.ToString() +
                "and core.ServiceChargeBudgets._status = 'Completed' " +
                "order by displayorder asc";

            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);
            Estate.BudgetList = new List<Estates>();
            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                DataRow d = dt.Rows[0];
                if (d[0].ToString() != "Error")
                {

                    foreach (DataRow dr in dt.Rows)
                    {
                        double c = 0;
                        if (dr[4] != DBNull.Value)
                        {
                            c = Convert.ToDouble(dr[4]);
                        }
                        DateTime scstart = new DateTime();
                        if (dr[0] != DBNull.Value)
                        {
                            scstart = Convert.ToDateTime(dr[0]);
                        }

                        DateTime scEnd = new DateTime();
                        if (dr[1] != DBNull.Value)
                        {
                            scEnd = Convert.ToDateTime(dr[1]);
                        }
                        string cstr = Controls.CurrencyString(c);
                        Estate.BudgetList.Add(new Estates
                        {
                            SCStartDate = Controls.DateString(scstart),
                            SCEndDate = Controls.DateString(scEnd),
                            SCScheduleName = dr[2].ToString(),
                            SCHeading = dr[3].ToString(),
                            SCCost = c,
                            SCCoststr = cstr,
                            BudgetId = Convert.ToInt32(dr[5]),
                            BudgetNotes = dr[7].ToString()
                        });

                    }
                }

                //string sched = "";
                bool Addsched = true;
                double app = 0;
                Estate.Schedules = new List<string>();
                foreach (Estates es in Estate.BudgetList)
                {
                    Addsched = true;
                    foreach (string sched in Estate.Schedules)
                    {
                        if (es.SCScheduleName == sched)
                        {
                            Addsched = false;
                            break;
                        }
                    }

                    if (Addsched == true)
                    {

                        //get apportionements. 
                        app = GetApportionment(es.BudgetId, es.SCScheduleName, PropertyID);
                        es.Apportionment = app;
                        es.strApportionment = app.ToString();

                        //contribution

                        es.contribution = es.SCCost * (es.Apportionment / 100);
                        es.strContribution = Controls.CurrencyString(es.contribution);

                        //
                        Estate.Schedules.Add(es.SCScheduleName);
                    }
                    else
                    {
                        es.Apportionment = app;
                        es.strApportionment = app.ToString();
                        es.contribution = es.SCCost * (es.Apportionment / 100);
                        es.strContribution = Controls.CurrencyString(es.contribution);
                    }

                }
            }
            return Estate;
        }

       

        public static Estates GetEstatedByUnitID(int UnitID)
        {
            string q = "select core.Estates.ID, core.Estates.Name from Core.Estates " +
                        "inner join core.Units on core.Estates.ID = core.Units.EstateID " +
                        "where core.Units.ID = " + UnitID.ToString();

            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

            Estates r = new Estates();
            foreach (DataRow dr in dt.Rows)
            {
                r.EstatedID = Convert.ToInt32(dr[0]);
                r.EstateName = dr[1].ToString();
            }

            return r;
        }

        public static double GetApportionment(int budgetID, string scheduleName, int PropertyID)
        {
            string q = "Select Apportionment from core.ServiceChargeBudgetApportionments where " +
                "budgetID = " + budgetID.ToString() + " and ScheduleName = '" + scheduleName + "' and " +
                " unitID = " + PropertyID.ToString();

            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(Models.GlobalVariables.CS, q);

            double r = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0].ToString() != "Error")
                {
                    if (dr[0] != DBNull.Value)
                    {
                        string h = dr[0].ToString();
                        r = Convert.ToDouble(dr[0]);
                    }
                }
            }

            return r;



        }

        public static Estates GetServiceChargeExpenditure(Estates Estate)
        {
            //gets budget ID
            Estate.TransactionList = new List<Estates>();
            string q = "select core.ServiceChargeBudgets.ID from core.ServiceChargeBudgets where estateID = " 
                + Estate.EstatedID + " and not fundtypeid = 4 and not fundtypeid = 5";
            //exclude opening balance funds and admin funds

            dbConnection db = new dbConnection();
            DataTable dt1 = db.GetDataTable(GlobalVariables.CS, q);

            if (dt1.Rows.Count > 0 && dt1.Rows[0][0].ToString() != "Error")
            {
                //set string for all budgetids

                q = "select cast(core.authorisedinvoices.invoicedate as date), transdesc, transamount, " +
                    "core.ServiceChargeBudgetDetail.Heading from core.Transactions " +
                    "inner join core.ServiceChargeBudgetDetail on core.Transactions.HeadingID = " +
                    "core.ServiceChargeBudgetDetail.id " + 
                    " inner join core.authorisedinvoices on core.transactions.id = core.authorisedinvoices.invoiceid " +
                    "where core.Transactions.TransactionTypeID = 4 " +
                    "and inerror is null " +
                    "and core.transactions.budgetid = ";

                for(int i = 0; i <= dt1.Rows.Count - 1; i++)
                {
                    int bid = 0;
                    if(i == 0)
                    {
                        int.TryParse(dt1.Rows[i][0].ToString(), out bid);
                        q += bid.ToString() + " ";
                    } else
                    {
                        int.TryParse(dt1.Rows[i][0].ToString(), out bid);
                        q += "or core.transactions.budgetid  = " + bid.ToString() + " ";
                    }
                }

                q +=  "order by core.authorisedinvoices.invoicedate desc"; 

                DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

                Estate.TransactionList = new List<Estates>();

                if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
                {
                   
                    foreach (DataRow dr in dt.Rows)
                    {
                        //Transaction date
                        DateTime TDate = new DateTime();
                        if (dr[0] != DBNull.Value)
                        {
                            DateTime.TryParse(dr[0].ToString(), out TDate);
                        }

                        //transaction cost
                        double c = 0;
                        if (dr[2] != DBNull.Value)
                        {
                            c = Convert.ToDouble(dr[2]);
                        }

                        
                        string cstr = Controls.CurrencyString(c);
                        Estate.TransactionList.Add(new Estates {
                            
                            TransDate = Controls.DateString(TDate),
                            TransDescription = dr[1].ToString(),
                            SCCost = c,
                            SCCoststr = cstr,
                        SCHeading = dr[3].ToString()});

                    }

                   
                }

            }
            return Estate;

        }

        public static Estates GetBVA(Estates Estate)
        {
            //get budegt
            //string q = "select core.ServiceChargeBudgetDetail.ScheduleName, core.ServiceChargeBudgetDetail.Heading, " +
            //            "core.ServiceChargeBudgetDetail.Cost, core.ServiceChargeBudgetDetail.BudgetID from core.ServiceChargeBudgetDetail " +
            //            "inner join core.ServiceChargeBudgets on core.ServiceChargeBudgets.BudgetID = core.ServiceChargeBudgetDetail.BudgetID " +
            //            "where core.ServiceChargeBudgets.EstateID = " + Estate.EstatedID + " and core.ServiceChargeBudgets.startDate = " +
            //            "'04/01/2017' and core.ServiceChargeBudgets.endDate = '03/31/2018'";

            string q = "select core.ServiceChargeBudgetDetail.ScheduleName, core.ServiceChargeBudgetDetail.Heading, " +
                        "core.ServiceChargeBudgetDetail.Cost, core.ServiceChargeBudgetDetail.BudgetID, core.ServiceChargeBudgets.startDate, " +
                        "core.ServiceChargeBudgets.endDate, core.ServiceChargeBudgetDetail.ID from core.ServiceChargeBudgetDetail " +
                        "inner join core.ServiceChargeBudgets on core.ServiceChargeBudgets.ID = core.ServiceChargeBudgetDetail.BudgetID " +
                        "where core.ServiceChargeBudgets.ID = " + Estate.BudgetId.ToString() + " and core.ServiceChargeBudgets._status = 'Completed'" +
                        " and not core.ServiceChargeBudgetDetail.Heading = 'Roundings' and not core.servicechargebudgets.FundTypeID = 4";

            dbConnection db = new dbConnection();
            DataTable BudgetDT = db.GetDataTable(GlobalVariables.CS, q);
            DateTime scStart = new DateTime();
            DateTime scEnd = new DateTime();

            Estate.BVAList = new List<Estates>();


            if (BudgetDT.Rows.Count > 0 && BudgetDT.Rows[0][0].ToString() != "Error")
            {
                DataRow dr = BudgetDT.Rows[0];
                Estate.BudgetId = Convert.ToInt32(dr[3]);

                
                DateTime.TryParse(dr[4].ToString(), out scStart);
                Estate.SCStartDate = scStart.ToShortDateString();

                DateTime.TryParse(dr[5].ToString(), out scEnd);
                Estate.SCEndDate = scEnd.ToShortDateString();

               int hid = Convert.ToInt32(dr[6]);

                foreach (DataRow d in BudgetDT.Rows)
                {
                    //Cost
                    double c = 0;
                    if (d[2] != DBNull.Value)
                    {
                        c = Convert.ToDouble(d[2]);
                    }
                    string cstr = Controls.CurrencyString(c);
                    Estate.BVAList.Add(new Estates
                    {
                        SCScheduleName = d[0].ToString(),
                        SCHeading = d[1].ToString(),
                        SCCost = c,
                        SCCoststr = cstr,
                        //HeadingID = hid
                    });
                }
                dr = null;
            }
            
            
            BudgetDT = null;


            //Expenditure

            q = "select core.Transactions.budgetSchedule, core.ServiceChargeBudgetDetail.Heading, core.Transactions.TransAmount " +
                "from core.Transactions " +
                "inner join core.ServiceChargeBudgetDetail on core.ServiceChargeBudgetDetail.id = " +
                " core.Transactions.HeadingID where core.transactions.budgetID = " + Estate.BudgetId.ToString() +
                "  and Core.Transactions.inError is null or core.Transactions.inError = 0";

            //q = "select core.Transactions.budgetSchedule, core.ServiceChargeBudgetDetail.Heading, core.Transactions.TransAmount " +
            //    "from core.Transactions where core.transactions.budgetID = " + Estate.BudgetId.ToString() +
            //    " and Core.Transactions.inError is null or core.Transactions.inError = 0";

            DataTable ExpDT = db.GetDataTable(GlobalVariables.CS, q);

            if (ExpDT.Rows.Count > 0)
            {

                DataRow dr1 = ExpDT.Rows[0];


                if (dr1[0].ToString() != "Error")
                {
                    string sched = "";
                    string head = "";
                    Estate.Schedules = new List<string>();
                    foreach (Estates es in Estate.BVAList)
                    {
                        sched = es.SCScheduleName;
                        head = es.SCHeading;
                        //Sets the schedule list used by the view to separate the tables
                        if (Estate.Schedules.Count == 0)
                        {
                            Estate.Schedules.Add(sched);
                        }
                        else
                        {
                            bool found = false;
                            foreach (string s in Estate.Schedules)
                            {

                                if (s == sched)
                                {
                                    found = true;
                                }
                            }
                            if (found == false)
                            {
                                Estate.Schedules.Add(sched);

                            }
                            else
                            {
                                found = false;
                            }
                        }

                        es.TrCost = 0;
                        foreach (DataRow dr2 in ExpDT.Rows)
                        {
                            if (dr2[0].ToString() == sched && dr2[1].ToString() == head)
                            {

                                if (dr2[2] != DBNull.Value)
                                {
                                    es.TrCost = es.TrCost + Convert.ToDouble(dr2[2]);
                                    es.TrCoststr = Controls.CurrencyString(es.TrCost);

                                }
                            }
                        }

                    }
                }
            }
            //totals
            
            foreach(Estates es in Estate.BVAList)
            {
                if(es.SCCost == 0)
                {
                    es.SCCoststr = "£0.00";
                }

                if (es.TrCost == 0)
                {
                    es.TrCoststr = "£0.00";
                }

                es.BVADiff = es.SCCost - es.TrCost;
                es.BVADiffstr = Controls.CurrencyString(es.BVADiff);
                 

                //Estate.TotalBudget = Estate.TotalBudget + es.SCCost;
                //Estate.TotalBudgetstr = Estate.TotalBudget.ToString("C", CultureInfo.CurrentCulture);

                //Estate.TotalExp = Estate.TotalExp + es.TrCost;
                //Estate.TotalExpstr = Estate.TotalExp.ToString("C", CultureInfo.CurrentCulture);

                //Estate.TotalDiff = Estate.TotalBudget - Estate.TotalExp;
                //Estate.TotalDiffstr = Estate.TotalDiff.ToString("C", CultureInfo.CurrentCulture);
            }


                return Estate;
        }

        public static Estates GetExpenditurebyPO(int ponumber)
        {
            //each db call is nested so only executed if previous call does not return an error. 

            //first get budetid, schedule name and heading
            string q = "select core.PurchaseOrders.BudgetID, core.PurchaseOrders.scheduleName, " +
                       "core.PurchaseOrders.Heading, core.purchaseorders.headingID from core.PurchaseOrders " +
                        "where core.PurchaseOrders.id = " + ponumber.ToString();

            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);
           
            Estates ReturnEst = new Estates();
            if (dt.Rows.Count > 0)
            {
                DataRow d = dt.Rows[0];
                if (d[0].ToString() != "Error")
                {
                    ReturnEst.BudgetId = Convert.ToInt32(d[0]);
                    ReturnEst.SCScheduleName = d[1].ToString();
                    ReturnEst.SCHeading = d[2].ToString();
                    ReturnEst.HeadingID = Convert.ToInt32(d[3]);


                    //Get expenditure details based on the above
                    q = "select sum(core.Transactions.TransAmount) from core.Transactions where " +
                        "budgetID = " + ReturnEst.BudgetId + " and budgetSchedule = '" +
                        ReturnEst.SCScheduleName + "' and HeadingID = " + ReturnEst.HeadingID;

                    DataTable dt1 = db.GetDataTable(GlobalVariables.CS, q);
                    if (dt1.Rows.Count > 0)
                    {
                        DataRow d1 = dt1.Rows[0];
                        if (d1[0] == DBNull.Value || d1[0].ToString() == "Error")
                        {
                            ReturnEst.TotalExp = 0;
                                ReturnEst.TotalExpstr = Controls.CurrencyString(ReturnEst.TotalExp);
                            
                          

                        }
                        else
                        {
                              ReturnEst.TotalExp = Convert.ToDouble(d1[0]);
                                ReturnEst.TotalExpstr = Controls.CurrencyString(ReturnEst.TotalExp);
                                 

                           
                        }
                        d1 = null;
                        dt1 = null;


                        //gets budgeted amount for scehdule and heading
                        q = "select sum(core.ServiceChargeBudgetDetail.Cost) from core.ServiceChargeBudgetDetail " +
                            "where BudgetID = " + ReturnEst.BudgetId + " and ScheduleName = '" + ReturnEst.SCScheduleName +
                            "' and Heading = '" + ReturnEst.SCHeading + "'";

                        DataTable dt2 = db.GetDataTable(GlobalVariables.CS, q);

                        if (dt2.Rows.Count > 0)
                        {
                            DataRow d2 = dt2.Rows[0];
                            if (d2[0] == DBNull.Value || d2[0].ToString() == "Error")
                            {
                                
                                    ReturnEst.TotalBudget = 0;
                                    ReturnEst.TotalBudgetstr = Controls.CurrencyString(ReturnEst.TotalBudget);
                                       
                                

                            } else
                            {
                               
                                    ReturnEst.TotalBudget = Convert.ToDouble(d2[0]);
                                    ReturnEst.TotalBudgetstr = Controls.CurrencyString(ReturnEst.TotalBudget);


                            }
                                    d2 = null;
                                }
                                dt2 = null;
                            }
                        }
                      
                    }
                    return ReturnEst;  
         }
    }
}