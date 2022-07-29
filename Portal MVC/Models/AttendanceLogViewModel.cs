using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data;
using System.Collections.ObjectModel;

namespace Portal_MVC.Models
{
    public class AttendanceLogViewModel
    {
        public AttendanceLogViewModel()
        {
            //AttendanceObj = new AttendanceVisits.AttendanceVisits();
            MaxDate = DateTime.Now;
            VisitDate = DateTime.Now;
            SendCustomerNotification = true;
            PortalViewable = true;
            AttendanceObj = new AttendanceVisits();


        }

        public AttendanceVisits AttendanceObj { get; set; }
        public List<SelectListItem> EstateList { get; set; }
        public int SelectedPropertyid { get; set; }
        public List<SelectListItem> AttendanceTypes { get; set; }
        public int SelectedAttendanceTypeID { get; set; }
       // public AttendanceVisits.AttendanceVisits AttendanceObj { get; set; }

        public string ImageUrls { get; set; }

        [Required(ErrorMessage = "Description of the Attendance is required.")]
        public string AttendanceDescription { get; set; }

        [Required(ErrorMessage = "Date and Time of the visit is required.")]
        public DateTime VisitDate { get; set; }

        public DateTime MaxDate { get; set; }

        public bool FireAlarm { get; set; }

        public bool SendCustomerNotification { get; set; }

        public bool PortalViewable { get; set; }

        public bool CaretakingVisit { get; set; }
        public bool InspectionVisit { get; set; }
        public bool CallOutVisit { get; set; }
        
        public void SetLists()
        {
            //List<Properties> estates = Models.PropertyMethods.GetAllEstates();
            //EstateList = new List<SelectListItem>();
            //foreach (Properties p in estates)
            //{
            //    bool IsSelected = false;
            //    if(p.ID == SelectedPropertyid && SelectedPropertyid > 0)
            //    {
            //        IsSelected = true;
            //    }

            //    EstateList.Add(new SelectListItem
            //    {
            //        Text = p.Address1,
            //        Value = p.ID.ToString(),
            //        Selected = IsSelected
            //    });
            //}

            //List<AttendanceType> types =
            //    AttendanceTypeMethods.AllAttendanceTypesList(GlobalVariables.GetConnection());
            //AttendanceTypes = new List<SelectListItem>();

            //foreach (AttendanceType t in types)
            //{
            //    bool IsSelected = false;
            //    if (t.id == SelectedAttendanceTypeID && SelectedAttendanceTypeID > 0)
            //    {
            //        IsSelected = true;
            //    }

            //    AttendanceTypes.Add(new SelectListItem
            //    {
            //        Text = t.Name,
            //        Value = t.id.ToString(),
            //        Selected = IsSelected
            //    });
            //}

        }
        
        //public void Insert()
        //{
        //    AttendanceObj.AttendanceTypeID = SelectedAttendanceTypeID;
        //    AttendanceObj.EstateID = SelectedPropertyid;
        //    if (!string.IsNullOrWhiteSpace(ImageUrls))
        //    {
        //        AttendanceObj.urlString = ImageUrls.Trim();
        //    }
        //    AttendanceObj.VisitDate = VisitDate;
        //    AttendanceObj.VisitDescription = AttendanceDescription;
        //    AttendanceObj.FireAlarmTest = FireAlarm;
        //    AttendanceObj.NotifyCustomer = SendCustomerNotification;
        //    AttendanceObj.PortalViewable = PortalViewable;

        //    AttendanceObj.Insert(GlobalVariables.GetConnection());

        //    //if (AttendanceObj.id > 0)
        //    //{
        //    //    try
        //    //    {
        //    //        AttendanceVisits.AttendanceNotifications notification =
        //    //        new AttendanceVisits.AttendanceNotifications(GlobalVariables.GetConnection(),
        //    //        AttendanceObj.EstateID,
        //    //         AttendanceObj.id, AttendanceObj.AttendingUser, 2);
        //    //        notification.SendColleagueNotifications();
        //    //        //System.Threading.Thread t =
        //    //        //    new System.Threading.Thread(new System.Threading.ThreadStart(notification.SendColleagueNotifications));
        //    //        //t.Start();
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        string h = ex.Message;
        //    //    }

                
        //    //} else
        //    //{
        //    //    //error
        //    //}


        //}
    }

    public class AttendanceHistoryViewModel : ViewModelBase
    {
        public ServiceChargeBudgetViewModel PropListViewModel { get; set; }
        public IEnumerable<Models.Properties> PropertyList { get; set; }
        public string EstateName { get; set; }

        public bool FromApp { get; set; }

        public List<SelectListItem> EstateList { get; set; }
        public int SelectedPropertyid { get; set; }
        public int SelectedAttendanceTypeID { get; set; }

     

        public AttendanceVisits Visit { get; set; }
        public async Task GetVisit(int id)
        {
            Visit = new AttendanceVisits();
            Visit = await AttendanceVisitMethods.GetAttendanceVisitsAsync(id);
            // new AttendanceVisits.ImageParams { width = 300, height = 300 });

            EstateName = EstatesDLL.EstateMethods.GetEstateNameByID(Visit.EstateID, GlobalVariables.GetConnection());
        }

        // public List<AttendanceVisits.AttendanceVisits> AttendanceList { get; set; }
        public List<Syncfusion.EJ2.Navigations.AccordionAccordionItem> AccordionList { get; set; }
        public string TestString { get; set; }
        
