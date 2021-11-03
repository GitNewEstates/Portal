using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Portal_MVC.Models
{
    public class BankAccount
    {
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public int ID { get; set; }
        public double TotalMoneyIn { get; set; }
        public double TotalMoneyOut { get; set; }
        public double TotalBalance { get; set; }
        public string TotalMoneyInStr { get; set; }
        public string TotalMoneyOutStr { get; set; }
        public string TotalBalanceStr { get; set; }

    }

    public static class BankAccountMethods
    {
        public static List<BankAccount> BankAccountListByEstate(int EstateID)
        {
            List<BankAccount> r = new List<BankAccount>();

            string q = "select distinct core.EstateBankAccounts.id, core.EstateBankAccounts.AccountName, core.EstateBankAccounts.AccountNumber, core.EstateBankAccounts.SortCode from core.EstateBankAccounts " +
                        "inner join core.EstateBudgetBankAccounts on core.EstateBankAccounts.id = core.EstateBudgetBankAccounts.AccountID " +
                        "inner join core.ServiceChargeBudgets on core.EstateBudgetBankAccounts.BudgetId = core.ServiceChargeBudgets.ID " +
                        "where core.ServiceChargeBudgets.EstateID = " + EstateID.ToString();

            dbConn.DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);

            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int id = 0;
                    int.TryParse(dr[0].ToString(), out id);


                    r.Add(new BankAccount
                    {
                        ID = id,
                        Name = dr[1].ToString(),
                        AccountNumber = dr[2].ToString(),
                        SortCode = dr[3].ToString()
                    });
                }
            }

            return r;
        }

        public static BankAccount GetBankAccountObject(int ID)
        {
            string q = "select id, AccountName, AccountNumber, SortCode from core.EstateBankAccounts where id = " + ID.ToString();

            dbConn.DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);

            BankAccount r = new BankAccount();
            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach(DataRow dr in dt.Rows)
                {
                    int _ID = 0;
                    int.TryParse(dr[0].ToString(), out _ID);
                    r.ID = _ID;
                    r.Name = dr[1].ToString();
                    r.AccountNumber = dr[2].ToString();
                    r.SortCode = dr[3].ToString();
                }
            }
            return r;
        }

        public static List<ServiceCharges> GetAllBankTransactions(int BankAccountID)
        {
            List<string> p = new List<string>();
            p.Add("@BankAccountID");
            List<object> o = new List<object>();
            o.Add(BankAccountID);

            dbConn.DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( "dbo.GetBankAccountTransactions", p, o);

            List<ServiceCharges> r = new List<ServiceCharges>();

            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                double TotalBalance = 0;


                foreach(DataRow dr in dt.Rows)
                {
                    int id = 0;
                    int.TryParse(dr[0].ToString(), out id);

                    DateTime TDate = new DateTime();
                    if (dr[2] != DBNull.Value)
                    {
                        DateTime.TryParse(dr[2].ToString(), out TDate);
                    }

                    //transaction cost
                    double c = 0;
                    if (dr[3] != DBNull.Value)
                    {
                        c = Convert.ToDouble(dr[3]);
                    }


                    //balance
                    if(dr[5].ToString() == "Cr")
                    {
                       
                        TotalBalance += c;

                    } else if(dr[5].ToString() == "Dr")
                    {
                        
                        TotalBalance -= c;
                    }

                    string cstr = Controls.CurrencyString(c);

                    r.Add(new ServiceCharges
                    {
                        ID = id,
                        TransDatestr = Controls.DateString(TDate),
                        TransDate = TDate,
                        TransDescription = dr[4].ToString(),
                        CrDr = dr[5].ToString(),
                        TransAmount = c,
                        TransAmountstr = cstr,
                        Balance = TotalBalance,
                        BalanceStr = Controls.CurrencyString(TotalBalance)

                });
                }
            }

            return r;
               
        }

        public static List<BankAccount> BankAccountAndBalanceList(int estateID)
        {
            List<string> p = new List<string>();
            p.Add("@EstateID");
            List<object> o = new List<object>();
            o.Add(estateID);

            dbConn.DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( "dbo.spEstateBankAccountsAndBalances", p, o);

            List<BankAccount> r = new List<BankAccount>();
            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach(DataRow dr in dt.Rows)
                {
                    int AccountID = 0;
                    int.TryParse(dr[1].ToString(), out AccountID);

                    double balance = 0;
                    double.TryParse(dr[5].ToString(), out balance);

                    r.Add(new BankAccount {
                        ID = AccountID,
                        Name = dr[2].ToString(),
                        SortCode = dr[3].ToString(),
                        AccountNumber = dr[4].ToString(),
                        TotalBalance = balance,
                        TotalBalanceStr = Controls.CurrencyString(balance)

                    });

                }
                
            }

            return r;
        }
    }
}