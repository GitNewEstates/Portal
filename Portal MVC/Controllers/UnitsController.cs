using Microsoft.AspNet.Identity;
using Portal_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal_MVC.Controllers
{
    [Authorize(Roles = "Administrator, Maintenance Operative")]
    public class UnitsController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();

            UnitListViewModel vm = new UnitListViewModel(ViewModelLevel.none);
            await vm.SetBaseDataAsync(id, email);
            await vm.UnitModel.LoadAsync();

            return View(vm);

        }

        // GET: Units
        public ActionResult AddUnit()
        {


            return View("AddUnit");
        }

        // GET: Units/Details/5
        public async Task<ActionResult> UnitAccount(string unitID)
        {
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();
            UnitDetailViewModel vm = new UnitDetailViewModel(ViewModelLevel.none);
            await vm.SetBaseDataAsync(id, email);
            await vm.LoadAsync(unitID);

            return View(vm);
        }

        // GET: Units/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Units/Create
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

        // GET: Units/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Units/Edit/5
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

        // GET: Units/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Units/Delete/5
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

    public class UnitListViewModel : Models.ViewModelBase
    {
        public UnitListViewModel(ViewModelLevel level):base(level)
        {
            UnitModel = new UnitListModel();
        }

        public UnitListModel UnitModel { get; set; }
    }

    public class UnitListModel
    {
        public UnitListModel()
        {
            Units = new List<Units>();
        }

        public List<Units> Units { get; set; }

        public async Task LoadAsync()
        {
            Units = await UnitMethods.GetUnitsAsync();
        }
    }

    public class UnitDetailViewModel : ViewModelBase
    {
        public UnitDetailViewModel(ViewModelLevel level): base(level)
        {
            unit = new Units();
            TabHeaders = new List<Syncfusion.EJ2.Navigations.TabTabItem>();
            StatementViewModel = new UnitStatementViewModel();
        }
        public async Task LoadAsync(string id)
        {
            SetTabHeaders();
            unit = await UnitMethods.GetUnitAsync(id);
            await StatementViewModel.LoadAsync(id);
        }
        public UnitStatementViewModel StatementViewModel { get; set; }
        private void SetTabHeaders()
        {
            TabHeaders = new List<Syncfusion.EJ2.Navigations.TabTabItem>();
            TabHeaders.Add(new Syncfusion.EJ2.Navigations.TabTabItem
            {
                Header = new Syncfusion.EJ2.Navigations.TabHeader
                { Text = "Details" },
                Content = "#unit-details"
            });

            TabHeaders.Add(new Syncfusion.EJ2.Navigations.TabTabItem
            {
                Header = new Syncfusion.EJ2.Navigations.TabHeader
                { Text = "Statement" },
                Content = "#unit-statement"
            });
        }
        public Units unit { get; set; }
        public List<Syncfusion.EJ2.Navigations.TabTabItem> TabHeaders { get; set; }
    }
}
