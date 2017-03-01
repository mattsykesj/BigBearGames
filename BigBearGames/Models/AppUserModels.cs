using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BigBearGames.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Articles = new List<Article>();
            Comments = new List<Comment>();
        }


        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
    }
}