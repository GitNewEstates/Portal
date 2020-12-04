using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Portal_MVC.Models
{
    public static class Controls
    {
        public static string CurrencyString(double Value)
        {
            string r = Value.ToString("C", new CultureInfo("en-GB"));
            return r;
        }

        public static string DateString(DateTime Value)
        {
            string r = string.Format("{0:dd/MM/yyyy}", Value);
            return r;
        }
    }
}