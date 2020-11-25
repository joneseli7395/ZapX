using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZapX.Models;

namespace ZapX.Services
{
    public interface IBTProjectService
    {
        public Task<bool> IsUserOnProject(string userId, int projectId);
        public Task<List<Project>> ListUserProjects(string userId);
        public Task AssignUsers(string userId, int projectId);
        public Task RemoveUsers(string userId, int projectId);
        public Task<ICollection<BTUser>> UsersOnProject(int projectId);
        public Task<ICollection<BTUser>> UsersNotOnProject(int projectId);
        public Task<ICollection<BTUser>> DevelopersOnProjectAsync(int projectId);
    }
}
