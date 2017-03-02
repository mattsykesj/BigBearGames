using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BigBearGames.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using BigBearGames.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity;


namespace BigBearGames.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        
        public async Task<ActionResult> Index()
        {
            string userName = HttpContext.User.Identity.Name;
            AppUser user = await UserManager.FindByNameAsync(userName);
            List<Comment> userComments;

            if (user != null)
            {
                using (var context = HttpContext.GetOwinContext().Get<AppIdentityDbContext>())
                {
                    userComments = context.Comments.Include(x => x.Article).Where(x => x.User.UserName == user.UserName).ToList();

                    var bloggerResult = await UserManager.IsInRoleAsync(user.Id, "Blogger");
                    ProfileModel model = new ProfileModel { Name = userName, Email = user.Email, Comments = userComments };
                    return View("UserProfile", model);
                }
                
            }
            else
                return View("Error", new string[] { "User Not Found, try logging in" });
        }

        [HttpGet]
        public ActionResult ResetEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetEmail(ResetEmailModel model)
        {
            
            AppUser userEmailCheck = UserManager.FindByEmail(model.newEmail); 
            if(userEmailCheck != null)
            {
                ModelState.AddModelError("", "User with that email is already registered");
                return View();
            }    
                
            var userId = HttpContext.User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                
                if (userId != null || userId != string.Empty)
                {
                    IdentityResult result = UserManager.SetEmail(userId, model.newEmail);
                    if (result.Succeeded)
                    {
                        return View("Success", new string[] { "Email reset" });
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
                else
                {
                   return View("Error", new string[] { "User Not Found" });
                }                                  
            }
          
         
            return View();
         
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel model)
        {
            AppUser user = await UserManager.FindByIdAsync(HttpContext.User.Identity.GetUserId());
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult validPass = await UserManager.PasswordValidator.ValidateAsync(model.newPassword);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.newPassword);
                        IdentityResult result = await UserManager.UpdateAsync(user);

                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                        else
                        {
                            return View("Success", new string[] { "Password reset" });
                        }
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }
            }
            else
            {
                return View("Error", new string[] { "User not found" });
            }

            return View();
        }

        [HttpPost]
        public ActionResult EditComment(Comment comment)
        {
            Comment commentToUpdate;
            using (var context = new AppIdentityDbContext())
            {
                commentToUpdate = context.Comments.Single(x => x.Key == comment.Key);
            }

            commentToUpdate.Body = comment.Body;

            using (var updateContext = new AppIdentityDbContext())
            {
                updateContext.Entry(commentToUpdate).State = System.Data.Entity.EntityState.Modified;
                updateContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditComment(int? Id)
        {
            Comment comment;
            if (Id != null)
            {
                using (var context = new AppIdentityDbContext())
                {
                     comment = context.Comments.Single(x => x.Key == Id);
                }
            }
            else
            {
                return View("Error", new string[] { "Comment not found" });
            }

            return View(comment);
        }

        public ActionResult DeleteComment(int? Id)
        {
            if(Id != null)
            {
                using (var context = new AppIdentityDbContext())
                {
                    var comment = context.Comments.Single(x => x.Key == Id);
                    context.Comments.Remove(comment);
                    context.SaveChanges();
                }
            }
            else
            {
                return View("Error", new string[] { "Comment not found" });
            }

            return RedirectToAction("Index");
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}