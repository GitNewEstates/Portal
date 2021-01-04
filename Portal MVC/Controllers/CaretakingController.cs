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
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ViewModel.Insert));
                    t.Start();
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
        public ActionResult AttendanceHistory()
        {
            if (Session["CustomerID"] != null && (int)Session["CustomerID"] > 0)
            {
                Models.AttendanceHistoryViewModel vm = new Models.AttendanceHistoryViewModel();
                vm.SetList();

                return View("ViewAttendanceHistory", vm);
            }
            else
            {
                return View("../Home/NotLoggedIn");
            }
        }

        [HttpPost]
        public PartialViewResult LoadAttendanceHistory(Models.AttendanceHistoryViewModel viewmodel)
        {
            viewmodel.SetList();
            viewmodel.TestString = "Test";
            return PartialView("ViewAttendanceHistory", viewmodel);
        }

      
    }
}