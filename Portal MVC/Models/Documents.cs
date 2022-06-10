using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dbConn;
using System.Data;

namespace Portal_MVC.Models
{
    public class Documents
    {

        private string _DocName;
        public string DocName
        {
            get
            {
                return _DocName;
            }
            set
            {
                _DocName = value;
            }
        }
        public int UserID { get; set; }
        public DateTime addedDate { get; set; }
        private string _AddedDateStr;
        public string AddedDateStr
        {
            get
            {
                return _AddedDateStr;
            }
            set
            {
                _AddedDateStr = value;
            }
        }
        public int id { get; set; }
        public byte[] Document { get; set; }
        private string _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                _FilePath = value;
            }

        }
        public string FileExtention { get; set; }
        public int DocTypeID { get; set; }
        public string AddedBy { get; set; }
        public string DocTypeName { get; set; }

        public long DocInstanceID { get; set; }

        private string _DocDescription;
        public string DocDescription
        {
            get
            {
                return _DocDescription;
            }
            set
            {
                _DocDescription = value;
            }
        }

        private bool _Rename;
        public bool Rename
        {
            get
            {
                return _Rename;
            }
            set
            {
                _Rename = value;
            }
        }

        private bool _SendWithWO;
        public bool SendWithWO
        {
            get
            {
                return _SendWithWO;
            }
            set
            {
                _SendWithWO = value;
            }
        }

    }

    public static class DocumentMethods
    {
        public static List<Documents> GetDocListByInstanceID(long instanceID)
        {
            string q = "Select id, name from core.documents where InstanceID = " + instanceID.ToString();


           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);

            List<Documents> r = new List<Documents>();

            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach (DataRow dr in dt.Rows)
                {

                    r.Add(new Documents
                    {
                        id = Convert.ToInt32(dr[0]),
                        DocName = dr[1].ToString(),
                        
                    });

                    

                }
            }

            return r;
        }

        public static Documents GetDocumentByID(long DocID)
        {
            string q = "Select document from core.documents where id = " + DocID.ToString();

           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);

            Documents r = new Documents();

            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                r.Document = DataRowToByteArray(dt.Rows[0], 0);
            }

            return r;
        }

        private static byte[] DataRowToByteArray(DataRow dataRow, int ColIndex)
        {
            byte[] docbtytes = null;

            if ((dataRow[ColIndex]).GetType() != typeof(System.DBNull))
            {
                docbtytes = (byte[])(dataRow[ColIndex]);
            }

            return docbtytes;
        }

     

    }
}
    


