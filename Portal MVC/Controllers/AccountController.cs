using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Portal_MVC.Models;

namespace Portal_MVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;

        public AccountController()
        {
           

        }

        private ActionResult RedirectToHome()
        {
            return View("Login", this);
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext dbContext )
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _context = dbContext;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task< ActionResult> Login(LoginViewModel model, string returnUrl,string forwardurl = "")
        {
            

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            Models.ConfirmEmailViewModel ConfEmailvm = new ConfirmEmailViewModel();
            switch (result)
            {
                
                case SignInStatus.Success:
                    //get the user
                    var user = await UserManager.FindByEmailAsync(model.Email);

                    //test if email confirmed
                    var IsEmailConfirmed = 
                        await UserManager.IsEmailConfirmedAsync(user.Id);

                    if (!IsEmailConfirmed)
                    {

                        ConfEmailvm.Email = user.Email;
                        return View("ConfirmationEmailSent", ConfEmailvm);
                    } else
                    {

                        //test if terms agreed
                        bool TermsAgreed = await IsTermsAgreed(user.Id);
                        if (!TermsAgreed)
                        {
                            TermsAgreedViewModel termModel = new TermsAgreedViewModel(user.Email);
                            await termModel.GetTermsAsync();
                            return View("Terms", termModel);
                        }

                      
                        //if not matched to a role then redirect
                        if (!await SetRoles(user.Id, user.Email))
                        {
                            //email adminstrators
                            await EmailAdminstratorsWithUnmatchedUsers(user.Email);
                            
                            //display no role set
                            return View("NoRoleSet");
                        }
                    
                    }

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        private async Task<bool> SetRoles(string userID, string _email)
        {
            //get roles
            IList<string> role = await
                UserManager.GetRolesAsync(userID);

            bool MatchedToRole =
                await MatchRoleToUser(role, _email);

            return MatchedToRole;
        } 

        private async Task<bool> MatchRoleToUser(IList<string> Roles, string UserEmail)
        {
            //this will set the session viarables of the user. Where that data is drawn from 
            //will depend on the role of the logged in user. 
            if(Roles.Count > 0)
            {
                string role = Roles[0];

                if(role == "Customer" || role == "Client")
                {
                    //get the customer ID and Name
                    Owner owner =
                        await OwnerMethods.GetOwnerByEmail(UserEmail);

                    Session["CustomerID"] = owner.id;
                    Session["CustomerName"] = owner.FullName;
                    Session["UserType"] = 1;

                    return true;
                } else if (role =="Administrator" || role == "Manager" || role == "Property Manager")
                {
                    APIUser user = await UserMethods.GetUserByEmail(UserEmail);
                    if (user.id > 0)
                    {
                        Session["CustomerID"] = user.id;
                        Session["CustomerName"] = user.FullName;
                        Session["UserType"] = 2;
                        return true;
                    } else
                    {
                        return false;
                    }
                }

              
            }

            return false;
        }

        private async Task<List<string>> GetAdminEmails()
        {
            GlobalVariables.CS = 
                WebConfigurationManager.ConnectionStrings["AccessConnection"].ConnectionString;

            string q = "select Email from AspNetUsers " +
                       "inner join AspNetUserRoles on AspNetUsers.Id = AspNetUserRoles.UserId " +
                        "inner join AspNetRoles on AspNetUserRoles.RoleId = AspNetRoles.id " +
                        "where AspNetRoles.Name = 'Administrator'";

          

            DataTable dt =
                await GlobalVariables.GetConnection().Connection.GetDataTableAsync(q);


            GlobalVariables.CS =
                WebConfigurationManager.ConnectionStrings["DeployConnection"].ConnectionString;


            List<string> emails = new List<string>();
            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach(DataRow dr in dt.Rows)
                {
                    emails.Add(dr[0].ToString());
                }
            }

            return emails;
        }

        private async Task EmailAdminstratorsWithUnmatchedUsers(string email)
        {
            //email the admins if someone has registered but their email address cannot be found

            string content = $"A user with email {email} has tried to register for the Portal. Their email address is not matched " +
                "within our Database. Please investigate and take the appropriate action";

            List<string> To = await GetAdminEmails();
            //send email
            Models.MailService mail = new MailService("Unvalidated Portal Registration",
                content, content, GlobalVariables.GetConnection(), 0, To);
            mail = await Models.MailServiceMethods.SendMailAPI(mail);
            if (mail.APIError != null)
            {
                if (mail.APIError.HasError)
                {
                    //error occurred
                }
            }

        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                var description = model.Description;
                 
                if (result.Succeeded)
                {
                    await SendConfirmationEmail(user.Email);

                    
                    var newuser = await UserManager.FindByEmailAsync(user.Email);

                    if (newuser != null)
                    {
                        if (!string.IsNullOrWhiteSpace(newuser.Id))
                        {
                            await InsertName(model.Name, newuser.Id, SetSignupType(model.Description));
                        }
                    }

                    //await mail.SendGridSend();
                    //string sent= mail.sentStatus.ToString();

                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    Models.ConfirmEmailViewModel vm = new ConfirmEmailViewModel();
                    vm.Email = user.Email;
                    return View("ConfirmationEmailSent", vm);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private string SetSignupType(string Phrase)
        {
            switch (Phrase)
            {
                case "I am a Director of a RMC or RTM":
                    return "Client";
                case "I am the Freeholder of a building managed by New Estate Management":
                    return "Freeholder";
                case "I own a property in a building or estate managed by New Estate Management":
                    return "Customer";
                case "I am the Tenant of a property within a block or estate managed by New Estate Management":
                    return "Tenant";
                case "I am a supplier to New Estate Management":
                    return "Supplier";
                case "I work for New Estate Management":
                    return "Colleague";
                    
                
            }

            return "";
        }

        private async Task InsertName(string name, string userid, string SignUpType)
        {
            List<string> c = new List<string>();
            List<string> p = new List<string>();
            List<object> o = new List<object>();

            c.Add("userid");
            c.Add("name");
            c.Add("signuptype");

            p.Add("@userid");
            p.Add("@name");
            p.Add("@SignUpType");

            o.Add(userid);
            o.Add(name);
            o.Add(SignUpType);

            Models.GlobalVariables.CS = ConfigurationManager.ConnectionStrings["AccessConnection"].ConnectionString;
            DataTable dt = 
                await GlobalVariables.GetConnection().Connection.InsertCommandAsync("UsersNames", c, p, o);

            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() == "Error")
            {
                //Error
            }

            Models.GlobalVariables.CS = ConfigurationManager.ConnectionStrings["DeployConnection"].ConnectionString;

        }

        private async Task SendConfirmationEmail(string email)
        {
            //get the new user ID
            var NewUser = await UserManager.FindByEmailAsync(email);

            string ConfirmationToken =
                await UserManager.GenerateEmailConfirmationTokenAsync(NewUser.Id);


            string url = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority +
                Url.Action("ConfirmEmail", "Account", new { userid = NewUser.Id, code = ConfirmationToken });
            List<string> To = new List<string> { NewUser.Email };
            //send email
            Models.MailService mail = new MailService("Email Confirmation",
                url, url, GlobalVariables.GetConnection(), 0, To);
            mail = await Models.MailServiceMethods.SendMailAPI(mail);
            if (mail.APIError != null)
            {
                if (mail.APIError.HasError)
                {
                    //error occurred
                }
            }
        }

        [HttpPost]
        public async Task<ContentResult>ReSendConfirmationEmail()
        {

            string email = User.Identity.GetUserName();

            if (!UserManager.IsEmailConfirmed(email)){

                await SendConfirmationEmail(email);
                return Content("Success");
            } else
            {
                return Content("ConfirmedAlready");
            }
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                string SignUpType = await GetSignUpType(userId);
                //set roles
                var user = await UserManager.FindByIdAsync(userId);
                bool isRoleSet = 
                    await SetUserRoles(user.Email, userId, SignUpType);

                if (!isRoleSet)
                {
                    //email admins
                    await EmailAdminstratorsWithUnmatchedUsers(user.Email);

                    //send to error view
                    return View("Unsuccessfulverification");
                }

            }
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        private async Task<bool> SetUserRoles(string email, string id, string signuptype)
        {
            //returns true if role set and false if role cannot be set

            switch (signuptype)
            {
                case "Customer":
                    return await SetCustomerRole(email, id);
                    
                case "Client":
                   return await SetClientRole(email, id);
                    
                case "Tenant":

                    break;
                case "Freeholder":

                    break;
                case "Supplier":

                    break;
                case "Colleague":
                    return await SetColleagueRole(email, id);
                    
            }


            return false;

        }

        private async Task<bool> SetCustomerRole(string email, string id)
        {
            //test if email present in customer account
            Owner owner = await OwnerMethods.GetOwnerByEmail(email);

            if (owner.id > 0)
            {
                //assign to Customer Role
                await UserManager.AddToRoleAsync(id, "Customer");
                return true;
            }
            return false; 
        }

        private async Task<bool> SetColleagueRole(string email, string id)
        {
            //Test if NEM Employee
            Models.APIUser nemuser
                = await UserMethods.GetUserByEmail(email);
            if (nemuser != null)
            {
                if (!string.IsNullOrWhiteSpace(nemuser.Role.Name))
                {
                    switch (nemuser.Role.Name)
                    {
                        case "Director":

                            await UserManager.AddToRoleAsync(id, "Administrator");

                            break;
                        case "Property Manager":
                            await UserManager.AddToRoleAsync(id, "Property Manager");
                            break;
                        case "Manager":
                            await UserManager.AddToRoleAsync(id, "Manager");
                            break;
                        case "Maintenance Operative":
                            await UserManager.AddToRoleAsync(id, "Maintenance Operative");
                            break;

                    }

                    return true;
                }
            }

            return false;
        }

        private async Task<bool> SetClientRole(string email, string id)
        {
            string json = await APIMethods.CallAPIGetEndPointAsync($"IsEstateClient/{email}");
          
            if(json == "true")
            {
                await UserManager.AddToRoleAsync(id, "Client");
                return true;
            }

            return false;

        }
        private async Task<string> GetSignUpType(string userid)
        {
            string q = $"select signuptype from UsersNames where userid  = '{userid}'";
            Models.GlobalVariables.CS = ConfigurationManager.ConnectionStrings["AccessConnection"].ConnectionString;
            DataTable dt = await GlobalVariables.GetConnection().Connection.GetDataTableAsync(q);
            string r = "";

            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                r = dt.Rows[0][0].ToString();
            }

            Models.GlobalVariables.CS = ConfigurationManager.ConnectionStrings["DeployConnection"].ConnectionString;

            return r;
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ForgotPasswordViewModel model = new ForgotPasswordViewModel();
            return View(model);
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);


                string url = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority +
                Url.Action("ResetPassword", "Account", new { userid = user.Id, code = code });

                string content = "Please reset your password by clicking <a href=\"" + url + "\">here</a>";

                List<string> To = new List<string> { user.Email };
                //send email
                Models.MailService mail = new MailService("Reset Password",
                    url, url, GlobalVariables.GetConnection(), 0, To);
                mail = await Models.MailServiceMethods.SendMailAPI(mail);
                if (mail.APIError != null)
                {
                    if (mail.APIError.HasError)
                    {
                        //error occurred
                    }
                }

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel();
            if (!string.IsNullOrEmpty(code))
            {
                resetPasswordViewModel.Code = code;
            }
            return code == null ? View("Error") : View(resetPasswordViewModel);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        public async Task<ActionResult> Terms()
        {
            TermsAgreedViewModel model = new TermsAgreedViewModel("");
            await model.GetTermsAsync();
            return View(model);
        }

        [HttpPost]
        public async Task< ActionResult> Terms(TermsAgreedViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                bool AddModelErrors = false;
                if (model.TermsList != null)
                {
                    if (model.TermsList.Count > 0)
                    {
                        if(model.TermsList.Count == 2)
                        {
                            AddModelErrors = true;
                            ModelState.AddModelError(string.Empty, "Cannot only select Accept or Decline.");
                        } else
                        {
                            string h = model.TermsList[0];
                            // process accet or decline
                            if (model.TermsList[0] == "Agreed")
                            {
                                model.TermsAgreed = true;
                            } else
                            {
                                model.TermsAgreed = false;
                            }


                           await model.InsertTermsAgreedAsync(user.Id);

                            if (model.TermsAgreed)
                            {
                                //send to account
                                //if not matched to a role then redirect
                                if (!await SetRoles(user.Id, user.Email))
                                {
                                    
                                    //should not get here becuase cannot agree to terms if no role is set.
                                }

                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                AddModelErrors = true;
                                ModelState.AddModelError(string.Empty, "Terms and Conditions must be accepted to contine;");
                            }
                        }
                    } else
                    {
                        AddModelErrors = true;
                        ModelState.AddModelError(string.Empty, "Must Accept or Decline Terms and Conditions");
                    }
                } else
                {
                    AddModelErrors=true;
                    ModelState.AddModelError(string.Empty, "Error occurred retrieving the Accept or Decline Value. Values collection is null");
                }

                if (AddModelErrors)
                {
                    return View(model);
                }


            } else
            {
                // return errors
                return View(model);
            }
            return View(model);
        }

        private async Task<bool> IsTermsAgreed(string userID)
        {
            string q = $"select IsAgreed from Terms where userid = '{userID}'";
            GlobalVariables.CS =
                WebConfigurationManager.ConnectionStrings["AccessConnection"].ConnectionString;

            DataTable dt = await GlobalVariables.GetConnection().Connection.GetDataTableAsync(q);
            bool Agreed = false;
            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                string IsAgreed = dt.Rows[0][0].ToString();
                bool.TryParse(IsAgreed, out Agreed);
            }

            GlobalVariables.CS =
                WebConfigurationManager.ConnectionStrings["DeployConnection"].ConnectionString;

            return Agreed;

        }

       

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }

  
}