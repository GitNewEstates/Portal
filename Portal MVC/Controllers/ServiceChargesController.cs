using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Portal_MVC.Models;

namespace Portal_MVC.Controllers
{
    [Authorize(Roles = "Customer,Client")]
    public class ServiceChargesController : Controller
    {
        // GET: ServiceCharge


        public ActionResult Index(int PropID = 0, string PropName = "")
        {
            if (Session["CustomerID"] != null && (int)Session["CustomerID"] != 0)
            {
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel(ViewModelLevel.none);

                if (PropID > 0)
                {
                    Session["SelectedPropertyID"] = PropID;
                    Session["SelectedProperty"] = PropName;
                    Session["IsDirector"] = EstateDirectors.EstateDirectorMethods.IsCustomerDirector(GlobalVariables.GetConnection(), PropID).ToString();
                }

                if (Session["SelectedPropertyID"] == null || (int)Session["SelectedPropertyID"] == 0)
                {
                    //Get Property List

                    mv.PropertyList = Models.PropertyMethods.GetAllOwnedProperties((int)Session["CustomerID"]);
                    mv.ViewName = "Index";
                    mv.ControllerName = "ServiceCharges";

                  
                }
                else
                {
                   

                    mv.Estate = Models.EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                    Session["EstateName"] = mv.Estate.EstateName;
                    mv.Estate.BudgetList = Models.EstateMethods.GetServiceChargePeriodList(mv.Estate.EstatedID);
                    mv.ViewName = "ServiceChargeBudget";
                    mv.ControllerName = "ServiceCharges";

                    
                }

                return View("SelectBudget", mv);
            } else
            {
                //return not logged in view
                return View("../Home/NotLoggedIn");
            }

             
            
        }

        public async Task<ActionResult> ServiceCharge_Dashboard()
        {
            ViewModelBase vm = new ViewModelBase(ViewModelLevel.Estate);
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();
            await vm.SetBaseDataAsync(id, email);

            return View(vm);
        }

        public async Task<ActionResult> ServiceChargeExpenditure(string PropID = "", string PropName = "")
        {
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();

            Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel(ViewModelLevel.Estate);
            await mv.SetBaseDataAsync(id, email);

            if (!string.IsNullOrWhiteSpace(PropID))
            {
                mv.SelectedEstateName = PropName;
                mv.SelectedEstateID = PropID;

            }
            else
            {
                mv.ViewName = "ServiceChargeExpenditure";
                mv.ControllerName = "ServiceCharges";
            }

            //if (Session["SelectedPropertyID"] == null || (int)Session["SelectedPropertyID"] == 0 )
            //{
            //    //Get Property List

            //    mv.PropertyList = Models.PropertyMethods.GetAllOwnedProperties((int)Session["CustomerID"]);
             
            //}
            //else
            //{
            //    mv.Estate = Models.EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
            //    mv.Estate = Models.EstateMethods.GetServiceChargeExpenditure(mv.Estate);
            //}

            return View("ServiceChargeExpenditure", mv);
            
        }

        public async Task<ActionResult> ServiceChargeBudget(int BudgetID = 0)
        {
            if (Session["CustomerID"] != null)
            {
                var id = User.Identity.GetUserId();
                var email = User.Identity.GetUserName();

                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel(ViewModelLevel.Estate);
                await mv.SetBaseDataAsync(id, email);

               
                mv.Estate = new Estates();
                mv.Estate.BudgetId = BudgetID;
                mv.Estate = EstateMethods.GetServiceChargeBudget(mv.Estate, (int)Session["SelectedPropertyID"]);

                return View("Budget", mv);

            }
            else
            {
                return View("../Home/NotLoggedIn");
            }
        }

        public ActionResult AllExpenditure(int PropID = 0)
        {
            if (Session["CustomerID"] != null)
            {
                if (Session["SelectedPropertyID"] == null)
                {
                    Session["SelectedPropertyID"] = PropID;
                    Session["SelectedProperty"] = Models.PropertyMethods.PropertyAddress(PropID);
                }

                //Models.GlobalVariables.SelectedProperty = property.Address1;
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel(ViewModelLevel.none);
                mv.Estate = Models.EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                mv.Estate = Models.EstateMethods.GetServiceChargeExpenditure(mv.Estate);
                //mv.Estate.PieChartData = new System.Data.DataTable();
                //mv.Estate.PieChartData.Columns.Add("Heading");
                //mv.Estate.PieChartData.Columns.Add("Cost");
                return View("ServiceChargeExpenditure", mv);
            } else
            {
                return View("../Home/NotLoggedIn");
            }
        }


