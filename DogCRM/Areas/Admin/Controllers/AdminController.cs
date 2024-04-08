using DogCRM.Areas.Admin.Data;
using DogCRM.Areas.Admin.Models;
using DogsEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DogCRM.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private DogCRMEntities dbContext;
        public AdminController()
        {
            dbContext = new DogCRMEntities();
        }
        public ActionResult ManageRole()
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult ManageRole(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                    Login objModel = new Login();
                    objModel.Email = model.Email;
                    objModel.Password = model.Password;
                    objModel.Role = model.Role;
                    dbContext.Logins.Add(objModel);
                    dbContext.SaveChanges();
                    ModelState.Clear();
                    TempData["Success"] = "Role created succesfuly.";
            }

            return View();
        }

        public ActionResult RoleList()
        {
            TempData["Success"] = TempData["SuccessMsg"];
            List<RoleView> modelList = new List<RoleView>();
            var res = dbContext.Logins.Where(x=>x.Role != 1).ToList();
            if (res != null)
            {
                for (int i = 0; i < res.Count(); i++)
                {
                    string role = string.Empty;
                    if(!string.IsNullOrEmpty(Convert.ToString(res[i].Role)))
                    {
                        role = Enum.GetName(typeof(UserRole), res[i].Role);
                    }

                    RoleView model = new RoleView();
                    model.Role = role;
                    model.Email = res[i].Email;
                    model.Id = res[i].Id;
                    modelList.Add(model);
                }
            }
            return PartialView("_ManageListPartial", modelList);
        }

        public ActionResult DeleteRoleById(int id)
        {
            var res = dbContext.Logins.Where(x => x.Id == id).FirstOrDefault();
            if (res != null)
            {
                dbContext.Logins.Remove(res);
                dbContext.SaveChanges();
                TempData["SuccessMsg"] = "Role deleted succesfuly.";
            }
            return RedirectToAction("RoleList", "Admin");
        }

        public ActionResult Edit(int id)
        {
            RoleView model = new RoleView();
            var res = dbContext.Logins.Where(x => x.Id == id).FirstOrDefault();
            if(res != null)
            {
                //string role = Enum.GetName(typeof(UserRole), res.Role);
                model.Id = res.Id;
                model.Role =Convert.ToString(res.Role);
                model.Email = res.Email;
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(RoleView model)
        {
            if (ModelState.IsValid)
            {
                //UserRole day = (int)Enum.GetNames(typeof(UserRole), model.Role);

                var res = dbContext.Logins.Where(x => x.Id == model.Id).FirstOrDefault();
                res.Id = model.Id;
                res.Role = Convert.ToInt32(model.Role);
                res.Email = model.Email;
                dbContext.SaveChanges();
                ModelState.Clear();
                TempData["SuccessMsg"] = "Role updated succesfuly.";
                return RedirectToAction("RoleList", "Admin");
            }
            return View();
        }
    }
}