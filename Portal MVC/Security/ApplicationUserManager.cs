using Microsoft.AspNet.Identity;
using Portal_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_MVC.Security
{
    public class ApplicationUserManager : UserManager<User>
    {
        public ApplicationUserManager(IUserStore<User> userStore) : base(userStore)
        {

        }
    }
}