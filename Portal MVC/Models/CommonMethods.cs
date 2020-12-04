using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_MVC.Models
{
    public static class CommonMethods
    {
        public static string SetNameString(string Title, string FName, string sName)
        {
            string name = "";

            if(Title != "")
            {
                name = Title + " ";
            }

            if (FName != "")
            {
                name = name + FName + " ";
            }

            if (Title != "")
            {
                name = name + sName;
            }
            return name;
        }
    }
}