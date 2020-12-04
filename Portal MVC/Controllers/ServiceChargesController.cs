using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Portal_MVC.Models;

namespace Portal_MVC.Controllers
{
    public class ServiceChargesController : Controller
    {
        // GET: ServiceCharge


        public ActionResult Index(int PropID = 0, string PropName = "")
        {
            if (Session["CustomerID"] != null && (int)Session["CustomerID"] != 0)
            {
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel();

                if (PropID > 0)
                {
                    Session["SelectedPropertyID"] = PropID;
                    Session["SelectedProperty"] = PropName;
                    Session["IsDirector"] = EstateDirectors.EstateDirectorMethods.IsCustomerDirector(GlobalVariables.CS, PropID).ToString();
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




        public ActionResult ServiceChargeExpenditure(int PropID = 0, string PropName = "")
        {
            if (Session["CustomerID"] != null || (int)Session["CustomerID"] != 0)
            {
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel();

                if (PropID > 0)
                {
                    Session["SelectedPropertyID"] = PropID;
                    Session["SelectedProperty"] = PropName;
                    Session["IsDirector"] = EstateDirectors.EstateDirectorMethods.IsCustomerDirector(GlobalVariables.CS, PropID).ToString();
                }

                if (Session["SelectedPropertyID"] == null || (int)Session["SelectedPropertyID"] == 0 )
                {
                    //Get Property List

                    mv.PropertyList = Models.PropertyMethods.GetAllOwnedProperties((int)Session["CustomerID"]);
                    mv.ViewName = "ServiceChargeExpenditure";
                    mv.ControllerName = "ServiceCharges";
                }
                else
                {
                    mv.Estate = Models.EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                    mv.Estate = Models.EstateMethods.GetServiceChargeExpenditure(mv.Estate);
                }

                return View("ServiceChargeExpenditure", mv);
            } else
            {
                return View("../Home/NotLoggedIn");
            }
        }

        public ActionResult ServiceChargeBudget(int BudgetID = 0)
        {
            if (Session["CustomerID"] != null)
            {
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel();
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
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel();
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
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel();
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
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel();
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
                Models.ServiceChargeBudgetViewModel mv = new Models.ServiceChargeBudgetViewModel();
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

        public ActionResult ServiceChargeAccount(int PropID = 0, string PropName = "")
        {
            if (Session["CustomerID"] != null && (int)Session["CustomerID"] != 0)
            {
                if(PropID != 0)
                {
                    Session["SelectedPropertyID"] = PropID;
                    Session["SelectedProperty"] = PropName;
                }

                ServiceChargeBudgetViewModel vm = new ServiceChargeBudgetViewModel();

                if (Session["SelectedPropertyID"] != null && (int)Session["SelectedPropertyID"] != 0)
                {
                    vm.MyServiceCharges = new ServiceCharges();
                    
                    vm.MyServiceCharges.AllTrans = ServiceChargeMethods.AllTransactions((int)Session["SelectedPropertyID"]);
                    vm.MyServiceCharges.AllTrans = ServiceChargeMethods.ReplaceTransactionDateWithPaidDate(vm.MyServiceCharges.AllTrans);
                    return View(vm);
                } else
                {
                    vm.PropertyList = Models.PropertyMethods.GetAllOwnedProperties((int)Session["CustomerID"]);
                    vm.ViewName = "ServiceChargeAccount";
                    vm.ControllerName = "ServiceCharges";
                    return View(vm);
                }
            } else
            {
                //return not logged in
                return View("../Home/NotLoggedIn");
            }
          
        }

    }
}
