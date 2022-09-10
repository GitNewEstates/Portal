using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal_MVC.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using Twilio.AspNet.Common;
using Syncfusion.EJ2.SplitButtons;

namespace Portal_MVC.Controllers
{
    [Authorize(Roles = "Administrator,Manager,Property Manager")]
    public class VidexIntercomController : Controller
    {
        private string useridid { get; set; }
        private string email { get; set; }

        private void SetBaseData()
        {
            useridid = User.Identity.GetUserId();
            email = User.Identity.GetUserName();
            

        }
        // GET: VidexIntercom
        public async Task<System.Web.Mvc.ActionResult> Index()
        {
            SetBaseData();
            VidexIntercomListViewModel vm = new VidexIntercomListViewModel(ViewModelLevel.none);
            await vm.SetBaseDataAsync(useridid, email);
            await vm.LoadAsync();
            return View(vm);
        }

        // GET: VidexIntercom/Details/5
        public async Task<System.Web.Mvc.ActionResult> IntercomDetail(int id)
        {
            SetBaseData();
            VidexIntercomDetailViewModel vm = new VidexIntercomDetailViewModel(ViewModelLevel.none);
            await vm.SetBaseDataAsync(useridid, email);
            await vm.LoadAsync(id);
            return View(vm);
        }

        public async Task<ActionResult> AddNew()
        {
            SetBaseData();
            VidexAddNewIntercomViewModel vm = new VidexAddNewIntercomViewModel(ViewModelLevel.none);
            await vm.SetBaseDataAsync(useridid, email);
            await vm.LoadAsync();
            return View(vm);
        }

        public async Task<ActionResult> MemLocationDetail(int id)
        {
            SetBaseData();
            MemoryLocationDetailViewModel vm = new MemoryLocationDetailViewModel(ViewModelLevel.none);
            await vm.SetBaseDataAsync(useridid, email);
            await vm.LoadAsync(id);
            return View(vm);
        }

        public async Task <ContentResult> QueryMemLocation(int id)
        {

            if (id > 0)
            {
                //get the mem location
                VidexIntercomMemoryLocation MemoryLocation =
                    await VidexIntercomMemoryLocationMethods.GetVidexIntercomMemoryLocationAsync(id);

                string json = VidexIntercomMemoryLocationMethods.JSONSerialize(MemoryLocation);

                //send the query message
                await VidexIntercomMemoryLocationMethods.SendQueryMessage(MemoryLocation);



                //System.Threading.Thread.Sleep(3000000);
                return Content(await GetResponse(MemoryLocation, ResponseType.Query));
            }
            GenericJSONResponseObject response = new GenericJSONResponseObject();
            response.ResponseMessage = "Error";
            response.Stringify();
            return Content(response.JSON);
        }

        [HttpPost]
        public async Task<ContentResult> EditPrimaryNumber(VidexIntercomMemoryLocation memlocation)
        {
            SetBaseData();
            memlocation.UserID = useridid;
            memlocation.UserName = email;
            await VidexIntercomMemoryLocationMethods.SendUpdatePrimaryNumberMessage(memlocation);

            return Content(await GetResponse(memlocation, ResponseType.Update));
        }

        [HttpPost]
        public async Task<ContentResult> GetNextRecord(int id)
        {
            VidexIntercomMemoryLocation location = 
                await VidexIntercomMemoryLocationMethods.GetVidexIntercomMemoryLocationAsync(id);

            return Content(VidexIntercomMemoryLocationMethods.JSONSerialize(location));
        }

        public enum ResponseType
        {
            Query,
            Update
        }

        private async Task<string> GetResponse(VidexIntercomMemoryLocation MemoryLocation, ResponseType type)
        {


            GenericJSONResponseObject response = new GenericJSONResponseObject();
            //get the intercom object
            VidexIntercom intercom = await
                VidexIntercomMethods.GetIntercomAsync(MemoryLocation.IntercomID);
            response.intercom = intercom;

            bool PollFound = false;
            DateTime now = DateTime.Now;
            DateTime nowPlusOne = now.AddMinutes(1);
            System.Threading.Thread.Sleep(5000); //allow for time for response to come in
            while (!PollFound)
            {
                VidexMemoryLocationQuery query = new VidexMemoryLocationQuery();
                if (type == ResponseType.Query)
                {
                    query = await
                    VidexMemoryLocationQueryMethods.GetAsync(intercom.Number, VidexIntercomMemoryLocationMethods.SetMeMoryLoaction(MemoryLocation.LocationNumber));
                } else if (type == ResponseType.Update)
                {
                    query = await
                    VidexMemoryLocationQueryMethods.GetAsync(intercom.Number, MemoryLocation.LocationName);
                }

                if (query.APIError != null)
                {
                    if (query.APIError.HasError)
                    {
                        response.ResponseMessage = query.APIError.Message;
                        break;
                    }
                }
                if (query.id > 0)
                {
                    PollFound = true;
                    response.ResponseMessage = "Found";
                    response.MemoryLocation =
                        DeconstructBody(query.body, MemoryLocation, type);
                    //delete the VidexMemoryLocationQuery object
                    await VidexMemoryLocationQueryMethods.DeleteAsync(query.id);


                    break;
                }
                else if (DateTime.Now >= nowPlusOne)
                {
                    PollFound = true;
                    response.ResponseMessage = "Time Out";
                    break;
                }

            }

            response.Stringify();
            return response.JSON;
        }

