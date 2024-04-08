using DogAPI.Models;
using DogsEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.WebPages.Html;

namespace DogAPI.Controllers
{
    public class EventController : ApiController
    {
        DogCRMEntities dbContext = new DogCRMEntities();


        [Route("api/Event/GetEventList")]
        [HttpGet]
        public HttpResponseMessage GetEventList()
        {
            try
            {
                var ImageName = string.Empty;
                var result = dbContext.EventManagers.ToList();
                List<EventShortModel> EventList = new List<EventShortModel>();
                if (result != null)
                {
                    foreach (var res in result)
                    {
                        EventShortModel model = new EventShortModel();
                        var eventName = dbContext.EventCategories.Where(x => x.Id == res.EventType).FirstOrDefault().Name;

                        var dateStart = string.Format("{0:ddd dd MMM}", res.StartDateTime);
                        if (!string.IsNullOrEmpty(res.ImageUrl))
                        {
                            ImageName = res.ImageUrl.Split(',')[0];
                        }
                        else
                        {
                            ImageName = "";
                        }
                        model.Id = res.Id;
                        model.EventName = res.EventName;
                        model.EventType = eventName;
                        model.ImagesUrl = ImageName;
                        model.Address = res.Address;
                        model.EventPrice = res.EventPrice;
                        model.StartDateTime = dateStart;
                        EventList.Add(model);
                    }
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, EventList);
                return response;
            }
            catch (Exception Ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message);
            }
        }

