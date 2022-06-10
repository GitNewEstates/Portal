using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace Portal_MVC.Models
{
    public class PostExpenseViewModel
    {
        public PostExpenseViewModel()
        {
            EstateList = new List<SelectListItem>();
        }
        public List<SelectListItem> EstateList { get; set; }
        public int SelectedPropertyid { get; set; }

        public DateTime ExpenseDate { get; set; }

        public DateTime MaxDate { get; set; }

        public async Task SetLists()
        {
            List<Properties> estates = await Models.PropertyMethods.GetAllEstatesAsync();
            EstateList = new List<SelectListItem>();
            foreach (Properties p in estates)
            {
                bool IsSelected = false;
                if (p.ID == SelectedPropertyid && SelectedPropertyid > 0)
                {
                    IsSelected = true;
                }

                EstateList.Add(new SelectListItem
                {
                    Text = p.Address1,
                    Value = p.ID.ToString(),
                    Selected = IsSelected
                });
            }


        }
    }
}