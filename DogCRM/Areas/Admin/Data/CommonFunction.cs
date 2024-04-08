using DogCRM.Areas.Admin.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;

namespace DogCRM.Areas.Admin.Data
{
    public class CommonFunction
    {

        public string GenrateRandomNo()
        {
            try
            {
                string _allowedChars = "123456789";
                Random randNum = new Random();
                char[] chars = new char[4];
                int allowedCharCount = _allowedChars.Length;
                for (int i = 0; i < 4; i++)
                {
                    chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
                }
                return new string(chars);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public bool SendTemplateMail(string Token, string Email)
        {

            string urlAddress = string.Format("{0}://{1}/EmailTemplate/EmailSend.html", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }
                string body = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                body = body.Replace("{{Token}}", Token);
                //send mail to agent using thread 
                //Thread SendAdmin = new Thread(() => commonService.SendMail("Admin Login Details", body, user.UserName, Convert.ToString(ConfigurationManager.AppSettings["AdminEmail"])));
                //SendAdmin.IsBackground = true;
                ////start thread to calling
                //SendAdmin.Start();
                MailMessage mail = new MailMessage();
                mail = new MailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["SmtpUser"]);
                mail.To.Add(Email);
                mail.Subject = "Forgot Password";
                mail.Body = body;
                mail.IsBodyHtml = true;
                NetworkCredential NetworkdCred = default(NetworkCredential);
                NetworkdCred = new NetworkCredential();
                NetworkdCred.UserName = ConfigurationManager.AppSettings["SmtpUser"];
                NetworkdCred.Password = ConfigurationManager.AppSettings["SmtpPass"];
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Host = ConfigurationManager.AppSettings["SmtpServer"];
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = NetworkdCred;
                SmtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
                SmtpServer.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpEnableSsl"]);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Timeout = 100000;
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                SmtpServer.Send(mail);
            }
            return true;
        }

