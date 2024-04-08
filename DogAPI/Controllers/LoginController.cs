using DogAPI.Helper;
using DogAPI.Models;
using DogsEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web;

namespace DogAPI.Controllers
{
    public class LoginController : ApiController
    {
        DogCRMEntities dbContext = new DogCRMEntities();
        CommonFunction commonService = new CommonFunction();

        [Route("api/Login/Login")]
        [HttpPost]
        public HttpResponseMessage Login(LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var res = dbContext.Logins.Where(x => x.Password == model.Password && x.Email == model.Email || x.Name == model.Email).FirstOrDefault();

                    bool valEmail = dbContext.Logins.Where(x => x.Email == model.Email).Any();

                    if (res != null)
                    {
                        string resToken = Guid.NewGuid().ToString();
                        var auth = dbContext.UserTokens.Where(x => x.UserId == res.Id).FirstOrDefault();
                        if (auth != null)
                        {
                            auth.UniqueId = resToken;
                            auth.UserId = res.Id;
                            auth.CreatedDate = DateTime.Now;
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            UserToken userModel = new UserToken();
                            userModel.UniqueId = resToken;
                            userModel.UserId = res.Id;
                            userModel.CreatedDate = DateTime.Now;
                            dbContext.UserTokens.Add(userModel);
                            dbContext.SaveChanges();
                        }
                        AuthView objAuth = new AuthView();
                        objAuth.Token = resToken;
                        objAuth.Id = res.Id;
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, objAuth);
                        return response;
                    }
                    else
                    {
                        if (!valEmail)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Enter valid email.");
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Enter valid password.");
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

        [Route("api/Login/CreateUser")]
        [HttpPost]
        public HttpResponseMessage CreateUser(SignUp model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool valEmail = dbContext.Logins.Where(x => x.Email == model.Email).Any();
                    if (valEmail == true)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Email are already exists.");
                    }
                    else
                    {
                        Login objModel = new Login();
                        objModel.Name = model.Name;
                        objModel.Email = model.Email;
                        objModel.Password = model.Password;
                        objModel.Role = (int)DisplayRole.User;
                        objModel.CreatedDate = DateTime.Now;
                        dbContext.Logins.Add(objModel);
                        dbContext.SaveChanges();

                        DogUserProfile objProf = new DogUserProfile();
                        objProf.UserId = objModel.Id;
                        objProf.Name = model.Name;
                        objProf.Email = model.Email;
                        objProf.Photo = "Default.png";
                        objProf.CreatedDate= DateTime.Now;
                        dbContext.DogUserProfiles.Add(objProf);
                        dbContext.SaveChanges();
                            
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, objModel);
                        return response;
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

        #region Forgot Password API
        [Route("api/Login/ForgotPassword")]
        [HttpPost]
        public HttpResponseMessage ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var res = dbContext.Logins.Where(x => x.Email == model.Email).FirstOrDefault();
                if (res != null)
                {
                    try
                    {
                        // var token = Guid.NewGuid();
                        var token = commonService.GenrateRandomNo();
                        SendTemplateMail(token, model.Email);
                        //MailMessage mail = new MailMessage();
                        //mail = new MailMessage();
                        //mail.From = new MailAddress(ConfigurationManager.AppSettings["SmtpUser"]);
                        //mail.To.Add(model.Email);
                        //mail.Subject = "Forgot Password";
                        //mail.Body = "";
                        //mail.IsBodyHtml = true;
                        //NetworkCredential NetworkdCred = default(NetworkCredential);
                        //NetworkdCred = new NetworkCredential();
                        //NetworkdCred.UserName = ConfigurationManager.AppSettings["SmtpUser"];
                        //NetworkdCred.Password = ConfigurationManager.AppSettings["SmtpPass"];
                        //SmtpClient SmtpServer = new SmtpClient();
                        //SmtpServer.Host = ConfigurationManager.AppSettings["SmtpServer"];
                        //SmtpServer.UseDefaultCredentials = false;
                        //SmtpServer.Credentials = NetworkdCred;
                        //SmtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
                        //SmtpServer.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpEnableSsl"]);
                        //SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //SmtpServer.Timeout = 100000;
                        //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                        //SmtpServer.Send(mail);

                        ConfirmToken returnResult = new ConfirmToken();
                        var resToken = dbContext.ForgotPasswords.Where(x => x.UserId == res.Id).FirstOrDefault();
                        if (resToken != null)
                        {
                            resToken.Token = token.ToString();
                            resToken.CreatedDate = DateTime.Now;
                            dbContext.SaveChanges();
                            returnResult.OTP = token.ToString();
                            returnResult.Id = res.Id;
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, returnResult);
                            return response;
                        }
                        else
                        {
                            ForgotPassword objForgot = new ForgotPassword();
                            objForgot.Token = token.ToString();
                            objForgot.UserId = res.Id;
                            objForgot.CreatedDate = DateTime.Now;
                            dbContext.ForgotPasswords.Add(objForgot);
                            dbContext.SaveChanges();
                            returnResult.OTP = token.ToString();
                            returnResult.Id = res.Id;
                            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, returnResult);
                            return response;
                        }
                    }
                    catch (Exception Ex)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, Ex.Message);
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Enter valid email.");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("api/Login/ConfirmToken")]
        [HttpPost]
        public HttpResponseMessage ConfirmToken(ConfirmToken model)
        {
            if (ModelState.IsValid)
            {
                var result = dbContext.ForgotPasswords.Where(x => x.UserId == model.Id && x.Token == model.OTP).FirstOrDefault();
                if (result != null)
                {
                    var date = result.CreatedDate;
                    var curdate = DateTime.Now;
                    TimeSpan duration = curdate - Convert.ToDateTime(date);
                    if (duration.TotalMinutes <= 10)
                    {
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result.UserId);
                        return response;
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "OTP Expired.");
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Enter valid code.");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("api/Login/ForgotPasswordNewEntry")]
        [HttpPost]
        public HttpResponseMessage ForgotPasswordNewEntry(NewPassword model)
        {
            if (ModelState.IsValid)
            {
                var res = dbContext.Logins.Where(x => x.Id == model.Id).FirstOrDefault();
                if (res != null)
                {
                    res.Password = model.Password;
                    //res.CreatedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "Password updated succesfuly.");
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User id are not valid.");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        #endregion

        [Route("api/Login/ChangePassword")]
        [HttpPost]
        [UserAuthentication]
        public HttpResponseMessage ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var res = dbContext.Logins.Where(x => x.Id == model.Id && x.Password == model.OldPassword).FirstOrDefault();
                bool resId = dbContext.Logins.Where(x => x.Id == model.Id).Any();
                bool psMatch = false;
                if (model.OldPassword == model.NewPassword)
                {
                    psMatch = true;
                }
                if (psMatch == true)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "New password should not be the same.");
                }
                if (res != null)
                {
                    res.Password = model.NewPassword;
                    //res.CreatedDate = DateTime.Now;
                    dbContext.SaveChanges();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, res);
                    return response;
                }
                else
                {
                    if (!resId)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Incorrect old password.");
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Incorrect user Id.");
                    }
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Route("api/Login/GuestLogin")]
        [HttpPost]
        public HttpResponseMessage GuestLogin(GuestModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resExist = dbContext.GuestUsers.Where(x => x.DeviceId == model.DeviceId).FirstOrDefault();
                    if (resExist != null)
                    {
                        resExist.Name = model.Name;
                        resExist.Token = model.Token;
                        resExist.DeviceId = model.DeviceId;
                        //resExist.IsActive = model.IsActive;
                        resExist.CreatedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, resExist);
                        return response;
                    }
                    else
                    {
                        GuestUser userModel = new GuestUser();
                        userModel.Name = model.Name;
                        userModel.Token = model.Token;
                        userModel.DeviceId = model.DeviceId;
                        //userModel.IsActive = model.IsActive;
                        userModel.CreatedDate = DateTime.Now;
                        dbContext.GuestUsers.Add(userModel);
                        dbContext.SaveChanges();
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, userModel);
                        return response;
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

        [Route("api/Login/EventList")]
        [HttpPost]
        public HttpResponseMessage EventList()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "");
            return response;
        }

        public bool SendTemplateMail(string Token,string  Email)
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

    }
}
