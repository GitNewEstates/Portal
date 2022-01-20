using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public static class GetConnectionObject
    {
        private static dbConn.DBConnectionObject ConnectionObject { get; set; }
        public static dbConn.DBConnectionObject GetConnection()
        {
            if(ConnectionObject == null)
            {
                ConnectionObject = new dbConn.DBConnectionObject(dbConn.ConnectionType.SQLDB,
                   "Server=tcp:nemserver2017.database.windows.net,1433;Initial Catalog=NEMDb2;Persist Security Info=False;User ID=adam.new;Password=N3westates1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", dbConn.DBType.SQL);
            }

            return ConnectionObject;
        }
    }
}
