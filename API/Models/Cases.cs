using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Cases
    {
        public int id { get; set; }
        public string  CaseTitle { get; set; }
    }

    public static class CaseMethods
    {
        public async static Task UpdateTitle(int caseID)
        {
            string cmd = "";
            await GetConnectionObject.GetConnection().Connection.ExecuteCommandAsync(cmd);
        }
    }
}
