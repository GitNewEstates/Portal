using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_MVC.Models
{
    public class DirectorBase
    {
        public ServiceChargeBudgetViewModel PropListViewModel { get; set; }
        public IEnumerable<Models.Properties> PropertyList { get; set; }
        public Estates Estate { get; set; }
        public string ViewName { get; set; }
        public string ControllerName { get; set; }
        public BankAccount SelectedBankAccount { get; set; }
        public List<BankAccount> BankAccountList { get; set; }

    }

   

    public class DirectorSummaryViewModel : DirectorBase
    {
        
        public List<ServiceCharges> AwaitingPaymentList { get; set; }
        public List<BankAccount> BankAccountBalanceList { get; set; }
        public List<ServiceCharges> BudgetList { get; set; }
        public ServiceCharges DefaulBudget { get; set; }
        public UnitBalancesViewModel UnitBalancesVM { get; set; }

    }

    public class UnitBalancesViewModel : DirectorBase
    {
        public List<ServiceCharges> BalancesList { get; set; }
        public double TotalBalances { get; set; }
        public string TotalBalancesStr { get; set; }
    }

    public class DirectorBudgetSummaryViewModel
    {
        public ServiceCharges BudgetSummary { get; set; }
    }

    public class DirectorBankAccountViewModel : DirectorBase
    {
        public List<ServiceCharges> TransactionList { get; set; }
        
        public List<Estates> AccountTransactionList { get; set; }
    }
}