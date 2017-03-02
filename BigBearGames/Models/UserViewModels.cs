using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using BigBearGames.Infrastructure;

namespace BigBearGames.Models
{
    public class CreateModel
    {
        [Required]
        [Display(Name = "User Name")]
        [RegularExpression("[A-Za-z][A-Za-z0-9._]{4,14}", ErrorMessage = "User Name Invalid")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Email Address Invalid")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Passwords must Match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class RoleEditModel
    {
        public AppRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
    }

    public class RoleModModel
    {
        [Required]
        public string RoleName { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToRemove { get; set; }
    }

    public class ProfileModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }

    public class ArticleIndex
    {
        public string ArticleTitle;
        public IEnumerable<Article> Articles { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string GetArticleString(ArticleTypeEnum articleEnum)
        {
            switch (articleEnum)
            {
                case ArticleTypeEnum.News:
                    return "label-info";
                case ArticleTypeEnum.Blog:
                    return "label-warning";
                case ArticleTypeEnum.Review:
                    return "label-danger";
                default: throw new ArgumentOutOfRangeException();

            }
        }
    }

    public class PagingInfo
    {
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int? ArticleType { get; set; }
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
            }
        }
    }

    public class ArticleView
    {
        public Article Article { get; set; }
        public IEnumerable<Comment> Comments { get; set; }

        public int GetCommentCount(IEnumerable<Comment> comments, int articleKey)
        {
            using (var context = new AppIdentityDbContext())
            {
                return context.Comments.Where(x => x.Article.Key == articleKey).Count();
            }
        }
    }


    public class ResetEmailModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Email Address Invalid")]
        public string newEmail { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Email Address Invalid")]
        [Compare("newEmail", ErrorMessage = "Emails must match")]
        public string confirmNewEmail { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("newPassword")]
        public string confirmPassword { get; set; }
    }

    
}