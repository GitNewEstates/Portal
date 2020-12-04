using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_MVC.Controllers
{
    public class PropertySummaryController : Controller
    {
        public ActionResult PropertySummary()
        {
            ViewBag.Test = "Property Summary";
            return View();
        }
    }
}