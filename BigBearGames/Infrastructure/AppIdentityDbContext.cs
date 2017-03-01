using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using BigBearGames.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace BigBearGames.Infrastructure
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext() : base("IdentityDb") { }

        static AppIdentityDbContext()
        {
            Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }

        public class IdentityDbInit : DropCreateDatabaseIfModelChanges<AppIdentityDbContext>
        {
            protected override void Seed(AppIdentityDbContext context)
            {
                PerformInitialSetup(context);
                base.Seed(context);
            }

            public void PerformInitialSetup(AppIdentityDbContext context)
            {
                AppUserManager UserManager = new AppUserManager(new UserStore<AppUser>(context));
                AppRoleManager RoleManager = new AppRoleManager(new RoleStore<AppRole>(context));

                string[] roleNames = { "Administrator", "Blogger", "User", };
                string userName = "Admin";
                string userEmail = "admin@example.com";
                string userPassword = "Secret1";

                foreach(string role in roleNames)
                {
                   if (!RoleManager.RoleExists(role))
                       RoleManager.Create(new AppRole(role));
                }

                AppUser user = UserManager.FindByName(userName);
                if(user == null)
                {
                    UserManager.Create(new AppUser { UserName = userName, Email = userEmail }, userPassword);
                    user = UserManager.FindByName(userName);
                }

                foreach (string role in roleNames)
                {
                    if (!UserManager.IsInRole(user.Id, role))
                    {
                        UserManager.AddToRole(user.Id, role);
                    }
                }

            }
        }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
    }
}