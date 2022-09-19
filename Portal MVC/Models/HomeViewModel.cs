using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel(ViewModelLevel level):base(level)
        {
            
            ClientFinancialListView = new List<ClientFinancialListView>();
            RepairAccordianObjects = new List<Syncfusion.EJ2.Navigations.AccordionAccordionItem>();
           
            NotificationObjectList = new List<object>();
            MaintOpDashViewModel = new MaintenanceOpDashViewModel();
            
            
        }
        public OpenFundBVAChartViewModel OpenFundBVAChartViewModel { get; set; }    
        public MaintenanceOpDashViewModel MaintOpDashViewModel { get; set; }

        public AttendanceLogAccordianViewModel AttendanceLogAccordianViewModel { get; set; }
        public async Task LoadCustomerDashboardDataAsync(string useremail)
        {
            
            APIEstates estate = await APIEstateMethods.GetEstateAsync(0, SelectedEstateID);
            NotificationviewModel = new NotificationSettingViewModel(useremail, estate.Name);

            OpenFundBVAChartViewModel = new OpenFundBVAChartViewModel(estate.id);
            await OpenFundBVAChartViewModel.GetData();

            if (owner != null)
            {
                 OwnedUnitsViewModel = new OwnedUnitsViewModel(owner.id, SelectedProperty.ID, true);
                await OwnedUnitsViewModel.GetData();
              
            }

            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            List<APIRepairs> 
                repairs = await RepairExtensions.GetRepairsList(estate.id, true) ;

            //top 5 only
            if(repairs != null)
            {
                //test for error
                if (repairs.Count > 0)
                {
                    if (!repairs[0].APIError.HasError)
                    {

                       

                        if (RepairAccordianObjects == null)
                        {
                            RepairAccordianObjects = new List<Syncfusion.EJ2.Navigations.AccordionAccordionItem>();
                        }
                        int repairMax = repairs.Count;
                        if (repairMax > 5)
                        {
                            repairMax = 5;
                        }

                        for (int i = 0; i <= repairMax - 1; i++)
                        {
                            RepairAccordianObjects.Add(new Syncfusion.EJ2.Navigations.AccordionAccordionItem
                            {
                                Header = repairs[i].RepairTitle,
                                Content = repairs[i].RepairDetails + $"</br></br><a href=\"{domainName}/RepairsMaintenance/RepairDetail?RepairID={repairs[i].ID}\">View More</a>"
                            }); 
                        }
                    } else
                    {
                        //error occurred
                        RepairErrorMessage = repairs[0].APIError.Message;
                    }
                }
            }

            //Attendance Logs
            AttendanceLogAccordianViewModel = new AttendanceLogAccordianViewModel(estate.id, 5);
            await AttendanceLogAccordianViewModel.GetData();

            if(NotificationObjectList != null)
            {
                NotificationObjectList.Add(new 
                {
                    id = 1,
                    name = "New Repair Raised",
                    hasChild = true
                });

                NotificationObjectList.Add(new
                {
                    id = 2,
                    pid = 1,
                    name = "Apply to all users",
                    hasChild = true
                });

                NotificationObjectList.Add(new
                {
                    id = 3,
                    name = "New Payment Raised",
                    hasChild = true
                });

                NotificationObjectList.Add(new
                {
                    id = 4,
                    pid = 3,
                    name = "Apply to all Units",
                    hasChild = true
                });
            }

        }
        public async Task LoadClientDataAsync()
        {
            APIEstates estate = new APIEstates
            {
                //id = SelectedProperty.ID
                id = 32
            };
            //rob - power tripped. 24 block 4.

            //await estate.GetOpenFundBVAList();
            //double chartheight = 150;
          
            //if (estate.OpenFundList != null)
            //{
            //    if(estate.OpenFundList.Count > 0)
            //    {
            //        if(estate.OpenFundList[0].APIError != null)
            //        {
            //            if (estate.OpenFundList[0].APIError.HasError)
            //            {
            //                ClientBVAChartErrorMessage = estate.OpenFundList[0].APIError.Message;

            //            } else
            //            {
                            
            //                ClientBVABudget = new List<BudgetActualChartData>();
            //                ClientBVAActual = new List<BudgetActualChartData>();

            //                for (int i = 0; i <= estate.OpenFundList.Count - 1; i++)
            //                {

            //                    chartheight += 50;
            //                    string BudgetLabelName = ControlsDLL.ControlActions.DoubelToCurrencyString2DP(estate.OpenFundList[i].TotalBudget);
            //                    string ActualLabelName = ControlsDLL.ControlActions.DoubelToCurrencyString2DP(estate.OpenFundList[i].TotalSpend);

            //                    double TwentyPercentofBudget = estate.OpenFundList[i].TotalBudget * 0.1;
            //                    double TwentyPercentofActual = estate.OpenFundList[i].TotalSpend * 0.1;

            //                    //if the actual spend is less than 20% of the budget then 
            //                    //set the amount to 20% so that the data series renders properly
            //                    if (estate.OpenFundList[i].TotalBudget > estate.OpenFundList[i].TotalSpend)
            //                    {
            //                        if (estate.OpenFundList[i].TotalSpend < TwentyPercentofBudget)
            //                        {
            //                            estate.OpenFundList[i].TotalSpend = TwentyPercentofBudget;
            //                        }
            //                    }
            //                    //else if ()
            //                    //{
            //                    //    //if budget is less than 20% of actual spend
            //                    //}


            //                    BudgetActualChartData BudgetData = new BudgetActualChartData
            //                    {
            //                        Name = estate.OpenFundList[i].FundName,
            //                        Amount = estate.OpenFundList[i].TotalBudget,
            //                        LabelName = BudgetLabelName
                                    
            //                    };

            //                    BudgetActualChartData ActualData = new BudgetActualChartData
            //                    {
            //                        Name = estate.OpenFundList[i].FundName,
            //                        Amount = estate.OpenFundList[i].TotalSpend,
            //                        LabelName = ActualLabelName
            //                    };

            //                    ClientBVABudget.Add(BudgetData);
            //                    ClientBVAActual.Add(ActualData);

            //                    //sets the x axis value.
            //                    double BudgetActualHighestVal = 0;
            //                    if(estate.OpenFundList[i].TotalBudget > estate.OpenFundList[i].TotalSpend)
            //                    {
            //                        BudgetActualHighestVal = estate.OpenFundList[i].TotalBudget;
            //                    } else
            //                    {
            //                        BudgetActualHighestVal = estate.OpenFundList[i].TotalSpend;
            //                    }

            //                    if(BudgetActualHighestVal > ClientXAxisMaxValue)
            //                    {
            //                        ClientXAxisMaxValue = BudgetActualHighestVal;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}


            //ClientBVAChartHeight = $"{chartheight}px";

            //Client financial Summary
            await estate.GetCurrentCashPosition();
            await estate.GetInvoiceAwaitingPaymentValue();
            await estate.GetUnauthorisedInvoiceValue();
            await estate.GetOutstandingdPOValue();

            string DeductionRightMarginValue = "25px";

            ClientFinancialListView.Add(new Models.ClientFinancialListView
            {
                text = $"Current Cash:",
                Elementid = "CurrentCash",
                icon = "fa-solid fa-sterling-sign",
                value = ControlsDLL.ControlActions.DoubelToCurrencyString2DP(estate.CurrentCashPosition),
                IconCss = "margin-right: 5px;",
                ValueCss = "float: right;"
            });

           

            ClientFinancialListView.Add(new Models.ClientFinancialListView
            {
                text = $"Invoices Awaiting Payment:",
                Elementid = "InvoicesAwaitingPayment",
                icon = "fa-solid fa-file-invoice",
                value = ControlsDLL.ControlActions.DoubelToCurrencyString2DP(estate.InvoicesAwaitingPaymenteValue),
                IconCss = "margin-right: 5px;",
                ValueCss = $"float: right; margin-right: {DeductionRightMarginValue};"
            });
            ClientFinancialListView.Add(new Models.ClientFinancialListView
            {
                text = $"Unauthorised Invoices:",
                Elementid = "UnauthedInvoices",
                icon = "fa-solid fa-ban",
                value = ControlsDLL.ControlActions.DoubelToCurrencyString2DP(estate.UnauthorisedInvoiceValue),
                IconCss = "margin-right: 5px;",
                ValueCss = $"float: right; margin-right: {DeductionRightMarginValue};"
            });

          
            ClientFinancialListView.Add(new Models.ClientFinancialListView
            {
                text = $"Unbilled Purchase Orders:",
                Elementid = "UnbilledPurchaseOrders",
                icon = "fa-solid fa-money-check",
                value = ControlsDLL.ControlActions.DoubelToCurrencyString2DP(estate.OutstandPOTotalValue),
                IconCss = "margin-right: 5px;",
                ValueCss = $"float: right; margin-right: {DeductionRightMarginValue};"
            });
            
            ClientFinancialListView.Add(new Models.ClientFinancialListView
            {
                text = $"Net Cash Position:",
                Elementid = "UnbilledPurchaseOrders",
                icon = "fa-solid fa-sterling-sign",
                value = ControlsDLL.ControlActions.DoubelToCurrencyString2DP(estate.NetCashPosition),
                IconCss = "margin-right: 5px;",
                ValueCss = $"float: right;"
            });

            NotificationviewModel.NewRepairNotification = true;

        }

        public NotificationSettingViewModel NotificationviewModel { get; set; }

        public string RepairErrorMessage { get; set; }

        public string ClientBVAChartErrorMessage { get; set; }  
        public object anonObj { get; set; }
        public string Name { get; set; }
        

        public List<object> NotificationObjectList { get; set; }
        
        public List<BudgetActualChartData> ClientBVABudget { get; set; }
        public List<BudgetActualChartData> ClientBVAActual { get; set; }

        public List<ClientFinancialListView> ClientFinancialListView { get; set; }

        
        public List<Syncfusion.EJ2.Navigations.AccordionAccordionItem> RepairAccordianObjects { get; set; }

        

    }

    public class UnpaidInvoiceDetailViewModel : ViewModelBase
    {
        public UnpaidInvoiceDetailViewModel(ViewModelLevel level = ViewModelLevel.Estate) : base(level)
        {
            UnpaidExpenditureList = new List<Expenditure>();
        }
        public List<Expenditure> UnpaidExpenditureList { get; set; }
        public async Task GetUnpaidInvoices()
        {
            UnpaidExpenditureList = await ExpenditureMethods.GetUnpaidExpenditureAsync(SelectedProperty.ID);
        }
    }

    public class UnauthorisedExpenditureViewModel : ViewModelBase
    {
        public UnauthorisedExpenditureViewModel(ViewModelLevel level = ViewModelLevel.Estate) : base(level)
        {
            UnAuthExpenditureList = new List<Expenditure>();
        }
        public List<Expenditure> UnAuthExpenditureList { get; set; }

        public async Task GetUnauthExpenditureList()
        {
            UnAuthExpenditureList = await ExpenditureMethods.GetUnauthorisedExpenditureAsync(SelectedProperty.ID);
        }
    }

    public class OutstandPOListViewModel : ViewModelBase
    {
        public OutstandPOListViewModel(ViewModelLevel level = ViewModelLevel.Estate) : base(level)
        {
            OutstaningPurchaseOrderList = new List<APIPurchaseOrders>();
        }
        public List<APIPurchaseOrders> OutstaningPurchaseOrderList { get; set; }

        public async Task SetOutstandingPurchaseOrderList()
        {
            OutstaningPurchaseOrderList = await 
                APIPurchaseOrderMethods.GetOpenPurchaseOrderList(SelectedProperty.ID);
        }
    }

    public class ClientBudgetActualChartData
    {
        public ClientBudgetActualChartData()
        {
            BudgetData = new List<BudgetActualChartData>();
            ActualData = new List<BudgetActualChartData>();

        }

        public string Name { get; set; }
        public List<BudgetActualChartData> BudgetData { get; set; }
        public List<BudgetActualChartData> ActualData { get; set; }
    }

    //public class ClientBudgetActualChartData
    //{
    //    public string Name { get; set; }
    //    public double Budget { get; set; }
    //    public double Actual { get; set; }

    //    public string color { get; set; }

    //    public string LabelName { get; set; }

    //}


    public class BudgetActualChartData
    {
        public string Name { get; set; }
        public double Amount { get; set; }

        public string  color { get; set; }

        public string LabelName { get; set; }   
        
    }

    public class ListViewBase
    {
        public int id { get; set; }
        public string text { get; set; }

        public string icon { get; set; }
    }

    public class ChildListViewBase : ListViewBase
    {

    }

   

    public class ClientFinancialListView :ListViewBase
    {
        public string value { get; set; }
        public string IconCss { get; set; }
        public string ValueCss { get; set; }
        public string Elementid { get; set;}
    }

    public class NotificationListViewObject : ListViewBase
    {
        public NotificationListViewObject()
        {
            child = new List<object>();
        }
        public List<object> child { get; set; }
    }

    public class NotificationSettingViewModel 
    {
        public NotificationSettingViewModel(string useremail, string estatename)
        {
            _email = useremail;
            _estateName = estatename;
        }

        private string _email { get; set; }
        private string _estateName { get; set; }
        public bool NewRepairNotification { get; set; }
        public bool NewRepairApplyToAllUnits { get; set; }

        public string NewRepairToolTip { get { return $"You will receive an email to {_email} whenever a new repair is raised at {_estateName}."; } }

        public bool NewInsuranceNotification { get; set; }
        public string NewInsuranceToolTip { get { return $"You will receive an email to {_email} whenever a new insurance policy is added to {_estateName}."; } }
        public bool NewAttendanceLogNotification { get; set; }
        public string NewAttendanceLogToolTip { get { return $"You will receive an email to {_email} whenever a new Attendance Log is submitted for {_estateName}."; } }
    }

   
    public class AddOwnersViewModel : ViewModelBase
    {
        public AddOwnersViewModel(ViewModelLevel level = ViewModelLevel.Estate) : base(level)
        {
            UnitList = new List<Units>();
            Titles = new List<string>
            {
                "Mr",
                "Mrs",
                "Miss",
                "Dr"

            };
            //
            Owner = new Owner();
            TabHeaders = new List<Syncfusion.EJ2.Navigations.TabTabItem>
            {
                new Syncfusion.EJ2.Navigations.TabTabItem 
                { 
                    Header = new Syncfusion.EJ2.Navigations.TabHeader 
                        { Text = "New Owner Details"}, 
                    Content = "#add-owner-details" 
                },
                //new Syncfusion.EJ2.Navigations.TabTabItem
                //{
                //    Header = new Syncfusion.EJ2.Navigations.TabHeader
                //        { Text = "Select Units"},
                //    Content = "#select-owned-units"
                //},

            };
        }
        public List<Units> UnitList { get; set; }
        public List<string> Titles { get; private set; }
        public List<Syncfusion.EJ2.Navigations.TabTabItem> TabHeaders { get; set; }
        public Owner Owner { get; set; }
        public async Task LoadDataAsync()
        {
            //UnitList = await UnitMethods.GetUnitsAsync(SelectedProperty.ID);
        }
    }

    public class OwnerListViewModel : ViewModelBase
    {
        public OwnerListViewModel(ViewModelLevel level = ViewModelLevel.Estate) : base(level)
        {
            OwnerList = new List<Owner>();
          
        }
        public List<Owner> OwnerList { get; set; }
        public List<string> Titles { get; private set; }
        public async Task LoadDataAsync()
        {
            OwnerList = await OwnerMethods.GetAllAsync();
        
        }
    }

    public class OwnerDetailViewModel : ViewModelBase
    {
        public OwnerDetailViewModel(ViewModelLevel level = ViewModelLevel.none) : base(level)
        {
            Owner = new Owner();
            Titles = new List<string>
            {
                "Mr",
                "Mrs",
                "Miss",
                "Dr"

            };
            OwnedProperties = new List<OwnedPropertyListViewObject>();
        }
        public List<string> Titles { get; private set; }
        public List<Units> UnitList { get; set; }
        public Owner Owner { get; set; }
        public List<OwnedPropertyListViewObject> OwnedProperties { get; set; }
        public DateTime OwnershipStartMaxDate { get { return DateTime.Today; } }
        public async Task LoadAsync(int id)
        {
            Owner = await OwnerMethods.GetOwnerByID(id);
            if (Owner != null)
            {
                List<Models.Units> units = new List<Models.Units>();
                units = await
                    Models.UnitMethods.GetCurrentOwnedUnits(Owner.id, 0, true);
                if (units != null)
                {
                    if (units.Count > 0)
                    {
                        if (!units[0].APIError.HasError)
                        {
                            foreach (var unit in units)
                            {
                                OwnedProperties.Add(new OwnedPropertyListViewObject
                                {
                                    id = unit.id,
                                    text = $"{unit.Name} - {ControlsDLL.ControlActions.DoubelToCurrencyString2DP(unit.Balance)}"
                                });
                            }
                        }
                    }
                }
            }

            UnitList = await UnitMethods.GetUnitsAsync();   
        }
    }

    public class MaintenanceOpDashViewModel : ViewModelBase
    {
        public MaintenanceOpDashViewModel(ViewModelLevel level = ViewModelLevel.none) : base(level)
        {

        }
    }

    public class TabcontrolTabHeaders
    {
        public string Text { get; set; }
    }
  
}