        public UploadModel ImageUpload(UploadModel model)
        {
            string fileName = string.Empty;
            #region Upload for Images
            foreach (HttpPostedFileBase file in model.ImageList)
            {
                if (file != null)
                {
                    if (model.ImageList.Count() > 4)
                    {
                        model.imgMessage = "Please select only 4 files.";
                        model.imgValid = false;
                        return model;
                        //return "Please select only 4 files.";
                    }
                    var supportedImagesTypes = new[] { "jpg", "jpeg", "png", "gif" };
                    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                    var fileExtNew = System.IO.Path.GetExtension(file.FileName);
                    if (supportedImagesTypes.Contains(fileExt))
                    {
                        string objPath = "~/Areas/Admin/UploadedFiles/";
                        bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(objPath));

                        if (!exists)
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(objPath));

                        var InputFileName = System.IO.Path.GetFileName(file.FileName);
                        var name = Guid.NewGuid() + fileExtNew;

                        fileName = fileName + name + ',';

                        var ServerSavePath = Path.Combine(HttpContext.Current.Server.MapPath(objPath) + name);

                        file.SaveAs(ServerSavePath);
                        //return fileName;
                    }
                    else
                    {
                        model.imgMessage = "Please Select only images.";
                        model.imgValid = false;
                        return model;
                    }
                }
            }
            #endregion
            model.imgMessage = fileName;
            model.imgValid = true;
            return model;
        }

        public UploadModel VideoUpload(UploadModel model, string videoTitle, string videoDescription,string Tags)
        {
            string VideoName = string.Empty;
            string cancel = string.Empty;
            
            #region Upload for Videos
            foreach (HttpPostedFileBase file in model.VideoList)
            {
                if (file != null)
                {
                    if (model.VideoList.Count() > 4)
                    {
                        model.vdoMessage = "Please select only 4 files.";
                        model.vdoValid = false;
                        return model;
                    }
                    //var supportedVideoTypes = new[] { "avi", "mpg", "wav", "mid", "wmv", "asf", "mpeg", "mp4", "mkv" };
                    var supportedVideoTypes = new[] { "avi", "mpg", "wav", "mid", "wmv", "asf", "mpeg", "mp4", "mkv", "WEBM", "MP2", "MPE", "MPV", "OGG", "M4P", "M4V", "QT", "MOV", "FLV", "AVCHD" };
                    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                    var fileExtNew = System.IO.Path.GetExtension(file.FileName);
                    if (supportedVideoTypes.Contains(fileExt))
                    {
                        int FileSize = 10 * 1024 * 1024;
                        if (file.ContentLength < FileSize)
                        {
                            #region LocalMachine
                            //string objPath = "~/Areas/Admin/UploadedVideos/";
                            //bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(objPath));

                            //if (!exists)
                            //    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(objPath));

                            //var InputFileName = Path.GetFileName(file.FileName);
                            //var name = Guid.NewGuid() + fileExtNew;

                            //VideoName = VideoName + name + ',';

                            //var ServerSavePath = Path.Combine(HttpContext.Current.Server.MapPath(objPath) + name);

                            //file.SaveAs(ServerSavePath);
                            #endregion
                            var Tag = Tags.Remove(Tags.Length - 1);
                            //RunMethod(file, videoTitle, videoDescription).Wait();
                            Task<string> task = Task.Run(async () => await RunMethod(file, videoTitle, videoDescription,Tag));
                            task.Wait();
                            string invoiceIdAsync = task.Result;
                            if (task.Result == "false")
                            {
                                cancel += cancel + file.FileName + ',';
                            }
                            else
                            {
                                VideoName += VideoName + task.Result + ',';
                            }
                        }
                        else
                        {
                            model.vdoMessage = "Videos Maximum allowed file size is 10 MB.";
                            model.vdoValid = false;
                            return model;
                        }
                    }
                    else
                    {
                        //ModelState.AddModelError("", "Only .avi, .mpg, .wav, .mid, .wmv, .asf and .mpeg Video formats are allowed.");
                        model.vdoMessage = "Only Video formats are allowed.";
                        model.vdoValid = false;
                        return model;
                    }
                }
            }

            #endregion

            model.vdoMessage = VideoName;
            if (string.IsNullOrEmpty(model.vdoMessage))
            {
                model.vdoValid = false;
            }
            else
            {
                model.vdoValid = true;
            }
            //model.vdoValid = true;
            model.cancelMessage = cancel;
            return model;
        }

        public async Task<string> RunMethod(HttpPostedFileBase VideoUrl,string videoTitle,string videoDescription,string Tag)
        {
            try
            {
                UserCredential credential;
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = ConfigurationManager.AppSettings["ClientId"],
                    ClientSecret = ConfigurationManager.AppSettings["ClientSecret"]
                },
                new[] { YouTubeService.Scope.YoutubeUpload },
                        "user",
                        CancellationToken.None);

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name,
                });

                string[] Tags = Tag.Split(new string[] { "," },
                                  StringSplitOptions.None);
                var video = new Video();
                video.Snippet = new VideoSnippet();
                video.Snippet.ChannelId = ConfigurationManager.AppSettings["ChannelId"];
                video.Snippet.Title = videoTitle;
                video.Snippet.Description = videoDescription;
                //video.Snippet.Tags = new string[] { "screeb", "screen Reeco" };
                video.Snippet.Tags = Tags;
                video.Snippet.CategoryId = ConfigurationManager.AppSettings["CategoryId"];
                video.Status = new VideoStatus();
                video.Status.PrivacyStatus = ConfigurationManager.AppSettings["PrivacyStatus"];

                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", VideoUrl.InputStream, "video/*");

                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                var result = videosInsertRequest.Upload();

                if (result.Status == Google.Apis.Upload.UploadStatus.Completed)
                {
                    return videosInsertRequest.ResponseBody.Id;
                }
                else if (result.Status == Google.Apis.Upload.UploadStatus.Uploading)
                {
                    return "Uploading..";
                }
                else
                {
                    return "false";
                    //return result.Exception.ToString();
                }
            }
            catch (Exception Ex)
            {
                return Ex.Message;
            }
        }

        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            UploadInPlayListByvideoId(video.Id, video.Snippet.Title, video.Snippet.Description,video.Snippet.Tags).Wait();
            //ViewBag.Message = "successfully uploaded." + video.Id;
            //Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
        }

        private async Task UploadInPlayListByvideoId(string VideoId, string Title, string Description,IList<string> tags)
        {
            var TagDetails = "";
            if(tags.Count>0)
            {
                foreach(var tag in tags)
                {
                    TagDetails = TagDetails + tag + ',';
                }
            }
            UserCredential credential;
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = ConfigurationManager.AppSettings["ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["ClientSecret"]
            },
            new[] { YouTubeService.Scope.Youtube },
                    "user",
                    CancellationToken.None, new FileDataStore(this.GetType().ToString()));

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name,
            });

            // Add a video to the newly created playlist.
            var newPlaylistItem = new PlaylistItem();
            newPlaylistItem.Snippet = new PlaylistItemSnippet();
            newPlaylistItem.Snippet.PlaylistId = ConfigurationManager.AppSettings["PlaylistId"];
            newPlaylistItem.Snippet.ResourceId = new ResourceId();
            newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
            newPlaylistItem.Snippet.ResourceId.VideoId = VideoId;
            newPlaylistItem.Snippet.ResourceId.ChannelId = ConfigurationManager.AppSettings["ChannelId"];
            newPlaylistItem.Snippet.Title = Title;
            newPlaylistItem.Snippet.Description = Description;
            newPlaylistItem.Snippet.ETag = TagDetails;
            newPlaylistItem = youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").Execute();
        }


        public string VideoUpdateById(string VideoId, string Title, string Description, IList<string> tags)
        {

            Task task = Task.Run(async () => await UploadInPlayListByvideoId(VideoId, Title, Description,tags));
            task.Wait();
            return task.ToString();
        }
        private async Task GetYoutubeList()
        {
            try
            {
                UserCredential credential;
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = "265697987346-implsi2q00d091n1d4j402gdbd8hrjje.apps.googleusercontent.com",
                ClientSecret = "Zoj1ToJfx0_3rmBtIrIeCe7D"
            },
                new[] { YouTubeService.Scope.Youtube },
                "user",
                CancellationToken.None,
                        new FileDataStore(this.GetType().ToString()));

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = this.GetType().ToString()
                });

                // Create a new, private playlist in the authorized user's channel.
                //var newPlaylist = new Playlist();
                //newPlaylist.Snippet = new PlaylistSnippet();
                //newPlaylist.Snippet.Title = "Test Playlist";
                //newPlaylist.Snippet.Description = "A playlist created with the YouTube API v3";
                //newPlaylist.Status = new PlaylistStatus();
                //newPlaylist.Status.PrivacyStatus = "public";
                //newPlaylist = await youtubeService.Playlists.Insert(newPlaylist, "snippet,status").ExecuteAsync();

                // Add a video to the newly created playlist.
                var newPlaylistItem = new PlaylistItem();
                newPlaylistItem.Snippet = new PlaylistItemSnippet();
                newPlaylistItem.Snippet.PlaylistId = "UChuArgSqQSSLNgCPhX8oZdA";
                newPlaylistItem.Snippet.ResourceId = new ResourceId();
                newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
                newPlaylistItem.Snippet.ResourceId.VideoId = "fLrNwuO7nzY";
                newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();

                Console.WriteLine("Playlist item id {0} was added to playlist id {1}.", newPlaylistItem.Id, "UCBR8-60-B28hp2BmDPdntcQ");
            }
            catch (Exception Ex)
            {

            }
        }

        private async Task GetChannelList()
        {
            try
            {
                UserCredential credential;
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "265697987346-implsi2q00d091n1d4j402gdbd8hrjje.apps.googleusercontent.com",
                    ClientSecret = "Zoj1ToJfx0_3rmBtIrIeCe7D"
                },
                new[] { YouTubeService.Scope.YoutubeReadonly },
                        "user",
                        CancellationToken.None, new FileDataStore(this.GetType().ToString()));

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name,
                });

                var channelsListRequest = youtubeService.Channels.List("contentDetails");
                channelsListRequest.Mine = true;
                var channelsListResponse = channelsListRequest.Execute();

                foreach (var channel in channelsListResponse.Items)
                {
                    // From the API response, extract the playlist ID that identifies the list
                    // of videos uploaded to the authenticated user's channel.
                    var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;

                    Console.WriteLine("Videos in list {0}", uploadsListId);

                    var nextPageToken = "";
                    while (nextPageToken != null)
                    {
                        var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
                        playlistItemsListRequest.PlaylistId = uploadsListId;
                        playlistItemsListRequest.MaxResults = 50;
                        playlistItemsListRequest.PageToken = nextPageToken;

                        // Retrieve the list of videos uploaded to the authenticated user's channel.
                        var playlistItemsListResponse = playlistItemsListRequest.Execute();

                        foreach (var playlistItem in playlistItemsListResponse.Items)
                        {
                            // Print information about each video.
                            Console.WriteLine("{0} ({1})", playlistItem.Snippet.Title, playlistItem.Snippet.ResourceId.VideoId);
                        }

                        nextPageToken = playlistItemsListResponse.NextPageToken;
                    }
                }
            }
            catch (Exception Ex)
            {
            }
        }
    }
}