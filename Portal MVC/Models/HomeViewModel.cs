using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel()
        {
            BudgetActualDataList = new List<BudgetActualChartData>();
            ClientFinancialListView = new List<ClientFinancialListView>();
            OwnedProperties = new List<OwnedPropertyListViewObject>();
            RepairAccordianObjects = new List<Syncfusion.EJ2.Navigations.AccordionAccordionItem>();
            AttendanceVisitCollection = new List<Syncfusion.EJ2.Navigations.AccordionAccordionItem>();
            NotificationObjectList = new List<object>();
        }
       
        public async Task LoadCustomerDashboardDataAsync()
        {

            APIEstates estate = new APIEstates { id = SelectedProperty.ID };
            estate = await APIEstateMethods.GetOpenFundBudgetAndSpendTotals(estate.id);

            //
            string BudgetLabelName = $"£{estate.OpenFundTotalBudget}";
            string ActualLabelName = $"£{estate.OpenFundTotalCost}";

            double TwentyPercentofBudget = estate.OpenFundTotalBudget * 0.2;
            double TwentyPercentofActual = estate.OpenFundTotalCost * 0.2;

            //if the actual spend is less than 20% of the budget then 
            //set the amount to 20% so that the data series renders properly
            if(estate.OpenFundTotalBudget > estate.OpenFundTotalCost)
            {
                if(estate.OpenFundTotalCost < TwentyPercentofBudget)
                {
                    estate.OpenFundTotalCost = TwentyPercentofActual;
                }
            }




            estate.OpenFundTotalCost = estate.OpenFundTotalBudget * 0.2;
            BudgetLabelAmount = $"{ControlsDLL.ControlActions.DoubelToCurrencyString2DP(estate.OpenFundTotalCost)}";


            if (BudgetActualDataList == null)
            {
                BudgetActualDataList = new List<BudgetActualChartData>();
            }
            BudgetActualDataList.Add(new BudgetActualChartData
            {
                Name = "Spend",
                Amount = estate.OpenFundTotalCost,
                color = "#0591bc",
                LabelName = ActualLabelName

            });

            BudgetActualDataList.Add(new BudgetActualChartData
            {
                Name = "Budget",
                Amount = estate.OpenFundTotalBudget,
                color = "grey",
                LabelName = BudgetLabelName
            });

            if (estate.OpenFundTotalBudget > estate.OpenFundTotalCost)
            {
                XAxisMaxValue = estate.OpenFundTotalBudget + 1000;

            } else if (estate.OpenFundTotalCost > estate.OpenFundTotalBudget)
            {
                XAxisMaxValue = estate.OpenFundTotalCost + 1000;
            } else
            {
                XAxisMaxValue = 12000;
            }

           

            if (owner != null)
            {
                List<Models.Units> units = new List<Models.Units>();
                units = await
                    Models.UnitMethods.GetCurrentOwnedUnits(owner.id, SelectedProperty.ID, true);
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

            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            List<APIRepairs> 
                repairs = await RepairExtensions.GetRepairsList(SelectedProperty.ID, true) ;

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

            List<AttendanceVisits> Attendances =
                await AttendanceVisitMethods.GetAttendanceVisitListAsync(SelectedProperty.ID);

            if (Attendances != null)
            {

                if (Attendances.Count > 0)
                {
                    if (Attendances[0].APIError != null)
                    {
                        if (Attendances[0].APIError.HasError)
                        {
                            AttendanceErrorMessage = Attendances[0].APIError.Message;
                        }
                        else
                        {
                            int attendancecount = Attendances.Count;
                            if (attendancecount > 5)
                            {
                                attendancecount = 5;
                            }

                            if (AttendanceVisitCollection == null)
                            {
                                AttendanceVisitCollection = new List<Syncfusion.EJ2.Navigations.AccordionAccordionItem>();
                            }

                            for (int i = 0; i <= attendancecount - 1; i++)
                            {
                                AttendanceVisitCollection.Add(new Syncfusion.EJ2.Navigations.AccordionAccordionItem
                                {
                                    Header = $"{Utils.DateFormatLong(Attendances[i].VisitDate)} {Attendances[i].AttendanceType.Name}",
                                    Content = $"{Attendances[i].VisitDescription} </br></br> <a href=\"{domainName}/caretaking/AttendanceDetail?VisitID={Attendances[i].id}\">View More</a>"
                                }) ;
                            }

                        }

                    }
                    else
                    {
                        AttendanceErrorMessage = "Error retrieving recent attendance visit logs";
                    }
                }
            }

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

            await estate.GetOpenFundBVAList();
            double chartheight = 150;
          
            if (estate.OpenFundList != null)
            {
                if(estate.OpenFundList.Count > 0)
                {
                    if(estate.OpenFundList[0].APIError != null)
                    {
                        if (estate.OpenFundList[0].APIError.HasError)
                        {
                            ClientBVAChartErrorMessage = estate.OpenFundList[0].APIError.Message;

                        } else
                        {
                            
                            ClientBVABudget = new List<BudgetActualChartData>();
                            ClientBVAActual = new List<BudgetActualChartData>();

                            for (int i = 0; i <= estate.OpenFundList.Count - 1; i++)
                            {

                                chartheight += 50;
                                string BudgetLabelName = ControlsDLL.ControlActions.DoubelToCurrencyString2DP(estate.OpenFundList[i].TotalBudget);
                                string ActualLabelName = ControlsDLL.ControlActions.DoubelToCurrencyString2DP(estate.OpenFundList[i].TotalSpend);

                                double TwentyPercentofBudget = estate.OpenFundList[i].TotalBudget * 0.1;
                                double TwentyPercentofActual = estate.OpenFundList[i].TotalSpend * 0.1;

                                //if the actual spend is less than 20% of the budget then 
                                //set the amount to 20% so that the data series renders properly
                                if (estate.OpenFundList[i].TotalBudget > estate.OpenFundList[i].TotalSpend)
                                {
                                    if (estate.OpenFundList[i].TotalSpend < TwentyPercentofBudget)
                                    {
                                        estate.OpenFundList[i].TotalSpend = TwentyPercentofBudget;
                                    }
                                }
                                //else if ()
                                //{
                                //    //if budget is less than 20% of actual spend
                                //}


                                BudgetActualChartData BudgetData = new BudgetActualChartData
                                {
                                    Name = estate.OpenFundList[i].FundName,
                                    Amount = estate.OpenFundList[i].TotalBudget,
                                    LabelName = BudgetLabelName
                                    
                                };

                                BudgetActualChartData ActualData = new BudgetActualChartData
                                {
                                    Name = estate.OpenFundList[i].FundName,
                                    Amount = estate.OpenFundList[i].TotalSpend,
                                    LabelName = ActualLabelName
                                };

                                ClientBVABudget.Add(BudgetData);
                                ClientBVAActual.Add(ActualData);

                                //sets the x axis value.
                                double BudgetActualHighestVal = 0;
                                if(estate.OpenFundList[i].TotalBudget > estate.OpenFundList[i].TotalSpend)
                                {
                                    BudgetActualHighestVal = estate.OpenFundList[i].TotalBudget;
                                } else
                                {
                                    BudgetActualHighestVal = estate.OpenFundList[i].TotalSpend;
                                }

                                if(BudgetActualHighestVal > ClientXAxisMaxValue)
                                {
                                    ClientXAxisMaxValue = BudgetActualHighestVal;
                                }
                            }
                        }
                    }
                }
            }


            ClientBVAChartHeight = $"{chartheight}px";

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

        }

        public string RepairErrorMessage { get; set; }
        public string AttendanceErrorMessage { get; set; }
        public string ClientBVAChartErrorMessage { get; set; }  
        public object anonObj { get; set; }
        public string Name { get; set; }
        public List<BudgetActualChartData> BudgetActualDataList { get; set; }

        public List<object> NotificationObjectList { get; set; }
        
        public List<BudgetActualChartData> ClientBVABudget { get; set; }
        public List<BudgetActualChartData> ClientBVAActual { get; set; }

        public List<ClientFinancialListView> ClientFinancialListView { get; set; }

        public List<OwnedPropertyListViewObject> OwnedProperties { get; set; }
        public List<Syncfusion.EJ2.Navigations.AccordionAccordionItem> RepairAccordianObjects { get; set; }

        public List<Syncfusion.EJ2.Navigations.AccordionAccordionItem> AttendanceVisitCollection { get; set; }

        public double XAxisMaxValue { get; set; }
        public double ClientXAxisMaxValue { get; set; }
        public string ClientBVAChartHeight { get; set; }
        public string BudgetLabelAmount { get; set; }
    }

    public class UnpaidInvoiceDetailViewModel : ViewModelBase
    {
        public UnpaidInvoiceDetailViewModel()
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
        public UnauthorisedExpenditureViewModel()
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
        public OutstandPOListViewModel()
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

    public class OwnedPropertyListViewObject : ListViewBase
    {
        public OwnedPropertyListViewObject()
        {
            icon = "fa-solid fa-building-user";
        }
        
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

   

  
}