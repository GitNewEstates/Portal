using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_MVC.Controllers
{
    public class CaretakingController : Controller
    {
        // GET: Caretaking
        public ActionResult Index()
        {
            if (Session["CustomerID"] != null && (int)Session["CustomerID"] > 0)
            { 
                return View("CaretakingDashboard");
            } else
            {
                return View("../Home/NotLoggedIn");
            }
        }

        public ActionResult AttendanceLog()
        {
            if (Session["CustomerID"] != null && (int)Session["CustomerID"] > 0)
            {
                Models.AttendanceLogViewModel vm = new Models.AttendanceLogViewModel();
                vm.SetLists();

                return View(vm);
            }
            else
            {
                return View("../Home/NotLoggedIn");
            }
            
        }
        [HttpPost]
        public ActionResult SubmitAttendance(Models.AttendanceLogViewModel ViewModel)
        {

            if (Session["CustomerID"] != null && (int)Session["CustomerID"] > 0)
            {
                if (ModelState.IsValid)
                {
                    ViewModel.AttendanceObj.AttendingUser = (int)Session["CustomerID"];
                    ViewModel.Insert();
                    //System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ViewModel.Insert));
                    //t.Start();
                    return View("CaretakingDashboard");
                }
                else
                {

                    ViewModel.SetLists();
                }
                return View("AttendanceLog", ViewModel);
            }
            else
            {
                return View("../Home/NotLoggedIn");
            }



        }
        public ActionResult AttendanceHistory(int PropID = 0, string PropName = "")
        {
            Models.AttendanceHistoryViewModel vm = new Models.AttendanceHistoryViewModel();
           
            if (Session["CustomerID"] != null && (int)Session["CustomerID"] > 0)
            {
                if (PropID == 0)
                {

                    PropID = (int)Session["SelectedPropertyID"];
                } else
                {
                    Session["SelectedPropertyID"] = PropID;
                    Session["SelectedProperty"] = PropName;
                }

                if ((int)Session["UserType"] > 1) //get list of properties when not customer type
                {
                    vm.PropListViewModel = new Models.ServiceChargeBudgetViewModel();
                    vm.PropListViewModel.PropertyList = Models.PropertyMethods.GetAllEstates();
                    vm.PropListViewModel.ControllerName = "Caretaking";
                    vm.PropListViewModel.ViewName = "AttendanceHistory";
                } else if(PropID == 0 && (int)Session["UserType"] ==1)
                {
                    vm.PropListViewModel = new Models.ServiceChargeBudgetViewModel();
                    vm.PropListViewModel.PropertyList = Models.PropertyMethods.GetAllOwnedProperties((int)Session["CustomerID"]);
                    vm.PropListViewModel.ControllerName = "Caretaking";
                    vm.PropListViewModel.ViewName = "AttendanceHistory";
                }
                else
                {
                    vm.SelectedPropertyid = PropID;
                    vm.SetAttendanceList();
                    
                }

                return View("ViewAttendanceHistory", vm);
            }
            else
            {
                return View("../Home/NotLoggedIn");
            }
        }
        
        public ViewResult AttendanceDetail(int VisitID = 0, bool isStatic = false, string guid = "")
        {
            
            if (isStatic)
            {
                //not logged in - must have guid and id
                if (!string.IsNullOrWhiteSpace(guid) && VisitID > 0)
                {

                    Models.AttendanceHistoryViewModel vm1 = new Models.AttendanceHistoryViewModel();
                    vm1.GetVisit(VisitID);
                    vm1.FromApp = true;
                    // return View("ViewAttendanceHistory", vm);
                    return View(vm1);
                } else
                {
                    return View("NotFound");
                }

            } else
            {
               
                
                if (Session["CustomerID"] != null && (int)Session["CustomerID"] > 0)
                {
                    if (VisitID > 0)
                    {
                        Models.AttendanceHistoryViewModel vm = new Models.AttendanceHistoryViewModel();
                        vm.FromApp = isStatic;
                        vm.GetVisit(VisitID);
                        return View(vm);
                    } else
                    {
                        return View("NotFound");
                    }
                }
                else
                {
                    return View("../Home/NotLoggedIn");
                }
              
            }

           
        }

        
       


        //public ActionResult LightingReview()
        //{
        //    vm.PropListViewModel = new Models.ServiceChargeBudgetViewModel();
        //    vm.PropListViewModel.PropertyList = Models.PropertyMethods.GetAllEstates();
        //    vm.PropListViewModel.ControllerName = "Caretaking";
        //    vm.PropListViewModel.ViewName = "AttendanceHistory";
        //}

        public ActionResult BuildingAreas()
        {
            Models.BuildingAreaViewModel vm = new Models.BuildingAreaViewModel();
            vm.SetLists();
            return View("BuildingArea", vm);
        }
        [HttpPost]
        public ActionResult GEtEstateBuildingAreas()
        {
            Models.BuildingAreaViewModel vm = new Models.BuildingAreaViewModel();
            vm.SetLists();
            vm.UpdateIDHeading = "Updated";

            return View("BuildingArea", vm);
        }
    }
}