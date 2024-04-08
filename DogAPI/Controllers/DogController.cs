using DogAPI.Helper;
using DogAPI.Models;
using DogsEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace DogAPI.Controllers
{

    public class DogController : ApiController
    {
        DogCRMEntities dbContext = new DogCRMEntities();
        [Route("api/Dog/CreateDog")]
        [HttpPost]
        [UserAuthentication]
        public HttpResponseMessage CreateDog(DogModel model)
        {
            if (ModelState.IsValid)
            {
                Dog objModel = new Dog();
                objModel.Breed = model.Breed;
                objModel.Name = model.Name;
                objModel.Size = model.Size;
                objModel.Age = model.Age;
                objModel.Gender = Convert.ToInt32(model.Gender);
                objModel.ImageUrl = model.ImagesUrl[0];
                objModel.VideoUrl = model.VideosUrl[0];
                objModel.Description = model.Description;
                objModel.Characteristics = model.Characteristics;
                objModel.CreatedDate = DateTime.Now;
                //by default approve false
                dbContext.Dogs.Add(objModel);
                dbContext.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, objModel);
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("api/Dog/UpdateDog")]
        [HttpPost]
        [UserAuthentication]
        public HttpResponseMessage UpdateDog(DogModel model)
        {
            if (ModelState.IsValid)
            {
                var objModel = dbContext.Dogs.Where(x => x.Id == model.Id).FirstOrDefault();
                if (objModel != null)
                {
                    objModel.Breed = model.Breed;
                    objModel.Name = model.Name;
                    objModel.Size = model.Size;
                    objModel.Age = model.Age;
                    objModel.Gender = Convert.ToInt32(model.Gender);
                    objModel.ImageUrl = model.ImagesUrl[0];
                    objModel.VideoUrl = model.VideosUrl[0];
                    objModel.Description = model.Description;
                    objModel.Characteristics = model.Characteristics;
                    // if approve is false then false other wise true
                    dbContext.SaveChanges();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "1");
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User id not found");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("api/Dog/DogDetailsList")]
        [HttpGet]
        public HttpResponseMessage DogDetailsList()
        {
            try
            {
                var result = dbContext.Dogs.ToList();

                List<DogModel> DogList = new List<DogModel>();
                if (result != null)
                {
                    foreach (var res in result)
                    {
                        var gender = Enum.GetName(typeof(DogGender), res.Gender);
                        DogModel model = new DogModel();
                        model.Id = res.Id;
                        model.Breed = res.Breed;
                        model.Name = res.Name;
                        model.Age = res.Age;
                        model.Gender = gender;

                        List<string> objImg = new List<string>();
                        if (!string.IsNullOrEmpty(res.ImageUrl))
                        {
                            foreach (var resImg in res.ImageUrl.Split(','))
                            {
                                objImg.Add(resImg);
                            }
                        }
                        model.ImagesUrl = objImg;
                        List<string> objVideo = new List<string>();

                        if (!string.IsNullOrEmpty(res.VideoUrl))
                        {
                            foreach (var resVideo in res.VideoUrl.Split(','))
                            {
                                objVideo.Add(resVideo);
                            }
                        }
                        model.VideosUrl = objVideo;
                        model.Characteristics = res.Characteristics;
                        model.Description = res.Description;

                        DogList.Add(model);
                    }
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DogList);
                return response;
            }
            catch (Exception Ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message);
            }
        }
        [Route("api/Dog/GetDogById/{id}")]
        [HttpGet]

        public HttpResponseMessage GetDogById(int Id)
        {
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Id)))
                {
                    var result = dbContext.Dogs.Where(x => x.Id == Id).FirstOrDefault();
                    if (result != null)
                    {
                        var gender = Enum.GetName(typeof(DogGender), result.Gender);
                        DogModel model = new DogModel();
                        model.Id = result.Id;
                        model.Breed = result.Breed;
                        model.Name = result.Name;
                        model.Age = result.Age;
                        model.Gender = gender;
                        List<string> objImg = new List<string>();
                        foreach (var resImg in result.ImageUrl.Split(','))
                        {
                            objImg.Add(resImg);
                        }
                        model.ImagesUrl = objImg;

                        List<string> objVideo = new List<string>();
                        foreach (var resVideo in result.ImageUrl.Split(','))
                        {
                            objVideo.Add(resVideo);
                        }
                        model.VideosUrl = objVideo;
                        model.Characteristics = result.Characteristics;
                        model.Description = result.Description;

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, model);
                        return response;
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Data are not found in this Id.");
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Id not allow null or empty.");
                }
            }
            catch (Exception Ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message);
            }
        }

        [Route("api/Dog/DogDataListWithBreedFilter")]
        [HttpGet]
        public HttpResponseMessage DogDataListWithBreedFilter()
        {
            try
            {
                var result = dbContext.Dogs.ToList();
                //var result = dbContext.Dogs.ToList().Select(x => new DogFilter
                //{
                //    Id = x.Id,
                //    Breed = x.Breed,
                //    ImagesUrl = x.ImageUrl,
                //    VideosUrl = x.VideoUrl,
                //    Name = x.Name
                //});
                var filterResult = (from c in result
                                    orderby c.Id
                                    select c).GroupBy(g => g.Breed).Select(x => x.FirstOrDefault());

                List<DogFilter> DogList = new List<DogFilter>();
                if (filterResult != null)
                {
                    foreach (var res in filterResult)
                    {
                        DogFilter model = new DogFilter();
                        model.Id = res.Id;
                        model.Breed = res.Breed;
                        model.Name = res.Name;
                        model.ImagesUrl = string.IsNullOrEmpty(res.ImageUrl) ? res.ImageUrl : res.ImageUrl.Split(',')[0];
                        model.VideosUrl = string.IsNullOrEmpty(res.VideoUrl) ? res.VideoUrl : res.VideoUrl.Split(',')[0];

                        DogList.Add(model);
                    }
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DogList);
                    return response;
                }
                HttpResponseMessage resResult = Request.CreateResponse(HttpStatusCode.OK, DogList);
                return resResult;
            }
            catch (Exception Ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message);
            }
        }

        [Route("api/Dog/DogDataListWithoutBreedFilter")]
        [HttpGet]
        public HttpResponseMessage DogDataListWithoutBreedFilter()
        {
            try
            {
                var result = dbContext.Dogs.OrderByDescending(x=>x.Id).ToList();
                //var result = dbContext.Dogs.ToList().Select(x => new DogFilter
                //{
                //    Id = x.Id,
                //    Breed = x.Breed,
                //    ImagesUrl = x.ImageUrl.Split(',')[0],
                //    VideosUrl = x.VideoUrl.Split(',')[0],
                //    Name = x.Name
                //});
                List<DogFilter> DogList = new List<DogFilter>();
                if (result != null)
                {
                    foreach (var res in result)
                    {

                        DogFilter model = new DogFilter();
                        model.Id = res.Id;
                        model.Breed = res.Breed;
                        model.Name = res.Name;
                        model.ImagesUrl = string.IsNullOrEmpty(res.ImageUrl) ? res.ImageUrl : res.ImageUrl.Split(',')[0];
                        model.VideosUrl = string.IsNullOrEmpty(res.VideoUrl) ? res.VideoUrl : res.VideoUrl.Split(',')[0];
                        DogList.Add(model);
                    }
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DogList);
                    return response;
                }
                HttpResponseMessage resResult = Request.CreateResponse(HttpStatusCode.OK, DogList);
                return resResult;
            }
            catch (Exception Ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message);
            }
        }
        [Route("api/Dog/GetDataByBreed")]
        [HttpGet]
        public HttpResponseMessage GetDataByBreeds(string BreedName)
        {
            try
            {
                var result = dbContext.Dogs.Where(x => x.Breed == BreedName.Trim()).ToList();

                List<DogFilter> DogList = new List<DogFilter>();
                if (result != null)
                {
                    foreach (var res in result)
                    {
                        DogFilter model = new DogFilter();
                        model.Id = res.Id;
                        model.Breed = res.Breed;
                        model.Name = res.Name;
                        model.ImagesUrl = string.IsNullOrEmpty(res.ImageUrl) ? res.ImageUrl : res.ImageUrl.Split(',')[0];
                        model.VideosUrl = string.IsNullOrEmpty(res.VideoUrl) ? res.VideoUrl : res.VideoUrl.Split(',')[0];
                        DogList.Add(model);
                    }
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, DogList);
                    return response;
                }
                HttpResponseMessage resResult = Request.CreateResponse(HttpStatusCode.OK, DogList);
                return resResult;
            }
            catch (Exception Ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message);
            }
        }

        [Route("api/Dog/UpdateDogUserProfile")]
        [HttpPost]
        [UserAuthentication]
        public HttpResponseMessage UpdateDogUserProfile(DogProfile model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = dbContext.DogUserProfiles.Where(x => x.UserId == model.UserId).FirstOrDefault();
                    if (result != null)
                    {
                        result.UserId = model.UserId;
                        result.Name = model.Name;
                        result.Email = model.Email;
                        result.Bio = model.Bio;
                        result.Photo = string.IsNullOrEmpty(model.Photo) ? "Default.png" : model.Photo;
                        result.Gender = model.Gender;
                        dbContext.SaveChanges();
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "1");
                        return response;
                    }
                    else
                    {
                        if (model.UserId > 0)
                        {
                            DogUserProfile objModel = new DogUserProfile();
                            objModel.UserId = model.UserId;
                            objModel.Name = model.Name;
                            objModel.Email = model.Email;
                            objModel.Bio = model.Bio;
                            objModel.Photo = string.IsNullOrEmpty(model.Photo) ? "Default.png" : model.Photo;
                            objModel.Gender = model.Gender;
                            objModel.CreatedDate = DateTime.Now;
                            dbContext.DogUserProfiles.Add(objModel);
                            dbContext.SaveChanges();
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, objModel);
                            return response;
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User are not found.");
                        }
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            catch (Exception Ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Ex.Message);
            }
        }

        [Route("api/Dog/GetDogUserById/{id}")]
        [HttpGet]
        public HttpResponseMessage GetDogUserById(int id)
        {
            if (id > 0)
            {
                //var result = (from login in dbContext.Logins
                //              join Dog in dbContext.DogUserProfiles
                //              on login.Id equals Dog.UserId into DogF
                //              from dogD in DogF.DefaultIfEmpty()
                //              where login.Id == id
                //              select new DogProfile
                //              {
                //                  UserId = login.Id,
                //                  Name = login.Name,
                //                  Email = login.Email,
                //                  Bio = dogD.Bio,
                //                  Gender = dogD.Gender,
                //                  Photo = dogD.Photo,
                //              }).FirstOrDefault();

                var res = dbContext.usp_getDogUserById(id).Single();
                if (res != null)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Not found records");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Id must be grether then 0(zero).");
            }
        }

        [Route("api/Dog/AddDogImages")]
        [HttpPost]
        public async Task<string> AddDogImages()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/UserProfile/");
            var provider = new MyMultipartFormDataStreamProviderUserDogProfileImage(sPath);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);
                string rtData = string.Empty;
                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    //file.Headers.ContentDisposition.FileName = strNewFN;
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);
                    rtData = file.LocalFileName;
                }

                string[] imgName = Regex.Split(rtData, @"\\");
                return imgName[imgName.Length - 1].ToString();
            }
            catch (System.Exception e)
            {
                return "Error";
            }
        }

    }

    [UserAuthentication]
    public class MyMultipartFormDataStreamProviderUserDogProfileImage : MultipartFormDataStreamProvider
    {
        public MyMultipartFormDataStreamProviderUserDogProfileImage(string path)
            : base(path)
        { }
        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            // override the filename which is stored by the provider (by default is bodypart_x)
            string strNewFN = Guid.NewGuid().ToString() + ".jpeg";
            string originalFileName = strNewFN; // headers.ContentDisposition.FileName.Trim('\"');
            return originalFileName;
        }
    }
}
