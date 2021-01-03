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
            return View("CaretakingDashboard");
        }

        public ActionResult AttendanceLog()
        {
            Models.AttendanceLogViewModel vm = new Models.AttendanceLogViewModel();
            vm.SetLists();

            return View(vm);
        }

        [HttpPost]
        public ActionResult SubmitAttendance(Models.AttendanceLogViewModel ViewModel)
        {
            var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadFiles"];
            var fileSave = System.Web.HttpContext.Current.Server.MapPath("UploadedFiles");
            var fileSavePath = Path.Combine(fileSave, httpPostedFile.FileName);

            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
            {
                fileData = binaryReader.ReadBytes(Request.Files[0].ContentLength);
            }
            
            foreach (var file in Request.Files)
            {
                if (file != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        // act on the Base64 data
                    }
                }
            }

            if (ModelState.IsValid)
            {

            } else
            {
                ViewModel.SetLists();
            }
            return View("AttendanceLog", ViewModel);
        }
    }
}