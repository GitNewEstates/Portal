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
    public class UserRolesController : Controller
    {
        // GET: UserRoles
        public async Task<ActionResult> Index()
        {
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();

            UserRoleViewModel model = new UserRoleViewModel();
            await model.SetBaseDataAsync(id, email);
            return View("../Admin/userRoles", model);
        }

        // GET: UserRoles/Details/5
        public ActionResult Update(int id)
        {
            return View();
        }

        // GET: UserRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserRoles/Create
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

        // GET: UserRoles/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserRoles/Edit/5
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

        // GET: UserRoles/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserRoles/Delete/5
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


    public class UserRoleViewModel : ViewModelBase
    {
        public UserRoleViewModel()
        {
            Users = new List<UserRoleObject>();

            for(int i = 1; i<= 10; i++)
            {
                Users.Add(new UserRoleObject()
                {
                    Name = $"Name {i}",
                    Email = $"Email {i}"
                });
            }
        }
        public List<UserRoleObject> Users { get; set; }
    }

    public class UserRoleObject
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public bool Admin { get; set; }
        public bool Client { get; set; }
        public bool Customer { get; set; }
        public bool MaintenanceOperative { get; set; }
        public bool Manager { get; set; }
        public bool PropertyManager { get; set; }
    }
}
