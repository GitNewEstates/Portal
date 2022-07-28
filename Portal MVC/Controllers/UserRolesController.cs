using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Portal_MVC.Models;
using System.Data;
using System.Configuration;
using Microsoft.AspNet.Identity.Owin;

namespace Portal_MVC.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserRolesController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public UserRolesController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            
        }
        public UserRolesController()
        {

        }
        // GET: UserRoles
        public async Task<ActionResult> Index()
        {
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();

            UserRoleViewModel model = new UserRoleViewModel();
            
            await model.SetBaseDataAsync(id, email);
            await model.LoadAsync();
            return View("../Admin/userRoles", model);
        }

         
        [HttpPost]
        public async Task< ContentResult> Update(UserRoleObject obj)
        {
            if (obj != null)
            {
                if (!string.IsNullOrWhiteSpace(obj.Email))
                {
                    var user = await UserManager.FindByEmailAsync(obj.Email);
                    if(user != null || !string.IsNullOrWhiteSpace( user.Id))
                    {
                        //remove roles
                        
                        await UserManager.AddToRoleAsync(user.Id, Role(obj));

                        await UserManager.RemoveFromRoleAsync(user.Id, RoleToRemove(obj));


                    } else
                    {
                        obj.APIError = new APIError(ErrorType.ProcessingError);
                        obj.APIError.HasError = true;
                        obj.APIError.Message = "Error obtaining user data";
                    }
                } else
                {
                    obj.APIError = new APIError(ErrorType.ProcessingError);
                    obj.APIError.HasError = true;
                    obj.APIError.Message = "Posted data does not contain user email";
                }

            } else
            {
                obj.APIError = new APIError(ErrorType.ProcessingError);
                obj.APIError.HasError = true;
                obj.APIError.Message = "Posted data not found";
            }

            return Content(UserRoleObjectMethods.JsonSerialize(obj), "application/json");
        }

        private string RoleToRemove(UserRoleObject obj)
        {
            
            if (obj.OriginalRole == "Admin")
            {
                return "Administrator";
                
            }
            if (obj.OriginalRole == "Manager")
            {
                return "Manager";

            }
            if (obj.OriginalRole == "PropManager")
            {
                return "Property Manager";

            }
            if (obj.OriginalRole == "MaintenanceOperative")
            {
                return "Maintenance Operative";

            }
            if (obj.OriginalRole == "Client")
            {
                return "Client";

            }
            if (obj.OriginalRole == "Customer")
            {
                return "Customer";

            }
            if (obj.OriginalRole == "Supplier")
            {
                return "Supplier";

            }
            return "";
        }

        private string Role(UserRoleObject obj)
        {
            if (obj.Admin)
            {
                return "Administrator";
            }
            if (obj.Manager)
            {
                return "Manager";
            }
            if (obj.PropertyManager)
            {
                return "Property Manager";
            }
            if (obj.MaintenanceOperative)
            {
                return "Maintenance Operative";
            }
            if (obj.Client)
            {
                return "Client";
            }
            if (obj.Customer)
            {
                return "Customer";
            }
            if (obj.Supplier)
            {
                return "Supplier";
            }

            return "";
        }

        // GET: UserRoles/Create
        [HttpPost]
        public ContentResult Create()
        {
            return Content("Success");
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
        }

        public async Task LoadAsync()
        {
            Users = await UserRoleObjectMethods.GetUserRoleObjects();
        }
        public List<UserRoleObject> Users { get; set; }
    }

    public class UserRoleObject : BaseClass
    {
        //public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public bool Admin { get; set; }
        public bool Client { get; set; }
        public bool Customer { get; set; }
        public bool MaintenanceOperative { get; set; }
        public bool Manager { get; set; }
        public bool PropertyManager { get; set; }
        public bool Supplier { get; set; }
        public string OriginalRole { get; set; }
    }

    public static class UserRoleObjectMethods
    {
        public async static Task<List<UserRoleObject>> GetUserRoleObjects()
        {

            string q = "select email, usersnames.name, aspnetroles.Name from AspNetUsers " +
                        "inner join aspnetuserroles on AspNetUsers.Id = AspNetUserRoles.UserId " +
                        "inner join aspnetroles on AspNetUserRoles.RoleId = AspNetRoles.Id " +
                        "inner join UsersNames on aspnetusers.Id = usersnames.userid" +
                        " order by usersnames.name asc";
            Models.GlobalVariables.CS = ConfigurationManager.ConnectionStrings["AccessConnection"].ConnectionString;
            DataTable dt = await GlobalVariables.GetConnection().Connection.GetDataTableAsync(q);
            List<UserRoleObject> r = new List<UserRoleObject>();
            
            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach(DataRow dr in dt.Rows)
                {
                    UserRoleObject obj = new UserRoleObject();
                    obj.Email = dr[0].ToString();
                    obj.Name = dr[1].ToString();

                    switch (dr[2].ToString())
                    {
                        case "Administrator":
                            obj.Admin = true;
                            break;
                        case "Manager":
                            obj.Manager = true;
                            break;
                        case "Property Manager":
                            obj.PropertyManager = true;
                            break;
                        case "Maintenance Operative":
                            obj.MaintenanceOperative = true;
                            break;
                        case "Client":
                            obj.Client = true;
                            break;
                        case "Customer":
                            obj.Customer = true;
                            break;
                        case "Supplier":
                            obj.Supplier = true;
                            break;
                    }

                    r.Add(obj);
                }
            }

            

            Models.GlobalVariables.CS = ConfigurationManager.ConnectionStrings["DeployConnection"].ConnectionString;

            return r;
        }

        public static string JsonSerialize(UserRoleObject obj)
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }

    //public static class UserRoleMethods
    //{
    //    public static string JsonSerialize(UserRoleObject obj)
    //    {

    //        return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
    //    }
    //}
}
