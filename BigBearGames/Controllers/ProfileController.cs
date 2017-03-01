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


namespace BigBearGames.Controllers
{
    public class ProfileController : Controller
    {
        [Authorize]
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
                    ProfileModel model = new ProfileModel { Name = userName, Email = user.Email, Comments = userComments, isBlogger = bloggerResult };
                    return View("UserProfile", model);
                }
                
            }
            else
                return View("Error", new string[] { "User Not Found, try logging in" });
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
    }
}