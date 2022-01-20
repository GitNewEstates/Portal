using Portal_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_MVC.Controllers
{
    public class PropertyDetailsController : Controller
    {
 

        // GET: PropertyDetails
        public ActionResult Insurances(int PropID = 0, string PropName = "", int PolicyID = 0
            )
        {
           
                if (PolicyID == 0) //if policy not selected
                {
                    if (Session["CustomerID"] != null && (int)Session["CustomerID"] != 0)
                    {
                        if (PropID != 0)
                        {
                            Session["SelectedPropertyID"] = PropID;
                            Session["SelectedProperty"] = PropName;
                            Session["IsDirector"] = EstateDirectors.EstateDirectorMethods.IsCustomerDirector(GlobalVariables.GetConnection(), PropID).ToString();
                        }

                        Models.InsurancesViewModel vm = new Models.InsurancesViewModel();

                        if (Session["SelectedPropertyID"] != null && (int)Session["SelectedPropertyID"] != 0)
                        {
                            //get list of all insurances
                            vm.InsuranceList = Models.InsuranceMethods.GetInsuranceList((int)Session["SelectedPropertyID"]);

                            vm.ViewName = "Insurances";
                            vm.ControllerName = "PropertyDetails";

                            //vm.MyServiceCharges = new ServiceCharges();

                            //vm.MyServiceCharges.AllTrans = ServiceChargeMethods.AllTransactions((int)Session["SelectedPropertyID"]);
                            //vm.MyServiceCharges.AllTrans = ServiceChargeMethods.ReplaceTransactionDateWithPaidDate(vm.MyServiceCharges.AllTrans);
                            return View(vm);
                        }
                        else
                        {
                            vm.PropListViewModel = new Models.ServiceChargeBudgetViewModel();
                            vm.PropListViewModel.PropertyList = Models.PropertyMethods.GetAllOwnedProperties((int)Session["CustomerID"]);
                            vm.ViewName = "InsurancesView";
                            vm.ControllerName = "PropertyDetails";
                            return View(vm);
                        }
                    }
                    else
                    {
                        //return not logged in
                        return View("../Home/NotLoggedIn");
                    }
                }
                else //Policy is selected
                {
                    

                    Models.InsurancesViewModel vm = new Models.InsurancesViewModel();
                    vm.SelectedInsurance = Models.InsuranceMethods.GetInsuranceByPolicyID(PolicyID);
                    vm.SelectedInsurance.Documents = Models.DocumentMethods.GetDocListByInstanceID(vm.SelectedInsurance.DocInstanceID);
                    Models.UsageData.InsertNewUsage(8, (int)Session["CustomerID"], vm.SelectedInsurance.PolicyName + " " +
                    vm.SelectedInsurance.StartDate.ToShortDateString() + " - " +
                    vm.SelectedInsurance.EndDate.ToShortDateString());

                    return View(vm);
                }
            
        }

        public ActionResult DownloadFile(int policyID, long DocID, string fileName)
        {
            try
            {
                Models.Documents Document = Models.DocumentMethods.GetDocumentByID(DocID);

                Models.UsageData.InsertNewUsage(9, (int)Session["CustomerID"], "Document ID " + DocID.ToString() + " - " + fileName);

                Response.Clear();
                Response.ContentType = "application/octect-stream";
                Response.AddHeader("Content-Disposition", "filename=" + fileName);
                long filesize = (long)Document.Document.Length;
                Response.AddHeader("Content-Lenght", filesize.ToString());
                Response.BinaryWrite(Document.Document);
                Response.Flush();
                Response.Close();
            }
            catch
            {

            }


            return RedirectToAction("Insurances", "PropertyDetails", new { PolicyID = policyID });
        }
    }
}