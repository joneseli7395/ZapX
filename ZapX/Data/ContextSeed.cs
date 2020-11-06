using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Models;

namespace ZapX.Data
{
    public enum Roles
    {
        Admin,
        ProjectManager,
        Developer,
        Submitter,
        NewUser
    }


    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Submitter.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.NewUser.ToString()));
        }

        public static async Task SeedDefaultUsersAsync(UserManager<BTUser> userManager)
        {
            #region SeedAdmin
            //Seed Default Admin User
            var defaultAdmin = new BTUser
            {
                UserName = "joneseli7395@gmail.com",
                Email = "joneseli7395@gmail.com",
                FirstName = "Eli",
                LastName = "Jones",
                Role = "Admin",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultAdmin.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultAdmin, "Password123!");
                    await userManager.AddToRoleAsync(defaultAdmin, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR ********");
                Debug.WriteLine("Error Seeding Default Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************");
                throw;
            }
            #endregion

            #region SeedProjectManager
            //Seed Default ProjectManager User
            var defaultPM = new BTUser
            {
                UserName = "araynor@coderfoundry.com",
                Email = "araynor@coderfoundry.com",
                FirstName = "Antonio",
                LastName = "Raynor",
                Role = "Project Manager",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultPM.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultPM, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultPM, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR ********");
                Debug.WriteLine("Error Seeding Default Project Manager User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************");
                throw;
            }
            #endregion

            #region SeedDev
            //Seed Default Developer User
            var defaultDev = new BTUser
            {
                UserName = "timthetatman@twitch.tv",
                Email = "timthetatman@twitch.tv",
                FirstName = "Tim",
                LastName = "Bo",
                Role = "Developer",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultDev.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultDev, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultDev, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR ********");
                Debug.WriteLine("Error Seeding Default Developer User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************");
                throw;
            }
            #endregion

            #region SeedSubmitter
            //Seed Default Submitter User
            var defaultSub = new BTUser
            {
                UserName = "wavy@twitch.tv",
                Email = "wavy@twitch.tv",
                FirstName = "Zach",
                LastName = "Wavy",
                Role = "Submitter",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultSub.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultSub, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultSub, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR ********");
                Debug.WriteLine("Error Seeding Default Submitter User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************");
                throw;
            }
            #endregion

            #region SeedNewUser
            //Seed Default ProjectManager User
            var defaultNew = new BTUser
            {
                UserName = "xqc@twitch.tv",
                Email = "xqc@twitch.tv",
                FirstName = "Felix",
                LastName = "Something",
                Role = "User",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultNew.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultNew, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultNew, Roles.NewUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR ********");
                Debug.WriteLine("Error Seeding Default New User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************");
                throw;
            }
            #endregion
        }
    }
}
