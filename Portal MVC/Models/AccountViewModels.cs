using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Portal_MVC.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string forwardurl { get; set; } = "";
    }

    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            DescriptionList = new List<string>
            {
                "I am a Director of a RMC or RTM",
                "I am the Freeholder of a building managed by New Estate Management",
                "I own a property in a building or estate managed by New Estate Management",
                "I am the Tenant of a property within a block or estate managed by New Estate Management",
                "I am a supplier to New Estate Management",
                "I work for New Estate Management"
            };
        }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public List<string> DescriptionList { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
    }

    public class ConfirmEmailViewModel
    {
        public string Email { get; set; }   
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class TermsAgreedViewModel
    {
        public TermsAgreedViewModel()
        {

        }
        public TermsAgreedViewModel(string _email)
        {
            TermsList = new List<string>();
            Email = _email;
          //  TermsAgreed = true;
        }

        public string Email { get; set;}

        public List<string> TermsList { get; set; }
  
        public bool TermsAgreed { get; set; }

    
        public string Terms { get; set; }

        public async Task GetTermsAsync()
        {
            string q = $"select terms from TermsText";
            GlobalVariables.CS =
                WebConfigurationManager.ConnectionStrings["AccessConnection"].ConnectionString;

            DataTable dt = await GlobalVariables.GetConnection().Connection.GetDataTableAsync(q);
          
            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                Terms = dt.Rows[0][0].ToString();
            }

            GlobalVariables.CS =
                WebConfigurationManager.ConnectionStrings["DeployConnection"].ConnectionString;

           
        }

        public async Task InsertTermsAgreedAsync(string userid)
        {
            GlobalVariables.CS =
               WebConfigurationManager.ConnectionStrings["AccessConnection"].ConnectionString;
            //delete first
            string cmd = $"delete from terms where userid = '{userid}'";

            await GlobalVariables.GetConnection().Connection.ExecuteCommandAsync(cmd);



            List<string> c = new List<string>();
            List<string> p = new List<string>();
            List<object> o = new List<object>();

        

            c.Add("UserID");
            c.Add("date");
            c.Add("IsAgreed");

            p.Add("@userid");
            p.Add("@date");
            p.Add("@isagreed");

            o.Add(userid);
            o.Add(DateTime.Now);
            o.Add(TermsAgreed);

            DataTable dt = await GlobalVariables.GetConnection().Connection.InsertCommandWithReturnIDAsync("terms", c, p, o);

            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "error")
            {
                //int.TryParse(dt.Rows[0][0].ToString(), out int id);
            }
            else
            {
                //error
            }

            GlobalVariables.CS =
                WebConfigurationManager.ConnectionStrings["DeployConnection"].ConnectionString;
        }
    }
}
