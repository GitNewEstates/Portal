using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class CaseListViewModel
    {
        public CaseListViewModel()
        {
            CaseListStr = new List<string>();
        }
        public IEnumerable<EstateCases> CaseList { get; set; }
        public List<string> CaseListStr { get; set; }
    }

    public class CaseDetailViewModel
    {
        public CaseDetailViewModel()
        {
            EstateCase = new EstateCases();
        }
        public EstateCases EstateCase { get; set; }
    }

    public class Cases : BaseClass
    {
        public Cases()
        {
            CreatedBy = new UsersDLL.Users();
            // CaseActionCollection = new ObservableCollection<CaseActions>();
            //Guid guid = Guid.NewGuid();
            //this.EmailGuid = guid.ToString();



        }

        public new UsersDLL.Users CreatedBy { get; set; }

        private string _CreatedDateStr;
        public string CreatedDateStr
        {
            get
            {
                return _CreatedDateStr;
            }
            set
            {
                _CreatedDateStr = value;

            }
        }


        public DateTime CompletionDate { get; set; }
        private string _CompletedDateStr;
        public string CompletedDateStr
        {
            get
            {
                return _CompletedDateStr;
            }
            set
            {
                _CompletedDateStr = value;
                //OnPropertyChanged("CompletedDateStr");
            }
        }

        public DateTime CreatedDate { get; set; }

        private UsersDLL.Users _CaseOwner;
        public UsersDLL.Users CaseOwner
        {
            get
            {
                return _CaseOwner;
            }
            set
            {
                _CaseOwner = value;
                //OnPropertyChanged("CaseOwner");
            }
        }

        private string _CaseTitle;
        public string CaseTitle
        {
            get
            {
                return _CaseTitle;
            }
            set
            {
                _CaseTitle = value;
                //OnPropertyChanged("CaseTitle");
            }
        }

        private string _CaseDetails;
        public string CaseDetails
        {
            get
            {
                return _CaseDetails;
            }
            set
            {
                _CaseDetails = value;
                //OnPropertyChanged("CaseDetails");
            }
        }

        private CaseActions _NextAction;
        public CaseActions NextAction
        {
            get
            {
                return _NextAction;
            }
            set
            {
                _NextAction = value;
                //OnPropertyChanged("NextAction");

            }
        }

        public string EmailGuid { get; set; }

        public enum CaseStatus
        {
            Open,
            Closed,
            Cancelled,
            None
        }

        private CaseStatus _caseStatus;
        public CaseStatus caseStatus
        {
            get
            {
                return _caseStatus;
            }
            set
            {
                _caseStatus = value;
                //OnPropertyChanged("caseStatus");
            }
        }

        public long DocInstanceId { get; set; }
        private IEnumerable<CaseActions> _CaseActionCollection;
        public IEnumerable<CaseActions> CaseActionCollection
        {
            get
            {
                return _CaseActionCollection;
            }
            set
            {
                _CaseActionCollection = value;
                //OnPropertyChanged("CaseActionCollection");
            }
        }
        private DateTime _TargetCompletionDate;
        public DateTime TargetCompletionDate
        {
            get { return _TargetCompletionDate; }
            set
            {
                _TargetCompletionDate = value;
                //OnPropertyChanged("TargetCompletionDate");
            }
        }



    }


    public class CaseActions : BaseClass
    {
        public CaseActions(CaseType _caseType, ActionStatus _status)
        {
            caseType = _caseType;
            actionStatus = _status;
            SendNotification = true;
        }

        //public TemplateTaskType TemplateTaskType { get; set; }

        public long TemplateDocInstanceID { get; set; }
        public int TemplateActionID { get; set; }

        public int CompletionDays { get; set; }

        private string _ActionDetails;
        public string ActionDetails
        {
            get
            {
                return _ActionDetails;
            }
            set
            {
                _ActionDetails = value;
                //OnPropertyChanged("ActionDetails");
            }
        }

        public string ActionDetailsHeader
        {
            get
            {
                string r = "";
                if (!string.IsNullOrWhiteSpace(ActionDetails))
                {
                    //max length 100
                    if (ActionDetails.Length < 100)
                    {
                        r = ActionDetails;
                    }
                    else
                    {
                        r = ActionDetails.Substring(0, 95);
                        r += "...";
                    }
                }

                return r;
            }
        }

        private string _ActionCompletedNote;
        public string ActionCompletedNote
        {
            get
            {
                return _ActionCompletedNote;
            }
            set
            {
                _ActionCompletedNote = value;
                //OnPropertyChanged("ActionCompletedNote");
            }
        }

        public ActionStatus actionStatus { get; set; }

        private UsersDLL.Users _ActionOwner;
        public UsersDLL.Users ActionOwner
        {
            get
            {
                return _ActionOwner;
            }
            set
            {
                _ActionOwner = value;
                //OnPropertyChanged("ActionOwner");
            }
        }
        public UsersDLL.Users ActionCreatedUser { get; set; }
        public UsersDLL.Users ActionCompletedUser { get; set; }

        public int CaseID { get; set; }

        public DateTime CreatedDate { get; set; }
        private string _CreatedDateStr;
        public string CreatedDateStr
        {
            get
            {
                return _CreatedDateStr;

            }
            set
            {
                _CreatedDateStr = value;
                //OnPropertyChanged("CreatedDateStr");
            }
        }

        private DateTime _TargetCompletionDate;
        public DateTime TargetCompletionDate
        {
            get
            {
                return _TargetCompletionDate;
            }
            set
            {
                _TargetCompletionDate = value;
                TargetCompletionDateStr = _TargetCompletionDate.ToLongDateString();
                //OnPropertyChanged("_TargetCompletionDate");
            }
        }
        private string _TargetCompletionDateStr;
        public string TargetCompletionDateStr
        {
            get
            {
                return _TargetCompletionDateStr;

            }
            set
            {
                _TargetCompletionDateStr = value;
                //OnPropertyChanged("TargetCompletionDateStr");
            }
        }

        public DateTime CompletedDate { get; set; }
        private string _CompletedDateStr;
        public string CompletedDateStr
        {
            get
            {
                return _CompletedDateStr;

            }
            set
            {
                _CompletedDateStr = value;
                //OnPropertyChanged("CompletedDateStr");
            }
        }

        public CaseType caseType { get; set; }

        public enum CaseType
        {
            EstateCase
        }

        private bool _SendNotification;
        public bool SendNotification
        {
            get { return _SendNotification; }
            set
            {
                _SendNotification = value;
                //OnPropertyChanged("SendNotification");
            }
        }

        //public void Insert()
        //{
        //    List<string> c = new List<string>();
        //    List<string> p = new List<string>();
        //    List<object> o = new List<object>();

        //    c.Add("ActionDetails");
        //    c.Add("ActionOwnerID");
        //    c.Add("ActionStatus");
        //    c.Add("CreatedDate");
        //    c.Add("CreatedOwnerID");
        //    c.Add("CaseID");
        //    c.Add("TargetCompletionDate");
        //    c.Add("TaskSequence");
        //    c.Add("IsNextTask");

        //    p.Add("@ActionDetails");
        //    p.Add("@ActionOwnerID");
        //    p.Add("@ActionStatus");
        //    p.Add("@CreatedDate");
        //    p.Add("@CreatedOwnerID");
        //    p.Add("@CaseID");
        //    p.Add("@TargetCompletionDate");
        //    p.Add("@TaskSequence");
        //    p.Add("@IsNextTask");

        //    o.Add(this.ActionDetails);
        //    o.Add(this.ActionOwner.UserID);
        //    o.Add(this.actionStatus.ToString());
        //    o.Add(DateTime.Now);
        //    o.Add(GlobalVariables.User.UserID);
        //    o.Add(this.CaseID);
        //    o.Add(this.TargetCompletionDate);
        //    o.Add(this.TaskSequence);
        //    o.Add(this.IsNextTask);

        //    
        //    DataTable dt = GlobalVariables.GetConnection().Connection.InsertCommandWithReturnID( GetTableName(), c, p, o);
        //    int id = 0;
        //    if(dt.Rows.Count> 0 && dt.Rows[0][0].ToString() != "Error")
        //    {
        //        int.TryParse(dt.Rows[0][0].ToString(), out id);
        //        this.CreatedDateStr = ControlsDLL.ControlActions.DateTimeFormat(this.CreatedDate);
        //        ActionCompletedUser = GlobalVariables.User;
        //    }

        //    this.id = id;
        //    this.TargetCompletionDateStr = ControlsDLL.ControlActions.DateFormat(this.TargetCompletionDate);
        //    OnCaseActionCreated();


        //}

        

        

        private int _TaskSequence;
        public int TaskSequence
        {
            get
            {
                return _TaskSequence;
            }
            set
            {
                _TaskSequence = value;
                //OnPropertyChanged("TaskSequence");
            }
        }
        public bool IsNextTask { get; set; }
        //public EstateCases ParentEstateCase { get; set; }
        private string GetTableName()
        {
            string TableName = "";
            switch (this.caseType)
            {
                case CaseType.EstateCase:
                    TableName = "core.EstateCaseActions";
                    break;
            }

            return TableName;
        }

     

 

        

    }
    public enum ActionStatus
    {
        Open, Closed, Cancelled, None
    }

    public class EstateCases : Cases
    {
        public EstateCases(int EstateID = 0, string EstateName = null,
            int _linkedObjID = 0)
        {
            Estate = new Estates { EstatedID = EstateID };
            if (!string.IsNullOrWhiteSpace(EstateName))
            {
                Estate.EstateName = EstateName;
            }
            CaseActionCollection = new ObservableCollection<CaseActions>();
            HistoryCollection = new ObservableCollection<CaseHistoryItem>();
   
            LinkedObjectID = _linkedObjID;
            CaseData = new CaseData();
        }

        public string StatusStr { get { return this.caseStatus.ToString(); } }
        public Estates Estate { get; set; }
        public int LinkedObjectID { get; set; }

        private string _TargetCompletionString;
        public string TargetCompletionString
        {
            get { return _TargetCompletionString; }
            set
            {
                _TargetCompletionString = value;
                //OnPropertyChanged("TargetCompletionString");
            }
        }

        public CaseData CaseData { get; set; }

   

        private IEnumerable<CaseHistoryItem> _HistoryCollection;
        public IEnumerable<CaseHistoryItem> HistoryCollection
        {
            get { return _HistoryCollection; }
            set
            {
                _HistoryCollection = value;
                //OnPropertyChanged("HistoryCollection");
            }
        }


        public void GetCaseHistory()
        {
            if (HistoryCollection != null)
            {
                //HistoryCollection.Clear();
            }
            else
            {
                HistoryCollection = new ObservableCollection<CaseHistoryItem>();
            }

          //  HistoryCollection =
            //    EstateCaseHistoryItemMethods.GetCaseHistoryItemCollection(this.id);


        }

   

       
    

        private int _TemplateID;
        public int TemplateID
        {
            get
            {
                return _TemplateID;
            }
            set
            {
                _TemplateID = value;
                if (_TemplateID > 0)
                {
                    //CreateTemplateActions
                }
            }
        }

       
        //public int TemplateID { get; set; }

        //linked objects
        public bool HasRepairs { get; set; }

        // public EstateCaseNotifications EstateCaseNotification { get; set; }

    }

    public static class EstateCaseMethods
    {
        public async static Task<DataTable> GetAllEstateCasesDataTable(
            int UserID = 0,
            int estateID = 0,
            EstateCases.CaseStatus status = Cases.CaseStatus.None,
            DateTime StartDate = new DateTime(), 
            DateTime EndDate = new DateTime(), 
            int userid = 0)
        {
            string q = "select core.EstateCases.id, core.EstateCases.caseTitle, " +
                "core.EstateCases.CaseDetails, core.Estates.id, " +
                    "core.Estates.Name,  core.EstateCases.caseownerid, core.EstateCases.docinstanceid, concat(Users.Users.firstname, ' ' , Users.Users.surname), " +
                    "core.estatecases.TargetCompletionDate, core.EstateCases.casestatus, " +
                    " core.EstateCases.CaseTemplateID" +
                    " from core.EstateCases " +
                    "inner join core.Estates on core.EstateCases.EstateID = core.Estates.ID " +
                    " inner join Users.Users on core.estatecases.caseownerid = Users.Users.id ";

            List<string> WhereParams = new List<string>();

            if (UserID > 0)
            {
                WhereParams.Add($" core.EstateCases.CaseOwnerID = {UserID}");
            }

            if (estateID > 0)
            {
                WhereParams.Add($" core.EstateCases.estateid = {estateID}");
            }

            if (status != Cases.CaseStatus.None)
            {
                WhereParams.Add($" core.estatecases.casestatus = '{status}'");
            }

            DateTime Test = new DateTime();
            if (StartDate > Test)
            {
                string date = ControlsDLL.ControlActions.DateTimeForDBQuery(StartDate);
                WhereParams.Add($" core.estatecases.createddate >= '{date}'");
            }

            if (EndDate > Test)
            {
                string date = ControlsDLL.ControlActions.DateTimeForDBQuery(EndDate);
                WhereParams.Add($" core.estatecases.createddate <= '{date}'");
            }

            if (userid > 0)
            {
                WhereParams.Add($" core.estatecases.caseownerid <= {userid}");
            }

            if (WhereParams.Count > 0)
            {
                q += " where ";
                int WhereCount = 0;
                foreach (string param in WhereParams)
                {
                    if (WhereCount == 0)
                    {
                        q += param;
                    }
                    else
                    {
                        q += " and " + param;
                    }
                    WhereCount += 1;
                }
            }

            q += " Order by core.estatecases.TargetCompletionDate asc";


            DataTable dt = await GlobalVariables.GetConnection().Connection.GetDataTableAsync(q);


            return dt;
        }

        public async static Task<List<EstateCases>> GetAllEstateCases(
            int UserID = 0,
            int estateID = 0,
            EstateCases.CaseStatus _status = Cases.CaseStatus.None,
           DateTime StartDate = new DateTime(), DateTime EndDate = new DateTime(),
           int userid = 0)
        {



            DataTable dt = await GetAllEstateCasesDataTable(UserID, estateID, _status,
                StartDate: StartDate, EndDate: EndDate, userid: userid);

            List<EstateCases> r = new List<EstateCases>();

            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        int id = Convert.ToInt32(dr[0].ToString());
                        int eID = Convert.ToInt32(dr[3].ToString());
                        int ownerid = Convert.ToInt32(dr[5].ToString());
                        int _TemplateID = Convert.ToInt32(dr[10].ToString());
                        long docinstanceid = (long)Convert.ToInt64(dr[6].ToString());
                        DateTime TargetDate = Convert.ToDateTime(dr[8].ToString());
                        //int.TryParse(dr[0].ToString(), out int id);
                        //int.TryParse(dr[3].ToString(), out int eID);
                        //int.TryParse(dr[5].ToString(), out int ownerid);
                        //int.TryParse(dr[10].ToString(), out int _TemplateID);
                        //long.TryParse(dr[6].ToString(), out long docinstanceid);
                        //DateTime.TryParse(dr[8].ToString(), out DateTime TargetDate);

                        EstateCases.CaseStatus status = new Cases.CaseStatus();


                        if (dr[9].ToString() == "Open")
                        {
                            status = Cases.CaseStatus.Open;
                        }
                        else if (dr[9].ToString() == "Closed")
                        {
                            status = Cases.CaseStatus.Closed;
                        }

                        r.Add(new EstateCases(eID, dr[4].ToString())
                        {
                            id = id,
                            CaseDetails = dr[2].ToString(),
                            CaseTitle = dr[1].ToString(),
                            CaseOwner = new UsersDLL.Users { UserID = ownerid, 
                                UserFullName = dr[7].ToString() },
                            DocInstanceId = docinstanceid,
                            TargetCompletionDate = TargetDate,
                            TargetCompletionString = TargetDate.ToShortDateString(),
                            caseStatus = status,
                            TemplateID = _TemplateID
                        });
                    } catch { }
                }
            }
            //List<EstateCases> r = new List<EstateCases>();
            return r;
        }

        public async static Task<EstateCases> GetEstateCase(int CaseID)
        {
            string q = "Select * from core.estatecases " +
                "inner join core.EstateCaseSettingData on core.estatecases.id = " +
                "core.EstateCaseSettingData.EstateCaseID " +
                "inner join core.CaseNotifications on core.EstateCases.id = core.CaseNotifications.CaseID " +
                "where core.estatecases.id = " + CaseID.ToString();

            DataTable dt = await
                GlobalVariables.GetConnection().Connection.GetDataTableAsync(q);

            EstateCases r = new EstateCases();
            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int caseid = Convert.ToInt32(dr[0].ToString());
                    int estID = Convert.ToInt32(dr[7].ToString());
                    int caseOwnerid = Convert.ToInt32(dr[4].ToString());
                    int caseCreatedbyid = Convert.ToInt32(dr[5].ToString());
                    int TemplateID = Convert.ToInt32(dr[10].ToString());
                    long docinsID = (long)Convert.ToInt64(dr[8].ToString());
                    DateTime targetdate = Convert.ToDateTime(dr[9].ToString());
                    DateTime CreatedDate = Convert.ToDateTime(dr[6].ToString());

                    
                    //int.TryParse(dr[14].ToString(), out int NotificationID);
                    //long.TryParse(dr[16].ToString(), out long NotificationDocInstance);


                    r.id = caseid;
                    r.CaseTitle = dr[1].ToString();
                    r.CaseDetails = dr[2].ToString();
                    if (dr[3].ToString() == "Closed")
                    {
                        r.caseStatus = EstateCases.CaseStatus.Closed;

                    }
                    else if (dr[3].ToString() == "Open")
                    {
                        r.caseStatus = EstateCases.CaseStatus.Open;
                    }

                    r.CaseOwner = new UsersDLL.Users { UserID = caseOwnerid };
                    r.CreatedBy = new UsersDLL.Users { UserID = caseCreatedbyid };
                    r.CreatedDate = CreatedDate;
                    r.Estate = new Estates { EstatedID = estID };
                    r.DocInstanceId = docinsID;
                    r.TargetCompletionDate = targetdate;
                    r.TemplateID = TemplateID;
                    r.CaseData.CaseID = caseid;   
                    r.EmailGuid = dr[11].ToString();
                    //r.EstateCaseNotification = new EstateCaseNotifications(r.id);
                    //r.EstateCaseNotification.id = NotificationID;
                    //r.EstateCaseNotification.DocinstanceID = NotificationDocInstance;


                }
            }

            return r;
        }
    }

    public class CaseHistoryItem : BaseClass
    {
        //base class for all Case History

        public CaseHistoryItem(CaseType _caseType)
        {
            caseType = _caseType;
            User = new UsersDLL.Users();
        }

        public enum CaseType
        {
            EstateCase
        }

        public CaseType caseType { get; set; }

        public int CaseID { get; set; }

        private string _HistoryNote;
        public string HistoryNote
        {
            get
            {
                return _HistoryNote;
            }
            set
            {
                _HistoryNote = value;
                //OnPropertyChanged("HistoryNote");
            }
        }

        private string _HistortyDateStr;
        public string HistortyDateStr
        {
            get
            {
                return _HistortyDateStr;
            }
            set
            {
                _HistortyDateStr = value;
                //OnPropertyChanged("HistortyDateStr");
            }
        }

        public DateTime HistoryDate { get; set; }



        public UsersDLL.Users User { get; set; }

       



    }

    public class CaseData
    {
        public int UnitID { get; set; }
        public int CaseID { get; set; }


    }

    class EstateCaseHistoryItem : BaseClass
    {
        //class containing specific properties and methods that relate specifically to estate cases


    }



}