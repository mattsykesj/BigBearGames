using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using BigBearGames.Models;
using System.Text;


namespace BigBearGames.Infrastructure
{
    public static class ArticleHelpers
    {
        public static MvcHtmlString GetCommentCount(this HtmlHelper helper, int id)
        {
            List<Comment> commentCount;
            using (var context = new AppIdentityDbContext())
            {
                commentCount = context.Comments.Where(x => x.Article.Key == id).ToList();
               
            }
            
            return new MvcHtmlString(commentCount.Count().ToString());
        }

        public static HtmlString GetArticleBody(this HtmlHelper helper, string body)
        {           
            return new HtmlString(body);
        }

        public static MvcHtmlString PageLinks(this HtmlHelper helper, PagingInfo pageInfo, Func<int, string> pageUrl)
        {            
            StringBuilder result = new StringBuilder();

            for(int i =1; i <= pageInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
                if(i == pageInfo.CurrentPage)
                {
                    tag.AddCssClass("selected");
                    tag.AddCssClass("btn btn-primary");
                }
                tag.AddCssClass("btn btn-default");
                result.Append(tag.ToString());
            }

            return MvcHtmlString.Create(result.ToString());
        }
    }
}