using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.DAL
{
    public static class IdentityDataInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<SecurityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByNameAsync(Constants.SuperUserName).Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.Id = Guid.NewGuid().ToString();
                user.UserName = Constants.SuperUserName;
                user.FirstName = "Super";
                user.LastName = "Admin";
                user.Email = "support@dcareug.com";
                user.EmailConfirmed = true;


                IdentityResult result = userManager.CreateAsync(user, "administrator").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, Constants.SuperRoleName).Wait();
                }
                else
                {
                    throw new Exception($"Default User Creation Error(s): {string.Join(",", result.Errors.Select(r => $"{r.Code}: {r.Description}"))}");
                }
            }
        }

        private static void SeedRoles(RoleManager<SecurityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("SmeUser").Result)
            {
                SecurityRole role = new SecurityRole("SmeUser");
                role.Id = Guid.NewGuid().ToString();
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;

                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Default Role for SMEs Creation Error(s): {string.Join(",", roleResult.Errors.Select(r => $"{r.Code}: {r.Description}"))}");
                }
            }

            if (!roleManager.RoleExistsAsync("UiaUser").Result)
            {
                SecurityRole role = new SecurityRole("UiaUser");
                role.Id = Guid.NewGuid().ToString();
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;

                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Default Role for USER_REGISTER Creation Error(s): {string.Join(",", roleResult.Errors.Select(r => $"{r.Code}: {r.Description}"))}");
                }
            }

            if (!roleManager.RoleExistsAsync(Constants.SuperRoleName).Result)
            {
                SecurityRole role = new SecurityRole(Constants.SuperRoleName);
                role.Id = Guid.NewGuid().ToString();
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;

                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Default Role Creation Error(s): {string.Join(",", roleResult.Errors.Select(r => $"{r.Code}: {r.Description}"))}");
                }
            }
        }

    }
}
