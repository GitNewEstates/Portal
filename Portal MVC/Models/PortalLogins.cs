using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using dbConn;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Portal_MVC.Models
{
    class PortalLogins
    {
        public int CustomerID { get; set; }
        public int CustomerTypeID { get; set; }
        public string customerName { get; set; }
        public bool LoginVerified { get; set; }
        public bool IsDirector { get; set; }
        public int UserType { get; set; }
        


        public static PortalLogins Login(object username, string password)
        {
            
           

            PortalLogins r = new PortalLogins();
            try
            {
                
                string pcheck = "";
                string salt = "";
                int UserID = 0;
                int UserType = 0;
                //check email and password
                string q = "select PWord, salt, customerid, UserType from core.customerportal where username = '" +
                        username.ToString() + "'";
                dbConnection db = new dbConnection();
                DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

                if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
                {
                    pcheck = dt.Rows[0][0].ToString();
                    salt = dt.Rows[0][1].ToString();
                    int.TryParse(dt.Rows[0][2].ToString(), out UserID);
                    int.TryParse(dt.Rows[0][3].ToString(), out UserType);
                }
                if (pcheck == BCrypt.Net.BCrypt.HashPassword(password, salt))
                {
                    r.LoginVerified = true;
                    r.UserType = UserType;
                    r.CustomerID = UserID;

                    //user id 1 = customer
                    //user id 2 = NEM User

                    string nameQ = "";
                    //get name
                    if (UserType == 1)
                    {
                        nameQ = "select concat(title, ' ', firstname, ' ', surname) from core.Leaseholders " +
                            " where id = " + r.CustomerID.ToString();
                    } else if (UserType == 2)
                    {
                        nameQ = "select concat(firstname, ' ', surname) from users.users where id = " + r.CustomerID.ToString();
                    }

                    DataTable namedb = db.GetDataTable(GlobalVariables.CS, nameQ);
                    if(namedb.Rows.Count > 0 && namedb.Rows[0][0].ToString() != "Error")
                    {
                        r.customerName = namedb.Rows[0][0].ToString();
                    }
                }
            }
            catch { }
            return r;
        }

        public static bool IsPasswordCorrect(string password, int CustomerID, int usertype)
        {
            bool r = false;
            try
            {
                List<string> ColNames = new List<string>();
                ColNames.Add("core.CustomerPortal.PWord");

                string q = "Select pword, salt from core.customerportal where customerID = " 
                    + CustomerID.ToString() + " and usertype = " + usertype.ToString();

                dbConnection db = new dbConnection();
                DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

                string pword = "";
                string salt = "";

                if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
                {
                    pword = dt.Rows[0][0].ToString();
                    salt = dt.Rows[0][0].ToString();
                }

                if (pword == BCrypt.Net.BCrypt.HashPassword(password, salt))
                {
                    r = true;
                }
            } catch { }
            return r;

        }

        public static List<string> GetPasswordandSalt(int customerID)
        {
            string q = "select PWord, Salt from core.customerportal where customerID = " + customerID.ToString();


            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

            List<string> r = new List<string>();
            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                r.Add(dt.Rows[0][0].ToString());
                r.Add(dt.Rows[0][1].ToString());
            }

            return r;
        }

        public static bool ChangePassword(string Password, int UserID, int usertype)
        {
            bool r;

            List<string> c = new List<string>();
            List<string> p = new List<string>();
            List<object> v = new List<object>();

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string password = BCrypt.Net.BCrypt.HashPassword(Password, salt);

            
            c.Add("PWord");
            c.Add("salt");

            
            p.Add("@PWord");
            p.Add("@salt");

          
            v.Add(password);
            v.Add(salt);

            dbConnection db = new dbConnection();
            DataTable dt = db.UpdateCommand(GlobalVariables.CS, "Core.CustomerPortal", c, p, v, 
                "where customerID = " + UserID.ToString() + " and UserType  = " + usertype.ToString());

            if (dt.Rows[0][0].ToString() == "Error")
            {
                r = false;
            }
            else
            {
                UsageData.InsertNewUsage(2, UserID);
                r = true;
            }

            return r;
        }


    }

    public class User : IdentityUser
    {

    }

    
}