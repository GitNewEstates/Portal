using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Portal_MVC.Models
{
    public class AttendanceLogViewModel
    {
        public AttendanceLogViewModel()
        {
            AttendanceObj = new AttendanceVisits.AttendanceVisits();
            MaxDate = DateTime.Now;
            VisitDate = DateTime.Now;
            
        }
        public List<SelectListItem> EstateList { get; set; }
        public int SelectedPropertyid { get; set; }
        public List<SelectListItem> AttendanceTypes { get; set; }
        public int SelectedAttendanceTypeID { get; set; }
        public AttendanceVisits.AttendanceVisits AttendanceObj { get; set; }

        public string ImageUrls { get; set; }

        [Required(ErrorMessage = "Description of the Attendance is required.")]
        public string AttendanceDescription { get; set; }

        [Required(ErrorMessage = "Date and Time of the visit is required.")]
        public DateTime VisitDate { get; set; }

        public DateTime MaxDate { get; set; }

        public bool CaretakingVisit { get; set; }
        public bool InspectionVisit { get; set; }
        public bool CallOutVisit { get; set; }
        
        public void SetLists()
        {
            List<Properties> estates = Models.PropertyMethods.GetAllEstates();
            EstateList = new List<SelectListItem>();
            foreach (Properties p in estates)
            {
                bool IsSelected = false;
                if(p.ID == SelectedPropertyid && SelectedPropertyid > 0)
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

            List<AttendanceVisits.AttendanceType> types = 
                AttendanceVisits.AttendanceTypeMethods.AllAttendanceTypesList(GlobalVariables.CS);
            AttendanceTypes = new List<SelectListItem>();

            foreach(AttendanceVisits.AttendanceType t in types)
            {
                bool IsSelected = false;
                if (t.id == SelectedAttendanceTypeID && SelectedAttendanceTypeID > 0)
                {
                    IsSelected = true;
                }

                AttendanceTypes.Add(new SelectListItem
                {
                    Text = t.Name,
                    Value = t.id.ToString(),
                    Selected = IsSelected
                });
            }

        }
        
        public void Insert()
        {
            AttendanceObj.AttendanceTypeID = SelectedAttendanceTypeID;
            AttendanceObj.EstateID = SelectedPropertyid;
            AttendanceObj.urlString = ImageUrls;
            AttendanceObj.VisitDate = VisitDate;
            AttendanceObj.VisitDescription = AttendanceDescription;

            AttendanceObj.Insert(GlobalVariables.CS);
            
        }
    }

    public class AttendanceHistoryViewModel
    {
        public List<SelectListItem> EstateList { get; set; }
        public int SelectedPropertyid { get; set; }
        public int SelectedAttendanceTypeID { get; set; }

        public List<AttendanceVisits.AttendanceVisits> AttendanceList { get; set; }
        public string TestString { get; set; }


        public void SetList()
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
    }
}