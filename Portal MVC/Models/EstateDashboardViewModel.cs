using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class EstateDashboardViewModel : ViewModelBase
    {
       
        public EstateDashboardViewModel(ViewModelLevel level = ViewModelLevel.Estate, int _estates = 1) : base(level)
        {
            
        }
        int EstateID { get; set; }

        public async Task LoadAsync()
        {
            PageTitle = $"{SelectedEstateName} - Estate Summary";
        }
    }
}