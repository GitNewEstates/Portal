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

     
  

        public ViewResult AttendanceDetail(int VisitID)
        {
            Models.AttendanceHistoryViewModel vm = new Models.AttendanceHistoryViewModel();
            if (Session["CustomerID"] != null && (int)Session["CustomerID"] > 0)
            {
                vm.GetVisit(VisitID);
               // return View("ViewAttendanceHistory", vm);
            }
            else
            {
                return View("../Home/NotLoggedIn");
            }
            return View(vm);
        }

      
    }
}