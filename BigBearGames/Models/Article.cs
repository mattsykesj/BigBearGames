using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BigBearGames.Models
{
    public enum ArticleTypeEnum
    {
        News,
        Blog,
        Review
    }

    public class Article
    {
        public Article()
        {
            Comments = new List<Comment>();
        }

        [Key]
        public int Key { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string ImgPath { get; set; }
        public ArticleTypeEnum ArticleType { get; set; }
        public DateTime ArticleDateTime { get; set; }
        public string Description { get; set; }
        public int Likes { get; set; }

        public AppUser User { get; set; } 
        public ICollection<Comment> Comments {get; set;}
    }
}