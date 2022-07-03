using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Threading.Tasks;

namespace Portal_MVC.Models
{
    public static class GlobalVariables
    {
        public static int DbConfig { get; set; } = 2; //defaults to 2 being the live database
        public static string CS { get; set; }
        //public static int CustomerID { get; set; }
        //public static string CustomerName { get; set; }
        //public static string SelectedProperty { get; set; }
        //public static int SelectedPropertyID { get; set; }
        //public static List<Models.Properties> PropList { get; set; }
        public static DataTable PieChartDataTable { get; set; }
      //  public static string EstateName { get; set; }

        public static int LogUsage { get; set; } //0 = true 1 = false

        public static APIConnectionObject APIConnection { get; set; }

        public static dbConn.DBConnectionObject GetConnection()
        {
            return new dbConn.DBConnectionObject(dbConn.ConnectionType.SQLDB, 
                GlobalVariables.CS, dbConn.DBType.SQL);
           
        }

        public static string ConvertStringToDate(string date)
        {
            //int.TryParse(date.Substring(0, 2), out int day);
            //int.TryParse(date.Substring(3, 2), out int month);
            //int.TryParse(date.Substring(5, 4), out int year);

            string day = date.Substring(0, 2);
            string month = date.Substring(3, 2);
            string year = date.Substring(6, 4);

            return $"{month}/{day}/{year}";
        }

        public async static Task SendProcessEmail(string ProcessName, List<string> ProcessContent)
        {
            string content = "<div>";

            foreach(string s in ProcessContent)
            {
                content += $"{s}</br>";
            }

            content += "</div>";

            MailServiceDLL.MailService mail = new MailServiceDLL.MailService($"Process Report - {ProcessName}", 
                content, content, GlobalVariables.GetConnection(), 0, new List<string> { "adam.new@newestates.co.uk"}, null, 2);

            await mail.SendGridSend();
        }

    }

    
}