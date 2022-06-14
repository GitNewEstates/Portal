using Portal_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal_MVC.Controllers
{
    public class ExpenseController : Controller
    {
        [HttpGet]
        // GET: Expense
        public async Task< ActionResult> Index()
        {
            Models.PostExpenseViewModel vm = new Models.PostExpenseViewModel();
            await vm.SetLists();
            vm.ExpenseDate = DateTime.Today;
            vm.MaxDate = DateTime.Today;

            return View("../Caretaking/PostExpenseView", vm);
        }

        [HttpGet]
        // GET: Expense/GetBudgetDetails/5
        public async Task<ContentResult> GetBudgetDetails(int id)
        {
            string json =
                await GlobalVariables.APIConnection.CallAPIGetEndPointAsync($"BudgetList/{id}"); 

            //deserialise
           List<ServiceChargeBudget> list
                = ServiceChargeBudgetMethods.DeserializedJSONToBudgetList(json);

            //remove income only budgets
            List<ServiceChargeBudget> r = new List<ServiceChargeBudget>();
            foreach(ServiceChargeBudget budget in list)
            {
                if(budget.PostingTypeID != 1)
                {
                    r.Add(budget);
                }
            }
            

            //serialise
            string rJson = ServiceChargeBudgetMethods.JsonSerialize(r);

            return Content(rJson, "application/json");
        }

        [HttpGet]
        public async Task<ContentResult> GetBudgetHeadings(int id, string schedule)
        {
            string json = await 
                GlobalVariables.APIConnection.CallAPIGetEndPointAsync($"BudgetHeadingList/{id}?scheduleName={schedule}");

            return Content(json, "application/json");
        }

        [HttpGet]
        // GET: Expense/GetBudgetScheduleDetails/5
        public async Task<ContentResult> GetBudgetScheduleDetails(int id)
        {
            string json =
                await GlobalVariables.APIConnection.CallAPIGetEndPointAsync($"BudgetScheduleList/{id}");
            return Content(json, "application/json");
        }

        // GET: Expense/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Expense/Create
        [HttpPost]
        public async Task<ContentResult> Create(string value)
        {
            try
            {
                //desrialise
                Models.Transaction transaction = new Models.Transaction();
                if (transaction.APIError.HasError)
                {
                    return Content(TransactionMethods.JsonSerialize(transaction),  "application/json");
                }

                //insert transaction
                transaction = await TransactionMethods.InsertAsync(transaction);

                if (transaction.APIError.HasError)
                {
                    return Content(TransactionMethods.JsonSerialize(transaction), "application/json");
                }

                //insert authorised invoice data
                AuthorisedInvoices authinvoice = new AuthorisedInvoices();
                authinvoice.TransactionId = transaction.id;
                authinvoice.AuthDate = DateTime.Now;
                authinvoice.AuthUser = 3; //replace with method to get userid
                authinvoice.AuthRequestUser = 3;
                authinvoice.InvoiceRef = ""; //replace with invoice ref from model 
                authinvoice.ProcessedDate = DateTime.Now;
                authinvoice.InvoiceDate = transaction.Date;


                //insert supplier payment data



                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Expense/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Expense/Edit/5
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

        // GET: Expense/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Expense/Delete/5
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
