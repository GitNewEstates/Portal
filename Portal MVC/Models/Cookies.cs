using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Portal_MVC.Models
{
    public  class CookieMethods
    {
        public static bool TestIfCookieEnabled()
        {
            //write cookie
            HttpCookie cookie = new HttpCookie("TestCookie");
            cookie["IsEnabled"] = "true";

           
            return true;
            

        }
    }
}