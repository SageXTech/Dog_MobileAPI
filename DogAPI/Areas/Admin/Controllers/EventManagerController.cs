using DogAPI.Areas.Admin.Data;
using DogAPI.Areas.Admin.Models;
using DogsEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DogAPI.Areas.Admin.Controllers
{
    public class EventManagerController : Controller
    {
        // GET: Admin/EventManager
        private DogCRMEntities dbContext;
        CommonFunction commonFun = new CommonFunction();
        public EventManagerController()
        {
            dbContext = new DogCRMEntities();
        }
        public ActionResult CreateEvent()
        {

            ViewBag.EventCategory = Eventcategory();

            return View();
        }
        [HttpPost]
        public ActionResult CreateEvent(EventModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.Location))
                {
                    var latLong = model.Location.Split(',');
                    var latTrim = latLong[0].Substring(7);
                    var longTrim = latLong[1].Remove(latLong[1].Length - 1);
                    model.lat = latTrim;
                    model.lng = longTrim;
                }
                UploadModel objUpload = new UploadModel();
                objUpload.ImageList = model.PostedImage;
                objUpload.VideoList = model.PostedVideo;

                string fileName = string.Empty;
                string VideoName = string.Empty;
                string canclMsg = string.Empty;
                //GetChannelList().Wait();
                //GetYoutubeList().Wait();
                //commonFun.Run(dog.Video,dog.videoTitle,dog.videoDescription).Wait();

                foreach (HttpPostedFileBase file in model.PostedImage)
                {
                    if (file != null)
                    {
                        var resUpload = commonFun.ImageUpload(objUpload);
                        if (!resUpload.imgValid)
                        {
                            ModelState.AddModelError("", resUpload.imgMessage);
                            //revoke images
                            revokeImages(resUpload.imgMessage);
                            return View();
                        }
                        else
                        {
                            fileName = resUpload.imgMessage;
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                model.ImagesUrl = fileName.Remove(fileName.Length - 1);
                            }
                        }

                    }
                    else
                    {
                        ViewBag.EventCategory = Eventcategory();
                        ModelState.AddModelError("", "Please select image file");
                        return View();
                    }
                }

                foreach (HttpPostedFileBase file in model.PostedVideo)
                {
                    if (file != null)
                    {
                        if (!string.IsNullOrEmpty(model.videoTitle) && !string.IsNullOrEmpty(model.videoDescription))
                        {
                            var resVdoUpload = commonFun.VideoUpload(objUpload, model.videoTitle, model.videoDescription);
                            if (!resVdoUpload.vdoValid)
                            {
                                ModelState.AddModelError("", resVdoUpload.vdoMessage);
                                return View();
                            }
                            else
                            {
                                VideoName = resVdoUpload.vdoMessage;
                                if (!string.IsNullOrEmpty(VideoName))
                                {
                                    model.VideosUrl = VideoName.Remove(VideoName.Length - 1);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Not allow null video title or video description");
                            revokeImages(fileName);
                            return View();
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    EventManager objEvent = new EventManager();
                    objEvent.EventName = model.EventName;
                    //objEvent.Location = model.Location;
                    objEvent.Location = model.lat + ',' + model.lng;
                    objEvent.Address = model.Address;
                    objEvent.Description = model.Description;
                    objEvent.ImageUrl = model.ImagesUrl;
                    objEvent.VideoUrl = string.IsNullOrEmpty(model.VideosUrl) ? "" : model.VideosUrl; 
                    objEvent.EventType = model.EventType;

                    objEvent.EventPrice = Convert.ToDecimal(model.EventPrice);
                    DateTime StartDate = DateTime.ParseExact(model.StartDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objEvent.StartDateTime = StartDate;
                    DateTime EndDate = DateTime.ParseExact(model.EndDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objEvent.EndDateTime = EndDate;
                    objEvent.CreatedDate = DateTime.Now;
                    dbContext.EventManagers.Add(objEvent);
                    dbContext.SaveChanges();
                    TempData["SuccessMSg"] = "Event Created successfuly.";
                    //SendNotification(model.lat, model.lng, model.EventName, model.Address, model.EventPrice);
                    return RedirectToAction("EventList", "EventManager");
                }
                else
                {
                    ModelState.AddModelError("",ModelState.ToString());
                }
                ViewBag.EventCategory = Eventcategory();
                return View();
            }
            catch(Exception Ex)
            {
                ModelState.AddModelError("", Ex.ToString());
                return View();
            }
        }

        public ActionResult EditEvent(int id)
        {
            EventModel model = new EventModel();
            var res = dbContext.EventManagers.Where(x => x.Id == id).FirstOrDefault();
            if (res != null)
            {
                var lat = string.Empty;
                var lng = string.Empty;
                if (!string.IsNullOrEmpty(res.Location))
                {
                    var location = res.Location.Split(',');
                    lat = location[0];
                    lng = location[1];
                }
                model.Id = res.Id;
                model.EventName = res.EventName;
                //objEvent.Location = model.Location;
                model.Location = "LngLat(" + lat + ',' + lng + ')';
                model.lat = lat;
                model.lng = lng;
                model.Address = res.Address;
                model.Description = res.Description;
                model.ImagesUrl = res.ImageUrl;
                model.VideosUrl = res.VideoUrl;
                model.EventType = res.EventType;
                model.EventPrice = res.EventPrice;
                var srtDate = Convert.ToDateTime(res.StartDateTime);
                var day=string.Empty;
                if (srtDate.Day <=9)
                {
                    day = 0+srtDate.Day.ToString();
                }
                else
                {
                    day = srtDate.Day.ToString();
                }
                var month=string.Empty;
                if (srtDate.Month <=9)
                {
                    month = 0+srtDate.Month.ToString();
                }
                else
                {
                    month= srtDate.Month.ToString();
                }
                var year = srtDate.Year;
                var startDate = day + "/" + month + "/" + year;

                model.StartDateTime = startDate;

                var endDate = Convert.ToDateTime(res.EndDateTime);
                var eDay = string.Empty;
                if (endDate.Day<=9)
                {
                    eDay = 0+endDate.Day.ToString();
                }
                else
                {
                    eDay= endDate.Day.ToString();
                }
                var eMonth = string.Empty;
                if (endDate.Month <=9)
                {
                    eMonth = 0+endDate.Month.ToString();
                }
                else
                {
                    eMonth= endDate.Month.ToString();
                }
                var eYear = endDate.Year;
                var EndDatetime = eDay + "/" + eMonth + "/" + eYear;
                model.EndDateTime = EndDatetime;

                //model.StartDateTime = Convert.ToString(res.StartDateTime);
                //model.EndDateTime = Convert.ToString(res.EndDateTime);
            }
            ViewBag.EventCategory = Eventcategory();

            return View(model);
        }
        [HttpPost]
        public ActionResult EditEvent(EventModel model)
        {
            string[] Tags = { };
            if (!string.IsNullOrEmpty(model.Tags))
            {
                Tags = model.Tags.Split(new string[] { "," },
                                   StringSplitOptions.None);
            }

            //var resUpload = commonFun.VideoUpdateById(model.VideosUrl, model.videoTitle, model.videoDescription, Tags);
            UploadModel objUpload = new UploadModel();
            objUpload.ImageList = model.PostedImage;
            objUpload.VideoList = model.PostedVideo;

            string fileName = string.Empty;
            string VideoName = string.Empty;
            string canclMsg = string.Empty;
            //GetChannelList().Wait();
            //GetYoutubeList().Wait();
            //commonFun.Run(dog.Video,dog.videoTitle,dog.videoDescription).Wait();

            foreach (HttpPostedFileBase file in model.PostedImage)
            {
                if (file != null)
                {
                    var resUpload = commonFun.ImageUpload(objUpload);
                    if (!resUpload.imgValid)
                    {
                        ModelState.AddModelError("", resUpload.imgMessage);
                        //revoke images
                        revokeImages(resUpload.imgMessage);
                        return View();
                    }
                    else
                    {
                        fileName = resUpload.imgMessage;
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            model.ImagesUrl = fileName.Remove(fileName.Length - 1);
                        }
                    }

                }
            }

            foreach (HttpPostedFileBase file in model.PostedVideo)
            {
                if (file != null)
                {
                    if (!string.IsNullOrEmpty(model.videoTitle) && !string.IsNullOrEmpty(model.videoDescription))
                    {
                        var resVdoUpload = commonFun.VideoUpload(objUpload, model.videoTitle, model.videoDescription);
                        if (!resVdoUpload.vdoValid)
                        {
                            ModelState.AddModelError("", resVdoUpload.vdoMessage);
                            return View();
                        }
                        else
                        {
                            VideoName = resVdoUpload.vdoMessage;
                            if (!string.IsNullOrEmpty(VideoName))
                            {
                                model.VideosUrl = VideoName.Remove(VideoName.Length - 1);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Not allow null video title or video description");
                        revokeImages(fileName);
                        return View();
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var result = dbContext.EventManagers.Where(x => x.Id == model.Id).FirstOrDefault();
                if (result != null)
                {
                    result.EventName = model.EventName;
                    result.Location = model.lat + ',' + model.lng;
                    result.Address = model.Address;
                    result.Description = model.Description;
                    result.ImageUrl = model.ImagesUrl;
                    result.VideoUrl = string.IsNullOrEmpty(model.VideosUrl) ? "" : model.VideosUrl;
                    result.EventType = model.EventType;
                    result.EventPrice = model.EventPrice;

                    DateTime StartDate = DateTime.ParseExact(model.StartDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    result.StartDateTime = StartDate;

                    DateTime EndDate = DateTime.ParseExact(model.EndDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    result.EndDateTime = EndDate;
                    //result.CreatedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    TempData["SuccessMSg"] = "Event Updated successfuly.";
                    return RedirectToAction("EventList", "EventManager");
                }
                else
                {
                    ModelState.AddModelError("", "records are not found");
                    return View();
                }
            }
            ViewBag.EventCategory = Eventcategory();
            return View();
        }

        public ActionResult EventList()
        {
            TempData["Success"] = TempData["SuccessMSg"];
            List<EventModel> ModelList = new List<EventModel>();
            var res = dbContext.EventManagers.OrderByDescending(x => x.Id).ToList();
            if (res != null)
            {
                for (int i = 0; i < res.Count(); i++)
                {
                    var eId = Convert.ToInt32(res[i].EventType);
                    var eventType = dbContext.EventCategories.Where(x => x.Id == eId).FirstOrDefault();
                    EventModel objModel = new EventModel();
                    objModel.Id = res[i].Id;
                    objModel.EventName = res[i].EventName;
                    objModel.Address = res[i].Address;
                    objModel.EventType = Convert.ToInt32(res[i].EventType);
                    objModel.EventTypeName = eventType.Name;
                    objModel.EventPrice = res[i].EventPrice;
                    objModel.ImagesUrl = res[i].ImageUrl;
                    objModel.VideosUrl = res[i].VideoUrl;
                    objModel.StartDateTime = Convert.ToString(res[i].StartDateTime);
                    objModel.EndDateTime =Convert.ToString(res[i].EndDateTime);
                    ModelList.Add(objModel);
                }
            }
            return PartialView("_EventListPartial", ModelList);
        }

        public ActionResult DeleteEventById(int id)
        {
            var res = dbContext.EventManagers.Where(x => x.Id == id).FirstOrDefault();
            if (res != null)
            {
                dbContext.EventManagers.Remove(res);
                dbContext.SaveChanges();
                //revoke images
                revokeImages(res.ImageUrl);
                //revoke videos
                // revokeVideos(res.VideoUrl);

                TempData["SuccessMSg"] = "Successfuly deleted.";
            }
            else
            {
                ModelState.AddModelError("", "Something wrong.");
            }

            return RedirectToAction("EventList", "EventManager");
        }

        public List<SelectListItem> Eventcategory()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            var EventCategory = dbContext.EventCategories.ToList();
            if (EventCategory != null)
            {
                for (int i = 0; i < EventCategory.Count; i++)
                {
                    listItems.Add(new SelectListItem
                    {
                        Text = EventCategory[i].Name,
                        Value = Convert.ToString(EventCategory[i].Id)
                    });
                }
            }
            return listItems;
        }


        public bool revokeImages(string ImageUrl)
        {
            string objImgPath = "~/Areas/Admin/UploadedFiles/";
            if (!string.IsNullOrEmpty(ImageUrl))
            {
                foreach (string fName in ImageUrl.Split(','))
                {
                    string fullPath = Request.MapPath(objImgPath + fName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }


        //public void SendNotification(string Latitude, string Longitude, string EventName, string Address, decimal? EventPrice)
        //{
        //    string Title = "Event date Declared";
        //    EventName = "doggy";
        //    try
        //    {
        //        int UserDeviceCount = dbContext.GuestUsers.Count();

        //        for (int i = 0; i <= UserDeviceCount; i = i + 500)
        //        {
        //            var UserDevice = dbContext.GuestUsers.OrderByDescending(x => x.Id).Select(x => x.DeviceId).Skip(i).Take(1000).ToList();
        //            Thread SendNotify = new Thread(() => SendNotification(UserDevice.ToArray(), Title, EventName));
        //            SendNotify.IsBackground = true;
        //            SendNotify.Start();
        //            //var UserDevice = dataContent.tbl_Coords.Where(x => !string.IsNullOrEmpty(x.NotificationToken) && x.City == "Gujarat").OrderByDescending(x => x.Id).Select(x => x.NotificationToken).Skip(i).Take(1000).ToList();
        //            //Thread SendNotify = new Thread(() => SendNotification(UserDevice.ToArray(), objNotification.Title, objNotification.Message));
        //            //SendNotify.IsBackground = true;
        //            //SendNotify.Start();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //return Json(-1);
        //    }

        //}

        //public void SendNotification(string[] deviceId, string title, string message)
        //{
        //    try
        //    {
        //        string resultOfNotification;
        //        WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        //        tRequest.Method = "post";
        //        tRequest.ContentType = "application/json";
        //        var data = new object();
        //        data = new
        //        {
        //            registration_ids = deviceId,
        //            notification = new
        //            {
        //                body = message,
        //                title = title,
        //                icon = "myicon",
        //                sound = "default",
        //            }
        //        };
        //        var serializer = new JavaScriptSerializer();
        //        var json = serializer.Serialize(data);
        //        Byte[] byteArray = Encoding.UTF8.GetBytes(json);
        //        tRequest.Headers.Add(string.Format("Authorization: key={0}", Convert.ToString(ConfigurationManager.AppSettings["applicationID"])));
        //        tRequest.Headers.Add(string.Format("Sender: id={0}", Convert.ToString(ConfigurationManager.AppSettings["senderId"])));
        //        tRequest.ContentLength = byteArray.Length;

        //        using (Stream dataStream = tRequest.GetRequestStream())
        //        {
        //            dataStream.Write(byteArray, 0, byteArray.Length);
        //            using (WebResponse tResponse = tRequest.GetResponse())
        //            {
        //                using (Stream dataStreamResponse = tResponse.GetResponseStream())
        //                {
        //                    using (StreamReader tReader = new StreamReader(dataStreamResponse))
        //                    {
        //                        String sResponseFromServer = tReader.ReadToEnd();
        //                        resultOfNotification = sResponseFromServer;
        //                    }
        //                }
        //            }
        //        }

        //        if (deviceId != null)
        //        {
        //            for (int i = 0; i < deviceId.Count(); i++)
        //            {
        //                Notification objNot = new Notification();
        //                objNot.DeviceId = deviceId[i];
        //                objNot.Title = title;
        //                objNot.Message = message;
        //                objNot.NotificationDescription = resultOfNotification;
        //                objNot.CreatedDate = DateTime.Now;
        //                dbContext.Notifications.Add(objNot);
        //                dbContext.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //return Content(HttpStatusCode.NotImplemented, ex.ToString());
        //    }
        //}
    }
}