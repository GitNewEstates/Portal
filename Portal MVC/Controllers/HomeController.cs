using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Portal_MVC.Models;
using NotificationSettings;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace Portal_MVC.Controllers
{
     public static class StaticVariables
    {
        public static List<Models.Properties> PropList { get; set; }
    }

    [Authorize(Roles = "Customer,Administrator,Client")]

    //Controller for sign in page
    public class HomeController : Controller
    {
       

        public async Task< ActionResult> Index(int PropID = 0, string PropName = "", int logdata = 0)
        {
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();

            
            //var auth = User.Identity.IsAuthenticated;

            HomeViewModel homeViewModel = new HomeViewModel();
            await homeViewModel.SetBaseDataAsync(id, email);
            
            homeViewModel.ViewName = "Index";
            homeViewModel.ControllerName = "Home";
            switch (homeViewModel.RoleName)
            {
                case "Administrator":
                    //Do speciofic work and send to Administrator Dashboard
                    homeViewModel.ViewName = "AdminDash";
                    break;
                case "Customer":
                    //Do specific work and send to Customer Dashboard
                    if(PropID > 0)
                    {
                        //selected from Estate List
                        homeViewModel.SelectedProperty.ID = PropID;
                        homeViewModel.SelectedProperty.Address1 = PropName;
                        
                    } else
                    {
                        //Customer only has one unit
                    }

                    await homeViewModel.LoadCustomerDashboardDataAsync();
                    break;
                case "Client":
                    homeViewModel.ViewName = "ClientDashboard";
                    
                    break;
            }
            return View(homeViewModel.ViewName, homeViewModel);

            //string viewName = "";
            //object anonObj = null;

            //string i = Session["CustomerName"].ToString();

            ////if own multiple properties then display list of all available properties
            //if (Session["CustomerID"] != null && (int)Session["CustomerID"] != 0)
            //{
            //    if (Session["UserType"].ToString() == "1" )
            //    {
                    

            //        if (PropID == 0)
            //        {
            //            homeViewModel.PropListViewModel.PropertyList
            //            = Models.PropertyMethods.GetAllOwnedProperties((int)Session["CustomerID"]);
            //            if (homeViewModel.PropListViewModel.PropertyList.Count() > 1)
            //            {
            //                //Sends to list of properties (viewname = index)
            //                homeViewModel.viewName = "Index";
            //                Session["SelectedPropertyID"] = 0;
            //                Session["SelectedProperty"] = null;
            //                Session["IsDirector"] = null;
            //                //Models.GlobalVariables.SelectedPropertyID = 0;
            //                //Models.GlobalVariables.SelectedProperty = null;
            //                homeViewModel.anonObj = homeViewModel.PropListViewModel.PropertyList;

            //            }
            //            else
            //            {
                            

            //                foreach(Models.Properties p in homeViewModel.PropListViewModel.PropertyList)
            //                {
            //                    PropID = p.ID;
            //                    PropName = p.Address1;
            //                }

            //                GetPropertySummary(homeViewModel, PropID, PropName);
            //                return View(homeViewModel.viewName, homeViewModel.anonObj);
            //            }
            //        } else
            //        {
            //            GetPropertySummary(homeViewModel, PropID, PropName);
            //            return View(homeViewModel.viewName, homeViewModel.anonObj);
            //        }
            //    } else if (Session["UserType"].ToString() == "2")
            //    {
            //        //get estates
            //        StaticVariables.PropList = Models.PropertyMethods.GetAllEstates();

            //        //Sends to list of properties (viewname = index)
            //        homeViewModel.viewName = "Index";
            //        Session["SelectedPropertyID"] = 0;
            //        Session["SelectedProperty"] = null;
            //        Session["IsDirector"] = null;
            //        //Models.GlobalVariables.SelectedPropertyID = 0;
            //        //Models.GlobalVariables.SelectedProperty = null;
            //        homeViewModel.anonObj = homeViewModel.PropListViewModel.PropertyList; ;
            //    } else if (Session["UserType"].ToString() == "3")
            //    {
            //        homeViewModel.viewName = @"../Caretaking/CaretakingDashboard";
            //        Session["SelectedPropertyID"] = 0;
            //        Session["SelectedProperty"] = null;
            //        Session["IsDirector"] = null;

            //        return RedirectToAction("Index", "Caretaking");
            //    }

            //}
            //return View(homeViewModel.viewName, homeViewModel);
        }

        private HomeViewModel GetPropertySummary(HomeViewModel homeViewModel, int PropID, string PropName)
        {
            //Sends to summary of the property
            homeViewModel.ViewName = "PropertySummary";


            // Models.GlobalVariables.SelectedProperty = StaticVariables.PropList[0].Address1;
            Session["SelectedPropertyID"] = PropID;
            Session["SelectedProperty"] = PropName;
            // Session["IsDirector"] = EstateDirectors.EstateDirectorMethods.IsCustomerDirector(GlobalVariables.CS, StaticVariables.PropList[0].ID).ToString();
            Models.MyAccountViewModel vm = GetViewModel();

            Estates es = new Estates();
            try
            {
                es = EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
            }
            catch { }

            Session["EstateName"] = es.EstateName;

            //Gets outstanding repairs
            vm.RepairVM = new RepairsMaintenanceViewModel();
            vm.RepairVM.Repair = RepairMethods.GetOutstandingRepairs(es.EstatedID);
            // try
            // {

            vm = GetAccountDetails(vm, (int)Session["SelectedPropertyID"]);
            // } catch { }
            vm.PageTitle = "Property Level Contact Preferences";
            homeViewModel.anonObj = vm;
            return homeViewModel;
        }

        private Models.MyAccountViewModel GetViewModel()
        {
            Models.MyAccountViewModel vm = new Models.MyAccountViewModel();
            Models.PropertySummaryViewModel pVM = new Models.PropertySummaryViewModel();

            //Gets all owner Notes

            vm.PropSummaryViewModel = pVM;
           // vm.PropSummaryViewModel.OwnerNotesList = Models.OwnerNotesMethods.GetAllOwnerNotes(Models.GlobalVariables.SelectedPropertyID, Models.GlobalVariables.CustomerID);

            return vm;
        }
        
        public ActionResult PropertySummary(Models.PropertyTypes PropType = PropertyTypes.None, int PropID = 0, string PropName = "")
        {

            if (Session["UserType"].ToString() == "1")
            {
                if (StaticVariables.PropList != null)
                {
                    foreach (Models.Properties p in StaticVariables.PropList)
                    {
                        //sets details to display on toolbar
                        if (p.ID == PropID)
                        {
                            Session["SelectedProperty"] = p.Address1;
                            Session["SelectedPropertyID"] = p.ID;
                           // Session["IsDirector"] = EstateDirectors.EstateDirectorMethods.IsCustomerDirector(GlobalVariables.CS, p.ID);
                            break;
                        }
                    }
                }
                else
                {
                    return View("NotLoggedIn");
                }

                //get View Model with summary notes
                Models.MyAccountViewModel vm = GetViewModel();
                vm.PageTitle = "Property Level Contact Preferences";
                vm = GetAccountDetails(vm, (int)Session["SelectedPropertyID"]);

                //get details for outstanding repairs table
                Estates Estate = new Estates();
                Estate = EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                Session["EstateName"] = Estate.EstateName;
                vm.RepairVM = new RepairsMaintenanceViewModel();
                vm.RepairVM.Repair = RepairMethods.GetOutstandingRepairs(Estate.EstatedID);
                return View(vm);
            }
            else if (Session["UserType"].ToString() == "2")
            {
                if (PropType == PropertyTypes.Estate)
                {
                    StaticVariables.PropList = Models.PropertyMethods.GetAllUnitsProperties(PropID);

                    //Sends to list of properties (viewname = index)
                    //viewName = "Index";
                    Session["SelectedPropertyID"] = 0;
                    Session["SelectedProperty"] = null;
                    Session["IsDirector"] = null;
                    //Models.GlobalVariables.SelectedPropertyID = 0;
                    //Models.GlobalVariables.SelectedProperty = null;
                    object anonObj = null;
                    anonObj = StaticVariables.PropList;

                    return View("Index", anonObj);
                } else if(PropType == PropertyTypes.Unit)
                {
                    //getowners
                    StaticVariables.PropList = Models.PropertyMethods.GetAllUnitsowners(PropID);

                    //Sends to list of properties (viewname = index)
                    //viewName = "Index";
                    Session["SelectedPropertyID"] = PropID;
                    Session["SelectedProperty"] = Models.PropertyMethods.GetPropertyName(PropID);
                    Session["IsDirector"] = null;
                    //Models.GlobalVariables.SelectedPropertyID = 0;
                    //Models.GlobalVariables.SelectedProperty = null;
                    object anonObj = null;
                    anonObj = StaticVariables.PropList;

                    return View("Index", anonObj);
                   
                }
                else if (PropType == PropertyTypes.Owner)
                {
                    //Session["IsDirector"] = EstateDirectors.EstateDirectorMethods.IsCustomerDirector(GlobalVariables.CS, PropID);
                    //Session["CustomerID"] = PropID;

                    //get View Model with summary notes
                    Models.MyAccountViewModel vm = GetViewModel();
                    vm.PageTitle = "Property Level Contact Preferences";
                    vm = GetAccountDetails(vm, (int)Session["SelectedPropertyID"]);

                    //get details for outstanding repairs table
                    Estates Estate = new Estates();
                    Estate = EstateMethods.GetEstatedByUnitID((int)Session["SelectedPropertyID"]);
                    Session["EstateName"] = Estate.EstateName;
                    vm.RepairVM = new RepairsMaintenanceViewModel();
                    vm.RepairVM.Repair = RepairMethods.GetOutstandingRepairs(Estate.EstatedID);
                    return View(vm);

                }

                
            }

            return View("NotLoggedIn");



        }

        public ActionResult PropertySummaryFromAccount()
        {
            Models.MyAccountViewModel vm = (Models.MyAccountViewModel)TempData["TempViewModel"];
            Models.PropertySummaryViewModel pVM = new Models.PropertySummaryViewModel();

            //Gets all owner Notes

            vm.PropSummaryViewModel = pVM;
            vm.PropSummaryViewModel.OwnerNotesList = Models.OwnerNotesMethods.GetAllOwnerNotes((int)Session["SelectedPropertyID"], (int)Session["CustomerID"]);

            vm.PageTitle = "Property Level Contact Preferences";
            
            return View("PropertySummary", vm);

        }

        //Account Level Contact Prefs
        private Models.MyAccountViewModel GetAccountDetails(Models.MyAccountViewModel mv)
        {
            Models.AccountDetails ad = 
                Models.AccountDetailsMethods.OwnerAccountDetails((int)Session["CustomerID"]);

            ad.NumberOfOwnedProperties = 
                Models.AccountDetailsMethods.NumberOfOwnedProperties((int)Session["CustomerID"]);
            
            mv.accountDetails = ad;

            mv.StaticaccountDetails = ad;
            mv.StaticaccountDetails.ContactPref = ad.ContactPref;

            return mv;
        }

        //Property Level Contact Pref
        private Models.MyAccountViewModel GetAccountDetails(Models.MyAccountViewModel mv, int UnitID)
        {
            Models.AccountDetails ad = Models.AccountDetailsMethods.OwnerAccountDetails((int)Session["CustomerID"], UnitID);
            ad.NumberOfOwnedProperties = 1;

            mv.accountDetails = ad;

            mv.StaticaccountDetails = ad;
            mv.StaticaccountDetails.ContactPref = mv.accountDetails.ContactPref;

            mv.GuidanceHeader = "Property Level Contact Preferences";

            mv.Guidance = " are the specific contact preferences for " +
            Session["SelectedProperty"].ToString() + ". This is where service charge " +
            "invoices and general correspondence will be sent for " + Session["SelectedProperty"].ToString() +
            ".These contact preferences can be set to an Agent if one " +
            "is employed to manage the property.";

            return mv;
        }

        private Models.MyAccountViewModel mv { get; set; }

        public ActionResult MyAccount()
        {
            if(Session["CustomerID"] != null)
            {

                Session["SelectedPropertyID"] = 0;
                Session["SelectedProperty"] = null;

                    if (mv == null)
                    {
                        mv = new Models.MyAccountViewModel();

                        mv.PageTitle = "Account Level Contact Preferences";
                        mv.GuidanceHeader = "Account Level Contact Preferences";

                    mv.Guidance = " are your personal contact details and preferences. If these are different to " +
                    "the Property Level Contact Preferences we will only use these details for important " +
                    "correspondence that we think should be sent to you directly instead of an Agent.";
                        
                        //"Property Level Contact Preferences are the specific contact preferences for " +
                        //"for the currently selected property. This is where service charge " +
                        //"invoices and general correspondence will be sent for " + GlobalVariables.SelectedProperty +
                        //".These contact preferences can be set to an Agent if one " +
                        //"is employed to manage your properties.";

                    mv = GetAccountDetails(mv);
                        mv.StaticaccountDetails.ContactPref = mv.accountDetails.ContactPref;
                    }
                    return View(mv);
               
            } else
            {
                return View("NotLoggedIn");
            }
            
        }

        private Models.NotificationSettingsViewModels Nmv { get; set; }
        public ActionResult NotificationSettings(int PropID = 0, string PropName = "")
        {
            if (Session["CustomerID"] != null && (int)Session["CustomerID"] != 0)
            {
                
                if (PropID != 0)
                {
                    Session["SelectedPropertyID"] = PropID;
                    Session["SelectedProperty"] = PropName;
                    
                }
                if (Nmv == null)
                {
                    Nmv = new Models.NotificationSettingsViewModels();
                    //get the settings
                    Nmv.NotificationSettingObj.UnitID = (int)Session["SelectedPropertyID"];
                    Nmv.NotificationSettingObj.CustomerID = (int)Session["CustomerID"];
                    Nmv.NotificationSettingObj.GetNotificationSettings(GlobalVariables.GetConnection());

                }
                if (Session["SelectedPropertyID"] != null && (int)Session["SelectedPropertyID"] != 0)
                {
                   
                    return View(Nmv);
                }
                else
                {
                    Nmv.PropListViewModel = new Models.ServiceChargeBudgetViewModel();
                    Nmv.PropListViewModel.PropertyList = Models.PropertyMethods.GetAllOwnedProperties((int)Session["CustomerID"]);
                    //vm.ViewName = "InsurancesView";
                    //vm.ControllerName = "PropertyDetails";
                    return View(Nmv);
                }

               
                //return View(Nmv);

            }
            else
            {
                return View("NotLoggedIn");
            }

        }


        #region UpdateChargeNotificationSettings

        [HttpPost]
        public ActionResult UpdateNotificationSettings(Models.NotificationSettingsViewModels SettingsObj)
        {
            SettingsObj.NotificationSettingObj.UnitID = (int)Session["SelectedPropertyID"];
            SettingsObj.NotificationSettingObj.CustomerID = (int)Session["CustomerID"];

            SettingsObj.HideAllConfirmations();

            SettingsObj.CompareNotificationSettings();
            //pass the full view model
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(SettingsObj.UpdateSettings));
            t.Start();
            
            ////Below works out which settings have been changed so that correct messages can be displayed
            //if(!SettingsObj.RepairNotificationStatic && SettingsObj.NotificationSettingObj.NewRepairNotification)
            //{
            //    SettingsObj.RepairNotificationReceiveMessage = true;
            //    SettingsObj.RepairNotificationCancelMessage = false;
            //    SettingsObj.RepairNotificationStatic = SettingsObj.NotificationSettingObj.NewRepairNotification;

            //}else if(SettingsObj.RepairNotificationStatic && !SettingsObj.NotificationSettingObj.NewRepairNotification)
            //{
            //    SettingsObj.RepairNotificationReceiveMessage = true;
            //    SettingsObj.RepairNotificationCancelMessage = false;
            //    SettingsObj.RepairNotificationStatic = SettingsObj.NotificationSettingObj.NewRepairNotification;
            //}

            //if (!SettingsObj.ChargeNotificationStatic && SettingsObj.NotificationSettingObj.NewAccountCharge)
            //{
            //    SettingsObj.ChargeNotificationReceiveMessage = true;
            //    SettingsObj.ChargeNotificationCancelMessage = false;
            //    SettingsObj.ChargeNotificationStatic = SettingsObj.NotificationSettingObj.NewAccountCharge;

            //}
            //else if (SettingsObj.ChargeNotificationStatic && !SettingsObj.NotificationSettingObj.NewRepairNotification)
            //{
            //    SettingsObj.ChargeNotificationCancelMessage = true;
            //    SettingsObj.ChargeNotificationReceiveMessage = false;
            //    SettingsObj.ChargeNotificationStatic = SettingsObj.NotificationSettingObj.NewAccountCharge;
            //}

            //if (!SettingsObj.PaymentNotificationStatic && SettingsObj.NotificationSettingObj.NewAccountPayment)
            //{
            //    SettingsObj.PaymentNotificationReceiveMessage = true;
            //    SettingsObj.PaymentNotificationCancelMessage = false;
            //    SettingsObj.PaymentNotificationStatic = SettingsObj.NotificationSettingObj.NewAccountPayment;

            //}
            //else if (SettingsObj.PaymentNotificationStatic && !SettingsObj.NotificationSettingObj.NewRepairNotification)
            //{
            //    SettingsObj.PaymentNotificationCancelMessage = true;
            //    SettingsObj.PaymentNotificationReceiveMessage = false;
            //    SettingsObj.PaymentNotificationStatic = SettingsObj.NotificationSettingObj.NewAccountPayment;
            //}

            //if (!SettingsObj.BudgetNotificationStatic && SettingsObj.NotificationSettingObj.NewSCBudget)
            //{
            //    SettingsObj.BudgetNotificationReceiveMessage = true;
            //    SettingsObj.BudgetNotificationCancelMessage = false;
            //    SettingsObj.BudgetNotificationStatic = SettingsObj.NotificationSettingObj.NewSCBudget;

            //}
            //else if (SettingsObj.BudgetNotificationStatic && !SettingsObj.NotificationSettingObj.NewRepairNotification)
            //{
            //    SettingsObj.BudgetNotificationCancelMessage = true;
            //    SettingsObj.BudgetNotificationReceiveMessage = false;
            //    SettingsObj.BudgetNotificationStatic = SettingsObj.NotificationSettingObj.NewSCBudget;
            //}

            //if (!SettingsObj.BudgetNotificationStatic && SettingsObj.NotificationSettingObj.NewSCBudget)
            //{
            //    SettingsObj.BudgetNotificationReceiveMessage = true;
            //    SettingsObj.BudgetNotificationCancelMessage = false;
            //    SettingsObj.BudgetNotificationStatic = SettingsObj.NotificationSettingObj.NewSCBudget;

            //}
            //else if (SettingsObj.BudgetNotificationStatic && !SettingsObj.NotificationSettingObj.NewRepairNotification)
            //{
            //    SettingsObj.BudgetNotificationCancelMessage = true;
            //    SettingsObj.BudgetNotificationReceiveMessage = false;
            //    SettingsObj.BudgetNotificationStatic = SettingsObj.NotificationSettingObj.NewSCBudget;
            //}

            return View("NotificationSettings", SettingsObj);
        }

        #endregion

        public ActionResult ChangePassword()
        {
            Models.MyChangePasswordViewModel Property = new MyChangePasswordViewModel();

            return View(Property);
        }

        [HttpPost]
        public ActionResult ChangePassword(Models.MyChangePasswordViewModel Property)
        {
            if (ModelState.IsValid != false)
            {

                if (Models.PortalLogins.IsPasswordCorrect(Property.OldPass, (int)Session["CustomerID"], (int)Session["usertype"]) == false)
                {
                    ModelState.AddModelError(string.Empty, "Your Old Password is incorrect.");
                }
                else
                {
                    //update password
                    if (Models.PortalLogins.ChangePassword
                        (Property.NewPass, (int)Session["CustomerID"], (int)Session["UserType"]) == true)
                    {
                        Property.SuccessPasswordUpdateMessage = "Your password has been successfully updated.";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An error occured. Your password has not been updated.");
                    }

                }

            }





            return View("ChangePassword", Property);
        }


        [HttpPost]
        public ActionResult MyAccount(Models.MyAccountViewModel Property)
        {
         

            if (ModelState.IsValid != false)
            {
                //test if update login email selected but no email entered
                if (Property.accountDetails.UpdateLoginEmail == true && Property.accountDetails.Email == null)
                {
                    Property.accountDetails.updateError = "Email Address must be entered to update email for log in.";
                    Property.accountDetails.Email = Property.StaticaccountDetails.Email;
                }
                else
                {

                    //set contact pref
                    if (Property.accountDetails.ContactPref == Models.AccountDetails.ContactPreferences.Post)
                    {
                        Property.accountDetails.PostOnly = true;
                    }
                    if (Property.accountDetails.ContactPref == Models.AccountDetails.ContactPreferences.email)
                    {
                        Property.accountDetails.EmailOnly = true;
                    }
                    if (Property.accountDetails.ContactPref == Models.AccountDetails.ContactPreferences.postandemail)
                    {
                        Property.accountDetails.PostAndEmail = true;
                    }
                    

                    //Test if there has been a change
                    bool DetailsChanged = false;
                    if (Models.AccountDetailsMethods.CompareAccountDetails(Property) == false)
                    {
                        DetailsChanged = true;
                    }


                    if (Property.accountDetails.UseforAllProperties == true)
                    {
                        Models.AccountDetailsMethods.UpdatePropertyAccountPreferences(Property.accountDetails, (int)Session["CustomerID"], (int)Session["SelectedPropertyID"]);
                        Property.SuccessfulAddressUpdate = true;
                    }

                    //sets error message that account details are not changed. 
                    if (DetailsChanged == false && Property.accountDetails.UseforAllProperties == false)
                    {
                        Property.accountDetails.updateError = "You must change your account details before attempting to update.";
                    }

                    //update if details have changed.
                    if (DetailsChanged == true)
                    {
                        //UPDATE ACCOUNT LEVEL DETAILS
                        if (Session["SelectedPropertyID"] == null) //selectedpropertyID will = 0 where main account details are being updated
                        {

                            Property = UpdatePreferences(Property);


                            //update Login Email
                            if (Property.accountDetails.UnableToUpdateLoginEmail == true)
                            {
                                Models.AccountDetailsMethods.UpdateLoginEmail(Property.accountDetails.Email, (int)Session["CustomerID"]);
                            }


                        }
                        //PROPERTY LEVEL
                        else
                        {
                            //update property contact preferences 

                            Property = UpdatePreferences(Property);
                        }
                    }
                }
            } else //modelstate errors
            {
                Property.accountDetails.updateError = "Your Contact Preferences have not been updated.";
            }

            Property.PageTitle = "Account Level Contact Preferences";
            return View(Property);
        }


        private Models.MyAccountViewModel UpdatePreferences(Models.MyAccountViewModel Property)
        {
            //test for contact preferences. cannot be post ane email or email only if 
            //no email.instead set to 'set at account level
            bool allowupdates = true;
            if (Property.accountDetails.Email == null || Property.accountDetails.Email == "")
            {
                if (Property.accountDetails.EmailOnly == true || Property.accountDetails.PostAndEmail == true)
                {
                    allowupdates = false;
                }
            }

            //update Account preferences
            if (allowupdates == true)
            {
                if(Session["SelectedPropertyID"] == null || Property.accountDetails.UpdateMainAccountDetails == true)
                {
                    Models.AccountDetailsMethods.UpdateUserAccountPreferences(Property.accountDetails, (int)Session["CustomerID"]);
                }

                //Selected property ID will not = 0 if updated form property level
                if(Session["SelectedPropertyID"] != null)
                {
                    Models.AccountDetailsMethods.UpdatePropertyAccountPreferences(Property.accountDetails, (int)Session["CustomerID"], (int)Session["SelectedPropertyID"]);
                }
                //update all of the owner's property contact preferences
                //needs  to be updated first as other update may affect the preferences
                if (Property.accountDetails.UseforAllProperties == true)
                {
                    //at account level can only update contact preferences where
                    //update all properties is selected. 
                    Models.AccountDetailsMethods.UpdatePropertyAccountPreferences(Property.accountDetails, (int)Session["CustomerID"]);
                }


                Property.SuccessfulAddressUpdate = true;
            }
            else
            {
                if (Property.accountDetails.EmailOnly == true)
                {
                    Property.accountDetails.updateError = "You cannot set your contact preferences to Email Only without an email address.";
                }
                if (Property.accountDetails.PostAndEmail == true)
                {
                    Property.accountDetails.updateError = "You cannot set your contact preferences to Post and Email without an email address.";
                }

            }

            return Property;
        }

        public void TestIfDirector()
        {

        }


        public ActionResult UpdateContacts(Models.MyAccountViewModel Property)
        {
            if (ModelState.IsValid == false)
            {
                ModelState.AddModelError("", "Your contact details have not been updated.");
            } else
            {

            }

            return View();
        }

        public ActionResult About(int PropID = 0)
        {
            foreach(Models.Properties p in StaticVariables.PropList)
            {
                if(p.ID == PropID)
                {
                    Session["SelectedProperty"] = p.Address1;
                    Session["SelectedPropertyID"] = p.ID;
                }
            }
            

            return View("PropertySummary");
           
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult LogOut()
        {
            Session["SelectedProperty"] = null;
            Session["SelectedPropertyID"] = null;
            Session["CustomerID"] = null;
            Session["CustomerName"] = null;

            Models.LoginViewModel loginViewModel = new Models.LoginViewModel();


            return View("../Account/Login", loginViewModel);
        }
    }
}