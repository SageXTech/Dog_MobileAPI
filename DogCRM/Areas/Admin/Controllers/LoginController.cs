using DogCRM.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DogsEntities;
using System.Web.Security;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using DogCRM.Areas.Admin.Data;

namespace DogCRM.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        DogCRMEntities dbContext = new DogCRMEntities();
        CommonFunction commonService = new CommonFunction();
        // GET: Admin/Login
        public ActionResult Login()
        {
            LoginModel model = new LoginModel();
            if (Request.Cookies["UserName"] != null || Request.Cookies["Password"] != null)
            {
                model.Email = Request.Cookies["UserName"].Value;
                model.Password = Request.Cookies["Password"].Value;
                model.RememberMe = true;
            }
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(LoginView model)
        {
            if(ModelState.IsValid)
            {
                var res = dbContext.Logins.Where(x => x.Email == model.Email && x.Password == model.Password).FirstOrDefault();
                if(res == null)
                {
                    ModelState.AddModelError("", "Opps, Username or Password invalid");
                    return View(model);
                }
                else
                {

                    if (model.RememberMe == true)
                    {
                        Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(30);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(30);
                    }
                    else
                    {
                        Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);

                    }
                    Response.Cookies["UserName"].Value = model.Email;
                    Response.Cookies["Password"].Value = model.Password;
                   
                    Session["UserId"] = res.Id;
                    Session["UserRole"] = res.Role;
                    if ((int)UserRole.SperAdmin == res.Role)
                    {
                        return RedirectToAction("ManageRole", "Admin");
                    }
                   else if ((int)UserRole.Admin == res.Role)
                    {
                        return RedirectToAction("ManageRole", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Dog");
                    }
                }
            }
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if(ModelState.IsValid)
            {
                var res = dbContext.Logins.Where(x=>x.Email==model.Email && x.Password == model.OldPassword).FirstOrDefault();
                if(res==null)
                {
                    ModelState.AddModelError("", "Record not found");
                }
                if(res != null)
                {
                    res.Email = model.Email;
                    res.Password = model.NewPassword;
                    dbContext.SaveChanges();

                    ModelState.Clear();
                    TempData["Success"] = "Password changed successfuly.";
                }
            }
            return View();
        }

        public ActionResult ForgotPasswordNew()
        {
            NewPassword model = new NewPassword();
            model.Email = Convert.ToString(TempData["confEmail"]);
            return View(model);
        }

        [HttpPost]
        public ActionResult ForgotPasswordNew(NewPassword model)
        {
            if (ModelState.IsValid)
            {
                    var res = dbContext.Logins.Where(x => x.Email == model.Email).FirstOrDefault();
                    if (res != null)
                    {
                        res.Password = model.Password;
                        dbContext.SaveChanges();

                        ModelState.Clear();
                        TempData["Success"] = "Password changed succesfuly.";
                    }
            }
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel objModel)
        {
            if (ModelState.IsValid)
            {
                var result = dbContext.Logins.Where(x => x.Email == objModel.Email).FirstOrDefault();
                if (result != null)
                {
                    try
                    {
                        var token = commonService.GenrateRandomNo();
                        commonService.SendTemplateMail(token, objModel.Email);

                        var resToken = dbContext.ForgotPasswords.Where(x => x.UserId == result.Id).FirstOrDefault();
                        if (resToken != null)
                        {
                            resToken.Token = token.ToString();
                            resToken.CreatedDate = DateTime.Now;
                            dbContext.SaveChanges();
                            TempData["Success"] = "Please check your email and get OTP";
                            TempData["TempEmail"] = result.Email;
                            TempData["UserId"] = result.Id;
                        }
                        else
                        {
                            ForgotPassword objForgot = new ForgotPassword();
                            objForgot.Token = Convert.ToString(token);
                            objForgot.UserId = result.Id;
                            objForgot.CreatedDate = DateTime.Now;
                            dbContext.ForgotPasswords.Add(objForgot);
                            dbContext.SaveChanges();
                            TempData["Success"] = "Please check your email and get OTP";
                            TempData["TempEmail"] = result.Email;
                            TempData["UserId"] = result.Id;
                        }

                        return RedirectToAction("TokenVerify", "Login");
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Login", "Login");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Not Found");
                    return View();
                }
            }
            return View();
        }

        public ActionResult TokenVerify()
        {
            TokenVerifyModel model = new TokenVerifyModel();
            model.Email = Convert.ToString(TempData["TempEmail"]);
            model.UserId = Convert.ToString(TempData["UserId"]);
            return View(model);
        }
        [HttpPost]
        public ActionResult TokenVerify(TokenVerifyModel model)
        {
            if(ModelState.IsValid)
            {
                var result = dbContext.ForgotPasswords.Where(x => x.Token == model.Token && x.UserId==Convert.ToInt32(model.UserId)).FirstOrDefault();
                if (result != null)
                {
                    var date = result.CreatedDate;
                    var curdate = DateTime.Now;
                    TimeSpan duration = curdate - Convert.ToDateTime(date);
                    if (duration.TotalMinutes <= 5)
                    {
                        TempData["confEmail"] = model.Email;
                        return RedirectToAction("ForgotPasswordNew", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Token is expired..please resend request");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "token are mis-match");
                    return View();
                }
            }
            return View();
        }
        public ActionResult LogOff()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Login");
        }
    }
}