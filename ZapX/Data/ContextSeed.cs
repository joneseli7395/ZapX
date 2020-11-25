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
        NewUser,
        Demo
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
            await roleManager.CreateAsync(new IdentityRole(Roles.Demo.ToString()));
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


            //Demo Users//
            //Can also just create new vars//
            string demoPassword = "Abc&123?";
            #region Demo SeedAdmin
            //Seed Demo Admin User
            defaultAdmin = new BTUser
            {
                UserName = "demoadmin@mailinator.com",
                Email = "demoadmin@mailinator.com",
                FirstName = "Winston",
                LastName = "Bishop",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultAdmin.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultAdmin, demoPassword);
                    await userManager.AddToRoleAsync(defaultAdmin, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultAdmin, Roles.Demo.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR ********");
                Debug.WriteLine("Error Seeding Demo Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************");
                throw;
            }
            #endregion

            #region Demo SeedProjectManager
            //Seed Demo ProjectManager User
            defaultPM = new BTUser
            {
                UserName = "demopm@mailinator.com",
                Email = "demopm@mailinator.com",
                FirstName = "Alex",
                LastName = "Smith",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultPM.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultPM, demoPassword);
                    await userManager.AddToRoleAsync(defaultPM, Roles.ProjectManager.ToString());
                    await userManager.AddToRoleAsync(defaultPM, Roles.Demo.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR ********");
                Debug.WriteLine("Error Seeding Demo Project Manager User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************");
                throw;
            }
            #endregion

            #region Demo SeedDev
            //Seed Demo Developer User
            defaultDev = new BTUser
            {
                UserName = "demodev@mailinator.com",
                Email = "demodev@mailinator.com",
                FirstName = "Tosin",
                LastName = "Abasi",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultDev.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultDev, demoPassword);
                    await userManager.AddToRoleAsync(defaultDev, Roles.Developer.ToString());
                    await userManager.AddToRoleAsync(defaultDev, Roles.Demo.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR ********");
                Debug.WriteLine("Error Seeding Demo Developer User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************");
                throw;
            }
            #endregion

            #region Demo SeedSubmitter
            //Seed Demo Submitter User
            defaultSub = new BTUser
            {
                UserName = "demosub@mailinator.com",
                Email = "demosub@mailinator.com",
                FirstName = "Nick",
                LastName = "Miller",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultSub.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultSub, demoPassword);
                    await userManager.AddToRoleAsync(defaultSub, Roles.Submitter.ToString());
                    await userManager.AddToRoleAsync(defaultSub, Roles.Demo.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR ********");
                Debug.WriteLine("Error Seeding Demo Submitter User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************");
                throw;
            }
            #endregion

            #region Demo SeedNewUser
            //Seed Demo ProjectManager User
            defaultNew = new BTUser
            {
                UserName = "demonew@mailinator.com",
                Email = "demonew@mailinator.com",
                FirstName = "Winston",
                LastName = "Schmidt",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultNew.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultNew, demoPassword);
                    await userManager.AddToRoleAsync(defaultNew, Roles.NewUser.ToString());
                    await userManager.AddToRoleAsync(defaultNew, Roles.Demo.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR ********");
                Debug.WriteLine("Error Seeding Demo New User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************");
                throw;
            }
            #endregion

        }
    }
}
