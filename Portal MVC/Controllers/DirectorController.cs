using Portal_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal_MVC.Controllers
{
    public class DirectorController : Controller
    {
        // GET: Director
        public ActionResult DirectorSummary(int AccountID = 0)
        {

            Models.DirectorSummaryViewModel vm = new DirectorSummaryViewModel();
            //invoices avaiting payment
            //if(AccountID == 0)
            //{
            //    vm.Estate = GetEstate();
            //    vm.BankAccountList = Models.BankAccountMethods.BankAccountListByEstate(vm.Estate.EstatedID);
            //    vm.ViewName = "DirectorSummary";
            //    vm.ControllerName = "Director";
            //} else
            //{
                vm.Estate = GetEstate();
                vm.BankAccountBalanceList = Models.BankAccountMethods.BankAccountAndBalanceList(vm.Estate.EstatedID);
                vm.AwaitingPaymentList = Models.ServiceChargeMethods.ListOfUnpaidInvoices(vm.Estate.EstatedID);
                vm.BudgetList = Models.ServiceChargeMethods.BudgetListByEstate(vm.Estate.EstatedID);
                vm.UnitBalancesVM = new UnitBalancesViewModel();
                vm.UnitBalancesVM.BalancesList = ServiceChargeMethods.GetUnitBalances(vm.Estate.EstatedID);

                //set default budget - which will be the one the summary defaults to viewing
                if(vm.BudgetList.Count > 0)
                {
                    vm.DefaulBudget = vm.BudgetList[0];
                }
            //}

            //invoices awaiting authorisation


            return View("DirectorSummary", vm);
        }

     

        public PartialViewResult DisplayBudgetData(int BudgetID = 0)
        {
            Models.DirectorBudgetSummaryViewModel vm = new DirectorBudgetSummaryViewModel();
            vm.BudgetSummary = Models.ServiceChargeMethods.GetBudgetSummary(BudgetID);
            return PartialView("_BudgetSummary", vm);
        }

        // GET: Director
        public ActionResult DirectorBankAccount(int AccountID = 0)
        {
            Models.DirectorBankAccountViewModel vm = new DirectorBankAccountViewModel();
            if (AccountID == 0)
            {
               
                vm.Estate = GetEstate();
                vm.BankAccountList = Models.BankAccountMethods.BankAccountListByEstate(vm.Estate.EstatedID);
                vm.ViewName = "DirectorBankAccount";
                vm.ControllerName = "Director";
            } else
            {
                //get bank account object
                vm.SelectedBankAccount = Models.BankAccountMethods.GetBankAccountObject(AccountID);
                vm.TransactionList = Models.BankAccountMethods.GetAllBankTransactions(AccountID);

                foreach(ServiceCharges s in vm.TransactionList)
                {
                    if(s.CrDr == "Cr")
                    {
                        vm.SelectedBankAccount.TotalMoneyIn += s.TransAmount;

                    } else if(s.CrDr == "Dr")
                    {
                        vm.SelectedBankAccount.TotalMoneyOut += s.TransAmount;
                    }
                }

                vm.SelectedBankAccount.TotalMoneyInStr = Controls.CurrencyString(vm.SelectedBankAccount.TotalMoneyIn);
                vm.SelectedBankAccount.TotalMoneyOutStr = Controls.CurrencyString(vm.SelectedBankAccount.TotalMoneyOut);

                vm.SelectedBankAccount.TotalBalance = vm.SelectedBankAccount.TotalMoneyIn - vm.SelectedBankAccount.TotalMoneyOut;

                vm.SelectedBankAccount.TotalBalanceStr = Controls.CurrencyString(vm.SelectedBankAccount.TotalBalance);
            }

            return View("BankAccount", vm);
        }

        public ActionResult UnitBalances()
        {
            UnitBalancesViewModel vm = new UnitBalancesViewModel();
            vm.Estate = GetEstate();
            vm.BalancesList = ServiceChargeMethods.GetUnitBalances(vm.Estate.EstatedID);

            if(vm.BalancesList != null)
            {
                foreach(ServiceCharges s in vm.BalancesList)
                {
                    if(s.Balance > 0)
                    {
                        vm.TotalBalances += s.Balance;
                    }
                }
            }

            vm.TotalBalancesStr = Controls.CurrencyString(vm.TotalBalances);

            return View("UnitBalances", vm); 
        }
        private Estates GetEstate()
        {
            Estates es = new Estates();
            
                
                try
                {
                    es = EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                }
                catch { }

                Session["EstateName"] = es.EstateName;
            

            return es;
        }


        private void SetEstateName()
        {
            if (Session["EstateName"] == null)
            {
                Estates es = new Estates();
                try
                {
                    es = EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                }
                catch { }

                Session["EstateName"] = es.EstateName;
            }
        }
    }

 
}