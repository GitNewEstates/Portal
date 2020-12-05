using System;
using System.Collections.Generic;
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
            return View("CaretakingDashboard");
        }

        public ActionResult AttendanceLog()
        {
            Models.AttendanceLogViewModel vm = new Models.AttendanceLogViewModel();
            vm.EstateList = Models.PropertyMethods.GetAllEstates();

            return View(vm);
        }
    }
}