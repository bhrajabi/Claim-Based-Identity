namespace IdentityClaim.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Security.Claims;

    internal sealed class Configuration : DbMigrationsConfiguration<IdentityClaim.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(IdentityClaim.Models.ApplicationDbContext context)
        {
            #region User

            if (context.Users.Count() == 0)
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                string[] roles = { "SupperAdmin", "Admin", "Manager", "User", "Editor" };
                foreach (var item in roles)
                {
                    var role = roleManager.FindByName(item);
                    if (role == null)
                    {
                        role = new IdentityRole(item);
                        roleManager.Create(role);
                    }
                }


                //AdminUser
                ApplicationUser newuser = new ApplicationUser
                {
                    EmailConfirmed = true,
                    UserName = "administrator@yourdomain.com",
                    Email = "administrator@yourdomain.com",
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    AccessFailedCount = 0,
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };
                userManager.Create(newuser, "P@ssw0rd");
                userManager.SetLockoutEnabled(newuser.Id, false);
                userManager.AddToRole(newuser.Id, "SupperAdmin");

                //Set Default Permissions For Admin User

                userManager.AddClaim(newuser.Id, new Claim("Permission", "Home.Index"));
                userManager.AddClaim(newuser.Id, new Claim("Permission", "Home.Permissions"));

                //Add Clients
                for (int i = 1; i <= 5; i++)
                {
                    ApplicationUser client = new ApplicationUser
                    {
                        EmailConfirmed = true,
                        UserName = "User" + i + "@yourdomain.com",
                        Email = "User" + i + "@yourdomain.com",
                        PhoneNumberConfirmed = true,
                        TwoFactorEnabled = false,
                        AccessFailedCount = 0,
                        SecurityStamp = Guid.NewGuid().ToString("D")
                    };
                    userManager.Create(client, "P@ssw0rd");
                    userManager.SetLockoutEnabled(client.Id, false);
                    userManager.AddToRole(client.Id, "User");

                    //Set Default Permissions For Clients
                    userManager.AddClaim(client.Id, new Claim("Permission", "Home.Index"));
                }
            }
            #endregion
        }
    }
}
