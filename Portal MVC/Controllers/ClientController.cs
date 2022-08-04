using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using System.Web.Mvc;
using Portal_MVC.Models;
using System.Threading.Tasks;

namespace Portal_MVC.Controllers
{
    [Authorize(Roles = "Client")]

    public class ClientController : Controller
    {
        // GET: Client
        public async Task< ActionResult> UnpaidInvoiceDetail(int estateID = 0)
        {
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();


            UnpaidInvoiceDetailViewModel vm = new UnpaidInvoiceDetailViewModel();
            await vm.SetBaseDataAsync(id, email);

            //testing value
            vm.SelectedProperty.ID = 32;

            await vm.GetUnpaidInvoices();

            return View(vm);
        }

        public async Task<ActionResult> unauthorisedInvoiceDetail(int estateID = 0)
        {
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();

            

            UnauthorisedExpenditureViewModel vm = new UnauthorisedExpenditureViewModel();

            //testing value
            vm.SelectedProperty.ID = 32;
            

            await vm.SetBaseDataAsync(id, email);
            await vm.GetUnauthExpenditureList();
            return View("UnauthorisedInvoiceDetail", vm);
        }

        public async Task<ActionResult> OutstandingPODetail(int estateID = 0)
        {
            var id = User.Identity.GetUserId();
            var email = User.Identity.GetUserName();



            OutstandPOListViewModel vm = new OutstandPOListViewModel();

            //testing value
            vm.SelectedProperty.ID = 32;


            await vm.SetBaseDataAsync(id, email);
            await vm.SetOutstandingPurchaseOrderList();
            return View(vm);
        }
    }
}