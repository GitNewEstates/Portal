using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Portal_MVC.Models;

namespace Portal_MVC.Controllers
{
    public class CasesController : Controller
    {
        // GET: Cases
        public async Task<ActionResult> Index(int EstateID = 0)
        {
            CaseListViewModel vm = new CaseListViewModel();
            EstateID = 4;
            vm.CaseList = 
                await EstateCaseMethods.GetAllEstateCases(estateID: EstateID, 
                _status: Cases.CaseStatus.None);

            return View(vm);
        }

        // GET: Cases/Details/5
        [HttpGet]
        public async Task< ActionResult> CaseDetail(int id = 0, 
            Models.CaseDetailViewModel viewModel = null)
        {
            if(viewModel == null)
            {
                viewModel = new CaseDetailViewModel();
            }

            if (viewModel.EstateCase.id == 0 && id > 0)
            {
                //New call for case detail
                viewModel.EstateCase = await EstateCaseMethods.GetEstateCase(id);
            } else if(viewModel.EstateCase.id > 0)
            {
                //call from update
            }
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult UpdateTitle()
        {
            return Json(new { Result = "success" });
        }

        // GET: Cases/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cases/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Cases/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Cases/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Cases/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Cases/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
