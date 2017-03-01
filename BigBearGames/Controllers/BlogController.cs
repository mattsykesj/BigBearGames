using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BigBearGames.Models;
using BigBearGames.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace BigBearGames.Controllers
{
    [Authorize(Roles="Blogger")]
    public class BlogController : Controller
    {
        [HttpPost]
        public ActionResult Edit(Article article)
        {
            Article articleToUpdate;
            using (var context = new AppIdentityDbContext())
            {
                articleToUpdate = context.Articles.Single(x => x.Key == article.Key);
            }

            articleToUpdate.Body = article.Body;
            articleToUpdate.ArticleType = article.ArticleType;
            articleToUpdate.Description = article.Description;
            articleToUpdate.ImgPath = article.ImgPath;
            
            using(var updateContext = new AppIdentityDbContext())
            {
                updateContext.Entry(articleToUpdate).State = System.Data.Entity.EntityState.Modified;
                updateContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? Id)
        {
            Article article;
            using (var context = new AppIdentityDbContext())
            {
                article = context.Articles.Single(x => x.Key == Id);
            }

            return View(article);
        }

        public ActionResult Delete(int? Id)
        {
            if (Id != null)
            {
                using (var context = new AppIdentityDbContext())
                {
                    var article = context.Articles.Single(x => x.Key == Id);
                    context.Articles.Remove(article);
                    context.SaveChanges();
                }
            }
            else
            {
                return View("Error", new string[] { "Article not found" });
            }

            return RedirectToAction("Index");

        }


        public ActionResult Index()
        {
            var userName = HttpContext.User.Identity.Name;
            List<Article> userArticles;

            using(var context = new AppIdentityDbContext())
            {
                userArticles = context.Articles.Where(x => x.User.UserName == userName).ToList();
            }

            return View(userArticles);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Article article)
        {
            var user = UserManager.FindByName(HttpContext.User.Identity.Name);
            if (user != null && ModelState.IsValid)
            {
               article.ArticleDateTime = DateTime.Now;
               article.User = user;
               using (var context = HttpContext.GetOwinContext().Get<AppIdentityDbContext>())
                {
                    context.Articles.Add(article);
                    context.SaveChanges();
                }

                return RedirectToAction("Index", "Article");
            }

            return View("Error", new string[] { "Blog Creation Failed" });
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