using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BigBearGames.Infrastructure;
using BigBearGames.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;


namespace BigBearGames.Controllers
{
    public class ArticleController : Controller
    {
        public int PageSize = 2;

        public ActionResult Index(int? articleType, int page = 1)
        {
            int TotalArticles;
            IEnumerable<Article> articles;
            string articleTitle;

            switch(articleType)
                {
                default:
                    articleTitle = "Articles";
                    break;

                case 0:
                    articleTitle = "News";
                    break;

                case 2:
                    articleTitle = "Reviews";
                    break;

                case 1:
                    articleTitle = "Blogs";
                    break;
            }

            if (articleType == null)
            {
                using (var context = new AppIdentityDbContext())
                {
                    TotalArticles = context.Articles.Count();

                    articles = context.Articles
                    .OrderByDescending(x => x.ArticleDateTime)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
                }
            }
            else
            {
                using (var context = new AppIdentityDbContext())
                {
                    TotalArticles = context.Articles.Where(x => x.ArticleType == (ArticleTypeEnum)articleType).Count();

                    articles = context.Articles
                        .Where(x => x.ArticleType == (ArticleTypeEnum)articleType)
                        .OrderByDescending(x => x.ArticleDateTime)
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();
                }
            }

            return View("Index", new ArticleIndex {
            Articles = articles, ArticleTitle = articleTitle,
            PagingInfo = new PagingInfo { ItemsPerPage = PageSize, CurrentPage = page, TotalItems = TotalArticles, ArticleType = articleType} });

        }

        public ActionResult News(int page = 1)
        {

            return RedirectToAction("Index", new { page = page, articleType = 0 });
        }

        public ActionResult Reviews(int page = 1)
        {
            return RedirectToAction("Index", new { page = page, articleType = 2 });
        }

        public ActionResult Blogs(int page = 1)
        {
            return RedirectToAction("Index", new { page = page, articleType = 1 });
        }

        public ActionResult ViewArticle(int? Id)
        {
            Article article;
            IEnumerable<Comment> comments;
            if (Id != null)
            {
                using (var context = new AppIdentityDbContext())
                {
                    article = context.Articles.Include(x => x.User).Single(x => x.Key == Id);
                    comments = context.Comments
                        .Include(x => x.User)
                        .Where(x => x.Article.Key == Id)
                        .OrderByDescending(x => x.CommentDateTime)
                        .ToList();
                }
            }
            else
            {
                return View("Error", new string[] { "Article Not Found" });
            }
                                                 
           return View(new ArticleView { Article = article, Comments = comments });   
        }

        [HttpPost]
        [Authorize]
        public ActionResult PostComment(int? Id,[Required] string commentBody)
        {
            Article article;
            AppUser user = UserManager.FindByName(HttpContext.User.Identity.Name); ;

            using (var context = HttpContext.GetOwinContext().Get<AppIdentityDbContext>())
            {
                article = context.Articles.Single(x => x.Key == Id);
                Comment comment = new Comment() { Body = commentBody, CommentDateTime = DateTime.Now, Article = article, User = user };
                context.Comments.Add(comment);
                context.SaveChanges();
            }
     
            return RedirectToAction("ViewArticle", new {id = Id });
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