using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Portal_MVC.Models;
using Newtonsoft.Json;

namespace Portal_MVC.Controllers
{
    public class OwnerController : Controller
    {
        // GET: Owner
        public async Task<ActionResult> AddOwner()
        {
            SetBaseData();
            AddOwnersViewModel viewmodel = new AddOwnersViewModel();
            await viewmodel.SetBaseDataAsync(id, email);
            await viewmodel.LoadDataAsync();
            return View(viewmodel);
        }

        private string id { get; set; }
        private string email { get; set; }

        private void SetBaseData()
        {
            id = User.Identity.GetUserId();
            email = User.Identity.GetUserName();
        }

        public async Task<ActionResult> OwnerList()
        {
            SetBaseData();
            OwnerListViewModel viewmodel = new OwnerListViewModel();

            await viewmodel.SetBaseDataAsync(id, email);
            await viewmodel.LoadDataAsync();
            return View(viewmodel);
        }

        public async Task<ActionResult> OwnerDetail(int ownerid)
        {
            SetBaseData();
            OwnerDetailViewModel viewModel = new OwnerDetailViewModel();
            await viewModel.SetBaseDataAsync(id, email);
            await viewModel.LoadAsync(ownerid);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ContentResult> AddOwnedUnit(OwnershipObj obj)
        {
           OwnershipObj obj1 = await OwnershipObjMethods.InsertAsync(obj);
            string h = obj1.UnitBalance; 

            if (obj1.HasError)
            {
                obj1.APIError = new APIError(ErrorType.APIConnectionError);
                obj1.APIError.HasError = true;
                obj1.APIError.Message = "API Error";
            }

           return Content(OwnershipObjMethods.SerializeOwnerToJson(obj1));
        }
        // GET: Owner/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Owner/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Owner/Create
        [HttpPost]
        public async Task<ContentResult> Create(Models.Owner owner)
        {
            try
            {
               // Models.Owner o = Models.OwnerMethods.JsonDeserialize(ownerjson);
                owner = await Models.OwnerMethods.InsertAsync(owner);
                if(owner == null || owner.id == 0)
                {
                    owner = new Models.Owner();
                    if (!owner.APIError.HasError)
                    {
                        owner.APIError.errorType = Models.ErrorType.APIDBInsertError;
                        owner.APIError.HasError = true;
                    }
                }
                string
                    json = Models.OwnerMethods.SerializeOwnerToJson(owner);

                return
                    Content(json);
            }
            catch (Exception ex)
            {
                Models.APIError error = new Models.APIError(Models.ErrorType.APIDBInsertError);
                error.Message = ex.Message;
                error.HasError = true;
                Models.Owner owner1 = new Models.Owner();
                owner1.APIError = error;
                return Content(Models.OwnerMethods.SerializeOwnerToJson(owner1));
            }
        }

        // GET: Owner/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Owner/Edit/5
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

        // GET: Owner/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Owner/Delete/5
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

    public class OwnershipObj : BaseClass
    {
        public int OwnerID { get; set; }
        public int UnitID { get; set; }
        public DateTime Startdate { get; set; }

        public string UnitBalance { get; set; }

       
    }
    public static class OwnershipObjMethods
    {
        public static OwnershipObj DeserializedJSONToOwnershipData(string json = "")
        {
            OwnershipObj obj = new OwnershipObj();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<OwnershipObj>(json);
                }
                catch (Exception ex)
                {
                    obj.APIError = new APIError(ErrorType.APIValidationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Ownership data object. Error: {ex.Message}"
                    };
                }
            }

            return obj;
        }

        public static string SerializeOwnerToJson(OwnershipObj owner)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(owner);
        }

        public async static Task<OwnershipObj> InsertAsync(OwnershipObj obj)
        {

            string returnjson = await
               APIMethods.CallAPIPostEndPointAsync("AddOwnedUnit", OwnershipObjMethods.SerializeOwnerToJson(obj));

            return DeserializedJSONToOwnershipData(returnjson);
        }
    }
}
