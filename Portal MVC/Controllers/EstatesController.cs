using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Portal_MVC.Models;

namespace Portal_MVC.Controllers
{
    public class EstatesController : Controller
    {
        private string id { get; set; }
        private string email { get; set; }

        private void SetBaseData()
        {
            id = User.Identity.GetUserId();
            email = User.Identity.GetUserName();
        }
        // GET: Estates
        public async Task<ActionResult> Index()
        {
            EstateViewModel model = new EstateViewModel();
            SetBaseData();
            await model.SetBaseDataAsync(id, email);
            await model.LoadAsync();
            return View(model);
        }

        // GET: Estates/Details/5
        public async Task<ActionResult> EstateDetail(int EstateID)
        {
            EstateDetailViewModel model = new EstateDetailViewModel();
            SetBaseData();
            await model.SetBaseDataAsync(id, email);
            await model.LoadAsync(EstateID);
            return View(model);
        }

        public async Task<ActionResult> AccessDetails(int AccessID)
        {
            EstateAccessDetailViewModel model = new EstateAccessDetailViewModel();
            SetBaseData();
            await model.SetBaseDataAsync(id, email);
            return View(model);

        }

        // GET: Estates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Estates/Create
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

        // GET: Estates/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Estates/Edit/5
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

        // GET: Estates/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Estates/Delete/5
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

    public class EstateViewModel : ViewModelBase
    {
        public EstateViewModel(ViewModelLevel level = ViewModelLevel.Estate) : base(level)
        {
            Estates = new List<APIEstates>();
        }
        public List<APIEstates> Estates { get; set; }

        public async Task LoadAsync()
        {
            Estates = await APIEstateMethods.GetEstateListAsync();
        }
    }

    public class EstateDetailViewModel : ViewModelBase
    {
        public EstateDetailViewModel(ViewModelLevel level = ViewModelLevel.Estate) : base(level)
        {
            Estate = new APIEstates();
            EstateAccessInstructions = new List<EstateAccessInstruction>();

            TabHeaders = new List<Syncfusion.EJ2.Navigations.TabTabItem>();
            TabHeaders.Add(new Syncfusion.EJ2.Navigations.TabTabItem 
            { Header = new Syncfusion.EJ2.Navigations.TabHeader 
            { Text = "Details" }, 
                Content = "#estate-details" });

            TabHeaders.Add(new Syncfusion.EJ2.Navigations.TabTabItem
            {
                Header = new Syncfusion.EJ2.Navigations.TabHeader
                { Text = "History" },
                Content = "#estate-history"
            });
            TabHeaders.Add(new Syncfusion.EJ2.Navigations.TabTabItem
            {
                Header = new Syncfusion.EJ2.Navigations.TabHeader
                { Text = "Financials" },
                Content = "#estate-financial"
            });
            TabHeaders.Add(new Syncfusion.EJ2.Navigations.TabTabItem
            {
                Header = new Syncfusion.EJ2.Navigations.TabHeader
                { Text = "Reports" },
                Content = "#estate-reports"
            });
            TabHeaders.Add(new Syncfusion.EJ2.Navigations.TabTabItem
            {
                Header = new Syncfusion.EJ2.Navigations.TabHeader
                { Text = "Cases" },
                Content = "#estate-cases"
            });

        }

        public APIEstates Estate { get; set; }
        public List<Syncfusion.EJ2.Navigations.TabTabItem> TabHeaders { get; set; }
        public List<EstateAccessInstruction> EstateAccessInstructions { get; set; }

        public async Task LoadAsync(int estateid)
        {
            Estate = await APIEstateMethods.GetEstateAsync(estateid);
            EstateAccessInstructions = await EstateAccessInstructionMethods.GetEstateAccessInstructions(estateid);
        }
    }

    public class EstateAccessDetailViewModel : ViewModelBase
    {
        public EstateAccessDetailViewModel(ViewModelLevel level = ViewModelLevel.Estate) : base(level)
        {
                
        }
    }
}
