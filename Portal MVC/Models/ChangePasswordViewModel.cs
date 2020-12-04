using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal_MVC.Models
{
    public class MyChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Your Current Password is Required.")]
        public string OldPass { get; set; }

        //[Compare("OldPassword", ErrorMessage = "You Old Password is incorrect.")]
        public string OldPasswordStatic { get; set; }


        [Required(ErrorMessage = "Your New Password is Required.")]
        [DataType(DataType.Password)]
        public string NewPass { get; set; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirmation of your New Password is Required.")]
        [Compare("NewPass", ErrorMessage = "The passwords entered do not match")]
        public string ConfirmNewPassword { get; set; }

        public string SuccessPasswordUpdateMessage { get; set; }
    }
}