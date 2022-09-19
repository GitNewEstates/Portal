using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class OwnedUnitsViewModel
    {
        //view model used for the list of owned units by an owner

        public OwnedUnitsViewModel(int _ownerId, int _estateid = 0, bool DisplaySCBalance = true)
        {
            OwnerID = _ownerId;
            EstateID = _estateid;
            displayBalance = DisplaySCBalance;
            OwnedProperties = new List<OwnedPropertyListViewObject>();
        }
        int EstateID { get; set; }
        int OwnerID { get; set; }
        bool displayBalance { get; set; }
        public List<OwnedPropertyListViewObject> OwnedProperties { get; set; }
        public async Task GetData()
        {
            
                List<Models.Units> units = new List<Models.Units>();
                units = await
                    Models.UnitMethods.GetCurrentOwnedUnits(OwnerID, EstateID, true);
                if (units != null)
                {
                    if (units.Count > 0)
                    {
                        if (!units[0].APIError.HasError)
                        {
                            foreach (var unit in units)
                            {
                                string _text = $"{unit.Name}";
                                if (displayBalance)
                                {
                                _text += $" - {ControlsDLL.ControlActions.DoubelToCurrencyString2DP(unit.Balance)}";
                                }
                                OwnedProperties.Add(new OwnedPropertyListViewObject
                                {
                                    id = unit.id,
                                    text = _text
                                });
                            }
                        }
                    }
                }
            }
        
    }

    public class OwnedPropertyListViewObject : ListViewBase
    {
        public OwnedPropertyListViewObject()
        {
            //icon used in the list of owned units
            icon = "fa-solid fa-building-user";
        }

    }

    public class OpenFundBVAChartViewModel
    {
        public OpenFundBVAChartViewModel(int estateid)
        {

            estate = new APIEstates();
            estate.id = estateid;
            BudgetActualDataList = new List<BudgetActualChartData>();
        }
        public APIEstates estate { get; set; }
        public List<BudgetActualChartData> BudgetActualDataList { get; set; }
        public double XAxisMaxValue { get; set; }
        public double ClientXAxisMaxValue { get; set; }
        public string ClientBVAChartHeight { get; set; }
        public string BudgetLabelAmount { get; set; }   
        public async Task GetData()
        {
            estate = await APIEstateMethods.GetOpenFundBudgetAndSpendTotals(estate.id);
            string BudgetLabelName = $"£{estate.OpenFundTotalBudget}";
            string ActualLabelName = $"£{estate.OpenFundTotalCost}";

            double TwentyPercentofBudget = estate.OpenFundTotalBudget * 0.2;
            double TwentyPercentofActual = estate.OpenFundTotalCost * 0.2;

            //if the actual spend is less than 20% of the budget then 
            //set the amount to 20% so that the data series renders properly
            if (estate.OpenFundTotalBudget > estate.OpenFundTotalCost)
            {
                if (estate.OpenFundTotalCost < TwentyPercentofBudget)
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

            }
            else if (estate.OpenFundTotalCost > estate.OpenFundTotalBudget)
            {
                XAxisMaxValue = estate.OpenFundTotalCost + 1000;
            }
            else
            {
                XAxisMaxValue = 12000;
            }
        }
    }

    public class AttendanceLogAccordianViewModel
    {
        public AttendanceLogAccordianViewModel(int _EstateID, int _MaxRecords = 0)
        {
            EstateID = _EstateID;
            AttendanceVisitCollection = new List<Syncfusion.EJ2.Navigations.AccordionAccordionItem>();
            MaxRecords = _MaxRecords;
        }

        int EstateID { get; set; }
        int MaxRecords { get; set; }
        public List<Syncfusion.EJ2.Navigations.AccordionAccordionItem> AttendanceVisitCollection { get; set; }
        public string AttendanceErrorMessage { get; set; }
        public async Task GetData()
        {
            List<AttendanceVisits> Attendances =
                await AttendanceVisitMethods.GetAttendanceVisitListAsync(EstateID);
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
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
                            if (MaxRecords > 0)
                            {
                                if (attendancecount > MaxRecords)
                                {
                                    attendancecount = MaxRecords;
                                }
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
                                });
                            }

                        }

                    }
                    else
                    {
                        AttendanceErrorMessage = "Error retrieving recent attendance visit logs";
                    }
                }
            }
        }
    }
}