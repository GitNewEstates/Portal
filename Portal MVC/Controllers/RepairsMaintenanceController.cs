using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal_MVC.Models;
using System.Globalization;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace Portal_MVC.Controllers
{ 
    [Authorize(Roles = "Customer,Client")]
    public class RepairsMaintenanceController : Controller
    {
        private int RepairID { get; set; }
        private RepairsMaintenanceViewModel vm { get; set; }

        // GET: RepairsMaintenance
        public ActionResult Summary()
        {
            return View("RepairsSummary");
        }

        public ActionResult ViewAllRepairs(int PropID = 0)
        {
            //need to selet estate
           
            if(Session["CustomerID"] != null && (int)Session["CustomerID"] != 0)
            {
                RepairsMaintenanceViewModel vm = new RepairsMaintenanceViewModel();
                if(PropID != 0 && (int)Session["SelectedPropertyID"] == 0) //property selected from list
                {
                 
                    Session["SelectedPropertyID"] = PropID;
                    Session["SelectedProperty"] = Models.PropertyMethods.PropertyAddress(PropID);
                    Session["IsDirector"] = EstateDirectors.EstateDirectorMethods.IsCustomerDirector(GlobalVariables.GetConnection(), PropID).ToString();
                    Estates e = EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);

                    Session["EstateName"] = e.EstateName;
                    e = null;
                }
                if ((int)Session["SelectedPropertyID"] == 0) //get list of properties
                {
                    vm.PropListViewModel = new ServiceChargeBudgetViewModel(ViewModelLevel.Estate);  
                    vm.PropListViewModel.PropertyList = Models.PropertyMethods.GetAllOwnedProperties((int)Session["CustomerID"]);
                    vm.PropListViewModel.ControllerName = "RepairsMaintenance";
                    vm.PropListViewModel.ViewName = "ViewAllRepairs";
                }
                else
                {
                    Estates Estate = new Estates();
                    Estate = EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                    Session["EstateName"] = Estate.EstateName; 
                    vm.Repair = RepairMethods.GetAllRepairs(Estate.EstatedID);
                    
                }
                return View("RepairsSummary", vm);

            } else
            {
                //return not logged in view
                return View("../Home/NotLoggedIn");
            }
        }

        public async Task<ActionResult> RepairDetail(int repairID, string updateConfirmation = "")
        {
            RepairDetailViewModel viewmodel = new RepairDetailViewModel();
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();
            await viewmodel.SetBaseDataAsync(id, email);
            viewmodel.Repair.ID = repairID;
            await viewmodel.SetReapir();
                RepairID = repairID;

                
                //vm.Repair = new Repairs();
                
                //vm.Repair = RepairMethods.GetAllRepairDetails(repairID);
                //vm.Repair.RepairHistory = new List<Repairs>();

                ////gets call reports
                //vm.CallReports = RepairMethods.GetRepairReport(repairID);

                ////gets service charge budget and expediture 
                //vm.ServiceChargeInfo = EstateMethods.GetExpenditurebyPO(vm.Repair.PONumber);

                ////Gets contractor name
                //vm.ContractorInfo = ContractorMethods.ContractorDetailsByPO(vm.Repair.PONumber);

                ////Gets completion cost
                //if (vm.Repair.Status == "Completed")
                //{
                //    vm.Repair.completionCost = RepairMethods.CompletedCost(vm.Repair.PONumber);
                //    vm.Repair.completionCostStr = Controls.CurrencyString(vm.Repair.completionCost);
                //}

                ////Repair Updates
                //vm.Repair.AutomaticUpdates = Models.RepairUpdateMethods.IsCustomerRegisteredForUpdates(repairID, (int)Session["CustomerID"]);
                //vm.AutomaticUpdateConfirmation = updateConfirmation;
                ////gets repair history
                //vm.Repair.RepairHistory = RepairMethods.GetRepairHistory(repairID);
                //vm.Repair.ID = repairID;
                return View(viewmodel);
           

            
        }

        public ActionResult OutstandingRepairs(int PropID = 0)
        {

            if (Session["CustomerID"] != null && (int)Session["CustomerID"] != 0)
            {

                RepairsMaintenanceViewModel vm = new RepairsMaintenanceViewModel();
                if (Session["SelectedPropertyID"] != null)
                {
                    if (PropID != 0 && Session["SelectedPropertyID"] != null)
                    {
                        Session["SelectedPropertyID"] = PropID;
                        Session["SelectedProperty"] = Models.PropertyMethods.PropertyAddress(PropID);
                        Session["IsDirector"] = EstateDirectors.EstateDirectorMethods.IsCustomerDirector(GlobalVariables.GetConnection(), PropID).ToString();
                        //Models.GlobalVariables.SelectedProperty = Models.PropertyMethods.PropertyAddress(PropID);
                        Estates e = EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                        Session["EstateName"] = e.EstateName;
                        e = null;
                    }
                    if ((int)Session["SelectedPropertyID"] == 0)
                    {
                        vm.PropListViewModel = new ServiceChargeBudgetViewModel(ViewModelLevel.none);
                        vm.PropListViewModel.PropertyList = Models.PropertyMethods.GetAllOwnedProperties((int)Session["CustomerID"]);
                        vm.PropListViewModel.ControllerName = "RepairsMaintenance";
                        vm.PropListViewModel.ViewName = "OutstandingRepairs";
                    } else
                    {
                        Estates Estate = new Estates();
                        Estate = EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                        vm.Repair = RepairMethods.GetOutstandingRepairs(Estate.EstatedID);
                        Session["EstateName"] = Estate.EstateName;
                    }
                }
                else
                {
                    Estates Estate = new Estates();
                    Estate = EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                    vm.Repair = RepairMethods.GetOutstandingRepairs(Estate.EstatedID);
                    Session["EstateName"] = Estate.EstateName;
                }
                return View("OutstandingRepairs", vm);

            }
            else
            {
                //return not logged in view
                return View("../Home/NotLoggedIn");
            }
        }

        [HttpPost]
        public ActionResult AutomaticUpdates(Models.RepairsMaintenanceViewModel vm)
        {
            string UpdateConfirmation = "";
            if(vm.Repair.AutomaticUpdates == false)
            {
                //delete from db
                try
                {
                    Models.RepairUpdateMethods.Remove((int)Session["CustomerID"], vm.Repair.ID);
                    Models.UsageData.InsertNewUsage(7, (int)Session["CustomerID"], "Repair ID " + vm.Repair.ID.ToString());
                } catch { }
               
                UpdateConfirmation = "Thank you. You will no longer receive an email whenever there is a change" +
                    " to this repair.";
                
            } else
            {
                //insert to db
                try
                {
                    Models.RepairUpdateMethods.AddNew((int)Session["CustomerID"], vm.Repair.ID);
                    Models.UsageData.InsertNewUsage(6, (int)Session["CustomerID"], "Repair ID " + vm.Repair.ID.ToString());
                }
                catch { }
                UpdateConfirmation = "Thank you. You will now receive an email whenever there is a change" +
                    " to this repair.";
            }
          

            return RedirectToAction("RepairDetail", new { repairID = vm.Repair.ID, updateConfirmation = UpdateConfirmation });
        }
    }
}