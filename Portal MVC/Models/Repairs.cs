using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Portal_MVC.Models
{
    public class Repairs
    {
        public int ID { get; set; }
        public int ReportID { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public int PONumber { get; set; }
        public int QuoteID { get; set; }
        public int HeadAssetID { get; set; }
        public int SubAssetID { get; set; }
        public string TargetDatestr { get; set; }
        public DateTime TargetDate { get; set; }
        public string RaisedDatestr { get; set; }
        public DateTime RaisedDate { get; set; }
        public int DocID { get; set; }
        public List<Repairs> AllRepairs { get; set; }
        public int budgetID { get; set; }
        public int EstateID { get; set; }
        public string BudgetSchedule { get; set; }
        public double EstimatedCost { get; set; }
        public string EstimatedCostStr { get; set; }
        public List<Repairs> RepairHistory { get; set; }
        public string UpdateNote { get; set; }
        public string HeadAssetName { get; set; }
        public string SubAssetName { get; set; }
        public string SubAssetLocation { get; set; }
        public double completionCost { get; set; }
        public string completionCostStr { get; set; }
        public string HeadAssetLocation { get; set; }

        public bool AutomaticUpdates { get; set; }
    }


    public class CallReports
    {
        public int ID { get; set; }
        public string ReporterName { get; set; }
        public string ReportTitle { get; set; }
        public string ReportDetails { get; set; }
        public string Outcome { get; set; }
        public DateTime NextActionDate { get; set; }
        public string NextActionDateStr { get; set; }
        public string NextAction { get; set; }
        public string ReporterType { get; set; }
        public bool DisplayAnon { get; set; }
        public DateTime ReportDate { get; set; }
        public string ReportDateStr { get; set; }
        

    }

    public static class RepairUpdateMethods
    {
        public static void AddNew(int customerId, int repairID)
        {
            string q = "insert into core.RepairUpdates values (" + customerId.ToString() + ", " +
                repairID.ToString() + ")";


            //DBConnectionObject db = GlobalVariables.GetConnection();
            GlobalVariables.GetConnection().Connection.ExecuteCommand( q);


        }

        public static void Remove(int customerID, int RepairID)
        {
            string q = "delete from core.repairupdates where customerID = " + customerID.ToString() +
                " and repairID = " + RepairID.ToString();

            //DBConnectionObject db = GlobalVariables.GetConnection();
            GlobalVariables.GetConnection().Connection.ExecuteCommand( q);
        }

        public static void InsertReportUpdateFailure(int RepairID, int CustomerID)
        {

            List<string> c = new List<string>()
            {
                "repairID",
                "customerID",
                "_date"
            };

            List<string> p = new List<string>()
            {
                "@repairID",
                "@customerID",
                "@_date"
            };

            List<object> o = new List<object>()
            {
                RepairID,
                CustomerID,
                DateTime.Now
            };

            //DBConnectionObject db = GlobalVariables.GetConnection();
            GlobalVariables.GetConnection().Connection.InsertCommandCurrent( "core.RepairUpdateFailures", c, p, o);
        }

        public static bool IsCustomerRegisteredForUpdates(int RepairID, int customerID)
        {
            string q = "select * from core.repairupdates where customerID = " + customerID.ToString() +
                " and repairID = " + RepairID.ToString();

            bool r = false;

           //DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = GlobalVariables.GetConnection().Connection.GetDataTable( q);


            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                int id = 0;
                   int.TryParse(dt.Rows[0][0].ToString(), out id);
                if (id > 0)
                {
                    r = true;
                }
            }

            return r;
        }
    }

    public static class RepairMethods
    {
        public static Repairs GetAllRepairs(int EstateID)
        {
            string q = "select core.Repairs.ID, core.Repairs.ReportID, core.Repairs.RepairTitle, " +
                        "core.Repairs.RepairDetails, core.Repairs.RaisedDate, " +
                        "core.Repairs.TargetCompletionDate,  core.RepairStatus.RepairStatus " +
                        "from core.Repairs inner join core.RepairStatus on core.Repairs.RepairStatusID " +
                        "= core.RepairStatus.id where core.repairs.EstateID = " + EstateID.ToString() +
                        " order by core.repairs.raisedDate Desc";

           //DBConnectionObject db = GlobalVariables.GetConnection();

            DataTable dt = GlobalVariables.GetConnection().Connection.GetDataTable(q);
            Repairs repairs = new Repairs();
            repairs.AllRepairs = new List<Repairs>();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                if (row[0].ToString() != "Error")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        //raised date
                        DateTime raised = new DateTime();
                        if (dr[4] != DBNull.Value)
                        {
                            raised = Convert.ToDateTime(dr[4]);
                        }

                        //Target Completion
                        DateTime target = new DateTime();
                        if (dr[5] != DBNull.Value)
                        {
                            target = Convert.ToDateTime(dr[5]);
                        }


                        repairs.AllRepairs.Add(new Repairs { ID = Convert.ToInt32(dr[0]), Title = dr[2].ToString(), Details = dr[3].ToString(), RaisedDate = raised, RaisedDatestr = Controls.DateString(raised), TargetDate = target, TargetDatestr = Controls.DateString(target), Status = dr[6].ToString() });
                    }
                }
            }
            return repairs;
        }

        public static List<Repairs> GetRepairHistory(int repairID)
        {
            string q = "select updateDate, updatenote from core.RepairUpdateNote " +
                " where repairID = " + repairID.ToString() + " order by UpdateDate desc";

           //DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = GlobalVariables.GetConnection().Connection.GetDataTable( q);
            List<Repairs> r = new List<Repairs>();
            if (dt.Rows.Count > 0)
            {
                DataRow d = dt.Rows[0];
                if (d[0].ToString() != "Error")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DateTime logged = new DateTime();
                        if (dr[0] != DBNull.Value)
                        {
                            logged = Convert.ToDateTime(dr[0]);
                        }
                        r.Add(new Repairs { RaisedDate = logged, RaisedDatestr = Controls.DateString(logged), UpdateNote = dr[1].ToString() });
                    }
                }
            }
            return r;
        }

        public static CallReports GetRepairReport(int repairID)
        {
            //Query with report outcome removed
            string q = "select core.CallReports.id, core.CallReports.ReporterName, core.CallReports.ReportTitle, " +
                        "core.CallReports.ReportDetails, core.CallReports.NextActionDate, core.CallReports.DisplayAnon, " +
                        "Core.NextActions.NextAction, core.reporterType.ReporterType, " +
                        "core.CallReports.ReportDate " +
                        "from core.CallReports inner join core.Repairs on core.Repairs.reportID = core.CallReports.id " +
                        "inner join  core.nextactions on core.NextActions.id = core.callreports.nextactionid " +
                        "inner join core.ReporterType on core.ReporterType.id = core.CallReports.ReportertypeID " +
                        "where core.Repairs.id = " + repairID.ToString();

            //Original query
            //"select core.CallReports.id, core.CallReports.ReporterName, core.CallReports.ReportTitle, " +
            //            "core.CallReports.ReportDetails, core.CallReports.NextActionDate, core.CallReports.DisplayAnon, " +
            //            "core.ReportOutcomes.Outcome, Core.NextActions.NextAction, core.reporterType.ReporterType, " +
            //            "core.CallReports.ReportDate " +
            //            "from core.CallReports inner join core.Repairs on core.Repairs.reportID = core.CallReports.id " +
            //            "inner join core.ReportOutcomes on core.reportOutcomes.id = core.callreports.outcomeid " +
            //            "inner join  core.nextactions on core.NextActions.id = core.callreports.nextactionid " +
            //            "inner join core.ReporterType on core.ReporterType.id = core.CallReports.reportTypeID " +
            //            "where core.Repairs.id = " + repairID.ToString();

           //DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = GlobalVariables.GetConnection().Connection.GetDataTable( q);
            CallReports c = new CallReports();
            if (dt.Rows.Count > 0)
            {
                DataRow d = dt.Rows[0];
                if (d[0].ToString() != "Error")
                {
                    foreach (DataRow dr in dt.Rows)
                    {


                        //anon
                        bool anon = false;
                        if (dr[5] != DBNull.Value)
                        {
                            anon = Convert.ToBoolean(dr[5]);
                        }
                        else
                        {
                            anon = true;
                        }

                        string name = "";
                        if (anon == true)
                        {
                            name = "Not Displayed";
                        }
                        else
                        {
                            name = dr[1].ToString();
                        }

                        //next action date
                        DateTime nextaxtiondate = new DateTime();
                        if (dr[4] != DBNull.Value)
                        {
                            nextaxtiondate = Convert.ToDateTime(dr[4]);

                        }

                        //Report Date
                        DateTime logged = new DateTime();
                        if (dr[8] != DBNull.Value)
                        {
                            logged = Convert.ToDateTime(dr[8]);
                        }

                        CallReports cd = new CallReports
                        {
                            ID = Convert.ToInt32(dr[0]),
                            ReporterName = name,
                            ReportTitle = dr[2].ToString(),
                            ReportDetails = dr[3].ToString(),
                            NextActionDateStr = Controls.DateString(nextaxtiondate),
                            
                            NextAction = dr[6].ToString(),
                            ReporterType = dr[7].ToString(),
                            ReportDate = logged,
                            ReportDateStr = Controls.DateString(logged)
                        };

                        c = cd;
                    }
                }
            }
            return c;
        }

        public static Repairs GetAllRepairDetails(int repairID)
        {
            string q = "select core.Repairs.RepairTitle, core.Repairs.RepairDetails, " +
                        "core.Repairs.RaisedDate, core.Repairs.TargetCompletionDate, " +
                        "core.Repairs.ScheduleName, core.Repairs.QupteID, core.RepairStatus.RepairStatus, " + 
                        "core.Repairs.HeadAssetID, core.Repairs.SubAssetID, " +
                        "core.PurchaseOrders.EstCost, core.PurchaseOrders.ID from core.Repairs inner join core.RepairStatus on " +
                        "core.Repairs.RepairStatusID = core.RepairStatus.id " +
                        "inner join core.PurchaseOrders on " +
                        "core.Repairs.PONumber = core.PurchaseOrders.id where core.Repairs.ID = " + repairID.ToString();


           
            DataTable dt = GlobalVariables.GetConnection().Connection.GetDataTable( q);
            Repairs c = new Repairs();
            if (dt.Rows.Count > 0)
            {
                DataRow d = dt.Rows[0];
                if (d[0].ToString() != "Error")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        //raised date
                        DateTime raised = new DateTime();
                        if (dr[2] != DBNull.Value)
                        {
                            raised = Convert.ToDateTime(dr[2]);
                        }

                        //Target Completion
                        DateTime target = new DateTime();
                        if (dr[3] != DBNull.Value)
                        {
                            target = Convert.ToDateTime(dr[3]);
                        }

                        //Estiated Cost
                        double est = 0;
                        if(dr[9] != DBNull.Value)
                        {
                            est = Convert.ToDouble(dr[9]);
                        }
                        string eststr = Controls.CurrencyString(est);

                        //po number
                        int po = 0;
                        if (dr[10] != DBNull.Value)
                        {
                            po = Convert.ToInt32(dr[10]);
                        }

                        Repairs r = new Repairs
                        {
                            Title = dr[0].ToString(),
                            Details = dr[1].ToString(),
                            RaisedDate = raised,
                            RaisedDatestr = Controls.DateString(raised),
                            TargetDate = target,
                            TargetDatestr = Controls.DateString(target),
                            Status = dr[6].ToString(),
                            EstimatedCostStr = eststr,
                            HeadAssetID = Convert.ToInt32(dr[7]),
                            SubAssetID = Convert.ToInt32(dr[8]),
                            PONumber = po

                        };
                        c = r;
                    }

                    //Get Asset Details
                    if(c.HeadAssetID != 0)
                    {
                        q = "Select AssetName, AssetLocation from core.headassets where id = " + c.HeadAssetID.ToString();

                        DataTable dt1 = GlobalVariables.GetConnection().Connection.GetDataTable( q);
                        if(dt1.Rows.Count > 0)
                        {
                            if(dt1.Rows[0][0] != DBNull.Value || dt1.Rows[0][0].ToString() != "Error")
                            {
                                c.HeadAssetName = dt1.Rows[0][0].ToString();
                                c.HeadAssetLocation = dt1.Rows[0][1].ToString();
                            }
                        }
                    }
                   

                    if (c.SubAssetID != 0)
                    {
                        q = "Select AssetName, AssetLocation from core.subassets where id = " + c.SubAssetID.ToString();

                        DataTable dt1 = GlobalVariables.GetConnection().Connection.GetDataTable( q);
                        if (dt1.Rows.Count > 0)
                        {
                            if (dt1.Rows[0][0] != DBNull.Value || dt1.Rows[0][0].ToString() != "Error")
                            {
                                c.SubAssetName = dt1.Rows[0][0].ToString();
                                c.SubAssetLocation = dt1.Rows[0][1].ToString();
                            }
                        }
                    }

                    //sets head asset location to null if sub asset location present. 
                    //view only needs one of the asset locations to be visible.
                    if(c.SubAssetLocation != null && c.SubAssetLocation != "")
                    {
                        c.HeadAssetLocation = null;
                    }
                }
            }

            //if quote id is not 0 get quote details


            return c;
        }

        public static double CompletedCost(int PONumber)
        {
            string q = "select sum(core.Transactions.TransAmount) from core.Transactions " +
                        "where PONumber = " + PONumber.ToString();

           //DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = GlobalVariables.GetConnection().Connection.GetDataTable( q);
            double d = 0;

            if(dt.Rows.Count > 0)
            {
                if(dt.Rows[0][0] != DBNull.Value || dt.Rows[0][0].ToString() != "Error")
                {
                    d = Convert.ToDouble(dt.Rows[0][0]);
                }
            }

            return d;
        }

        public static Repairs GetOutstandingRepairs(int EstateID)
        {
            string q = "select core.Repairs.ID, core.Repairs.ReportID, core.Repairs.RepairTitle, " +
                        "core.Repairs.RepairDetails, core.Repairs.RaisedDate, " +
                        "core.Repairs.TargetCompletionDate,  core.RepairStatus.RepairStatus " +
                        "from core.Repairs inner join core.RepairStatus on core.Repairs.RepairStatusID " +
                        "= core.RepairStatus.id where core.repairs.EstateID = " + EstateID.ToString() +
                        " and  core.Repairs.RepairStatusId = 1 " +
                        " order by core.repairs.raisedDate Asc";

           //DBConnectionObject db = GlobalVariables.GetConnection();

            DataTable dt = GlobalVariables.GetConnection().Connection.GetDataTable(q);
            Repairs repairs = new Repairs();
            repairs.AllRepairs = new List<Repairs>();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                if (row[0].ToString() != "Error")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        //raised date
                        DateTime raised = new DateTime();
                        if (dr[4] != DBNull.Value)
                        {
                            raised = Convert.ToDateTime(dr[4]);
                        }

                        //Target Completion
                        DateTime target = new DateTime();
                        if (dr[5] != DBNull.Value)
                        {
                            target = Convert.ToDateTime(dr[5]);
                        }


                        repairs.AllRepairs.Add(new Repairs { ID = Convert.ToInt32(dr[0]),
                            Title = dr[2].ToString(),
                            Details = dr[3].ToString(),
                            RaisedDate = raised,
                            RaisedDatestr = Controls.DateString(raised),
                            TargetDate = target,
                            TargetDatestr = Controls.DateString(target),
                            Status = dr[6].ToString() });
                    }
                }
            }

            return repairs;
        }



    }

    public class APIRepairs
    {
        public string JSONSerialize()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public APIRepairs()
        {


            APIError = new APIError(ErrorType.None);
            //UpdateUsers = new ObservableCollection<RepairUpdateUser>();
        }

        public APIError APIError { get; set; }

        public int ID { get; set; }
        public int ReportID { get; set; }
        public int HeadAssetID { get; set; }
        public int SubAssetID { get; set; }
        public int RepairStatusID { get; set; }

        private string _RepairStatus;
        public string RepairStatus
        {
            get
            {
                return _RepairStatus;
            }
            set
            {
                _RepairStatus = value;
            }
        }

        private string _RepairTitle;
        public string RepairTitle
        {
            get
            {
                return _RepairTitle;
            }
            set
            {
                _RepairTitle = value;

            }
        }


        private string _RepairDetails;
        public string RepairDetails
        {
            get
            {
                return _RepairDetails;
            }
            set
            {
                _RepairDetails = value;
            }
        }

        public DateTime RaisedDate { get; set; }

        public DateTime TargetDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public string CompletionDateStr { get; set; }

        public int QuoteID { get; set; }
        public string DocInstanceID { get; set; }


        public int EstateID { get; set; }

        private int ValueChanged { get; set; }
        private double _EstimatedAmount;
        public double EstimatedAmount
        {
            get
            {
                return _EstimatedAmount;
            }
            set
            {
                _EstimatedAmount = value;
            }
        }

        public int SupplierID { get; set; }

        public double CostStr { get; set; }
        public double FinalCost { get; set; }

        //public ObservableCollection<RepairUpdateUser> UpdateUsers { get; set; }

        //public void InsertPOtoRepair()
        //{
        //    string q = "Update core.repairs set PONumber = " + this.PONumber.ToString() +
        //        " where id = " + this.ID.ToString();


        //    GlobalVariables.ConnectionObject.Connection.ExecuteCommand( q);
        //}

        //used when update note done on separate string. Also a static method available



    }

    public static class RepairExtensions
    {
        public static APIRepairs DeserializedJSONToRepair(string json = "")
        {
            APIRepairs obj = new APIRepairs();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj =  JsonConvert.DeserializeObject<APIRepairs>(json);
                }
                catch (Exception ex)
                {
                    obj.APIError = new APIError(ErrorType.APIValidationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Repair. Error: {ex.Message}"
                    };
                }
            }

            return obj;
        }

        public static List<APIRepairs> DeserializedJSONToRepairList(string json = "")
        {
            List<APIRepairs> obj = new List<APIRepairs>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<APIRepairs>>(json);
                }
                catch (Exception ex)
                {

                    APIError error = new APIError(ErrorType.JSONDeserializationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Repair List. Error: {ex.Message}"
                    };

                    obj.Add(new APIRepairs
                    {
                        APIError = error
                    });
                }
            }

            return obj;
        }

        public static string SerializeRepairToJson(Repairs repair)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(repair);
        }

        public static APIError IsRepairValid(APIRepairs repair)
        {
            //tests for validation
            //Mandator Fields are :
            //RepairDetails
            //RepairTitle
            //EstateID
            //CallReportID
            //target completion Date

            if (string.IsNullOrEmpty(repair.RepairDetails))
            {
                return new APIError(ErrorType.APIValidationError)
                {
                    HasError = true,
                    Message = "No Repair Details Provided"
                };

            }
            if (string.IsNullOrEmpty(repair.RepairTitle))
            {
                return new APIError(ErrorType.APIValidationError)
                {
                    HasError = true,
                    Message = "No Repair Title Provided"
                };

            }
            if (repair.EstateID == 0)
            {
                return new APIError(ErrorType.APIValidationError)
                {
                    HasError = true,
                    Message = "No Estate ID Provided"
                };

            }

            if (repair.ReportID == 0)
            {
                return new APIError(ErrorType.APIValidationError)
                {
                    HasError = true,
                    Message = "No Call Report ID Provided"
                };

            }

            DateTime test = new DateTime();
            if (repair.TargetDate == test)
            {
                return new APIError(ErrorType.APIValidationError)
                {
                    HasError = true,
                    Message = "No Target Date Provided"
                };
            }

            return new APIError(ErrorType.None);
        }

        public async static Task<APIRepairs> GetRepair(int RepairID)
        {
            
            await APIAuthExtensions.SetAPIConfigAsync();

            
            string ReturnJson = await
                GlobalVariables.APIConnection.CallAPIGetEndPointAsync($"Repairs/{RepairID}");
            return ReturnRepairFromAPICall(ReturnJson);


        }
        private static APIRepairs ReturnRepairFromAPICall(string json)
        {
            //will return a repair deserialized from the json returned from API
            //if error occurs with API call will set the APIError on the repair
            //and reset the apierror object on the global api connection object
            APIRepairs repair = new APIRepairs();
            if (!GlobalVariables.APIConnection.APIError.HasError)
            {
                //serialize
                repair = DeserializedJSONToRepair(json);
            }
            else
            {
                string errormessage = GlobalVariables.APIConnection.APIError.Message;
                ErrorType ty = GlobalVariables.APIConnection.APIError.errorType;

                repair.APIError.HasError = true;
                repair.APIError.errorType = ty;
                repair.APIError.Message = errormessage;

                GlobalVariables.APIConnection.ResetAPIError();
            }

            return repair;
        }

        public async static Task<List<APIRepairs>> GetRepairsList(int EstateID, bool Openonly = false)
        {
            string ReturnJson = await
           GlobalVariables.APIConnection.CallAPIGetEndPointAsync($"OpenRepairList/{EstateID}");

            return DeserializedJSONToRepairList(ReturnJson);
        }



    }
    public class RepairUpdateUser
    {
        public RepairUpdateUser()
        {
            APIError = new APIError(ErrorType.None);
        }
        public APIError APIError { get; set; }
        public int RepairID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public bool SendNotificationEmail { get; set; }
        public int UserID { get; set; }

        List<string> ColNames = new List<string> { "RepairID", "Name", "Email" };
        List<string> ParamNames = new List<string> { "@RepairID", "@Name", "@Email" };
        private List<object> Objects()
        {
            return new List<object> { RepairID, Name, Email };
        }


       

    }

    public static class RepairUpdateUserMethods
    {
        public static string JsonSerialize(RepairUpdateUser repair)
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(repair);
        }
        public static RepairUpdateUser DeserializedJSONToRepairUpdateUser(string json = "")
        {
            RepairUpdateUser obj = new RepairUpdateUser();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<RepairUpdateUser>(json);
                }
                catch (Exception ex)
                {
                    obj.APIError = new APIError(ErrorType.APIValidationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Repair Update. Error: {ex.Message}"
                    };
                }
            }

            return obj;
        }
        public async static Task<ObservableCollection<RepairUpdateUser>> GetRepairUpdateUsersAsync(int _repairId)
        {
            string q = $"select * from core.RepairUpdateNotifications where repairid = {_repairId}";

            DataTable dt = await
                GlobalVariables.GetConnection().Connection.GetDataTableAsync(q);
            ObservableCollection<RepairUpdateUser> r = new ObservableCollection<RepairUpdateUser>();
            APIError error = DBInsertErrorMethods.DBErrorCheck(dt);

            if (!error.HasError)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    r.Add(new RepairUpdateUser()
                    {
                        Name = dr[2].ToString(),
                        Email = dr[3].ToString()
                    });
                }
            }

            return r;
        }

       
    }


}