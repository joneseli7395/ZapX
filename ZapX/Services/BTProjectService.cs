using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Data;
using ZapX.Models;

namespace ZapX.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;

        //Constructor
        public BTProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        //Methods
        public async Task<bool> IsUserOnProject(string userId, int projectId)
        {
            //Project project = await _context.Projects
            //    .Include(u => u.ProjectUsers.Where(u => u.UserId == userId)).ThenInclude(u => u.User)
            //    .FirstOrDefaultAsync(u => u.Id == projectId);
            //bool result = project.ProjectUsers.Any(u => u.UserId == userId);
            //return result;

            var user = await _context.ProjectUsers.FirstOrDefaultAsync(pu => pu.UserId == userId && pu.ProjectId == projectId);
            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }

            //Mackenzie's code
            //return _context.ProjectUsers.Where(pu => pu.UserId == userId && pu.ProjectId == projectId).Any();
        }

        public async Task<ICollection<Project>> ListUserProjects(string userId)
        {
            BTUser user = await _context.Users
                .Include(p => p.ProjectUsers).ThenInclude(p => p.Project)
                .FirstOrDefaultAsync(p => p.Id == userId);
            List<Project> projects = user.ProjectUsers.SelectMany(p => (IEnumerable<Project>)p.Project).ToList();
            return projects;
        }

        public async Task AssignUsers(string userId, int projectId)
        {
            if (!await IsUserOnProject(userId, projectId))
            {
                try
                {
                    await _context.ProjectUsers.AddAsync(new ProjectUser { ProjectId = projectId, UserId = userId });
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"*** Error *** - Error adding user to project");
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public async Task RemoveUsers(string userId, int projectId)
        {
            if (await IsUserOnProject(userId, projectId))
            {
                try
                {
                    ProjectUser projectUser = await _context.ProjectUsers
                        .Where(pu => pu.UserId == userId && pu.ProjectId == projectId)
                        .FirstOrDefaultAsync();
                    //ProjectUser projectUser = new ProjectUser()
                    //{
                    //    ProjectId = projectId,
                    //    UserId = userId
                    //};
                    _context.ProjectUsers.Remove(projectUser);
                    await _context.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"*** Error *** - Error removing user from project");
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public async Task<ICollection<BTUser>> UsersOnProject(int projectId)
        {
            Project project = await _context.Projects
                .Include(p => p.ProjectUsers).ThenInclude(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == projectId);
            List<BTUser> projectusers = project.ProjectUsers.Select(p => p.User).ToList();
            return projectusers;
        }

        public async Task<ICollection<BTUser>> UsersNotOnProject(int projectId)
        {
            //Access Users in the previous method, reference projectId, if the BTUser Id property is found in projectId, return false, send results to list and return (since previous method already uses joining table, isnt necessary) 
            //return await _context.Users.Where(u => IsUserOnProject(u.Id, projectId).Result == false).ToListAsync();

            var users = await _context.Users.ToListAsync();
            ICollection<BTUser> Users = new List<BTUser>();
            foreach (var user in users)
            {
                var result = IsUserOnProject(user.Id, projectId).Result;
                if (result == false)
                {
                    Users.Add(user);
                }
            }
            return Users;
        }

    }
}
