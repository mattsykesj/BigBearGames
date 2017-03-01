using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using BigBearGames.Models;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using BigBearGames.Infrastructure;


namespace BigBearGames.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult LogOut()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");    
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string name, bool? newUser)
        {
            if (newUser == true)
                ViewBag.NewUser = newUser;
            if (name != String.Empty)
                ViewBag.Name = name;
            if (HttpContext.User.Identity.IsAuthenticated && !HttpContext.User.IsInRole("Administrator"))
                return View("Error", new string[] { "Access Denied" });
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await UserManager.FindAsync(details.Name, details.Password);

                if(user == null)
                {
                    ModelState.AddModelError("", "Invalid name or password");
                }
                else
                {
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
                    if (returnUrl == String.Empty || returnUrl == "/Account/Login")
                        returnUrl = "/Home/Index";
                    return Redirect(returnUrl);
                }

            }
            return View(details);
        }

        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private AppUserManager UserManager { get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            } }
    }
}