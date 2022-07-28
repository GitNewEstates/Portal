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
            BudgetActualDataList.Add(new BudgetActualChartData
            {
                Name = "Actual",
                Amount = 5000,
                color = "blue"

            });
            BudgetActualDataList.Add(new BudgetActualChartData
            {
                Name = "Budget",
                Amount = 10000, 
                color = "grey"
            });

           

            XAxisMaxValue = 12000;

        

            OwnedProperties = new List<OwnedPropertyListViewObject>();
           

            RepairAccordianObjects = new List<Syncfusion.EJ2.Navigations.AccordionAccordionItem>();
            RepairAccordianObjects.Add(new Syncfusion.EJ2.Navigations.AccordionAccordionItem
            {
                Header = "Repair 1",
                Content = "Microsoft ASP.NET is a set of technologies in the Microsoft .NET Framework for building Web applications and XML Web services. ASP.NET pages execute on the server and generate markup such as HTML, WML, or XML that is sent to a desktop or mobile browser. ASP.NET pages use a compiled,event-driven programming model that improves performance and enables the separation of application logic and user interface."
            }) ;
            RepairAccordianObjects.Add(new Syncfusion.EJ2.Navigations.AccordionAccordionItem
            {
                Header = "Repair 2",
                Content = "The Model-View-Controller (MVC) architectural pattern separates an application into three main components: the model, the view, and the controller. The ASP.NET MVC framework provides an alternative to the ASP.NET Web Forms pattern for creating Web applications. The ASP.NET MVC framework is a lightweight, highly testable presentation framework that (as with Web Forms-based applications) is integrated with existing ASP.NET features, such as master pages and membership-based authentication."
            });
            RepairAccordianObjects.Add(new Syncfusion.EJ2.Navigations.AccordionAccordionItem
            {
                Header = "Repair 3",
                Content = "Repair 3 Content"
            });
            RepairAccordianObjects.Add(new Syncfusion.EJ2.Navigations.AccordionAccordionItem
            {
                Header = "Repair 4",
                Content = "Repair 4 Content"
            });
            RepairAccordianObjects.Add(new Syncfusion.EJ2.Navigations.AccordionAccordionItem
            {
                Header = "Repair 5",
                Content = "Repair 5 Content"
            });

            AttendanceVisitCollection = new List<Syncfusion.EJ2.Navigations.AccordionAccordionItem>();
            AttendanceVisitCollection.Add(new Syncfusion.EJ2.Navigations.AccordionAccordionItem
            {
                Header = "Attendance 1",
                Content = "Attendance Detail 1"
            }) ;

            AttendanceVisitCollection.Add(new Syncfusion.EJ2.Navigations.AccordionAccordionItem
            {
                Header = "Attendance 2",
                Content = "Attendance Detail 2"
            });
            AttendanceVisitCollection.Add(new Syncfusion.EJ2.Navigations.AccordionAccordionItem
            {
                Header = "Attendance 3",
                Content = "Attendance Detail 3"
            });

        }
       
        public async Task LoadCustomerDashboardDataAsync()
        {
            if (owner != null)
            {
                List<Models.Units> units = new List<Models.Units>();
                units = await
                    Models.UnitMethods.GetCurrentOwnedUnits(owner.id, SelectedProperty.ID);
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
                                    text = unit.Name
                                });
                            }
                        }
                    }
                }
            }
        }
        
        public object anonObj { get; set; }
        public string Name { get; set; }
        public List<BudgetActualChartData> BudgetActualDataList { get; set; }
        public List<OwnedPropertyListViewObject> OwnedProperties { get; set; }
        public List<Syncfusion.EJ2.Navigations.AccordionAccordionItem> RepairAccordianObjects { get; set; }

        public List<Syncfusion.EJ2.Navigations.AccordionAccordionItem> AttendanceVisitCollection { get; set; }

        public double XAxisMaxValue { get; set; }
    }

    public class BudgetActualChartData
    {
        public string Name { get; set; }
        public double Amount { get; set; }

        public string  color { get; set; }
        
    }

    public class OwnedPropertyListViewObject
    {
        public OwnedPropertyListViewObject()
        {
            icon = "fa-solid fa-building-user";
        }
        public int id { get; set; }
        public string text { get; set; }

        public string icon { get; set; }
    }

  
}