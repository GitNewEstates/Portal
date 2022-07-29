using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Portal_MVC.Models;
using System.Data;
using System.Configuration;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace Portal_MVC.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        // GET: Users
        public async Task< ActionResult> Index()
        {
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();

            UsersViewModel model = new UsersViewModel();

            await model.SetBaseDataAsync(id, email);
           
            return View("../Admin/Users", model);
        }
    }

    public class UserObject : PersonBase
    {
        public bool IsLockedOut { get; set; }
    }

    public class UsersViewModel : ViewModelBase
    {
        public static List<UserObject> UsersList { get; set; }
    }
}