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

    public class ViewModelBase
    {
        public ViewModelBase()
        {
            Properties = new List<Properties>();
            SelectedProperty = new Properties(PropertyTypes.None);
            owner = new Owner();
        }
        public string RoleName { get; set; }
        public Properties SelectedProperty { get; set; }
        public string NameofUser { get; set; }

        public string ViewName { get; set; }
        public string ControllerName { get; set; }

        public List<Properties> Properties { get; set; }


        public int NotificationCount { get; set; } = 0;

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
                Properties = await PropertyMethods.GetOwnedEstatesAsync(owner.id);
                if(Properties.Count == 1)
                {
                    SelectedProperty = Properties[0];
                    SelectedProperty.ID = Properties[0].ID;
                    SelectedProperty.Address1 = Properties[0].Address1;
                }



            } else
            {
                APIUser user = await UserMethods.GetUserByEmail(email);
                NameofUser = user.FullName;

          

            }

            NotificationCount = 2;
        }
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