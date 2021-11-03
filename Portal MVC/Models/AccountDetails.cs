using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using dbConn;
using System.Data;
using System.ComponentModel;

namespace Portal_MVC.Models
{
    public class AccountDetails
    {
        [Required(ErrorMessage = "Must have the first line of your address")]
        [MaxLength(30, ErrorMessage = "Cannot be greater than 30 characters")]
        public string ContactAdd1 { get; set; }
        

        [MaxLength(30, ErrorMessage = "Cannot be greater than 30 characters")]
        public string ContactAdd2 { get; set; }

        [MaxLength(30, ErrorMessage = "Cannot be greater than 30 characters")]
        public string ContactAdd3 { get; set; }

        [MaxLength(30, ErrorMessage = "Cannot be greater than 30 characters")]
        public string ContactAdd4 { get; set; }

        [Required(ErrorMessage = "Must have the post code of your address")]
        [MaxLength(10, ErrorMessage = "Cannot be greater than 10 characters")]
        public string ContactAdd5 { get; set; }

        
        [MaxLength(12, ErrorMessage = "Entered phone number format is not valid.")]
        public string phoneNumber { get; set; }

        
        [EmailAddress(ErrorMessage = "Email address is invalid.")]
        [MaxLength(100, ErrorMessage = "Email address must be less that 100 characters")]
        public string Email { get; set; }

        [Display(Name = "Update email for log in?")]
        public bool UpdateLoginEmail { get; set; }

        [Display(Name = "Post Only")]
        public bool PostOnly { get; set; }

        [Display(Name = "Post and Email")]
        public bool PostAndEmail { get; set; }

        [Display(Name = "Email Only")]
        public bool EmailOnly { get; set; }

        public bool UnableToUpdateLoginEmail { get; set; }

        [Display(Name = "Update Account Contact Preferences?")]
        public bool UpdateMainAccountDetails { get; set; }

        [Display(Name = "Apply Contact Preferences to all properties?")]
        public bool UseforAllProperties { get; set; }

        public enum ContactPreferences
        {
            Post,
            email,
            postandemail
           
        }

        public ContactPreferences ContactPref { get; set; }
        public int NumberOfOwnedProperties { get; set; }

        public string updateError { get; set; }
    }

    public static class AccountDetailsMethods
    {
        //From account level
        private static AccountDetails SetContactPreferences(AccountDetails ad, int CustomerID)
        {
            string q = "Select Postonly, emailonly, PostAndEmail from core.LeaseholderContactPreferences where CustomerID = " + CustomerID.ToString();

           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);
            
            //set first to compare
            if(dt.Rows.Count> 0)
            {
                DataRow dr = dt.Rows[0];
                if (dr[0] != DBNull.Value)
                {
                    ad.PostOnly = Convert.ToBoolean(dr[0]);
                    if(ad.PostOnly == true)
                    {
                        ad.ContactPref = AccountDetails.ContactPreferences.Post;
                    }
                    
                }
                if (dr[1] != DBNull.Value)
                {
                    ad.EmailOnly = Convert.ToBoolean(dr[1]);
                    if (ad.EmailOnly == true)
                    {
                        ad.ContactPref = AccountDetails.ContactPreferences.email;
                    }
                }
                if (dr[2] != DBNull.Value)
                {
                    ad.PostAndEmail = Convert.ToBoolean(dr[2]);
                    if (ad.PostAndEmail == true)
                    {
                        ad.ContactPref = AccountDetails.ContactPreferences.postandemail;
                    }
                }
            }

