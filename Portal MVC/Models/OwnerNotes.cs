using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using dbConn;

namespace Portal_MVC.Models
{
    public class OwnerNotesProperties
    {
        public string details { get; set; }
        public string addedby { get; set; }
        public DateTime dateAdded { get; set; }
    }

    public static class OwnerNotesMethods
    {
        public static List<OwnerNotesProperties> GetAllOwnerNotes(int unitID, int customerID)
        {
            List<OwnerNotesProperties> rlist = new List<OwnerNotesProperties>();

            string q = "select  core.OwnerNotes.AddedDate, core.OwnerNotes.userID, core.OwnerNotes.details " +
                "from core.OwnerNotes where unitID = " + unitID.ToString() +
                "and OwnerID = " + customerID.ToString() +
                "and PortalViewable = 1 order by AddedDate desc";

           DBConnectionObject db = GlobalVariables.GetConnection();

            DataTable dt = db.Connection.GetDataTable( q);

            foreach(DataRow dr in dt.Rows)
            {

                rlist.Add(new OwnerNotesProperties { dateAdded = Convert.ToDateTime(dr[0]), addedby = dr[1].ToString(), details = dr[2].ToString() });
            }

            if(rlist.Count == 0)
            {
                rlist.Add(new OwnerNotesProperties { details = "No history items exist for this property." });
            }

            return rlist;

        }

    }
}