        public ActionResult IndexFromList(int PropID = 0)
        {
            if (Session["CustomerID"] != null)
            {
                if (Session["SelectedPropertyID"] == null)
                {
                    Session["SelectedPropertyID"] = PropID;
                    Session["SelectedProperty"] = Models.PropertyMethods.PropertyAddress(PropID);
                }

                //Models.GlobalVariables.SelectedProperty = property.Address1;
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel(ViewModelLevel.none);
                mv.Estate = Models.EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                mv.Estate.BudgetList = Models.EstateMethods.GetServiceChargePeriodList(mv.Estate.EstatedID);
                mv.ViewName = "ServiceChargeBudget";
                mv.ControllerName = "ServiceCharges";

                //mv.Estate = Models.EstateMethods.GetServiceChargeBudget(mv.Estate);
                //mv.Estate.PieChartData = new System.Data.DataTable();
                //mv.Estate.PieChartData.Columns.Add("Heading");
                //mv.Estate.PieChartData.Columns.Add("Cost");
                return View("SelectBudget", mv);
            }
            else
            {
                return View("../Home/NotLoggedIn");
            }
        }

        public ActionResult BVA(int PropID = 0, string PropName = "")
        {
            if (Session["CustomerID"] != null || (int)Session["CustomerID"] != 0)
            {
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel(ViewModelLevel.none);
                if ((int)Session["SelectedPropertyID"] == 0 && PropID > 0)
                {
                    Session["SelectedPropertyID"] = PropID;
                    Session["SelectedProperty"] = PropName;
                }

                if (Session["SelectedPropertyID"] == null || (int)Session["SelectedPropertyID"] == 0)
              
                {
                    //Get Property List

                    mv.PropertyList = Models.PropertyMethods.GetAllOwnedProperties((int)Session["CustomerID"]);
                    mv.ViewName = "BVA";
                    mv.ControllerName = "ServiceCharges";
                }
                else
                {
                    if(mv.Estate == null)
                    {
                        mv.Estate = new Estates();
                    }
                    mv.Estate = Models.EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                    mv.Estate.BudgetList = Models.EstateMethods.GetServiceChargePeriodList(mv.Estate.EstatedID);
                    mv.ViewName = "BVAComparison";
                    mv.ControllerName = "ServiceCharges";
                    // mv.Estate.SCEndDate = "31/03/2018";
                    //mv.Estate = Models.EstateMethods.GetBVA(mv.Estate);
                }


                return View("SelectBudget", mv);
            } else
            {
                return View("../Home/NotLoggedIn");
            }
        }

        public ActionResult BVAComparison(int BudgetID = 0)
        {
            if (Session["CustomerID"] != null && (int)Session["CustomerID"] > 0)
            {
                //if (Models.GlobalVariables.SelectedPropertyID == 0)
                //{
                //    Models.GlobalVariables.SelectedPropertyID = PropID;
                //    Models.GlobalVariables.SelectedProperty = Models.PropertyMethods.PropertyAddress(PropID);
                //}

                //Models.GlobalVariables.SelectedProperty = property.Address1;
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel(ViewModelLevel.none);
                mv.Estate = new Estates();
                mv.Estate = Models.EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                mv.Estate.BudgetId = BudgetID;
                mv.Estate = Models.EstateMethods.GetBVA(mv.Estate);
                //mv.Estate.PieChartData = new System.Data.DataTable();
                //mv.Estate.PieChartData.Columns.Add("Heading");
                //mv.Estate.PieChartData.Columns.Add("Cost");
                return View("BudgetVActual", mv);
            }
            else
            {
                return View("../Home/NotLoggedIn");
            }
        }

        public async Task<ActionResult> ServiceChargeAccount(string PropID = "", string PropName = "")
        {
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();
            

            UnitServiceChargesViewModel vm = new UnitServiceChargesViewModel(ViewModelLevel.Unit);
            await vm.SetBaseDataAsync(id, email);
            if (!string.IsNullOrWhiteSpace(PropID))
            {
                vm.SelectedUnitID = PropID;
                vm.SelectedUnitName = PropName;

            }
            if (!string.IsNullOrWhiteSpace(vm.SelectedUnitID))
            {
                //Units unit = await UnitMethods.GetUnitsAsync()
                await vm.unitstatementViewModel.LoadAsync(vm.SelectedUnitID);
            } else
            {
                vm.ViewName = "ServiceChargeAccount";
                vm.ControllerName = "ServiceCharges";
            }

            return View(vm);

            

           
        }

    }

    public class UnitServiceChargesViewModel: ViewModelBase
    {
        public UnitServiceChargesViewModel(ViewModelLevel level = ViewModelLevel.Unit):base(level)
        {
            unitstatementViewModel = new UnitStatementViewModel();
        }

        public UnitStatementViewModel unitstatementViewModel { get; set; }
    }

    public class UnitStatementViewModel
    {
        public UnitStatementViewModel()
        {
            Transactions = new List<UnitStatmentTransaction>();
        }
        public List<UnitStatmentTransaction> Transactions { get; set; }

        public async Task LoadAsync(string SelectedUnitID)
        {
            Transactions = await UnitStatementMethods.GetunitStatementAsync(SelectedUnitID);
        }
    }
}
