using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using dbConn;
using Portal_MVC.Models;
using System.Globalization;

namespace Portal_MVC.Models
{
    public class ServiceCharges
    {
        public int ID { get; set; }
        public double TransAmount { get; set; }
        public string TransAmountstr { get; set; }
        public DateTime? TransDate { get; set; }
        public string TransDatestr { get; set; }
        public string TransDescription { get; set; }
        public string TransType { get; set; }
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
            string q = "select * from core.Transactions inner join core.TransactionTypes " +
                        "on core.Transactions.TransactionTypeID = core.TransactionTypes.id " +
                        "where core.Transactions.unitID = " + unitID.ToString() + " and core.Transactions.inError is null " +
                        "order by core.Transactions.TransDate asc";

            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);
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
                        if (d[1] != DBNull.Value)
                        {
                            date = Convert.ToDateTime(d[1]);
                        }

                        //trans amount
                        double am = 0;
                        if (d[7] != DBNull.Value)
                        {
                            am = Convert.ToDouble(d[7]);
                        }

                        int id = Convert.ToInt32(d[0]);
                        

                        rlist.Add(new ServiceCharges
                        {
                            ID = id,
                            TransDate = date,
                            TransDatestr = Controls.DateString(date),
                            TransDescription = d[5].ToString(),
                            TransAmount = am,
                            TransAmountstr = Controls.CurrencyString(am),
                            TransType = d[13].ToString(),
                            CrDr = d[14].ToString(),
                            //Balance = bal,
                            //BalanceStr = Controls.CurrencyString(bal)

                        });

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


                bal = 0;
                double startBal = 0;


                for (int i = 0; i <= rlist.Count - 1; i++)
                {
                    if (rlist.Count == 1)
                    {
                        rlist[i].Balance = startBal;
                        rlist[i].BalanceStr = Controls.CurrencyString(startBal);

                    }
                    else
                    {
                        
                        if (rlist[i].CrDr == "Cr")
                        {
                            bal -= rlist[i].TransAmount;
                            rlist[i].Balance = bal;
                            rlist[i].BalanceStr = Controls.CurrencyString(bal);
                        }
                        else
                        {
                            bal += rlist[i].TransAmount;
                            rlist[i].Balance = bal;
                            rlist[i].BalanceStr = Controls.CurrencyString(bal);
                        }


                    }
                }



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

            dbConnection db = new dbConnection();
            DataTable dataTable = db.GetDataTable(GlobalVariables.CS, PaidDateStr);

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

            dbConn.dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

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

            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

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
            dbConnection db = new dbConnection();
            List<string> p = new List<string>();
            p.Add("@budgetID");

            List<object> o = new List<object>();
            o.Add(BudgetID);

            ServiceCharges r = new ServiceCharges();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, "dbo.spGetBudgetSummary", p, o);

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

            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, "dbo.spGetUnitBalances", p, o);

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
}