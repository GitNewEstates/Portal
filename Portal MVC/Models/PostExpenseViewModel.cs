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
            SupplierList = new List<SelectListItem>();
            FundList = new List<SelectListItem>();
            PaymentTypeList = new List<SelectListItem>();
        }
        public List<SelectListItem> SupplierList { get; set; }
        public List<SelectListItem> PaymentTypeList { get; set; }
        public List<SelectListItem> FundList { get; set; }
        public int SelectedSupplierID { get; set; }

        public int SelectedFundID { get; set;   }

        public string PaymentMethod { get; set; }

        public DateTime ExpenseDate { get; set; }

        public DateTime MaxDate { get; set; }

        public double Cost { get; set; }

        public async Task SetLists()
        {
            

            List<Supplier> suppliers = await SupplierMethods.GetSupplierListAsync();

            SupplierList = new List<SelectListItem>();

            foreach (Supplier s in suppliers)
            {
                bool IsSelected = false;
                if (s.id == SelectedSupplierID && SelectedSupplierID > 0)
                {
                    IsSelected = true;
                }

                SupplierList.Add(new SelectListItem
                {
                    Text = s.Name,
                    Value = s.id.ToString(),
                    Selected = IsSelected
                });
            }

            //id always needs to be NEM EstateID
            List<Fund> _FundList = await FundMethods.GetFundList(24);
            foreach (Fund s in _FundList)
            {



                FundList.Add(new SelectListItem
                {
                    Text = s.FundName,
                    Value = s.id.ToString(),
                    Selected = false
                }) ;
            }

            PaymentTypeList.Add(new SelectListItem
            {
                Text = "Debit Card",
                Selected = false
            });
            PaymentTypeList.Add(new SelectListItem
            {
                Text = "Cash",
                Selected = false
            });

}
    }
}