        public VidexIntercomMemoryLocation DeconstructBody(string Body, VidexIntercomMemoryLocation memoryLocation, ResponseType type)
        {
            string[] array = Body.Split(' ');
            if (type == ResponseType.Query)
            {


                //set location
                string locationstr = RemoveReturns(array[1]);

                int location;
                int.TryParse(array[1], out location);
                memoryLocation.LocationNumber = location;
                memoryLocation.LocationName = RemoveReturns(array[3]);
                string code = RemoveReturns(array[5]);
                int icode;
                int.TryParse(code, out icode);
                memoryLocation.AccessCode = icode;
                bool PrimarySet = false;
                //find "TEL"
                for (int i = 0; i <= array.Length - 1; i++) 
                { 
                    string val = RemoveReturns(array[i]);

                    switch (val)
                    {
                        case "TEL":
                            if (!PrimarySet)
                            {
                                memoryLocation.PrimaryNumber = RemoveReturns(array[i + 1]);
                                PrimarySet = true;
                            }
                            
                            break;
                        case "1)":

                            memoryLocation.Divert1 = RemoveReturns(array[i + 1]);
                            break;
                        case "2)":

                            memoryLocation.Divert2 = RemoveReturns(array[i + 1]);
                            break;
                        case "3)":

                            memoryLocation.Divert3 = RemoveReturns(array[i + 1]);
                            break;
                    }

                    
                }


                string dtoStr = RemoveReturns(array[7]);
                if (dtoStr == "0")
                {
                    memoryLocation.DialToOpen = false;
                }
                else if (dtoStr == "1")
                {
                    memoryLocation.DialToOpen = true;
                }

               
            } else if(type == ResponseType.Update)
            {
                memoryLocation.LocationName = array[1];
            }

            return memoryLocation;
        }

        public string RemoveReturns(string val)
        {
            int i = val.IndexOf('\r');
            if(i >= 0)
            {
                val= val.Remove(i, 1);
            }

            int n = val.IndexOf("=");
            if (n >= 0)
            {
                val= val.Remove(n, 1);
            }
            int x = val.IndexOf("_");
            if (x >= 0)
            {
                val = val.Remove(x, 1);
            }

            return val;
        }

        // GET: VidexIntercom/Create
        public System.Web.Mvc.ActionResult Create()
        {
            return View();
        }

        // POST: VidexIntercom/Create
        [System.Web.Mvc.HttpPost]
        public async Task<ContentResult> Create(VidexIntercom intercom)
        {
            try
            {
                intercom.Number = RemoveReturns(intercom.Number);
                intercom =
                    await VidexIntercomMethods.InsertAsync(intercom);



            }
            catch
            {
                intercom.APIError = new APIError(ErrorType.APIDBInsertError);
                intercom.APIError.HasError = true;
                intercom.APIError.Message = "Error Inserting";

            }
            return Content(VidexIntercomMethods.JSONSerialize(intercom));
        }

        // GET: VidexIntercom/Edit/5
        public System.Web.Mvc.ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VidexIntercom/Edit/5
        [System.Web.Mvc.HttpPost]
        public System.Web.Mvc.ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: VidexIntercom/Delete/5
        public System.Web.Mvc.ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VidexIntercom/Delete/5
        [System.Web.Mvc.HttpPost]
        public System.Web.Mvc.ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }

    
    public class InboundSMSController : TwilioController
    {
        //this is the webhook for responses to queries to the intercom
        [HttpPost]
        public async Task Index(SmsRequest incomingMessage)
        {
            


            var messagingResponse = new MessagingResponse();
            try
            {
                //TextMessage message = new TextMessage(SMSSenders.General);
                //message.ReceipientNumber = "07493510769";
                //message.MessageBody = incomingMessage.Body;
                //await TextMessageMethods.SendMessage(message);


                VidexMemoryLocationQuery query = new VidexMemoryLocationQuery();
                query.ToNumber = incomingMessage.From;
                query.body = incomingMessage.Body;

                string[] array = query.body.Split(' ');
                //int memlocaton = -1;
                //int.TryParse(array[1], out memlocaton);
                query.MemoryLocation = array[1];

                query = await VidexMemoryLocationQueryMethods.InsertAsync(query);

                //TextMessage message1 = new TextMessage(SMSSenders.General);
                //message1.ReceipientNumber = "07493510769";
                //message1.MessageBody = query.id.ToString();
                //await TextMessageMethods.SendMessage(message1);

                messagingResponse.Message($"{query.MemoryLocation}");

                
            } catch (Exception ex)
            {

            }
            //return TwiML(messagingResponse);
        }
    }

