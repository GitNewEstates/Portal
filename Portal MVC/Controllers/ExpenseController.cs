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
        public async Task<ContentResult> Create(PortalTransaction portaltransaction)
        {
            List<string> ProcessLogs = new List<string>();

            //return api error
            try
            {
                int UserID = (int)Session["CustomerID"];

                //insert the docs
                string DocinstanceID = "";
             
                if (portaltransaction.ImageUrls != null)
                {
                    ProcessLogs.Add($"{portaltransaction.ImageUrls.Count()} images to add");
                    int docCount = 0;
                    foreach (string url in portaltransaction.ImageUrls)
                    {
                        docCount += 1;
                        Documents doc = new Documents();
                        doc.DocName = $"Uploaded Document {docCount}";
                        doc.FileExtention = ".html";
                        doc.DocInstanceID = DocinstanceID;
                        doc.UserID = UserID;
                        doc.url = url;

                        doc = await DocumentMethods.InsertDocumentAsync(doc);
                        DocinstanceID = doc.DocInstanceID;
                    }
                   
                }
                ProcessLogs.Add($"Doc Instance Id = {DocinstanceID}");

                //contruct the transaction from a Portal Transaction
                Transaction transaction = new Transaction();
                transaction.Amount = portaltransaction.Amount;
                transaction.BudgetID = portaltransaction.BudgetID;
                transaction.ScheduleName = portaltransaction.ScheduleName;
                transaction.HeadingID = portaltransaction.HeadingID;
                transaction.Description = portaltransaction.Description;
                transaction.TransactionTypeID = 4; //Expenditure
                transaction.DocinstanceID = DocinstanceID;

                ProcessLogs.Add($"Inserting Portal Transaction");

                //insert transaction - do sync as needs the trans id to continue
                transaction = await TransactionMethods.InsertAsync(transaction);


                if (transaction.APIError.HasError)
                {
                    ProcessLogs.Add($"Transaction Returned error. ");

                    await GlobalVariables.SendProcessEmail("Add Expense", ProcessLogs);

                    return Content(TransactionMethods.JsonSerialize(transaction), "application/json");
                }

                //DateTime TestDate = new DateTime();

                DateTime invDate = new DateTime();
                try
                {
                    ProcessLogs.Add($"Result of converting model date of {portaltransaction.TransactionDate} to converible string is {GlobalVariables.ConvertStringToDate(portaltransaction.TransactionDate)}");
                    DateTime.TryParse(GlobalVariables.ConvertStringToDate(portaltransaction.TransactionDate), out invDate);
                    ProcessLogs.Add($"Parsed Invoice Date is {invDate}");
                } catch
                {
                    ProcessLogs.Add($"Error converting {portaltransaction.TransactionDate} to date");
                    invDate = DateTime.Now;
                    
                }
                
             
                //insert authorised invoice data
                AuthorisedInvoices authinvoice = new AuthorisedInvoices();
                authinvoice.TransactionId = transaction.id;
                authinvoice.AuthDate = DateTime.Now;
                ProcessLogs.Add($"auth date = {authinvoice.AuthDate}");
                authinvoice.AuthUser = UserID; 
                authinvoice.AuthRequestUser = UserID;
                authinvoice.InvoiceRef = ""; //replace with invoice ref from model 
                authinvoice.ProcessedDate = DateTime.Now;
                ProcessLogs.Add($"Process date = {authinvoice.ProcessedDate}");
                authinvoice.ProcessedUser = UserID;
                authinvoice.InvoiceDate = invDate;
                ProcessLogs.Add($"invoice date date = {authinvoice.InvoiceDate}");

                ProcessLogs.Add($"Inserting Auth Data");
                authinvoice = await
                    AuthorisedInvoicesMethods.InsertAsync(authinvoice);
                if (authinvoice.APIError.HasError)
                {
                    //roll back transaction - mark in error?
                    ProcessLogs.Add($"auth data return error");
                    transaction.APIError = authinvoice.APIError;
                    
                    await GlobalVariables.SendProcessEmail("Add Expense", ProcessLogs);

                    return Content(TransactionMethods.JsonSerialize(transaction), "application/json");
                }

                DateTime paidDate = new DateTime();
                try
                {

                    DateTime.TryParse(GlobalVariables.ConvertStringToDate(portaltransaction.TransactionDate), out paidDate);
                }
                catch
                {
                    ProcessLogs.Add($"Error converting {portaltransaction.TransactionDate} to date");
                    paidDate = DateTime.Now;
                }



               // DateTime paidDate = GlobalVariables.ConvertStringToDate(portaltransaction.TransactionDate);

                //insert supplier payment data
                Models.SupplierPayment payment = new SupplierPayment();
                payment.TransactionID = transaction.id;
                payment.SupplierID = portaltransaction.SupplierID;
                payment.StatusID = 1; //always marked as paid. 
                payment.paidDate = paidDate;
                payment.UserID = UserID;
                payment.BankAccountID = 16; // always nem bank accounr

                ProcessLogs.Add($"Inserting supplier payment data");
                payment = await SupplierPaymentMethods.InsertAsync(payment);

                if (payment.APIError.HasError)
                {
                    //roll back transaction - mark in error?
                    ProcessLogs.Add($"Supplier Payment data returned error");
                    transaction.APIError = authinvoice.APIError;

                    await GlobalVariables.SendProcessEmail("Add Expense", ProcessLogs);

                    return Content(TransactionMethods.JsonSerialize(transaction), "application/json");
                }

                //await GlobalVariables.SendProcessEmail("Add Expense", ProcessLogs);
                return Content(TransactionMethods.JsonSerialize(transaction), "application/json");
            }
            catch (Exception ex)
            {
                Transaction t = new Transaction();
                t.APIError.HasError = true;
                t.APIError.Message = ex.Message;
                return Content(TransactionMethods.JsonSerialize(t), "application/json");
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
