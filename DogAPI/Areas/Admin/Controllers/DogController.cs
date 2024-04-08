using DogAPI.Areas.Admin.Data;
using DogAPI.Areas.Admin.Models;
using DogsEntities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DogAPI.Areas.Admin.Controllers
{
    public class DogController : Controller
    {
        private DogCRMEntities dbContext;
        CommonFunction commonFun = new CommonFunction();
        public DogController()
        {
            dbContext = new DogCRMEntities();
        }
        // GET: Admin/Dog
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(DogModel dog)
        {
            UploadModel objUpload = new UploadModel();
            objUpload.ImageList = dog.files;
            objUpload.VideoList = dog.Video;

            string fileName = string.Empty;
            string VideoName = string.Empty;
            string canclMsg = string.Empty;

            foreach (HttpPostedFileBase file in dog.files)
            {
                if (file != null)
                {
                    var resUpload = commonFun.ImageUpload(objUpload);
                    if (!resUpload.imgValid)
                    {
                        ModelState.AddModelError("", resUpload.imgMessage);
                        return View();
                    }
                    else
                    {
                        fileName = resUpload.imgMessage;
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            dog.ImagesUrl = fileName.Remove(fileName.Length - 1);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Please select image file");
                    return View();
                }
            }
            foreach (HttpPostedFileBase file in dog.Video)
            {
                if (file != null)
                {
                    if (!string.IsNullOrEmpty(dog.videoTitle) || !string.IsNullOrEmpty(dog.videoDescription))
                    {
                        var resVdoUpload = commonFun.VideoUpload(objUpload, dog.videoTitle, dog.videoDescription);
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
                                dog.VideosUrl = VideoName.Remove(VideoName.Length - 1);
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
                Dog model = new Dog();
                model.Breed = dog.Breed.Trim();
                model.Name = dog.Name.Trim();
                model.Age = dog.Age;
                model.Gender = Convert.ToInt32(dog.Gender);
                model.ImageUrl = dog.ImagesUrl;
                model.VideoUrl = string.IsNullOrEmpty(dog.VideosUrl) ? "": dog.VideosUrl;
                model.Characteristics = dog.Characteristics;
                model.Size = dog.Size;
                model.Description = dog.Description;
                model.CreatedDate = DateTime.Now;
                dbContext.Dogs.Add(model);

                dbContext.SaveChanges();

                ModelState.Clear();
               // TempData["Success"] = "Dog created successfuly.";
                TempData["SuccessMSg"] = "Event Updated successfuly.";
                return RedirectToAction("DogList", "Dog");
            }
            else
            {
                //revoke images
                revokeImages(dog.ImagesUrl);
                //revoke videos
                revokeVideos(dog.VideosUrl);
            }
            return View();
        }

        public ActionResult DogList()
        {
            TempData["Success"] = TempData["SuccessMSg"];
            List<DogModel> ModelList = new List<DogModel>();
            var res = dbContext.Dogs.OrderByDescending(x=>x.Id).ToList();
            if (res != null)
            {
                for (int i = 0; i < res.Count(); i++)
                {
                    string Gender = string.Empty;
                    if (!string.IsNullOrEmpty(Convert.ToString(res[i].Gender)))
                    {
                        Gender = Enum.GetName(typeof(DogGender), res[i].Gender);
                    }
                    DogModel objModel = new DogModel();
                    objModel.Id = res[i].Id;
                    objModel.Breed = res[i].Breed;
                    objModel.Name = res[i].Name;
                    objModel.Size = res[i].Size;
                    objModel.Age = res[i].Age;
                    objModel.Gender = Gender;
                    objModel.ImagesUrl = res[i].ImageUrl;
                    objModel.VideosUrl = res[i].VideoUrl;
                    objModel.Characteristics = res[i].Characteristics;
                    objModel.Description = res[i].Description;
                    ModelList.Add(objModel);
                }
            }
            return PartialView("_DogListPartial", ModelList);
        }

        public ActionResult DeleteDogById(int id)
        {
            var res = dbContext.Dogs.Where(x => x.Id == id).FirstOrDefault();
            if (res != null)
            {
                dbContext.Dogs.Remove(res);
                dbContext.SaveChanges();
                //revoke images
                revokeImages(res.ImageUrl);
                //revoke videos
                revokeVideos(res.VideoUrl);

                TempData["SuccessMSg"] = "Successfuly deleted.";
            }
            else
            {
                ModelState.AddModelError("", "Something wrong.");
            }

            return RedirectToAction("DogList", "Dog");
        }

        public ActionResult Edit(int id)
        {
            DogModel model = new DogModel();
            var res = dbContext.Dogs.Where(x => x.Id == id).FirstOrDefault();
            if (res != null)
            {
                model.Id = res.Id;
                model.Breed = res.Breed;
                model.Name = res.Name;
                model.Size = res.Size;
                model.Age = res.Age;
                model.Gender = Convert.ToString(res.Gender);
                model.ImagesUrl = res.ImageUrl;
                model.VideosUrl = res.VideoUrl;
                model.Characteristics = res.Characteristics;
                model.Description = res.Description;
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(DogModel dog)
        {

            UploadModel objUpload = new UploadModel();
            objUpload.ImageList = dog.files;
            objUpload.VideoList = dog.Video;

            string fileName = string.Empty;
            string VideoName = string.Empty;
            string[] Tags= { };
            if (!string.IsNullOrEmpty(dog.Tags))
            {
               Tags = dog.Tags.Split(new string[] { "," },
                                       StringSplitOptions.None);
            }

            foreach (HttpPostedFileBase file in dog.files)
            {
                if (file != null)
                {
                    var resUpload = commonFun.ImageUpload(objUpload);
                    if (!resUpload.imgValid)
                    {
                        ModelState.AddModelError("", resUpload.imgMessage);
                        return View();
                    }
                    else
                    {
                        fileName = resUpload.imgMessage;
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            dog.ImagesUrl = fileName.Remove(fileName.Length - 1);
                        }
                    }
                }
            }
            foreach (HttpPostedFileBase file in dog.Video)
            {
                if (file != null)
                {
                    if (!string.IsNullOrEmpty(dog.videoTitle) || !string.IsNullOrEmpty(dog.videoDescription))
                    {
                        var resVdoUpload = commonFun.VideoUpload(objUpload, dog.videoTitle, dog.videoDescription);
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
                                dog.VideosUrl = VideoName.Remove(VideoName.Length - 1);
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

                var res = dbContext.Dogs.Where(x => x.Id == dog.Id).FirstOrDefault();
                res.Id = dog.Id;
                res.Breed = dog.Breed.Trim();
                res.Name = dog.Name.Trim();
                res.Age = dog.Age;
                res.Gender = Convert.ToInt32(dog.Gender);
                res.ImageUrl = dog.ImagesUrl;
                res.VideoUrl = string.IsNullOrEmpty(dog.VideosUrl) ? "" : dog.VideosUrl; 
                res.Characteristics = dog.Characteristics;
                res.Description = dog.Description;
                res.Size = dog.Size;
                dbContext.SaveChanges();
                ModelState.Clear();
                TempData["SuccessMSg"] = "Dog updated successfuly.";
                return RedirectToAction("DogList", "Dog");
            }
            return View();
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

        //revoke videos
        public bool revokeVideos(string VideoUrl)
        {
            string objPath = "~/Areas/Admin/UploadedVideos/";
            if (!string.IsNullOrEmpty(VideoUrl))
            {
                foreach (string fName in VideoUrl.Split(','))
                {
                    string fullPath = Request.MapPath(objPath + fName);
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

        //public ActionResult GetVideo(YouTubeData objYouTubeData)
        //{
        //    try
        //    {
        //        var yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "Your API Key" });
        //        var channelsListRequest = yt.Channels.List("contentDetails");
        //        channelsListRequest.ForUsername = "kkrofficial";
        //        var channelsListResponse = channelsListRequest.Execute();
        //        foreach (var channel in channelsListResponse.Items)
        //        {
        //            // of videos uploaded to the authenticated user's channel.
        //            var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;
        //            var nextPageToken = "";
        //            while (nextPageToken != null)
        //            {
        //                var playlistItemsListRequest = yt.PlaylistItems.List("snippet");
        //                playlistItemsListRequest.PlaylistId = uploadsListId;
        //                playlistItemsListRequest.MaxResults = 50;
        //                playlistItemsListRequest.PageToken = nextPageToken;
        //                // Retrieve the list of videos uploaded to the authenticated user's channel.
        //                var playlistItemsListResponse = playlistItemsListRequest.Execute();
        //                foreach (var playlistItem in playlistItemsListResponse.Items)
        //                {
        //                    // Print information about each video.
        //                    //Console.WriteLine("Video Title= {0}, Video ID ={1}", playlistItem.Snippet.Title, playlistItem.Snippet.ResourceId.VideoId);
        //                    var qry = (from s in ObjEdbContext.ObjTubeDatas where s.Title == playlistItem.Snippet.Title select s).FirstOrDefault();
        //                    if (qry == null)
        //                    {
        //                        objYouTubeData.VideoId = "https://www.youtube.com/embed/" + playlistItem.Snippet.ResourceId.VideoId;
        //                        objYouTubeData.Title = playlistItem.Snippet.Title;
        //                        objYouTubeData.Descriptions = playlistItem.Snippet.Description;
        //                        objYouTubeData.ImageUrl = playlistItem.Snippet.Thumbnails.High.Url;
        //                        objYouTubeData.IsValid = true;
        //                        ObjEdbContext.ObjTubeDatas.Add(objYouTubeData);
        //                        ObjEdbContext.SaveChanges();
        //                        ModelState.Clear();

        //                    }
        //                }
        //                nextPageToken = playlistItemsListResponse.NextPageToken;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ViewBag.ErrorMessage = "Some exception occured" + e;
        //        return RedirectToAction("GetYouTube");
        //    }

        //    return RedirectToAction("GetYouTube");
        //}
    }
}