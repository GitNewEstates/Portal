using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using dbConn;
using Portal_MVC.Models;
using System.Globalization;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Portal_MVC.Models
{

    public class Fund : BaseClass
    {

        public string FundName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int EstateID { get; set; }

        public FundStatus FundStatus { get; set; }
        public string DocInstanceID { get; set; }
    }
    public enum FundStatus
    {
        None,
        Progress, //Budget being caluclated
        Completed, //Budget Completed and expenditure being recored
        YearEnded, //Year Ended and Accounts being prepared
        Reconciled, //Year Ended and Accounts Done
    }

    public static class FundMethods
    {
        public async static Task<List<Fund>> GetFundList(int EstateID)
        {
           string json = await 
                GlobalVariables.APIConnection.CallAPIGetEndPointAsync($"FundList/{EstateID}");

            return DeserializedJSONToSupplierList(json);
        }

        public static List<Fund> DeserializedJSONToSupplierList(string json = "")
        {
            List<Fund> obj = new List<Fund>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<Fund>>(json);
                }
                catch (Exception ex)
                {
                    Fund fund = new Fund();
                    fund.APIError.errorType = ErrorType.APIValidationError;
                    fund.APIError.HasError = true;
                    fund.APIError.Message = $"Error Deserializing JSON to Fund List. Error: {ex.Message}";
                    obj.Add(fund);
                }
            }

            return obj;
        }
    }


    public class ServiceCharges
    {
        public int ID { get; set; }
        public double TransAmount { get; set; }
        public string TransAmountstr { get; set; }
        public DateTime? TransDate { get; set; }
        public string TransDatestr { get; set; }
        public string TransDescription { get; set; }
        public string TransType { get; set; }
        public int TransTypeID { get; set; }
        public string CrDr { get; set; }
        public List<ServiceCharges> AllTrans { get; set; }
        public double Balance { get; set; }
        public string BalanceStr { get; set; }
        public string SupplierName { get; set; }
        public string BudgetName { get; set; }
        public int BudgetID { get; set; }

        public double TotalCharges { get; set; }
        public string TotalChargesStr { get; set; }

        public double FutureCharges { get; set; }
        public string FutureChargesStr { get; set; }

        public double TotalBudget { get; set; }
        public string TotalBudgetStr { get; set; }

        public double TotalExpenditure{ get; set; }
        public string TotalExpenditureStr { get; set; }

        public double TotalArrears { get; set; }
        public string TotalArrearsStr { get; set; }

        public double TotalPayments { get; set; }
        public string TotalPaymentsStr{ get; set; }

        public double CommittedFunds { get; set; }
        public string CommittedFundsStr { get; set; }
    }

    public static class ServiceChargeMethods
    {
        public static List<ServiceCharges> AllTransactions(int unitID)
        {
            //string q = "select * from core.Transactions inner join core.TransactionTypes " +
            //            "on core.Transactions.TransactionTypeID = core.TransactionTypes.id " +
            //            "where core.Transactions.unitID = " + unitID.ToString() + " and core.Transactions.inError is null " +
            //            "order by core.Transactions.TransDate asc";

           DBConnectionObject db = GlobalVariables.GetConnection();
          //  DataTable dt = db.Connection.GetDataTable( q);
            List<string> P = new List<string>();
            P.Add("@unitId");

            List<object> o = new List<object>();
            o.Add(unitID);


            DataTable dt =
                db.Connection.GetDataTable("spGetUnitServiceChargeData", P, o);
            List<ServiceCharges> rlist = new List<ServiceCharges>();

            if(dt.Rows.Count > 0)
            {
                double bal = 0;
                foreach (DataRow d in dt.Rows)
                {

                    if (d[0] != DBNull.Value && d[0].ToString() != "Error")
                    {
                        //date
                        DateTime date = new DateTime();
                        if (d[2] != DBNull.Value)
                        {
                            date = Convert.ToDateTime(d[2]);
                        }

                        //trans amount
                        double am = 0;
                        if (d[8] != DBNull.Value)
                        {
                            am = Convert.ToDouble(d[8]);
                        }

                        int id = Convert.ToInt32(d[1]);
                        int TransactionTypeId = Convert.ToInt32(d[7]);
                        string transtype = "";
                        if(TransactionTypeId == 1)
                        {
                            transtype = "Payment";
                            bal -= am;

                        } else if(TransactionTypeId == 2)
                        {
                            transtype = "Charge";
                            bal += am;
                        }

                        rlist.Add(new ServiceCharges
                        {
                            ID = id,
                            TransDate = date,
                            TransDatestr = Controls.DateString(date),
                            TransDescription = d[6].ToString(),
                            TransAmount = am,
                            TransAmountstr = Controls.CurrencyString(am),
                            TransType = transtype, 
                            TransTypeID = TransactionTypeId,
                            //CrDr = d[14].ToString(),
                            Balance = bal,
                            BalanceStr = Controls.CurrencyString(bal)

                        }) ;

                    }
                }

                //bal = rlist[rlist.Count - 1].TransAmount;
                //double startBal = rlist[rlist.Count - 1].TransAmount;


                //for (int i = 0;  i <= rlist.Count -1 ; i++)
                //{
                //    if(i == rlist.Count - 1)
                //    {
                //        rlist[i].Balance = startBal;
                //        rlist[i].BalanceStr = Controls.CurrencyString(startBal);

                //    } else
                //    {
                //        if(rlist[rlist.Count - (i + 2)].CrDr == "Cr")
                //        {
                //            bal -= rlist[rlist.Count - (i + 2)].TransAmount;
                //            rlist[rlist.Count - (i + 2)].Balance = bal;
                //            rlist[rlist.Count - (i + 2)].BalanceStr = Controls.CurrencyString(bal);
                //        } else
                //        {
                //            bal += rlist[rlist.Count - (i + 2)].TransAmount;
                //            rlist[rlist.Count - (i + 2)].Balance = bal;
                //            rlist[rlist.Count - (i+2)].BalanceStr = Controls.CurrencyString(bal);
                //        }
                        
                        
                //    }
                //}


                //bal = 0;
                //double startBal = 0;


                //for (int i = 0; i <= rlist.Count - 1; i++)
                //{
                //    if (rlist.Count == 1)
                //    {
                //        rlist[i].Balance = startBal;
                //        rlist[i].BalanceStr = Controls.CurrencyString(startBal);

                //    }
                //    else
                //    {
                        
                //        if (rlist[i].CrDr == "Cr")
                //        {
                //            bal -= rlist[i].TransAmount;
                //            rlist[i].Balance = bal;
                //            rlist[i].BalanceStr = Controls.CurrencyString(bal);
                //        }
                //        else
                //        {
                //            bal += rlist[i].TransAmount;
                //            rlist[i].Balance = bal;
                //            rlist[i].BalanceStr = Controls.CurrencyString(bal);
                //        }


                //    }
                //}



            }
            return rlist;
        }

        public static List<ServiceCharges> ReplaceTransactionDateWithPaidDate(List<ServiceCharges> TransList)
        {
            //replaces the transaction date with the paid date for transactions that are payments
            //to reduce db calls loop creats the query string

            string PaidDateStr = "select transactionID, paiddate from core.paymentdata where transactionID = ";

            int c = 0;
            for (int i = 0; i <= TransList.Count - 1; i++)
            {
                if (TransList[i].TransType == "Payment")
                {

                    if (c == 0)
                    {
                        PaidDateStr += TransList[i].ID.ToString();
                    }
                    else
                    {
                        PaidDateStr += " or transactionid = " + TransList[i].ID.ToString();
                    }

                    c += 1;
                }
            }

            DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dataTable = db.Connection.GetDataTable( PaidDateStr);

            if (dataTable.Rows.Count > 0 && dataTable.Rows[0][0].ToString() != "Error")
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    //get trans ID
                    
                    int id = Convert.ToInt32(row[0]);
                    foreach (ServiceCharges tr in TransList)
                    {
                        if (tr.ID == id)
                        {
                            DateTime _date = Convert.ToDateTime(row[1]); 

                            tr.TransDate = _date;
                            tr.TransDatestr = Controls.DateString(_date);

                            break;
                        }
                    }

                }
            }



            return TransList;
        }

        public static List<ServiceCharges> ListOfUnpaidInvoices(int EstateID)
        {
            string q = "select core.transactions.id, core.transactions.TransDesc, core.transactions.TransAmount, core.Contractors.Name from core.transactions " +
                        "inner join core.SupplierPayments on core.Transactions.id = core.SupplierPayments.TransID " +
                        "inner join core.Contractors on core.SupplierPayments.supplierID = core.Contractors.id " +
                        "inner join core.ServiceChargeBudgets on core.Transactions.budgetID = core.ServiceChargeBudgets.ID " +
                        "where core.ServiceChargeBudgets.EstateID = " + EstateID.ToString() + " and core.SupplierPayments.paidDate is null";

            dbConn.DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);

            List<ServiceCharges> r = new List<ServiceCharges>();

            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach(DataRow dr in dt.Rows)
                {
                    int tID = 0;
                    int.TryParse(dr[0].ToString(), out tID);

                    double amount = 0;
                    double.TryParse(dr[2].ToString(), out amount);


                    r.Add(new ServiceCharges
                    {
                        ID = tID,
                        TransDescription = dr[1].ToString(),
                        TransAmount = amount,
                        TransAmountstr = Controls.CurrencyString(amount),
                        SupplierName = dr[3].ToString()
                    });
                }
            }
            return r;

        }

        public static List<ServiceCharges> BudgetListByEstate(int EstateID)
        {
            string q = "select id, budgetname from core.ServiceChargeBudgets " +
                "where EstateID = " + EstateID.ToString() + 
                " and _status = 'Completed' and not fundtypeid = 4 ";

           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);

            List<ServiceCharges> r = new List<ServiceCharges>();

            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach(DataRow dr in dt.Rows)
                {
                    int Bid = 0;
                    int.TryParse(dr[0].ToString(), out Bid);
                    r.Add(new ServiceCharges
                    {
                       BudgetID = Bid,
                       BudgetName = dr[1].ToString()
                    });
                }
            }
            return r;
        }

        public static ServiceCharges GetBudgetSummary(int BudgetID)
        {
           DBConnectionObject db = GlobalVariables.GetConnection();
            List<string> p = new List<string>();
            p.Add("@budgetID");

            List<object> o = new List<object>();
            o.Add(BudgetID);

            ServiceCharges r = new ServiceCharges();
            DataTable dt = db.Connection.GetDataTable( "dbo.spGetBudgetSummary", p, o);

            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                //total payments
                //total expenditure
                //Total Budget
                //Total Charges
                //Totals Arrears
                //Committed Funds

                foreach(DataRow dr in dt.Rows)
                {
                    double TotalPayments = 0;
                    double TotalExpenditure = 0;
                    double TotalBudget = 0;
                    double TotalCharges = 0;
                    double TotalArrears = 0;
                    double CommittedFunds = 0;
                    double FutureCharges = 0;

                    double.TryParse(dr[0].ToString(), out TotalPayments);
                    double.TryParse(dr[1].ToString(), out TotalExpenditure);
                    double.TryParse(dr[2].ToString(), out TotalBudget);
                    double.TryParse(dr[3].ToString(), out TotalCharges);
                    double.TryParse(dr[4].ToString(), out TotalArrears);
                    double.TryParse(dr[5].ToString(), out CommittedFunds);
                    double.TryParse(dr[6].ToString(), out FutureCharges);

                    r.TotalPayments = TotalPayments;
                    r.TotalPaymentsStr = Controls.CurrencyString(TotalPayments);

                    r.TotalExpenditure = TotalExpenditure;
                    r.TotalExpenditureStr = Controls.CurrencyString(TotalExpenditure);

                    r.TotalBudget = TotalBudget;
                    r.TotalBudgetStr = Controls.CurrencyString(TotalBudget);

                    r.TotalCharges = TotalCharges;
                    r.TotalChargesStr = Controls.CurrencyString(TotalCharges);

                    r.TotalArrears = TotalArrears;
                    r.TotalArrearsStr = Controls.CurrencyString(TotalArrears);

                    r.FutureCharges = FutureCharges;
                    r.FutureChargesStr = Controls.CurrencyString(FutureCharges);

                    r.CommittedFunds = CommittedFunds;
                    r.CommittedFundsStr = Controls.CurrencyString(CommittedFunds);
                }
            }

            return r;
        }

        public static List<ServiceCharges> GetUnitBalances(int EstateID)
        {
            List<string> p = new List<string>();
            p.Add("@EstateID");

            List<object> o = new List<object>();
            o.Add(EstateID);

           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( "dbo.spGetUnitBalances", p, o);

            List<ServiceCharges> r = new List<ServiceCharges>();

            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach(DataRow dr in dt.Rows)
                {
                    int unitid = 0;
                    int.TryParse(dr[0].ToString(), out unitid);

                    double bal = 0;
                    double.TryParse(dr[2].ToString(), out bal);

                    r.Add(new ServiceCharges
                    {
                        ID = unitid,
                        TransDescription = dr[1].ToString(),
                        Balance = bal,
                        BalanceStr = Controls.CurrencyString(bal)
                    });
                }
            }

            return r;
        }
    }
    public class ServiceChargeBudget : BaseClass
    {
        public string BudgetName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ChargePeriod { get; set; }

        public int FundID { get; set; }

        public FundStatus FundStatus { get; set; }

        public string BudgetStatus { get; set; }
        public int FundTypeID { get; set; }
        public int PostingTypeID { get; set; }
    }

    public static class ServiceChargeBudgetMethods
    {
        public async static Task<List<ServiceChargeBudget>> GetBudgetList(int FundID)
        {
            string json =
                await GlobalVariables.APIConnection.CallAPIGetEndPointAsync($"BudgetList/{FundID}");


             return DeserializedJSONToBudgetList(json);




        }

        public static string JsonSerialize(List<ServiceChargeBudget> BudgetList)
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(BudgetList);
        }
        public static List<ServiceChargeBudget> DeserializedJSONToBudgetList(string json = "")
        {
            List<ServiceChargeBudget> obj = new List<ServiceChargeBudget>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<ServiceChargeBudget>>(json);
                }
                catch (Exception ex)
                {
                    ServiceChargeBudget budget = new ServiceChargeBudget();
                    budget.APIError.errorType = ErrorType.APIValidationError;
                    budget.APIError.HasError = true;
                    budget.APIError.Message = $"Error Deserializing JSON to Budget List. Error: {ex.Message}";
                    obj.Add(budget);
                }
            }

            return obj;
        }
    }

    public class BudgetSchedule : BaseClass
    {
        

        private string _Schedule;
        public string Schedule
        {
            get { return _Schedule; }
            set
            {
                _Schedule = value;
                
            }
        }
    }

    public static class BudgetScheduleMethods
    {
        public async static Task<string> GetScheduleListAPIJSON(int budgetid)
        {
            return await GlobalVariables.APIConnection.CallAPIGetEndPointAsync($"BudgetScheduleList/{budgetid}");
        }
        public async static Task<List<BudgetSchedule>> GetScheduleListAPI(int budgetid)
        {
            string json = await GlobalVariables.APIConnection.CallAPIGetEndPointAsync($"BudgetScheduleList/{budgetid}");

            return DeserializedJSONToScheduleList(json);
        }

        public static List<BudgetSchedule> DeserializedJSONToScheduleList(string json = "")
        {
            List<BudgetSchedule> obj = new List<BudgetSchedule>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<BudgetSchedule>>(json);
                }
                catch (Exception ex)
                {
                    BudgetSchedule schedule = new BudgetSchedule();
                    schedule.APIError.errorType = ErrorType.APIValidationError;
                    schedule.APIError.HasError = true;
                    schedule.APIError.Message = $"Error Deserializing JSON to Schedule List. Error: {ex.Message}";
                    obj.Add(schedule);
                }
            }

            return obj;
        }
    }
}