            AccountDetails ad1 = new AccountDetails();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0] != DBNull.Value)
                {
                    ad1.PostOnly = Convert.ToBoolean(dr[0]);
                    if (ad1.PostOnly == true)
                    {
                        ad1.ContactPref = AccountDetails.ContactPreferences.Post;
                    }

                }
                if (dr[1] != DBNull.Value)
                {
                    ad1.EmailOnly = Convert.ToBoolean(dr[1]);
                    if (ad1.EmailOnly == true)
                    {
                        ad1.ContactPref = AccountDetails.ContactPreferences.email;
                    }
                }
                if (dr[2] != DBNull.Value)
                {
                    ad1.PostAndEmail = Convert.ToBoolean(dr[2]);
                    if (ad1.PostAndEmail == true)
                    {
                        ad1.ContactPref = AccountDetails.ContactPreferences.postandemail;
                    }
                }

                if(ad.ContactPref != ad1.ContactPref)
                {
                    
                    ad.EmailOnly = false;
                    ad.PostAndEmail = false;
                    ad.PostOnly = false;
                }
            }

            return ad;
        }

        //from prop level
        private static AccountDetails.ContactPreferences SetContactPreferences(int CustomerID, int PropertyID)
        {
            string q = "Select Postonly, emailonly, PostAndEmail from core.LeaseholderContactPreferences where CustomerID = " + CustomerID.ToString() + " and PropertyID = " + PropertyID.ToString();

           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);
            AccountDetails.ContactPreferences ad = new AccountDetails.ContactPreferences();
            ad = AccountDetails.ContactPreferences.postandemail; //sets default
            //set first to compare
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                if (dr[0] != DBNull.Value)
                {
                    ad = AccountDetails.ContactPreferences.Post;
                }
                if (dr[1] != DBNull.Value)
                {
                    ad = AccountDetails.ContactPreferences.email;
                }
                if (dr[2] != DBNull.Value)
                {
                    ad = AccountDetails.ContactPreferences.postandemail;
                }
            }

            return ad;
        }

        //account details where property is selected
        public static AccountDetails OwnerAccountDetails(int CustomerID, int PropertyID)
        {
            string q = "Select * from core.LeaseholderContactPreferences " +
                        "where core.LeaseholderContactPreferences.CustomerID = " + CustomerID.ToString() + " and UnitID = " + PropertyID.ToString();

            AccountDetails r = new AccountDetails();
            List<AccountDetails> rList = new List<AccountDetails>();
           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);

            foreach (DataRow dr in dt.Rows)
            {

                rList.Add(new AccountDetails { ContactAdd1 = dr[5].ToString(), ContactAdd2 = dr[6].ToString(), ContactAdd3 = dr[7].ToString(), ContactAdd4 = dr[8].ToString(), ContactAdd5 = dr[9].ToString(), phoneNumber = dr[11].ToString() });
                if (dr[10] != DBNull.Value)
                {

                    rList[0].Email = dr[10].ToString();
                }

                if(dr[1] != DBNull.Value)
                {
                    rList[0].PostOnly = Convert.ToBoolean(dr[1]);
                    if (rList[0].PostOnly == true)
                    {
                        rList[0].ContactPref = AccountDetails.ContactPreferences.Post;
                    }
                }

                if (dr[2] != DBNull.Value)
                {
                    rList[0].EmailOnly = Convert.ToBoolean(dr[2]);
                    if (rList[0].EmailOnly == true)
                    {
                        rList[0].ContactPref = AccountDetails.ContactPreferences.email;
                    }
                }

                if (dr[3] != DBNull.Value)
                {
                    rList[0].PostAndEmail = Convert.ToBoolean(dr[3]);
                    if (rList[0].PostAndEmail == true)
                    {
                        rList[0].ContactPref = AccountDetails.ContactPreferences.postandemail;
                    }
                }

               
            }
            if (rList.Count != 0)
            {
                r = rList[0];
            }


            return r;
        }

        //Select Main Account Contact Preferences
        public static AccountDetails OwnerAccountDetails(int CustomerID)
        {
            string q = "Select * from core.Leaseholders " +
                        "where core.Leaseholders.id = " + CustomerID.ToString();

            AccountDetails r = new AccountDetails();
            List<AccountDetails> rList = new List<AccountDetails>();
           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);

            foreach (DataRow dr in dt.Rows)
            {
                
                rList.Add(new AccountDetails {
                    ContactAdd1 = dr[4].ToString(),
                    ContactAdd2 = dr[5].ToString(),
                    ContactAdd3 = dr[6].ToString(),
                    ContactAdd4 = dr[7].ToString(),
                    ContactAdd5 = dr[8].ToString(),
                    phoneNumber = dr[9].ToString(),
                    EmailOnly = Convert.ToBoolean(dr[13]),
                    PostOnly = Convert.ToBoolean(dr[14]),
                    PostAndEmail = Convert.ToBoolean(dr[15]) });

                if (dr[11] != DBNull.Value)
                {
                   
                    rList[0].Email = dr[11].ToString();
                }

               
            }
            if(rList.Count != 0)
            {
                r = rList[0];
            }

            if(r.PostOnly == true)
            {
                r.ContactPref = AccountDetails.ContactPreferences.Post;
            }
            if (r.EmailOnly == true)
            {
                r.ContactPref = AccountDetails.ContactPreferences.email;
            }
            if (r.PostAndEmail == true)
            {
                r.ContactPref = AccountDetails.ContactPreferences.postandemail;
            }

            
            return r;
        }

        //updates mian user account contact preferences
        public static void UpdateUserAccountPreferences(Models.AccountDetails Account, int customerID)
        {
            List<string> colnames = new List<string>();
            colnames.Add("Address_1");
            colnames.Add("Address_2");
            colnames.Add("Address_3");
            colnames.Add("Address_4");
            colnames.Add("Address_5");
            colnames.Add("Email_1");
            colnames.Add("ContactNumber_1");
            colnames.Add("emailonly");
            colnames.Add("postonly");
            colnames.Add("postandemail");

            List<string> PList = new List<string>();
            PList.Add("@Address_1");
            PList.Add("@Address_2");
            PList.Add("@Address_3");
            PList.Add("@Address_4");
            PList.Add("@Address_5");
            PList.Add("@email_1");
            PList.Add("@ContactNumber_1");
            PList.Add("@emailonly");
            PList.Add("@postonly");
            PList.Add("@postandemail");

            List<object> values = new List<object>();
            values.Add(Account.ContactAdd1);
           
            if (Account.ContactAdd2 == null)
            {
                values.Add("");

            }
            else
            {
                values.Add(Account.ContactAdd2);
            }

            if (Account.ContactAdd3 == null)
            {
                values.Add("");

            }
            else
            {
                values.Add(Account.ContactAdd3);
            }


            if (Account.ContactAdd4 == null)
            {
                values.Add("");

            }
            else
            {
                values.Add(Account.ContactAdd4);
            }

            values.Add(Account.ContactAdd5);
            if (Account.Email== null)
            {
                values.Add("");

            } else
            {
                values.Add(Account.Email);
            }

            if (Account.phoneNumber == null)
            {
                values.Add("");

            }
            else
            {
                values.Add(Account.phoneNumber);
            }
            values.Add(Account.EmailOnly);
            values.Add(Account.PostOnly);
            values.Add(Account.PostAndEmail);



           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.UpdateCommand( "core.Leaseholders", colnames, PList, values, " where ID = " + customerID.ToString());
        }

        public static void UpdateContactPreferences(Models.AccountDetails Account, int customerID)
        {

            bool post = false;
            bool email = false;
            bool postandemail = false;

            if(Account.ContactPref == AccountDetails.ContactPreferences.Post)
            {
                post = true;
            }
            if (Account.ContactPref == AccountDetails.ContactPreferences.email)
            {
                email = true;
            }
            if (Account.ContactPref == AccountDetails.ContactPreferences.postandemail)
            {
                postandemail = true;
            }
            List<string> colnames = new List<string>();
            colnames.Add("PostOnly");
            colnames.Add("Emailonly");
            colnames.Add("PostandEmail");
          
            List<string> PList = new List<string>();
            PList.Add("@PostOnly");
            PList.Add("@Emailonly");
            PList.Add("@PostandEmail");

            List<object> values = new List<object>();
            values.Add(post);
            values.Add(email);
            values.Add(postandemail);
           

           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.UpdateCommand( "core.LeaseholderContactPreferences", colnames, PList, values, " where customerID = " + customerID.ToString());
        }
        
        public static bool CompareAccountDetails(Models.MyAccountViewModel mv)
        {
            //returns true if same false if different
            bool r = true;

            if(mv.accountDetails.EmailOnly != mv.StaticaccountDetails.EmailOnly)
            {
                return false;
            }

            if (mv.accountDetails.PostOnly != mv.StaticaccountDetails.PostOnly)
            {
                return false;
            }

            if (mv.accountDetails.PostAndEmail != mv.StaticaccountDetails.PostAndEmail)
            {
                return false;
            }
            Models.AccountDetails NewAccount = mv.accountDetails;
            Models.AccountDetails OldAccount = mv.StaticaccountDetails;


            if (OldAccount.ContactAdd1 != NewAccount.ContactAdd1)
            {
                return false;
            }
            if (OldAccount.ContactAdd2 != NewAccount.ContactAdd2)
            {
                return false;
            }
            if (OldAccount.ContactAdd3 != NewAccount.ContactAdd3)
            {
                return false;
            }
            if (OldAccount.ContactAdd4 != NewAccount.ContactAdd4)
            {
                return false;
            }
            if (OldAccount.ContactAdd5 != NewAccount.ContactAdd5)
            {
                return false;
            }
            if (OldAccount.phoneNumber != NewAccount.phoneNumber)
            {
                return false;
            }
            if (OldAccount.Email != NewAccount.Email)
            {
                return false;
            }


            return r;
        }

        public static bool TestIFPreferencesChanged(AccountDetails NewDetails, AccountDetails OldAccount)
        {
            //returns flase if no change
            bool r = false;

            AccountDetails.ContactPreferences c = new AccountDetails.ContactPreferences();
            if(OldAccount.EmailOnly == true)
            {
                c = AccountDetails.ContactPreferences.email;
            }

            if (OldAccount.PostOnly == true)
            {
                c = AccountDetails.ContactPreferences.Post;
            }
            if (OldAccount.PostAndEmail == true)
            {
                c = AccountDetails.ContactPreferences.postandemail;
            }

            if(c != NewDetails.ContactPref)
            {
                r = true;
            }
            return r;
        }

        public static void UpdateLoginEmail(string Email, int CustomerID)
        {
            List<string> colnames = new List<string>();
            colnames.Add("Username");
          
            List<string> PList = new List<string>();
            PList.Add("@Username");
            
            List<object> values = new List<object>();
            values.Add(Email);

           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.UpdateCommand( "core.CustomerPortal", colnames, PList, values, " where customerID = " + CustomerID.ToString());

        }

        public static int NumberOfOwnedProperties(int CustomerID)
        {
            string q = "select count(unitID) from core.PropertyOwnership where OwnerID = " + CustomerID.ToString();

           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);
            int i = 0;
            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                int.TryParse(dt.Rows[0][0].ToString(), out i);
            }
            return i;
        }

        //for updating single property contact preferences
        public static void UpdatePropertyAccountPreferences(Models.AccountDetails Account, int customerID, int propertyID = 0)
        {
            //if PropertyID = 0 then will update contact prefs for all properties. 

            bool post = false;
            bool email = false;
            bool postandemail = false;

            if (Account.ContactPref == AccountDetails.ContactPreferences.Post)
            {
                post = true;
            }
            if (Account.ContactPref == AccountDetails.ContactPreferences.email)
            {
                email = true;
            }
            if (Account.ContactPref == AccountDetails.ContactPreferences.postandemail)
            {
                postandemail = true;
            }

            List<string> colnames = new List<string>();
            colnames.Add("PostOnly");
            colnames.Add("Emailonly");
            colnames.Add("PostandEmail");
            colnames.Add("Address_1");
            colnames.Add("Address_2");
            colnames.Add("Address_3");
            colnames.Add("Address_4");
            colnames.Add("Address_5");
            colnames.Add("email");
            colnames.Add("contactnumer");

            List<string> PList = new List<string>();
            PList.Add("@PostOnly");
            PList.Add("@Emailonly");
            PList.Add("@PostandEmail");
            PList.Add("@Address_1");
            PList.Add("@Address_2");
            PList.Add("@Address_3");
            PList.Add("@Address_4");
            PList.Add("@Address_5");
            PList.Add("@email");
            PList.Add("@contactnumer");

            List<object> values = new List<object>();
            values.Add(post);
            values.Add(email);
            values.Add(postandemail);
            values.Add(Account.ContactAdd1);
            if (Account.ContactAdd2 == null)
            {
                values.Add("");

            }
            else
            {
                values.Add(Account.ContactAdd2);
            }

            if (Account.ContactAdd3 == null)
            {
                values.Add("");

            }
            else
            {
                values.Add(Account.ContactAdd3);
            }
            if (Account.ContactAdd4 == null)
            {
                values.Add("");

            }
            else
            {
                values.Add(Account.ContactAdd4);
            }
            values.Add(Account.ContactAdd5);
            if (Account.Email == null)
            {
                values.Add("");

            }
            else
            {
                values.Add(Account.Email);
            }
            if (Account.phoneNumber == null)
            {
                values.Add("");

            }
            else
            {
                values.Add(Account.phoneNumber);
            }


           DBConnectionObject db = GlobalVariables.GetConnection();
            if(propertyID > 0)
            {
                DataTable dt = db.Connection.UpdateCommand( "core.LeaseholderContactPreferences", colnames, PList, values, " where customerID = " + customerID.ToString() + " and unitId = " + propertyID.ToString());
            } else
            {
                DataTable dt = db.Connection.UpdateCommand( "core.LeaseholderContactPreferences", colnames, PList, values, " where customerID = " + customerID.ToString());
            }
            

        }

        //For updating an owner's properties contact preferences
        //public static void UpdateAllOwnersPropertyAccountPreferences(Models.AccountDetails Account, int customerID)
        //{
        //    bool post = false;
        //    bool email = false;
        //    bool postandemail = false;

        //    if (Account.ContactPref == AccountDetails.ContactPreferences.Post)
        //    {
        //        post = true;
        //    }
        //    if (Account.ContactPref == AccountDetails.ContactPreferences.email)
        //    {
        //        email = true;
        //    }
        //    if (Account.ContactPref == AccountDetails.ContactPreferences.postandemail)
        //    {
        //        postandemail = true;
        //    }

        //    List<string> colnames = new List<string>();
        //    colnames.Add("PostOnly");
        //    colnames.Add("Emailonly");
        //    colnames.Add("PostandEmail");
        //    colnames.Add("Address_1");
        //    colnames.Add("Address_2");
        //    colnames.Add("Address_3");
        //    colnames.Add("Address_4");
        //    colnames.Add("Address_5");
        //    colnames.Add("email");
        //    colnames.Add("contactnumer");

        //    List<string> PList = new List<string>();
        //    PList.Add("@PostOnly");
        //    PList.Add("@Emailonly");
        //    PList.Add("@PostandEmail");
        //    PList.Add("@Address_1");
        //    PList.Add("@Address_2");
        //    PList.Add("@Address_3");
        //    PList.Add("@Address_4");
        //    PList.Add("@Address_5");
        //    PList.Add("@email");
        //    PList.Add("@contactnumer");

        //    List<object> values = new List<object>();
        //    values.Add(post);
        //    values.Add(email);
        //    values.Add(postandemail);
        //    values.Add(Account.ContactAdd1);

        //    if (Account.ContactAdd2 == null)
        //    {
        //        values.Add("");

        //    }
        //    else
        //    {
        //        values.Add(Account.ContactAdd2);
        //    }

        //    if (Account.ContactAdd3 == null)
        //    {
        //        values.Add("");

        //    }
        //    else
        //    {
        //        values.Add(Account.ContactAdd3);
        //    }
        //    if (Account.ContactAdd4 == null)
        //    {
        //        values.Add("");

        //    }
        //    else
        //    {
        //        values.Add(Account.ContactAdd4);
        //    }
        //    values.Add(Account.ContactAdd5);
        //    if (Account.Email == null)
        //    {
        //        values.Add("");

        //    }
        //    else
        //    {
        //        values.Add(Account.Email);
        //    }
        //    if (Account.phoneNumber == null)
        //    {
        //        values.Add("");

        //    }
        //    else
        //    {
        //        values.Add(Account.phoneNumber);
        //    }


        //   DBConnectionObject db = GlobalVariables.GetConnection();
        //    DataTable dt = db.UpdateCommand( "core.LeaseholderContactPreferences", colnames, PList, values, " where customerID = " + customerID.ToString());

        //}

    }
}