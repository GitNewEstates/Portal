using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Portal_MVC.Models
{
    public class BaseClass
    {
        public BaseClass()
        {
            APIError = new APIError(ErrorType.None);
        }
        public APIError APIError { get; set; }

        private int _id;
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
             
            }
        }

        public string DocinstanceID { get; set; }

        private bool _HasError;
        public bool HasError
        {
            get
            {
                return _HasError;
            }
            set
            {
                _HasError = value;
            
            }
        }

        public int CreatedBy { get; set; }
    }


    public class IDBase
    {
        public IDBase()
        {
            APIError = new APIError(ErrorType.None);
        }
        public int id { get; set; }
        public string DocInstanceID { get; set; }
        public APIError APIError { get; set; }
        public string UserName { get; set; }
        public string UserID { get; set; }
    }

    public class NameBase : IDBase
    {
        public string Name { get; set; }
    }

    public class AddressBase : NameBase
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PostCode { get; set; }

    }

    public class BankAccountBase : AddressBase
    {
        public string SortCode { get; set; }
        public string AccountNumber { get; set; }
    }

    public class PersonBase : AddressBase
    {
        public string FullName { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string LandlinePhone { get; set; }

        public void SetFullName()
        {
            string fullname = "";
            if (!string.IsNullOrWhiteSpace(Title))
            {
                fullname += $"{Title}";
            }
            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                fullname += $" {FirstName}";
            }
            if (!string.IsNullOrWhiteSpace(LastName))
            {
                fullname += $" {LastName}";
            }

            FullName = fullname;
        }
    }

    public class GenericJSONResponseObject
    {
        public GenericJSONResponseObject()
        {
            MemoryLocation = new VidexIntercomMemoryLocation();
            intercom = new VidexIntercom();
        }
        public string JSON { get; set; }
        public string ResponseMessage { get; set; }
        public VidexIntercom intercom { get; set; }
        public VidexIntercomMemoryLocation MemoryLocation { get; set; }

        public void Stringify()
        {
            JSON = Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

     
    }
    public class ViewModelBase
    {
        public ViewModelBase(ViewModelLevel level, int SelectedEstateID = 0)
        {
            Properties = new List<Properties>();
            SelectedProperty = new Properties(PropertyTypes.None);
            SelectedProperty.ID = SelectedEstateID;
            Level = level;
            owner = new Owner();
            NotesList = new List<APINotes>();
        }

        public string AddButtonIconCss { get { return "fa-solid fa-square-plus"; } }
        public string ConfirmButtonIconCss { get { return "fa-solid fa-square-check"; } }
        public string CancelButtonIconCss { get { return "fa-solid fa-xmark"; } }
        public string EditButtonIconCss { get { return "fa-regular fa-pen-to-square"; } }
        public string saveButtonIconCss { get { return "fa-solid fa-floppy-disk"; } }
        public string RoleName { get; set; }

        public List<APINotes> NotesList { get; set; }

        public ViewModelLevel Level { get; set; }
        public Properties SelectedProperty { get; set; }

        private string _SelectedEstateID;
        
        public string SelectedEstateID
        {
            get { return _SelectedEstateID; }
            set
            {
                _SelectedEstateID = value;
                if (!string.IsNullOrWhiteSpace(_SelectedEstateID))
                {
                    IsPropertySelected = true;
                }
            }
        }
        public string SelectedEstateName { get; set; }
        private string _SelectedUnitID;
        public string SelectedUnitID
        {
            get { return _SelectedUnitID; }
            set
            {
                _SelectedUnitID = value;
                if(!string.IsNullOrWhiteSpace(_SelectedUnitID))
                {
                    IsPropertySelected = true;
                }
            }
        }
        public string SelectedUnitName { get; set; }
        public string NameofUser { get; set; }

        public string ViewName { get; set; }
        public string ControllerName { get; set; }

        public List<Properties> Properties { get; set; }


        public int NotificationCount { get; set; } = 0;
        public bool IsPropertySelected { get; private set; }
       public Owner owner { get; set; }    

        public async Task SetBaseDataAsync(string userid, string email)
        {
            //set the role
            List<string> roles = await BaseHelpers.UserRolesList(userid);

            foreach(string role in roles)
            {
                RoleName = role;
            }

            //set the NameofUser
            if(RoleName == "Customer" || RoleName == "Client")
            {
                owner = await OwnerMethods.GetOwnerByEmail(email);
                NameofUser = owner.FullName;
                



            } else
            {
                APIUser user = await UserMethods.GetUserByEmail(email);
                NameofUser = user.FullName;

          

            }

            if (Level == ViewModelLevel.Estate)
            {
                if (string.IsNullOrWhiteSpace(SelectedEstateID))
                {
                    Properties = await PropertyMethods.GetOwnedEstatesAsync(owner.id);
                    if (Properties.Count == 1)
                    {
                        SelectedProperty = Properties[0];
                        SelectedProperty.ID = 1;// Properties[0].UniqueID;
                        SelectedProperty.Address1 = Properties[0].Address1;

                        SelectedEstateID = Properties[0].UniqueID;
                        SelectedEstateName = Properties[0].Address1;
                        IsPropertySelected = true;
                    }
                }
            } else if(Level == ViewModelLevel.Unit)
            {
                if(string.IsNullOrWhiteSpace(SelectedUnitID))
                {
                    Properties = Models.PropertyMethods.GetAllOwnedProperties(owner.id);
                    if (Properties.Count == 1)
                    {
                        SelectedProperty = Properties[0];
                        SelectedProperty.ID = Properties[0].ID;
                        SelectedProperty.Address1 = Properties[0].Address1;

                        SelectedUnitID = Properties[0].UniqueID;
                        SelectedEstateName = Properties[0].Address1;
                        IsPropertySelected= true;
                    }
                }
            }


            //NotificationCount = 2;
        }
    }

    public enum ViewModelLevel
    {
        Estate,
        Unit,
        none
    }

    public static class BaseHelpers
    {
        public async static Task<List<string>> UserRolesList(string userid)
        {
                GlobalVariables.CS =
                WebConfigurationManager.ConnectionStrings["AccessConnection"].ConnectionString;

            string q = "select AspNetRoles.Name from AspNetUserRoles " +
                        "inner join AspNetRoles on AspNetUserRoles.RoleId = AspNetRoles.Id " +
                        $"where AspNetUserRoles.UserId = '{userid}'";

            DataTable dt = await 
                GlobalVariables.GetConnection().Connection.GetDataTableAsync(q);

            List<string> r = new List<string>();
            if(dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "Error")
            {
                foreach(DataRow dr in dt.Rows)
                {
                    r.Add(dr[0].ToString());
                }
            }


            GlobalVariables.CS =
               WebConfigurationManager.ConnectionStrings["DeployConnection"].ConnectionString;
            return r;
        }
    }
}