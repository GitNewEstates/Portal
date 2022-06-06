using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Portal_MVC.Models
{
    public static class Utils
    {

        public static string SetAddressBlock(string add1, string add2, string add3, string add4, string add5)
        {
            string r = "";
            if (!string.IsNullOrWhiteSpace(add1))
            {
                r = add1;
            }
            if (!string.IsNullOrWhiteSpace(add2))
            {
                r += '\r' + add2;
            }
            if (!string.IsNullOrWhiteSpace(add3))
            {
                r += '\r' + add3;
            }
            if (!string.IsNullOrWhiteSpace(add4))
            {
                r += '\r' + add4;
            }
            if (!string.IsNullOrWhiteSpace(add5))
            {
                r += '\r' + add5.ToUpper();
            }

            //string r = "";
            //if (!string.IsNullOrWhiteSpace(add1))
            //{
            //    r = add1;
            //}
            //if (!string.IsNullOrWhiteSpace(add2))
            //{
            //    r +=  add2;
            //}
            //if (!string.IsNullOrWhiteSpace(add3))
            //{
            //    r +=  add3;
            //}
            //if (!string.IsNullOrWhiteSpace(add4))
            //{
            //    r +=  add4;
            //}
            //if (!string.IsNullOrWhiteSpace(add5))
            //{
            //    r +=  add5.ToUpper();
            //}

            return r;
        }

        public static string DateTimeFormat(DateTime Date)
        {
            if (!string.IsNullOrWhiteSpace(Date.ToString()))
            {
                try
                {
                    return Date.ToString("dd/MM/yyyy HH:mm:ss");
                }
                catch (Exception ex)
                {
                    return Date.ToString();
                }


            }
            else
            {
                return Date.ToString();
            }
        }

        public static string DateFormat(DateTime Date)
        {
            if (!string.IsNullOrWhiteSpace(Date.ToString()))
            {
                try
                {
                    return Date.ToString("dd/MM/yyyy");
                }
                catch (Exception ex)
                {
                    return Date.ToString();
                }


            }
            else
            {
                return Date.ToString();
            }
        }

        public static string DateFormatLong(DateTime Date)
        {
            if (!string.IsNullOrWhiteSpace(Date.ToString()))
            {
                try
                {
                    int day = Date.Day;
                    string suf = "";
                    switch (day)
                    {
                        case 1:
                        case 21:
                        case 31:

                            suf = "st";
                            break;
                        case 2:
                        case 22:
                            suf = "nd";
                            break;
                        case 3:
                        case 23:
                            suf = "rd";
                            break;
                        default:
                            suf = "th";
                            break;


                    }

                    string month = Date.Month.ToString();
                    string year = Date.Year.ToString();



                    return day.ToString() + suf + " " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Date.Month) + " " + year;
                }
                catch (Exception ex)
                {
                    return Date.ToString();
                }


            }
            else
            {
                return Date.ToString();
            }
        }
    }
}