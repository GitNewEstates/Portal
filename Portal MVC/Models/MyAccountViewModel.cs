using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Portal_MVC.Models
{
    public class MyAccountViewModel
    {
        public string PageTitle { get; set;}
        public bool AllowAddressUpdates { get; set; }
        public Models.AccountDetails accountDetails { get; set; }
        //Holds details to test if has been changes
        public Models.AccountDetails StaticaccountDetails { get; set; }
        private string _SuccessfulAddressUpdateMessage = "Your Contact Preferences have been successfully updated.";
        public string SuccessfulAddressUpdateMessage
        {
            get
            {
                return _SuccessfulAddressUpdateMessage;
            }
        }
        public bool SuccessfulAddressUpdate { get; set; }
        public RepairsMaintenanceViewModel RepairVM { get; set; }

        public enum ContactPreferences
        {
            [Display(Name = "Post Only")]
            Post,

            [Display(Name = "Email Only")]
            enail,

            [Display(Name = "Post and Email")]
            postandemail


        }

        public Models.PropertySummaryViewModel PropSummaryViewModel { get; set; }

        public string GuidanceHeader { get; set; }
        public string Guidance { get; set; }

        


    }

    
}