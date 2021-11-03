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
        public long docID { get; set; }
        public long DocInstanceID { get; set; }
        public string Name { get; set; }
        public byte[] Document { get; set; }
        public string FileExt { get; set; }
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
                        docID = Convert.ToInt32(dr[0]),
                        Name = dr[1].ToString(),
                        
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
    