        [Route("api/Event/GetEventById")]
        [HttpGet]
        public HttpResponseMessage GetEventById(int EventId, int UserId)
        {
            try
            {
                var result = dbContext.EventManagers.Where(x => x.Id == EventId).FirstOrDefault();

                EventModel Event = new EventModel();
                if (result != null)
                {
                    bool eventInterest = dbContext.UserEventInteresteds.Where(x => x.UserId == UserId && x.EventId == EventId).Any();
                    if (!eventInterest)
                    {
                        Event.EventInterested = false;
                    }
                    else
                    {
                        Event.EventInterested = true;
                    }
                    var eventCount = dbContext.UserEventInteresteds.Where(x => x.EventId == EventId).Count();
                    if (eventCount > 0)
                    {
                        Event.EventInterestedCount = eventCount;
                    }
                    else
                    {
                        Event.EventInterestedCount = 0;
                    }
                    var eventName = dbContext.EventCategories.Where(x => x.Id == result.EventType).FirstOrDefault().Name;
                    var dateStart = string.Format("{0:dd MMM yyyy hh:mm tt}", result.StartDateTime);
                    var dateEnd = string.Format("{0:dd MMM yyyy hh:mm tt}", result.EndDateTime);

                    if (!string.IsNullOrEmpty(result.Location))
                    {
                        Event.Location = result.Location.Split(',');
                    }
                    Event.Id = result.Id;
                    Event.EventName = result.EventName;
                    Event.Address = result.Address;
                    Event.Description = result.Description;
                    Event.EventType = eventName;
                    Event.EventPrice = result.EventPrice;
                    Event.StartDateTime = dateStart;
                    Event.EndDateTime = dateEnd;

                    List<string> objImg = new List<string>();
                    if (!string.IsNullOrEmpty(result.ImageUrl))
                    {
                        foreach (var resImg in result.ImageUrl.Split(','))
                        {
                            objImg.Add(resImg);
                        }
                    }
                    Event.ImagesUrl = objImg;
                    List<string> objVideo = new List<string>();

                    if (!string.IsNullOrEmpty(result.VideoUrl))
                    {
                        foreach (var resVideo in result.VideoUrl.Split(','))
                        {
                            objVideo.Add(resVideo);
                        }
                    }
                    Event.VideosUrl = objVideo;
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, Event);
                return response;
            }
            catch (Exception Ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message);
            }
        }

        [Route("api/Event/InsertEventInterestedUser")]
        [HttpPost]
        public HttpResponseMessage InsertEventInterestedUser(EventInterestedModel model)
        {
            if (ModelState.IsValid)
            {
                var UserEventInter = dbContext.UserEventInteresteds.Where(x => x.UserId == model.UserId && x.EventId == model.EventId).FirstOrDefault();

                if (UserEventInter != null)
                {
                    dbContext.UserEventInteresteds.Remove(UserEventInter);
                    dbContext.SaveChanges();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "0");
                    return response;
                }
                else
                {
                    UserEventInterested objModel = new UserEventInterested();
                    objModel.EventId = model.EventId;
                    objModel.UserId = model.UserId;
                    objModel.Interested = 1;
                    objModel.CreatedDate = DateTime.Now;
                    dbContext.UserEventInteresteds.Add(objModel);
                    dbContext.SaveChanges();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "1");
                    return response;
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("api/Event/GetEventCategoryList")]
        [HttpGet]
        public HttpResponseMessage GetEventCategoryList()
        {
            EventMultipleList modelList = new EventMultipleList();

            List<itemDetails> catItems = new List<itemDetails>();
            var EventCategory = dbContext.EventCategories.ToList();
            if (EventCategory != null)
            {
                for (int i = 0; i < EventCategory.Count; i++)
                {
                    catItems.Add(new itemDetails
                    {
                        Text = EventCategory[i].Name,
                        Value = EventCategory[i].Id,
                        Selected = false
                    });
                }
            }
            //modelList[0].categoryList=catItems;
            modelList.categoryList = catItems;

            List<itemDetails> priceItems = new List<itemDetails>();

            priceItems.Add(new itemDetails
            {
                Text = "0-500",
                Value = 1,
                Selected = false
            });
            priceItems.Add(new itemDetails
            {
                Text = "501-2000",
                Value = 1,
                Selected = false
            });
            priceItems.Add(new itemDetails
            {
                Text = "Above 2000",
                Value = 1,
                Selected = false
            });

            modelList.priceList = priceItems;

            List<itemDetails> dateItems = new List<itemDetails>();

            dateItems.Add(new itemDetails
            {
                Text = "Today",
                Value = 1,
                Selected = false
            });
            dateItems.Add(new itemDetails
            {
                Text = "Tomorrow",
                Value = 1,
                Selected = false
            });
            dateItems.Add(new itemDetails
            {
                Text = "Weekend",
                Value = 1,
                Selected = false
            });

            modelList.dateList = dateItems;

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, modelList);
            return response;
        }

        [Route("api/Event/EventListWithFilter")]
        [HttpPost]
        public HttpResponseMessage EventListWithFilter(EventFilter model)
        {
            try
            {
                List<EventManager> list = new List<EventManager>();

                if (model.EventType.Count > 0)
                {
                    foreach (var eType in model.EventType)
                    {
                        var firstresult = dbContext.EventManagers.Where(x => x.EventType == eType).FirstOrDefault();
                        if (firstresult != null)
                        {
                            list.Add(firstresult);
                        }
                    }
                }

                if (model.EventPrice.Count > 0)
                {
                    foreach (var eprice in model.EventPrice)
                    {
                        decimal Price;
                        if (eprice == 1)
                        {
                            Price = 500;
                            var firstresult = dbContext.EventManagers.Where(x => x.EventPrice <= Price).FirstOrDefault();
                            if (firstresult != null)
                            {
                                list.Add(firstresult);
                            }
                        }
                        if (eprice == 2)
                        {
                            Price = 2000;
                            var firstresult = dbContext.EventManagers.Where(x => x.EventPrice > 500 && x.EventPrice <= Price).FirstOrDefault();
                            if (firstresult != null)
                            {
                                list.Add(firstresult);
                            }
                        }
                        if (eprice == 3)
                        {
                            Price = 2000;
                            var firstresult = dbContext.EventManagers.Where(x => x.EventPrice > Price).FirstOrDefault();
                            if (firstresult != null)
                            {
                                list.Add(firstresult);
                            }
                        }
                    }
                }

                if (model.EventDate.Count > 0)
                {
                    foreach (var eDate in model.EventDate)
                    {
                        if (eDate == 1)
                        {
                            var ecDate = DateTime.Now;
                            var firstresult = dbContext.EventManagers.Where(x => x.EndDateTime == ecDate).FirstOrDefault();
                            if (firstresult != null)
                            {
                                list.Add(firstresult);
                            }
                        }
                        if (eDate == 2)
                        {
                            var today = DateTime.Now;
                            var ecDate = today.AddDays(1);
                            var firstresult = dbContext.EventManagers.Where(x => x.EndDateTime == ecDate).FirstOrDefault();
                            if (firstresult != null)
                            {
                                list.Add(firstresult);
                            }
                        }
                        if (eDate == 3)
                        {
                            DateTime date = DateTime.Now.AddDays(7);
                            while (date.DayOfWeek != DayOfWeek.Sunday)
                            {
                                date = date.AddDays(-1);
                            }
                            var firstresult = dbContext.EventManagers.Where(x => x.EndDateTime <= date).FirstOrDefault();
                            if (firstresult != null)
                            {
                                list.Add(firstresult);
                            }
                        }
                    }
                }

                var query = list.Distinct().ToList();

                var ImageName = string.Empty;
                List<EventShortModel> EventList = new List<EventShortModel>();
                if (query != null)
                {
                    foreach (var res in query)
                    {
                        EventShortModel modelL = new EventShortModel();
                        var eventName = dbContext.EventCategories.Where(x => x.Id == res.EventType).FirstOrDefault().Name;

                        var dateStart = string.Format("{0:ddd dd MMM}", res.StartDateTime);
                        if (!string.IsNullOrEmpty(res.ImageUrl))
                        {
                            ImageName = res.ImageUrl.Split(',')[0];
                        }
                        else
                        {
                            ImageName = "";
                        }
                        modelL.Id = res.Id;
                        modelL.EventName = res.EventName;
                        modelL.EventType = eventName;
                        modelL.ImagesUrl = ImageName;
                        modelL.Address = res.Address;
                        modelL.EventPrice = res.EventPrice;
                        modelL.StartDateTime = dateStart;
                        EventList.Add(modelL);
                    }
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, EventList);
                return response;
            }
            catch (Exception Ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message);
            }
        }
    }
}
