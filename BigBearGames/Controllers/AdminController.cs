using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using BigBearGames.Infrastructure;
using BigBearGames.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;


namespace BigBearGames.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        [HttpPost]
        public ActionResult Search(string searchString)
        {
            return View("Index", UserManager.Users.Where(x => x.UserName.Contains(searchString) || x.Email.Contains(searchString)));
        }

        
        public ActionResult OrderName(string order)
        {
            if (order == "name_asc")
            {
                ViewBag.Order = order;
                return View("Index", UserManager.Users.OrderBy(x => x.UserName));
            }
            else
            {
                ViewBag.Order = order;
                return View("Index", UserManager.Users.OrderByDescending(x => x.UserName));
            }
        }
        
        public ActionResult Index()
        {
            return View(UserManager.Users);
        }

        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { UserName = model.Name, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    IdentityResult roleResult = await UserManager.AddToRoleAsync(user.Id, "User");
                    if (result.Succeeded)
                    {
                        var email = new Emails();
                        await email.NewUserEmail(model);
                        if (HttpContext.User.IsInRole("Administrator"))
                            return RedirectToAction("Index");
                        else
                            return RedirectToAction("Login", "Account", new { @name = model.Name, @newUser = true });
                    }                   
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach(string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if(user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "User not found" });
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if(user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, string email, string password)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if(user != null)
            {
                user.Email = email;
                IdentityResult validEmail = await UserManager.UserValidator.ValidateAsync(user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }

                IdentityResult validPass = null;
                if(password != string.Empty)
                {
                    validPass = await UserManager.PasswordValidator.ValidateAsync(password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }

             if((validEmail.Succeeded && validPass == null) || 
                    (validEmail.Succeeded && password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = await UserManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }

                }
              
            }
            else
            {
                ModelState.AddModelError("", "User not found");
            }

            return View(user);
        }

    }
}