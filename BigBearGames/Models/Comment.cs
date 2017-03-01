using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BigBearGames.Models
{
    public class Comment
    {
        [Key]
        public int Key { get; set; }
        public string Body { get; set; }
        public DateTime CommentDateTime { get; set; }

        public  Article Article { get; set; }
        public  AppUser User { get; set; }
    }
}