    public class InboundUpdateResponseController : TwilioController
    {
        //this is the webhook for responses to update commands to the intercom
        [HttpPost]
        public async Task Index(SmsRequest incomingMessage)
        {
            var messagingResponse = new MessagingResponse();
            try
            {
               

                //APT NEM  TEL =07493510760  TEL (DIVERT 1) =  TEL (DIVERT 2) =  APT NEM  TEL (DIVERT 3) =  OK  VIDEX  GSM 

                VidexMemoryLocationQuery query = new VidexMemoryLocationQuery();
                query.ToNumber = incomingMessage.From;
                query.body = incomingMessage.Body;

                string[] array = query.body.Split(' ');
                //int memlocaton = -1;
                //int.TryParse(array[1], out memlocaton);
                query.MemoryLocation = array[1];

                query = await VidexMemoryLocationQueryMethods.InsertAsync(query);
                //TextMessage message = new TextMessage(SMSSenders.General);
                //message.ReceipientNumber = "+447493510769";
                //message.MessageBody = incomingMessage.From;
                //await TextMessageMethods.SendMessage(message);


                messagingResponse.Message($"{query.MemoryLocation}");
            }
            catch (Exception ex)
            {

            }
            //return TwiML(messagingResponse);
        }
    }



    public class SubmitModel
    {
        public string Name { get; set; }
        public string PrimaryKey { get; set; }
        public string Value { get; set; }
    }
    public class VidexIntercomListViewModel : ViewModelBase
    {
        public VidexIntercomListViewModel(ViewModelLevel level) : base(level)
        {
            IntercomList = new List<VidexIntercom>();
           
        }

        
        public List<VidexIntercom> IntercomList { get; set; }
        public async Task LoadAsync()
        {
            IntercomList = await VidexIntercomMethods.GetIntercomListAsync(SelectedEstateID);
        }
    }

    public class VidexIntercomDetailViewModel : ViewModelBase
    {
        public VidexIntercomDetailViewModel(ViewModelLevel level) : base(level)
        {
            MemLoctionList = new List<VidexIntercomMemoryLocation>();
            Intercom = new VidexIntercom();
            TabHeaders = new List<Syncfusion.EJ2.Navigations.TabTabItem>
            {
                new Syncfusion.EJ2.Navigations.TabTabItem
                {
                    Header = new Syncfusion.EJ2.Navigations.TabHeader
                        { Text = "Intercom Details"},
                    Content = "#intercom-details"
                },
                new Syncfusion.EJ2.Navigations.TabTabItem
                {
                    Header = new Syncfusion.EJ2.Navigations.TabHeader
                        { Text = "History"},
                    Content = "#intercom-history"
                },

            };
        }
        public List<Syncfusion.EJ2.Navigations.TabTabItem> TabHeaders { get; set; }
        public List<VidexIntercomMemoryLocation> MemLoctionList { get; set; }
        public VidexIntercom Intercom { get; set; }
        public async Task LoadAsync(int id)
        {
            Intercom = await VidexIntercomMethods.GetIntercomAsync(id);
            MemLoctionList = await VidexIntercomMemoryLocationMethods.GetVidexIntercomMemoryLocationsAsync(id);

        }
    }

    public class VidexAddNewIntercomViewModel : ViewModelBase
    {
        public VidexAddNewIntercomViewModel(ViewModelLevel level) : base(level)
        {
            NewIntercom = new VidexIntercom();
            Estates = new List<APIEstates>();
            NewIntercom.CreateMemoryLocations = true;
        }
        public async Task LoadAsync()
        {
            Estates = await APIEstateMethods.GetEstateListAsync();
            
        }
        public VidexIntercom NewIntercom { get; set; }


        public List<APIEstates> Estates { get; set; }
    }

    public class MemoryLocationDetailViewModel : ViewModelBase
    {
        public MemoryLocationDetailViewModel(ViewModelLevel Level) : base(Level)
        {
            MemoryLocation = new VidexIntercomMemoryLocation();
            DBMemoryLocation = new VidexIntercomMemoryLocation();
            Intercom = new VidexIntercom();
           
        }
        
        public async Task LoadAsync(int id)
        {
            MemoryLocation = new VidexIntercomMemoryLocation();
            
            NotesList = await APINotesMethods.GetNotesAsync("core.VidexIntercomNotes", id);
            Intercom = await VidexIntercomMethods.GetIntercomAsync(MemoryLocation.IntercomID);
            DBMemoryLocation =  await VidexIntercomMemoryLocationMethods.GetVidexIntercomMemoryLocationAsync(id);
            MemoryLocation.id = id;
            MemoryLocation.IntercomID = DBMemoryLocation.IntercomID;
        }
        public VidexIntercomMemoryLocation MemoryLocation { get; set; }

        public VidexIntercomMemoryLocation DBMemoryLocation { get; set; }
        public VidexIntercom Intercom { get; set; }

    }
}