        public void SetEstateList()
        {
            List<Properties> estates = Models.PropertyMethods.GetAllEstates();
            EstateList = new List<SelectListItem>();
            foreach (Properties p in estates)
            {
                bool IsSelected = false;
                if (p.ID == SelectedPropertyid && SelectedPropertyid > 0)
                {
                    IsSelected = true;
                }

                EstateList.Add(new SelectListItem
                {
                    Text = p.Address1,
                    Value = p.ID.ToString(),
                    Selected = IsSelected
                });
            }
        }

        //public void SetAttendanceList(int estateID = 0)
        //{
        //    int estateid = 0;
        //    if (estateID == 0)
        //    {
        //        string q = $"select estateid from core.units where id ={SelectedPropertyid}";

        //        dbConn.DBConnectionObject db = GlobalVariables.GetConnection();
        //        System.Data.DataTable dt = db.Connection.GetDataTable(q);
              
        //        if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
        //        {
        //            int.TryParse(dt.Rows[0][0].ToString(), out estateid);
        //        }
        //    } else
        //    {
        //        estateid = estateID;
        //    }

        //    AttendanceList = new List<AttendanceVisits.AttendanceVisits>();
        //    AttendanceList = 
        //        AttendanceVisits.AttendanceVisitsMethods.AttendanceHistoryList(estateid, GlobalVariables.GetConnection(),
        //        new AttendanceVisits.ImageParams { width = 200, height = 100 });

        //    AccordionList = new List<Syncfusion.EJ2.Navigations.AccordionAccordionItem>();
        //    foreach(AttendanceVisits.AttendanceVisits item in AttendanceList)
        //    {
        //        AccordionList.Add(new Syncfusion.EJ2.Navigations.AccordionAccordionItem
        //        { Header = item.VisitDate.ToLongDateString(),
        //        Content = ContentHTML(item)});
        //    }
        //}

        //private string ContentHTML(AttendanceVisits.AttendanceVisits item)
        //{
        //    string r  =
        //        "<div class=\"row\"><div class=\"container\"> " +
        //               "<div>" +
        //                    "<h4>" + item.AttendanceStr + "</H4>" +
        //                "</div>" +
        //                "<div class=\"col-lg-5 col-md-6 col-sm-12 col-xs-12\" style=\"padding-left: 0px !Important\">" +
        //                    "<p>" + item.VisitDescription + "</p>" +
                            
        //                "</div>" +
        //                "<div class=\"col-lg-7 col-md-6 col-sm-12 col-xs-12\">";

          
        //    foreach(string url in item.UrlList)
        //    {
               
        //        r += "<img class=\"img-responsive thumbPreview\" style=\"display: inline-block; padding-right: 5px; margin-bottom: 5px !Important; vertical-align: top;\" src = \"" + url + "\" />";
                
        //    }

        //    r += "</div></div>";
   

        //    r += "<div class=\"container\">" +
        //              "<div class=\"\">" +
        //                  "<div>" +
        //                     "<a style=\"width:150px !Important;\" class=\"CentreSmallButton\" href=\"/Caretaking/AttendanceDetail?VisitID=" + item.id.ToString() + "\">view more</a>" +
        //                  "</div>" +
        //          "</div>" +
        //      "</div>";

         

        //    return r;
        //}

        
    }

    public class BuildingAreaViewModel
    {
        [Required(ErrorMessage = "Area Name is required.")]
        public string AreaName { get; set; }

        public List<SelectListItem> EstateList { get; set; }
        public int SelectedPropertyid { get; set; }
        public int SelectedAttendanceTypeID { get; set; }

        public string ViewName { get; set; }
        public string ControllerName { get; set; }

        public void SetLists()
        {
            List<Properties> estates = Models.PropertyMethods.GetAllEstates();
            EstateList = new List<SelectListItem>();
        }

        public string ImageUrls { get; set; }

        public string UpdateIDHeading { get; set; }
    }

    public static class AttendanceTypeMethods
    {
        //public static IEnumerable<AttendanceType> AllAttendanceTypesIEnumerable(dbConn.DBConnectionObject ConObject)
        //{
        //    DataTable dt = GetAttendanceTypeDataTable(conString);
        //    IEnumerable<AttendanceType> r = new  IEnumerable<AttendanceType>
        //}

        //public static ObservableCollection<AttendanceType> AllAttendanceTypesCollection(dbConn.DBConnectionObject ConObject)
        //{
        //    DataTable dt = GetAttendanceTypeDataTable(ConObject);
        //    ObservableCollection<AttendanceType> r = new ObservableCollection<AttendanceType>();

        //    if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
        //    {
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            r.Add(SetFromDataRow(dr));
        //        }
        //    }

        //    return r;
        //}

        //public static List<AttendanceType> AllAttendanceTypesList(dbConn.DBConnectionObject ConObject)
        //{
        //    DataTable dt = GetAttendanceTypeDataTable(ConObject);
        //    List<AttendanceType> r = new List<AttendanceType>();
        //    if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
        //    {
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            r.Add(SetFromDataRow(dr));
        //        }
        //    }

        //    return r;

        //}

        //private static AttendanceType SetFromDataRow(DataRow dr)
        //{
        //    int.TryParse(dr[0].ToString(), out int id);
        //    AttendanceType r = new AttendanceType
        //    {
        //        id = id,
        //        Name = dr[1].ToString()
        //    };

        //    return r;
        //}

        //private static DataTable GetAttendanceTypeDataTable(dbConn.DBConnectionObject ConObject)
        //{
        //    string q = "select * from core.AttendanceTypes";


        //    DataTable dt = ConObject.Connection.GetDataTable(q);
        //    return dt;
        //}
    }
}